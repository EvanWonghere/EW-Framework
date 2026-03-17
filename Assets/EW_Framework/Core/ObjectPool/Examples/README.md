# ObjectPool 使用示例

本目录演示 **同步对象池（SyncPoolManager）**、**IPoolable 生命周期** 与 **AutoDespawn 自动回收** 的用法，便于在场景中验证对象池行为。

## 前置准备

1. **预制体**：在场景中创建一个带 `MeshFilter` + `MeshRenderer` 的 GameObject（如 Cube），挂上 `PooledObjectExample`（实现 IPoolable）；若需自动回收再挂 `AutoDespawn`。拖到 Project 中生成 Prefab，删除场景中的实例。
2. **同步池**：运行时 `SyncPoolManager` 会以 `PersistentMonoSingleton` 自动存在，无需手动创建。

## 脚本与用法

| 脚本 | 作用 | 操作 |
|------|------|------|
| **PooledObjectExample** | 实现 IPoolable，演示 OnSpawn/OnDespawn | 挂在预制体根节点上，运行后从池取出/回收时会在控制台打印并可选高亮。 |
| **SyncPoolSpawnerExample** | 使用 SyncPoolManager 生成/回收实例 | 将上一步预制体赋给 `prefab`，运行后 **Space** 生成、**Backspace** 回收最后一个、**C** 清空所有池；可勾选「仅自动回收」则只生成，由预制体上的 AutoDespawn 按时回收。 |

## 推荐场景搭建

1. 场景中新建空物体，命名为 `ObjectPoolDemo`，挂 `SyncPoolSpawnerExample`，在 Inspector 中指定 **Prefab**（带 `PooledObjectExample` 的预制体）。
2. 若需「生成后自动消失」：在预制体上挂 `AutoDespawn`，设置 `delay`（秒），并勾选 Spawner 的 **Spawn Only (Auto Despawn)**。
3. 运行后按 **Space** 在 spawn 点附近生成实例，观察 Console 的 OnSpawn/OnDespawn 日志；按 **Backspace** 回收最后一个，按 **C** 清空池。

## 异步池（Addressables）用法简述

- **AsyncPoolManager** 使用 Addressables + AssetReference，适合按地址异步加载的预制体。
- 调用方式：`await AsyncPoolManager.Instance.SpawnAsync(assetRef, position, rotation, parent);`
- 回收与同步池相同：`AsyncPoolManager.Instance.Despawn(instance);`
- 需在 Project 中配置 Addressables 资源，并将预制体赋给 `AssetReference` 使用。本示例场景仅演示同步池。

## 注意

- 示例仅用于演示 Spawn/Despawn 与 IPoolable 生命周期，不包含业务逻辑。
- `AutoDespawn` 当前仅与 **SyncPoolManager** 配合使用；若项目只使用 AsyncPoolManager，需自行实现定时回收或扩展 AutoDespawn。
