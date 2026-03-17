# ObjectPool

基于 Unity 官方 `IObjectPool<T>` 的对象池模块，提供同步与异步（Addressables）两种管理器，并约定统一的 `IPoolable` 生命周期接口。

## 职责

- 通过对象池替代频繁的 `Instantiate/Destroy`，减少 GC 与运行时开销。
- 为同步场景与 Addressables 异步加载场景统一提供 Spawn/Despawn 入口。
- 为业务对象提供清晰的生命周期回调（`OnSpawn` / `OnDespawn`）。

## 结构

- `Base/IPoolable.cs`
  - 可池化对象接口：
    - `void OnSpawn()`：从池中取出时调用，用于重置状态、播放特效等。
    - `void OnDespawn()`：回收到池中前调用，用于清理状态、停止特效等。
- `Manager/SyncPoolManager.cs`
  - 同步对象池管理器：`SyncPoolManager : PersistentMonoSingleton<SyncPoolManager>`.
  - 以 **Prefab → Pool** 与 **Instance → Prefab** 两张字典管理对象：
    - `Spawn(prefab, position, rotation, parent)`：替代 `Instantiate`，自动从对应池中获取或创建实例。
    - `Despawn(instance)`：替代 `Destroy`，将实例归还池中，若非池内对象则直接销毁。
    - `ClearAllPools()`：销毁所有池中对象并清理父节点，适合场景切换或内存紧张时调用。
- `Manager/AsyncPoolManager.cs`
  - 异步对象池管理器：`AsyncPoolManager : PersistentMonoSingleton<AsyncPoolManager>`.
  - 以 Addressables `AssetReference` 的 GUID 为 key：
    - `SpawnAsync(assetRef, position, rotation, parent)`：先异步加载 prefab，再通过对象池复用实例；内部对并发初始化同一 GUID 做合并与错误处理。
    - `Despawn(instance)`：与同步池语义一致。
    - `ClearAllPools()`：释放所有池、Addressables 句柄与父节点。
- `Utilities/AutoDespawn.cs`
  - 挂在实例上的工具组件：在 `OnEnable` 启动协程，等待一定时间后调用 `SyncPoolManager.Instance.Despawn`，适合特效、飘字、一次性音效等。
- `Examples/`
  - 示例脚本与场景使用见 [Examples/README.md](Examples/README.md)。

## 使用建议

- **简单同步场景**（场景内直接引用 prefab）：
  - 使用 `SyncPoolManager`：
    - `Spawn(prefab, pos, rot, parent)` 替代 `Instantiate`.
    - `Despawn(instance)` 替代 `Destroy`.
    - 对需要生命周期感知的对象实现 `IPoolable`，在回调中重置状态。
- **Addressables 驱动的异步场景**：
  - 使用 `AsyncPoolManager`：
    - 通过 `AssetReference` 调用 `await SpawnAsync(assetRef, pos, rot, parent)` 获取实例。
    - 使用相同的 `Despawn` 接口归还实例。

## 边界与注意

- 当前 `AutoDespawn` 仅与 `SyncPoolManager` 配合使用；若项目只依赖异步池，可参考其实现编写对应的自动回收逻辑。
- 若实例被外部直接 `Destroy` 而不经过 `Despawn`，内部映射表中可能残留条目；在高频场景中建议统一通过池管理对象生命周期。
- Addressables 相关行为依赖正确的资源配置与 `AssetReference`；加载失败或被取消时，`AsyncPoolManager` 会记录错误并返回 `null`，调用方需做好判空处理。

