using UnityEngine;
using WSH._09.SO;
using Random = UnityEngine.Random;

namespace WSH._01.Scripts
{
    public class PuzzleProcess : MonoBehaviour
    {
        [SerializeField] private CorrectPuzzleDataSO[] _correctPuzzleDataSO;
        
        [SerializeField] private int _randomValue;

        [SerializeField] private bool _proccessing;
        [SerializeField] private bool _complete;

        private void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            _randomValue = Random.Range(0, _correctPuzzleDataSO.Length);

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void Update()
        {
            if (!_proccessing) return;
            StopProccess();

            for (int i = 0; i < _correctPuzzleDataSO[_randomValue].correctPuzzleData.Length; i++)
            {
                if (transform.GetChild(i).TryGetComponent<TileControl>(out TileControl tileControl))
                {
                    if (tileControl.currentRotationZ != _correctPuzzleDataSO[_randomValue].correctPuzzleData[i])
                    {
                        _complete = false;
                        break;
                    }
                    else
                    {
                        _complete = true;
                    }
                }
            }

            if (_complete) EndMiniGame();
        }

        public void EndMiniGame()
        {
            Debug.Log(123);
        }
        
        public void Proccess()
        {
            _proccessing = true;
        }

        public void StopProccess()
        {
            _proccessing = false;
        }
    }
}
