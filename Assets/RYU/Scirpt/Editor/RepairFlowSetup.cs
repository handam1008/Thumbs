using System.Collections.Generic;
using System.Linq;
using RYU.Scirpt.Guest;
using RYU.Scirpt.Player;
using RYU.Scirpt.Repair;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 메뉴 Thumbs > 수리 흐름 연결 : 지금 열린 가게 씬에 RepairItemSpawner, MiniGameLauncher를 붙이고 참조를 채운다.
// 여러 번 눌러도 컴포넌트가 중복으로 붙지 않는다. 끝나면 씬을 저장(Ctrl+S)하면 된다.
public static class RepairFlowSetup
{
    const string ItemPrefabFolder = "Assets/AJH/Prefabs";
    const string MiniGamePrefabFolder = "Assets/WSH/02.Prefab";
    const int ElectricLayer = 6;

    [MenuItem("Thumbs/수리 흐름 연결")]
    public static void Setup()
    {
        var guestPatrol = Object.FindAnyObjectByType<GuestPatrol>();
        var handRaycast = Object.FindAnyObjectByType<HandRaycast>();
        var itemSpawn = GameObject.Find("ItemSpawn");
        if (guestPatrol == null || handRaycast == null || itemSpawn == null)
        {
            EditorUtility.DisplayDialog("수리 흐름 연결", "씬에서 GuestPatrol, HandRaycast(Player), ItemSpawn 중 하나를 못 찾았어요.", "확인");
            return;
        }

        // 1. 전선 보고 E -> 미니게임
        var player = handRaycast.gameObject;
        var cam = player.GetComponentInChildren<Camera>();
        var disable = new List<Object>
        {
            player.GetComponent<PlayerMovement>(),
            handRaycast,
            player.GetComponentInChildren<View>(),
            cam,
            cam != null ? cam.GetComponent<AudioListener>() : null,
        };

        var launcher = GetOrAdd<MiniGameLauncher>(player);
        var so = new SerializedObject(launcher);
        so.FindProperty("handRaycast").objectReferenceValue = handRaycast;
        SetArray(so.FindProperty("disableWhilePlaying"), disable.Where(o => o != null).ToList());
        SetArray(so.FindProperty("miniGamePrefabs"), LoadMiniGamePrefabs());
        so.ApplyModifiedProperties();

        // 2. 손님 도착 -> 물건 생성, 수리 완료 -> 물건 제거 + 골드
        var spawner = GetOrAdd<RepairItemSpawner>(guestPatrol.gameObject);
        so = new SerializedObject(spawner);
        so.FindProperty("guestPatrol").objectReferenceValue = guestPatrol;
        so.FindProperty("miniGameLauncher").objectReferenceValue = launcher;
        so.FindProperty("itemSpawn").objectReferenceValue = itemSpawn.transform;
        SetArray(so.FindProperty("itemPrefabs"), LoadItemPrefabs());
        so.ApplyModifiedProperties();

        // 3. 전선을 보고 누른 E에는 손 애니메이션이 안 나오게
        var thumb = Object.FindAnyObjectByType<ThumbAnimation>();
        if (thumb != null)
        {
            so = new SerializedObject(thumb);
            so.FindProperty("handRaycast").objectReferenceValue = handRaycast;
            so.ApplyModifiedProperties();
        }

        // 4. 빌드에서도 미니게임 씬을 불러올 수 있게 Build Profiles 씬 목록에 추가
        AddToBuildScenes(new SerializedObject(launcher).FindProperty("miniGameScenePath").stringValue);

        EditorSceneManager.MarkSceneDirty(guestPatrol.gameObject.scene);
        Selection.objects = new Object[] { guestPatrol.gameObject, player };
        Debug.Log("[RepairFlowSetup] 연결 완료. 씬을 저장하세요.");
    }

    // 전기 레이어(6) 부품이 하나라도 있는 프리팹만 쓴다 (전선이 없으면 E를 누를 곳이 없으니까)
    static List<Object> LoadItemPrefabs()
    {
        var result = new List<Object>();
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { ItemPrefabFolder }))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (prefab.GetComponentsInChildren<Transform>(true).Any(t => t.gameObject.layer == ElectricLayer))
                result.Add(prefab);
        }
        return result;
    }

    // WSH 퍼즐 컨트롤러(Onegame/TwogamePuzzleProcess)가 들어 있는 프리팹만 미니게임으로 쓴다
    static List<Object> LoadMiniGamePrefabs()
    {
        var result = new List<Object>();
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { MiniGamePrefabFolder }))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (prefab.GetComponentInChildren<WSH._01.Scripts.OnegamePuzzleProcess>(true) != null ||
                prefab.GetComponentInChildren<WSH._01.Scripts.TwogamePuzzleProcess>(true) != null)
                result.Add(prefab);
        }
        return result;
    }

    static void AddToBuildScenes(string path)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (scenes.Any(s => s.path == path)) return;
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        return go.TryGetComponent(out T existing) ? existing : Undo.AddComponent<T>(go);
    }

    static void SetArray(SerializedProperty prop, List<Object> values)
    {
        prop.arraySize = values.Count;
        for (int i = 0; i < values.Count; i++)
            prop.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }
}
