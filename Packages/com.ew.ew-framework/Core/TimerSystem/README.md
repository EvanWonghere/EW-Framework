# TimerSystem

`Core/TimerSystem` 提供一个**全局 Timer 管理器**与带版本校验的 `TimerHandle` 句柄，用于在 Unity 中实现：

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
  - `TimerHandle Register(float duration, Action onComplete, bool isLooping=false, bool useUnscaledTime=false, Action<float> onUpdate=null)`
  - `duration` 会被 Clamp 到 \(\ge 0\)
  - `onUpdate` 每帧回调 `progress`（0~1），`duration <= 0` 时为 1
- **清理**
  - `void ClearAllTimers()`

### TimerHandle（返回的句柄）

`readonly struct`，内部记录 `Timer` 实例与注册时的 `Version`。`Timer` 完成或取消后会被回收复用，回池时 `Version + 1`，旧句柄因此失效。

- **有效性**
  - `IsValid`：Timer 尚未被回收（完成 / 取消后，要到下一次 `TimerManager.Update` 清扫或 `ClearAllTimers()` 才回收，此前仍有效且 `IsDone` / `IsCancelled` 为 true）
  - `IsActive`：`IsValid` 且未完成、未取消
- **控制**（句柄失效时为空操作，不会影响复用该实例的其它计时器）
  - `Pause()` / `Resume()` / `Cancel()`
- **状态**（句柄失效时返回默认值）
  - `Duration` / `TimeElapsed`
  - `IsPaused` / `IsCancelled` / `IsDone`
  - `IsLooping` / `UseUnscaledTime`
- `default(TimerHandle)` 表示"没有计时器"（`IsValid == false`），可代替原来的 `null`；句柄之间可用 `==` 比较。

## 行为约定与边界

- **重入安全**：`onComplete` / `onUpdate` 中可以调用 `TimerManager.Instance.ClearAllTimers()` 并立即 `Register` 新计时器：不会因列表修改导致异常（Manager 在 Update 中采用“Tick 与回收分离”的 Sweep 方式），回调返回后 `Tick` 发现自己已被回收（`Version` 变了）就不再改动这个实例，因此不会把复用该实例的新计时器标记为完成。
- **duration = 0 & isLooping = true**：会在每帧都触发一次 `onComplete`（等价于“每帧回调”）。如不希望该行为，请在业务层禁止此组合。
- **持有句柄**：可以放心长期持有旧句柄；判断"是否还在跑"请用 `IsActive`，不要用 `!IsDone`（失效句柄的 `IsDone` 也是 false）。
- **线程**：仅设计为主线程使用（Unity Update 驱动），不保证线程安全。

## 使用示例

```csharp
// 2 秒后执行（受 timeScale 影响）
TimerHandle t = TimerManager.Instance.Register(2f, () => Debug.Log("Done"));

// 暂停/恢复/取消
t.Pause();
t.Resume();
t.Cancel();

// 每 1 秒触发一次（不受 timeScale 影响）
TimerHandle loop = TimerManager.Instance.Register(
    duration: 1f,
    onComplete: () => Debug.Log("Tick"),
    isLooping: true,
    useUnscaledTime: true
);
```

## Examples

示例脚本与“场景搭建说明”在：

- `Packages/com.ew.ew-framework/Samples~/Core-TimerSystem-Examples/README.md`（通过 Package Manager 导入 Sample 后，副本位于 `Assets`）
