using UnityEngine;

namespace EW_Framework.Core.StateMachine.Examples
{
    /// <summary>
    /// Presentation only: tints the chaser by its current state and draws the chase-enter range as a ring,
    /// so state transitions are visible at a glance. Colours are set at runtime, so it works with any render pipeline.
    /// </summary>
    [RequireComponent(typeof(StateMachineExampleDriver))]
    public sealed class StateMachineExampleVisuals : MonoBehaviour
    {
        [SerializeField] private Renderer _chaserRenderer;
        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private Color _targetColor = new(0.25f, 0.55f, 1f);
        [SerializeField, Min(0f)] private float _colorBlendSpeed = 8f;

        private const int RingSegments = 64;

        private StateMachineExampleDriver _driver;
        private Material _chaserMaterial;
        private Material _targetMaterial;
        private Material _ringMaterial;
        private LineRenderer _ring;

        public static Color ColorFor(string stateName) => stateName switch
        {
            "IdleState" => new Color(0.6f, 0.6f, 0.65f),
            "PatrolState" => new Color(0.3f, 0.8f, 0.4f),
            "ChaseState" => new Color(0.95f, 0.3f, 0.25f),
            "PauseState" => new Color(1f, 0.8f, 0.2f),
            _ => Color.white
        };

        private void Awake()
        {
            _driver = GetComponent<StateMachineExampleDriver>();
            if (_chaserRenderer == null) _chaserRenderer = GetComponent<Renderer>();

            // Renderer.material creates a per-object instance; destroyed in OnDestroy.
            if (_chaserRenderer != null) _chaserMaterial = _chaserRenderer.material;
            if (_targetRenderer != null)
            {
                _targetMaterial = _targetRenderer.material;
                _targetMaterial.color = _targetColor;
            }

            _ringMaterial = new Material(Shader.Find("Sprites/Default"));
            var ringObject = new GameObject("ChaseRangeRing");
            ringObject.transform.SetParent(transform, false);
            _ring = ringObject.AddComponent<LineRenderer>();
            _ring.sharedMaterial = _ringMaterial;
            _ring.loop = true;
            _ring.useWorldSpace = false;
            _ring.positionCount = RingSegments;
            _ring.widthMultiplier = 0.06f;
            _ring.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void Update()
        {
            Color stateColor = ColorFor(_driver.CurrentStateName);

            if (_chaserMaterial != null)
            {
                _chaserMaterial.color = Color.Lerp(_chaserMaterial.color, stateColor, Time.deltaTime * _colorBlendSpeed);
            }

            // Ring sits on the ground (objects are unit primitives centred at y = 0).
            float radius = _driver.ChaseEnterRange;
            for (int i = 0; i < RingSegments; i++)
            {
                float a = i * Mathf.PI * 2f / RingSegments;
                _ring.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, -0.48f, Mathf.Sin(a) * radius));
            }
            _ring.startColor = _ring.endColor = new Color(stateColor.r, stateColor.g, stateColor.b, 0.8f);
        }

        private void OnDestroy()
        {
            if (_chaserMaterial != null) Destroy(_chaserMaterial);
            if (_targetMaterial != null) Destroy(_targetMaterial);
            if (_ringMaterial != null) Destroy(_ringMaterial);
        }
    }
}
