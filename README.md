# EW Framework

一个基于 Unity/C# 的 Gameplay Framework 模板项目，目标是同时满足：

- **可复用模板**：沉淀通用模块与规范，便于后续项目复用。
- **能力展示仓库**：通过 Demo 场景展示 Gameplay Ability 系统设计思路。

## 项目状态

当前仓库已进入 **实现推进阶段**：

- 已实现（Core）：`ObjectPool`、`SharedVariables`、`Singleton`、`SOEventBus`、`StateMachine`、`TimerSystem`（均含模块文档；可运行示例以 UPM **Samples** 形式提供，见「快速开始」）
- 规划/待推进（Modules/Demo）：Ability / Attribute / Effect / Tag / Combat 的规格落地与 Demo 场景实现

## 技术栈与环境

- Unity: `6000.3.11f1`
- Render Pipeline: URP (`com.unity.render-pipelines.universal`)
- Input: Input System (`com.unity.inputsystem`)
- Test: Unity Test Framework (`com.unity.test-framework`)

## 输入系统约定（Input System）

- 本仓库 **统一使用新版 Input System**（`com.unity.inputsystem`），包括导入 Samples 后的示例脚本。
- 示例脚本应优先使用 `UnityEngine.InputSystem.InputAction`（在 `OnEnable/OnDisable` 中 Enable/Disable，在 `OnDestroy` 中 Dispose），避免使用旧版 `Input.GetKey*` API。

## Core 模块索引（已实现）

- **ObjectPool**：基于 Unity `IObjectPool<T>` 的对象池（同步 GameObject 池 / Addressables 异步池 / 纯 C# 引用池）。
  - 同步 Spawn/Despawn 替代 Instantiate/Destroy；支持 `IPoolable` 生命周期。
  - `ReferencePoolManager` 为纯 C# 类型提供 Acquire/Release 与 `IReference<T>.OnReturnPool()` 归还回调。
  - 文档：`Packages/com.ew.ew-framework/Core/ObjectPool/README.md`；示例说明：`Packages/com.ew.ew-framework/Samples~/Core-ObjectPool-Examples/README.md`（导入 Sample 后可在 `Assets` 中查看副本）
- **SharedVariables**：基于 ScriptableObject 的共享变量（变更事件 + 可选持久化 `ISaveable`）。
  - 类型驱动的 SO 资产（Int/Float/...）用于跨对象共享读写。
  - 示例提供监听、修改、以及基于 PlayerPrefs 的可选存取。
  - 文档：`Packages/com.ew.ew-framework/Core/SharedVariables/README.md`；示例说明：`Packages/com.ew.ew-framework/Samples~/Core-SharedVariables-Examples/README.md`
- **Singleton**：单例基类集合（纯 C# `Singleton<T>`、场景级 `MonoSingleton<T>`、跨场景 `PersistentMonoSingleton<T>`）。
  - 统一懒加载与防御逻辑，便于 Core/Modules 的管理器实现。
  - 文档：`Packages/com.ew.ew-framework/Core/Singleton/README.md`
- **SOEventBus**：ScriptableObject Event Channel（原子通道 & 聚合通道 `SafeEvent`）。
  - 发布方/订阅方通过 Channel 资产解耦；支持 void 与多种载荷类型通道。
  - Editor 支持触发测试与监听列表可视化；聚合 SO 通过 `SafeEvent` 组合多事件。
  - 文档：`Packages/com.ew.ew-framework/Core/SOEventBus/README.md`；示例说明：`Packages/com.ew.ew-framework/Samples~/Core-SOEventBus-Examples/README.md`
- **StateMachine**：轻量泛型 FSM（按状态类型缓存实例，支持 ChangeState 与 Push/Pop）。
  - Context 驱动状态逻辑；`IStackState<T>` 提供 Pause/Resume 语义。
  - 文档：`Packages/com.ew.ew-framework/Core/StateMachine/README.md`；示例说明：`Packages/com.ew.ew-framework/Samples~/Core-StateMachine-Examples/README.md`
- **TimerSystem**：全局计时器管理器 + 可池化 Timer 句柄（Delay/Loop、Pause/Resume/Cancel、Scaled/Unscaled）。
  - 与 `ObjectPool` 的 `ReferencePoolManager` 集成，复用 Timer 降低 GC。
  - 文档：`Packages/com.ew.ew-framework/Core/TimerSystem/README.md`；示例说明：`Packages/com.ew.ew-framework/Samples~/Core-TimerSystem-Examples/README.md`

## UPM 包（供其它工程引用）

框架代码位于嵌入式包 [`Packages/com.ew.ew-framework/`](Packages/com.ew.ew-framework/)，包名为 `com.ew.ew-framework`。

- **对外安装（推荐）**：在其它项目的 `Packages/manifest.json` 中用 **Git URL** 引用本仓库的 **`release`** 分支（包路径 `Packages/com.ew.ew-framework`）。示例与注意事项见包内 [`README.md`](Packages/com.ew.ew-framework/README.md)。
- **`release` 分支**：用于发布可供其它工程引用的 UPM 布局；日常开发可在 `main` 上进行，发版或稳定快照时合并/推送到 `release`。

## 目录规划（目标态）

`Packages/com.ew.ew-framework/` 采用以下分层：

- `Core/`: 基础设施层（事件总线、基础契约）
- `Modules/`: 领域模块层（Ability、Attribute、Effect、Tag、Combat）
- `Utils/`: 通用工具层（辅助与扩展）
- `Demo/`: 展示层（独立 Demo 场景与演示资源）

依赖方向：

```text
Demo -> Modules -> Core
```

说明：

- `Core` 与各模块的**可运行示例**以 UPM **Samples**（`Samples~`）分发；导入后复制到项目的 `Assets`。正式 Gameplay Demo 场景建议集中放到 `Packages/com.ew.ew-framework/Demo/`（或未来单独 Demo 包），以满足依赖方向与叙事组织。

## 核心模块（规划）

- **Ability**
  - 管理能力激活、生命周期、冷却、取消与打断。
- **Attribute**
  - 管理属性值与统一变更入口。
- **Effect**
  - 表达和执行即时/持续/周期效果，支持叠层策略。
- **Tag**
  - 提供状态门控（Required/Blocked）与标签查询。
- **Combat**
  - 组织目标筛选、命中判定与结算流程。

## Demo 场景索引（规划）

- `Demo_Ability_Basics`
  - 展示能力激活闭环（触发 -> 激活 -> 冷却）。
- `Demo_Effect_Stacking`
  - 展示效果叠层策略（Refresh/Independent/Replace）。
- `Demo_Combat_Interaction`
  - 展示战斗交互、Tag 门控和属性变化联动。

## 快速开始

1. 使用 Unity `6000.3.11f1` 打开项目。
2. 打开 `Assets/Scenes/SampleScene.unity`（当前默认场景）。
3. **导入示例资源**：菜单 **Window > Package Manager**，选择 **Packages: In Project** 或左侧 **EW Framework**（`com.ew.ew-framework`），在 **Samples** 中按需点击 **Import**（将 `Samples~` 中的场景与脚本复制到 `Assets`；部分示例依赖 Input System / TextMesh Pro，请确保 `manifest.json` 中已包含与项目版本匹配的包）。
4. 导入后，在 `Assets` 中打开并 Play 对应场景（名称与包内 Sample 一致），例如：
   - `ObjectPoolDemo.unity`
   - `EventBusDemo.unity`
   - `SharedVariablesDemo.unity`
   - `StateMachineExampleDemo.unity`
   - `TimerSystemDemo.unity`
   - （可选）`AudioSystem` 模块：`AudioSystemDemo.unity`
5. 阅读以下文档理解架构与 Modules/Demo 规划：
   - `Docs/Planning/EWF_BASELINE_AUDIT.md`
   - `Docs/Planning/EWF_ARCHITECTURE_BLUEPRINT.md`
   - `Docs/Planning/EWF_MODULE_SPECS.md`
   - `Docs/Planning/EWF_DEMO_DESIGN.md`

## 扩展原则

- 优先通过接口与事件通信，避免跨模块直接访问内部状态。
- 新增能力前先补规格文档，确保语义一致再进入实现。
- Demo 只负责展示与验证，不承载核心业务规则。

## 文档索引

- 基线盘点：`Docs/Planning/EWF_BASELINE_AUDIT.md`
- 架构蓝图：`Docs/Planning/EWF_ARCHITECTURE_BLUEPRINT.md`
- 模块规格：`Docs/Planning/EWF_MODULE_SPECS.md`
- Demo 方案：`Docs/Planning/EWF_DEMO_DESIGN.md`
- 决策记录：`Docs/Planning/EWF_DOCUMENTATION_DECISIONS.md`

## 路线概览

- M1: 架构蓝图与模块规格定稿
- M2: Demo 方案定稿
- M3: README 完整化并进入实现阶段

## 详细路线图（Roadmap）

### Phase A：文档闭环（当前阶段）

- 完成并对齐以下文档：
  - `Docs/Planning/EWF_BASELINE_AUDIT.md`
  - `Docs/Planning/EWF_ARCHITECTURE_BLUEPRINT.md`
  - `Docs/Planning/EWF_MODULE_SPECS.md`
  - `Docs/Planning/EWF_DEMO_DESIGN.md`
- 明确首轮实现范围（P0/P1/P2）与验收标准。

### Phase B：首轮实现（下一阶段）

- 先落地 Ability + Attribute + Effect 最小闭环。
- 再接入 Tag/Combat，实现可解释的门控与结算流程。
- 最后按 Demo 设计文档落地三个展示场景。

### Phase C：展示增强（后续阶段）

- 增强调试可视化（状态面板、事件流、失败原因）。
- 补充测试覆盖（激活规则、叠层规则、门控规则）。
- 持续沉淀为可迁移模板（目录规范、命名规范、文档模板）。

## 不在当前范围（Out of Scope）

以下内容暂不纳入当前阶段交付：

- 网络同步（多人联机）
- **完整**存档系统（Save/Load 管线、多槽位、云同步等）；Core 层提供 `ISaveable` 与 SharedVariables 的预留持久化能力，供后续接入
- 完整 AI 行为系统
- 美术表现深度打磨（高级 VFX/SFX/动画管线）

## FAQ

### Q1: 这是可直接用于生产的框架吗？

A: 当前不是。仓库处于“Core 基建已落地、Gameplay Modules/Demo 持续推进”的阶段；更偏向模板沉淀与能力展示，待 Modules 与测试完善后再评估生产可用性。

### Q2: 为什么先做文档而不是直接写代码？

A: 该项目兼具“模板化 + 展示化”双目标。先固定模块边界与语义，可以显著降低返工并提升后续 Demo 叙事质量。

### Q3: Demo 场景什么时候可运行？

A: Core 模块的示例场景在导入 UPM Samples 后可运行（见「快速开始」第 3–4 步）。Gameplay Demo 场景将按 `Docs/Planning/EWF_DEMO_DESIGN.md` 推进落地后逐步可运行。

### Q4: 现有可复用资产是什么？

A: 当前可复用的 Core 基建包括：`ObjectPool`、`SharedVariables`、`Singleton`、`SOEventBus`、`StateMachine`、`TimerSystem`（见上方「Core 模块索引（已实现）」与各模块 README/Examples）。

### Q5: 如果我要新增模块，第一步做什么？

A: 先补模块规格（职责、数据模型、生命周期、验收标准），再进入实现。避免语义未定先编码。

### Q6: 如何在别的 Unity 项目里使用本框架？

A: 在目标工程的 `Packages/manifest.json` 的 `dependencies` 中加入对 **`com.ew.ew-framework`** 的引用：

- **Git（推荐）**：使用本仓库 **`release`** 分支与包子路径，例如  
  `"com.ew.ew-framework": "https://github.com/EvanWonghere/EW-Framework.git?path=Packages/com.ew.ew-framework#release"`  
  （可将 `#release` 换成 tag 或 commit 以锁定版本。）
- **本地 `file:`**：指向你本机克隆仓库中的 `Packages/com.ew.ew-framework` 目录。

详细步骤、依赖说明与 Samples 导入方式见 [`Packages/com.ew.ew-framework/README.md`](Packages/com.ew.ew-framework/README.md)。

## 术语表（Glossary）

- `AbilityDefinition`：能力静态配置，描述能力规则与参数。
- `AbilitySpec`：能力与角色绑定后的运行态信息。
- `Activation`：一次能力触发尝试及其执行上下文。
- `Attribute`：角色可变数值状态（如 HP/MP）。
- `Effect`：对属性或状态施加影响的规则载体。
- `Tag`：状态标签系统，用于门控与条件判断。
- `Event Channel`：基于 ScriptableObject 的事件通信通道。
- `Cooldown`：能力激活后再次可用前的等待窗口。
- `Interrupt`：能力执行过程被外部条件中止。
- `Demo Script`：一套可重复演示步骤与观察点。
