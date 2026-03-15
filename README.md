# EW Framework

一个基于 Unity/C# 的 Gameplay Framework 模板项目，目标是同时满足：

- **可复用模板**：沉淀通用模块与规范，便于后续项目复用。
- **能力展示仓库**：通过 Demo 场景展示 Gameplay Ability 系统设计思路。

## 项目状态

当前仓库处于 **文档驱动规划阶段**：

- 已有：`SO EventBus` 基础能力（`Assets/EW_Framework/Core/SOEventBus/`）、`SharedVariables` 共享变量与可选持久化（`Assets/EW_Framework/Core/SharedVariables/`，见模块内 README）
- 规划中：Ability、Attribute、Effect、Tag、Combat 模块规格与 Demo 设计
- 待落地：模块代码实现与 Demo 场景实现

## 技术栈与环境

- Unity: `6000.3.11f1`
- Render Pipeline: URP (`com.unity.render-pipelines.universal`)
- Input: Input System (`com.unity.inputsystem`)
- Test: Unity Test Framework (`com.unity.test-framework`)

## 目录规划（目标态）

`Assets/EW_Framework/` 采用以下分层：

- `Core/`: 基础设施层（事件总线、基础契约）
- `Modules/`: 领域模块层（Ability、Attribute、Effect、Tag、Combat）
- `Utils/`: 通用工具层（辅助与扩展）
- `Demo/`: 展示层（独立 Demo 场景与演示资源）

依赖方向：

```text
Demo -> Modules -> Core
```

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
3. 阅读以下文档理解规划：
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

A: 当前不是。项目处于“文档规划完成、实现待推进”状态，目标是先确保架构语义稳定，再进入编码。

### Q2: 为什么先做文档而不是直接写代码？

A: 该项目兼具“模板化 + 展示化”双目标。先固定模块边界与语义，可以显著降低返工并提升后续 Demo 叙事质量。

### Q3: Demo 场景什么时候可运行？

A: 目前已完成 Demo 设计文档，后续按 `Docs/Planning/EWF_DEMO_DESIGN.md` 进入场景实现后可运行。

### Q4: 现有可复用资产是什么？

A: 目前可复用的基建包括：`SO EventBus`（`Assets/EW_Framework/Core/SOEventBus/`）、`SharedVariables`（`Assets/EW_Framework/Core/SharedVariables/`，含共享变量与可选 `ISaveable` 持久化）。

### Q5: 如果我要新增模块，第一步做什么？

A: 先补模块规格（职责、数据模型、生命周期、验收标准），再进入实现。避免语义未定先编码。

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
