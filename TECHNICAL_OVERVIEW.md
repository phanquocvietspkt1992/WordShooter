# Technical Overview

## Configuration

- **Unity:** 6000.6.7f1 (`ProjectSettings/ProjectVersion.txt`).
- **Runtime:** C# `MonoBehaviour`, Unity IMGUI (`OnGUI`), legacy `Input`, `AudioSource`, `Resources`, and `PlayerPrefs`.
- **Mobile:** portrait-oriented iOS and Android project with explicit IL2CPP backends. Android minimum API is 26, target API is 36, and the configured architecture is ARM64.
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
- Focus loss or an OS pause interrupts input/audio and flushes preferences. Active rounds remain paused after both focus and OS pause recover; gameplay resumes only after a cannon tap. This does not persist an unfinished round across process termination.
- Back/Escape shares the existing navigation path and dismisses exit confirmation before navigating. Exit confirmation blocks gameplay input; previously paused rounds remain paused when it closes.
- Production admin remains enabled by design: three menu-logo taps within two seconds toggle it. Partial taps reset on interruption, and startup returns to user mode.

## Audio and visuals

Music, gun sounds, effects, and pronunciation use separate audio sources. Pronunciations are bundled WAV clips; only the active topic’s clips are loaded for a round. Speech is queued at first hit, half health, and destruction. Music and gun audio are ducked while speech is active or queued.

The space backdrop, nebulae, galaxy, meteors, word cards, projectiles, cannon, and explosions are drawn with IMGUI and shared textures. The destruction popup contains only the Vietnamese meaning. It uses a rounded translucent neon frame, expands and fades in, and disappears after 1.25 seconds; the explosion effect remains for 2.5 seconds.

Base GUI styles are reused until the source skin label changes. Fitted styles are keyed by source style, text, alignment, and width, with a 256-entry bound; popup styles are reused by font size. Popup reveal animation uses separate cached sizes rather than mutating the measurement style. This removes repeated style construction for unchanged UI, not all per-frame IMGUI allocations.

## Build and validation

Open the project with the Unity version in `ProjectSettings/ProjectVersion.txt`. Use **File > Build Profiles** for mobile builds. `WordShooterMobileBuild.ExportIOS` and `WordShooterMobileBuild.BuildAndroidBundle` also support batch exports with `-wordShooterOutput`; see [README](README.md). These use `BuildOptions.None`, validate platform/backend requirements, and report build failures explicitly without embedding signing credentials. Android bundles include native symbol tables for Play Console crash reports. When `WORDSHOOTER_KEYSTORE`, `WORDSHOOTER_KEYSTORE_PASS`, `WORDSHOOTER_KEY_ALIAS`, and `WORDSHOOTER_KEY_PASS` are set, the bundle is signed with that upload key and the previous keystore settings are restored afterwards; otherwise Unity debug-signs it and logs a warning. `WordShooterIOSPostProcess` sets `ITSAppUsesNonExemptEncryption = NO` in exported iOS projects.

Stop Play mode before using **Word Shooter > Run Gameplay Checks** or the focused **Word Shooter > Run Mobile Stability Checks**. These checks are Editor-only, not a Unity Test Framework assembly. UI checks explicitly pass the built-in Game skin to style initialization rather than accessing `GUI.skin` outside `OnGUI`. Each gameplay case restores progression, lifetime/daily XP, and score preferences that it can modify. Neither suite proves native device lifecycle, performance, or store submission requirements.

Validation on October 30, 2026, in Unity 6000.6.7f1: the full gameplay suite passed 41,602 assertions, the admin suite 166, and the focused mobile suite 284. The non-development iOS export succeeded with `ITSAppUsesNonExemptEncryption = false` and `CFBundleVersion` 1, and Xcode 27.1 compiled the Release device build with `CODE_SIGNING_ALLOWED=NO`. The Android AAB built successfully with a throwaway key and passed `jarsigner -verify`; it contains only arm64-v8a libraries, all with 16 KB (`0x4000`) LOAD alignment, plus symbol tables. Its merged manifest has min API 26, target API 36, portrait orientation, and no permissions beyond the internal `DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION`. The iOS export includes Unity privacy manifests; this is not a complete privacy compliance review. No store upload or real-device test was performed.

Xcode 27.1 warns that the selected platform is no longer supported by libc++. The warning comes from Unity's IL2CPP compile flags (`-mios-version-min=11.0`), not from the project's iOS 15.0 minimum, so raising the minimum does not remove it. Test the oldest supported device before distribution rather than treating successful compilation as compatibility certification.

Before public release, validate the IPA and AAB signed with the real keys, device performance, privacy manifests and data disclosures, and current SDK requirements. Store drafts are in `docs/store`. Names/identifiers remain unchanged pending a store identity decision. Audio redistribution rights are unconfirmed and the current audio is retained for internal testing only; icon, privacy/support information, and signing/store setup remain release prerequisites.

The cannon asset is documented in `Assets/Resources/Art/LICENSE.txt`; audio credits are in `Assets/Resources/Audio/License.txt` and `Assets/Resources/Audio/Pronunciation/Source.txt`.
