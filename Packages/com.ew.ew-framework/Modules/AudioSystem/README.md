# AudioSystem（Modules）

基于 **SOEventBus（Event Channel）** 的命令式音频模块：业务侧只需向 `AudioRequestChannelSO` 发布 `AudioCommand`，由 `AudioManager` 统一调度 BGM 与各类音效播放器（对象池 + TimerSystem）。

## 职责

- **命令总线**：`AudioCommand` + `AudioRequestChannelSO` 解耦“请求方”和“播放实现”
- **播放调度**：`AudioManager` 负责 BGM Crossfade、SFX/UI/Voice 的生成与管理
- **资源复用**：SFX 使用对象池（`SyncPoolManager`），避免频繁 Instantiate/Destroy
- **定时回收 / Fade**：使用 `TimerSystem` 实现播完回收、FadeIn/FadeOut
- **3D 跟随**：支持跟随 `Transform`；目标失活时自动回收避免“幽灵音源”
- **可观测性（Debug）**：`AudioManager.ActiveSFXCount` 与 `RecentCommands`

## 目录结构

```text
Packages/com.ew.ew-framework/Modules/AudioSystem/
  Runtime/
    Scripts/
      AudioManager.cs
      AudioPlayer.cs
      AudioRequests.cs
      AudioRequestChannelSO.cs
      AudioRequestListener.cs
      AudioRequestRaiser.cs
    Prefabs/
      SFX_Player.prefab
  （示例）../Samples~/Modules-AudioSystem-Examples/
    README.md
    AudioSystemExamplePanel.cs
    AudioSystemExampleFollowTargetMover.cs
    AudioSystemDemo.unity
    *.mp3
    EW_AudioRequestChannel.asset
```

## 快速接入（推荐步骤）

1. **创建通道资产**：Project 右键 → Create → `EW_Framework/Audio/AudioRequestChannel`，得到 `AudioRequestChannelSO`。
2. **场景放置 `AudioManager`**（全局单例）并配置：
   - `audioRequestChannel`：指向第 1 步创建的 Channel
   - `sfxPlayerPrefab`：指向 `Runtime/Prefabs/SFX_Player.prefab`（或你的自定义 prefab，需挂 `AudioPlayer`）
3. **确保依赖已初始化**：
   - 对象池：`SyncPoolManager.Instance` 可用
   - 计时器：`TimerManager.Instance` 可用
4. **业务侧发命令**：对同一 `AudioRequestChannelSO` 调用 `Raise(cmd)`。

## Listener / Raiser（可选，Inspector 友好）

与 Core 层 **SOEventBus**（`GameEventListener<T>` / `GameEventRaiser<T>`）对齐，本模块提供：

| 类型 | 作用 |
|------|------|
| `AudioRequestRaiser` | 在 GameObject 上指定 `AudioRequestChannelSO`，在逻辑里调用 `Raise(AudioCommand)`，把「往哪个通道发」固定在场景 / Prefab 上。 |
| `AudioRequestListener` | 订阅同一通道，用 Inspector 里的 `UnityEvent<AudioCommand>` 配置响应，无需手写 `RegisterListener` / `UnregisterListener`。 |

详细约定见：`Packages/com.ew.ew-framework/Core/SOEventBus/README.md`。

**二者都是可选的**：只持有通道资产并在代码里 `Raise` / `RegisterListener` 也能跑通全流程。框架仍提供这两个薄封装，是为了让 **策划、关卡、美术、技术美术等少写 C#、多在 Inspector 里绑资源与回调**——这也是 Event Channel 在本项目里希望覆盖的协作方式；若只用 `AudioRequestChannelSO` 而从不使用 Raiser / Listener，会少掉这一层对非程序角色的友好度。

**与 `AudioManager` 的关系**：播放与调度仍由场景中的 `AudioManager` 通过代码订阅通道完成。`AudioRequestListener` 适合 **并行** 监听（调试面板、埋点、旁路逻辑等），不要用它再实现一套与 `AudioManager` 重复的播放职责。

## 核心数据结构

### `AudioCommand`

位置：`Runtime/Scripts/AudioRequests.cs`  
命名空间：`EW_Framework.Modules.AudioSystem.Runtime`

- **`AudioCommandType`**：`Play / StopByKey / StopAll / StopByType / Pause / Resume / SetVolume`（以及 `Stop` 作为 StopAll 的别名语义）
- **`AudioKey`**：用于去重、追踪、定向停止/暂停/恢复/调音量
- **`ConcurrencyPolicy`**（同 Key 并发策略）：
  - `PlayAdditive`：允许叠加（默认）
  - `Override`：顶替旧的（先 Stop 再 Play）
  - `Ignore`：旧的还在播就忽略新的
- **`FadeDuration`**：
  - `Play` 时用于 FadeIn
  - `Stop*` 时用于 FadeOut
- **3D**：
  - `Is3D` + `FixedPosition`
  - 或 `FollowTarget`（跟随移动）

### 事件流（简图）

```mermaid
flowchart LR
  gameplay[GameplayScripts] -->|Raise(AudioCommand)| channel[AudioRequestChannelSO]
  channel --> manager[AudioManager]
  manager -->|Spawn/Despawn| pool[SyncPoolManager]
  manager --> player[AudioPlayer]
  player -->|Fade/AutoDespawn| timer[TimerManager]
```

## FMOD 注意事项

若工程集成了 **FMOD**，运行时动态 `AddComponent<AudioSource>()` 可能触发 FMOD 输出切换错误（`Cannot call this command after System::init`）。

本模块的 BGM Source 采用 **预配置（SerializeField）+ Editor OnValidate 自动创建子物体** 的方式，避免运行时 AddComponent：

- 子物体名：`BGM_Source_A` / `BGM_Source_B`
- 运行时仅做“查找/校验/配置”，不会 AddComponent

## Examples（演示与回归）

参见：[`Samples~/Modules-AudioSystem-Examples/README.md`](../../Samples~/Modules-AudioSystem-Examples/README.md)（或通过 Package Manager 导入 Sample）。  
提供 `OnGUI` 面板脚本，不依赖生成新的 `.unity` 场景文件即可演示：

- BGM：Play/Stop/Pause/Resume/SetVolume（Crossfade）
- SFX：2D/3D/Follow + StopByKey/Pause/Resume/SetVolume + StopAll/StopByType + Concurrency + Fade
- UI/Voice：Play + StopByKey/Pause/Resume/SetVolume + StopByType

## 常见问题

### 1) 没声音 / 报对象池或 Timer 为空

- **现象**：Console 提示 `SyncPoolManager.Instance is null` 或 `TimerManager.Instance is null`
- **处理**：确保场景中按 Core 模块的示例正确初始化了对象池与 TimerSystem

### 2) `AudioType` 与 Unity 的 `UnityEngine.AudioType` 冲突

在示例脚本中如果出现歧义，使用别名即可（示例已处理）：

```csharp
using AudioType = EW_Framework.Modules.AudioSystem.Runtime.AudioType;
```
