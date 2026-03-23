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
- 首次拉取后，Unity 会按 `package.json` 解析 **Addressables、UniTask、Newtonsoft.Json** 等依赖；若解析失败，请检查网络与 `manifest.json` 中的 registry 配置。

## 依赖说明

包在 `package.json` 中声明了运行时依赖：**Addressables**、**UniTask**、**Newtonsoft.Json**（与 `SharedVariables` 序列化相关）。消费项目需能通过 UPM 解析上述包。

## 示例（Samples）

在 **Window > Package Manager** 中选择本包，展开 **Samples**，按需导入。导入后资源会复制到项目的 `Assets` 下。

部分示例使用 **Input System**、**TextMesh Pro**；若导入后编译报错，请在目标项目的 `manifest.json` 中补充 `com.unity.inputsystem` 与 TextMesh Pro / `com.unity.ugui` 等（与目标 Unity 版本一致）。

## 程序集

- `EW.Framework.Core`：`Core/`
- `EW.Framework.Modules`：`Modules/`
- `EW.Framework.Editor`：`Core/SOEventBus/Editor/`

## 外部项目验收清单（建议）

1. 仅添加本包及依赖后，项目应能**零编译错误**通过编译。
2. 按需导入 Sample，在目标 Unity 版本下打开对应 `.unity` 场景并 Play 验证。
