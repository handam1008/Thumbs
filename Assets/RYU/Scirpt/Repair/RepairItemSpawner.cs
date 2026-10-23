using RYU.Scirpt.Guest;
using UnityEngine;

namespace RYU.Scirpt.Repair
{
    // 손님이 카운터에 도착하면 AJH 가전 프리팹 중 하나를 ItemSpawn 위치에 생성한다.
    public class RepairItemSpawner : MonoBehaviour
    {
        [SerializeField] private GuestPatrol guestPatrol;
        [SerializeField] private MiniGameLauncher miniGameLauncher;
        [SerializeField] private Transform itemSpawn;
        [SerializeField] private int repairReward = 500;              // 수리 성공 시 골드
        [SerializeField] private GameObject[] itemPrefabs;            // Assets/AJH/Prefabs
        [SerializeField] private LayerMask electricLayer = 1 << 6;    // HandRaycast의 whatIsElectric과 같은 레이어

        public GameObject CurrentItem { get; private set; }

        private void OnEnable()
        {
            guestPatrol.OnGuestarrivce += SpawnItem;
            miniGameLauncher.OnRepairCleared += HandleRepaired;
        }

        private void OnDisable()
        {
            guestPatrol.OnGuestarrivce -= SpawnItem;
            miniGameLauncher.OnRepairCleared -= HandleRepaired;
        }

        // 수리 완료: 물건을 치우고 골드 지급, 손님은 돌아간다
        private void HandleRepaired()
        {
            if (CurrentItem == null) return;
            Destroy(CurrentItem);
            CurrentItem = null;
            if (GoldManager.Instance != null) GoldManager.Instance.AddGold(repairReward);
            guestPatrol.SendGuestAway();
        }

        private void SpawnItem()
        {
            if (CurrentItem != null) Destroy(CurrentItem); // 카운터에는 물건 하나만

            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            CurrentItem = Instantiate(prefab, itemSpawn.position, itemSpawn.rotation);
            PrepareWires(CurrentItem);
        }

        // AJH 프리팹 대부분은 전선(전기 레이어) 부품에 Collider가 없어서 레이캐스트에 안 걸린다.
        // 생성할 때 Collider와 Object(하이라이트용)를 붙여 준다. 이미 있으면 건드리지 않는다.
        private void PrepareWires(GameObject item)
        {
            foreach (MeshRenderer meshRenderer in item.GetComponentsInChildren<MeshRenderer>())
            {
                GameObject part = meshRenderer.gameObject;
                if ((electricLayer.value & (1 << part.layer)) == 0) continue;

                if (!part.TryGetComponent<Collider>(out _)) part.AddComponent<MeshCollider>();
                if (!part.TryGetComponent<RYU.Object>(out _)) part.AddComponent<RYU.Object>();
            }
        }
    }
}
