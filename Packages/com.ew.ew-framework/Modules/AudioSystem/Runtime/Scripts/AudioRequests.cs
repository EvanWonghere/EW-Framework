using System;
using UnityEngine;
using UnityEngine.Audio;

namespace EW_Framework.Modules.AudioSystem.Runtime
{
    public enum AudioType
    {
        BGM,
        SFX,
        Voice,
        UI,
        Other,
    }

    // Extended command semantics
    public enum AudioCommandType
    {
        Play,
        Stop,
        StopByKey,
        StopAll,
        StopByType,
        Pause,
        Resume,
        SetVolume,
    }

    // AAA-grade concurrency policy for same-key SFX
    public enum ConcurrencyPolicy
    {
        PlayAdditive, // allow overlap (default)
        Override,     // stop existing, play new
        Ignore,       // if existing still playing, ignore new
    }

    [Serializable]
    public struct AudioCommand
    {
        public AudioCommandType CmdType;
        public string AudioKey;

        [Header("Play Settings")]
        public AudioClip Clip;
        public AudioType Type;
        public AudioMixerGroup MixerGroup;
        [Range(0f, 1f)]
        public float Volume;
        public float Pitch;
        public bool Loop;

        [Header("3D Tracking Settings")]
        public bool Is3D;
        public Vector3 FixedPosition;
        public Transform FollowTarget;

        // New options
        public ConcurrencyPolicy Concurrency;
        public float FadeDuration;

        // ===== Static Factory Methods =====

        public static AudioCommand Play2D(
            string key,
            AudioClip clip,
            AudioType type = AudioType.SFX,
            float volume = 1f,
            bool loop = false,
            AudioMixerGroup mixerGroup = null,
            float pitch = 1f,
            ConcurrencyPolicy concurrency = ConcurrencyPolicy.PlayAdditive,
            float fadeDuration = 0f)
        {
            return new AudioCommand
            {
                CmdType = AudioCommandType.Play,
                AudioKey = key ?? string.Empty,
                Clip = clip,
                Type = type,
                MixerGroup = mixerGroup,
                Volume = volume,
                Pitch = pitch,
                Loop = loop,
                Is3D = false,
                FixedPosition = Vector3.zero,
                FollowTarget = null,
                Concurrency = concurrency,
                FadeDuration = fadeDuration,
            };
        }

        public static AudioCommand PlayFixed3D(
            string key,
            AudioClip clip,
            Vector3 position,
            AudioType type = AudioType.SFX,
            float volume = 1f,
            bool loop = false,
            AudioMixerGroup mixerGroup = null,
            float pitch = 1f,
            ConcurrencyPolicy concurrency = ConcurrencyPolicy.PlayAdditive,
            float fadeDuration = 0f)
        {
            return new AudioCommand
            {
                CmdType = AudioCommandType.Play,
                AudioKey = key ?? string.Empty,
                Clip = clip,
                Type = type,
                MixerGroup = mixerGroup,
                Volume = volume,
                Pitch = pitch,
                Loop = loop,
                Is3D = true,
                FixedPosition = position,
                FollowTarget = null,
                Concurrency = concurrency,
                FadeDuration = fadeDuration,
            };
        }

        public static AudioCommand PlayFollow3D(
            string key,
            AudioClip clip,
            Transform target,
            AudioType type = AudioType.SFX,
            float volume = 1f,
            bool loop = false,
            AudioMixerGroup mixerGroup = null,
            float pitch = 1f,
            ConcurrencyPolicy concurrency = ConcurrencyPolicy.PlayAdditive,
            float fadeDuration = 0f)
        {
            return new AudioCommand
            {
                CmdType = AudioCommandType.Play,
                AudioKey = key ?? string.Empty,
                Clip = clip,
                Type = type,
                MixerGroup = mixerGroup,
                Volume = volume,
                Pitch = pitch,
                Loop = loop,
                Is3D = true,
                FixedPosition = Vector3.zero,
                FollowTarget = target,
                Concurrency = concurrency,
                FadeDuration = fadeDuration,
            };
        }

        public static AudioCommand StopByKey(string key, AudioType type = AudioType.SFX)
        {
            return new AudioCommand
            {
                CmdType = AudioCommandType.StopByKey,
                AudioKey = key ?? string.Empty,
                Clip = null,
                Type = type,
                MixerGroup = null,
                Volume = 1f,
                Pitch = 1f,
                Loop = false,
                Is3D = false,
                FixedPosition = Vector3.zero,
                FollowTarget = null,
                Concurrency = ConcurrencyPolicy.PlayAdditive,
                FadeDuration = 0f,
            };
        }

        public static AudioCommand StopAll(AudioType type = AudioType.SFX) => new()
        {
            CmdType = AudioCommandType.StopAll,
            AudioKey = string.Empty,
            Type = type,
        };

        public static AudioCommand StopByType(AudioType type) => new()
        {
            CmdType = AudioCommandType.StopByType,
            AudioKey = string.Empty,
            Type = type,
        };

        public static AudioCommand PauseByKey(string key, AudioType type = AudioType.SFX) => new()
        {
            CmdType = AudioCommandType.Pause,
            AudioKey = key ?? string.Empty,
            Type = type,
        };

        public static AudioCommand ResumeByKey(string key, AudioType type = AudioType.SFX) => new()
        {
            CmdType = AudioCommandType.Resume,
            AudioKey = key ?? string.Empty,
            Type = type,
        };

        public static AudioCommand SetVolumeByKey(string key, float volume, AudioType type = AudioType.SFX) => new()
        {
            CmdType = AudioCommandType.SetVolume,
            AudioKey = key ?? string.Empty,
            Type = type,
            Volume = volume,
        };
    }
}
