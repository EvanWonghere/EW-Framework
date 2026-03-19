using System;
using UnityEngine;
using EW_Framework.Modules.AudioSystem.Runtime;
using AudioType = EW_Framework.Modules.AudioSystem.Runtime.AudioType;

namespace EW_Framework.Modules.AudioSystem.Examples
{
    public class AudioSystemExamplePanel : MonoBehaviour
    {
        [Header("Channel")]
        public AudioRequestChannelSO channel;

        [Header("Clips")]
        public AudioClip bgmClip;
        public AudioClip sfxClip;
        public AudioClip uiClip;
        public AudioClip voiceClip;

        [Header("Follow Target (optional)")]
        public Transform followTarget;

        [Header("Command Defaults")]
        public string sfxKey = "SFX_Key_01";
        public string uiKey = "UI_Key_01";
        public string voiceKey = "Voice_Key_01";
        [Range(0f, 1f)] public float volume = 1f;
        [Min(0f)] public float fadeDuration = 0.15f;
        public ConcurrencyPolicy concurrency = ConcurrencyPolicy.PlayAdditive;

        private Rect _windowRect = new Rect(20, 20, 440, 520);
        private Vector2 _scroll;
        private float _pitch = 1f;
        private bool _loop;

        private void OnGUI()
        {
            _windowRect = GUI.Window(GetInstanceID(), _windowRect, DrawWindow, "AudioSystem Example Panel");
        }

        private void DrawWindow(int id)
        {
            float viewHeight = Mathf.Max(100f, _windowRect.height - 34f); // leave room for title bar + drag
            _scroll = GUILayout.BeginScrollView(_scroll, false, true, GUILayout.Height(viewHeight));

            GUILayout.Label(channel == null ? "Channel: (未配置)" : $"Channel: {channel.name}");
            GUILayout.Space(6);

            GUILayout.Label("Defaults");
            sfxKey = TextFieldRow("SFX Key", sfxKey);
            uiKey = TextFieldRow("UI Key", uiKey);
            voiceKey = TextFieldRow("Voice Key", voiceKey);

            volume = SliderRow("Volume", volume, 0f, 1f);
            _pitch = SliderRow("Pitch", _pitch, 0.2f, 2f);
            fadeDuration = SliderRow("Fade(s)", fadeDuration, 0f, 2f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Loop", GUILayout.Width(80));
            _loop = GUILayout.Toggle(_loop, _loop ? "On" : "Off", GUILayout.Width(80));
            GUILayout.EndHorizontal();

            concurrency = EnumRow("Concurrency", concurrency);

            GUILayout.Space(10);
            GUILayout.Label("BGM");
            if (GUILayout.Button("Play BGM (fade ignored, uses crossfade)")) RaiseSafe(new AudioCommand
            {
                CmdType = AudioCommandType.Play,
                Type = AudioType.BGM,
                Clip = bgmClip,
                Volume = volume,
                Pitch = 1f,
                Loop = true,
            });
            if (GUILayout.Button("Stop BGM")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.Stop, Type = AudioType.BGM, FadeDuration = fadeDuration });
            if (GUILayout.Button("Pause BGM")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.Pause, Type = AudioType.BGM });
            if (GUILayout.Button("Resume BGM")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.Resume, Type = AudioType.BGM });
            if (GUILayout.Button("Set BGM Volume")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.SetVolume, Type = AudioType.BGM, Volume = volume });

            GUILayout.Space(10);
            GUILayout.Label("SFX (2D / 3D / Follow)");
            if (GUILayout.Button("Play 2D SFX")) RaiseSafe(AudioCommand.Play2D(sfxKey, sfxClip, AudioType.SFX, volume, _loop, null, _pitch, concurrency, fadeDuration));
            if (GUILayout.Button("Play Fixed 3D SFX (at this object pos)")) RaiseSafe(AudioCommand.PlayFixed3D(sfxKey, sfxClip, transform.position, AudioType.SFX, volume, _loop, null, _pitch, concurrency, fadeDuration));
            if (GUILayout.Button("Play Follow 3D SFX")) RaiseSafe(AudioCommand.PlayFollow3D(sfxKey, sfxClip, followTarget, AudioType.SFX, volume, _loop, null, _pitch, concurrency, fadeDuration));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("StopByKey")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.StopByKey, Type = AudioType.SFX, AudioKey = sfxKey, FadeDuration = fadeDuration });
            if (GUILayout.Button("PauseByKey")) RaiseSafe(AudioCommand.PauseByKey(sfxKey, AudioType.SFX));
            if (GUILayout.Button("ResumeByKey")) RaiseSafe(AudioCommand.ResumeByKey(sfxKey, AudioType.SFX));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("SetVolumeByKey")) RaiseSafe(AudioCommand.SetVolumeByKey(sfxKey, volume, AudioType.SFX));
            if (GUILayout.Button("StopAll SFX")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.StopAll, Type = AudioType.SFX, FadeDuration = fadeDuration });
            if (GUILayout.Button("StopByType (SFX)")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.StopByType, Type = AudioType.SFX, FadeDuration = fadeDuration });

            GUILayout.Space(10);
            GUILayout.Label("UI / Voice (as SFX types)");
            if (GUILayout.Button("Play UI")) RaiseSafe(AudioCommand.Play2D(uiKey, uiClip, AudioType.UI, volume, false, null, 1f, concurrency, fadeDuration));
            if (GUILayout.Button("Play Voice")) RaiseSafe(AudioCommand.Play2D(voiceKey, voiceClip, AudioType.Voice, volume, false, null, 1f, concurrency, fadeDuration));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stop UI (ByKey)")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.StopByKey, Type = AudioType.UI, AudioKey = uiKey, FadeDuration = fadeDuration });
            if (GUILayout.Button("Stop Voice (ByKey)")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.StopByKey, Type = AudioType.Voice, AudioKey = voiceKey, FadeDuration = fadeDuration });
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause UI (ByKey)")) RaiseSafe(AudioCommand.PauseByKey(uiKey, AudioType.UI));
            if (GUILayout.Button("Resume UI (ByKey)")) RaiseSafe(AudioCommand.ResumeByKey(uiKey, AudioType.UI));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause Voice (ByKey)")) RaiseSafe(AudioCommand.PauseByKey(voiceKey, AudioType.Voice));
            if (GUILayout.Button("Resume Voice (ByKey)")) RaiseSafe(AudioCommand.ResumeByKey(voiceKey, AudioType.Voice));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SetVol UI (ByKey)")) RaiseSafe(AudioCommand.SetVolumeByKey(uiKey, volume, AudioType.UI));
            if (GUILayout.Button("SetVol Voice (ByKey)")) RaiseSafe(AudioCommand.SetVolumeByKey(voiceKey, volume, AudioType.Voice));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("StopByType UI")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.StopByType, Type = AudioType.UI, FadeDuration = fadeDuration });
            if (GUILayout.Button("StopByType Voice")) RaiseSafe(new AudioCommand { CmdType = AudioCommandType.StopByType, Type = AudioType.Voice, FadeDuration = fadeDuration });
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            GUILayout.Label("Tips");
            GUILayout.Label("- FollowTarget 失活时，AudioPlayer 会自动回收");
            GUILayout.Label("- Concurrency=Override/Ignore 仅对同 Key 的活跃列表生效");

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void RaiseSafe(AudioCommand cmd)
        {
            if (channel == null)
            {
                Debug.LogWarning("[AudioSystemExample] channel is not assigned.", this);
                return;
            }
            channel.Raise(cmd);
        }

        private static string TextFieldRow(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(80));
            value = GUILayout.TextField(value ?? string.Empty);
            GUILayout.EndHorizontal();
            return value;
        }

        private static float SliderRow(string label, float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}: {value:0.###}", GUILayout.Width(160));
            value = GUILayout.HorizontalSlider(value, min, max);
            GUILayout.EndHorizontal();
            return value;
        }

        private static T EnumRow<T>(string label, T value) where T : Enum
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(120));
            var names = Enum.GetNames(typeof(T));
            int idx = Array.IndexOf(names, value.ToString());
            idx = Mathf.Clamp(idx, 0, names.Length - 1);
            int newIdx = GUILayout.SelectionGrid(idx, names, 3);
            GUILayout.EndHorizontal();
            return (T)Enum.Parse(typeof(T), names[newIdx]);
        }
    }
}

