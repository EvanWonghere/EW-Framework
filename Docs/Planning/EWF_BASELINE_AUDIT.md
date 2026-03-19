# EW Framework 现状盘点（Baseline Audit）

## 1) 项目定位与阶段结论

- 定位：该仓库目标是 `Unity + C#` 的可复用 Gameplay Framework 模板，并兼具 Gameplay Ability Showcase。
- 当前阶段：处于实现推进阶段，**Core 基建已落地**（`ObjectPool` / `SharedVariables` / `Singleton` / `SOEventBus` / `StateMachine` / `TimerSystem`），尚未形成 Ability/Effect/Tag/Combat 的系统化 Modules 与正式 Demo 场景体系。
- 盘点结论：适合采用“文档先行 -> 模块落地 -> Demo 场景化展示”的推进方式。

## 2) 环境与依赖基线

- Unity 版本：`6000.3.11f1`（来源：`ProjectSettings/ProjectVersion.txt`）。
- 渲染与输入：
  - `com.unity.render-pipelines.universal: 17.3.0`
  - `com.unity.inputsystem: 1.19.0`
- 测试与时间线：
  - `com.unity.test-framework: 1.6.0`
  - `com.unity.timeline: 1.8.11`
- 其他关键包：`com.unity.ai.navigation`、`com.unity.visualscripting` 等已可作为后续扩展基础。

## 3) 目录与资产现状

- 业务核心目录：`Assets/EW_Framework/`
  - `Core/`：已落地多项可复用基建：
    - `Assets/EW_Framework/Core/ObjectPool/`
    - `Assets/EW_Framework/Core/SharedVariables/`
    - `Assets/EW_Framework/Core/Singleton/`
    - `Assets/EW_Framework/Core/SOEventBus/`
    - `Assets/EW_Framework/Core/StateMachine/`
    - `Assets/EW_Framework/Core/TimerSystem/`
  - `Modules/`：尚未落地稳定领域模块（Ability/Attribute/Effect/Tag/Combat 仍以规格为主）
  - `Utils/`：按需补充，当前不作为阶段性重点
- 场景现状：
  - `Assets/Scenes/SampleScene.unity` 作为默认场景仍存在
  - Core 子模块均提供可运行的 Examples 场景（用于验证 API 与最佳实践），但**尚无**按 `Demo_<Domain>_<Topic>` 组织的正式 Gameplay Demo 场景体系
- 文档现状：
  - 项目根目录 `README.md` 已作为对外入口，区分“已实现 Core”与“规划 Modules/Demo”
  - `Docs/Planning/` 作为“当前事实 + 当前规划”的集合持续维护

## 4) 已有能力（可复用资产）

## 4.1 SO EventBus 基础

- 参数化与无参事件通道：
  - `GameEventSO<T>`（泛型）
  - `GameEventSO`（Void）
- 监听与触发组件：
  - Listener / Raiser 基础类
  - 常用基础类型的 channel/listener/raiser 快速类
- 编辑器辅助：
  - 运行时触发按钮与监听数显示

## 4.2 ObjectPool（同步/异步对象池 + 引用池）

- 基于 Unity `IObjectPool<T>` 的 GameObject 池（同步）与 Addressables 异步池（以 `AssetReference` 为 key）。
- 提供纯 C# `ReferencePoolManager`（Acquire/Release），配合 `IReference<T>.OnReturnPool()` 归还回调以清理字段引用。
- 入口与说明：
  - `Assets/EW_Framework/Core/ObjectPool/README.md`
  - `Assets/EW_Framework/Core/ObjectPool/Examples/README.md`

## 4.3 SharedVariables（共享变量 + 可选持久化）

- ScriptableObject 驱动的共享变量（类型化 SO 资产），提供变更事件与可选持久化接口 `ISaveable`。
- Examples 提供监听、修改、以及基于 PlayerPrefs 的简单 Save/Load 演示（用于验证语义，不绑定最终存档方案）。
- 入口与说明：
  - `Assets/EW_Framework/Core/SharedVariables/README.md`
  - `Assets/EW_Framework/Core/SharedVariables/Examples/README.md`

## 4.4 Singleton（纯 C# / MonoBehaviour 单例基类）

- `Singleton<T>`（纯 C# 懒加载）+ `MonoSingleton<T>`（场景级）+ `PersistentMonoSingleton<T>`（跨场景常驻）。
- 统一基础设施管理器的单例写法（如对象池、计时器管理器）。
- 入口与说明：
  - `Assets/EW_Framework/Core/Singleton/README.md`

## 4.5 StateMachine（轻量泛型 FSM）

- 泛型 FSM，按状态类型缓存实例；支持 `ChangeState` 与堆栈式 `Push/Pop`（Pause/Resume 语义）。
- Examples 提供可运行场景，演示 Input System + HUD 可视化与切换原因记录。
- 入口与说明：
  - `Assets/EW_Framework/Core/StateMachine/README.md`
  - `Assets/EW_Framework/Core/StateMachine/Examples/README.md`

## 4.6 TimerSystem（全局 Timer 管理）

- `TimerManager` + `Timer` 句柄：Delay/Loop、Pause/Resume/Cancel、Scaled/Unscaled。
- 与 `ObjectPool` 的 `ReferencePoolManager` 集成，复用 Timer 以降低 GC。
- Examples 提供可运行场景，演示 timeScale 影响与 ClearAll 的安全性。
- 入口与说明：
  - `Assets/EW_Framework/Core/TimerSystem/README.md`
  - `Assets/EW_Framework/Core/TimerSystem/Examples/README.md`

## 4.7 设计质量简评

- 优点：
  - 事件式通信与基础设施（对象池/计时器/共享变量/FSM）已具备，适合后续模块解耦与可观测性建设。
  - 类型化资产与统一基类降低样板代码，便于团队协作与资产管理。
- 约束：
  - 目前仍以 Core 基建为主，尚未形成 gameplay 领域模型（Ability/Effect/Tag/Combat/Attribute）的运行时闭环。

## 5) 缺口清单（按优先级）

## P0 缺口（模板与 showcase 必需）

- Ability Runtime 概念与规格（Definition/Spec/Instance/Activation）
- Attribute 与数值变更模型（Base/Current/Modifier）
- Gameplay Effect 生命周期与叠层语义

## P1 缺口（系统可组合性）

- Gameplay Tag 规则（Owned/Required/Blocked）
- 输入到能力调度的策略规范（触发、打断、优先级）

## P2 缺口（展示与传播）

- Demo 场景剧本化设计（可重复演示）
- 将规划文档与实现现状持续对齐（避免读者误判成熟度）

## 6) 风险与技术债

- 架构风险：
  - 若不先定义模块边界，后续实现可能形成强耦合与循环依赖。
- 展示风险：
  - 若先写功能再补文档，showcase 叙事路径会不清晰，影响复用与传播。
- 维护风险：
  - 缺少统一命名/目录规范会导致资产命名和脚本职责漂移。
- 测试风险（未来）：
  - 若行为语义（堆叠、优先级、打断）未文档化，测试标准难以统一。

## 7) 建议优先动作（文档向）

1. 完成架构蓝图：明确 `Core / Modules / Utils / Demo` 的依赖方向。
2. 完成五大模块规格：把“能力语义”固定为团队共同语言。
3. 完成 Demo 设计稿：先定义“展示什么、如何验证成功”。
4. 完成 README v1：建立外部理解入口，降低 onboarding 成本。

## 8) 验收标准（针对本盘点）

- 能回答“现在有什么、缺什么、先做什么”三个问题。
- 能直接作为后续蓝图文档与 README 的输入。
- 能让新加入的开发者在 10 分钟内理解项目当前状态。
