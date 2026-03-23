# StateMachine

本模块提供一个**轻量、可缓存状态实例**的泛型有限状态机（FSM），用于在 Unity/C# 项目中管理对象的离散行为阶段（如 Idle/Patrol/Chase、UI 面板开关、交互流程等）。

## 目录结构

- `Base/`
  - `IState<T>`：状态契约（Enter/Update/Exit）
  - `IStackState<T>`：可选扩展契约（OnPause/OnResume），用于 Push/Pop 场景
  - `StateMachine<T>`：状态机实现（按类型缓存状态实例）
- （示例）`Samples~/Core-StateMachine-Examples/`：演示用例（Input System + HUD + 追逐示例），不承载业务逻辑

## 设计要点

- **Context 驱动**：`T` 为状态机宿主（环境/上下文），状态方法均以 `context` 作为唯一外部依赖入口。
- **按类型缓存**：`StateMachine<T>` 会按 `TState` 的 `Type` 缓存状态实例（避免频繁 new、便于状态持有少量运行时数据）。
- **强语义切换**
  - `Start<TState>()`：启动并进入初始状态（重复调用会给 Warning 并切换到目标状态）
  - `ChangeState<TState>()`：Exit 当前 -> Enter 新状态
  - `PushState<TState>()` / `PopState()`：堆栈式覆盖状态（适合 Pause/Modal 等）

## 基础用法

1. 为宿主类型实现若干状态类：
   - `class IdleState : IState<MyController> { ... }`
2. 在宿主中持有并驱动状态机：
   - `fsm = new StateMachine<MyController>(this);`
   - `fsm.Start<IdleState>();`
   - 在 `Update()` 中调用 `fsm.Update();`
3. 在状态内部根据条件切换：
   - `context.ChangeState<ChaseState>();`（建议由宿主暴露强类型包装方法，避免状态直接持有 fsm 引用）

## Stack 扩展（Push/Pop）

当你使用 `PushState/PopState`（例如 Gameplay -> PauseMenu -> 回到 Gameplay）时：

- 被覆盖的状态不会触发 `Exit`（避免“暂停”被误当成结束）
- 若该状态实现了 `IStackState<T>`：
  - `PushState` 时会调用 `OnPause(context)`
  - `PopState` 恢复时会调用 `OnResume(context)`

这能把“暂停/恢复”的语义从 Enter/Exit 中分离出来，减少状态逻辑歧义。

## 最佳实践建议

- **状态应尽量无外部副作用**：把输入、Unity 引用查找、日志节流等集中在宿主/外层，状态更关注纯粹的“条件与转移”。
- **避免在状态中做昂贵查找**：例如频繁 `GetComponent`、`Find`、或每帧分配对象。
- **切换原因可视化**：建议在宿主记录“最近一次切换原因”（Examples 已演示），便于调试与 Demo 叙事。
- **Clear 的时机**：当宿主销毁/禁用且不再复用 FSM 时调用 `Clear()` 释放缓存与堆栈引用。

## Examples

见包内 [`Samples~/Core-StateMachine-Examples/README.md`](../../Samples~/Core-StateMachine-Examples/README.md)（或通过 Package Manager 导入 Sample 后在 `Assets` 中打开）：

- 使用 **新版 Input System**（`InputAction`）进行输入
- `Idle -> Patrol -> Chase` 基本切换 + `Esc` Push/Pop Pause
- HUD 展示当前状态、最近切换原因与关键数值

