using UnityEngine;

namespace EW_Framework.Core.StateMachine.Examples
{
    public sealed class StateMachineExampleHud : MonoBehaviour
    {
        [SerializeField] private StateMachineExampleDriver _driver;

        // Layout is authored for a 720p-tall Game view and scaled to the actual height.
        private const float ReferenceHeight = 720f;

        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _stateStyle;
        private Texture2D _panelTexture;

        private void OnGUI()
        {
            if (_driver == null) return;

            // IMGUI APIs (including GUI.skin) must be called from within OnGUI.
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold };
                _bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 18 };
                _stateStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
                _panelTexture = new Texture2D(1, 1);
                _panelTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
                _panelTexture.Apply();
            }

            float scale = Screen.height / ReferenceHeight;
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));

            const float pad = 16f;
            const float width = 620f;
            float x = pad;
            float y = pad;

            int lines = _driver.RecentTransitions.Count;
            float height = 150f + lines * 24f;
            GUI.DrawTexture(new Rect(x - 8f, y - 6f, width, height), _panelTexture);

            GUI.Label(new Rect(x, y, width, 34f), "StateMachine Example", _titleStyle);
            y += 38f;

            string state = _driver.CurrentStateName;
            _stateStyle.normal.textColor = StateMachineExampleVisuals.ColorFor(state);
            GUI.Label(new Rect(x, y, width, 28f), $"State: {state}", _stateStyle);
            y += 30f;

            GUI.Label(new Rect(x, y, width, 24f),
                $"Distance: {_driver.DistanceToTarget():0.0}   (chase < {_driver.ChaseEnterRange:0}, lose > {_driver.ChaseLoseRange:0})",
                _bodyStyle);
            y += 28f;

            foreach (string transition in _driver.RecentTransitions)
            {
                GUI.Label(new Rect(x, y, width, 24f), transition, _bodyStyle);
                y += 24f;
            }

            y += 4f;
            GUI.Label(new Rect(x, y, width, 24f), "WASD / Arrows: move target    Esc: pause (Push / Pop)", _bodyStyle);

            GUI.matrix = previous;
        }

        private void OnDestroy()
        {
            if (_panelTexture != null) Destroy(_panelTexture);
        }
    }
}
