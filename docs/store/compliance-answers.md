# Store compliance answers (draft)

> Prepared from the code as of branch `release/store-readiness`. You are legally responsible for every answer you submit; re-verify after any change that adds networking, SDKs, ads, purchases, or accounts.

## What the build does with data (verified in code and settings)

- No networking code (`UnityWebRequest`, sockets, HTTP) in `Assets/Scripts`.
- Packages: only built-in Unity modules; no ads, analytics, IAP, or third-party SDKs.
- Unity Analytics, Cloud Diagnostics, Engine Diagnostics, and the crash-report API are disabled (`UnityConnectSettings.asset`, `enableCrashReportAPI: 0`). Hardware statistics are disabled (`submitAnalytics: 0`).
- Saved locally only via `PlayerPrefs`: XP, best scores, topic unlock/completion flags, daily word counters, audio settings, admin flag, and migration markers.
- No permissions requested beyond the Unity defaults; no camera, microphone, location, contacts, or photos.

## Apple – App Privacy ("nutrition label")

- **Do you or your third-party partners collect data from this app?** → **No, we do not collect data from this app.**
  Result: "Data Not Collected". Data stored only on device and never transmitted does not count as collection.
- **Tracking:** none. No App Tracking Transparency prompt is needed.
- **Privacy manifest:** Unity's exported `PrivacyInfo.xcprivacy` already declares required-reason APIs (UserDefaults `CA92.1`, file timestamp, disk space, system boot time). Re-check Xcode's privacy report at archive time.

## Apple – Export compliance

- The game uses no encryption beyond what iOS provides, and makes no network calls.
- `Assets/Editor/WordShooterIOSPostProcess.cs` writes `ITSAppUsesNonExemptEncryption = NO` into the exported `Info.plist`, so App Store Connect will not ask on each upload. Remove it if you ever add custom or non-exempt encryption.

## Apple – Age rating questionnaire (expected answers)

All content categories: **None** (no violence against people or animals, no horror, profanity, sexual content, drugs, alcohol, gambling, simulated gambling, contests, medical info). Cartoon/fantasy violence: the player fires at falling *words* with a cannon and they explode — answer **None** or at most **Infrequent/Mild Cartoon or Fantasy Violence**; choose consistently on both stores. No unrestricted web access, no user-generated content, no messaging. Expected rating: **4+**.

If the app is put in the **Kids** category, Apple and Google families rules apply (stricter review, parental gate for external links, ads/analytics restrictions). The current build has none of those features, but the Kids category is a separate decision.

## Google Play – Data safety

- **Does your app collect or share any of the required user data types?** → **No.**
- **Is all user data encrypted in transit?** → Not applicable (no data transmitted).
- **Do you provide a way for users to request that their data be deleted?** → Not applicable; uninstalling deletes all local data.

## Google Play – Other declarations

- **Ads:** No ads.
- **App access:** All functionality is available without login. Mention the admin gesture in the access instructions (see below).
- **Content rating (IARC):** answer as in the Apple section; category "Game" or "Reference, News, or Educational" depending on the chosen Play category. Expected result: Everyone / PEGI 3 (possibly PEGI 7 if mild fantasy violence is selected).
- **Target audience and content:** choose the age groups you intend. Including under-13 ages enrolls the app in the Families policy requirements.
- **Government apps / financial features / health:** No.

## App review notes (paste into App Store Connect "Notes" and Play "App access")

> Word Galaxy is an offline English-vocabulary game; no account is needed. To play: choose a level, choose a topic, then touch or drag in the playfield to aim and fire at the falling words. Tap the cannon to pause.
>
> The app includes a hidden admin/testing mode used by the developer for content QA: tap the WORD GALAXY logo on the main menu three times within two seconds. Admin mode opens all topics so reviewers can see every content area; progress earned in admin mode is not saved, and the app returns to normal mode on the next launch. Tapping the logo three times again turns it off. It does not unlock purchases (the app has none) or change any external behavior.

Apple Guideline 2.3.1 forbids *hidden, dormant, or undocumented* features, so disclosing the gesture in review notes is important if admin mode remains in production.

## Rights to verify before public release

| Asset | Status | Action |
|---|---|---|
| Sound effects (`Assets/Resources/Audio/*.ogg`) | Kenney "Digital Audio", CC0 (`License.txt` present) | OK |
| Background music `WordGalaxyArcadeTheme.wav` | Origin not documented | Record who made it and under what license |
| Pronunciation audio (1,640 clips, macOS "Samantha" voice) | Commercial redistribution not confirmed | Confirm rights or replace with licensed TTS/recordings |
| Vocabulary text and example sentences | Origin not documented | Confirm they are original or properly licensed |
| Cannon/tower art (`Assets/Resources/Art`) | Nido, OpenGameArt, CC0 (`LICENSE.txt` present) | OK |
| Fonts | Unity default | OK |
