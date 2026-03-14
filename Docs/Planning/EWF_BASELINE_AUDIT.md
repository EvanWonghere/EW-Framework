# EW Framework 现状盘点（Baseline Audit）

## 1) 项目定位与阶段结论

- 定位：该仓库目标是 `Unity + C#` 的可复用 Gameplay Framework 模板，并兼具 Gameplay Ability Showcase。
- 当前阶段：处于早期基建阶段，已有 `SO EventBus`，尚未形成 Ability/Effect/Tag/Combat 的系统化模块。
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
  - `Core/`：已存在 `SOEventBus`
  - `Modules/`：当前空（或仅占位）
  - `Utils/`：当前空（或仅占位）
- 场景现状：
  - 仅发现 `Assets/Scenes/SampleScene.unity`
  - 暂无 `Demo`/`Showcase` 场景体系
- 文档现状：
  - 项目根目录尚无正式 `README.md`
  - 当前缺少面向外部读者与开发者 onboarding 的主文档

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

## 4.2 设计质量简评

- 优点：
  - 事件式通信已具备，适合后续模块解耦。
  - 类型化 channel 命名明确，便于团队协作与资产管理。
- 约束：
  - 目前仍是“单一基建点”，尚未形成 gameplay 领域模型（Ability/Effect/Tag）。

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
- README 与术语体系（对外解释成本）

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
