# Word Galaxy

A portrait-mode Unity game for learning English vocabulary. Choose an A1–B2 planet and one of thirty-nine topics, aim a fixed gun at falling words, and discover Vietnamese meanings on the first hit.

All game interface labels and instructions are in English. Vocabulary entries retain Vietnamese translations and include an example sentence showing the term in context.

## Space theme

- Deep navy space, purple/cyan nebulae, and 90 deterministic twinkling stars across three scrolling depth layers.
- Slowly drifting nebula layers and a rotating three-arm spiral galaxy with a luminous core. Four staggered meteor cycles cross the background with tapered blue-white tails and fade in/out, including quiet intervals. All are rendered behind the UI/targets using existing textures and fixed loop counts, without spawning objects or consuming gameplay randomness.
- Neon data capsules with energy segments, shield-hit flashes, cyan plasma shots with white cores, and violet/cyan destruction shockwaves.
- Holographic HUD, menu, result and meaning panels with subtle scanlines and high-contrast text.
- The existing red cannon remains the only gun, anchored on a small illuminated station deck at the bottom center.
- An original retro arcade loop plays in the background, ducks while vocabulary is pronounced, and resumes at its normal level afterward.
- Each level-topic combination contains 50 English words with Vietnamese meanings, example sentences and bundled offline pronunciation. Only the selected topic's audio is loaded for a round. Star animation does not consume gameplay random numbers.

## Play in Unity

1. Install Unity 6 (6000.0 or newer) with the iOS Build Support module if you plan to build for iPhone.
2. In Unity Hub, open this `WordShooter` folder as a project.
3. Open `Assets/Scenes/Prototype.unity` and press Play. The Editor bootstrap creates the scene and registers it for builds if it is missing; the game UI is created at runtime.
4. In Build Profiles, choose iOS, set the orientation to Portrait, and build. Open the generated Xcode project, select your iPhone, and set a signing Team under Signing & Capabilities.

For a phone-shaped preview in the Editor, select **Word Shooter > Phone Portrait Preview**. It selects a reusable **390 x 844** portrait Game view resolution without changing build targets or installing a simulator. This previews the portrait layout, not a device bezel or simulated notch.

## Run on an iPhone

You need a Mac, Xcode, Unity iOS Build Support, and an iPhone. A free Apple ID with Personal Team signing is generally enough to install a development build directly from Xcode; the free provisioning profile has limitations and must be renewed periodically. Apple's paid Developer Program is typically needed for TestFlight, App Store distribution, and longer-lived distribution workflows.

## Controls and rules

- The gun stays at the bottom center. Only its barrel rotates, up to 70 degrees left or right of vertical.
- A single active game controller owns the cannon; duplicate runtime controllers are disabled and removed before initializing. Every shot and the aiming guide share the same cannon muzzle position.
- On phones, touch or drag in the falling-word area to aim and fire automatically; no fire button is displayed.
- Tap the center of the cannon to pause the round while you read; tap it again to resume. Pausing stops word movement, projectiles, gunfire, effects, and all game audio. While paused, only the cannon tap can resume play; other touches cannot fire. The cannon's **− / +** volume buttons sit below it on the left and right and do not pause the game. New words wait until the game resumes.
- Swipe right from the left edge to go back: topic selection returns to the main menu, results return to topic selection, and gameplay opens the exit confirmation without discarding the round.
- When you pause after hitting a word, the game shows five prepared example sentences for that word. Resume to hide them and return to the single original example in the meaning panel.
- On a keyboard, hold **Left / Right arrow keys** to move the visible crosshair horizontally; the barrel follows it. Aiming speed is 480 UI units/second, twice the previous 240. **Up / Down** do not change aim. Hold **Space** simultaneously to fire. Releasing the arrows keeps your aim, and the gun itself never moves.
- In the Editor, select the **Game** view so it receives keyboard input. You can also click/drag with a mouse or touchpad to aim and fire automatically. Hold **Space** to fire from keyboard aim. Switching to arrow keys continues from the pointer's last aim position.
- Shots travel along the barrel direction. Tapping a word only aims; a projectile must hit it to count. The first word along a projectile's path absorbs the shot.
- Every word is a valid target. Its health equals its number of letters: at Plasma I, **apple takes 5 hits**, **rice takes 4**. Each projectile is consumed on impact and deals damage based on the gun level when fired.
- Every projectile hit immediately shows that word's part of speech, Vietnamese meaning, and first example sentence in the top panel, even if the word still has health. Pausing after a hit shows five prewritten English examples for that word; resuming hides the extra examples and restores the original first example. Hitting another word updates the panel; firing without a hit or missing leaves it unchanged. Replay clears the panel. Vocabulary tags are abbreviated as **(n)**, **(v)**, **(adj)**, and **(adv)**.
- Words flash when hit; the segments below each word show its remaining health. The final hit creates an explosion, awards 10 points, and also displays that word's Vietnamese meaning near the explosion for 2.5 seconds. The header keeps the latest hit word's meaning and example sentence.
- The first hit on each word pronounces it in English using bundled offline audio; later hits on the same target do not repeat it. Consecutive words are spoken in order, without overlapping voices. Use the cannon's **− / +** controls below and beside the gun (decrease on the left, increase on the right) to change gun volume in 10% steps; the setting is saved. Gunfire temporarily drops to 10% of the selected level while speech is active or queued, then returns to that level; other effects keep their volume. Speech can finish on the result screen. Tap a missed vocabulary item on the result screen to hear it again. Replay or disabling the game clears speech and restores the selected gun volume.
- Other targets, projectiles, aiming, firing, and spawning continue during explosions. There are no questions, correct/wrong answers, or feedback pauses.
- There are no lives or time limits. A round ends only after all 50 words in the selected level and topic have been destroyed. The HUD shows completed vocabulary (for example, 12/50) instead of a countdown.
- Words reaching the cyan boundary are queued behind the remaining words and fall again with full health until destroyed. Destroyed words do not return during that round, and each awards 10 points exactly once. Words missed at least once remain in the end-of-round review with their Vietnamese meanings; tap a word to hear its pronunciation.

## Levels and topics

Choose a planet first, then choose one of its thirty-nine vocabulary topics:

| Level | Planet  | Vocabulary focus              | Fall speed |
| ----- | ------- | ----------------------------- | ---------: |
| A1    | Earth   | Beginner vocabulary           |         78 |
| A2    | Mars    | Elementary vocabulary         |         88 |
| B1    | Saturn  | Intermediate vocabulary       |         98 |
| B2    | Neptune | Upper-intermediate vocabulary |        108 |

Topics: Food, Home, School, Travel, Jobs, Body, Nature, Technology, Sports, Work, Office, Information Technology, IT at Work, Meeting Essentials, Engineering, Business, Finance, Human Resources, Customer Service, Office & Workplace, People & Colleagues, Daily Work Activities, Workplace Communication, Meetings, Discussion & Opinions, Planning & Priorities, Project Management, Tasks & Deadlines, Problem Solving, Critical Thinking & Decisions, Software Development, Programming & Code Review, Databases & APIs, Systems & Architecture, Testing & Quality, Deployment & DevOps, Incidents & Troubleshooting, Jobs & Careers, and Performance & Professional Growth. In the game, topics appear in English in a numbered, single-column list; swipe up to move the list up and reveal later topics, or swipe down to move it back down. The twenty work-focused topics add the supplied 1,000 vocabulary entries, including alternative meanings for repeated terms, with Vietnamese meanings and workplace example sentences. Vocabulary is labeled with a part of speech: noun (n), verb (v), adjective (adj), or adverb (adv). Each of the 156 level-topic combinations has 50 entries, and a personal best is saved separately for every combination on the device. Complete all 50 entries to finish; missed targets retry after the remaining entries. The four planets have their own accent colors; the shared space, galaxy and meteor effects remain.

The existing food vocabulary retains its level-specific words. The current 50-word sets for the other topics are shared across planets; the planets still change target speed and retain independent records. Fully distinct CEFR-tiered word banks for all thirty-nine topics require additional authored vocabulary.

| Topic            | Vietnamese label    | Focus                                       |
| ---------------- | ------------------- | ------------------------------------------- |
| Food             | Ẩm thực             | Meals, ingredients and cooking              |
| Home             | Nhà cửa             | Rooms, furniture and household tasks        |
| School           | Trường học          | Classes, subjects and school supplies       |
| Travel           | Du lịch             | Transport, accommodation and travel         |
| Jobs             | Nghề nghiệp         | Occupations and roles                       |
| Body             | Cơ thể              | Body parts and health                       |
| Nature           | Thiên nhiên         | Weather, plants and landscapes              |
| Tech             | Công nghệ           | Everyday devices and applications           |
| Sports           | Thể thao            | Sports, equipment and competition           |
| Work             | Công việc           | Employment, pay and work conditions         |
| Office           | Văn phòng           | Office equipment and spaces                 |
| IT               | Công nghệ thông tin | Software, networks and data                 |
| IT at Work       | IT công sở          | Helpdesk, troubleshooting and workplace IT   |
| Meeting          | Cuộc họp            | Agendas, discussion and decisions           |
| Technical        | Kỹ thuật            | Engineering, machines and measurement       |
| Business         | Kinh doanh          | Companies, markets and growth               |
| Finance          | Tài chính           | Banking, payments and investment            |
| Human Resources  | Nhân sự             | Recruitment, development and staff support  |
| Customer Service | Chăm sóc khách hàng | Support, complaints and customer experience |
| Office & Workplace | Văn phòng             | Workplace roles, spaces, policies and resources |
| People & Colleagues | Đồng nghiệp           | Professional roles, traits and collaboration |
| Daily Work Activities | Công việc hằng ngày | Common work actions and follow-up |
| Communication | Giao tiếp trong công việc | Workplace messages, responses and updates |
| Meetings | Họp hành | Agendas, participation and meeting follow-up |
| Discussion & Opinions | Thảo luận và ý kiến | Agreement, disagreement and recommendations |
| Planning & Priorities | Lập kế hoạch và ưu tiên | Goals, estimates, dependencies and scheduling |
| Project Management | Quản lý dự án | Scope, Agile workflows, risks and deliverables |
| Tasks & Deadlines | Công việc và thời hạn | Assignment, ownership, progress and deadlines |
| Problem Solving | Giải quyết vấn đề | Root causes, troubleshooting and workarounds |
| Critical Thinking & Decisions | Tư duy và quyết định | Evidence, assumptions, reasoning and trade-offs |
| Software Development | Phát triển phần mềm | Applications, requirements, releases and maintenance |
| Programming & Code Review | Lập trình và review code | Code constructs, reviews and refactoring |
| Database & API | Cơ sở dữ liệu và API | Queries, schemas, endpoints and integration |
| System & Architecture | Hệ thống và kiến trúc | Infrastructure, services and system design |
| Testing & Quality | Kiểm thử và chất lượng | Test types, automation and quality assurance |
| Deployment & DevOps | Triển khai và DevOps | CI/CD, containers, infrastructure and monitoring |
| Incidents & Troubleshooting | Sự cố và xử lý lỗi | Outages, diagnostics, recovery and postmortems |
| Jobs & Career | Công việc và sự nghiệp | Job search, interviews and career development |
| Performance & Professional Growth | Hiệu suất và phát triển chuyên môn | Productivity, feedback, skills and growth |

## Plasma upgrades

| Score   | Level      | Damage per shot | Visuals                                             |
| ------- | ---------- | --------------- | --------------------------------------------------- |
| 0–99    | Plasma I   | 1               | Cyan plasma                                         |
| 100–249 | Plasma II  | 2               | Larger mint plasma, longer trail, stronger gun glow |
| 250–499 | Plasma III | 3               | Violet plasma, pulsing energy ring around the gun   |
| 500+    | Plasma IV  | 4               | White core with gold glow, strongest muzzle flash   |

Every destroyed word still awards 10 points. Crossing a threshold displays a two-second upgrade notification, plays the upgrade sound, and emits a ring from the fixed gun without interrupting gameplay. The HUD shows level, damage and progress to the next threshold, or MAX at level IV. In-flight bullets retain their original damage/color; only newly fired shots use the upgrade. Cosmetic bullet growth does not enlarge collision hitboxes. Fire interval (0.22 seconds), projectile speed (850 UI units/second), and aiming speed remain unchanged. Replay resets to level I.

## Prototype scope

- Four CEFR levels (A1, A2, B1, B2), thirty-nine topics per level, and 50 vocabulary entries per level-topic combination, with Vietnamese meanings and example sentences ordered from more common to more specialized terms
- Untimed rounds ending after all 50 selected vocabulary targets are destroyed, retries for missed targets, four tiered fall speeds, and per-topic personal-best records
- Continuously falling targets, letter-based health, hit flashes, explosions, meaning reveals, score, and a result/review summary
- Fixed directional gun, visible projectiles, simultaneous touch aiming/firing, and swept collision detection
- UI and aiming coordinates scale together within the phone's safe area
- One textured red single-barrel cannon at the bottom center; falling-word sparkles and entry rings; layered muzzle flashes, shot smoke, and tapered glowing projectile trails; radial hit sparks; dual explosion shockwaves, smoke, glowing debris, and floating score/meaning reveals
- Vocabulary targets use antialiased rounded gradient cards, soft shadows, high-contrast white text sized to fit long words, segmented pill health bars, a gentle entrance fade and moving highlight. Meaning reveals reuse the card artwork and fade out together with their text. Decorative effects do not alter target hitboxes or move the cannon.

The cannon uses Nido's **Tower Defence Basic Towers** PNG asset under **CC0 1.0**. Source and licensing are recorded in [the art license](Assets/Resources/Art/LICENSE.txt). Only the cannon image is drawn, without an additional tower/base image. Rotation is composed in local UI coordinates before safe-area scaling, keeping the cannon anchored and its visible muzzle aligned with projectile origins at every screen scale. Imported textures are shared resources and are not destroyed with individual game objects. Glow and shockwave textures are generated once at runtime and released with the game object; no additional packages are needed. Visual effects do not change projectile collisions, damage, or keyboard aiming speed.

The shooter uses Unity's built-in legacy input module and does not require a physics package. The original arcade background loop is a locally synthesized PCM WAV; pronunciation WAV files in `Assets/Resources/Audio/Pronunciation` were generated locally with macOS Samantha (US English). Both play through Unity on all targets without runtime text-to-speech or Internet access. Only clips for the active topic are loaded during a round. Replace the prototype UI and the automatic scene bootstrap with authored Unity scenes/prefabs as the game grows.

## Gameplay checks

Audio checks verify the bundled pronunciation for every unique vocabulary term, selected-topic loading, first-hit gameplay speech, FIFO playback, adjustable/persisted gun volume, isolated voice/effect sources, exact 10% gun volume while speaking, restoration after the queue finishes, and cleanup on replay/disable. Vocabulary checks cover all 156 level-topic pools, 50 entries per pool, five prewritten examples per vocabulary entry, supported part-of-speech labels, tier-specific speeds, per-topic high scores and the missed-word review.

With Play mode stopped, select **Word Shooter > Run Gameplay Checks** in Unity. The checks cover level and topic selection, finite vocabulary queues in catalog order, retries without respawning destroyed words, exact 50-word completion, no elapsed-time deadline, angle limits, fast/moving projectile collisions, nearest-target selection, per-letter health, scoring on destruction only, pause and back navigation, firing/audio lockout while paused, uninterrupted gameplay, falling animation and smoke lifetimes, explosion cleanup, penalty-free misses, automatic touch firing, left/right-only keyboard aiming at double speed, crosshair bounds, switching input methods, safe-area coordinates, and round cleanup.

For headless validation with Unity 6.0 installed at the default macOS location:

```sh
/Applications/Unity/Hub/Editor/6000.0.85f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -quit -projectPath "$PWD" \
  -executeMethod WordShooterGameplayChecks.Run -logFile -
```
