using UnityEngine;

namespace EW_Framework.Modules.AudioSystem.Examples
{
    public class AudioSystemExampleFollowTargetMover : MonoBehaviour
    {
        [Header("Motion")]
        public Vector3 center = Vector3.zero;
        public float radius = 2f;
        public float angularSpeed = 90f;
        public bool useUnscaledTime = true;

        private float _angleDeg;

        private void OnEnable()
        {
            _angleDeg = 0f;
        }

        private void Update()
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _angleDeg += angularSpeed * dt;
            float rad = _angleDeg * Mathf.Deg2Rad;

            transform.position = center + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
        }
    }
}

