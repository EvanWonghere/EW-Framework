# SOEventBus（Event Channel）

基于 ScriptableObject 的事件通道，属于 EW Framework **Core** 层，对应架构中的 **Event Channel** 能力。

## 职责

- 提供**无参**与**带泛型载荷**的事件通道（ScriptableObject 资产）。
- 提供在场景中**订阅**（Listener）与**发布**（Raise）的 MonoBehaviour 组件。
- 运行期解耦：发布方与订阅方仅依赖 Channel 资产，不直接引用彼此。

## 与架构的对应关系

- **EWF_ARCHITECTURE_BLUEPRINT.md**：跨模块通信优先使用 Event Channel；ScriptableObject 资源命名前缀 `EW_`。
- **EWF_MODULE_SPECS.md**：关键状态变化应可通过 EventBus/Event Channel 观测。
- 本模块即上述「Event Channel」的参考实现，供 Modules/Demo 订阅与广播事件。

## 目录与命名

| 类型           | 说明 |
|----------------|------|
| `GameEventSO`  | 无参通道（void），CreateAssetMenu：`EW_Framework/Event Channels/Void`，默认文件名 `EW_VoidEventChannel`。 |
| `GameEventSO<T>` | 泛型通道基类（抽象），派生类见 `DataTypeDrivenChannel`。 |
| `*EventChannelSO` | 具体通道（Int/Bool/Float/String/Color/Vector2/Vector3/Quaternion/Transform），CreateAssetMenu：`EW_Framework/Event Channels/<Type>`，默认文件名 `EW_<Type>EventChannel`。 |
| `GameEventListener` / `GameEventListener<T>` | 在 OnEnable 注册、OnDisable 注销，通过 UnityEvent 响应。 |
| `GameEventRaiser` / `GameEventRaiser<T>` | 调用 `channel.Raise()` 或 `Raise(value)`。 |
| `SafeVoidEvent` / `SafeEvent<T>` | 可复用事件内核，无 SO 约束，供业务「聚合 SO」内嵌多事件使用。 |
| `BaseScriptableObject` | 可选基类：带 `description` 的 ScriptableObject，供自定义聚合 SO 等继承。 |

资源命名遵循蓝图：新建通道时默认带 `EW_` 前缀，便于识别与检索。

## 使用方式

1. **创建通道**：Project 中右键 → Create → `EW_Framework/Event Channels/Void` 或 `EW_Framework/Event Channels/<Type>`，得到对应 SO 资产。
2. **订阅**：在 GameObject 上挂 `GameEventListener`（无参）或 `IntEventListener` 等，将通道资产赋给 `channel`，在 `response` 中配置回调。
3. **发布**：在需要触发的 GameObject 上挂 `GameEventRaiser`（无参）或对应泛型 Raiser，赋同一通道，在逻辑中调用 `Raise()` 或 `Raise(value)`。
4. **代码订阅**：对通道 SO 调用 `RegisterListener(Action)` / `RegisterListener(Action<T>)`，在适当时机 `UnregisterListener`，注意与生命周期一致（如 OnEnable/OnDisable）。

## 边界与注意

- **空 channel**：Raiser 未赋值 channel 时 `Raise` 会打 Warning 并 return；Listener 未赋值且在运行中时会在 Editor 下打一次 Warning。
- **Editor**：无参通道与所有泛型通道在 Inspector 中支持「Trigger Event (Raise)」和「Active Listeners」显示；泛型通道的测试触发使用默认值（值类型为 default，引用类型为 null）。
- **Domain Reload**：仅在 Editor 下，Channel SO 在 OnDisable 时清空监听列表，避免重载后悬空引用；Build 中无此逻辑。

## 聚合 SO 模式（SafeEvent / SafeVoidEvent）

若希望**一个 ScriptableObject 承载多个事件**（如 `OnShowTool`、`OnHideTool`、`OnToolSelected`），可直接在业务 SO 中声明 `SafeVoidEvent` / `SafeEvent<T>` 字段，无需为每种通道各写一个 SO 子类：

- 在 SO 中 `public readonly SafeVoidEvent OnX = new();` 或 `public readonly SafeEvent<YourType> OnY = new();`。
- 业务代码对 SO 引用该 SO 资产，调用 `so.OnX.Register(...)` / `so.OnX.Raise()` 等。
- **必须**在 SO 的 `OnDisable` 中对该 SO 内所有 SafeEvent 调用 `Clear()`（仅 `#if UNITY_EDITOR` 即可），避免 Domain Reload 后悬空引用；与单通道 `GameEventSO` 的清理语义一致。

本模块内的 `GameEventSO` / `GameEventSO<T>` 已改为内聚 `SafeVoidEvent` / `SafeEvent<T>` 实现，行为不变。

**正确使用示例**：见包内 [`Samples~/Core-SOEventBus-Examples/`](../../Samples~/Core-SOEventBus-Examples/)（通过 Package Manager 导入 Sample 后在 `Assets` 中查看），内含原子 SO（一 SO 一通道）与聚合 SO（一 SO 多事件）的完整示例脚本与说明。

## 扩展

- 新增载荷类型：继承 `GameEventSO<T>`，添加 `[CreateAssetMenu]`，并在 `DataTypeDrivenChannel` 中补充对应 Listener/Raiser 与（可选）Editor 注册即可。
- 聚合多事件：使用 `SafeVoidEvent` / `SafeEvent<T>` 在自定义 SO 中组合，并记得在 OnDisable 中 Clear。
