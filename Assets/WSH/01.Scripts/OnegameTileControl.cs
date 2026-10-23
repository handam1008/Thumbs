using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace WSH._01.Scripts
{
    public class OnegameTileControl : MonoBehaviour
    {
        private OnegamePuzzleProcess _puzzleProcess;
        [SerializeField] private float angle = 360f;

        [field: SerializeField] public float currentRotationZ { get; private set; }
        public bool _rotating;
        public bool _complete;

        private void Awake()
        {
            _puzzleProcess = GetComponentInParent<OnegamePuzzleProcess>();
        }

        public bool IsCorret(float answer)
        {
            float diff = Mathf.Repeat(currentRotationZ - answer, angle);
            return diff < 0.01f || diff > angle - 0.01f;
        }

        public void Init(float z)
        {
            currentRotationZ = z;
            transform.rotation = Quaternion.Euler(0, 0, z);
        }

    private void OnMouseDown()
        {
            if (_rotating || _complete) return;
            _rotating = true;
            
            currentRotationZ -= 90;
            if (currentRotationZ == -270) currentRotationZ = 90;
            
            transform.DORotate(new Vector3(0, 0, currentRotationZ), 0.5f)
                .SetEase(Ease.OutQuint)
                .OnComplete(() =>
                {
                    _rotating = false;
                });
            _puzzleProcess.Proccess();
        }
    }
}