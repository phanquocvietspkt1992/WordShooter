# Word Galaxy: Technical Overview

## Platforms

- **iOS:** configured for iOS 15.0 and above; requires Unity iOS Build Support and Xcode.
- **Android:** configured for Android API 26 and above, ARM64.
- **macOS:** used for Unity Editor development and iOS builds. No standalone desktop player target is documented or verified.

Both mobile targets use portrait orientation. Other Unity targets may require separate configuration and testing.

## Stack

- **Language:** C# for runtime gameplay and Unity Editor tooling.
- **Engine:** Unity 6000.6.7f1.
- **UI/input:** Unity IMGUI (`OnGUI`), `GUI.matrix`, and the legacy `Input` API for touch, mouse, and keyboard.
- **Collision:** custom swept-rectangle math; gameplay does not use Rigidbody simulation.
- **Audio/assets/state:** `AudioSource`, local `Resources`, and `PlayerPrefs`.
- **Build/checks:** Unity Build Profiles and Xcode for iOS; custom Editor checks from **Word Shooter > Run Gameplay Checks**.

## Structure

`VocabularyShooterGame` is the main `MonoBehaviour`: it owns the four screens, input, round state, falling words, projectiles, UI, and audio. It creates itself after scene load if absent and rejects duplicate controllers.

| File                                             | Role                                                  |
| ------------------------------------------------ | ----------------------------------------------------- |
| `Assets/Scripts/VocabularyShooterGame.cs`        | Main game controller and UI.                          |
| `Assets/Scripts/VocabularyTopicData.cs`          | Topic catalog, meanings, examples, and word metadata. |
| `Assets/Scripts/VocabularyWorkplaceTopicData.cs` | Workplace vocabulary additions.                       |
| `Assets/Scripts/WordShooterBallistics.cs`        | Aim limits and swept projectile collision.            |
| `Assets/Scripts/WordShooterArt.cs`               | Cannon loading and generated visual textures.         |
| `Assets/Editor/PrototypeSceneSetup.cs`           | Prototype scene setup and phone preview.              |
| `Assets/Editor/WordShooterGameplayChecks.cs`     | Editor-only gameplay and content checks.              |

`PrototypeSceneSetup` registers `Assets/Scenes/Prototype.unity`, sets portrait orientation, and offers a 390x844 Game view preview.

## Gameplay and Data

- Four CEFR choices (A1-B2), 39 topics, and 50 words per selected level/topic. Food uses level-specific legacy lists; most other topics currently reuse their word bank across CEFR levels.
- Each word has letter-based health, Vietnamese meaning, example, part of speech, and bundled pronunciation. Missed words are retried; a round ends after all 50 are destroyed.
- The fixed gun aims up to 70 degrees left/right. Projectiles travel at 850 UI units/s; firing interval is approximately 0.485 s. Collision sweeps across frame movement to reduce tunneling.
- Five gun levels unlock at 0, 100, 220, 360, and 450 points. Damage per shot is approximately 0.735, 0.882, 1.029, 1.176, and 1.323; each level also increases fall speed and projectile effects.
- Pronunciation is queued at first contact, half health, and destruction. Music, gunfire, effects, and voice use separate audio sources; gun volume, music preference, and per-topic best scores are saved locally.

## Verification

Open the project with the Unity version in `ProjectSettings/ProjectVersion.txt`. The custom Editor check suite covers ballistics, damage/progression, round completion, touch/back navigation, audio, layout, vocabulary pools, and generated art. Stop Play mode before running it. This is a menu-driven check suite, not a Unity Test Framework assembly.
