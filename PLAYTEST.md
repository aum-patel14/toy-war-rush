# Playtest Checklist — Toy War Rush (Mob Control parity)

Mark each test after playing. Roadmap complete when all boxes are checked.

## Browser (`OPEN_GAME.bat` or `PREVIEW.bat`)

| Test | Pass |
|------|------|
| HUD pills: army (top-left), coins (top-right), level center | ☐ |
| Hold screen → visible blue unit stream from cannon | ☐ |
| Gate hit → flash + burst + floating ×2 / +N | ☐ |
| Red mob wall appears in last 30% of level | ☐ |
| Mid-lane red enemies reduce army on contact | ☐ |
| Fortress = visible clash (not instant stat check) | ☐ |
| Win → coins persist after refresh (localStorage) | ☐ |
| Shop: 3 upgrades purchasable with coins | ☐ |
| Level map: 30 levels, stars, unlock progression | ☐ |
| Lose → Watch Ad Continue (demo confirm) revives 50% army | ☐ |
| Biomes: L1–10 desert, L11–20 cave, L21+ outdoor | ☐ |

## Unity (Boot.unity → Play)

| Test | Pass |
|------|------|
| Boot → Main Menu → LET'S GO → Gameplay loads level | ☐ |
| Desert grey lane + sand shoulders (not blue bedroom) | ☐ |
| Visible cannon + blue projectile arc into formation | ☐ |
| Gate flash + army multiply | ☐ |
| Red enemy units on lane + at fortress | ☐ |
| Fortress siege depletes both sides before win/lose | ☐ |
| Defeat → Watch Ad Continue (stub succeeds) | ☐ |
| Main Menu → Levels grid (30), Shop remove-ads stub | ☐ |
| Upgrades affect fire rate / start army / gates | ☐ |
| Complete Levels 1–5 without console errors | ☐ |

## Ship

| Step | Done |
|------|------|
| Capture store assets per `preview/STORE_ASSETS.md` | ☐ |
| `BUILD_ANDROID.bat` produces APK | ☐ |
