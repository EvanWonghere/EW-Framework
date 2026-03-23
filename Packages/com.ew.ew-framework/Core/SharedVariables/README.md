# SharedVariables

基于 ScriptableObject 的共享变量模块，支持运行时读写、变更事件与可选的持久化（通过 `ISaveable`）。

## 依赖

- **Newtonsoft.Json**（`com.unity.nuget.newtonsoft-json`）：序列化/反序列化由 Newtonsoft 完成，未使用 Unity 内置 `JsonUtility`。
- 泛型类型 **T 必须能被 Newtonsoft.Json 正确序列化**（如 `int`、`float`、`string`、简单 DTO 等）。若使用 Unity 原生类型（如 `Vector3`、`Color`）或复杂引用类型，需自行保证可序列化或提供转换器。

## 设计说明

- **`[Serializable]`**：用于 Unity Inspector 的序列化展示，与 Newtonsoft 的 JSON 序列化无关。
- **`SharedVariableSO<T>`**：当前为 `abstract` 类，**仅允许通过子类使用**（如 `SharedIntSO`、`SharedFloatSO`），不可直接实例化。

## 结构

- `Base/ISaveable.cs`：可存档接口（SaveKey、GetSaveData、LoadData）。
- `Base/SharedVariableSO.cs`：抽象基类，提供 `Value`、`OnValueChanged`、`initialValue`/`runtimeValue` 及 `ISaveable` 实现。`saveKey` 为空时不参与持久化，由上层存档系统根据 `SaveKey` 决定是否读写。
- `DataTypeDrivenVariable/`：按类型拆分的具体共享变量（Int、Bool、Float、String、Color、Vector2、Vector3、Quaternion），对应 SOEventBus 的 DataTypeDrivenChannel 结构，用于在 Project 中创建资产。
- （示例）`Samples~/Core-SharedVariables-Examples/`：展示用例（监听、修改、可选持久化），见 [`Samples~/Core-SharedVariables-Examples/README.md`](../../Samples~/Core-SharedVariables-Examples/README.md)。
