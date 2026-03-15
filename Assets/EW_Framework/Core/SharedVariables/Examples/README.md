# SharedVariables 展示用例（Examples）

本文件夹提供监听、修改与可选持久化三种展示用法，便于在场景中验证 SharedVariables 行为。

## 前置准备

1. 在 Project 中创建共享变量资源：右键 → Create → EW Framework → Shared Variables → Int / Bool / Float / String / Color / Vector2 / Vector3 / Quaternion（基础类型定义在 `DataTypeDrivenVariable/`）。
2. 需要**可选持久化**时，在资源的 **Save Settings** 中填写 `saveKey`（如 `player_score`）；不填则不会参与存档。

## 脚本与用法

| 脚本 | 作用 | 操作 |
|------|------|------|
| **SharedVariableListenerExample** | 监听共享变量变化 | 拖入 SharedIntSO / SharedFloatSO，运行后当变量被修改时会在控制台打印新值。 |
| **SharedVariableModifierExample** | 修改共享变量 | 拖入要改的变量，运行后 **Q/E** 增减 Int，**W/S** 增减 Float。 |
| **SharedVariablePersistenceExample** | 可选持久化 | 将带 `saveKey` 的 SharedIntSO / SharedFloatSO 拖入列表，运行后 **F5** 存档、**F9** 读档（使用 PlayerPrefs）。 |
| **SimpleSaveLoadHelper** | 存档工具（静态） | 对任意 `ISaveable` 集合按 `SaveKey` 非空项做 Save/Load，本示例用 PlayerPrefs；可替换为文件或云存档。 |

## 推荐场景搭建

1. 同一场景中放一个 **Listener**、一个 **Modifier**、一个 **Persistence**，并指向同一批 SharedIntSO / SharedFloatSO 资源。
2. 其中一个变量设 `saveKey`，另一个不设：修改后按 F5 存档，重启场景后按 F9，仅带 `saveKey` 的变量会恢复。

## 类型说明

- 基础类型（**SharedIntSO / SharedBoolSO / SharedFloatSO / SharedStringSO / SharedColorSO / SharedVector2SO / SharedVector3SO / SharedQuaternionSO**）定义在 `../DataTypeDrivenVariable/PrimitiveSharedVariables.cs`，用于在 Project 中创建资产；继承自 `SharedVariableSO<T>`。
