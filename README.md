# Word Galaxy

Word Galaxy is a portrait Unity game for practicing English vocabulary. Choose a CEFR level and topic, aim the fixed cannon at falling words, and learn each word’s Vietnamese meaning and usage as you play.

The game interface is in English. Each vocabulary entry includes a part of speech, Vietnamese meaning, five example sentences, and bundled English pronunciation.

## Run in Unity

1. Open this folder in Unity Hub with Unity **6000.6.7f1**.
2. Open `Assets/Scenes/Prototype.unity` and press **Play**. The Editor setup registers the scene for builds; the game controller and interface are created at runtime.
3. To preview a phone layout, choose **Word Shooter > Phone Portrait Preview**. This selects a 390 × 844 Game view resolution.

For an iPhone build, use **File > Build Profiles**, select iOS, build the Xcode project, and sign it in Xcode. See [the iPhone install guide](IOS_INSTALL_GUIDE.md).

## Mobile validation and release builds

- Stop Play mode and run **Word Shooter > Run Mobile Stability Checks** for focused lifecycle, Back navigation, UI caching, mobile configuration, and production admin regression checks.
- Losing focus or backgrounding an active round pauses it. Returning does not automatically resume gameplay; tap the cannon to continue.
- Android system Back/Escape and the existing Back controls share navigation behavior. Back dismisses an open exit confirmation first.
- The production admin gesture remains available: tap the menu logo three times within two seconds. Admin progress is not saved; startup returns to user mode.
- Mobile builds explicitly use IL2CPP. Android retains minimum API 26 and ARM64, with target API 36.

Headless validation, using the Unity executable for this project version:

```sh
"$UNITY" -batchmode -nographics -projectPath "$PWD" -buildTarget iOS \
  -executeMethod WordShooterGameplayChecks.RunMobileStabilityChecks \
  -quit -logFile /tmp/wordgalaxy-mobile-checks.log
```

Reproducible non-development exports use the enabled build scenes and require an explicit output path outside `Assets`:

```sh
"$UNITY" -batchmode -nographics -projectPath "$PWD" -buildTarget iOS \
  -executeMethod WordShooterMobileBuild.ExportIOS \
  -wordShooterOutput /absolute/path/WordGalaxy-iOS \
  -quit -logFile /tmp/wordgalaxy-ios-build.log

"$UNITY" -batchmode -nographics -projectPath "$PWD" -buildTarget Android \
  -executeMethod WordShooterMobileBuild.BuildAndroidBundle \
  -wordShooterOutput /absolute/path/WordGalaxy.aab \
  -quit -logFile /tmp/wordgalaxy-android-build.log
```

To sign the Android bundle with your upload key, export these variables before running the Android command. Keep the keystore and passwords outside the repository; `*.jks` and `*.keystore` are ignored by Git.

```sh
export WORDSHOOTER_KEYSTORE=/secure/path/upload.jks
export WORDSHOOTER_KEYSTORE_PASS=...
export WORDSHOOTER_KEY_ALIAS=upload
export WORDSHOOTER_KEY_PASS=...
```

Without them, Unity debug-signs the bundle, which Google Play rejects. Android bundles include native symbol tables for Play Console crash reports, and iOS exports declare `ITSAppUsesNonExemptEncryption = NO`.

Install the corresponding Unity Build Support modules first. Android also requires the SDK/NDK/JDK supported by this Unity version. The scripts do not configure signing credentials, app identity, icons, or store metadata. An Android bundle is not upload-ready without release signing; an iOS export still needs Xcode archive/signing and device testing.

**Distribution is not yet cleared:** the bundled Samantha pronunciation is retained for internal testing, but its redistribution rights have not been confirmed. Verify audio rights, privacy declarations/manifests, icons, final identifiers, signing, SDK/store requirements, and real-device behavior before public release. The current level labels do not imply that every topic has independently graded CEFR vocabulary.

## Store release checklist

Status as of October 30, 2026. Checked items are verified in this repository; unchecked items still require work, credentials, decisions, or device/store validation. Production admin is intentionally retained.

### Completed

- [x] Active rounds pause on focus loss or OS pause, and gameplay resumes only after the player taps Resume.
- [x] Back, Escape, swipe, and on-screen Back controls share navigation; Back closes exit confirmation first.
- [x] IMGUI base and fitted styles are cached with a bounded fitted-style cache.
- [x] Mobile stability checks run headlessly. The last iOS-target run passed 284 assertions.
- [x] iOS and Android use IL2CPP; Android uses minimum API 26, target API 36, and ARM64.
- [x] The Unity iOS release export and unsigned Xcode Release build succeeded.
- [x] The full gameplay suite passes headlessly (41,602 assertions); admin checks pass (166). Three stale or failing checks were fixed: daily XP tiers, the cosmetic-reward message for a saved but unclaimed topic, and the explosion popup layout.
- [x] Deprecated `FindObjectOfType` calls are replaced with `FindAnyObjectByType`.
- [x] Background music is imported as compressed Vorbis instead of PCM. Pronunciation clips were already mono 22 kHz Vorbis, loaded on demand.
- [x] Unity hardware statistics are disabled; Analytics, Cloud Diagnostics, and Engine Diagnostics were already off.
- [x] iOS exports set `ITSAppUsesNonExemptEncryption = NO` automatically, and the iOS build number starts at 1.
- [x] Drafts are ready in [docs/store](docs/store): [privacy policy](docs/store/privacy-policy.md), [store listing](docs/store/store-listing.md), and [compliance answers and review notes](docs/store/compliance-answers.md).

### 1. Required release decisions and rights

- [ ] Choose the final store name. The project currently uses `WordShooter`; the game UI uses `WORD GALAXY`.
- [ ] Choose final permanent iOS and Android application IDs. The iOS ID is currently `com.viet.WordShooter`.
- [ ] Decide whether the game targets general audiences or children. If it targets children, review Apple Kids and Google Play Families requirements.
- [ ] Confirm commercial redistribution rights for the Samantha pronunciation clips, or replace them with licensed audio.
- [ ] Confirm the source and license for `WordGalaxyArcadeTheme.wav`; the Kenney license covers the included SFX, not necessarily this music.
- [ ] Approve the admin disclosure drafted in [compliance answers](docs/store/compliance-answers.md). Admin is intentionally available in production through three taps on the menu logo.

### 2. Accounts, signing, and identity

- [ ] Prepare Apple Developer Program and App Store Connect access.
- [ ] Prepare Google Play Console access and Play App Signing.
- [ ] Create and back up an Android upload key outside the repository. Never commit keystores or passwords.
- [ ] Configure the iOS team and signing for release archives in Xcode.
- [ ] Define version and build-number rules for iOS and Android uploads.

### 3. Assets and store listing

- [ ] Create iOS icons, including the 1024 × 1024 App Store icon.
- [ ] Create Android launcher and adaptive icons.
- [ ] Add a branded splash or launch screen, or confirm that the default Unity splash is acceptable.
- [ ] Prepare iPhone and Android screenshots. The iOS target currently includes iPad (`targetDevice: 2`); keep iPad and add 13" screenshots, or switch to iPhone only.
- [ ] Finalize the drafted [store listing](docs/store/store-listing.md), then add the support URL and support email.
- [ ] Describe the levels accurately: most topics share vocabulary across A1–B2 unless the content is reclassified.

### 4. Privacy and compliance

- [ ] Fill in and publish the drafted [privacy policy](docs/store/privacy-policy.md), and link it from both stores.
- [ ] Complete Apple App Privacy and Google Play Data safety using the final signed builds and actual SDK contents.
- [ ] Review generated iOS privacy manifests and required-reason API declarations in the final Xcode archive.
- [ ] Complete age-rating questionnaires.
- [ ] Confirm the export-compliance answer (the build declares no non-exempt encryption) and complete EU trader status, if applicable.

### 5. Android build readiness

- [x] Unity Android Build Support, SDK, NDK, and JDK are installed for Unity 6000.6.7f1.
- [x] `WordShooterMobileBuild.BuildAndroidBundle` produced an AAB signed with a throwaway key: ARM64 only, target API 36, no extra permissions, symbol tables included.
- [x] Every bundled native library uses 16 KB LOAD alignment.
- [ ] Create the real upload key and build the signed release AAB with the `WORDSHOOTER_*` variables.
- [ ] Test system Back, predictive Back, gesture navigation, and button navigation on real Android devices.

### 6. iOS build readiness

- [ ] Confirm the minimum iOS version (currently 15.0). The Xcode 27.1 libc++ warning comes from Unity's IL2CPP compile flags (`-mios-version-min=11.0`), so raising the project minimum does not remove it; the build still succeeds.
- [ ] Archive, sign, and validate a release build in Xcode.
- [ ] Upload the build to TestFlight and resolve App Store Connect warnings.
- [ ] Test notch, home indicator, Control Center, lock, call, and app-switch interruptions on real iPhones.

### 7. Runtime quality before public release

- [ ] Profile CPU, memory, frame time, and allocations on low-end iOS and Android devices.
- [ ] Measure topic loading time and memory use after repeated topic changes.
- [ ] Verify that pronunciation queueing stays understandable during rapid hits.
- [ ] Consider reducing `PlayerPrefs.Save()` frequency after measuring its impact.
- [ ] Verify Vietnamese diacritics, text fit, touch targets, and safe-area layout across screen sizes.
- [ ] Add a first-play tutorial or a clear pause and listen hint.

### 8. Learning content quality

- [ ] Choose whether A1–B2 are gameplay difficulty labels or curated CEFR content levels.
- [ ] If they are CEFR levels, reclassify topic vocabulary and review meanings, examples, and parts of speech.
- [ ] Replace inferred parts of speech with reviewed values for ambiguous words and phrases.
- [ ] Have a qualified reviewer check meanings, examples, and pronunciation.

### 9. Testing and launch

- [ ] Run internal testing on iOS TestFlight and Google Play internal testing.
- [ ] Test fresh install, upgrade from older save data, force-close, offline mode, and repeated rounds.
- [ ] For a personal Google Play developer account created after November 13, 2023, run a closed test with at least 12 testers opted in continuously for 14 days, then apply for production access.
- [ ] Triage tester feedback, crashes, and performance issues before production.
- [ ] Use phased rollout or staged release, and monitor crashes and reviews after launch.

### Optional after the first release

- [ ] Persist and resume an unfinished round after process termination without awarding duplicate XP.
- [ ] Add accuracy, time, or missed-word metrics so personal bests can distinguish completed rounds.
- [ ] Add separate pronunciation volume and reduced-effects settings.
- [ ] Migrate IMGUI screens to uGUI or TextMeshPro gradually if profiling or accessibility work justifies it.

## Play

- Select one of four levels: A1 (Earth), A2 (Mars), B1 (Saturn), or B2 (Neptune). Then choose one of 39 topics.
- On a phone, touch or drag in the playfield to aim and fire. On a computer, click or drag with the mouse; hold **Left/Right** to aim with the keyboard and **Space** to fire.
- Tap the cannon to pause or resume. The controls below it adjust gun volume and toggle background music. A rightward swipe from the left edge navigates back; during play it opens the exit confirmation.
- A projectile must hit a word. Each word’s health is based on its letter count; a hit reveals its part of speech, Vietnamese meaning, and example in the HUD.
- Destroying a word awards 10 points. Missed words return with full health after the remaining words. There are no lives or time limit; clear all 50 entries to finish the round.
- The destruction popup shows only the Vietnamese meaning inside a rounded, translucent hologram frame. It expands and fades in, then disappears after 1.25 seconds. The explosion artwork lasts longer.
- Words hit, damaged, and destroyed trigger queued pronunciation at the first hit, half health, and destruction. Missed words appear in the result review and can be tapped to hear them.

## Levels and progression

| Level | Planet | Base fall speed |
| --- | --- | ---: |
| A1 | Earth | 23.4 UI units/s |
| A2 | Mars | 26.4 UI units/s |
| B1 | Saturn | 29.4 UI units/s |
| B2 | Neptune | 32.4 UI units/s |

The five Plasma levels unlock at scores **0, 100, 220, 360, and 450**. Each destroyed word is worth 10 points. Shot damage scales from approximately 0.735 at Plasma I to 1.323 at Plasma V; fractional damage carries across hits. New shots use the current level, while bullets already in flight keep the damage they started with. Personal bests are saved separately for every level and topic on the device.

There are 39 topics with 50 entries each. Food retains level-specific vocabulary; most other topics share their 50-word bank across levels. The workplace topics include specialized terms and phrases, with multiple meanings retained for repeated terms.

## Project map

- `Assets/Scripts/VocabularyShooterGame.cs` — game flow, input, scoring, audio, and IMGUI rendering.
- `Assets/Scripts/VocabularyTopicData.cs` and `VocabularyWorkplaceTopicData.cs` — topic catalog and vocabulary.
- `Assets/Scripts/WordShooterBallistics.cs` — aim limits and swept projectile collision.
- `Assets/Scripts/WordShooterArt.cs` — shared colors, space animation math, and runtime textures.
- `Assets/Editor/PrototypeSceneSetup.cs` — prototype scene setup and phone preview.
- `Assets/Resources/Audio/` — music, sound effects, and offline pronunciation.

See [Technical Overview](TECHNICAL_OVERVIEW.md) for implementation details.
