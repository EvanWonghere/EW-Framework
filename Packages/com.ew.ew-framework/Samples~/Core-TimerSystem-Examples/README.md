# TimerSystem Examples

本目录提供一套“无需额外资源、可直接挂到 GameObject 上运行”的最小示例，用于验证 `Core/TimerSystem` 的 API 语义与最佳实践使用方式。

> 注意：按仓库约定，这里**不提供/不提交** `.unity` 场景文件；仅提供“场景构思 + 推荐搭建步骤 + 示例脚本”。

## 示例场景构思：`Demo_TimerSystem_Basics`

**目标**：用最少成本把以下观察点讲清楚：

- **一次性延迟**：到时触发 onComplete
- **循环计时器**：按固定间隔触发
- **暂停/恢复/取消**：对单个 timer 句柄生效
- **Scaled vs Unscaled**：`Time.timeScale = 0` 时，scaled timer 停止而 unscaled timer 继续
- **清空所有**：`ClearAllTimers()` 在回调中调用也不会引发异常

## 脚本与用法

| 脚本 | 作用 | 操作 |
|------|------|------|
| **TimerSystemExampleDriver** | 注册/控制多个 Timer，并记录事件日志 | 挂在场景任意物体上运行；按键触发创建/暂停/恢复/取消/清空/切换 timeScale。 |
| **TimerSystemExampleHud** | 运行时 HUD，可视化当前 timers 状态与日志 | 挂在任意物体上，Inspector 引用 Driver。 |

## 推荐场景搭建（不提供/不修改 .unity）

1. 在任意测试场景中新建空物体 `TimerSystemDemo_Driver`，挂 `TimerSystemExampleDriver`。
2. 新建空物体 `TimerSystemDemo_HUD`，挂 `TimerSystemExampleHud`，并在 Inspector 中把 `driver` 指向第 1 步物体上的 Driver。
3. Play 后在 Game 视图左上角查看 HUD 与按键提示，按下对应按键观察 Console/HUD 变化。

## 键位说明

- `1`：注册 **Scaled** 一次性延迟（默认 2 秒）
- `2`：注册 **Unscaled** 一次性延迟（默认 2 秒）
- `3`：注册 **Scaled** 循环计时器（默认间隔 1 秒）
- `P`：暂停/恢复“最近注册的 timer”
- `C`：取消“最近注册的 timer”
- `R`：清空所有 timers（调用 `TimerManager.Instance.ClearAllTimers()`）
- `T`：切换 `Time.timeScale`（1 ↔ 0），用于观察 scaled/unscaled 差异

