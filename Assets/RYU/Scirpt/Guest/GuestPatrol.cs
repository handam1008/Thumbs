using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RYU.Scirpt.Guest
{
    public class GuestPatrol : MonoBehaviour
    {
        [SerializeField] private GameObject[] guest;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private float moveDuration = 3f; 
        
        public event Action OnGuestarrivce;
        public event Action OnRepair;
       
        private void Update()
        {
            if(Keyboard.current.pKey.wasPressedThisFrame)
                SpawnGuest();
        }

        private void SpawnGuest()
        {
            Vector3 dir = endPoint.position - spawnPoint.position;
            int random = UnityEngine.Random.Range(0, guest.Length );
            GameObject guestObj = Instantiate(guest[random], spawnPoint.position, Quaternion.LookRotation(dir)); 
            StartCoroutine(MoveGuest(guestObj.transform));
            OnGuestarrivce?.Invoke();
            
        }

        private IEnumerator MoveGuest(Transform guestTransform)
        {
            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                guestTransform.position = Vector3.Lerp(spawnPoint.position, endPoint.position, elapsed / moveDuration);
                yield return null;
            }
        }
    }
}
