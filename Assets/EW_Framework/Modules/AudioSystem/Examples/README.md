# AudioSystem Examples（Modules）

本目录提供一组**不依赖 .unity 场景文件**的演示脚本，用于验证 `AudioSystem` 的关键能力：

- **SOEventBus 集成**：通过 `AudioRequestChannelSO.Raise(AudioCommand)` 驱动播放/停止等命令
- **命令语义**：`Play / StopByKey / StopAll / StopByType / Pause / Resume / SetVolume`
- **并发策略**：同 `AudioKey` 的 `PlayAdditive / Override / Ignore`
- **Fade**：`FadeDuration` 支持 FadeIn / FadeOut（基于 `TimerSystem`）
- **3D Follow**：跟随目标移动；目标失活时自动回收，避免“幽灵音源”
- **可观测性**：`AudioManager.ActiveSFXCount` 与 `AudioManager.RecentCommands` 便于调试

## 使用方式（推荐）

1. 在任意场景中放置一个全局 `AudioManager`（确保对象池与 TimerSystem 已初始化）。
2. 创建一个 `AudioRequestChannelSO` 资产，并把它赋给：
   - `AudioManager.audioRequestChannel`
   - 示例面板脚本的 `channel`
3. 确保 `AudioManager.sfxPlayerPrefab` 指向带 `AudioPlayer` 的预制体。
4. 在场景里创建一个空物体，挂上：
   - `AudioSystemExamplePanel`
   - （可选）`AudioSystemExampleFollowTargetMover`（用于移动 FollowTarget）
5. 在 `AudioSystemExamplePanel` Inspector 中配置：
   - `channel`
   - `bgmClip` / `sfxClip` / `uiClip` / `voiceClip`
   - `followTarget`（可选）
6. Play 后使用面板按钮触发各类命令，观察：
   - 音频行为是否符合预期
   - `AudioManager` Inspector 中 `ActiveSFXCount/RecentCommands` 是否更新

## 覆盖点清单

- `SFX`：Play(2D/3D/Follow)、StopByKey、Pause/Resume/SetVolume、StopAll、StopByType
- `UI/Voice`：Play、StopByKey、Pause/Resume/SetVolume、StopByType
- `BGM`：Play/Stop/Pause/Resume/SetVolume（播放采用 BGM Crossfade）

## 注意

- 本示例偏向“覆盖功能面”，不绑定具体项目资源管线（AudioMixer / Addressables 等）。
- 若未配置 `TimerManager` 或 `SyncPoolManager`，模块会输出明确错误日志。
