# ObjectPool

基于 Unity 官方 `IObjectPool<T>` 的对象池模块，提供 **GameObject 池**（同步 / 异步 Addressables）与 **纯 C# 引用池**（ReferencePoolManager），并约定统一的 `IPoolable` / `IReference<T>` 生命周期接口。

## 职责

- 通过对象池替代频繁的 `Instantiate/Destroy` 或 `new`，减少 GC 与运行时开销。
- 为同步场景与 Addressables 异步加载场景统一提供 Spawn/Despawn 入口。
- 为纯 C# 类型提供无字典、O(1) 的引用池（Acquire/Release）。
- 为业务对象提供清晰的生命周期回调（`OnSpawn`/`OnDespawn` 或 `OnReturnPool`）。

## 结构

- `Base/IPoolable.cs`
  - 可池化 **GameObject** 接口：
    - `void OnSpawn()`：从池中取出时调用，用于重置状态、播放特效等。
    - `void OnDespawn()`：回收到池中前调用，用于清理状态、停止特效等。
- `Base/IReference.cs`
  - 可池化 **纯 C# 类型** 接口（无 Unity 依赖）：
    - `void OnReturnPool()`：归还池时调用，必须清空引用并重置字段，防止脏数据与泄漏。
- `Base/PoolItem.cs`
  - 挂在池内 GameObject 上，持有所属 `IObjectPool<GameObject>`，用于 Despawn 时定位池。
- `Manager/SyncPoolManager.cs`
  - 同步 **GameObject** 池：`SyncPoolManager : PersistentMonoSingleton<SyncPoolManager>`.
  - 以 **Prefab → Pool** 单表 + 实例上的 `PoolItem` 管理：
    - `Spawn(prefab, position, rotation, parent)`：替代 `Instantiate`，从对应池取或创建实例。
    - `Despawn(instance)`：替代 `Destroy`，通过 `PoolItem.Pool` 归还；非池内对象直接销毁。
    - `ClearAllPools()`：Dispose 所有池并清理父节点，适合场景切换或内存紧张时调用。
- `Manager/AsyncPoolManager.cs`
  - 异步对象池管理器：`AsyncPoolManager : PersistentMonoSingleton<AsyncPoolManager>`.
  - 以 Addressables `AssetReference` 的 GUID 为 key：
    - `SpawnAsync(assetRef, position, rotation, parent)`：先异步加载 prefab，再通过对象池复用实例；内部对并发初始化同一 GUID 做合并与错误处理。
    - `Despawn(instance)`：与同步池语义一致。
    - `ClearAllPools()`：释放所有池、Addressables 句柄与父节点。
- `Manager/ReferencePoolManager.cs`
  - 纯 C# **引用池**（静态类，无 MonoBehaviour）：
    - `Acquire<T>()`：从类型 T 的池中取一个实例（无字典查找，O(1)）。T 须实现 `IReference<T>` 且具无参构造。
    - `Release<T>(obj)`：将对象归还类型 T 的池，会调用 `obj.OnReturnPool()`。
    - `Clear<T>()`：清空类型 T 的池内对象，池本身仍可继续使用。
  - 约定：**仅对通过 `Acquire<T>()` 得到的实例调用 `Release<T>()`**，否则会污染该类型池。
- `Utilities/AutoDespawn.cs`
  - 挂在实例上的工具组件：在 `OnEnable` 启动协程，等待一定时间后调用 `SyncPoolManager.Instance.Despawn`，适合特效、飘字、一次性音效等。
- （示例）包内 `Samples~/Core-ObjectPool-Examples/`：通过 Package Manager 导入 Sample 后，见 `Assets` 中副本的 README 或 [`Samples~/Core-ObjectPool-Examples/README.md`](../../Samples~/Core-ObjectPool-Examples/README.md)。

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
- **纯 C# 引用池**（如 DTO、临时结构、非 MonoBehaviour 逻辑对象）：
  - 实现 `IReference<T>`（T 为自身类型），在 `OnReturnPool()` 中清空引用并重置字段。
  - 使用 `ReferencePoolManager.Acquire<MyType>()` 取实例，用完后 `ReferencePoolManager.Release(obj)` 归还；**不要**对非 Acquire 得到的实例或其它类型调用 `Release`。

## 边界与注意

- 当前 `AutoDespawn` 仅与 `SyncPoolManager` 配合使用；若项目只依赖异步池，可参考其实现编写对应的自动回收逻辑。
- 若 GameObject 实例被外部直接 `Destroy` 而不经过 `Despawn`，`PoolItem` 会随之销毁，无残留表项；但仍建议统一通过池管理生命周期。
- Addressables 相关行为依赖正确的资源配置与 `AssetReference`；加载失败或被取消时，`AsyncPoolManager` 会记录错误并返回 `null`，调用方需做好判空处理。
- **Reference 池**：仅对通过 `Acquire<T>()` 得到的对象调用 `Release<T>()`；未实现线程安全，多线程使用需在业务侧加锁。

