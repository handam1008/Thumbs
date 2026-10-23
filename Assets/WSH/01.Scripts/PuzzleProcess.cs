using UnityEngine;
using WSH._09.SO;
using Random = UnityEngine.Random;

namespace WSH._01.Scripts
{
    public class PuzzleProcess : MonoBehaviour
    {
        [SerializeField] private PuzzleDataSO[] _correctPuzzleDataSO;
        [SerializeField] private PuzzleDataSO _firstPuzzleDataSO;
        
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
                if (transform.GetChild(i).TryGetComponent(out TileControl tile))
                    tile.Init(_firstPuzzleDataSO.puzzleData[i]);
                else
                    transform.GetChild(i).transform.rotation = Quaternion.Euler(0, 0, _firstPuzzleDataSO.puzzleData[i]);
            }
        }

        private void Update()
        {
            if (!_proccessing) return;
            StopProccess();

            for (int i = 0; i < _correctPuzzleDataSO[_randomValue].puzzleData.Length; i++)
            {
                if (transform.GetChild(i).TryGetComponent<TileControl>(out TileControl tileControl))
                {
                    if (tileControl.currentRotationZ != _correctPuzzleDataSO[_randomValue].puzzleData[i])
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

        public static event System.Action OnCleared; // 가게 쪽(MiniGameLauncher)에서 받아서 미니게임을 닫는다

        public void EndMiniGame()
        {
            Debug.Log(123);
            OnCleared?.Invoke();
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
