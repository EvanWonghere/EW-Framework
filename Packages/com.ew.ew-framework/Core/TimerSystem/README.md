# TimerSystem

`Core/TimerSystem` 提供一个**全局 Timer 管理器**与可池化的 `Timer` 句柄，用于在 Unity 中实现：

- 一次性延迟执行（Delay）
- 循环/定时执行（Loop / Interval）
- 暂停/恢复/取消（Pause/Resume/Cancel）
- Scaled / Unscaled 时间（受 `Time.timeScale` 影响或不受影响）

本模块与 `Core/ObjectPool` 的 `ReferencePoolManager` 集成，用于复用 `Timer` 实例以降低 GC。

## 依赖

- `Core/Singleton`：`TimerManager : PersistentMonoSingleton<TimerManager>`（跨场景常驻）
- `Core/ObjectPool`：`ReferencePoolManager`（`Timer : IReference<Timer>`）

## 核心 API

### TimerManager

- **注册**
  - `Timer Register(float duration, Action onComplete, bool isLooping=false, bool useUnscaledTime=false, Action<float> onUpdate=null)`
  - `duration` 会被 Clamp 到 \(\ge 0\)
  - `onUpdate` 每帧回调 `progress`（0~1），`duration <= 0` 时为 1
- **清理**
  - `void ClearAllTimers()`

### Timer（返回的句柄）

- **控制**
  - `Pause()` / `Resume()` / `Cancel()`
- **状态**
  - `Duration` / `TimeElapsed`
  - `IsPaused` / `IsCancelled` / `IsDone`
  - `IsLooping` / `UseUnscaledTime`

## 行为约定与边界

- **重入安全**：`onComplete` 中可以调用 `TimerManager.Instance.ClearAllTimers()`，不会因列表修改导致异常（Manager 在 Update 中采用“Tick 与回收分离”的 Sweep 方式）。
- **duration = 0 & isLooping = true**：会在每帧都触发一次 `onComplete`（等价于“每帧回调”）。如不希望该行为，请在业务层禁止此组合。
- **线程**：仅设计为主线程使用（Unity Update 驱动），不保证线程安全。

## 使用示例

```csharp
// 2 秒后执行（受 timeScale 影响）
Timer t = TimerManager.Instance.Register(2f, () => Debug.Log("Done"));

// 暂停/恢复/取消
t.Pause();
t.Resume();
t.Cancel();

// 每 1 秒触发一次（不受 timeScale 影响）
Timer loop = TimerManager.Instance.Register(
    duration: 1f,
    onComplete: () => Debug.Log("Tick"),
    isLooping: true,
    useUnscaledTime: true
);
```

## Examples

示例脚本与“场景搭建说明”在：

- `Packages/com.ew.ew-framework/Samples~/Core-TimerSystem-Examples/README.md`（通过 Package Manager 导入 Sample 后，副本位于 `Assets`）
