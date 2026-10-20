# EW Framework（UPM 包）

本目录为 Unity Package Manager 包根目录，包名为 `com.ew.ew-framework`。

## 在本开发仓库中使用

将本仓库作为 Unity 项目打开时，`Packages/com.ew.ew-framework` 会被自动识别为**嵌入式包**，无需在 `manifest.json` 中额外声明。

## 在其它 Unity 项目中引用

框架源码位于仓库内 **`Packages/com.ew.ew-framework/`**（包名 `com.ew.ew-framework`）。**稳定安装请使用远程 `release` 分支**（该分支用于存放已通过编译验证的 UPM 布局与版本）。

### 方式 A：本地路径（适合与本仓库联调）

克隆或下载本仓库到本机后，在目标项目的 `Packages/manifest.json` 的 `dependencies` 中加入（按实际路径修改）：

```json
"com.ew.ew-framework": "file:../../../EW-Framework/Packages/com.ew.ew-framework"
```

`file:` 后为**绝对路径**，或**相对于目标项目 `Packages/manifest.json` 所在目录**的相对路径。

### 方式 B：Git URL + 子目录（推荐：引用 release 分支）

在目标项目 `Packages/manifest.json` 的 `dependencies` 中加入：

```json
"com.ew.ew-framework": "https://github.com/EvanWonghere/EW-Framework.git?path=Packages/com.ew.ew-framework#release"
```

- `#release` 表示跟踪 **`release`** 分支；若需锁定版本，可改为 **tag**（如 `#v0.1.0`）或 **commit SHA**。
- Unity Package Manager 需能访问 GitHub（HTTPS）；公司内网若禁用外网，请改用方式 A 或私有 Git 镜像。
- 首次拉取后，Unity **理论上**会按本包 `package.json` 解析 **Addressables、UniTask、Newtonsoft.Json**；但对「仅通过 Git 安装的包」，**嵌套的 Git 依赖有时不会自动写入消费工程的 `manifest.json`**，会导致 `Cysharp` / `UniTask` 找不到。若出现该情况，请见下方 **「消费项目建议显式声明的依赖」**。

### 消费项目建议显式声明的依赖（避免 Cysharp / UniTask 与 Burst 报错）

在目标工程的 `Packages/manifest.json` 的 `dependencies` 中，除 `com.ew.ew-framework` 外，建议**并列加入**（与 `package.json` 中版本对齐；若已有同名键，保留一条即可）：

```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
"com.unity.addressables": "2.9.1",
"com.unity.nuget.newtonsoft-json": "3.2.2"
```

说明：

- **`CS0246: Cysharp` / `UniTask<>`**：多为 UniTask 未安装或未参与编译；显式声明后回到 Unity 让其重新解析。
- **`Failed to resolve assembly: EW.Framework.Editor`（Burst / Mono.Cecil）**：多为 **`EW.Framework.Core` 因缺 UniTask 等未编过**，Editor 程序集未生成时的连锁报错；先解决 Core 与 UniTask，再观察是否仍出现。

保存 `manifest.json` 后回到 Unity；若仍异常，可尝试 **Assets → Reimport All**，或关闭 Unity 后删除工程内 `Library/ScriptAssemblies` 再打开。

## 依赖说明

包在 `package.json` 中声明了运行时依赖：**Addressables**、**UniTask**、**Newtonsoft.Json**（与 `SharedVariables` 序列化相关）。消费项目需能通过 UPM 解析上述包；**若仅用 Git URL 引用本包且子依赖未自动出现，请在上文「消费项目建议显式声明的依赖」中手动补齐。**

## 示例（Samples）

在 **Window > Package Manager** 中选择本包，展开 **Samples**，按需导入。导入后资源会复制到项目的 `Assets` 下。

部分示例使用 **Input System**、**TextMesh Pro**；若导入后编译报错，请在目标项目的 `manifest.json` 中补充 `com.unity.inputsystem` 与 TextMesh Pro / `com.unity.ugui` 等（与目标 Unity 版本一致）。

`EventBusDemo` 与 `SharedVariablesDemo` 的 UI 使用 TextMesh Pro 默认字体。新工程若尚未导入 TMP 资源，请先执行 **Window > TextMeshPro > Import TMP Essential Resources**，再导入这两个示例，否则场景中的文字会丢失字体与材质引用。

## 程序集

- `EW.Framework.Core`：`Core/`
- `EW.Framework.Modules`：`Modules/`
- `EW.Framework.Editor`：`Core/SOEventBus/Editor/`
- `EW.Framework.Tests.Editor`：`Tests/Editor/`（EditMode 测试，仅在编辑器且启用测试时编译）

## 测试

本仓库中为嵌入式包，测试直接出现在 **Test Runner > EditMode** 中。在其它工程中通过 Git URL 引用时，需在该工程 `manifest.json` 顶层加入以下配置，本包测试才会参与运行：

```json
"testables": ["com.ew.ew-framework"]
```

## 外部项目验收清单（建议）

1. 仅添加本包及依赖后，项目应能**零编译错误**通过编译。
2. 按需导入 Sample，在目标 Unity 版本下打开对应 `.unity` 场景并 Play 验证。
3. （可选）按上文加入 `testables` 后，在 Test Runner 中运行 EditMode 测试。
