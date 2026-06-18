# Toy War Rush

Casual hyper-runner / army builder for Android (Unity 6).

## One-Click Setup (Windows)

**Double-click `SETUP.bat`** or run:

```powershell
.\setup.ps1
```

This will:
1. Install Unity Hub (if missing)
2. Open the project in Unity Hub

## Unity Editor (one-time, ~2 minutes)

1. Install **Unity 6 (6000.0 LTS)** with **Android Build Support** and **URP**
2. Open this project — wait for packages to import
3. Menu: **ToyWarRush → Setup Everything (One Click)**
4. Open `Assets/_Game/Scenes/Boot.unity` → Press **Play**

That's it. Scenes, prefabs, levels, and data assets are auto-generated.

## Controls

- **Swipe / drag** left-right to steer the army
- **Editor:** hold left mouse + move = swipe

## What's included

- 35+ gameplay scripts (managers, army, gates, save, UI)
- One-click editor wizard creates 5 levels, 6 unit tiers, prefabs, scenes
- Android settings: API 23+, IL2CPP, ARM64+ARMv7
- AdMob/Firebase stubs ready for Phase 3

## Project Structure

```
Assets/_Game/
├── Scripts/          Core, Army, Level, UI, Economy, Save, Ads, Audio
├── ScriptableObjects/  UnitData, LevelData, Upgrades (create in editor)
├── Prefabs/          Units, Gates, Obstacles, UI
├── Scenes/           Boot, MainMenu, Gameplay, Loading
├── Art/              Models, Textures, Materials
└── Audio/            SFX and Music
```

## Phase 0 Checklist (done in repo)

- [x] Folder structure
- [x] `.gitignore` for Unity
- [x] `.cursorrules` for Cursor
- [x] `Packages/manifest.json` with URP, TMP, Input System, Mobile feature
- [x] Core C# scripts (GameManager, ArmyManager, Gates, Save, UI stubs)

## Unity Editor Setup (manual steps)

### 1. Create scenes

| Scene | Purpose |
|-------|---------|
| `Boot.unity` | BootLoader + all DontDestroyOnLoad managers |
| `MainMenu.unity` | Main menu UI |
| `Gameplay.unity` | Player, Army, LevelManager, HUD |
| `Loading.unity` | Optional loading screen |

### 2. Boot scene hierarchy

```
[GameManager]
[SaveManager]
[AudioManager]
[AnalyticsManager]
[AdManager]
[CurrencyManager]
[RewardSystem]
[BootLoader]
```

### 3. Gameplay scene hierarchy

```
[LevelManager]
[ArmyManager]
[UnitFactory]
[UnitEvolutionManager]
[MergeSystem]
[Player] (tag: Player) → PlayerController
  └── FormationRoot
[UIManager] → HUD, ResultScreen
[FXManager]
[ObjectPoolManager]
```

### 4. Create ScriptableObjects

Right-click in Project → **ToyWarRush**:

- `UnitEvolutionConfig` — assign 6 UnitData assets
- `LevelData_001` … `LevelData_005` for MVP test levels
- `AdPlacementConfig` — test AdMob IDs

### 5. Android build settings

- Min API: **23**
- Scripting: **IL2CPP**
- Architectures: **ARM64** + **ARMv7**
- Graphics: **URP**, Forward rendering

### 6. Placeholder prefabs (MVP)

| Prefab | Components |
|--------|------------|
| `Tier1_Unit` | Capsule + UnitController + trigger collider |
| `MultiplyGate` | Cube + GateController + TMP text + trigger |
| `Obstacle_Cat` | Cube + ObstacleController + trigger |
| `Fortress` | Large cube + FortressController + trigger |

Tag the **Player** root object as `Player`.

## Development Phases

| Phase | Status | Deliverable |
|-------|--------|-------------|
| 0 Setup | **Scaffolded** | This repo |
| 1 MVP Core | Scripts ready | 5 playable test levels in editor |
| 2 Content | Pending | 30 levels, evolution, save |
| 3 Polish | Pending | Ads, Firebase, IAP |
| 4 Soft Launch | Pending | Play Store closed test |

## Cursor Workflow

Use `.cursorrules` context. Prompt pattern:

```
Context: Unity 6, C#, Toy War Rush.
Existing: GameManager, ArmyManager, EventBus, ObjectPool.
Task: [specific feature]
```

## Monetization (stubs)

- `AdManager` — replace with Google Mobile Ads SDK
- `AnalyticsManager` — replace with Firebase
- `ShopManager` — add Unity IAP

## License

Private — all rights reserved.
