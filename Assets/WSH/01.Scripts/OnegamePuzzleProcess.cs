using System;
using System.Collections;
using UnityEngine;
using UnityEngine.WSA;
using WSH._09.SO;
using Random = UnityEngine.Random;

namespace WSH._01.Scripts
{
    public class OnegamePuzzleProcess : MonoBehaviour
    {
        [SerializeField] private PuzzleDataSO[] _correctPuzzleDataSO;
        [SerializeField] private PuzzleDataSO[] _firstPuzzleDataSO;

        [SerializeField] private GameObject _canvas;
        [SerializeField] private GameObject _explain;
        
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
                transform.GetChild(i).GetComponent<OnegameTileControl>().Init(_firstPuzzleDataSO[_randomValue].puzzleData[i]);
            }
        }

        private void OnEnable()
        {
            _explain.SetActive(true);
            _canvas.SetActive(false);
        }

        private void Update()
        {
            if (!_proccessing) return;
            StopProccess();

            for (int i = 0; i < _correctPuzzleDataSO[_randomValue].puzzleData.Length; i++)
            {
                if (transform.GetChild(i).TryGetComponent<OnegameTileControl>(out OnegameTileControl tileControl))
                {
                    if (!tileControl.IsCorret(_correctPuzzleDataSO[_randomValue].puzzleData[i])) // 하나라도 틀리면 미완료
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
            StartCoroutine(End());
        }
        
        public void Proccess()
        {
            _proccessing = true;
        }

        public void StopProccess()
        {
            _proccessing = false;
        }

        IEnumerator End()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<OnegameTileControl>(out OnegameTileControl tileControl))
                {
                    tileControl._complete = true;
                }
            }
            _explain.SetActive(false);
            yield return new WaitForSeconds(2f);
            _canvas.SetActive(true);
            yield return new WaitForSeconds(2f);
            OnCleared?.Invoke(); // 가게 쪽(MiniGameLauncher)이 받아서 미니게임을 닫는다
        }

        public static event Action OnCleared;
    }
}
