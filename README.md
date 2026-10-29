# Word Galaxy

Word Galaxy is a portrait Unity game for practicing English vocabulary. Choose a CEFR level and topic, aim the fixed cannon at falling words, and learn each word’s Vietnamese meaning and usage as you play.

The game interface is in English. Each vocabulary entry includes a part of speech, Vietnamese meaning, five example sentences, and bundled English pronunciation.

## Run in Unity

1. Open this folder in Unity Hub with Unity **6000.6.7f1**.
2. Open `Assets/Scenes/Prototype.unity` and press **Play**. The Editor setup registers the scene for builds; the game controller and interface are created at runtime.
3. To preview a phone layout, choose **Word Shooter > Phone Portrait Preview**. This selects a 390 × 844 Game view resolution.

For an iPhone build, use **File > Build Profiles**, select iOS, build the Xcode project, and sign it in Xcode. See [the iPhone install guide](IOS_INSTALL_GUIDE.md).

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
