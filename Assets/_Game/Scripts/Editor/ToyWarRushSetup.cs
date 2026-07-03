#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
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
        var unitPrefabs = CreateAllUnitPrefabs(unitData);
        var levels = CreateLevelDataAssets();
        var adConfig = CreateAdConfig();
        var visualLibrary = CreateVisualAssetLibrary();
        var upgradeData = CreateUpgradeDataAssets();
        var gatePrefab = CreateGatePrefab();
        var obstaclePrefab = CreateObstaclePrefab();
        var obstacleEntries = CreateTypedObstaclePrefabs();
        var collectiblePrefab = CreateCollectiblePrefab();
        var fortressPrefab = CreateFortressPrefab();
        var fxPrefabs = CreateFXPrefabs();
        CreateBootScene();
        CreateGameplayScene(evolutionConfig, levels, unitPrefabs[0], gatePrefab, obstaclePrefab, obstacleEntries, collectiblePrefab, fortressPrefab, fxPrefabs, adConfig, upgradeData, visualLibrary);
        CreateMainMenuScene(upgradeData);
        SetupBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("Toy War Rush",
                "Setup complete!\n\n1. Open Boot scene\n2. Press Play\n\nScenes, prefabs, and data assets are ready.",
                "OK");
        }
        else
        {
            Debug.Log("[ToyWarRush] Setup complete. Open Boot.unity and press Play.");
        }
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
            $"{Root}/Materials",
            $"{Root}/ScriptableObjects/Visual"
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
            new[] { (GateOperation.Add, 5f, 50f, 0f), (GateOperation.Multiply, 2f, 115f, -1f), (GateOperation.Add, 8f, 180f, 1f) },
            new[] { (GateOperation.Add, 10f, 45f, 0f), (GateOperation.Multiply, 2f, 110f, -1f), (GateOperation.Add, 12f, 175f, 1f) },
            new[] { (GateOperation.Add, 8f, 50f, 0f), (GateOperation.Subtract, 3f, 120f, 0f), (GateOperation.Multiply, 2f, 190f, -0.6f) },
            new[] { (GateOperation.Multiply, 2f, 55f, -1f), (GateOperation.Add, 15f, 120f, 0f), (GateOperation.Multiply, 2f, 185f, 1f) },
            new[] { (GateOperation.Add, 12f, 50f, 0f), (GateOperation.Subtract, 4f, 115f, 0f), (GateOperation.Multiply, 2f, 180f, -0.7f), (GateOperation.Add, 20f, 245f, 0.5f) },
            new[] { (GateOperation.Add, 10f, 48f, 0f), (GateOperation.Multiply, 2f, 112f, 1f), (GateOperation.Subtract, 5f, 176f, 0f), (GateOperation.Multiply, 2f, 240f, -0.5f) },
            new[] { (GateOperation.Multiply, 2f, 52f, -1f), (GateOperation.Add, 18f, 118f, 0f), (GateOperation.Subtract, 6f, 184f, 0f), (GateOperation.Multiply, 3f, 250f, 0f) },
            new[] { (GateOperation.Add, 15f, 50f, 0f), (GateOperation.Multiply, 2f, 115f, -1f), (GateOperation.Subtract, 5f, 180f, 1f), (GateOperation.Add, 25f, 245f, 0f) },
            new[] { (GateOperation.Multiply, 2f, 48f, 0f), (GateOperation.Add, 20f, 113f, -1f), (GateOperation.Subtract, 7f, 178f, 0f), (GateOperation.Multiply, 2f, 243f, 1f) },
            new[] { (GateOperation.Add, 25f, 45f, 0f), (GateOperation.Multiply, 2f, 110f, -1f), (GateOperation.Subtract, 8f, 175f, 0f), (GateOperation.Multiply, 3f, 240f, 0.5f), (GateOperation.Add, 30f, 305f, 0f) },
        };

        for (int i = 0; i < 30; i++)
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
            asset.worldName = lvl <= 10 ? "Desert Highway" : lvl <= 20 ? "Cave" : "Outdoor";
            asset.theme = lvl <= 10 ? WorldTheme.Garden : lvl <= 20 ? WorldTheme.Garage : WorldTheme.Space;
            asset.startingArmySize = 4 + i % 10;
            asset.fortressHP = 40 + i * 8 + (i % 10 == 4 || i % 10 == 9 ? 30 : 0);
            asset.fortressDefenderCount = 8 + i * 2;
            asset.coinsReward = 50 + i * 20;
            asset.threeStarRequirement = 75;
            asset.twoStarRequirement = 45;
            asset.hasBoss = i % 10 == 4 || i % 10 == 9;
            asset.gates = new List<GateData>();
            foreach (var g in gateLayouts[i % 10])
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
                new() { type = ObstacleType.SockBall, zPosition = 85 + i * 3, xPosition = i % 2 == 0 ? 1.2f : -1.2f, unitDamage = 2 + i / 2 },
                new() { type = ObstacleType.ToyTrain, zPosition = 150 + i * 2, xPosition = i % 2 == 0 ? -1f : 1f, unitDamage = 3 + i / 2 },
            };
            if (i >= 2)
                asset.obstacles.Add(new ObstacleData { type = ObstacleType.Cat, zPosition = 215 + i, xPosition = 0.3f, unitDamage = 3 + i });
            asset.collectibles = new List<CollectibleData>
            {
                new() { zPosition = 100, xPosition = -1.4f, armyBonus = 1 },
                new() { zPosition = 165, xPosition = 1.3f, armyBonus = 1 },
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

    private static VisualAssetLibrary CreateVisualAssetLibrary()
    {
        string path = $"{Root}/ScriptableObjects/Visual/VisualAssetLibrary.asset";
        var asset = AssetDatabase.LoadAssetAtPath<VisualAssetLibrary>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<VisualAssetLibrary>();
            AssetDatabase.CreateAsset(asset, path);
        }
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static UpgradeData[] CreateUpgradeDataAssets()
    {
        var specs = new (string id, string name, int cost)[]
        {
            ("fire_rate", "Cannon Fire Rate", 100),
            ("start_army", "Starting Army", 150),
            ("gate_bonus", "Gate Bonus +10%", 120),
        };
        var results = new UpgradeData[specs.Length];
        for (int i = 0; i < specs.Length; i++)
        {
            string path = $"{Root}/ScriptableObjects/Economy/Upgrade_{specs[i].id}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<UpgradeData>();
                AssetDatabase.CreateAsset(asset, path);
            }
            asset.upgradeId = specs[i].id;
            asset.displayName = specs[i].name;
            asset.baseCost = specs[i].cost;
            asset.valuePerLevel = 1f;
            EditorUtility.SetDirty(asset);
            results[i] = asset;
        }
        return results;
    }

    private static GameObject CreateProjectilePrefab()
    {
        string path = $"{Root}/Prefabs/Units/CannonProjectile.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "CannonProjectile";
        go.transform.localScale = Vector3.one * 0.35f;
        go.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.2f, 0.55f, 0.95f), "ProjectileBlue");
        Object.DestroyImmediate(go.GetComponent<SphereCollider>());
        go.AddComponent<CannonProjectile>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateEnemyUnitPrefab()
    {
        string path = $"{Root}/Prefabs/Units/EnemyUnit.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = new GameObject("EnemyUnit");
        var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body.name = "Body";
        body.transform.SetParent(go.transform, false);
        body.transform.localScale = new Vector3(0.62f, 0.76f, 0.62f);
        body.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.92f, 0.22f, 0.18f), "EnemyRed");
        Object.DestroyImmediate(body.GetComponent<SphereCollider>());

        var eyeL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eyeL.transform.SetParent(go.transform, false);
        eyeL.transform.localScale = Vector3.one * 0.08f;
        eyeL.transform.localPosition = new Vector3(-0.08f, 0.1f, 0.22f);
        eyeL.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(1f, 0.95f, 0.95f), "EnemyEye");
        Object.DestroyImmediate(eyeL.GetComponent<SphereCollider>());

        var eyeR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eyeR.transform.SetParent(go.transform, false);
        eyeR.transform.localScale = Vector3.one * 0.08f;
        eyeR.transform.localPosition = new Vector3(0.08f, 0.1f, 0.22f);
        eyeR.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(1f, 0.95f, 0.95f), "EnemyEye");
        Object.DestroyImmediate(eyeR.GetComponent<SphereCollider>());

        var hornL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        hornL.transform.SetParent(go.transform, false);
        hornL.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);
        hornL.transform.localPosition = new Vector3(-0.12f, 0.32f, -0.04f);
        hornL.transform.localRotation = Quaternion.Euler(20f, 0f, 25f);
        hornL.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.7f, 0.12f, 0.1f), "EnemyHorn");
        Object.DestroyImmediate(hornL.GetComponent<CapsuleCollider>());

        var hornR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        hornR.transform.SetParent(go.transform, false);
        hornR.transform.localScale = new Vector3(0.04f, 0.08f, 0.04f);
        hornR.transform.localPosition = new Vector3(0.12f, 0.32f, -0.04f);
        hornR.transform.localRotation = Quaternion.Euler(20f, 0f, -25f);
        hornR.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.7f, 0.12f, 0.1f), "EnemyHorn");
        Object.DestroyImmediate(hornR.GetComponent<CapsuleCollider>());

        go.name = "EnemyUnit";
        go.transform.localScale = new Vector3(0.64f, 0.64f, 0.64f);
        var rootCol = go.AddComponent<CapsuleCollider>();
        rootCol.isTrigger = true;
        rootCol.center = new Vector3(0f, 0.1f, 0f);
        rootCol.height = 0.9f;
        rootCol.radius = 0.28f;
        go.AddComponent<EnemyUnitController>();
        var enemyAnim = go.AddComponent<MobVisualAnimator>();
        var enemySo = new SerializedObject(enemyAnim);
        enemySo.FindProperty("enemyStyle").boolValue = true;
        enemySo.ApplyModifiedPropertiesWithoutUndo();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject[] CreateAllUnitPrefabs(UnitData[] unitData)
    {
        var colors = new[]
        {
            new Color(0.39f, 0.71f, 0.96f),
            new Color(0.26f, 0.65f, 0.96f),
            new Color(0.12f, 0.53f, 0.90f),
            new Color(0.49f, 0.34f, 0.76f),
            new Color(1f, 0.65f, 0.15f),
            new Color(1f, 0.84f, 0.31f),
        };
        var prefabs = new GameObject[unitData.Length];
        for (int i = 0; i < unitData.Length; i++)
        {
            string path = $"{Root}/Prefabs/Units/Tier{(i + 1)}_Unit.prefab";
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                unitData[i].prefab = existing;
                EditorUtility.SetDirty(unitData[i]);
                prefabs[i] = existing;
                continue;
            }
            var go = new GameObject($"Tier{i + 1}_Unit");
            go.name = $"Tier{i + 1}_Unit";
            go.transform.localScale = Vector3.one * unitData[i].scale;
            var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Body";
            body.transform.SetParent(go.transform, false);
            body.transform.localScale = new Vector3(0.68f, 0.82f, 0.68f);
            body.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(colors[i], $"UnitTier{i + 1}");
            Object.DestroyImmediate(body.GetComponent<SphereCollider>());

            var eyeL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeL.transform.SetParent(go.transform, false);
            eyeL.transform.localScale = Vector3.one * 0.08f;
            eyeL.transform.localPosition = new Vector3(-0.09f, 0.1f, 0.24f);
            eyeL.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.92f, 0.97f, 1f), $"UnitEye{i + 1}");
            Object.DestroyImmediate(eyeL.GetComponent<SphereCollider>());

            var eyeR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeR.transform.SetParent(go.transform, false);
            eyeR.transform.localScale = Vector3.one * 0.08f;
            eyeR.transform.localPosition = new Vector3(0.09f, 0.1f, 0.24f);
            eyeR.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.92f, 0.97f, 1f), $"UnitEye{i + 1}");
            Object.DestroyImmediate(eyeR.GetComponent<SphereCollider>());

            var stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.transform.SetParent(go.transform, false);
            stripe.transform.localScale = new Vector3(0.18f, 0.05f, 0.5f);
            stripe.transform.localPosition = new Vector3(0f, -0.12f, 0.03f);
            stripe.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.05f, 0.27f, 0.54f), $"UnitStripe{i + 1}");
            Object.DestroyImmediate(stripe.GetComponent<BoxCollider>());

            var rootCol = go.AddComponent<CapsuleCollider>();
            rootCol.isTrigger = true;
            rootCol.center = new Vector3(0f, 0.08f, 0f);
            rootCol.height = 0.95f;
            rootCol.radius = 0.3f;
            go.AddComponent<MobVisualAnimator>();
            go.AddComponent<UnitController>();
            prefabs[i] = PrefabUtility.SaveAsPrefabAsset(go, path);
            unitData[i].prefab = prefabs[i];
            EditorUtility.SetDirty(unitData[i]);
            Object.DestroyImmediate(go);
        }
        return prefabs;
    }

    private static GameObject CreateUnitPrefab(UnitData data)
    {
        return CreateAllUnitPrefabs(new[] { data })[0];
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

    private static GameObject CreateCollectiblePrefab()
    {
        string path = $"{Root}/Prefabs/Level/Collectible.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Collectible";
        go.transform.localScale = Vector3.one * 0.6f;
        go.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(1f, 0.9f, 0.43f), "CollectibleGold");
        go.GetComponent<SphereCollider>().isTrigger = true;
        go.AddComponent<CollectibleController>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static List<LevelManager.ObstaclePrefabEntry> CreateTypedObstaclePrefabs()
    {
        var entries = new List<LevelManager.ObstaclePrefabEntry>();
        entries.Add(new LevelManager.ObstaclePrefabEntry
        {
            type = ObstacleType.SockBall,
            prefab = CreateObstacleVariant("Obstacle_Sock", new Color(0.95f, 0.25f, 0.3f), new Vector3(1.4f, 0.8f, 1.2f))
        });
        entries.Add(new LevelManager.ObstaclePrefabEntry
        {
            type = ObstacleType.ToyTrain,
            prefab = CreateObstacleVariant("Obstacle_Train", new Color(0.2f, 0.55f, 0.9f), new Vector3(2f, 1f, 1.5f))
        });
        entries.Add(new LevelManager.ObstaclePrefabEntry
        {
            type = ObstacleType.Cat,
            prefab = CreateObstacleVariant("Obstacle_Cat", new Color(1f, 0.65f, 0.45f), new Vector3(1.1f, 1.1f, 1.1f))
        });
        return entries;
    }

    private static GameObject CreateObstacleVariant(string name, Color color, Vector3 scale)
    {
        string path = $"{Root}/Prefabs/Obstacles/{name}.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(color, name + "Mat");
        go.GetComponent<BoxCollider>().isTrigger = true;
        if (go.GetComponent<ObstacleController>() == null)
            go.AddComponent<ObstacleController>();
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

        var root = new GameObject("Fortress");
        var cols = new[]
        {
            new Color(1f, 0.42f, 0.21f),
            new Color(1f, 0.9f, 0.43f),
            new Color(0.31f, 0.8f, 0.77f),
            new Color(1f, 0.52f, 0.64f)
        };
        int i = 0;
        for (int row = 0; row < 3; row++)
        for (int ci = 0; ci < 4; ci++)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = $"Block_{row}_{ci}";
            block.transform.SetParent(root.transform);
            block.transform.localScale = new Vector3(0.9f, 0.7f, 0.8f);
            block.transform.localPosition = new Vector3((ci - 1.5f) * 1f, row * 0.75f + 0.5f, 0f);
            block.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(cols[i % cols.Length], $"FortBlock{i}");
            Object.DestroyImmediate(block.GetComponent<BoxCollider>());
            i++;
        }
        var boxCol = root.AddComponent<BoxCollider>();
        boxCol.isTrigger = true;
        boxCol.size = new Vector3(4.5f, 3f, 2f);
        boxCol.center = new Vector3(0f, 1.5f, 0f);
        root.AddComponent<FortressController>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static FXManager.FXEntry[] CreateFXPrefabs()
    {
        return new[]
        {
            new FXManager.FXEntry { name = "CannonFire", prefab = CreateBurstFX("FX_CannonFire", new Color(0.5f, 0.85f, 1f)) },
            new FXManager.FXEntry { name = "CannonImpact", prefab = CreateBurstFX("FX_CannonImpact", new Color(0.2f, 0.7f, 1f)) },
            new FXManager.FXEntry { name = "GateHit", prefab = CreateBurstFX("FX_GateHit", new Color(0.4f, 0.9f, 0.5f)) },
            new FXManager.FXEntry { name = "CollectPickup", prefab = CreateBurstFX("FX_Collect", new Color(1f, 0.9f, 0.4f)) },
            new FXManager.FXEntry { name = "ObstacleHit", prefab = CreateBurstFX("FX_ObstacleHit", new Color(1f, 0.3f, 0.3f)) },
            new FXManager.FXEntry { name = "FortressHit", prefab = CreateBurstFX("FX_FortressHit", new Color(1f, 0.5f, 0.2f)) },
        };
    }

    private static GameObject CreateBurstFX(string name, Color color)
    {
        string path = $"{Root}/Prefabs/FX/{name}.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        if (!AssetDatabase.IsValidFolder($"{Root}/Prefabs/FX"))
            AssetDatabase.CreateFolder($"{Root}/Prefabs", "FX");

        var go = new GameObject(name);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.35f;
        main.startLifetime = 0.45f;
        main.startSpeed = 5f;
        main.startSize = 0.35f;
        main.startColor = color;
        main.loop = false;
        main.maxParticles = 40;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 30) });
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;
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
        List<LevelManager.ObstaclePrefabEntry> obstacleEntries, GameObject collectiblePrefab,
        GameObject fortressPrefab, FXManager.FXEntry[] fxEntries, AdPlacementConfig adConfig,
        UpgradeData[] upgradeData, VisualAssetLibrary visualLibrary)
    {
        string path = $"{Root}/Scenes/Gameplay.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var asphalt = new Color(0.42f, 0.44f, 0.46f);
        var sand = new Color(0.86f, 0.73f, 0.52f);

        var laneMat = visualLibrary != null && visualLibrary.LaneMaterialOverride != null
            ? visualLibrary.LaneMaterialOverride
            : CreateColoredMaterial(asphalt, "LaneGround");
        var sideMat = visualLibrary != null && visualLibrary.SideMaterialOverride != null
            ? visualLibrary.SideMaterialOverride
            : CreateColoredMaterial(sand, "SandDesert");

        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.79f, 0.72f, 0.6f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 40f;
        RenderSettings.fogEndDistance = 240f;

        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(3, 1, 15);
        ground.isStatic = true;
        ground.GetComponent<Renderer>().sharedMaterial = laneMat;

        var sandL = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sandL.name = "SandLeft";
        sandL.transform.localScale = new Vector3(4f, 1f, 25f);
        sandL.transform.position = new Vector3(-12f, 0f, 35f);
        sandL.GetComponent<Renderer>().sharedMaterial = sideMat;

        var sandR = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sandR.name = "SandRight";
        sandR.transform.localScale = new Vector3(4f, 1f, 25f);
        sandR.transform.position = new Vector3(12f, 0f, 35f);
        sandR.GetComponent<Renderer>().sharedMaterial = sideMat;

        for (int i = 0; i < 12; i++)
        {
            var dash = GameObject.CreatePrimitive(PrimitiveType.Plane);
            dash.name = $"Dash_{i}";
            dash.transform.localScale = new Vector3(0.06f, 1f, 1.2f);
            dash.transform.position = new Vector3(0, 0.02f, 6f + i * 7f);
            dash.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(1f, 1f, 1f, 0.85f), $"Dash{i}");
            Object.DestroyImmediate(dash.GetComponent<Collider>());
        }

        var bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bush.name = "Decor_Bush";
        bush.transform.position = new Vector3(-5f, 0.4f, 30f);
        bush.transform.localScale = new Vector3(1.5f, 0.8f, 1.2f);
        bush.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.35f, 0.55f, 0.28f), "DesertBush");
        Object.DestroyImmediate(bush.GetComponent<Collider>());

        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0, 0, 0);
        var playerCol = player.AddComponent<BoxCollider>();
        playerCol.isTrigger = true;
        playerCol.size = new Vector3(4f, 2f, 2f);
        playerCol.center = new Vector3(0, 1f, 0);
        player.AddComponent<PlayerController>();

        GameObject cannon;
        if (visualLibrary != null && visualLibrary.CannonPrefabOverride != null)
        {
            cannon = PrefabUtility.InstantiatePrefab(visualLibrary.CannonPrefabOverride) as GameObject;
            cannon.name = "Cannon";
            cannon.transform.SetParent(player.transform);
            cannon.transform.localPosition = new Vector3(0, 0.6f, -1.2f);
            cannon.transform.localRotation = Quaternion.identity;
            cannon.transform.localScale = Vector3.one;
        }
        else
        {
            cannon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cannon.name = "Cannon";
            cannon.transform.SetParent(player.transform);
            cannon.transform.localPosition = new Vector3(0, 0.6f, -1.2f);
            cannon.transform.localScale = new Vector3(0.7f, 0.5f, 0.7f);
            cannon.GetComponent<Renderer>().sharedMaterial = CreateColoredMaterial(new Color(0.35f, 0.35f, 0.38f), "CannonGrey");
            Object.DestroyImmediate(cannon.GetComponent<Collider>());
        }

        var muzzle = new GameObject("Muzzle");
        muzzle.transform.SetParent(cannon.transform);
        muzzle.transform.localPosition = new Vector3(0, 0.6f, 0.5f);

        var formation = new GameObject("FormationRoot");
        formation.transform.SetParent(player.transform);
        formation.transform.localPosition = new Vector3(0, 0, 1.5f);

        var projectilePrefab = visualLibrary != null && visualLibrary.ProjectilePrefabOverride != null
            ? visualLibrary.ProjectilePrefabOverride
            : CreateProjectilePrefab();
        var shooter = player.AddComponent<CannonShooter>();
        var shooterSo = new SerializedObject(shooter);
        shooterSo.FindProperty("projectilePrefab").objectReferenceValue = projectilePrefab;
        shooterSo.FindProperty("muzzle").objectReferenceValue = muzzle.transform;
        shooterSo.FindProperty("formationTarget").objectReferenceValue = formation.transform;
        shooterSo.ApplyModifiedPropertiesWithoutUndo();

        var army = CreateManager<ArmyManager>("ArmyManager");
        SetSerializedField(army, "evolutionConfig", evolutionConfig);
        SetSerializedField(army, "formationRoot", formation.transform);

        CreateManager<UnitFactory>("UnitFactory");
        SetSerializedField(GameObject.Find("UnitFactory"), "evolutionConfig", evolutionConfig);
        var visualUnitPrefab = visualLibrary != null && visualLibrary.UnitPrefabOverride != null
            ? visualLibrary.UnitPrefabOverride
            : unitPrefab;
        SetSerializedField(GameObject.Find("UnitFactory"), "defaultUnitPrefab", visualUnitPrefab);

        CreateManager<UnitEvolutionManager>("UnitEvolutionManager");
        CreateManager<MergeSystem>("MergeSystem");
        var fxMgr = CreateManager<FXManager>("FXManager");
        WireFXManager(fxMgr, fxEntries);
        CreateManager<ObjectPoolManager>("ObjectPoolManager");

        var enemyPrefab = visualLibrary != null && visualLibrary.EnemyUnitPrefabOverride != null
            ? visualLibrary.EnemyUnitPrefabOverride
            : CreateEnemyUnitPrefab();
        var enemyRoot = new GameObject("EnemyRoot");
        var enemyMgr = CreateManager<EnemyArmyManager>("EnemyArmyManager");
        SetSerializedField(enemyMgr, "enemyUnitPrefab", enemyPrefab);
        SetSerializedField(enemyMgr, "enemyRoot", enemyRoot.transform);

        var levelMgr = CreateManager<LevelManager>("LevelManager");
        SetSerializedField(levelMgr, "levels", levels);
        var visualGatePrefab = visualLibrary != null && visualLibrary.GatePrefabOverride != null
            ? visualLibrary.GatePrefabOverride
            : gatePrefab;
        var visualObstaclePrefab = visualLibrary != null && visualLibrary.ObstaclePrefabOverride != null
            ? visualLibrary.ObstaclePrefabOverride
            : obstaclePrefab;
        var visualCollectiblePrefab = visualLibrary != null && visualLibrary.CollectiblePrefabOverride != null
            ? visualLibrary.CollectiblePrefabOverride
            : collectiblePrefab;
        var visualFortressPrefab = visualLibrary != null && visualLibrary.FortressPrefabOverride != null
            ? visualLibrary.FortressPrefabOverride
            : fortressPrefab;
        SetSerializedField(levelMgr, "gatePrefab", visualGatePrefab);
        SetSerializedField(levelMgr, "obstaclePrefab", visualObstaclePrefab);
        SetObstaclePrefabs(levelMgr, obstacleEntries);
        SetSerializedField(levelMgr, "collectiblePrefab", visualCollectiblePrefab);
        SetSerializedField(levelMgr, "fortressPrefab", visualFortressPrefab);

        var adMgr = GameObject.Find("AdManager") ?? CreateManager<AdManager>("AdManager");
        SetSerializedField(adMgr, "config", adConfig);

        var cam = Camera.main;
        if (cam != null)
        {
            cam.gameObject.AddComponent<CameraFollow>();
            cam.transform.position = new Vector3(0, 12, -10);
            cam.backgroundColor = new Color(0.77f, 0.7f, 0.57f);
            cam.fieldOfView = 52f;
        }

        var sun = Object.FindFirstObjectByType<Light>();
        if (sun != null)
        {
            sun.color = new Color(1f, 0.96f, 0.88f);
            sun.intensity = 1.2f;
            sun.transform.rotation = Quaternion.Euler(42f, -32f, 0f);
        }

        var canvas = new GameObject("UI");
        var canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        var uiMgr = canvas.AddComponent<UIManager>();
        var hud = new GameObject("HUD");
        hud.transform.SetParent(canvas.transform, false);
        var hudRect = hud.AddComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.offsetMin = hudRect.offsetMax = Vector2.zero;
        var hudCtrl = hud.AddComponent<HUDController>();

        var armyText = CreateTMP(hud.transform, "ArmyCount", new Vector2(0.05f, 0.92f), new Vector2(0.35f, 0.98f), "5", 48);
        var tierText = CreateTMP(hud.transform, "Tier", new Vector2(0.05f, 0.86f), new Vector2(0.45f, 0.91f), "TOY SOLDIER", 24);
        var coinsText = CreateTMP(hud.transform, "Coins", new Vector2(0.72f, 0.86f), new Vector2(0.95f, 0.91f), "0", 24);
        var levelText = CreateTMP(hud.transform, "Level", new Vector2(0.65f, 0.92f), new Vector2(0.95f, 0.98f), "Level 1", 32);
        var progressBg = CreateUIImage(hud.transform, "ProgressBg", new Vector2(0.05f, 0.82f), new Vector2(0.95f, 0.85f), new Color(1, 1, 1));
        var progressFill = CreateUIImage(hud.transform, "ProgressFill", new Vector2(0.05f, 0.82f), new Vector2(0.95f, 0.85f), new Color(0.42f, 0.8f, 0.46f));
        progressFill.type = UnityEngine.UI.Image.Type.Filled;
        progressFill.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        progressFill.fillAmount = 0f;

        SetSerializedField(hudCtrl.gameObject, "armyCountText", armyText);
        SetSerializedField(hudCtrl.gameObject, "tierText", tierText);
        SetSerializedField(hudCtrl.gameObject, "coinsText", coinsText);
        SetSerializedField(hudCtrl.gameObject, "levelText", levelText);
        SetSerializedField(hudCtrl.gameObject, "progressFill", progressFill);

        var result = new GameObject("ResultScreen");
        result.transform.SetParent(canvas.transform, false);
        var resultUI = result.AddComponent<ResultScreenUI>();
        var victory = CreateResultPanel(result.transform, "VictoryPanel", "YOU WIN!", new Color(0.2f, 0.8f, 0.3f, 0.92f), resultUI, true);
        var defeat = CreateResultPanel(result.transform, "DefeatPanel", "TRY AGAIN!", new Color(0.9f, 0.3f, 0.3f, 0.92f), resultUI, false);
        defeat.SetActive(false);
        SetSerializedField(resultUI.gameObject, "victoryPanel", victory);
        SetSerializedField(resultUI.gameObject, "defeatPanel", defeat);
        var starsText = victory.transform.Find("StarsText")?.GetComponent<TextMeshProUGUI>();
        var resultCoinsText = victory.transform.Find("CoinsText")?.GetComponent<TextMeshProUGUI>();
        if (starsText != null) SetSerializedField(resultUI.gameObject, "starsText", starsText);
        if (resultCoinsText != null) SetSerializedField(resultUI.gameObject, "coinsRewardText", resultCoinsText);
        result.SetActive(false);

        SetSerializedField(uiMgr.gameObject, "hud", hudCtrl);
        SetSerializedField(uiMgr.gameObject, "resultScreen", resultUI);

        var upgradePanel = CreateUpgradePanel(canvas.transform, upgradeData);
        upgradePanel.SetActive(false);
        SetSerializedField(uiMgr.gameObject, "upgradeMenu", upgradePanel.GetComponent<UpgradeMenuUI>());

        var loader = new GameObject("GameplaySceneLoader");
        loader.AddComponent<GameplaySceneLoader>();

        EditorSceneManager.SaveScene(scene, path);
    }

    private static TextMeshProUGUI CreateTMP(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string text, float fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return tmp;
    }

    private static UnityEngine.UI.Image CreateUIImage(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var img = go.AddComponent<UnityEngine.UI.Image>();
        img.color = color;
        return img;
    }

    private static GameObject CreateResultPanel(Transform parent, string name, string label, Color bg, ResultScreenUI resultUI, bool isVictory)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        go.AddComponent<UnityEngine.UI.Image>().color = bg;
        CreateTMP(go.transform, "Label", new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.72f), label, 56);
        CreateTMP(go.transform, "StarsText", new Vector2(0.1f, 0.45f), new Vector2(0.9f, 0.54f), "⭐⭐⭐", 40).gameObject.name = "StarsText";
        CreateTMP(go.transform, "CoinsText", new Vector2(0.1f, 0.36f), new Vector2(0.9f, 0.44f), "+50", 32).gameObject.name = "CoinsText";

        if (isVictory)
        {
            var nextBtn = CreateResultButton(go.transform, "NextBtn", new Vector2(0.15f, 0.18f), new Vector2(0.85f, 0.28f), "Next Level ➡", resultUI, "OnNextLevel");
            CreateResultButton(go.transform, "MenuBtn", new Vector2(0.25f, 0.08f), new Vector2(0.75f, 0.16f), "Home", resultUI, "OnMainMenu");
        }
        else
        {
            CreateResultButton(go.transform, "ContinueBtn", new Vector2(0.15f, 0.32f), new Vector2(0.85f, 0.42f), "Watch Ad ▶ Continue", resultUI, "OnWatchAdContinue");
            CreateResultButton(go.transform, "RetryBtn", new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.3f), "Try Again 🔄", resultUI, "OnRetry");
            CreateResultButton(go.transform, "MenuBtn", new Vector2(0.25f, 0.08f), new Vector2(0.75f, 0.16f), "Home", resultUI, "OnMainMenu");
        }
        return go;
    }

    private static UnityEngine.UI.Button CreateResultButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string label, ResultScreenUI target, string method)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        go.AddComponent<UnityEngine.UI.Image>().color = new Color(0.42f, 0.8f, 0.46f);
        var btn = go.AddComponent<UnityEngine.UI.Button>();
        CreateTMP(go.transform, "Text", Vector2.zero, Vector2.one, label, 28);
        if (method == "OnNextLevel") UnityEventTools.AddPersistentListener(btn.onClick, target.OnNextLevel);
        else if (method == "OnRetry") UnityEventTools.AddPersistentListener(btn.onClick, target.OnRetry);
        else if (method == "OnMainMenu") UnityEventTools.AddPersistentListener(btn.onClick, target.OnMainMenu);
        else if (method == "OnWatchAdContinue") UnityEventTools.AddPersistentListener(btn.onClick, target.OnWatchAdContinue);
        return btn;
    }

    private static void WireFXManager(GameObject fxMgr, FXManager.FXEntry[] entries)
    {
        var comp = fxMgr.GetComponent<FXManager>();
        var so = new SerializedObject(comp);
        var prop = so.FindProperty("effects");
        prop.ClearArray();
        for (int i = 0; i < entries.Length; i++)
        {
            prop.InsertArrayElementAtIndex(i);
            var el = prop.GetArrayElementAtIndex(i);
            el.FindPropertyRelative("name").stringValue = entries[i].name;
            el.FindPropertyRelative("prefab").objectReferenceValue = entries[i].prefab;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetObstaclePrefabs(GameObject levelMgr, List<LevelManager.ObstaclePrefabEntry> entries)
    {
        var comp = levelMgr.GetComponent<LevelManager>();
        var so = new SerializedObject(comp);
        var prop = so.FindProperty("obstaclePrefabs");
        prop.ClearArray();
        for (int i = 0; i < entries.Count; i++)
        {
            prop.InsertArrayElementAtIndex(i);
            var el = prop.GetArrayElementAtIndex(i);
            el.FindPropertyRelative("type").enumValueIndex = (int)entries[i].type;
            el.FindPropertyRelative("prefab").objectReferenceValue = entries[i].prefab;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreatePanel(Transform parent, string name, string label, Color bg)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        go.AddComponent<UnityEngine.UI.Image>().color = bg;
        CreateTMP(go.transform, "Label", new Vector2(0.1f, 0.4f), new Vector2(0.9f, 0.6f), label, 56);
        return go;
    }

    private static void CreateMainMenuScene(UpgradeData[] upgradeData)
    {
        string path = $"{Root}/Scenes/MainMenu.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var canvas = new GameObject("UI");
        var canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        var menuUI = canvas.AddComponent<MainMenuUI>();
        var uiMgrGo = new GameObject("UIManager");
        uiMgrGo.transform.SetParent(canvas.transform, false);
        var uiMgr = uiMgrGo.AddComponent<UIManager>();

        CreateTMP(canvas.transform, "Title", new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.72f), "TOY WAR RUSH", 72);
        CreateTMP(canvas.transform, "Subtitle", new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.54f), "Mob Control style army rush!", 28);

        CreateButton(canvas.transform, "PlayButton", new Vector2(0.2f, 0.36f), new Vector2(0.8f, 0.46f), "LET'S GO!", menuUI, "OnPlayPressed");
        CreateButton(canvas.transform, "LevelsButton", new Vector2(0.2f, 0.28f), new Vector2(0.8f, 0.36f), "Levels", menuUI, "OnLevelsPressed");
        CreateButton(canvas.transform, "ShopButton", new Vector2(0.25f, 0.2f), new Vector2(0.75f, 0.28f), "Shop", menuUI, "OnShopPressed");
        CreateButton(canvas.transform, "UpgradeButton", new Vector2(0.25f, 0.12f), new Vector2(0.75f, 0.2f), "Upgrades", menuUI, "OnUpgradePressed");

        var coinsText = CreateTMP(canvas.transform, "Coins", new Vector2(0.05f, 0.92f), new Vector2(0.3f, 0.97f), "0", 36);
        var gemsText = CreateTMP(canvas.transform, "Gems", new Vector2(0.7f, 0.92f), new Vector2(0.95f, 0.97f), "0", 36);
        SetSerializedField(menuUI.gameObject, "coinsText", coinsText);
        SetSerializedField(menuUI.gameObject, "gemsText", gemsText);

        var upgradePanel = CreateUpgradePanel(canvas.transform, upgradeData);
        upgradePanel.SetActive(false);
        SetSerializedField(uiMgr.gameObject, "upgradeMenu", upgradePanel.GetComponent<UpgradeMenuUI>());
        SetSerializedField(uiMgr.gameObject, "mainMenu", menuUI);

        var shopPanel = CreateShopPanel(canvas.transform);
        shopPanel.SetActive(false);
        SetSerializedField(uiMgr.gameObject, "shopUI", shopPanel.GetComponent<ShopUI>());

        var levelSelect = CreateLevelSelectPanel(canvas.transform);
        levelSelect.SetActive(false);
        SetSerializedField(uiMgr.gameObject, "levelSelect", levelSelect.GetComponent<LevelSelectUI>());

        EditorSceneManager.SaveScene(scene, path);
    }

    private static GameObject CreateUpgradePanel(Transform parent, UpgradeData[] upgrades)
    {
        var go = CreatePanel(parent, "UpgradePanel", "UPGRADES", new Color(0.1f, 0.12f, 0.2f, 0.95f));
        var ui = go.AddComponent<UpgradeMenuUI>();
        var so = new SerializedObject(ui);
        var upProp = so.FindProperty("upgrades");
        upProp.ClearArray();
        for (int i = 0; i < upgrades.Length; i++)
        {
            upProp.InsertArrayElementAtIndex(i);
            upProp.GetArrayElementAtIndex(i).objectReferenceValue = upgrades[i];
        }
        var lvlTexts = new TextMeshProUGUI[upgrades.Length];
        var costTexts = new TextMeshProUGUI[upgrades.Length];
        for (int i = 0; i < upgrades.Length; i++)
        {
            float y = 0.62f - i * 0.12f;
            CreateTMP(go.transform, $"UpName{i}", new Vector2(0.1f, y), new Vector2(0.55f, y + 0.08f), upgrades[i].displayName, 28);
            lvlTexts[i] = CreateTMP(go.transform, $"UpLvl{i}", new Vector2(0.58f, y), new Vector2(0.72f, y + 0.08f), "Lv.0", 24);
            costTexts[i] = CreateTMP(go.transform, $"UpCost{i}", new Vector2(0.74f, y), new Vector2(0.88f, y + 0.08f), "100", 24);
            var btnGo = new GameObject($"BuyBtn{i}");
            btnGo.transform.SetParent(go.transform, false);
            var rect = btnGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.88f, y);
            rect.anchorMax = new Vector2(0.95f, y + 0.08f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            btnGo.AddComponent<UnityEngine.UI.Image>().color = new Color(0.42f, 0.8f, 0.46f);
            var btn = btnGo.AddComponent<UnityEngine.UI.Button>();
            int idx = i;
            UnityEventTools.AddVoidPersistentListener(btn.onClick, () => ui.PurchaseUpgrade(idx));
            CreateTMP(btnGo.transform, "Text", Vector2.zero, Vector2.one, "Buy", 22);
        }
        so.FindProperty("upgradeLevelTexts").arraySize = lvlTexts.Length;
        so.FindProperty("upgradeCostTexts").arraySize = costTexts.Length;
        for (int i = 0; i < lvlTexts.Length; i++)
        {
            so.FindProperty("upgradeLevelTexts").GetArrayElementAtIndex(i).objectReferenceValue = lvlTexts[i];
            so.FindProperty("upgradeCostTexts").GetArrayElementAtIndex(i).objectReferenceValue = costTexts[i];
        }
        so.ApplyModifiedPropertiesWithoutUndo();
        CreateButton(go.transform, "CloseBtn", new Vector2(0.3f, 0.06f), new Vector2(0.7f, 0.12f), "Close", ui, "OnClose");
        return go;
    }

    private static GameObject CreateShopPanel(Transform parent)
    {
        var go = CreatePanel(parent, "ShopPanel", "SHOP", new Color(0.12f, 0.1f, 0.18f, 0.95f));
        var shop = go.AddComponent<ShopUI>();
        var shopMgr = new GameObject("ShopManager");
        shopMgr.AddComponent<ShopManager>();
        var so = new SerializedObject(shop);
        so.FindProperty("shopManager").objectReferenceValue = shopMgr.GetComponent<ShopManager>();
        so.ApplyModifiedPropertiesWithoutUndo();
        CreateTMP(go.transform, "NoAdsLabel", new Vector2(0.1f, 0.45f), new Vector2(0.9f, 0.52f), "Remove Ads (IAP stub)", 28);
        var noAdsBtn = CreateButton(go.transform, "NoAdsBtn", new Vector2(0.25f, 0.35f), new Vector2(0.75f, 0.43f), "Buy Remove Ads", shop, "OnPurchaseNoAds");
        CreateButton(go.transform, "ShopClose", new Vector2(0.3f, 0.08f), new Vector2(0.7f, 0.14f), "Close", shop, "OnClose");
        return go;
    }

    private static GameObject CreateLevelSelectPanel(Transform parent)
    {
        var go = CreatePanel(parent, "LevelSelectPanel", "LEVELS", new Color(0.08f, 0.1f, 0.16f, 0.96f));
        var ui = go.AddComponent<LevelSelectUI>();
        var grid = new GameObject("Grid");
        grid.transform.SetParent(go.transform, false);
        var gridRect = grid.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.05f, 0.15f);
        gridRect.anchorMax = new Vector2(0.95f, 0.85f);
        gridRect.offsetMin = gridRect.offsetMax = Vector2.zero;
        var layout = grid.AddComponent<UnityEngine.UI.GridLayoutGroup>();
        layout.cellSize = new Vector2(120, 80);
        layout.spacing = new Vector2(8, 8);
        layout.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 5;

        var btnPrefab = new GameObject("LevelBtnTemplate");
        btnPrefab.transform.SetParent(grid.transform, false);
        btnPrefab.AddComponent<RectTransform>();
        btnPrefab.AddComponent<UnityEngine.UI.Image>().color = new Color(0.25f, 0.45f, 0.85f);
        btnPrefab.AddComponent<UnityEngine.UI.Button>();
        CreateTMP(btnPrefab.transform, "Label", Vector2.zero, Vector2.one, "1", 22);
        btnPrefab.SetActive(false);

        var so = new SerializedObject(ui);
        so.FindProperty("gridRoot").objectReferenceValue = grid.transform;
        so.FindProperty("levelButtonPrefab").objectReferenceValue = btnPrefab;
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateButton(go.transform, "MapClose", new Vector2(0.3f, 0.05f), new Vector2(0.7f, 0.11f), "Close", ui, "OnClose");
        return go;
    }

    private static UnityEngine.UI.Button CreateButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string label, MonoBehaviour target, string method)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        go.AddComponent<UnityEngine.UI.Image>().color = new Color(0.42f, 0.8f, 0.46f);
        var btn = go.AddComponent<UnityEngine.UI.Button>();
        CreateTMP(go.transform, "Text", Vector2.zero, Vector2.one, label, 36);
        if (method == "OnPlayPressed" && target is MainMenuUI playMenu)
            UnityEventTools.AddPersistentListener(btn.onClick, playMenu.OnPlayPressed);
        else if (method == "OnShopPressed" && target is MainMenuUI shopMenu)
            UnityEventTools.AddPersistentListener(btn.onClick, shopMenu.OnShopPressed);
        else if (method == "OnLevelsPressed" && target is MainMenuUI levelsMenu)
            UnityEventTools.AddPersistentListener(btn.onClick, levelsMenu.OnLevelsPressed);
        else if (method == "OnUpgradePressed" && target is MainMenuUI upgradeMenu)
            UnityEventTools.AddPersistentListener(btn.onClick, upgradeMenu.OnUpgradePressed);
        else if (method == "OnClose")
        {
            var closeTarget = target as UpgradeMenuUI;
            if (closeTarget != null)
                UnityEventTools.AddPersistentListener(btn.onClick, closeTarget.OnClose);
            else if (target is ShopUI shop)
                UnityEventTools.AddPersistentListener(btn.onClick, shop.OnClose);
            else if (target is LevelSelectUI map)
                UnityEventTools.AddPersistentListener(btn.onClick, map.OnClose);
        }
        else if (method == "OnPurchaseNoAds" && target is ShopUI shopUi)
            UnityEventTools.AddPersistentListener(btn.onClick, shopUi.OnPurchaseNoAds);
        return btn;
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
