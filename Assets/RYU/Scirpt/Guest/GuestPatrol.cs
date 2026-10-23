using System;
using System.Collections;
using UnityEngine;

namespace RYU.Scirpt.Guest
{
    public class GuestPatrol : MonoBehaviour
    {
        [SerializeField] private GameObject[] guest;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private float moveDuration = 3f;
        [SerializeField] private float respawnDelay = 3f; // 손님이 나가고 다음 손님이 올 때까지(초)

        public event Action OnGuestarrivce;
        public event Action OnRepair;

        private GameObject _currentGuest;

        private void Start()
        {
            SpawnGuest(); // 게임 시작하자마자 첫 손님
        }

        private void SpawnGuest()
        {
            Vector3 dir = endPoint.position - spawnPoint.position;
            int random = UnityEngine.Random.Range(0, guest.Length );
            _currentGuest = Instantiate(guest[random], spawnPoint.position, Quaternion.LookRotation(dir));
            StartCoroutine(Come(_currentGuest.transform));
        }

        // 수리가 끝나면 호출: 손님이 입구로 돌아가 사라지고, respawnDelay초 뒤 다음 손님이 온다
        public void SendGuestAway()
        {
            if (_currentGuest == null) return;
            StartCoroutine(Leave(_currentGuest));
            _currentGuest = null;
            OnRepair?.Invoke();
        }

        private IEnumerator Come(Transform guestTransform)
        {
            yield return MoveGuest(guestTransform, spawnPoint.position, endPoint.position);
            OnGuestarrivce?.Invoke(); // 끝 지점에 도착한 뒤에 알림
        }

        private IEnumerator Leave(GameObject guestObj)
        {
            guestObj.transform.rotation = Quaternion.LookRotation(spawnPoint.position - endPoint.position); // 입구 쪽으로 돌아서
            yield return MoveGuest(guestObj.transform, endPoint.position, spawnPoint.position);
            Destroy(guestObj);

            yield return new WaitForSeconds(respawnDelay);
            SpawnGuest();
        }

        private IEnumerator MoveGuest(Transform guestTransform, Vector3 from, Vector3 to)
        {
            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                guestTransform.position = Vector3.Lerp(from, to, elapsed / moveDuration);
                yield return null;
            }
        }
    }
}
