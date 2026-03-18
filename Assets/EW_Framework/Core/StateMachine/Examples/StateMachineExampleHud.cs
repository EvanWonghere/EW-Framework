using UnityEngine;

namespace EW_Framework.Core.StateMachine.Examples
{
    public sealed class StateMachineExampleHud : MonoBehaviour
    {
        [SerializeField] private StateMachineExampleDriver _driver;

        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;

        private void OnGUI()
        {
            if (_driver == null) return;

            // IMGUI APIs (including GUI.skin) must be called from within OnGUI.
            _titleStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            _bodyStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 14
            };

            const float pad = 12f;
            float x = pad;
            float y = pad;

            GUI.Label(new Rect(x, y, 520f, 28f), "StateMachine Example", _titleStyle);
            y += 30f;

            GUI.Label(new Rect(x, y, 520f, 22f), $"Current: {_driver.CurrentStateName}", _bodyStyle);
            y += 20f;

            GUI.Label(new Rect(x, y, 900f, 22f), $"Last: {_driver.LastTransition} @ t={_driver.LastTransitionTime:0.00}", _bodyStyle);
            y += 20f;

            GUI.Label(new Rect(x, y, 520f, 22f), $"DistanceToTarget: {_driver.DistanceToTarget():0.00}", _bodyStyle);
            y += 24f;

            GUI.Label(new Rect(x, y, 900f, 22f), "Controls: WASD/↑↓←→ move target | Esc toggle pause (Push/Pop)", _bodyStyle);
        }
    }
}

