using System;
using UnityEngine;

namespace RYU.Scirpt.Player
{
    public class View : PlayerModule
    {
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private Vector2 pitchLimits = new(-80f, 80f);

        private Transform ViewRoot => transform;

        private float _currentPitch;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            _currentPitch -= Controller.LookInput.y * sensitivity;
            _currentPitch = Mathf.Clamp(_currentPitch, pitchLimits.x, pitchLimits.y);
            
            ViewRoot.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);
            
            Root.Rotate(Vector3.up * (sensitivity * Controller.LookInput.x));
        }
    }
}