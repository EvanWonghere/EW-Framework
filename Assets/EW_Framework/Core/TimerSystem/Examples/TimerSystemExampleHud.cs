using UnityEngine;

namespace EW_Framework.Core.TimerSystem.Examples
{
    public sealed class TimerSystemExampleHud : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TimerSystemExampleDriver _driver;

        [Header("Layout")]
        [SerializeField, Min(10f)] private float _width = 760f;
        [SerializeField, Min(10f)] private float _height = 560f;
        [SerializeField, Min(8f)] private float _padding = 14f;

        [Header("Typography")]
        [SerializeField, Min(10)] private int _titleFontSize = 22;
        [SerializeField, Min(8)] private int _textFontSize = 16;
        [SerializeField, Min(8)] private int _smallFontSize = 14;

        private GUIStyle _titleStyle;
        private GUIStyle _textStyle;
        private GUIStyle _smallStyle;

        private void EnsureStyles()
        {
            if (_titleStyle != null && _textStyle != null && _smallStyle != null) return;

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Max(10, _titleFontSize),
                fontStyle = FontStyle.Bold
            };

            _textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Max(8, _textFontSize)
            };

            _smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Max(8, _smallFontSize),
                normal = { textColor = new Color(1f, 1f, 1f, 0.85f) }
            };
        }

        private void OnValidate()
        {
            _width = Mathf.Max(10f, _width);
            _height = Mathf.Max(10f, _height);
            _padding = Mathf.Max(0f, _padding);

            _titleFontSize = Mathf.Max(10, _titleFontSize);
            _textFontSize = Mathf.Max(8, _textFontSize);
            _smallFontSize = Mathf.Max(8, _smallFontSize);

            _titleStyle = null;
            _textStyle = null;
            _smallStyle = null;
        }

        private void OnGUI()
        {
            EnsureStyles();

            Rect rect = new Rect(_padding, _padding, _width, _height);
            GUI.Box(rect, GUIContent.none);

            GUILayout.BeginArea(new Rect(rect.x + _padding, rect.y + _padding, rect.width - _padding * 2f, rect.height - _padding * 2f));

            GUILayout.Label("TimerSystem Demo HUD", _titleStyle);
            GUILayout.Label($"timeScale: {Time.timeScale:0.###} | unscaledTime: {Time.unscaledTime:0.000}", _smallStyle);
            GUILayout.Space(6f);

            GUILayout.Label("Keys: [1] scaled delay, [2] unscaled delay, [3] loop, [P] pause/resume last, [C] cancel last, [R] clear all, [T] toggle timeScale", _smallStyle);
            GUILayout.Space(10f);

            if (_driver == null)
            {
                GUILayout.Label("Driver is null. Assign TimerSystemExampleDriver in Inspector.", _textStyle);
                GUILayout.EndArea();
                return;
            }

            Timer last = _driver.LastTimer;
            string lastState = last == null
                ? "<None>"
                : $"Dur={last.Duration:0.###} Elapsed={last.TimeElapsed:0.###} Paused={last.IsPaused} Done={last.IsDone} Cancelled={last.IsCancelled} Loop={last.IsLooping} Unscaled={last.UseUnscaledTime}";
            GUILayout.Label($"LastTimer: {lastState}", _textStyle);
            GUILayout.Space(8f);

            var timers = _driver.Timers;
            GUILayout.Label($"Active handles tracked by demo: {timers?.Count ?? 0}", _textStyle);
            if (timers != null)
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    Timer t = timers[i];
                    if (t == null)
                    {
                        GUILayout.Label($"- [{i}] <null>", _smallStyle);
                        continue;
                    }

                    float progress = t.Duration <= 0f ? 1f : Mathf.Clamp01(t.TimeElapsed / t.Duration);
                    GUILayout.Label(
                        $"- [{i}] {(t.UseUnscaledTime ? "Unscaled" : "Scaled")} {(t.IsLooping ? "Loop" : "Once")} " +
                        $"progress={progress:0.00} elapsed={t.TimeElapsed:0.###}/{t.Duration:0.###} paused={t.IsPaused}",
                        _smallStyle
                    );
                }
            }

            GUILayout.Space(10f);
            GUILayout.Label("Recent log:", _textStyle);
            var logs = _driver.LogLines;
            if (logs != null)
            {
                for (int i = 0; i < logs.Count; i++)
                {
                    GUILayout.Label(logs[i], _smallStyle);
                }
            }

            GUILayout.EndArea();
        }
    }
}

