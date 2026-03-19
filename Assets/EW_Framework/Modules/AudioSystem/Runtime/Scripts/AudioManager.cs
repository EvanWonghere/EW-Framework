using System.Collections.Generic;
using UnityEngine;
using EW_Framework.Core.ObjectPool.Manager;
using EW_Framework.Core.Singleton;
using EW_Framework.Core.TimerSystem;

namespace EW_Framework.Modules.AudioSystem.Runtime
{
    /// <summary>
    /// Global Audio Manager.
    /// Listens to AudioRequestChannelSO and executes AudioCommand.
    /// </summary>
    public class AudioManager : PersistentMonoSingleton<AudioManager>
    {
        [Header("Event Channel (SOEventBus)")]
        [Tooltip("Audio command channel. AudioManager listens to it and performs actual playback.")]
        public AudioRequestChannelSO audioRequestChannel;

        [Header("Pool Settings")]
        [Tooltip("Prefab that has an AudioPlayer component (used for SFX/UI/Voice/etc).")]
        public GameObject sfxPlayerPrefab;

        [Header("BGM Settings")]
        [Tooltip("The duration of the BGM cross-fade (seconds).")]
        public float bgmCrossfadeDuration = 1.5f;

        [Header("BGM Sources (pre-configured, no runtime AddComponent)")]
        [SerializeField] private AudioSource _bgmSourceA;
        [SerializeField] private AudioSource _bgmSourceB;
        private bool _isUsingSourceA = true;

        private Timer _crossfadeTimer;

        // Core tracker: active SFX players (Key -> list)
        private readonly Dictionary<string, List<AudioPlayer>> _activeSFXTracker = new();

        [Header("Debug & Observability")]
        [Tooltip("Debug-only counter. It tracks registrations/unregistrations under AudioManager's control and may not equal the number of still-fading-out pooled objects (e.g., StopAll with FadeDuration).")]
        public int ActiveSFXCount = 0;
        [Tooltip("Recent commands for quick observability (max 15).")]
        public List<string> RecentCommands = new();

        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                EnsureBgmSourcesReady();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this == null) return;
            EnsureBgmSourcesExistInEditor();
        }
#endif

        private void OnEnable()
        {
            if (Instance != this) return;
            if (audioRequestChannel == null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Debug.LogWarning($"[AudioManager] {name}: audioRequestChannel is not assigned. AudioManager will not receive commands.", this);
#endif
                return;
            }
            audioRequestChannel.RegisterListener(OnAudioCommand);
        }

        private void OnDisable()
        {
            if (Instance != this) return;
            if (audioRequestChannel != null) audioRequestChannel.UnregisterListener(OnAudioCommand);
            _activeSFXTracker.Clear();
            ActiveSFXCount = 0;
            RecentCommands.Clear();
        }

        private void OnAudioCommand(AudioCommand cmd)
        {
            LogCommand(cmd);

            if (cmd.Type == AudioType.BGM)
            {
                HandleBgmCommand(cmd);
                return;
            }

            HandleSfxCommand(cmd);
        }

        // ================= SFX / UI / Voice =================

        private void HandleSfxCommand(AudioCommand cmd)
        {
            switch (cmd.CmdType)
            {
                case AudioCommandType.Play:
                    PlaySfx(cmd);
                    break;
                case AudioCommandType.Stop:
                case AudioCommandType.StopAll:
                    StopAllSfx(cmd.FadeDuration);
                    break;
                case AudioCommandType.StopByKey:
                    StopSfxByKey(cmd.AudioKey, cmd.FadeDuration);
                    break;
                case AudioCommandType.StopByType:
                    StopSfxByType(cmd.Type, cmd.FadeDuration);
                    break;
                case AudioCommandType.Pause:
                case AudioCommandType.Resume:
                case AudioCommandType.SetVolume:
                    DispatchToTrackedPlayersByKey(cmd);
                    break;
                default:
                    Debug.LogWarning($"[AudioManager] Unsupported command: {cmd.CmdType}");
                    break;
            }
        }

        private void PlaySfx(AudioCommand cmd)
        {
            if (sfxPlayerPrefab == null)
            {
                Debug.LogError("[AudioManager] SFX Player prefab is not configured.", this);
                return;
            }
            if (cmd.Clip == null)
            {
                Debug.LogWarning("[AudioManager] Play requested with null clip. Skip.", this);
                return;
            }
            if (SyncPoolManager.Instance == null)
            {
                Debug.LogError("[AudioManager] SyncPoolManager.Instance is null. Ensure ObjectPool is initialized in this scene.", this);
                return;
            }

            string key = cmd.AudioKey ?? string.Empty;

            // Concurrency policy for same key
            if (!string.IsNullOrEmpty(key) && _activeSFXTracker.TryGetValue(key, out var activeList) && activeList != null && activeList.Count > 0)
            {
                if (cmd.Concurrency == ConcurrencyPolicy.Ignore) return;
                if (cmd.Concurrency == ConcurrencyPolicy.Override) StopSfxByKey(key, cmd.FadeDuration);
            }

            Vector3 spawnPos = cmd.Is3D
                ? (cmd.FollowTarget != null ? cmd.FollowTarget.position : cmd.FixedPosition)
                : Vector3.zero;

            GameObject playerObj = SyncPoolManager.Instance.Spawn(sfxPlayerPrefab, spawnPos, Quaternion.identity, transform);
            if (playerObj == null)
            {
                Debug.LogError($"[AudioManager] Failed to spawn player from pool. Prefab: {(sfxPlayerPrefab != null ? sfxPlayerPrefab.name : "null")}", this);
                return;
            }

            if (!playerObj.TryGetComponent<AudioPlayer>(out var player))
            {
                Debug.LogError($"[AudioManager] Spawned object '{playerObj.name}' does not have AudioPlayer component. Check prefab setup.", this);
                return;
            }

            RegisterActiveSfx(key, player);
            player.ExecuteCommand(cmd);
        }

        private void DispatchToTrackedPlayersByKey(AudioCommand cmd)
        {
            string key = cmd.AudioKey ?? string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"[AudioManager] {cmd.CmdType} requires AudioKey for targeting.", this);
                return;
            }

            if (!_activeSFXTracker.TryGetValue(key, out var players) || players == null || players.Count == 0) return;

            var snapshot = new List<AudioPlayer>(players);
            foreach (var p in snapshot)
            {
                if (p == null) continue;
                p.ExecuteCommand(cmd);
            }

            CleanupKey(key);
        }

        private void RegisterActiveSfx(string key, AudioPlayer player)
        {
            if (string.IsNullOrEmpty(key) || player == null) return;
            if (!_activeSFXTracker.TryGetValue(key, out var list) || list == null)
            {
                list = new List<AudioPlayer>();
                _activeSFXTracker[key] = list;
            }
            if (!list.Contains(player)) list.Add(player);
            ActiveSFXCount++;
        }

        public void UnregisterPlayer(string key, AudioPlayer player)
        {
            if (Instance != this) return;
            if (string.IsNullOrEmpty(key) || player == null) return;
            if (!_activeSFXTracker.TryGetValue(key, out var list) || list == null) return;
            if (list.Remove(player)) ActiveSFXCount = Mathf.Max(0, ActiveSFXCount - 1);
            if (list.Count == 0) _activeSFXTracker.Remove(key);
        }

        private void StopSfxByKey(string key, float fadeDuration)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (!_activeSFXTracker.TryGetValue(key, out var list) || list == null || list.Count == 0) return;

            var snapshot = new List<AudioPlayer>(list);
            foreach (var p in snapshot)
            {
                if (p == null) continue;
                p.ExecuteCommand(new AudioCommand
                {
                    CmdType = AudioCommandType.StopByKey,
                    AudioKey = key,
                    Type = AudioType.SFX,
                    FadeDuration = fadeDuration,
                });
            }

            CleanupKey(key);
        }

        private void StopAllSfx(float fadeDuration)
        {
            foreach (var kvp in _activeSFXTracker)
            {
                var key = kvp.Key;
                var snapshot = new List<AudioPlayer>(kvp.Value);
                foreach (var p in snapshot)
                {
                    if (p == null) continue;
                    p.ExecuteCommand(new AudioCommand
                    {
                        CmdType = AudioCommandType.StopAll,
                        AudioKey = key,
                        Type = AudioType.SFX,
                        FadeDuration = fadeDuration,
                    });
                }
            }

            _activeSFXTracker.Clear();
            ActiveSFXCount = 0;
        }

        private void StopSfxByType(AudioType type, float fadeDuration)
        {
            var keysToCleanup = new List<string>();
            foreach (var kvp in _activeSFXTracker)
            {
                var key = kvp.Key;
                var snapshot = new List<AudioPlayer>(kvp.Value);
                bool stoppedAny = false;
                foreach (var p in snapshot)
                {
                    if (p == null) continue;
                    if (p.CurrentType != type) continue;
                    stoppedAny = true;
                    p.ExecuteCommand(new AudioCommand
                    {
                        CmdType = AudioCommandType.StopByType,
                        AudioKey = key,
                        Type = type,
                        FadeDuration = fadeDuration,
                    });
                }
                if (stoppedAny) keysToCleanup.Add(key);
            }

            foreach (var key in keysToCleanup) CleanupKey(key);
        }

        private void CleanupKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (!_activeSFXTracker.TryGetValue(key, out var list) || list == null) return;
            list.RemoveAll(p => p == null || string.IsNullOrEmpty(p.CurrentKey));
            if (list.Count == 0) _activeSFXTracker.Remove(key);
        }

        // ================= BGM =================

        private void HandleBgmCommand(AudioCommand cmd)
        {
            switch (cmd.CmdType)
            {
                case AudioCommandType.Play:
                    PlayBGM(cmd.Clip, cmd.Volume);
                    break;
                case AudioCommandType.Stop:
                case AudioCommandType.StopAll:
                    StopBGM();
                    break;
                case AudioCommandType.Pause:
                    PauseBGM();
                    break;
                case AudioCommandType.Resume:
                    ResumeBGM();
                    break;
                case AudioCommandType.SetVolume:
                    SetBGMVolume(cmd.Volume);
                    break;
                default:
                    Debug.LogWarning($"[AudioManager] Unsupported BGM command: {cmd.CmdType}", this);
                    break;
            }
        }

        private void StopBGM()
        {
            if (_bgmSourceA == null || _bgmSourceB == null) EnsureBgmSourcesReady();
            _bgmSourceA?.Stop();
            _bgmSourceB?.Stop();
        }

        private void PauseBGM()
        {
            if (_bgmSourceA == null || _bgmSourceB == null) EnsureBgmSourcesReady();
            _bgmSourceA?.Pause();
            _bgmSourceB?.Pause();
        }

        private void ResumeBGM()
        {
            if (_bgmSourceA == null || _bgmSourceB == null) EnsureBgmSourcesReady();
            if (_bgmSourceA != null && _bgmSourceA.clip != null) _bgmSourceA.UnPause();
            if (_bgmSourceB != null && _bgmSourceB.clip != null) _bgmSourceB.UnPause();
        }

        private void SetBGMVolume(float volume)
        {
            if (_bgmSourceA == null || _bgmSourceB == null) EnsureBgmSourcesReady();
            volume = Mathf.Clamp01(volume);
            var active = _isUsingSourceA ? _bgmSourceA : _bgmSourceB;
            if (active != null && active.isPlaying) active.volume = volume;
        }

        private void PlayBGM(AudioClip newClip, float targetVolume)
        {
            if (newClip == null) return;
            if (TimerManager.Instance == null)
            {
                Debug.LogError("[AudioManager] TimerManager.Instance is null. Ensure TimerSystem is initialized in this scene.", this);
                return;
            }
            if (_bgmSourceA == null || _bgmSourceB == null) EnsureBgmSourcesReady();
            if (_bgmSourceA == null || _bgmSourceB == null) return;

            if (bgmCrossfadeDuration < 0f) bgmCrossfadeDuration = 0f;
            targetVolume = Mathf.Clamp01(targetVolume);

            AudioSource activeSource = _isUsingSourceA ? _bgmSourceA : _bgmSourceB;
            AudioSource fadingSource = _isUsingSourceA ? _bgmSourceB : _bgmSourceA;

            if (activeSource.clip == newClip && activeSource.isPlaying) return;

            _isUsingSourceA = !_isUsingSourceA;
            fadingSource = activeSource;
            activeSource = _isUsingSourceA ? _bgmSourceA : _bgmSourceB;

            activeSource.clip = newClip;
            activeSource.volume = 0f;
            activeSource.Play();

            float startFadeOutVolume = fadingSource.volume;

            if (bgmCrossfadeDuration <= 0f)
            {
                if (fadingSource.isPlaying) fadingSource.Stop();
                activeSource.volume = targetVolume;
                return;
            }

            if (_crossfadeTimer != null && !_crossfadeTimer.IsDone) _crossfadeTimer.Cancel();

            _crossfadeTimer = TimerManager.Instance.Register(
                duration: bgmCrossfadeDuration,
                onComplete: () =>
                {
                    fadingSource.Stop();
                    activeSource.volume = targetVolume;
                },
                isLooping: false,
                useUnscaledTime: true,
                onUpdate: t =>
                {
                    activeSource.volume = Mathf.Lerp(0f, targetVolume, t);
                    if (fadingSource.isPlaying)
                        fadingSource.volume = Mathf.Lerp(startFadeOutVolume, 0f, t);
                }
            );
        }

        // ================= Observability =================

        private void LogCommand(AudioCommand cmd)
        {
            if (RecentCommands == null) RecentCommands = new List<string>();
            if (RecentCommands.Count > 15) RecentCommands.RemoveAt(0);
            RecentCommands.Add($"[{System.DateTime.Now:HH:mm:ss}] {cmd.CmdType} | {cmd.Type} | {cmd.AudioKey} | {cmd.Concurrency} | Fade={cmd.FadeDuration:0.###}");
        }

        // ================= FMOD-safe BGM source setup =================

        private void EnsureBgmSourcesReady()
        {
            // Try auto-bind from children (runtime-safe, no AddComponent)
            if (_bgmSourceA == null || _bgmSourceB == null)
            {
                var a = transform.Find("BGM_Source_A");
                var b = transform.Find("BGM_Source_B");
                if (a != null) _bgmSourceA = a.GetComponent<AudioSource>();
                if (b != null) _bgmSourceB = b.GetComponent<AudioSource>();
            }

            if (_bgmSourceA == null || _bgmSourceB == null)
            {
                Debug.LogError("[AudioManager] BGM AudioSources are not configured. Please assign _bgmSourceA/_bgmSourceB or let OnValidate auto-create child sources (BGM_Source_A/B).", this);
                return;
            }

            _bgmSourceA.loop = true;
            _bgmSourceB.loop = true;
            _bgmSourceA.playOnAwake = false;
            _bgmSourceB.playOnAwake = false;
        }

#if UNITY_EDITOR
        private void EnsureBgmSourcesExistInEditor()
        {
            if (_bgmSourceA == null) _bgmSourceA = GetOrCreateChildAudioSource("BGM_Source_A");
            if (_bgmSourceB == null) _bgmSourceB = GetOrCreateChildAudioSource("BGM_Source_B");
        }

        private AudioSource GetOrCreateChildAudioSource(string childName)
        {
            var t = transform.Find(childName);
            if (t == null)
            {
                var go = new GameObject(childName);
                go.transform.SetParent(transform, false);
                t = go.transform;
            }

            var src = t.GetComponent<AudioSource>();
            if (src == null) src = t.gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            return src;
        }
#endif
    }
}