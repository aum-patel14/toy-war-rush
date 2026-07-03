# Recording & QA — Toy War Rush Preview

Use the local preview at **http://localhost:8080** (run `PREVIEW.bat`) or open `preview/index.html` directly.

## Best settings for Play Store / YouTube

| Setting | Value |
|---------|--------|
| **Aspect ratio** | 9:16 (portrait) — built into the preview frame |
| **Resolution** | Record at **1080×1920** if your tool allows |
| **Duration** | 30–60 seconds per clip |
| **Tool (Windows)** | Xbox Game Bar (`Win + G`) or OBS Studio |

## OBS Studio (recommended)

1. Download [OBS Studio](https://obsproject.com/)
2. Scene → Add **Window Capture** → select browser with Toy War Rush
3. Crop to the phone frame (9:16)
4. Output → Recording → **1920×1080** or custom **1080×1920**
5. Click **Start Recording** → Play the game → Stop

## What to capture (marketing shots)

1. **Hook (0–5s)** — Army evolving to Titan with particle burst
2. **Core loop (5–25s)** — Passing ×2 gates, army growing
3. **Obstacle dodge (25–35s)** — Swerving around cat / train
4. **Win moment (35–45s)** — Fortress battle + 3-star victory screen

## Fullscreen mode

Click **Fullscreen** on the start screen before recording to remove the desktop frame.

## Upload checklist

- [ ] No browser URL bar visible (use fullscreen or crop)
- [ ] 30+ FPS smooth recording
- [ ] Show swipe steering in first 3 seconds
- [ ] End on Victory screen with stars
- [ ] Export as MP4 (H.264)

## Mobile QA checklist (Phase 2)

Run through on **Android Chrome** and **iPhone Safari**:

### Start screen
- [ ] LET'S GO shows tutorial on first run (3 steps)
- [ ] Skip tutorial works
- [ ] Best level / army displays from localStorage
- [ ] Fullscreen button works

### Gameplay
- [ ] Swipe steering responsive (no page scroll)
- [ ] Army count updates on gates
- [ ] First gate triggers brief slow-mo
- [ ] Sound toggle (🔊/🔇) works and persists
- [ ] Haptic feedback on gate hit (Android)
- [ ] Obstacle red telegraph ring visible before hit
- [ ] Collectible sparkle + pickup sound
- [ ] Fortress power bar appears near end
- [ ] Level 5 and 10 show BOSS FORT label

### Flow
- [ ] Win → coin counter ticks up
- [ ] Next Level advances (1→10, then Play Again)
- [ ] Lose → Try Again works
- [ ] Home returns to menu with stats updated
- [ ] No red error banner at top

### Levels
- [ ] Level 1 completable in under 2 minutes without instructions
- [ ] Level 5+ feels challenging but fair
- [ ] 10 handcrafted levels all load

### Desktop
- [ ] Arrow keys / A D steering works
- [ ] OPEN_GAME.bat opens game offline

## Drop recording into landing page

After recording, save as `docs/assets/hero-trailer.mp4` and add to `docs/index.html` hero section (optional upgrade from SVG mockup).
