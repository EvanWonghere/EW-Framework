# Singleton

提供纯 C# 与 Unity `MonoBehaviour` 两类单例基类，作为 Core 与 Modules 层服务/管理器的基础设施。

## 职责

- 统一单例模式的写法，避免在业务代码中重复实现懒加载与防御逻辑。
- 对纯逻辑类提供线程安全的懒加载单例实现（无 Unity 依赖）。
- 对 `MonoBehaviour` 管理器类提供场景级与跨场景持久两种单例形态。

## 结构

- `Base/Singleton.cs`
  - 纯 C# 泛型单例基类：`Singleton<T> where T : Singleton<T>`.
  - 使用 `Lazy<T>` 实现线程安全的懒加载，适合数据管理、算法类等非 MonoBehaviour 类型。
- `MonoSingleton.cs`
  - 场景生命周期单例：`MonoSingleton<T> where T : MonoSingleton<T>`.
  - 特性：
    - `Instance` 会优先在场景中查找已有实例，若不存在则自动创建一个新的 GameObject 并挂载组件。
    - 防止 Editor 播放结束时的幽灵对象访问：退出前标记 `_isQuitting`，此后访问 `Instance` 仅给出 Warning 并返回 null。
- `PersistentMonoSingleton.cs`
  - 跨场景持久单例：`PersistentMonoSingleton<T> : MonoSingleton<T> where T : PersistentMonoSingleton<T>`.
  - 特性：
    - 只有真正的 `Instance` 会被标记为 `DontDestroyOnLoad`，并在有父节点时先移动到根节点再持久化，以避免 Unity 报错。

## 使用建议

- **纯逻辑服务**（不依赖 Unity 生命周期）：
  - 继承 `Singleton<T>`，将构造函数设为 `private` 或 `protected`，通过 `T.Instance` 访问。
- **场景级管理器**（随场景加载/卸载）：
  - 继承 `MonoSingleton<T>`，在场景中放置一个实例，或完全依赖运行时自动创建的 GameObject。
- **跨场景全局服务**（如全局配置、音频管理、全局对象池管理器等）：
  - 继承 `PersistentMonoSingleton<T>`，即使切换场景也保持唯一实例不被销毁。

## 边界与注意

- `MonoSingleton` / `PersistentMonoSingleton` 仍假定从主线程访问，虽包含锁，但 Unity 对象的创建与销毁应限定在主线程。
- 自动创建 GameObject 的行为适合「工具型」或「基础设施」组件；若需要严格手动配置，可在模块文档中明确说明或封装额外开关。

