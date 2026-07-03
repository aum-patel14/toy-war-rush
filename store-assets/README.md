# Play Store Assets — Toy War Rush

Place exported files here before uploading to Google Play Console.

## Required assets

| Asset | Size | Source |
|-------|------|--------|
| App icon | 512×512 PNG | Export from `docs/assets/logo.svg` |
| Feature graphic | 1024×500 PNG | Screenshot + title from landing page |
| Phone screenshots | 1080×1920 (min 2) | Record via `preview/RECORDING.md` |
| 7-inch tablet | 1200×1920 (optional) | Same recordings, cropped |
| Privacy policy URL | — | `https://aum-patel14.github.io/toy-war-rush/privacy.html` |

## Internal testing track checklist

1. Build APK: Unity menu **ToyWarRush → Build Android APK**  
   Or batch: `"D:\Unity Hub\6000.5.0f1\Editor\Unity.exe" -batchmode -nographics -quit -projectPath "D:\Play Game" -executeMethod ToyWarRushBuild.BuildAndroidBatch`
2. Create app in [Google Play Console](https://play.google.com/console)
3. Upload APK/AAB to **Internal testing**
4. Add testers via email list
5. Link privacy policy URL from `docs/privacy.html`
6. Complete content rating questionnaire (likely PEGI 3 / Everyone)

## Store listing copy (draft)

**Short description:** Grow your toy army, pass math gates, smash fortresses!

**Full description:**  
Toy War Rush is a kid-friendly army runner. Swipe to steer your toy soldiers through magic gates (+10, ×2), dodge silly obstacles, collect bonus troops, and crush block fortresses at the end of each level. Evolve from Toy Soldiers to Ultra Titans!

## Build output

- APK path: `Build/Android/ToyWarRush.apk`
- Re-run **ToyWarRush → Setup Everything** before first build if scenes are broken
