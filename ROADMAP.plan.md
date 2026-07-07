# Toy War Rush — Development Roadmap

**Strategy:** Two tracks in parallel.
- **Browser preview** (`preview/index.html`) = the visual + gameplay **reference**. Keep it polished; it defines "what good looks like."
- **Unity project** (`Assets/_Game/`) = the **ship target** for Google Play. Close the gaps until it matches the browser, then polish and launch.

> Rule of thumb: if a mechanic or feel is unclear in Unity, play the browser build and match it.

---

## Current state (analysis)

### Browser preview — polished, near feature-complete
- 2.5D lane with perspective projection, screen shake, first-gate slow-mo
- Cannon auto-fire + hold-to-fire-faster, visible blue unit stream
- Drag steering into `+ / × / -` gates with flash, unit burst, floating labels
- Mid-lane red enemies + red mob wall (last ~30%), then fortress clash phase
- 30 levels (handcrafted + procedural), 3 biomes (desert/cave/outdoor), boss every 5th level
- Tier evolution + EVOLVE flash, confetti, haptics, SFX
- Shop (fire rate / start army / gate bonus), localStorage coins, stars, level map, watch-ad revive, tutorial

### Unity — implementation complete, playtest pending
- Managers + EventBus + ObjectPool + ScriptableObjects (matches `.cursorrules`)
- Core parity systems implemented: fortress clash, enemy attrition, cannon stream, gate juice, biomes, HUD fortress bar
- One-click editor generator (`ToyWarRushSetup.cs`) builds scenes, prefabs, 30 levels, materials
- `PLAYTEST.md` checklist still needs in-editor verification

---

## Phase 0 — Verify Unity boots (gate for everything below)
- [x] Unity 6 (6000.5.0f1) installed at `D:\Unity Hub\6000.5.0f1\`
- [x] Build settings include Boot → MainMenu → Gameplay scenes
- [x] Boot scene has all managers (GameManager, SaveManager, BootLoader, etc.)
- [x] **Fix applied:** Player + EnemyUnit prefabs now get kinematic `Rigidbody` (required for `OnTriggerEnter` on gates/enemies)
- [ ] Close Unity Editor, then re-run setup: **ToyWarRush → Setup Everything** (or `SETUP_UNITY.bat`)
- [ ] Press Play on `Boot.unity`; confirm Boot → Main Menu → Gameplay flow
- [ ] Walk the Unity section of `PLAYTEST.md`; record every console error
- [ ] Triage: list which parity gaps are real bugs vs. tuning

## Phase 1 — Core gameplay parity (Unity)
- [x] Mid-lane enemies collide and subtract from army on contact (`EnemyUnitController` + `ArmyManager`)
- [x] Rework `FortressController` into a **visible clash**: blue units advance, both sides visibly deplete, then resolve win/lose
- [x] Tune `CannonShooter` + `CannonProjectile` to read as a continuous stream (rate, arc, spread)
- [x] Confirm gate `+ / × / -` math + army count matches browser formulas (subtract keeps min 1 army)
- [x] Verify defeat path: army hits 0 → Defeat state → Watch-Ad Continue stub revives ~50% of army at level start

## Phase 2 — Visual & juice parity (Unity vs browser reference)
- [x] Gate hit: flash + unit burst + floating `×N` / `+N` label (`FloatingTextFx`, `GameplayJuice`)
- [x] Tier-up EVOLVE moment: flash + confetti + SFX + shake (`UnitEvolutionManager`)
- [x] Screen shake on gate/obstacle/fortress; first-gate slow-mo beat (`GameplayJuice`)
- [x] Biome parity: desert (L1–10), cave (L11–20), outdoor (L21+) palettes (`BiomeEnvironmentController`)
- [x] HUD parity: army pill, coins, level center, progress bar, fortress VS bar (`HUDController`)

## Phase 3 — Meta & economy (Unity)
- [x] Shop upgrades affect fire rate / start army / gate bonus in-run (`UpgradeRuntime`)
- [x] Coins + stars persist across sessions (`SaveManager` / `LocalSave`)
- [x] Level map: 30 levels, star display, unlock progression (`LevelSelectUI`)
- [x] Win reward flow: coin count-up, star calc thresholds (`ResultScreenUI`, `LevelManager.CalculateStars`)

## Phase 4 — Browser reference upkeep (parallel, low effort)
- [x] Browser preview is the canonical feel reference (`preview/index.html`)
- [x] GitHub Pages demo (`docs/preview/`) mirrors browser build
- [ ] When a mechanic changes, update browser first, then port to Unity (ongoing process)

## Phase 5 — Ship prep (Unity)
- [ ] Replace `AdManager` stub with Google Mobile Ads SDK (real + test IDs) — stub auto-succeeds for dev
- [ ] Replace `AnalyticsManager` stub with Firebase — events logged to console for now
- [ ] Add Unity IAP to `ShopManager` (remove-ads + coin packs) — `PurchaseWithIAP` stub wired
- [x] `BUILD_ANDROID.bat` → `ToyWarRushBuild.BuildAndroidBatch`
- [ ] Capture store assets per `preview/STORE_ASSETS.md`
- [ ] Google Play closed test → address feedback

---

## Notes / constraints
- Follow `.cursorrules`: singleton managers, ScriptableObjects, ObjectPool for spawned objects, `[SerializeField]` fields, null-check `Instance`, TMP for text, cache components in `Awake()`, avoid `Update()` allocations.
- **Next action:** Close Unity Editor → run **ToyWarRush → Setup Everything** → Play `Boot.unity` and verify.
