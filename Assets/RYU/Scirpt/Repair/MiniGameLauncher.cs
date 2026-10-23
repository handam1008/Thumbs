using System;
using System.Collections;
using RYU.Scirpt.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using WSH._01.Scripts;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace RYU.Scirpt.Repair
{
    // 전선을 보고 E를 누르면 WSH 미니게임 씬(카메라)을 가게 씬 위에 추가로 불러오고,
    // 그 안에 미니게임 프리팹 하나를 무작위로 생성한다.
    // 퍼즐을 풀면(Onegame/TwogamePuzzleProcess.OnCleared) 또는 ESC를 누르면 닫고 플레이어 조작을 돌려준다.
    public class MiniGameLauncher : MonoBehaviour
    {
        [SerializeField] private HandRaycast handRaycast;
        [SerializeField] private Behaviour[] disableWhilePlaying;   // 이동, 시점, 레이캐스트, 플레이어 카메라 등
        [SerializeField] private GameObject[] miniGamePrefabs;     // Assets/WSH/02.Prefab의 1_MiniGame, 2_MiniGame
        [SerializeField] private string miniGameScenePath = "Assets/WSH/00.Scene/MinGame.unity";
        [SerializeField] private Vector3 miniGameOffset = new(0f, 1000f, 0f); // 가게와 겹치지 않게 멀리 띄운다

        public bool IsPlaying { get; private set; }
        public event Action OnRepairCleared;

        private Scene _scene;
        private bool _busy; // 씬을 불러오거나 닫는 중

        private void OnEnable()
        {
            OnegamePuzzleProcess.OnCleared += HandleCleared;
            TwogamePuzzleProcess.OnCleared += HandleCleared;
        }

        private void OnDisable()
        {
            OnegamePuzzleProcess.OnCleared -= HandleCleared;
            TwogamePuzzleProcess.OnCleared -= HandleCleared;
        }

        private void Update()
        {
            if (_busy || Keyboard.current == null) return;

            if (!IsPlaying && Keyboard.current.eKey.wasPressedThisFrame && handRaycast.Current != null)
                StartCoroutine(Open());
            else if (IsPlaying && Keyboard.current.escapeKey.wasPressedThisFrame)
                StartCoroutine(Close());
        }

        private IEnumerator Open()
        {
            _busy = true;

#if UNITY_EDITOR
            // 에디터에서는 Build Settings에 등록하지 않아도 경로로 불러올 수 있다
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(miniGameScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
#else
            yield return SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(miniGameScenePath), LoadSceneMode.Additive);
#endif
            _scene = SceneManager.GetSceneByPath(miniGameScenePath);

            // 미니게임 프리팹은 원점 기준으로 만들어져 있다 (씬 카메라는 z=-13에서 바라봄)
            GameObject miniGame = Instantiate(miniGamePrefabs[UnityEngine.Random.Range(0, miniGamePrefabs.Length)]);
            SceneManager.MoveGameObjectToScene(miniGame, _scene); // 씬을 닫을 때 같이 사라지게

            foreach (GameObject root in _scene.GetRootGameObjects())
            {
                root.transform.position += miniGameOffset;
                // 가게 씬에 이미 EventSystem이 있으므로 미니게임 쪽 것은 끈다
                if (root.TryGetComponent<EventSystem>(out _)) root.SetActive(false);
            }

            SetPlayerControl(false);
            IsPlaying = true;
            _busy = false;
        }

        private IEnumerator Close()
        {
            _busy = true;
            IsPlaying = false;

            yield return SceneManager.UnloadSceneAsync(_scene);

            SetPlayerControl(true);
            _busy = false;
        }

        private void HandleCleared()
        {
            if (!IsPlaying || _busy) return;
            StartCoroutine(Close());
            OnRepairCleared?.Invoke();
        }

        private void SetPlayerControl(bool active)
        {
            foreach (Behaviour behaviour in disableWhilePlaying)
                if (behaviour != null) behaviour.enabled = active;

            // 미니게임 타일은 마우스로 클릭하므로 커서를 풀어 준다
            Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !active;
        }
    }
}
