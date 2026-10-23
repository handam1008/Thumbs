using System;
using System.Collections;
using UnityEngine;
using WSH._09.SO;

namespace WSH._01.Scripts
{
    public class TwogamePuzzleProcess : MonoBehaviour
    {
        [SerializeField] private PuzzleDataSO _correctPuzzleDataSO;
        
        [SerializeField] private GameObject _canvas;
        [SerializeField] private GameObject _explain;
        
        [SerializeField] private int _randomValue;

        [SerializeField] private bool _proccessing;
        [SerializeField] private bool _complete;

        private void OnEnable()
        {
            _explain.SetActive(true);
            _canvas.SetActive(false);
        }

        private void Update()
        {
            if (!_proccessing) return;
            StopProccess();

            for (int i = 0; i < _correctPuzzleDataSO.puzzleData.Length; i++)
            {
                if (transform.GetChild(i).TryGetComponent<TwogameTileControl>(out TwogameTileControl tileControl))
                {
                    if (tileControl.IsWrong(_correctPuzzleDataSO.puzzleData[i]))
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
                if (transform.GetChild(i).TryGetComponent<TwogameTileControl>(out TwogameTileControl tileControl))
                {
                    tileControl._complete = true;
                }
            }
            _explain.SetActive(false);
            yield return new WaitForSeconds(2f);
            _canvas.SetActive(true);
            yield return new WaitForSeconds(2f);
            //여기다 다음씬 추가하면 될듯
            OnCleared?.Invoke(); // 가게 쪽(MiniGameLauncher)이 받아서 미니게임을 닫는다
        }

        public static event Action OnCleared;
    }
}
