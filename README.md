# EW Framework

Unity 6 / C# 的 Gameplay 基础框架，以 UPM 包 [`com.ew.ew-framework`](Packages/com.ew.ew-framework/) 组织，可通过 Git URL 引入其它工程。每个模块附文档与可导入的示例（UPM Samples），核心逻辑有 EditMode 单元测试。

| 模块 | 层 | 做什么 | 文档 | 示例 |
|---|---|---|---|---|
| **SOEventBus** | Core | ScriptableObject 事件通道：发布方与订阅方只依赖通道资产；Inspector 扩展可手动触发事件、查看当前监听者数量 | [README](Packages/com.ew.ew-framework/Core/SOEventBus/README.md) | ✓ |
| **ObjectPool** | Core | 同步 GameObject 池、Addressables 异步池、纯 C# 引用池 | [README](Packages/com.ew.ew-framework/Core/ObjectPool/README.md) | ✓ |
| **StateMachine** | Core | 泛型 FSM：按类型缓存状态实例，支持 ChangeState 与 Push/Pop 状态栈 | [README](Packages/com.ew.ew-framework/Core/StateMachine/README.md) | ✓ |
| **TimerSystem** | Core | 全局计时器：Delay / Loop、Pause / Resume / Cancel、Scaled / Unscaled 时间，Timer 由引用池复用 | [README](Packages/com.ew.ew-framework/Core/TimerSystem/README.md) | ✓ |
| **SharedVariables** | Core | ScriptableObject 共享变量：值变更事件 + 可选的 JSON 存取（`ISaveable`） | [README](Packages/com.ew.ew-framework/Core/SharedVariables/README.md) | ✓ |
| **Singleton** | Core | 纯 C# `Singleton<T>`、场景级 `MonoSingleton<T>`、跨场景 `PersistentMonoSingleton<T>` | [README](Packages/com.ew.ew-framework/Core/Singleton/README.md) | — |
| **AudioSystem** | Modules | 命令式音频：通过事件通道发送 `AudioCommand`，由 `AudioManager` 调度 BGM 交叉淡入淡出与对象池化的 SFX | [README](Packages/com.ew.ew-framework/Modules/AudioSystem/README.md) | ✓ |

**当前状态**：上表模块已实现；Ability / Attribute / Effect / Tag / Combat 等 Gameplay 模块仍在规划，见文末 [Roadmap](#roadmap)。

## 技术栈与环境

- Unity `6000.4.7f1`（包声明最低 `6000.0`）
- Render Pipeline: URP；Input: Input System；Test: Unity Test Framework
- 包依赖：Addressables、UniTask、Newtonsoft.Json

## 快速开始

1. 使用 Unity `6000.4.7f1` 打开本仓库（默认场景 `Assets/Scenes/SampleScene.unity`）。框架以嵌入式包形式位于 `Packages/com.ew.ew-framework/`，无需额外声明。
2. **导入示例**：**Window > Package Manager** → 选择 **EW Framework**（`com.ew.ew-framework`）→ **Samples** 中按需 **Import**。示例会被复制到 `Assets`，其中部分依赖 Input System / TextMesh Pro。本仓库已自带 TMP Essential Resources；在其它工程中导入示例时，需先执行 **Window > TextMeshPro > Import TMP Essential Resources**，否则 `EventBusDemo`、`SharedVariablesDemo` 中的文字会丢失字体引用。
3. 在 `Assets` 中打开并 Play 对应场景：`ObjectPoolDemo`、`EventBusDemo`、`SharedVariablesDemo`、`StateMachineExampleDemo`、`TimerSystemDemo`、`AudioSystemDemo`。

## 在其它 Unity 工程中引用

在目标工程 `Packages/manifest.json` 的 `dependencies` 中加入（`#release` 可换成 tag 或 commit 以锁定版本）：

```json
"com.ew.ew-framework": "https://github.com/EvanWonghere/EW-Framework.git?path=Packages/com.ew.ew-framework#release"
```

- **消费项目请显式声明依赖**：Unity 对「仅通过 Git 安装的包」有时**不会**把本包 `package.json` 里的 **Git 子依赖**（如 UniTask）自动合并进消费工程，会导致 `Cysharp` / `UniTask` 找不到、进而 `EW.Framework.Editor` 未编译、Burst 报无法解析程序集。请在消费项目的 `Packages/manifest.json` 中**同时**加入与本包 `package.json` 一致的 `com.cysharp.unitask`（及 Addressables、Newtonsoft 若尚未存在），详见包内 README「消费项目建议显式声明的依赖」。
- **`release` 分支**：用于发布可供其它工程引用的 UPM 布局；日常开发在 `main` 上进行，发版或稳定快照时合并/推送到 `release`。
- 本地联调可用 `file:` 路径引用，详见包内 [`README.md`](Packages/com.ew.ew-framework/README.md)。

## 运行测试

测试位于 `Packages/com.ew.ew-framework/Tests/Editor/`（程序集 `EW.Framework.Tests.Editor`），覆盖 StateMachine、ReferencePoolManager、Timer 与 TimerHandle 失效校验、SharedVariableSO、事件内核 SafeEvent、AsyncPoolManager 并发首次加载、ClearAllTimers 回调重入，以及"每个 ScriptableObject / MonoBehaviour 类都有自己的 MonoScript、示例资产的脚本引用有效"的资产检查。均在 EditMode 下运行，需要逐帧驱动的管理器通过反射手动调用 `Update`。

- **编辑器内**：**Window > General > Test Runner** → **EditMode** → **Run All**。
- **命令行**（需先关闭打开本工程的编辑器）：

  ```bash
  <Unity 可执行文件> -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -testResults ./TestResults/editmode.xml
  ```

  macOS 上 Unity 可执行文件一般位于 `/Applications/Unity/Hub/Editor/<版本>/Unity.app/Contents/MacOS/Unity`。退出码为 0 表示全部通过，结果写入 `TestResults/editmode.xml`（NUnit XML）。
- **在消费工程中运行本包测试**：通过 Git URL 引用的包默认不参与测试，需在消费工程 `manifest.json` 顶层加入 `"testables": ["com.ew.ew-framework"]`。

## 设计取舍

### 为什么用 ScriptableObject 事件通道

- **解耦方式**：发布方与订阅方只共同引用一个通道资产（`GameEventSO` / `GameEventSO<T>`），可在 Inspector 中拖拽连线；跨 Prefab、跨场景通信不需要互相持有引用，也不需要全局单例查找。
- **可调试**：自定义 Inspector 可在编辑器里直接 Raise 事件、显示当前监听者数量，排查"事件发了没人收"一类问题。
- **事件内核 `SafeEvent`**：监听者存于 `HashSet`，重复注册不会重复回调；`Raise` 时先拷贝快照再遍历，回调中注销自己或他人不会抛出"集合已修改"异常。代价是回调顺序不保证，且每次 `Raise` 分配一个快照数组。
- **编辑器下的残留监听**：SO 资产在退出 Play 后仍然存在，因此通道在编辑器下的 `OnDisable` 中清空监听列表，避免旧监听指向已销毁的对象。
- **一类一文件**：Unity 只为一个 .cs 文件生成一个 MonoScript，多个类同处一个文件时只有一个能被资产或组件引用（实测 6000.4 绑定的是文件中的第一个类）。因此每个具体的通道 SO、共享变量 SO 和 Listener / Raiser 组件都单独成文件，并由测试检查。
- **资产数量问题**：一个事件一个资产会让资产膨胀，所以 `SafeEvent` / `SafeVoidEvent` 可单独使用，让一个业务 SO 内嵌多个事件（聚合模式，见 SOEventBus 示例）。

### 三种对象池各自适用的场景

| 池 | 适用 | 关键实现 |
|---|---|---|
| `SyncPoolManager` | 已直接引用 Prefab、需要同步拿到实例：子弹、特效、飘字、SFX 播放器 | 以 Prefab 为 key 维护池；实例上挂 `PoolItem` 记住所属池，所以 `Despawn` 只需传实例；不是从池里取出的对象会直接 `Destroy` |
| `AsyncPoolManager` | 通过 Addressables `AssetReference` 按需加载的资源 | 以资源 GUID 为 key，首次 `SpawnAsync` 时异步加载 Prefab 再建池；`ClearAllPools` 时释放 Addressables 句柄，资源才会真正卸载。对同一 GUID 的并发首次请求共享同一次加载：创建任务以 `AsyncLazy` 保存，因为 `UniTask` 在未完成时只能被 await 一次（`Preserve()` 只解决完成后的重复 await，挂起期间的第二个等待方仍会抛异常） |
| `ReferencePoolManager` | 纯 C# 对象：Timer、命令、临时数据 | 静态泛型类 `Cache<T>` 为每个类型持有一个池，类型本身就是 key，无字典查找；归还时调用 `IReference<T>.OnReturnPool()` 重置字段 |

两个 GameObject 池都基于 Unity 官方 `UnityEngine.Pool.ObjectPool<T>`，超过 `maxSize` 的归还实例直接销毁。三者都关闭了 `collectionCheck` 以省去归还时的查重开销，代价是重复归还不会报错，调用方需保证"只归还自己取出的对象，且只归还一次"。

### Timer 为什么用引用池

- **动机**：计时器在 Gameplay 中是高频、短命的对象（技能冷却、Buff、渐变、延迟回收），每次 `new` 都会产生 GC。`Timer` 实现 `IReference<Timer>`，`TimerManager.Register` 通过 `ReferencePoolManager.Acquire` 取实例，到期或取消后由 `TimerManager` 统一 `Release` 回池。
- **更新顺序**：新注册的 Timer 先进入待添加列表，在下一次 `TimerManager.Update` 开始时才并入活动列表；每帧先逆序 Tick 全部 Timer，再单独一趟清扫回收已完成 / 已取消的 Timer。回调里注册新计时器不会修改正在遍历的列表。
- **句柄代数校验**：池化意味着 Timer 完成或取消后，同一个实例会分配给下一次 `Register`。如果把实例本身交给调用方，仍持有旧引用的一方再调用 `Cancel()` 就会取消掉别人的计时器。因此 `Register` 返回 `readonly struct TimerHandle`，记录实例和注册时的 `Version`；Timer 回池时 `Version + 1`，旧句柄随之失效，`Cancel` / `Pause` / `Resume` 变为空操作。调用方可以放心长期持有句柄，用 `IsActive` 判断计时器是否仍在运行。代价是这是一次破坏性 API 变更（`Register` 原先返回 `Timer`），且失效句柄读不到计时器的最终状态。

## 目录结构

```text
Packages/com.ew.ew-framework/
  Core/       基础设施：ObjectPool、SharedVariables、Singleton、SOEventBus、StateMachine、TimerSystem
  Modules/    领域模块：AudioSystem（Gameplay 模块规划中）
  Samples~/   各模块的可导入示例（场景 + 脚本）
  Tests/      EditMode 单元测试
Docs/Planning/  架构蓝图、模块规格、Demo 设计等规划文档
```

依赖方向为 `Demo -> Modules -> Core`，Core 不依赖 Modules。

## 输入系统约定（Input System）

- 本仓库**统一使用新版 Input System**（`com.unity.inputsystem`），包括导入 Samples 后的示例脚本。
- 示例脚本应优先使用 `UnityEngine.InputSystem.InputAction`（在 `OnEnable/OnDisable` 中 Enable/Disable，在 `OnDestroy` 中 Dispose），避免使用旧版 `Input.GetKey*` API。

## Roadmap

以下内容**尚未实现**，规格文档见 `Docs/Planning/`：

- **PlayMode 测试**：覆盖 `SyncPoolManager`、`TimerManager` 等需要逐帧驱动的管理器。
- **Gameplay Ability 展示**：按 `EWF_MODULE_SPECS.md` 实现以下模块，并按 `EWF_DEMO_DESIGN.md` 落地 Demo 场景。
  - **Ability**：能力激活、生命周期、冷却、取消与打断。
  - **Attribute**：属性值与统一变更入口。
  - **Effect**：即时 / 持续 / 周期效果，支持叠层策略。
  - **Tag**：状态门控（Required / Blocked）与标签查询。
  - **Combat**：目标筛选、命中判定与结算流程。
  - Demo 场景：`Demo_Ability_Basics`（触发 → 激活 → 冷却）、`Demo_Effect_Stacking`（Refresh / Independent / Replace 叠层）、`Demo_Combat_Interaction`（战斗交互、Tag 门控与属性联动）。
- **调试可视化**：状态面板、事件流、失败原因。

不在当前范围：网络同步、完整存档管线（Core 仅提供 `ISaveable` 与 SharedVariables 的持久化接口）、完整 AI 行为系统、美术表现深度打磨。

<details>
<summary>规划术语表</summary>

- `AbilityDefinition`：能力静态配置，描述能力规则与参数。
- `AbilitySpec`：能力与角色绑定后的运行态信息。
- `Activation`：一次能力触发尝试及其执行上下文。
- `Attribute`：角色可变数值状态（如 HP/MP）。
- `Effect`：对属性或状态施加影响的规则载体。
- `Tag`：状态标签系统，用于门控与条件判断。
- `Event Channel`：基于 ScriptableObject 的事件通信通道。
- `Cooldown`：能力激活后再次可用前的等待窗口。
- `Interrupt`：能力执行过程被外部条件中止。

</details>

## 文档索引

- 基线盘点：`Docs/Planning/EWF_BASELINE_AUDIT.md`
- 架构蓝图：`Docs/Planning/EWF_ARCHITECTURE_BLUEPRINT.md`
- 模块规格：`Docs/Planning/EWF_MODULE_SPECS.md`
- Demo 方案：`Docs/Planning/EWF_DEMO_DESIGN.md`
- 决策记录：`Docs/Planning/EWF_DOCUMENTATION_DECISIONS.md`

## 许可证

[MIT](LICENSE)
