using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace WSH._01.Scripts
{
    public class TileControl : MonoBehaviour
    {
        [SerializeField] private float _currentRotationZ;
        [SerializeField] public bool _rotating;
        
        private void OnMouseDown()
        {
            if (_rotating) return;
            
            _rotating = true;
            transform.DORotate(new Vector3(transform.rotation.x,transform.rotation.y ,_currentRotationZ - 90),0.5f)
                .SetEase(Ease.OutQuint)
                .OnComplete(() => _currentRotationZ = 90).OnComplete(() => _rotating = false);
        }
    }
}