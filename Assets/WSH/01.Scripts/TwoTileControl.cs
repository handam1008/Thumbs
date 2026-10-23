using DG.Tweening;
using UnityEngine;

namespace WSH._01.Scripts
{
    public class TwogameTileControl : MonoBehaviour
    {
        private TwogamePuzzleProcess _puzzleProcess;

        [field: SerializeField] public float currentRotationZ { get; private set; }
        public bool _rotating;
        public bool _complete;

        private void Awake()
        {
            _puzzleProcess = GetComponentInParent<TwogamePuzzleProcess>();
        }

        public bool IsWrong(float answer)
        {
            if(answer != currentRotationZ) return true;
            return false;
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
