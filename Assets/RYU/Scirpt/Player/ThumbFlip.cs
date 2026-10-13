using UnityEngine;
using UnityEngine.InputSystem;

namespace RYU.Scirpt.Player
{
    // 따봉 <-> 역따봉 회전. easeOutCirc로 처음엔 빠르게 돌고 끝에서 부드럽게 멈춘다.
    public class ThumbFlip : MonoBehaviour
    {
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private float reversedAngle = 180f; // 반대로 돌리고 싶으면 -180

        public bool IsReversed { get; private set; }

        private Quaternion _baseRotation;
        private float _from, _to, _current, _elapsed;

        private void Awake()
        {
            _baseRotation = transform.localRotation;
            _elapsed = duration; // 시작할 땐 멈춘 상태
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.aKey.wasPressedThisFrame)
                SetReversed(!IsReversed);

            if (_elapsed >= duration) return;

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / duration);
            _current = Mathf.Lerp(_from, _to, EaseOutCirc(t));
            transform.localRotation = _baseRotation * Quaternion.Euler(0f, 0f, _current);
        }

        public void SetReversed(bool reversed)
        {
            IsReversed = reversed;
            _from = _current; // 도는 중에 다시 눌러도 지금 각도에서 이어서 돈다
            _to = reversed ? reversedAngle : 0f;
            _elapsed = 0f;
        }

        // https://easings.net/#easeOutCirc
        private static float EaseOutCirc(float x) => Mathf.Sqrt(1f - (x - 1f) * (x - 1f));
    }
}
