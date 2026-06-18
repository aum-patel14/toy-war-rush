#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using TMPro;

public static class ToyWarRushSetup
{
    private const string Root = "Assets/_Game";

    [MenuItem("ToyWarRush/Setup Everything (One Click)")]
    public static void SetupEverything()
    {
        EnsureTag("Player");
        EnsureFolders();
        var unitData = CreateUnitDataAssets();
        var evolutionConfig = CreateEvolutionConfig(unitData);
        var levels = CreateLevelDataAssets();
        var adConfig = CreateAdConfig();
        var unitPrefab = CreateUnitPrefab(unitData[0]);
        var gatePrefab = CreateGatePrefab();
        var obstaclePrefab = CreateObstaclePrefab();
        var fortressPrefab = CreateFortressPrefab();
        CreateBootScene();
        CreateGameplayScene(evolutionConfig, levels, unitPrefab, gatePrefab, obstaclePrefab, fortressPrefab, adConfig);
        CreateMainMenuScene();
        SetupBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Toy War Rush",
            "Setup complete!\n\n1. Open Boot scene\n2. Press Play\n\nScenes, prefabs, and data assets are ready.",
            "OK");
    }

    private static void EnsureFolders()
    {
        string[] folders =
        {
            $"{Root}/ScriptableObjects/Units",
            $"{Root}/ScriptableObjects/Levels",
            $"{Root}/ScriptableObjects/Economy",
            $"{Root}/Prefabs/Units",
            $"{Root}/Prefabs/Gates",
            $"{Root}/Prefabs/Obstacles",
            $"{Root}/Prefabs/Level",
            $"{Root}/Scenes",
            $"{Root}/Materials"
        };
        foreach (var f in folders)
        {
            if (!AssetDatabase.IsValidFolder(f))
            {
                var parts = f.Split('/');
                AssetDatabase.CreateFolder(string.Join("/", parts, 0, parts.Length - 1), parts[^1]);
            }
        }
    }

    private static void EnsureTag(string tag)
    {
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tags = tagManager.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    private static UnitData[] CreateUnitDataAssets()
    {
        var specs = new (UnitTier tier, string name, int hp, int dmg, int size, float scale, UnitTier evolves)[]
        {
            (UnitTier.ToySoldier, "Toy Soldier", 10, 5, 1, 0.6f, UnitTier.Knight),
            (UnitTier.Knight, "Knight", 20, 10, 10, 0.7f, UnitTier.Robot),
            (UnitTier.Robot, "Robot", 35, 18, 25, 0.85f, UnitTier.Mech),
            (UnitTier.Mech, "Mech", 55, 30, 50, 1.0f, UnitTier.Titan),
            (UnitTier.Titan, "Titan", 90, 50, 100, 1.3f, UnitTier.UltraTitan),
            (UnitTier.UltraTitan, "Ultra Titan", 150, 80, 200, 1.6f, UnitTier.UltraTitan),
        };

        var results = new UnitData[specs.Length];
        for (int i = 0; i < specs.Length; i++)
        {
            var s = specs[i];
            string path = $"{Root}/ScriptableObjects/Units/UnitData_{s.tier}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<UnitData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<UnitData>();
                AssetDatabase.CreateAsset(asset, path);
            }
            asset.tier = s.tier;
            asset.displayName = s.name;
            asset.maxHP = s.hp;
            asset.attackDamage = s.dmg;
            asset.requiredArmySize = s.size;
            asset.scale = s.scale;
            asset.evolvesInto = s.evolves;
            EditorUtility.SetDirty(asset);
            results[i] = asset;
        }
        return results;
    }

    private static UnitEvolutionConfig CreateEvolutionConfig(UnitData[] unitData)
    {
        string path = $"{Root}/ScriptableObjects/Units/UnitEvolutionConfig.asset";
        var config = AssetDatabase.LoadAssetAtPath<UnitEvolutionConfig>(path);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<UnitEvolutionConfig>();
            AssetDatabase.CreateAsset(config, path);
        }
        config.tierData = unitData;
        EditorUtility.SetDirty(config);
        return config;
    }

    private static List<LevelData> CreateLevelDataAssets()
    {
        var levels = new List<LevelData>();
        var gateLayouts = new[]
        {
            new[] { (GateOperation.Add, 5f, 15f, 0f), (GateOperation.Multiply, 2f, 35f, 0f) },
            new[] { (GateOperation.Add, 10f, 12f, -1f), (GateOperation.Multiply, 2f, 40f, 1f) },
            new[] { (GateOperation.Add, 8f, 10f, 0f), (GateOperation.Subtract, 3f, 30f, 0f), (GateOperation.Multiply, 2f, 50f, 0f) },
            new[] { (GateOperation.Multiply, 2f, 20f, -1f), (GateOperation.Multiply, 2f, 45f, 1f) },
            new[] { (GateOperation.Add, 15f, 15f, 0f), (GateOperation.Divide, 2f, 35f, 0f), (GateOperation.Multiply, 3f, 55f, 0f) },
        };

        for (int i = 0; i < 5; i++)
        {
            int lvl = i + 1;
            string path = $"{Root}/ScriptableObjects/Levels/LevelData_{lvl:D3}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.levelNumber = lvl;
            asset.worldName = "Bedroom";
            asset.theme = WorldTheme.Bedroom;
            asset.startingArmySize = 5 + i;
            asset.fortressHP = 50 + i * 25;
            asset.fortressDefenderCount = 10 + i * 2;
            asset.coinsReward = 50 + i * 10;
            asset.threeStarRequirement = 80;
            asset.twoStarRequirement = 40;
            asset.gates = new List<GateData>();
            foreach (var g in gateLayouts[i])
            {
                asset.gates.Add(new GateData
                {
                    operation = g.Item1,
                    value = g.Item2,
                    zPosition = g.Item3,
                    xPosition = g.Item4
                });
            }
            asset.obstacles = new List<ObstacleData>
            {
                new() { type = ObstacleType.SockBall, zPosition = 25 + i * 5, xPosition = i % 2 == 0 ? 1f : -1f, unitDamage = 2 + i }
            };
            EditorUtility.SetDirty(asset);
            levels.Add(asset);
        }
        return levels;
    }

    private static AdPlacementConfig CreateAdConfig()
    {
        string path = $"{Root}/ScriptableObjects/Economy/AdPlacementConfig.asset";
        var asset = AssetDatabase.LoadAssetAtPath<AdPlacementConfig>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<AdPlacementConfig>();
            AssetDatabase.CreateAsset(asset, path);
        }
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static GameObject CreateUnitPrefab(UnitData data)
    {
        string path = $"{Root}/Prefabs/Units/Tier1_Unit.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = "Tier1_Unit";
        go.transform.localScale = Vector3.one * 0.6f;
        go.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.2f, 0.8f, 0.3f), "UnitGreen");
        var col = go.GetComponent<CapsuleCollider>();
        col.isTrigger = true;
        go.AddComponent<UnitController>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        data.prefab = prefab;
        EditorUtility.SetDirty(data);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateGatePrefab()
    {
        string path = $"{Root}/Prefabs/Gates/MathGate.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "MathGate";
        go.transform.localScale = new Vector3(2.5f, 2f, 0.3f);
        go.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.3f, 0.9f, 0.4f), "GatePositive");
        var col = go.GetComponent<BoxCollider>();
        col.isTrigger = true;

        var textGo = new GameObject("ValueText");
        textGo.transform.SetParent(go.transform);
        textGo.transform.localPosition = new Vector3(0, 0, -0.6f);
        textGo.transform.localRotation = Quaternion.Euler(0, 180, 0);
        var tmp = textGo.AddComponent<TextMeshPro>();
        tmp.text = "+10";
        tmp.fontSize = 4;
        tmp.alignment = TextAlignmentOptions.Center;

        var posMat = CreateColoredMaterial(new Color(0.3f, 0.9f, 0.4f), "GatePositive");
        var negMat = CreateColoredMaterial(new Color(0.9f, 0.3f, 0.3f), "GateNegative");
        var gate = go.AddComponent<GateController>();
        var so = new SerializedObject(gate);
        so.FindProperty("positiveMat").objectReferenceValue = posMat;
        so.FindProperty("negativeMat").objectReferenceValue = negMat;
        so.FindProperty("valueText").objectReferenceValue = tmp;
        so.FindProperty("gateRenderer").objectReferenceValue = go.GetComponent<MeshRenderer>();
        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateObstaclePrefab()
    {
        string path = $"{Root}/Prefabs/Obstacles/Obstacle_Generic.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Obstacle_Generic";
        go.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        go.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.9f, 0.5f, 0.2f), "ObstacleOrange");
        go.GetComponent<BoxCollider>().isTrigger = true;
        go.AddComponent<ObstacleController>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateFortressPrefab()
    {
        string path = $"{Root}/Prefabs/Level/Fortress.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Fortress";
        go.transform.localScale = new Vector3(4f, 3f, 2f);
        go.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.6f, 0.2f, 0.2f), "FortressRed");
        var col = go.GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.2f, 1.2f, 1.2f);
        go.AddComponent<FortressController>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static Material CreateColoredMaterial(Color color, string name)
    {
        string path = $"{Root}/Materials/{name}.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        mat = new Material(shader) { color = color };
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static void CreateBootScene()
    {
        string path = $"{Root}/Scenes/Boot.unity";
        if (File.Exists(path)) return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateManager<SaveManager>("SaveManager");
        CreateManager<GameManager>("GameManager");
        CreateManager<AudioManager>("AudioManager");
        CreateManager<AnalyticsManager>("AnalyticsManager");
        CreateManager<AdManager>("AdManager");
        CreateManager<CurrencyManager>("CurrencyManager");
        CreateManager<RewardSystem>("RewardSystem");
        CreateManager<CloudSave>("CloudSave");
        var boot = new GameObject("BootLoader");
        boot.AddComponent<BootLoader>();
        EditorSceneManager.SaveScene(scene, path);
    }

    private static void CreateGameplayScene(UnitEvolutionConfig evolutionConfig, List<LevelData> levels,
        GameObject unitPrefab, GameObject gatePrefab, GameObject obstaclePrefab,
        GameObject fortressPrefab, AdPlacementConfig adConfig)
    {
        string path = $"{Root}/Scenes/Gameplay.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(3, 1, 10);
        ground.isStatic = true;

        var player = new GameObject("Player");
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider>();
        playerCol.isTrigger = true;
        playerCol.size = new Vector3(4f, 2f, 2f);
        playerCol.center = new Vector3(0, 1f, 0);
        player.AddComponent<PlayerController>();
        var formation = new GameObject("FormationRoot");
        formation.transform.SetParent(player.transform);
        formation.transform.localPosition = Vector3.zero;

        var army = CreateManager<ArmyManager>("ArmyManager");
        SetSerializedField(army, "evolutionConfig", evolutionConfig);
        SetSerializedField(army, "formationRoot", formation.transform);

        CreateManager<UnitFactory>("UnitFactory");
        SetSerializedField(GameObject.Find("UnitFactory"), "evolutionConfig", evolutionConfig);
        SetSerializedField(GameObject.Find("UnitFactory"), "defaultUnitPrefab", unitPrefab);

        CreateManager<UnitEvolutionManager>("UnitEvolutionManager");
        CreateManager<MergeSystem>("MergeSystem");
        CreateManager<FXManager>("FXManager");
        CreateManager<ObjectPoolManager>("ObjectPoolManager");

        var levelMgr = CreateManager<LevelManager>("LevelManager");
        SetSerializedField(levelMgr, "levels", levels);
        SetSerializedField(levelMgr, "gatePrefab", gatePrefab);
        SetSerializedField(levelMgr, "obstaclePrefab", obstaclePrefab);

        var adMgr = GameObject.Find("AdManager") ?? CreateManager<AdManager>("AdManager");
        SetSerializedField(adMgr, "config", adConfig);

        var fortress = (GameObject)PrefabUtility.InstantiatePrefab(fortressPrefab);
        fortress.transform.position = new Vector3(0, 1.5f, 80f);
        fortress.GetComponent<FortressController>()?.Initialize(100);

        var canvas = new GameObject("UI");
        var canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        var uiMgr = canvas.AddComponent<UIManager>();
        var hud = new GameObject("HUD");
        hud.transform.SetParent(canvas.transform);
        hud.AddComponent<HUDController>();

        var result = new GameObject("ResultScreen");
        result.transform.SetParent(canvas.transform);
        result.AddComponent<ResultScreenUI>();
        result.SetActive(false);

        SetSerializedField(uiMgr.gameObject, "hud", hud.GetComponent<HUDController>());
        SetSerializedField(uiMgr.gameObject, "resultScreen", result.GetComponent<ResultScreenUI>());

        EditorSceneManager.SaveScene(scene, path);
    }

    private static void CreateMainMenuScene()
    {
        string path = $"{Root}/Scenes/MainMenu.unity";
        if (File.Exists(path)) return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var canvas = new GameObject("UI");
        canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        canvas.AddComponent<MainMenuUI>();
        EditorSceneManager.SaveScene(scene, path);
    }

    private static GameObject CreateManager<T>(string name) where T : Component
    {
        var go = new GameObject(name);
        go.AddComponent<T>();
        return go;
    }

    private static void SetSerializedField(GameObject go, string fieldName, object value)
    {
        if (go == null) return;
        var comp = go.GetComponent<MonoBehaviour>();
        if (comp == null) return;
        var so = new SerializedObject(comp);
        var prop = so.FindProperty(fieldName);
        if (prop == null) return;
        switch (value)
        {
            case Object obj:
                prop.objectReferenceValue = obj;
                break;
            case List<LevelData> list:
                prop.ClearArray();
                for (int i = 0; i < list.Count; i++)
                {
                    prop.InsertArrayElementAtIndex(i);
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = list[i];
                }
                break;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetupBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene($"{Root}/Scenes/Boot.unity", true),
            new EditorBuildSettingsScene($"{Root}/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene($"{Root}/Scenes/Gameplay.unity", true),
        };
    }
}
#endif
