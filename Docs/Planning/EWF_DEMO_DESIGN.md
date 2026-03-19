# EW Framework Demo 场景设计文档

## 1) 设计原则

- 每个 Demo 只讲一个核心价值，避免“全塞一场景”。
- 演示路径固定、可重复、可观测。
- 每个 Demo 都有明确成功判定，便于后续验收。

## 2) Demo 总览

| Demo 场景 | 核心目标 | 重点模块 |
|---|---|---|
| `Demo_Ability_Basics` | 展示能力激活闭环（触发、执行、冷却） | Ability + Tag + EventBus |
| `Demo_Effect_Stacking` | 展示效果叠层与持续行为 | Effect + Attribute + EventBus |
| `Demo_Combat_Interaction` | 展示战斗交互与门控协作 | Combat + Ability + Tag + Attribute |

## 2.1 Core Examples（已可运行）

除上述“Gameplay Demo（规划）”外，当前仓库的 Core 子模块已提供可直接运行的 Examples 场景，用于验证子模块 API 与最佳实践（不承载正式 Demo 叙事与业务规则）：

- `Assets/EW_Framework/Core/ObjectPool/Examples/ObjectPoolDemo.unity`
- `Assets/EW_Framework/Core/SOEventBus/Examples/EventBusDemo.unity`
- `Assets/EW_Framework/Core/SharedVariables/Examples/SharedVariablesDemo.unity`
- `Assets/EW_Framework/Core/StateMachine/Examples/StateMachineExampleDemo.unity`
- `Assets/EW_Framework/Core/TimerSystem/Examples/TimerSystemDemo.unity`

定位说明：

- Examples 仅用于“子模块自证”（API 语义、边界行为、最佳实践），正式 Gameplay Demo 仍以 `Demo_<Domain>_<Topic>` 命名并建议放到 `Assets/EW_Framework/Demo/`，以满足 `Demo -> Modules -> Core` 的依赖方向与叙事组织。

---

## 3) Demo_Ability_Basics

## 3.1 场景目标

- 让观众在 1 分钟内理解：能力有激活条件、状态变化和冷却。

## 3.2 演示前置

- 玩家角色具备至少 1 个主动能力。
- UI 有 3 个基础区块：能力状态、冷却状态、事件日志（文本即可）。

## 3.3 操作步骤

1. 空状态下触发能力一次，观察进入 Active。
2. 立即再次触发同能力，观察被冷却阻止。
3. 冷却结束后再次触发，观察可重新激活。
4. 添加一个阻断 Tag（例如眩晕），再次触发并验证失败。

## 3.4 观察点

- `CanActivate` 结果（成功/失败原因）是否可见。
- 状态流是否按 `Ready -> Active -> Cooldown -> Ready`。
- Tag 阻断是否生效且可解释。

## 3.5 成功判定

- 同一能力在冷却中不可重复激活。
- 阻断状态下返回明确失败原因。
- 事件日志完整记录每次状态变化。

---

## 4) Demo_Effect_Stacking

## 4.1 场景目标

- 让观众理解 Effect 在 `Refresh/Independent/Replace` 三种策略下的差异。

## 4.2 演示前置

- 至少一个可重复施加的 Buff 和一个 Debuff。
- UI 显示当前层数、剩余时间、目标属性变化。

## 4.3 操作步骤

1. 施加 `Refresh` 类型效果两次，观察层数与持续时间变化。
2. 切换到 `Independent`，连续施加两次，观察多实例并存。
3. 切换到 `Replace`，新效果覆盖旧效果并刷新表现。
4. 观察效果到期移除后属性回到预期区间。

## 4.4 观察点

- 叠层策略是否与文档语义一致。
- 周期 Tick 是否按预期触发。
- 过期或移除时是否产生可观测事件。

## 4.5 成功判定

- 三种策略结果差异清晰且可复现。
- 属性变化曲线与日志一致，无幽灵状态残留。

---

## 5) Demo_Combat_Interaction

## 5.1 场景目标

- 展示能力、战斗、属性、Tag 的联动闭环。

## 5.2 演示前置

- 至少 1 名玩家与 1 名敌人对象。
- 可视化显示：命中结果、伤害载荷、目标标签与关键属性。

## 5.3 操作步骤

1. 对目标释放技能，验证命中与伤害结算。
2. 给目标添加防御/免疫 Tag，再次释放并观察结果差异。
3. 触发一个带控制效果的技能，观察目标状态变化与后续门控。
4. 清除控制后再次攻击，验证状态恢复与流程恢复。

## 5.4 观察点

- 命中判定与 Tag 门控是否一致。
- 伤害载荷流向是否可追踪（来源 -> 结算 -> 属性变化）。
- 控制状态是否影响后续能力触发与战斗行为。

## 5.5 成功判定

- 战斗流程可完整走通且可解释。
- 任一关键失败（未命中/被门控）都有明确原因显示。

---

## 6) 最小调试 UI 需求（用于后续实现验收）

- Ability 面板：当前状态、剩余冷却、最后失败原因。
- Effect 面板：活跃效果列表、层数、剩余时间、Tick 计数。
- Tag 面板：当前标签集合与最近变更。
- Combat 面板：最近命中结果与伤害载荷摘要。
- Event 流：最近 20 条关键事件（时间戳 + 事件名 + 关键字段）。

## 7) 演示时长与节奏建议

- Demo 1：1.5 分钟
- Demo 2：2 分钟
- Demo 3：2.5 分钟
- 总时长：6 分钟内，适合面试/展示/录屏。

## 8) 验收清单（场景级）

- 每个 Demo 是否有可重复操作脚本？
- 每个关键结论是否可被 UI 或日志观测？
- 每个失败分支是否有可解释原因？
- 演示者是否可在 1 次 rehearsal 内稳定完成？
