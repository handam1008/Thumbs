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
        
        public float currentRotationZ {get; private set;}
        [SerializeField] public bool _rotating;

        private void Awake()
        {
            _puzzleProcess = GetComponentInParent<PuzzleProcess>();
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