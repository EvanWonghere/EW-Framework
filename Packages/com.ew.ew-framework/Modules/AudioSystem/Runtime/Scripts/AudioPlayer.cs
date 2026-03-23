using UnityEngine;
using EW_Framework.Core.ObjectPool.Base;
using EW_Framework.Core.TimerSystem;
using EW_Framework.Core.ObjectPool.Manager;

namespace EW_Framework.Modules.AudioSystem.Runtime
{

    /// <summary>
    /// Audio Player Component
    /// Mounted on the AudioSource Prefab
    /// Implements IPoolable, supports spawning and returning from the pool
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, IPoolable
    {
        private AudioSource _audioSource;
        private Timer _despawnTimer;
        private Timer _fadeTimer;
        private Transform _followTarget;
        private bool _is3D;

        public string CurrentKey { get; private set; } = string.Empty;
        public AudioType CurrentType { get; private set; } = AudioType.Other;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_is3D && _followTarget != null)
            {
                // Unity overrides == null for destroyed objects; also guard against inactive targets
                if (!_followTarget.gameObject.activeInHierarchy)
                {
                    if (SyncPoolManager.Instance == null)
                    {
                        Debug.LogError("[AudioPlayer] Follow target became inactive, but SyncPoolManager.Instance is null. Cannot despawn.", this);
                        _followTarget = null;
                        return;
                    }
                    SyncPoolManager.Instance.Despawn(gameObject);
                    return;
                }
                transform.position = _followTarget.position;
            }
        }

        public void OnSpawn()
        {
            _audioSource.playOnAwake = false;
        }

        public void OnDespawn()
        {
            var oldKey = CurrentKey;

            // Clear logic when returning to the object pool
            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.outputAudioMixerGroup = null;
            _followTarget = null;
            CurrentKey = string.Empty;
            CurrentType = AudioType.Other;
            _is3D = false;

            // If the timer is still running when returning to the object pool (e.g. forced scene switch), cancel it
            if (_despawnTimer != null && !_despawnTimer.IsDone)
            {
                _despawnTimer.Cancel();
                _despawnTimer = null;
            }
            if (_fadeTimer != null && !_fadeTimer.IsDone)
            {
                _fadeTimer.Cancel();
                _fadeTimer = null;
            }

            if (!string.IsNullOrEmpty(oldKey) && AudioManager.Instance != null)
            {
                AudioManager.Instance.UnregisterPlayer(oldKey, this);
            }
        }

        /// <summary>
        /// Execute audio command on this player.
        /// </summary>
        public void ExecuteCommand(AudioCommand cmd)
        {
            switch (cmd.CmdType)
            {
                case AudioCommandType.Play:
                    Play(cmd);
                    break;
                case AudioCommandType.Stop:
                case AudioCommandType.StopByKey:
                case AudioCommandType.StopAll:
                case AudioCommandType.StopByType:
                    if (SyncPoolManager.Instance == null)
                    {
                        Debug.LogError("[AudioPlayer] SyncPoolManager.Instance is null. Cannot despawn audio player.", this);
                        return;
                    }
                    StopAndDespawn(cmd.FadeDuration);
                    break;
                case AudioCommandType.Pause:
                    _audioSource.Pause();
                    _despawnTimer?.Pause();
                    _fadeTimer?.Pause();
                    break;
                case AudioCommandType.Resume:
                    _audioSource.UnPause();
                    _despawnTimer?.Resume();
                    _fadeTimer?.Resume();
                    break;
                case AudioCommandType.SetVolume:
                    _audioSource.volume = Mathf.Clamp01(cmd.Volume);
                    break;
            }
        }

        private void StopAndDespawn(float fadeDuration)
        {
            if (SyncPoolManager.Instance == null)
            {
                Debug.LogError("[AudioPlayer] SyncPoolManager.Instance is null. Cannot despawn audio player.", this);
                return;
            }

            fadeDuration = Mathf.Max(0f, fadeDuration);
            if (fadeDuration <= 0f)
            {
                SyncPoolManager.Instance.Despawn(gameObject);
                return;
            }
            if (TimerManager.Instance == null)
            {
                Debug.LogError("[AudioPlayer] TimerManager.Instance is null. Cannot fade out; despawning immediately.", this);
                SyncPoolManager.Instance.Despawn(gameObject);
                return;
            }

            if (_fadeTimer != null && !_fadeTimer.IsDone) _fadeTimer.Cancel();
            float start = _audioSource.volume;
            _fadeTimer = TimerManager.Instance.Register(
                duration: fadeDuration,
                onComplete: () => SyncPoolManager.Instance.Despawn(gameObject),
                isLooping: false,
                useUnscaledTime: true,
                onUpdate: t => { _audioSource.volume = Mathf.Lerp(start, 0f, t); }
            );
        }

        private void Play(AudioCommand cmd)
        {
            if (_despawnTimer != null && !_despawnTimer.IsDone)
            {
                _despawnTimer.Cancel();
                _despawnTimer = null;
            }
            if (_fadeTimer != null && !_fadeTimer.IsDone)
            {
                _fadeTimer.Cancel();
                _fadeTimer = null;
            }

            CurrentKey = cmd.AudioKey ?? string.Empty;
            CurrentType = cmd.Type;
            _followTarget = null;
            _is3D = cmd.Is3D;

            _audioSource.clip = cmd.Clip;
            float targetVolume = Mathf.Clamp01(cmd.Volume);
            _audioSource.pitch = Mathf.Max(0f, cmd.Pitch <= 0f ? 1f : cmd.Pitch);
            _audioSource.loop = cmd.Loop;
            _audioSource.outputAudioMixerGroup = cmd.MixerGroup;

            _audioSource.spatialBlend = cmd.Is3D ? 1f : 0f;
            if (cmd.Is3D)
            {
                if (cmd.FollowTarget != null) _followTarget = cmd.FollowTarget;
                else transform.position = cmd.FixedPosition;
            }

            if (_audioSource.clip == null)
            {
                Debug.LogWarning("[AudioPlayer] Clip is null. Skip playing.", this);
                return;
            }

            float fadeDuration = Mathf.Max(0f, cmd.FadeDuration);
            if (fadeDuration > 0f && TimerManager.Instance != null)
            {
                _audioSource.volume = 0f;
                float start = 0f;
                _fadeTimer = TimerManager.Instance.Register(
                    duration: fadeDuration,
                    onComplete: () => { _audioSource.volume = targetVolume; },
                    isLooping: false,
                    useUnscaledTime: true,
                    onUpdate: t => { _audioSource.volume = Mathf.Lerp(start, targetVolume, t); }
                );
            }
            else
            {
                _audioSource.volume = targetVolume;
            }

            _audioSource.Play();

            if (!cmd.Loop)
            {
                if (TimerManager.Instance == null)
                {
                    Debug.LogError("[AudioPlayer] TimerManager.Instance is null. Cannot schedule despawn; audio player may leak in pool.", this);
                    return;
                }
                if (SyncPoolManager.Instance == null)
                {
                    Debug.LogError("[AudioPlayer] SyncPoolManager.Instance is null. Cannot despawn audio player.", this);
                    return;
                }

                _despawnTimer = TimerManager.Instance.Register(
                    duration: _audioSource.clip.length,
                    onComplete: () => SyncPoolManager.Instance.Despawn(gameObject),
                    useUnscaledTime: true
                );
            }
        }
    }
}