using System.Collections.Generic;
using RYU.Scirpt.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// 메뉴 Thumbs > 가게 만들기 : 기본 도형으로 전파사 그레이박스 씬을 생성한다.
// 좌표 기준: 바닥 y=0, 가게 폭 x -6~6, 안쪽 벽 z=0, 입구 z=10. 단위는 미터.
// 구역: z 0~2.6 카운터 안쪽(작업 공간) / z=3 카운터(오른쪽에 출입구) / z 3.4~10 손님 공간
// 분위기: 해 질 녘, 어두운 가게. 구석엔 잡동사니, 늘어진 전선과 스파크.
public static class ShopBuilder
{
    const string ScenePath = "Assets/RYU/Scene/Shop.unity";
    const string MatFolder = "Assets/RYU/Materials/Shop";

    const float HalfWidth = 6f;   // 가게 폭의 절반
    const float Depth = 10f;      // 안쪽 벽 ~ 입구
    const float Height = 3.4f;    // 천장 높이
    const float CounterZ = 3f;
    const float GapLeft = 2.6f;   // 카운터 출입구 x 범위
    const float GapRight = 4.0f;

    static readonly Dictionary<string, Material> mats = new Dictionary<string, Material>();
    static System.Random rng;

    static readonly Color WallColor = new Color(0.42f, 0.40f, 0.34f);
    static readonly Color MetalColor = new Color(0.20f, 0.20f, 0.22f);
    static readonly Color[] JunkColors =
    {
        new Color(0.55f, 0.42f, 0.28f), new Color(0.45f, 0.36f, 0.25f),   // 골판지
        new Color(0.70f, 0.70f, 0.66f), new Color(0.18f, 0.18f, 0.20f),   // 가전 흰색/검정
        new Color(0.40f, 0.48f, 0.55f), new Color(0.55f, 0.30f, 0.20f),
    };
    static readonly Color[] CableColors =
    {
        new Color(0.05f, 0.05f, 0.05f), new Color(0.60f, 0.10f, 0.08f),
        new Color(0.75f, 0.60f, 0.10f), new Color(0.35f, 0.35f, 0.35f),
    };

    [MenuItem("Thumbs/가게 만들기")]
    public static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        mats.Clear();
        rng = new System.Random(7); // 매번 같은 배치가 나오도록 고정

        var root = new GameObject("Shop").transform;
        BuildRoom(Group("Room", root));
        BuildFrontWall(Group("FrontWall", root));
        BuildCounter(Group("Counter", root));
        BuildWorkArea(Group("WorkArea", root));
        BuildShelves(Group("Shelves", root));
        BuildClutter(Group("Clutter", root));
        BuildElectric(Group("Electric", root));
        BuildStreet(Group("Street", root));
        BuildMarkers(Group("Markers", root));
        BuildLights(Group("Lights", root));
        BuildPostFX(root);
        BuildPlayer();

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.06f, 0.065f, 0.08f);

        System.IO.Directory.CreateDirectory("Assets/RYU/Scene");
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("[ShopBuilder] 가게 생성 완료: " + ScenePath);
    }

    // ---------- 방 ----------
    static void BuildRoom(Transform p)
    {
        float w = HalfWidth * 2, cz = Depth / 2, h = Height;
        Box("Floor", p, new Vector3(0, -0.05f, cz), new Vector3(w, 0.1f, Depth), "Floor", new Color(0.22f, 0.23f, 0.21f));
        Box("Ceiling", p, new Vector3(0, h + 0.05f, cz), new Vector3(w, 0.1f, Depth), "Ceiling", new Color(0.40f, 0.39f, 0.36f));
        Box("Wall_Left", p, new Vector3(-HalfWidth - 0.1f, h / 2, cz), new Vector3(0.2f, h, Depth), "Wall", WallColor);
        Box("Wall_Right", p, new Vector3(HalfWidth + 0.1f, h / 2, cz), new Vector3(0.2f, h, Depth), "Wall", WallColor);
        Box("Wall_Back", p, new Vector3(0, h / 2, -0.1f), new Vector3(w + 0.4f, h, 0.2f), "Wall", WallColor);

        var trim = new Color(0.20f, 0.16f, 0.12f);
        Box("Trim_Left", p, new Vector3(-HalfWidth + 0.02f, 0.08f, cz), new Vector3(0.04f, 0.16f, Depth), "Trim", trim);
        Box("Trim_Right", p, new Vector3(HalfWidth - 0.02f, 0.08f, cz), new Vector3(0.04f, 0.16f, Depth), "Trim", trim);
        Box("Trim_Back", p, new Vector3(0, 0.08f, 0.02f), new Vector3(w, 0.16f, 0.04f), "Trim", trim);

        var stain = new Color(0.30f, 0.28f, 0.23f);
        Box("Stain_1", p, new Vector3(-HalfWidth + 0.01f, 0.6f, 7.8f), new Vector3(0.01f, 0.9f, 1.3f), "Stain", stain);
        Box("Stain_2", p, new Vector3(HalfWidth - 0.01f, 3.0f, 4.2f), new Vector3(0.01f, 0.6f, 1.6f), "Stain", stain);
        Box("Stain_3", p, new Vector3(-1.5f, 2.9f, 0.01f), new Vector3(1.4f, 0.5f, 0.01f), "Stain", stain);
    }

    // 입구 벽: 가운데 문(폭 1.4, 높이 2.3) + 양옆 진열창
    static void BuildFrontWall(Transform p)
    {
        var frame = new Color(0.12f, 0.13f, 0.15f);
        var glass = new Color(0.25f, 0.32f, 0.45f); // 해 질 녘 하늘
        float z = Depth + 0.1f;
        float headerH = Height - 2.3f;
        float winCenter = (1.1f + HalfWidth - 0.4f) / 2;
        float winWidth = HalfWidth - 0.4f - 1.1f;

        Box("Header", p, new Vector3(0, 2.3f + headerH / 2, z), new Vector3(HalfWidth * 2 + 0.4f, headerH, 0.2f), "Wall", WallColor);
        foreach (int s in new[] { -1, 1 })
        {
            string side = s < 0 ? "L" : "R";
            Box("Sill_" + side, p, new Vector3(s * (0.7f + HalfWidth) / 2, 0.45f, z), new Vector3(HalfWidth - 0.7f, 0.9f, 0.2f), "Wall", WallColor);
            Box("Pillar_Outer_" + side, p, new Vector3(s * (HalfWidth - 0.2f), 1.6f, z), new Vector3(0.4f, 1.4f, 0.2f), "Wall", WallColor);
            Box("Pillar_Door_" + side, p, new Vector3(s * 0.9f, 1.6f, z), new Vector3(0.4f, 1.4f, 0.2f), "Wall", WallColor);
            Box("Window_" + side, p, new Vector3(s * winCenter, 1.6f, z), new Vector3(winWidth, 1.4f, 0.05f), "Glass", glass, emission: 0.4f);
            Box("WindowFrame_" + side, p, new Vector3(s * winCenter, 1.6f, z - 0.06f), new Vector3(0.06f, 1.4f, 0.06f), "Frame", frame);
            Box("DoorFrame_" + side, p, new Vector3(s * 0.72f, 1.15f, z - 0.02f), new Vector3(0.06f, 2.3f, 0.26f), "Frame", frame);
        }
        Box("DoorFrame_Top", p, new Vector3(0, 2.32f, z - 0.02f), new Vector3(1.5f, 0.06f, 0.26f), "Frame", frame);
        Box("Sign", p, new Vector3(0, 2.85f, z - 0.12f), new Vector3(2.8f, 0.6f, 0.04f), "Sign", new Color(0.10f, 0.18f, 0.32f));
    }

    // ---------- 카운터 ----------
    // 왼쪽 벽부터 진열장 - 메인 카운터 - [출입구] - 짧은 카운터 - 오른쪽 벽
    static void BuildCounter(Transform p)
    {
        var wood = new Color(0.32f, 0.21f, 0.13f);
        var top = new Color(0.42f, 0.40f, 0.37f);
        const float mainLeft = -4f;

        // 메인 카운터
        float mainCenter = (mainLeft + GapLeft) / 2, mainWidth = GapLeft - mainLeft;
        Box("Main_Body", p, new Vector3(mainCenter, 0.5f, CounterZ), new Vector3(mainWidth, 1.0f, 0.7f), "CounterWood", wood);
        Box("Main_Top", p, new Vector3(mainCenter, 1.03f, CounterZ), new Vector3(mainWidth + 0.1f, 0.06f, 0.85f), "CounterTop", top);

        // 출입구 오른쪽 짧은 카운터 + 들어 올린 뚜껑
        float rightCenter = (GapRight + HalfWidth) / 2, rightWidth = HalfWidth - GapRight;
        Box("Right_Body", p, new Vector3(rightCenter, 0.5f, CounterZ), new Vector3(rightWidth, 1.0f, 0.7f), "CounterWood", wood);
        Box("Right_Top", p, new Vector3(rightCenter, 1.03f, CounterZ), new Vector3(rightWidth, 0.06f, 0.85f), "CounterTop", top);
        float gap = GapRight - GapLeft;
        Box("FlipLid", p, new Vector3(GapRight + 0.03f, 1.06f + gap / 2, CounterZ), new Vector3(0.05f, gap, 0.85f), "CounterTop", top);

        // 왼쪽 진열장 (유리 상판)
        float caseCenter = (-HalfWidth + mainLeft) / 2, caseWidth = mainLeft + HalfWidth;
        Box("Case_Body", p, new Vector3(caseCenter, 0.4f, CounterZ), new Vector3(caseWidth, 0.8f, 0.7f), "Frame", new Color(0.12f, 0.13f, 0.15f));
        Box("Case_Glass", p, new Vector3(caseCenter, 0.92f, CounterZ), new Vector3(caseWidth - 0.05f, 0.24f, 0.65f), "Glass", new Color(0.25f, 0.32f, 0.45f), emission: 0.4f);
        Box("Case_Top", p, new Vector3(caseCenter, 1.05f, CounterZ), new Vector3(caseWidth, 0.02f, 0.75f), "Frame", new Color(0.12f, 0.13f, 0.15f));

        // 카운터 위 소품. 플레이어 정면(x=0) 기준 왼쪽에 수리 매트(물건 소환 자리)
        Box("RepairMat", p, new Vector3(-1.0f, 1.065f, CounterZ - 0.05f), new Vector3(1.2f, 0.01f, 0.6f), "RepairMat", new Color(0.12f, 0.35f, 0.24f));
        Box("Register", p, new Vector3(1.8f, 1.21f, CounterZ), new Vector3(0.45f, 0.3f, 0.4f), "Register", new Color(0.18f, 0.18f, 0.20f), rot: new Vector3(0, 180, 0));
        Box("Register_Screen", p, new Vector3(1.8f, 1.42f, CounterZ - 0.05f), new Vector3(0.3f, 0.12f, 0.03f), "Screen", new Color(0.3f, 0.9f, 0.5f), emission: 1.2f);

        // 작업 스탠드: 카운터 위를 비추는 가장 밝은 빛
        Cylinder("Lamp_Base", p, new Vector3(-2.4f, 1.08f, CounterZ - 0.15f), new Vector3(0.2f, 0.02f, 0.2f), "Metal", MetalColor);
        Box("Lamp_Pole", p, new Vector3(-2.4f, 1.35f, CounterZ - 0.15f), new Vector3(0.03f, 0.55f, 0.03f), "Metal", MetalColor);
        Box("Lamp_Head", p, new Vector3(-2.15f, 1.6f, CounterZ - 0.1f), new Vector3(0.4f, 0.08f, 0.15f), "Metal", MetalColor);
        var lamp = AddLight("Lamp_Light", p, new Vector3(-1.6f, 1.5f, CounterZ), new Color(1f, 0.78f, 0.5f), 2.5f, 3.5f);
        lamp.shadows = LightShadows.Soft;

        Box("ToolTray", p, new Vector3(0.3f, 1.1f, CounterZ - 0.2f), new Vector3(0.4f, 0.08f, 0.25f), "Tray", new Color(0.55f, 0.15f, 0.12f));
        Cylinder("SolderStand", p, new Vector3(0.9f, 1.1f, CounterZ - 0.2f), new Vector3(0.12f, 0.04f, 0.12f), "Metal", MetalColor);
        for (int i = 0; i < 8; i++)
            Box("Part_" + i, p, new Vector3(R(-3.5f, 2.3f), 1.07f, R(CounterZ - 0.3f, CounterZ + 0.3f)), new Vector3(0.04f, 0.015f, R(0.03f, 0.08f)),
                "Metal2", new Color(0.55f, 0.55f, 0.58f), rot: new Vector3(0, R(0, 180), 0));
        Cables("CounterCable", p, new Vector3(0.2f, 1.07f, CounterZ), 0.4f, 3);
    }

    // 카운터 안쪽: 작업대 + 공구판, 부품장, 두꺼비집, 잡동사니
    static void BuildWorkArea(Transform p)
    {
        // 안쪽 벽 오른편 작업대와 공구판
        Box("Workbench_Top", p, new Vector3(2.8f, 0.9f, 0.45f), new Vector3(3.2f, 0.06f, 0.8f), "CounterTop", new Color(0.42f, 0.40f, 0.37f));
        foreach (float x in new[] { 1.3f, 4.3f })
            Box("Workbench_Leg_" + x, p, new Vector3(x, 0.44f, 0.45f), new Vector3(0.08f, 0.87f, 0.7f), "Metal", MetalColor);
        Box("Pegboard", p, new Vector3(2.8f, 1.8f, 0.03f), new Vector3(3.0f, 1.2f, 0.05f), "Pegboard", new Color(0.45f, 0.34f, 0.22f));
        for (int i = 0; i < 7; i++)
        {
            float x = 1.6f + i * 0.4f;
            Box("Tool_Handle_" + i, p, new Vector3(x, 1.65f, 0.08f), new Vector3(0.06f, 0.25f, 0.04f), "Tool", new Color(0.55f, 0.12f, 0.10f));
            Box("Tool_Head_" + i, p, new Vector3(x, 1.88f + (i % 2) * 0.05f, 0.08f), new Vector3(0.03f, 0.2f + (i % 3) * 0.05f, 0.03f), "Metal2", new Color(0.55f, 0.55f, 0.58f));
        }
        Cylinder("CableCoil", p, new Vector3(4.0f, 2.1f, 0.1f), new Vector3(0.3f, 0.03f, 0.3f), "Cable0", CableColors[0], rot: new Vector3(90, 0, 0));
        Box("Bench_Radio", p, new Vector3(2.0f, 1.08f, 0.4f), new Vector3(0.5f, 0.3f, 0.25f), "Junk3", JunkColors[3], rot: new Vector3(0, 10, 0));
        Box("Bench_Fan", p, new Vector3(3.4f, 1.13f, 0.45f), new Vector3(0.35f, 0.4f, 0.35f), "Junk2", JunkColors[2], rot: new Vector3(0, -20, 0));

        // 안쪽 벽 왼편 부품 서랍장
        Box("PartsCabinet", p, new Vector3(-2.2f, 0.6f, 0.3f), new Vector3(1.5f, 1.2f, 0.5f), "Cabinet", new Color(0.22f, 0.30f, 0.38f));
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                Box("Drawer_" + r + "_" + c, p, new Vector3(-2.75f + c * 0.36f, 0.2f + r * 0.27f, 0.56f), new Vector3(0.3f, 0.2f, 0.02f), "Drawer", new Color(0.65f, 0.60f, 0.30f));

        // 두꺼비집 (5일차 정전 이벤트용) — 오른쪽 벽, 카운터 안쪽
        Box("FuseBox", p, new Vector3(HalfWidth - 0.07f, 1.7f, 1.2f), new Vector3(0.12f, 0.6f, 0.45f), "FuseBox", new Color(0.55f, 0.55f, 0.50f));
        Box("FuseBox_Lever", p, new Vector3(HalfWidth - 0.15f, 1.75f, 1.2f), new Vector3(0.05f, 0.12f, 0.05f), "Lever", new Color(0.1f, 0.1f, 0.1f));

        Box("Poster_Back", p, new Vector3(-0.3f, 2.0f, 0.01f), new Vector3(0.5f, 0.7f, 0.01f), "Poster1", new Color(0.60f, 0.50f, 0.25f), rot: new Vector3(0, 0, 4));
        Pile("Pile_BackLeft", p, new Vector3(-5.1f, 0, 1.0f), new Vector2(1.2f, 1.4f), 7);
    }

    // 왼쪽 벽 손님 공간: 수리 대기 가전이 꽉 찬 선반
    static void BuildShelves(Transform p)
    {
        var frame = new Color(0.25f, 0.26f, 0.28f);
        float x = -HalfWidth + 0.35f;
        const float z0 = 4.5f, z1 = 9.0f;
        foreach (float z in new[] { z0, (z0 + z1) / 2, z1 })
            Box("Upright_" + z, p, new Vector3(x, 1.1f, z), new Vector3(0.6f, 2.2f, 0.05f), "ShelfFrame", frame);

        for (int i = 0; i < 4; i++)
        {
            float y = 0.15f + i * 0.6f;
            Box("Board_" + i, p, new Vector3(x, y, (z0 + z1) / 2), new Vector3(0.6f, 0.04f, z1 - z0 + 0.05f), "ShelfFrame", frame);

            // 판 위에 크기가 다른 상자(가전 대용)를 채운다. 맨 위 판엔 높이 제한 없이 쌓는다.
            float z = z0 + 0.1f;
            int n = 0;
            while (z < z1 - 0.3f)
            {
                float w = R(0.3f, 0.7f);
                float h = i == 3 ? R(0.3f, 0.7f) : R(0.2f, 0.5f);
                int c = rng.Next(JunkColors.Length);
                Box("Item_" + i + "_" + n++, p, new Vector3(x + R(-0.05f, 0.05f), y + 0.02f + h / 2, z + w / 2), new Vector3(R(0.3f, 0.5f), h, w),
                    "Junk" + c, JunkColors[c], rot: new Vector3(0, R(-8, 8), 0));
                z += w + R(0.02f, 0.12f);
            }
        }

        Box("Poster_Left", p, new Vector3(-HalfWidth + 0.01f, 2.9f, 6.75f), new Vector3(0.01f, 0.5f, 0.9f), "Poster2", new Color(0.22f, 0.40f, 0.52f));
    }

    // ---------- 손님 공간 잡동사니 ----------
    static void BuildClutter(Transform p)
    {
        Pile("Pile_FrontLeft", p, new Vector3(-4.4f, 0, 9.4f), new Vector2(1.4f, 0.9f), 11);
        Pile("Pile_Right", p, new Vector3(HalfWidth - 0.6f, 0, 6.6f), new Vector2(0.8f, 2.2f), 9);
        Pile("Pile_CounterFront", p, new Vector3(4.9f, 0, 3.9f), new Vector2(1.0f, 0.5f), 4);
        BuildTVStack(Group("TVStack", p), new Vector3(HalfWidth - 0.8f, 0, Depth - 0.6f));

        Cables("FloorCable_FrontLeft", p, new Vector3(-3.6f, 0.015f, 8.6f), 0.8f, 6);
        Cables("FloorCable_Right", p, new Vector3(4.6f, 0.015f, 5.6f), 0.7f, 5);
        Cables("FloorCable_Back", p, new Vector3(-4.2f, 0.015f, 1.6f), 0.6f, 4);
    }

    // 오래된 브라운관 TV 더미. 맨 위 하나는 노이즈 화면이 켜져 있다.
    static void BuildTVStack(Transform p, Vector3 basePos)
    {
        var body = new Color(0.25f, 0.24f, 0.22f);
        var dead = new Color(0.06f, 0.07f, 0.07f);
        Vector3[] offsets = { new Vector3(-0.3f, 0.23f, 0), new Vector3(0.3f, 0.23f, 0.05f), new Vector3(0, 0.69f, 0) };
        for (int i = 0; i < offsets.Length; i++)
        {
            var pos = basePos + offsets[i];
            var tv = Box("TV_" + i, p, pos, new Vector3(0.55f, 0.45f, 0.5f), "TVBody", body, rot: new Vector3(0, R(-12, 12), 0));
            bool on = i == offsets.Length - 1;
            var screen = Box("Screen", tv.transform, Vector3.zero, Vector3.one, on ? "TVStatic" : "TVDead",
                on ? new Color(0.55f, 0.70f, 0.85f) : dead, emission: on ? 1.5f : 0);
            screen.transform.localPosition = new Vector3(0, 0.02f, -0.51f);
            screen.transform.localRotation = Quaternion.identity;
            screen.transform.localScale = new Vector3(0.75f, 0.7f, 0.04f);

            if (on)
            {
                var glow = AddLight("TV_Glow", p, pos + new Vector3(0, 0, -0.6f), new Color(0.5f, 0.65f, 1f), 0.8f, 2.5f);
                Flicker(glow, 0.8f, 0.04f, 0.05f, screen.GetComponent<Renderer>());
            }
        }
    }

    // ---------- 전기 ----------
    static void BuildElectric(Transform p)
    {
        var conduit = new Color(0.30f, 0.30f, 0.32f);
        float cz = Depth / 2;

        // 벽을 따라가는 배관, 두꺼비집에서 올라가는 관
        Cylinder("Conduit_Right", p, new Vector3(HalfWidth - 0.07f, Height - 0.25f, cz), new Vector3(0.06f, Depth, 0.06f), "Conduit", conduit, rot: new Vector3(90, 0, 0));
        Cylinder("Conduit_Left", p, new Vector3(-HalfWidth + 0.07f, Height - 0.15f, cz), new Vector3(0.05f, Depth, 0.05f), "Conduit", conduit, rot: new Vector3(90, 0, 0));
        Cylinder("Conduit_Back", p, new Vector3(0, Height - 0.2f, 0.07f), new Vector3(0.05f, HalfWidth * 2, 0.05f), "Conduit", conduit, rot: new Vector3(0, 0, 90));
        float fuseTop = 2.0f;
        Cylinder("Conduit_Fuse", p, new Vector3(HalfWidth - 0.07f, (fuseTop + Height - 0.25f) / 2, 1.2f), new Vector3(0.05f, Height - 0.25f - fuseTop, 0.05f), "Conduit", conduit);

        // 천장에서 늘어진 전선 (손님 공간)
        for (int i = 0; i < 9; i++)
        {
            float len = R(0.4f, 1.2f);
            var pos = new Vector3(R(-5f, 5f), Height - len / 2, R(4f, 9.5f));
            int c = rng.Next(CableColors.Length);
            Box("HangingCable_" + i, p, pos, new Vector3(0.02f, len, 0.02f), "Cable" + c, CableColors[c],
                rot: new Vector3(R(-12, 12), 0, R(-12, 12)));
        }

        // 천장에 매달린 알전구 (앞쪽 왼편 잡동사니 위)
        var bulbPos = new Vector3(-3.6f, 2.4f, 8.4f);
        float cord = Height - bulbPos.y - 0.05f;
        Box("Bulb_Cord", p, new Vector3(bulbPos.x, Height - cord / 2, bulbPos.z), new Vector3(0.015f, cord, 0.015f), "Cable0", CableColors[0]);
        var bulb = Prim(PrimitiveType.Sphere, "Bulb", p, bulbPos, Vector3.one * 0.1f, Mat("Bulb", new Color(1f, 0.75f, 0.4f), 2f), Vector3.zero);
        var bulbLight = AddLight("Bulb_Light", p, bulbPos + Vector3.down * 0.12f, new Color(1f, 0.7f, 0.4f), 0.9f, 3.5f);
        Flicker(bulbLight, 0.9f, 0.1f, 0.04f, bulb.GetComponent<Renderer>());

        // 망가진 콘센트: 전선이 삐져나와 스파크가 튄다
        float wx = HalfWidth - 0.02f;
        Box("Outlet", p, new Vector3(wx, 0.35f, 4.6f), new Vector3(0.03f, 0.14f, 0.09f), "Outlet", new Color(0.65f, 0.62f, 0.55f));
        Box("Outlet_Wire_1", p, new Vector3(wx - 0.06f, 0.3f, 4.58f), new Vector3(0.012f, 0.012f, 0.14f), "Cable1", CableColors[1], rot: new Vector3(20, 60, 0));
        Box("Outlet_Wire_2", p, new Vector3(wx - 0.06f, 0.32f, 4.63f), new Vector3(0.012f, 0.012f, 0.12f), "Cable0", CableColors[0], rot: new Vector3(-15, 110, 0));
        Spark("Spark_Outlet", p, new Vector3(wx - 0.1f, 0.33f, 4.6f), new Vector3(0, -90, 0));

        // 늘어진 전선 끝에서도 가끔 튄다
        Box("LooseCable", p, new Vector3(2.6f, Height - 0.6f, 7.5f), new Vector3(0.02f, 1.2f, 0.02f), "Cable2", CableColors[2], rot: new Vector3(0, 0, 8));
        Spark("Spark_Ceiling", p, new Vector3(2.68f, Height - 1.2f, 7.5f), new Vector3(90, 0, 0));
    }

    // 문 밖 길거리 (해 질 녘, 가로등)
    static void BuildStreet(Transform p)
    {
        float z = Depth;
        Box("Sidewalk", p, new Vector3(0, -0.05f, z + 2f), new Vector3(20, 0.1f, 4), "Sidewalk", new Color(0.35f, 0.34f, 0.33f));
        Box("Road", p, new Vector3(0, -0.07f, z + 6f), new Vector3(20, 0.1f, 4), "Road", new Color(0.14f, 0.14f, 0.16f));
        Box("Building_Across", p, new Vector3(0, 3f, z + 9f), new Vector3(20, 6, 1), "Building", new Color(0.22f, 0.20f, 0.20f), emission: 0.05f);
        for (int i = 0; i < 6; i++)
            Box("LitWindow_" + i, p, new Vector3(-6.5f + i * 2.6f, 1.8f + (i % 2) * 1.6f, z + 8.45f), new Vector3(0.8f, 0.6f, 0.05f),
                "LitWindow", new Color(1f, 0.8f, 0.45f), emission: 1.2f);
        AddLight("StreetLight", p, new Vector3(3f, 3.5f, z + 2.5f), new Color(1f, 0.65f, 0.35f), 3f, 8f);
    }

    // 코드에서 위치를 참조할 빈 오브젝트 + 구도 확인용 손님 자리
    static void BuildMarkers(Transform p)
    {
        Marker("PlayerSpawn", p, new Vector3(0, 0, 1.8f));                    // 카운터 앞 정위치
        Marker("ItemSpawn", p, new Vector3(-1.0f, 1.07f, CounterZ - 0.05f));  // 정위치 기준 왼쪽 카운터 위
        Marker("BigItemSpawn", p, new Vector3(-1.2f, 0, 1.6f));               // 냉장고·세탁기: 정위치 왼쪽 바닥
        Marker("CustomerSpot", p, new Vector3(0, 0, 4.0f));                   // 손님이 서는 자리
        Marker("DoorPoint", p, new Vector3(0, 0, Depth + 0.5f));              // 손님 등장/퇴장 지점

        // 2D 초상화로 교체 예정
        Prim(PrimitiveType.Capsule, "Customer_Placeholder", p, new Vector3(0, 0.85f, 4.0f), new Vector3(0.5f, 0.85f, 0.35f),
            Mat("Customer", new Color(0.45f, 0.50f, 0.70f)), Vector3.zero);
    }

    static void BuildLights(Transform p)
    {
        var tube = new Color(0.85f, 0.95f, 1f);
        float y = Height - 0.03f;

        // 작업 공간·카운터 앞은 희미하게 켜져 있고, 입구 쪽 형광등은 고장 나서 깜빡인다
        foreach (float z in new[] { 1.3f, 4.5f })
        {
            Box("Fluorescent_" + z, p, new Vector3(0, y, z), new Vector3(2.0f, 0.06f, 0.2f), "Tube", tube, emission: 0.8f);
            AddLight("CeilingLight_" + z, p, new Vector3(0, y - 0.2f, z), tube, 0.7f, 6f).shadows = LightShadows.Soft;
        }

        var broken = Box("Fluorescent_Broken", p, new Vector3(0, y, 7.5f), new Vector3(2.0f, 0.06f, 0.2f), "TubeBroken", tube, emission: 1.2f);
        var backLight = AddLight("CeilingLight_Broken", p, new Vector3(0, y - 0.2f, 7.5f), tube, 1.1f, 6f);
        backLight.shadows = LightShadows.Soft;
        Flicker(backLight, 1.1f, 0.12f, 0.35f, broken.GetComponent<Renderer>());
    }

    // 후처리: 빛 번짐(블룸), 가장자리 어둡게(비네트), 살짝 누런 색감
    static void BuildPostFX(Transform p)
    {
        string path = MatFolder + "/ShopVolume.asset";
        AssetDatabase.DeleteAsset(path);
        System.IO.Directory.CreateDirectory(MatFolder);
        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, path);

        var bloom = profile.Add<Bloom>(true);
        bloom.threshold.Override(0.9f);
        bloom.intensity.Override(1.3f);
        var vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(0.45f);
        vignette.smoothness.Override(0.5f);
        var color = profile.Add<ColorAdjustments>(true);
        color.colorFilter.Override(new Color(1f, 0.93f, 0.82f));
        color.saturation.Override(-15f);
        color.contrast.Override(10f);
        foreach (var component in profile.components)
            AssetDatabase.AddObjectToAsset(component, profile);

        var volume = new GameObject("PostFX").AddComponent<Volume>();
        volume.transform.SetParent(p);
        volume.isGlobal = true;
        volume.sharedProfile = profile;
    }

    // 1인칭 플레이어: MainScene의 Player와 같은 구성 (몸에 Controller·Input·Movement, 카메라에 View)
    static void BuildPlayer()
    {
        const float height = 1.8f;
        var player = new GameObject("Player");
        player.transform.position = new Vector3(0, height / 2 + 0.05f, 1.8f);

        var cc = player.AddComponent<CharacterController>();
        cc.height = height;
        cc.radius = 0.35f;
        player.AddComponent<PlayerController>();
        player.AddComponent<InputHandler>();
        player.AddComponent<PlayerMovement>();

        var cam = new GameObject("Main Camera").AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = new Vector3(0, 0.7f, 0); // 눈높이 약 1.6m
        cam.fieldOfView = 70;
        cam.nearClipPlane = 0.05f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.09f, 0.14f);
        cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        cam.gameObject.AddComponent<AudioListener>();
        cam.gameObject.AddComponent<View>();
    }

    // ---------- 헬퍼 ----------
    static float R(float min, float max) => min + (float)rng.NextDouble() * (max - min);

    // 바닥에 상자를 3단으로 대충 쌓는다 (위로 갈수록 좁게)
    static void Pile(string name, Transform parent, Vector3 center, Vector2 area, int count)
    {
        var p = Group(name, parent);
        int[] perLayer = { (count + 1) / 2, count / 3, Mathf.Max(1, count - (count + 1) / 2 - count / 3) };
        for (int layer = 0; layer < perLayer.Length; layer++)
        {
            float shrink = 1f - layer * 0.3f;
            for (int i = 0; i < perLayer[layer]; i++)
            {
                float h = R(0.2f, 0.45f);
                var pos = center + new Vector3(R(-0.5f, 0.5f) * area.x * shrink, layer * 0.4f + h / 2, R(-0.5f, 0.5f) * area.y * shrink);
                int c = rng.Next(JunkColors.Length);
                Box("Junk_" + layer + "_" + i, p, pos, new Vector3(R(0.25f, 0.55f), h, R(0.25f, 0.55f)),
                    "Junk" + c, JunkColors[c], rot: new Vector3(0, R(-30, 30), 0));
            }
        }
    }

    // 바닥(또는 카운터)에 아무렇게나 널린 전선
    static void Cables(string name, Transform parent, Vector3 center, float radius, int count)
    {
        var p = Group(name, parent);
        for (int i = 0; i < count; i++)
        {
            int c = rng.Next(CableColors.Length);
            var pos = center + new Vector3(R(-radius, radius), 0, R(-radius, radius));
            Box("Cable_" + i, p, pos, new Vector3(0.025f, 0.025f, R(0.4f, 1.2f)), "Cable" + c, CableColors[c], rot: new Vector3(0, R(0, 180), 0));
        }
    }

    // 불규칙하게 튀는 스파크 + 같이 번쩍이는 작은 조명. rot 방향(+z)으로 튄다.
    static void Spark(string name, Transform parent, Vector3 pos, Vector3 rot)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(rot);

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 2.3f;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.01f, 0.025f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.9f, 0.5f), new Color(0.6f, 0.8f, 1f));
        main.gravityModifier = 1f;
        main.maxParticles = 60;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0.1f, 10),
            new ParticleSystem.Burst(0.25f, 5),
            new ParticleSystem.Burst(1.4f, 14),
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 45;
        shape.radius = 0.01f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 4;
        renderer.sharedMaterial = SparkMat();

        var light = AddLight(name + "_Light", go.transform, pos, new Color(0.7f, 0.8f, 1f), 1.5f, 1.5f);
        Flicker(light, 1.5f, 0.05f, 0.75f, null);
    }

    static Material SparkMat()
    {
        if (mats.TryGetValue("Spark", out var cached)) return cached;
        string path = MatFolder + "/Spark.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            m = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            AssetDatabase.CreateAsset(m, path);
        }
        m.SetColor("_BaseColor", new Color(2f, 1.8f, 1.2f)); // HDR로 블룸에 걸리게
        EditorUtility.SetDirty(m);
        mats["Spark"] = m;
        return m;
    }

    static Light AddLight(string name, Transform parent, Vector3 pos, Color color, float intensity, float range)
    {
        var l = new GameObject(name).AddComponent<Light>();
        l.transform.SetParent(parent);
        l.transform.position = pos;
        l.type = LightType.Point;
        l.color = color;
        l.intensity = intensity;
        l.range = range;
        return l;
    }

    static void Flicker(Light light, float max, float interval, float offChance, Renderer glow)
    {
        var f = light.gameObject.AddComponent<LightFlicker>();
        f.maxIntensity = max;
        f.interval = interval;
        f.offChance = offChance;
        f.glow = glow;
    }

    static Transform Group(string name, Transform parent)
    {
        var t = new GameObject(name).transform;
        t.SetParent(parent);
        return t;
    }

    static void Marker(string name, Transform parent, Vector3 pos)
    {
        var t = new GameObject(name).transform;
        t.SetParent(parent);
        t.position = pos;
    }

    static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 size, string mat, Color color, float emission = 0, Vector3 rot = default)
    {
        return Prim(PrimitiveType.Cube, name, parent, pos, size, Mat(mat, color, emission), rot);
    }

    static GameObject Cylinder(string name, Transform parent, Vector3 pos, Vector3 size, string mat, Color color, Vector3 rot = default)
    {
        // 원기둥 기본 높이가 2라서 y 스케일을 절반으로 맞춘다
        return Prim(PrimitiveType.Cylinder, name, parent, pos, new Vector3(size.x, size.y / 2, size.z), Mat(mat, color), rot);
    }

    static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, Vector3 rot)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(rot);
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        // 전선·나사·포스터처럼 얇은 장식은 걸려 넘어지지 않게 충돌체를 뺀다
        if (Mathf.Min(scale.x, Mathf.Min(scale.y, scale.z)) < 0.05f)
            Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }

    // 같은 이름의 머티리얼은 에셋 하나를 재사용한다 (재생성 시 색만 갱신)
    static Material Mat(string name, Color color, float emission = 0)
    {
        if (mats.TryGetValue(name, out var cached)) return cached;

        System.IO.Directory.CreateDirectory(MatFolder);
        string path = MatFolder + "/" + name + ".mat";
        var shader = Shader.Find("Universal Render Pipeline/Lit");

        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            m = new Material(shader);
            AssetDatabase.CreateAsset(m, path);
        }
        m.color = color;
        if (emission > 0)
        {
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            m.SetColor("_EmissionColor", color * emission);
        }
        else
        {
            m.DisableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", Color.black);
        }
        EditorUtility.SetDirty(m);
        mats[name] = m;
        return m;
    }
}
