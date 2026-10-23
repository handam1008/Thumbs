using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace WSH._01.Scripts
{
    public class TileControl : MonoBehaviour
    {
        private PuzzleProcess _puzzleProcess;
        
        [field:SerializeField] public float currentRotationZ {get; private set;}
        [SerializeField] public bool _rotating;

        private void Awake()
        {
            _puzzleProcess = GetComponentInParent<PuzzleProcess>();
        }

        // 시작 각도를 화면 회전과 판정값(currentRotationZ)에 똑같이 넣는다
        public void Init(float z)
        {
            currentRotationZ = z;
            transform.rotation = Quaternion.Euler(0, 0, z);
        }

        private void OnMouseDown()
        {
            if (_rotating) return;
            _rotating = true;
            
            currentRotationZ -= 90;
            if (currentRotationZ == -270) currentRotationZ = 90;
            
            transform.DORotate(new Vector3(transform.rotation.x, transform.rotation.y, currentRotationZ), 0.5f)
                .SetEase(Ease.OutQuint)
                .OnComplete(() =>
                {
                    _rotating = false;
                });
            _puzzleProcess.Proccess();
        }
    }
}