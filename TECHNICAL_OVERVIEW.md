# Technical Overview

## Configuration

- **Unity:** 6000.6.7f1 (`ProjectSettings/ProjectVersion.txt`).
- **Runtime:** C# `MonoBehaviour`, Unity IMGUI (`OnGUI`), legacy `Input`, `AudioSource`, `Resources`, and `PlayerPrefs`.
- **Mobile:** portrait-oriented iOS and Android project. Android minimum API is 26 and the configured architecture is ARM64.
- **Physics:** no Rigidbody simulation. Aim and projectile collision use custom geometry and swept rectangles.

## Runtime structure

`VocabularyShooterGame` is the main controller. It creates itself after a scene loads if needed, prevents duplicate controllers, and owns the menu, topic selection, gameplay, results, input, audio, and visual effects. UI and game objects are drawn at runtime; `Assets/Scenes/Prototype.unity` is a minimal bootstrap scene.

| File | Responsibility |
| --- | --- |
| `Assets/Scripts/VocabularyShooterGame.cs` | Screens, round state, touch/mouse/keyboard input, spawning, combat, audio, UI, and effects. |
| `Assets/Scripts/VocabularyTopicData.cs` | Core topic catalog, meanings, examples, and part-of-speech inference. |
| `Assets/Scripts/VocabularyWorkplaceTopicData.cs` | Workplace vocabulary additions. |
| `Assets/Scripts/LegacyFoodExampleData.cs` | Additional examples for legacy food entries. |
| `Assets/Scripts/WordShooterBallistics.cs` | Cannon angle limits and swept projectile/word collision. |
| `Assets/Scripts/WordShooterArt.cs` | Planet colors, deterministic space motion, and generated textures. |
| `Assets/Editor/PrototypeSceneSetup.cs` | Scene registration, portrait setup, and 390 × 844 preview. |
| `Assets/Editor/WordShooterGameplayChecks.cs` | Editor-only gameplay and vocabulary checks. |

## Gameplay and data

- Four levels (A1–B2), 39 topics, and 50 entries per level/topic: 7,800 level-topic entries. Food uses level-specific legacy lists; most other topics share a bank across levels.
- Each entry has a word or phrase, Vietnamese meaning, part of speech, five examples, and local pronunciation audio. Repeated terms in workplace topics may keep separate meanings.
- Base fall speeds are 23.4, 26.4, 29.4, and 32.4 UI units per second. Upgrades also increase the fall-speed multiplier.
- Words have letter-based health. Missed words return to the queue with full health; destroyed words do not respawn. A round ends when all 50 entries are destroyed.
- Aiming is limited to 70 degrees either side of vertical. Touch/mouse input aims and fires automatically; keyboard aiming uses Left/Right at 480 UI units per second and Space fires. Projectiles travel at 850 UI units per second with an approximately 0.485-second firing interval.
- Swept collision accounts for projectile travel and target movement, reducing tunneling. The first word along a projectile’s path absorbs it.
- Plasma thresholds are 0, 100, 220, 360, and 450 points. Approximate shot damage is 0.735, 0.882, 1.029, 1.176, and 1.323; fractional damage carries between hits. Each destroyed word awards 10 points.
- Gun volume, background music preference, and per-level/topic best scores are stored locally with `PlayerPrefs`.

## Audio and visuals

Music, gun sounds, effects, and pronunciation use separate audio sources. Pronunciations are bundled WAV clips; only the active topic’s clips are loaded for a round. Speech is queued at first hit, half health, and destruction. Music and gun audio are ducked while speech is active or queued.

The space backdrop, nebulae, galaxy, meteors, word cards, projectiles, cannon, and explosions are drawn with IMGUI and shared textures. The destruction popup contains only the Vietnamese meaning. It uses a rounded translucent neon frame, expands and fades in, and disappears after 1.25 seconds; the explosion effect remains for 2.5 seconds.

## Build and validation

Open the project with the Unity version in `ProjectSettings/ProjectVersion.txt`. Use **File > Build Profiles** for mobile builds. The Editor menu provides **Word Shooter > Run Gameplay Checks**; stop Play mode before using it. These checks are Editor-only, not a Unity Test Framework assembly. A headless batch run currently stops in `CheckCompactControls` because it calls `GUI.skin` outside `OnGUI`; do not treat that batch invocation as a passing validation run.

The cannon asset is documented in `Assets/Resources/Art/LICENSE.txt`; audio credits are in `Assets/Resources/Audio/License.txt` and `Assets/Resources/Audio/Pronunciation/Source.txt`.
