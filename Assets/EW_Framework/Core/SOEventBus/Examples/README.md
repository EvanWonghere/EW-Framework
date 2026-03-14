# SOEventBus 使用示例

本目录为 **原子 SO 模式** 与 **聚合 SO 模式** 的正确用法示例，仅作参考，不参与运行时逻辑；可复制到业务工程或按需删改。

## 示例场景

- **EventBusDemo.unity**：可运行示例场景，覆盖无参/带参原子通道与聚合通道的订阅、发布与 UI 触发。
- **用法**：在 Project 中双击打开 `EventBusDemo.unity`，运行后通过 Canvas 上的按钮触发各通道，在 Console 观察对应 Listener/Consumer 的日志。
- **场景结构**：原子区（无参 Void + 带参 Int 的 Listener/Raiser 与触发按钮）、聚合区（`Example_AggregatedChannelSO` + Consumer 与 RequestStart / RequestStop / ValueChanged 等触发按钮）。

## 原子 SO 模式（一 SO 一通道）

- **含义**：一个 ScriptableObject 资产对应一条事件通道，使用本模块提供的 `GameEventSO` / `GameEventSO<T>` 或 `*EventChannelSO`。

### 无参通道（void）

- **示例脚本**：
  - `Example_Atomic_Listener.cs`：订阅无参通道，OnEnable 注册、OnDisable 注销，收到时打印日志。
  - `Example_Atomic_Raiser.cs`：持有同一通道引用，提供 `Raise()`（可绑到 UI 按钮或按键）。
- **使用步骤**：
  1. Create → `EW_Framework/Event Channels/Void`，得到无参通道资产。
  2. 场景中两个 GameObject 分别挂 `Example_Atomic_Listener` 与 `Example_Atomic_Raiser`，二者 `channel` 指向该资产。
  3. 运行后通过 Raiser 的 `Raise()` 或通道 Inspector 的「Trigger Event」触发，Listener 会打印。

### 带参通道（如 Int）

- **示例脚本**：
  - `Example_Atomic_IntListener.cs`：订阅 `IntEventChannelSO`，OnEnable 注册、OnDisable 注销，收到时打印载荷值。
  - `Example_Atomic_IntRaiser.cs`：持有同一 Int 通道，提供 `Raise(int)` 与 `RaiseWithTestValue()`（用 Inspector 中的 `testValue` 触发，可绑到按钮）。
- **使用步骤**：
  1. Create → `EW_Framework/Event Channels/Int`，得到 `IntEventChannelSO` 资产。
  2. 场景中两个 GameObject 分别挂 `Example_Atomic_IntListener` 与 `Example_Atomic_IntRaiser`，二者 `channel` 指向该资产。
  3. 运行后通过 Raiser 的 `RaiseWithTestValue()` / `Raise(int)` 或通道 Inspector 的「Trigger Event (Raise with default)」触发，Listener 会打印收到的值。

## 聚合 SO 模式（一 SO 多事件）

- **含义**：一个 ScriptableObject 内声明多个 `SafeVoidEvent` / `SafeEvent<T>`，由业务在 OnDisable 中统一 `Clear()`。
- **示例脚本**：
  - `Example_AggregatedChannelSO.cs`：聚合通道 SO，内含 `OnRequestStart`、`OnRequestStop`、`OnValueChanged(int)`，并在 OnDisable 中 Clear。
  - `Example_Aggregated_Consumer.cs`：MonoBehaviour 引用该 SO，OnEnable 注册、OnDisable 注销，并暴露可调用的 `Raise*` 方法便于测试。
- **使用步骤**：
  1. Create → `EW_Framework/Examples/Aggregated Channel`，得到 `Example_AggregatedChannelSO` 资产。
  2. 场景中建一个 GameObject，挂 `Example_Aggregated_Consumer`，将上一步资产赋给 `channel`。
  3. 运行后可在 Inspector 或代码中调用 `RaiseRequestStart()` / `RaiseRequestStop()` / `RaiseValueChanged(int)`，观察日志与生命周期是否正确。

## 注意

- 示例仅用于演示订阅/发布与生命周期（Register–Unregister、Clear），不包含业务逻辑。
- 聚合 SO 的 `OnDisable` 中必须对该 SO 内**所有** SafeEvent 调用 `Clear()`，否则 Domain Reload 后可能产生悬空引用。
