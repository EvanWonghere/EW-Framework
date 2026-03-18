# StateMachine Examples

本目录提供一套“可直接挂到 GameObject 上运行”的最小示例，用于验证 `Core/StateMachine` 的 API 语义与最佳实践使用方式。

## 脚本与用法

| 脚本 | 作用 | 操作 |
|------|------|------|
| **StateMachineExampleDriver** | 状态机宿主（Idle/Patrol/Chase + Push/Pop Pause） | 拖入 `target` 引用后运行；按 **Esc** 进入/退出 Pause（Input System）。 |
| **StateMachineExampleTarget** | 可控目标（供 Chase 追逐） | **WASD** 或 **方向键** 移动（Input System）。 |
| **StateMachineExampleHud** | Game 视图 HUD | 显示当前状态、最近切换原因/时间、与目标距离。 |

## 示例场景构思：`Demo_StateMachine_Basics`

**目标**：用最少的美术/资源成本，把以下观察点一次性讲清楚：

- **基础切换**：`Idle -> Patrol -> Chase` 的切换时机与 Enter/Exit 调用顺序
- **堆栈状态**：按 `Esc` Push/Pop `PauseState`，并展示 `OnPause/OnResume` 语义（被压栈的状态会收到回调）
- **可视化**：HUD 直接看到当前状态、最近切换原因/时间、与目标距离

### 组成

- `StateMachineExampleDriver`（挂在一个“追逐者”物体上）
- `StateMachineExampleTarget`（挂在一个“目标”物体上）
- `StateMachineExampleHud`（挂在任意物体上，引用 driver）

### 演示步骤（脚本化）

1. Play 后默认进入 `Idle`，等待 1 秒自动切 `Patrol`
2. `Patrol` 会在场景中巡逻（圆周或往返），当目标进入半径 \(R\) 时切 `Chase`
3. `Chase` 会追逐目标；目标离开半径 \(R_{lose}\) 或追逐超时，回到 `Patrol`
4. 任意时刻按 `Esc`：
   - Push `PauseMenu`，暂停追逐/巡逻计时（展示 `OnPause`）
   - 再按 `Esc` Pop 回到上一状态并继续（展示 `OnResume`）

## 推荐场景搭建（不提供/不修改 .unity）

参考本仓库其它 Core 模块的 Examples，本示例建议你在任意测试场景中手动搭建：

1. 创建空物体 `SM_Driver`，挂 `StateMachineExampleDriver`
   - 在 Inspector 将 `target` 指向步骤 2 的 `SM_Target`
2. 创建空物体 `SM_Target`，挂 `StateMachineExampleTarget`
3. 创建空物体 `SM_HUD`，挂 `StateMachineExampleHud`
   - 在 Inspector 将 `driver` 指向步骤 1 的 `SM_Driver`

运行后：

- **WASD/方向键**：移动目标
- **Esc**：Push/Pop Pause
- HUD：显示当前状态、最近切换原因/时间、与目标距离

## 注意

- 示例脚本放在 `Core` 下仅用于演示该子模块；正式 Demo 场景建议放到 `Assets/EW_Framework/Demo/`，以满足 `Demo -> Modules -> Core` 的依赖方向。

