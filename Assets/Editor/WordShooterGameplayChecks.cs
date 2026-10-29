using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class WordShooterGameplayChecks
{
    private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static int passed;

    [MenuItem("Word Shooter/Run Gameplay Checks")]
    public static void Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before running checks.");
        passed = 0;
        CheckBallistics();
        RunCase(CheckShooting);
        RunCase(CheckTouchControls);
        RunCase(CheckGunPausesFallingWords);
        RunCase(CheckBackNavigation);
        RunCase(CheckGunVolumeControlLayout);
        RunCase(CheckCompactControls);
        RunCase(CheckSingleGunOrigin);
        RunCase(CheckWordHealth);
        RunCase(CheckMeaningOnHit);
        RunCase(CheckPronunciation);
        RunCase(CheckPronunciationReset);
        RunCase(CheckContinuousHits);
        RunCase(CheckMissedWords);
        RunCase(CheckRepeatedMeaningReview);
        RunCase(CheckExplosionLifetime);
        RunCase(CheckExplosionMeaningTypography);
        RunCase(CheckContinuousSpawning);
        RunCase(CheckNearestTarget);
        RunCase(CheckMovingTarget);
        RunCase(CheckDeadlineCollision);
        RunCase(CheckKeyboardAim);
        RunCase(CheckKeyboardAimBounds);
        RunCase(CheckAimInputSwitching);
        RunCase(CheckRoundReset);
        RunCase(CheckExitRound);
        RunCase(CheckEndRound);
        RunCase(CheckNoTimeLimit);
        RunCase(CheckSafeAreaCoordinates);
        RunCase(CheckGameplayHudLayout);
        RunCase(CheckVisualEffects);
        RunCase(CheckVocabularyRotation);
        RunCase(CheckCefrLevels);
        CheckTopicVocabularyData();
        RunCase(CheckTopicMenuAndTouchScrolling);
        RunCase(CheckWorkplacePhraseVocabulary);
        RunCase(CheckTopicSelectionAndRecords);
        RunCase(CheckLevelRecordsAndReview);
        RunCase(CheckFallingEffects);
        RunCase(CheckPlasmaThresholds);
        RunCase(CheckPlasmaDamage);
        RunCase(CheckProjectileVisualLevels);
        RunCase(CheckPlasmaUpgrade);
        RunCase(CheckProjectilePowerSnapshot);
        CheckGeneratedArt();
        CheckRoundedWordCards();
        CheckScaledGunPivot();
        CheckSpaceTheme();
        CheckMovingSpaceEffects();
        Debug.Log("WordShooter gameplay checks passed: " + passed);
    }

    private static void CheckGeneratedArt()
    {
        var gun = WordShooterArt.LoadGun();
        var glow = WordShooterArt.CreateRadial(false);
        var ring = WordShooterArt.CreateRadial(true);
        try
        {
            Require(gun.width == 139 && gun.height == 253, "Selected cannon asset loads at original resolution");
            Require(glow.width == 64 && ring.width == 64, "Effect textures use bounded resolution");
            Require(!glow.isReadable && !ring.isReadable, "Generated effects release CPU pixel buffers");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(glow);
            UnityEngine.Object.DestroyImmediate(ring);
        }
    }

    private static void CheckSpaceTheme()
    {
        var randomState = UnityEngine.Random.state;
        foreach (var size in new[] { new Vector2(390f, 700f), new Vector2(900f, 700f) })
        {
            for (var index = 0; index < 90; index++)
            {
                var point = WordShooterArt.StarPosition(index, size, 1000f);
                Require(point.x >= 0f && point.x < size.x && point.y >= 0f && point.y < size.y,
                    "Space stars remain within the visible canvas");
            }
        }
        var expected = UnityEngine.Random.value;
        UnityEngine.Random.state = randomState;
        Require(UnityEngine.Random.value == expected, "Star animation does not change gameplay randomness");
        UnityEngine.Random.state = randomState;
        var start = WordShooterArt.StarPosition(1, new Vector2(390f, 700f), 0f);
        var end = WordShooterArt.StarPosition(1, new Vector2(390f, 700f), 1f);
        Require(Mathf.Abs(end.y - start.y - 9f) < 0.001f, "Star layer scrolls at the intended speed");
        var background = WordShooterArt.SpaceBackground;
        var text = WordShooterArt.SpaceText;
        var contrast = (Luminance(text) + 0.05f) / (Luminance(background) + 0.05f);
        Require(contrast >= 7f, "Space HUD text has at least 7:1 contrast against base background");
        Require(WordShooterArt.Plasma.b > 0.9f && WordShooterArt.Plasma.g > 0.8f,
            "Plasma palette is cyan rather than orange");
    }

    private static float Luminance(Color color)
    {
        return 0.2126f * Mathf.GammaToLinearSpace(color.r)
            + 0.7152f * Mathf.GammaToLinearSpace(color.g)
            + 0.0722f * Mathf.GammaToLinearSpace(color.b);
    }

    private static void CheckMovingSpaceEffects()
    {
        var randomState = UnityEngine.Random.state;
        var size = new Vector2(390f, 844f);
        var start = WordShooterArt.MeteorPosition(0, size, 0f, out var startAlpha);
        var middle = WordShooterArt.MeteorPosition(0, size, 1.2f, out var middleAlpha);
        var end = WordShooterArt.MeteorPosition(0, size, 2.4f, out var endAlpha);
        Require(startAlpha == 0f && endAlpha == 0f && middleAlpha > 0.7f, "Meteors fade in and out smoothly");
        Require(middle.x > start.x && middle.y > start.y && end.y > middle.y, "Meteors travel diagonally downward");
        WordShooterArt.MeteorPosition(0, size, 4f, out var idleAlpha);
        Require(idleAlpha == 0f, "Meteor cycles have quiet intervals");
        var repeated = WordShooterArt.MeteorPosition(0, size, 9.2f, out _);
        Require(Vector2.Distance(middle, repeated) < 0.01f, "Meteor cycles repeat without accumulating objects");
        var center = WordShooterArt.GalaxyCenter(size, 0f);
        Require(Vector2.Distance(center, WordShooterArt.GalaxyCenter(size, 20f)) > 5f, "Galaxy center drifts over time");
        for (var index = 0; index < 100; index++)
        {
            var position = WordShooterArt.GalaxyCenter(size, index * 100f);
            Require(position.x > 0f && position.x < size.x && position.y > 0f && position.y < size.y,
                "Galaxy drift stays within the phone canvas");
        }
        var expected = UnityEngine.Random.value;
        UnityEngine.Random.state = randomState;
        Require(UnityEngine.Random.value == expected, "Moving space effects preserve gameplay randomness");
        UnityEngine.Random.state = randomState;
    }

    private static void CheckRoundedWordCards()
    {
        var card = WordShooterArt.CreateWordCard();
        try
        {
            Require(card.width == 252 && card.height == 116, "Word cards render at double UI resolution");
            Require(!card.isReadable, "Card texture releases CPU pixel memory");
            Require(card.wrapMode == TextureWrapMode.Clamp, "Card edges do not repeat");
            Require(WordShooterArt.RoundedCardDistance(Vector2.zero) > 0f, "Card corners are transparent");
            Require(WordShooterArt.RoundedCardDistance(new Vector2(63f, 29f)) < 0f, "Card center is solid");
            Require(Mathf.Abs(WordShooterArt.RoundedCardDistance(new Vector2(63f, 58f))) < 0.001f,
                "Rounded artwork uses the original target bounds");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(card);
        }
    }

    private static void CheckScaledGunPivot()
    {
        var pivot = new Vector2(195f, 635f);
        foreach (var scale in new[] { 0.5f, 1f, 1.5f, 2f })
        {
            var canvas = Matrix4x4.TRS(new Vector3(24f, 40f, 0f), Quaternion.identity,
                new Vector3(scale, scale, 1f));
            foreach (var angle in new[] { -70f, 0f, 70f })
            {
                var rotated = WordShooterArt.RotateAtPivot(canvas, pivot, angle);
                Require(Vector3.Distance(rotated.MultiplyPoint3x4(pivot), canvas.MultiplyPoint3x4(pivot)) < 0.001f,
                    "Rendered cannon stays fixed at every angle and UI scale");
                var direction = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), -Mathf.Cos(angle * Mathf.Deg2Rad));
                var renderedMuzzle = rotated.MultiplyPoint3x4(pivot + Vector2.down * 44f);
                Require(Vector3.Distance(renderedMuzzle, canvas.MultiplyPoint3x4(pivot + direction * 44f)) < 0.001f,
                    "Rendered muzzle matches projectile origin at every UI scale");
            }
        }
    }

    private static void CheckVisualEffects(VocabularyShooterGame game)
    {
        Call(game, "TryFire");
        Require(Get<float>(game, "muzzleFlash") > 0f, "Firing starts muzzle flash");
        var puffs = Get<IList>(game, "shotPuffs");
        Require(puffs.Count == 1, "Each shot creates one smoke effect");
        var puffPosition = Get<Vector2>(puffs[0], "position");
        var puffDirection = Get<Vector2>(puffs[0], "direction");
        Call(game, "SetAimTarget", new Vector2(30f, 250f), true);
        Require(Get<Vector2>(puffs[0], "position") == puffPosition
            && Get<Vector2>(puffs[0], "direction") == puffDirection, "Smoke remains at original muzzle after aiming changes");
        Call(game, "UpdateShotEffects", 0.1f);
        Require(Get<float>(game, "muzzleFlash") == 0f, "Muzzle flash expires");
        var word = Spawn(game, 132f, 300f);
        Call(game, "OnWordHit", word, 1);
        Require(Get<IList>(game, "hitSparks").Count == 1, "Every hit produces sparks");
        Call(game, "UpdateShotEffects", 0.3f);
        Require(Get<IList>(game, "hitSparks").Count == 0, "Hit sparks are cleaned up");
        Require(puffs.Count == 0, "Shot smoke expires after 0.4 seconds");
        Call(game, "OnWordHit", word, 1);
        Call(game, "StartRound", "A1");
        Require(Get<IList>(game, "hitSparks").Count == 0 && Get<float>(game, "muzzleFlash") == 0f,
            "Replay clears transient visuals");
        Require(puffs.Count == 0, "Replay clears shot smoke");
        Call(game, "TryFire");
        Call(game, "EndRound");
        Require(Get<float>(game, "muzzleFlash") == 0f, "Result screen clears flash");
        Require(puffs.Count == 0, "Result screen clears smoke");
    }

    private static void CheckVocabularyRotation(VocabularyShooterGame game)
    {
        var entries = (Array)typeof(VocabularyShooterGame).GetField("FoodWords",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Require(entries.Length == 60, "Original food vocabulary remains available");
        Call(game, "StartRound", "A1");
        for (var cycle = 0; cycle < 2; cycle++)
        {
            Call(game, "StartRound", "A1");
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var pool = Property<Array>(game, "CurrentWordPool");
            for (var index = 0; index < pool.Length; index++)
            {
                var entry = Call(game, "NextWord");
                var word = Get<string>(entry, "word");
                Require(seen.Add(word), "Each round initially queues each word exactly once");
                Require(ReferenceEquals(entry, pool.GetValue(index)),
                    "Words appear in curated common-to-specialized catalog order");
                Require(!string.IsNullOrWhiteSpace(Get<string>(entry, "meaning"))
                    && !string.IsNullOrWhiteSpace(Get<string>(entry, "example")), "Every word has meaning and example");
                Require(word.Length >= 3 && word.Length <= 15, "Words fit target cards and letter-health rules");
            }
        }
        Require(((ICollection)Get<object>(game, "wordBag")).Count == 0, "Exhausted vocabulary bag does not refill itself");
        Call(game, "UpdateSpawning", 10f);
        Require(Get<IList>(game, "fallingWords").Count == 0, "Empty bag does not spawn extra words");
        Call(game, "StartRound", "A1");
        Require(((ICollection)Get<object>(game, "wordBag")).Count == 50, "Replay queues a fresh set of 50 words");
    }

    private static void CheckCefrLevels(VocabularyShooterGame game)
    {
        var pools = (Array)typeof(VocabularyShooterGame).GetField("LevelWordPools",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var ids = (string[])typeof(VocabularyShooterGame).GetField("LevelIds",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var speeds = (float[])typeof(VocabularyShooterGame).GetField("LevelSpeeds",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Require(ids.Length == 4 && pools.Length == 4, "Four CEFR food levels are defined");
        for (var levelIndex = 0; levelIndex < ids.Length; levelIndex++)
        {
            var pool = (Array)pools.GetValue(levelIndex);
            Require(pool.Length == 50, "Each CEFR level contains exactly 50 food words");
            Call(game, "StartRound", ids[levelIndex]);
            Require(Get<string>(game, "currentLevel") == ids[levelIndex], "Selected CEFR level is retained");
            Require(Property<string>(game, "RoundProgressText") == "0/50", "Each CEFR round starts with zero of fifty completed");
            Require(Property<Color>(game, "CurrentPlanetColor") == WordShooterArt.PlanetColor(levelIndex),
                "Each CEFR level has its own planet palette");
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < pool.Length; index++)
            {
                var entry = pool.GetValue(index);
                var word = Get<string>(entry, "word");
                Require(seen.Add(word), "Each CEFR vocabulary has no duplicates within its topic");
                Require(Get<string>(entry, "level") == ids[levelIndex], "Vocabulary is tagged to its CEFR tier");
                Require(Get<string>(entry, "topic") == "food", "Food entries carry the selected topic");
                Require(!string.IsNullOrWhiteSpace(Get<string>(entry, "meaning"))
                    && !string.IsNullOrWhiteSpace(Get<string>(entry, "example")), "Every level word has meaning and example");
                Require(word.Length >= 3 && word.Length <= 15, "Level words remain readable on target cards");
            }
            Call(game, "SpawnWord");
            var spawned = Get<IList>(game, "fallingWords");
            var spawnedWord = spawned[0];
            var swayAmplitude = Get<float>(spawnedWord, "swayAmplitude");
            Require(swayAmplitude >= 28f && swayAmplitude <= 42f,
                "New words receive a lively but bounded sway amplitude");
            var expectedSpeed = speeds[levelIndex];
            var speed = Get<float>(spawnedWord, "speed");
            Require(speed >= expectedSpeed * 0.9f && speed <= expectedSpeed * 1.15f,
                "Each CEFR tier uses its configured fall speed");
        }
    }

    private static void CheckTopicVocabularyData()
    {
        var topics = VocabularyTopicData.Topics;
        var ids = (string[])typeof(VocabularyShooterGame).GetField("TopicIds",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var pools = (Array)typeof(VocabularyShooterGame).GetField("TopicLevelWordPools",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Require(topics.Length == 39 && ids.Length == 39 && pools.Length == 4,
            "Four levels expose all thirty-nine topics");
        var uniquePronunciations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var topicIndex = 0; topicIndex < topics.Length; topicIndex++)
        {
            var topic = topics[topicIndex];
            Require(topic.Id == ids[topicIndex] && topic.Words.Length == 50,
                "Every topic has its expected id and fifty vocabulary records");
            var uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var word in topic.Words)
            {
                Require(uniqueWords.Add(word.Text + "\n" + word.Meaning) || topicIndex >= 19,
                    "Legacy topics do not contain duplicate vocabulary records");
                Require(word.Text.Length >= 3 && word.Text.Length <= 40
                    && !string.IsNullOrWhiteSpace(word.Meaning)
                    && !string.IsNullOrWhiteSpace(word.Example)
                    && !word.Example.StartsWith("We use ", StringComparison.Ordinal)
                    && word.Example.IndexOf(word.Text, StringComparison.OrdinalIgnoreCase) >= 0,
                    "Topic vocabulary includes a readable term, meaning, and example");
                Require(word.PartOfSpeech == "n" || word.PartOfSpeech == "v"
                    || word.PartOfSpeech == "adj" || word.PartOfSpeech == "adv",
                    "Every vocabulary entry has a supported part-of-speech label");
                Require(word.AdditionalExamples.Length == 4,
                    "Every topic word has four additional example sentences");
                foreach (var example in word.AdditionalExamples)
                    Require(!string.IsNullOrWhiteSpace(example)
                        && example.IndexOf(word.Text, StringComparison.OrdinalIgnoreCase) >= 0
                        && !string.Equals(example, word.Example, StringComparison.Ordinal),
                        "Additional examples are distinct and include their vocabulary term");
                if (uniquePronunciations.Add(word.Text))
                {
                    var clip = Resources.Load<AudioClip>("Audio/Pronunciation/"
                        + VocabularyTopicData.PronunciationResourceName(word.Text));
                    Require(clip != null && clip.samples > 0 && clip.length > 0f,
                        "Every unique topic word has offline pronunciation");
                }
            }
            for (var levelIndex = 0; levelIndex < 4; levelIndex++)
            {
                var levelPools = (Array)pools.GetValue(levelIndex);
                var pool = (Array)levelPools.GetValue(topicIndex);
                Require(pool.Length == 50, "Every level-topic combination contains fifty words");
                var poolWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var entry in pool)
                {
                    Require((poolWords.Add(Get<string>(entry, "word") + "\n" + Get<string>(entry, "meaning"))
                        || topicIndex >= 19)
                        && Get<string>(entry, "level") == new[] { "A1", "A2", "B1", "B2" }[levelIndex]
                        && Get<string>(entry, "topic") == topic.Id,
                        "Level-topic entries are tagged and unique");
                    Require(Get<string[]>(entry, "examples").Length == 5,
                        "Every playable word has five total example sentences");
                }
            }
        }
        var allWords = (Array)typeof(VocabularyShooterGame).GetField("AllWords",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Require(allWords.Length >= uniquePronunciations.Count,
            "Global vocabulary includes every topic word and retained legacy food word");
        foreach (var entry in allWords)
        {
            Require(Get<string[]>(entry, "examples").Length == 5,
                "Every retained vocabulary entry has five stored examples");
            var clip = Resources.Load<AudioClip>("Audio/Pronunciation/"
                + VocabularyTopicData.PronunciationResourceName(Get<string>(entry, "word")));
            Require(clip != null && clip.samples > 0 && clip.length > 0f,
                "Every retained vocabulary entry has a pronunciation clip");
        }
    }

    private static void CheckTopicMenuAndTouchScrolling(VocabularyShooterGame game)
    {
        var ids = (string[])typeof(VocabularyShooterGame).GetField("TopicIds",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var levelNames = (string[])typeof(VocabularyShooterGame).GetField("LevelNames",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var planetNames = (string[])typeof(VocabularyShooterGame).GetField("PlanetNames",
            BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var topics = VocabularyTopicData.Topics;
        var getTopicName = typeof(VocabularyShooterGame).GetMethod("TopicName",
            BindingFlags.Static | BindingFlags.NonPublic);
        Call(game, "PrepareStyles");
        var rowHeight = (float)typeof(VocabularyShooterGame).GetField("TopicRowHeight",
            BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
        var rowGap = (float)typeof(VocabularyShooterGame).GetField("TopicRowGap",
            BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
        Require(ids.Length == 39 && topics.Length == ids.Length,
            "Every topic has a numbered English menu label");
        Require(VocabularyTopicData.InferPartOfSpeech("project", "The project is ready.") == "n"
            && VocabularyTopicData.InferPartOfSpeech("develop", "We develop a new feature.") == "v"
            && VocabularyTopicData.InferPartOfSpeech("available", "The service is available.") == "adj"
            && VocabularyTopicData.InferPartOfSpeech("quickly", "Please respond quickly.") == "adv",
            "Common noun, verb, adjective, and adverb tags are inferred correctly");
        Require(levelNames.Length == 4 && planetNames.Length == 4,
            "All level and planet labels are available in English");
        for (var index = 0; index < topics.Length; index++)
        {
            var displayedName = (string)getTopicName.Invoke(null, new object[] { index });
            Require(!string.IsNullOrWhiteSpace(displayedName)
                && displayedName == topics[index].DisplayName
                && displayedName.IndexOfAny("áàảãạăắằẳẵặâấầẩẫậđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵ".ToCharArray()) < 0,
                "Topic menu labels are displayed in English");
        }

        foreach (var label in levelNames)
            Require(IsEnglishLabel(label), "Level names are displayed in English");
        foreach (var label in planetNames)
            Require(IsEnglishLabel(label), "Planet names are displayed in English");

        Call(game, "UpdateLayout");
        var viewport = Property<Rect>(game, "TopicSelectionViewport");
        var topicTextWidth = viewport.width - 14f - 32f;
        for (var index = 0; index < topics.Length; index++)
        {
            var displayedName = (string)getTopicName.Invoke(null, new object[] { index });
            var style = (GUIStyle)Call(game, "CreateTopicNameStyle", displayedName, topicTextWidth);
            Require(style.fontSize >= 9 && !style.wordWrap
                && style.CalcSize(new GUIContent(displayedName)).x <= topicTextWidth - 4f,
                "Every full topic name fits on one line in its vertical row");
        }
        var start = viewport.center;
        Set(game, "screenState", Enum.Parse(typeof(VocabularyShooterGame).GetNestedType(
            "ScreenState", BindingFlags.NonPublic), "TopicSelection"));
        Set(game, "topicScrollPosition", Vector2.zero);
        var originalTopic = Get<string>(game, "currentTopic");
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 71, phase = TouchPhase.Began }, start);
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 71, phase = TouchPhase.Moved }, start + Vector2.down * 5f);
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 71, phase = TouchPhase.Moved }, start + Vector2.down * 40f);
        Require(Get<Vector2>(game, "topicScrollPosition").y == 40f,
            "Dragging up moves the topic list up to reveal topics below");
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 71, phase = TouchPhase.Ended }, start + Vector2.down * 45f);
        Require(Get<string>(game, "currentTopic") == originalTopic
            && Get<object>(game, "screenState").ToString() == "TopicSelection",
            "A topic swipe cannot activate a row");
        var maxScroll = Property<float>(game, "TopicScrollMax");
        Set(game, "topicScrollPosition", new Vector2(0f, maxScroll));
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 74, phase = TouchPhase.Began }, start);
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 74, phase = TouchPhase.Moved }, start + Vector2.up * 40f);
        Require(Mathf.Abs(Get<Vector2>(game, "topicScrollPosition").y - (maxScroll - 40f)) < 0.001f,
            "Dragging down moves the topic list down toward its beginning");
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 74, phase = TouchPhase.Canceled }, start + Vector2.up * 40f);
        Set(game, "topicScrollPosition", new Vector2(0f, maxScroll));
        var rowStride = rowHeight + rowGap;
        var lastTopicPoint = new Vector2(viewport.center.x,
            viewport.yMin + (ids.Length - 1) * rowStride + rowHeight * 0.5f - maxScroll);
        Require((int)Call(game, "TopicIndexAtPoint", lastTopicPoint) == ids.Length - 1,
            "The last topic is reachable after scrolling to the bottom");
        Set(game, "topicScrollPosition", Vector2.zero);
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 72, phase = TouchPhase.Began }, start);
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 72, phase = TouchPhase.Moved }, start + Vector2.up * 4000f);
        Require(Get<Vector2>(game, "topicScrollPosition").y == 0f,
            "Dragging back toward the beginning clamps at the top");
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 72, phase = TouchPhase.Canceled }, start + Vector2.up * 4000f);

        Set(game, "topicScrollPosition", Vector2.zero);
        var topicIndex = 1;
        var tap = new Vector2(viewport.center.x,
            viewport.yMin + topicIndex * rowStride + rowHeight * 0.5f);
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 73, phase = TouchPhase.Began }, tap);
        Call(game, "HandleTopicSelectionTouch",
            new Touch { fingerId = 73, phase = TouchPhase.Ended }, tap);
        Require(Get<string>(game, "currentTopic") == ids[topicIndex]
            && Get<object>(game, "screenState").ToString() == "Playing",
            "A deliberate tap selects the topic under the finger");
    }

    private static bool IsEnglishLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label)) return false;
        foreach (var character in label)
            if (character >= 128) return false;
        return true;
    }

    private static void CheckTopicSelectionAndRecords(VocabularyShooterGame game)
    {
        const string foodKey = "WordGalaxy.BestScore.A1.food";
        const string homeKey = "WordGalaxy.BestScore.A1.home";
        const string legacyFoodKey = "WordGalaxy.BestScore.A1";
        var hadFood = PlayerPrefs.HasKey(foodKey);
        var oldFood = PlayerPrefs.GetInt(foodKey);
        var hadHome = PlayerPrefs.HasKey(homeKey);
        var oldHome = PlayerPrefs.GetInt(homeKey);
        var hadLegacyFood = PlayerPrefs.HasKey(legacyFoodKey);
        var oldLegacyFood = PlayerPrefs.GetInt(legacyFoodKey);
        try
        {
            PlayerPrefs.DeleteKey(foodKey);
            PlayerPrefs.DeleteKey(homeKey);
            PlayerPrefs.DeleteKey(legacyFoodKey);
            Call(game, "StartRound", "A1");
            Set(game, "score", 20);
            Call(game, "EndRound");
            Require(PlayerPrefs.GetInt(foodKey) == 20, "Food record is saved under its level-topic key");

            Call(game, "TryStartTopic", "home");
            Require(Get<string>(game, "currentTopic") == "home"
                && Property<string>(game, "RoundProgressText") == "0/50"
                && Property<bool>(game, "CanShoot"),
                "Choosing a topic starts an untimed fifty-word round");
            Require(Get<int>(game, "bestScore") == 0, "Topic records do not leak between topics");
            Set(game, "score", 30);
            Call(game, "EndRound");
            Require(PlayerPrefs.GetInt(homeKey) == 30 && PlayerPrefs.GetInt(foodKey) == 20,
                "Each topic retains its own personal best");
        }

        finally
        {
            if (hadFood) PlayerPrefs.SetInt(foodKey, oldFood);
            else PlayerPrefs.DeleteKey(foodKey);
            if (hadHome) PlayerPrefs.SetInt(homeKey, oldHome);
            else PlayerPrefs.DeleteKey(homeKey);
            if (hadLegacyFood) PlayerPrefs.SetInt(legacyFoodKey, oldLegacyFood);
            else PlayerPrefs.DeleteKey(legacyFoodKey);
            PlayerPrefs.Save();
        }
    }

    private static void CheckWorkplacePhraseVocabulary(VocabularyShooterGame game)
    {
        var topic = VocabularyTopicData.GetTopic("problem_solving");
        VocabularyTopicData.Word phrase = null;
        foreach (var word in topic.Words)
            if (word.Text == "root-cause analysis") phrase = word;
        Require(phrase != null, "Imported topics retain multi-word vocabulary terms");
        Require(VocabularyTopicData.LetterCount(phrase.Text) == 17,
            "Phrase health counts letters but ignores spaces");
        var pronunciationName = VocabularyTopicData.PronunciationResourceName("CI/CD");
        Require(pronunciationName.StartsWith("WorkVocabulary/", StringComparison.Ordinal),
            "Acronym and punctuation pronunciation paths are safe Resources names");
        Require(topic.Words.Length == 50, "Repeated terms with distinct usage keep all fifty topic entries");
    }

    private static void CheckLevelRecordsAndReview(VocabularyShooterGame game)
    {
        const string a1Key = "WordGalaxy.BestScore.A1";
        const string b2Key = "WordGalaxy.BestScore.B2";
        const string a1FoodKey = "WordGalaxy.BestScore.A1.food";
        const string b2FoodKey = "WordGalaxy.BestScore.B2.food";
        var hadA1 = PlayerPrefs.HasKey(a1Key);
        var oldA1 = PlayerPrefs.GetInt(a1Key);
        var hadB2 = PlayerPrefs.HasKey(b2Key);
        var oldB2 = PlayerPrefs.GetInt(b2Key);
        var hadA1Food = PlayerPrefs.HasKey(a1FoodKey);
        var oldA1Food = PlayerPrefs.GetInt(a1FoodKey);
        var hadB2Food = PlayerPrefs.HasKey(b2FoodKey);
        var oldB2Food = PlayerPrefs.GetInt(b2FoodKey);
        try
        {
            PlayerPrefs.DeleteKey(a1Key);
            PlayerPrefs.DeleteKey(b2Key);
            PlayerPrefs.DeleteKey(a1FoodKey);
            PlayerPrefs.DeleteKey(b2FoodKey);
            Call(game, "StartRound", "A1");
            Set(game, "score", 120);
            Call(game, "EndRound");
            Require(PlayerPrefs.GetInt(a1Key) == 120 && PlayerPrefs.GetInt(a1FoodKey) == 120
                && Get<bool>(game, "newRecord"),
                "Round completion persists a new personal best");
            Call(game, "StartRound", "B2");
            Require(Get<int>(game, "bestScore") == 0 && PlayerPrefs.GetInt(b2Key) == 0
                && PlayerPrefs.GetInt(b2FoodKey) == 0,
                "Personal bests are independent between CEFR levels");

            var first = Spawn(game, 100f, 510f);
            var duplicate = Spawn(game, 220f, 510f);
            Set(duplicate, "entry", Get<object>(first, "entry"));
            Call(game, "RemoveMissedWords");
            var missed = Get<IList>(game, "missedWords");
            Require(missed.Count == 1, "Review collection deduplicates a missed word");
            Require(ReferenceEquals(missed[0], Get<object>(first, "entry"))
                && !string.IsNullOrWhiteSpace(Get<string>(missed[0], "meaning")),
                "Review entries retain their Vietnamese meanings");
        }
        finally
        {
            if (hadA1) PlayerPrefs.SetInt(a1Key, oldA1);
            else PlayerPrefs.DeleteKey(a1Key);
            if (hadB2) PlayerPrefs.SetInt(b2Key, oldB2);
            else PlayerPrefs.DeleteKey(b2Key);
            if (hadA1Food) PlayerPrefs.SetInt(a1FoodKey, oldA1Food);
            else PlayerPrefs.DeleteKey(a1FoodKey);
            if (hadB2Food) PlayerPrefs.SetInt(b2FoodKey, oldB2Food);
            else PlayerPrefs.DeleteKey(b2FoodKey);
            PlayerPrefs.Save();
        }
    }

    private static void CheckFallingEffects(VocabularyShooterGame game)
    {
        var word = Spawn(game, 132f, 250f);
        Set(word, "swayAmplitude", 12f);
        Set(word, "swayFrequency", 1.44f);
        Set(word, "swayPhase", 0f);
        var pivot = Property<Vector2>(game, "GunPosition");
        Call(game, "MoveWords", 0.2f);
        Require(Mathf.Abs(Get<float>(word, "age") - 0.2f) < 0.001f, "Fall animation advances with gameplay time");
        Require(Mathf.Abs(Get<float>(word, "x") - (132f + Mathf.Sin(0.2f * 1.44f) * 12f)) < 0.001f
            && Get<float>(word, "y") == 270f, "Words sway horizontally while falling at the same vertical speed");
        Require(Get<float>(word, "previousX") == 132f, "Sway preserves swept collision history");
        Require(Get<float>(word, "previousY") == 250f, "Fall effects preserve swept collision history");
        Set(game, "score", 500);
        Call(game, "MoveWords", 0.2f);
        Require(Mathf.Abs(Get<float>(word, "y") - 296.4f) < 0.001f,
            "Falling words speed up by thirty-two percent at maximum plasma level");
        Set(game, "spawnTimer", 0.01f);
        Call(game, "UpdateSpawning", 0.02f);
        var spawnTimer = Get<float>(game, "spawnTimer");
        Require(spawnTimer >= 4.2f / 1.32f && spawnTimer <= 5f / 1.32f,
            "Words spawn farther apart while retaining the plasma speed adjustment");
        Require(Property<Vector2>(game, "GunPosition") == pivot, "Animated effects never move the cannon");
    }

    private static void CheckPlasmaThresholds(VocabularyShooterGame game)
    {
        var points = new[] { 0, 90, 99, 100, 210, 219, 220, 350, 359, 360, 440, 449, 450, 600 };
        var levels = new[] { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5 };
        for (var index = 0; index < points.Length; index++)
        {
            Set(game, "score", points[index]);
            Require(Property<int>(game, "PlasmaLevel") == levels[index], "Plasma upgrades at exact score thresholds");
            var progress = Property<float>(game, "UpgradeProgress");
            Require(progress >= 0f && progress <= 1f, "Progress stays normalized at every score");
        }
        Set(game, "score", 160);
        Require(Mathf.Abs(Property<float>(game, "UpgradeProgress") - 0.5f) < 0.001f,
            "Progress measures current tier rather than whole score");
        Set(game, "score", 450);
        Require(Property<float>(game, "UpgradeProgress") == 1f, "Max level shows full progress without another threshold");
    }

    private static void CheckPlasmaDamage(VocabularyShooterGame game)
    {
        var scores = new[] { 0, 100, 220, 360, 450 };
        var expectedDamage = new[] { 0.735f, 0.882f, 1.029f, 1.176f, 1.323f };
        var expectedShots = new[] { 7, 6, 5, 5, 4 };
        for (var level = 1; level <= 5; level++)
        {
            Call(game, "StartRound", "A1");
            Set(game, "score", scores[level - 1]);
            var word = Spawn(game, 132f, 300f);
            Set(word, "hitsRemaining", 5);
            for (var shot = 1; shot <= expectedShots[level - 1]; shot++)
            {
                Set(game, "shotCooldown", 0f);
                Call(game, "TryFire");
                var projectile = Get<IList>(game, "projectiles")[0];
                Require(Mathf.Abs(Get<float>(projectile, "damage") - expectedDamage[level - 1]) < 0.001f,
                    "New shot stores the level damage multiplier");
                Require(Get<int>(projectile, "level") == level, "New shot stores its plasma visual tier");
                Require(Get<Vector2>(projectile, "position") == Property<Vector2>(game, "MuzzlePosition"),
                    "All upgraded shots originate from the same gun");
                Require(Get<Vector2>(projectile, "velocity").magnitude == 850f, "Upgrades preserve projectile speed");
                Require(Mathf.Abs(Get<float>(game, "shotCooldown") - 0.22f / 0.7f / 0.4f / 1.2f / 1.35f) < 0.001f,
                    "All five levels use thirty-five percent more shots per second");
                Call(game, "MoveProjectiles", 0.5f);
                var totalDamage = shot * expectedDamage[level - 1];
                Require(Get<int>(word, "hitsRemaining") == Mathf.Max(0, 5 - Mathf.FloorToInt(totalDamage + 0.0001f)),
                    "Fractional damage accumulates and clamps health");
            }
            Require(Get<int>(game, "score") == scores[level - 1] + 10, "Destruction awards exactly ten points at every tier");
            Require(Get<int>(game, "destroyedCount") == 1, "Overkill does not duplicate rewards");
        }
    }

    private static void CheckProjectileVisualLevels(VocabularyShooterGame game)
    {
        var expectedTrails = new[] { 3, 8, 10, 12, 14 };
        var expectedSparks = new[] { 0, 3, 4, 5, 6 };
        var expectedMuzzleStreaks = new[] { 3, 6, 8, 10, 12 };
        var previousPower = 0f;
        var previousColor = Color.clear;
        for (var level = 1; level <= 5; level++)
        {
            Require((int)CallStatic("ProjectileTrailCount", level) == expectedTrails[level - 1],
                "Projectile trails increase at every gun level");
            Require((int)CallStatic("ProjectileSparkCount", level) == expectedSparks[level - 1],
                "Projectile sparks increase at every gun level");
            Require((int)CallStatic("MuzzleStreakCount", level) == expectedMuzzleStreaks[level - 1],
                "Muzzle streaks increase at every gun level");
            var power = (float)CallStatic("ProjectilePowerScale", level);
            var color = (Color)CallStatic("PlasmaColor", level);
            Require(level == 1 ? Mathf.Abs(power - 1f) < 0.001f : power > previousPower,
                "Level 1 stays standard and upgraded projectile cores grow stronger");
            Require(level == 1 ? color == WordShooterArt.Plasma : color != previousColor,
                "Level 1 keeps the standard color and each upgrade changes it");
            previousPower = power;
            previousColor = color;
        }
    }

    private static void CheckPlasmaUpgrade(VocabularyShooterGame game)
    {
        foreach (var boundary in new[] { 100, 220, 360, 450 })
        {
            Call(game, "StartRound", "A1");
            Set(game, "score", boundary - 10);
            var target = Spawn(game, 132f, 300f);
            var other = Spawn(game, 18f, 230f);
            Set(target, "hitsRemaining", 1);
            Call(game, "CaptureTouch", 7, new Vector2(195f, 300f));
            Call(game, "OnWordHit", target, 1);
            Require(Get<float>(game, "upgradeTimer") == 2f, "Threshold kill triggers upgrade notification");
            Require(Get<IList>(game, "fallingWords").Contains(other), "Level-up preserves other targets");
            Require(Get<int>(game, "aimFingerId") == 7 && Property<bool>(game, "CanShoot"), "Level-up never interrupts aiming or firing");
            Call(game, "UpdateShotEffects", 2.1f);
            Require(Get<float>(game, "upgradeTimer") == 0f, "Upgrade effect expires");
        }
        Call(game, "StartRound", "A1");
        Require(Property<int>(game, "PlasmaLevel") == 1 && Get<float>(game, "upgradeTimer") == 0f,
            "Replay resets power and upgrade visuals");
    }

    private static void CheckProjectilePowerSnapshot(VocabularyShooterGame game)
    {
        Set(game, "score", 90);
        Call(game, "TryFire");
        Set(game, "score", 100);
        var word = Spawn(game, 132f, 300f);
        Set(word, "hitsRemaining", 5);
        Call(game, "MoveProjectiles", 0.5f);
        Require(Get<int>(word, "hitsRemaining") == 5
            && Mathf.Abs(Get<float>(word, "damageCarry") - 0.735f) < 0.001f,
            "In-flight shots retain reduced damage from firing time");
        Set(game, "shotCooldown", 0f);
        Call(game, "TryFire");
        Call(game, "MoveProjectiles", 0.5f);
        Require(Get<int>(word, "hitsRemaining") == 4
            && Mathf.Abs(Get<float>(word, "damageCarry") - 0.617f) < 0.001f,
            "New shots add the upgraded reduced fractional damage");
    }

    private static void CheckBallistics()
    {
        var origin = new Vector2(195f, 620f);
        Require(WordShooterBallistics.AimDirection(origin, origin + Vector2.down * 100f) == Vector2.down, "Aim up");
        var right = WordShooterBallistics.AimDirection(origin, origin + Vector2.right * 100f);
        Require(Mathf.Abs(right.magnitude - 1f) < 0.0001f && right.y < 0f, "Normalized upward aim");
        Require(Mathf.Abs(Vector2.Angle(Vector2.down, right) - 70f) < 0.001f, "Right limit is 70 degrees");
        var left = WordShooterBallistics.AimDirection(origin, origin + new Vector2(-100f, 100f));
        Require(left.x < 0f && Mathf.Abs(Vector2.Angle(Vector2.down, left) - 70f) < 0.001f, "Left limit");
        var word = new Rect(100f, 250f, 126f, 58f);
        Require(WordShooterBallistics.TryHitWord(new Vector2(150f, 600f), new Vector2(150f, 100f),
            word, 5f, out var fraction) && fraction > 0f && fraction < 1f, "Fast shot cannot tunnel");
        Require(!WordShooterBallistics.TryHitWord(new Vector2(50f, 600f), new Vector2(50f, 100f),
            word, 5f, out _), "Off-target shot misses");
        Require(WordShooterBallistics.TryHitWord(new Vector2(96f, 600f), new Vector2(96f, 100f),
            word, 5f, out _), "Projectile radius counts");
        Require(!WordShooterBallistics.TryHitWord(new Vector2(150f, 600f), new Vector2(150f, 700f),
            word, 5f, out _), "Target behind shot is not hit");
    }

    private static void RunCase(Action<VocabularyShooterGame> check)
    {
        var owner = new GameObject("WordShooterGameplayCheck") { hideFlags = HideFlags.HideAndDontSave };
        try
        {
            var game = owner.AddComponent<VocabularyShooterGame>();
            Call(game, "Awake");
            Set(game, "uiWidth", 390f);
            Set(game, "uiHeight", 700f);
            Call(game, "StartRound", "A1");
            check(game);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(owner);
        }
    }

    private static void CheckShooting(VocabularyShooterGame game)
    {
        var pivot = Property<Vector2>(game, "GunPosition");
        Call(game, "TryFire");
        Call(game, "TryFire");
        var shots = Get<IList>(game, "projectiles");
        Require(shots.Count == 1, "Shot cooldown blocks duplicates");
        Require(Get<Vector2>(shots[0], "position") == pivot + Vector2.down * 44f, "Shot starts at muzzle");
        Call(game, "MoveProjectiles", 0.1f);
        Require(Mathf.Abs(Get<Vector2>(shots[0], "position").y - (pivot.y - 129f)) < 0.01f, "Shot speed is 850");
        Call(game, "MoveProjectiles", 1f);
        Require(shots.Count == 0 && Get<int>(game, "score") == 0, "Missed shots are removed without penalty");
        Set(game, "shotCooldown", 0f);
        Call(game, "TryFire");
        Require(shots.Count == 1, "Can shoot again after cooldown");
    }

    private static void CheckGunPausesFallingWords(VocabularyShooterGame game)
    {
        var pauseArea = Property<Rect>(game, "GunPauseTouchArea");
        var tapPoint = pauseArea.center;
        var word = Spawn(game, 132f, 300f);
        var initialY = Get<float>(word, "y");
        var entry = Get<object>(word, "entry");
        Require(Get<string[]>(entry, "examples").Length == 5,
            "The paused target has five prepared examples to display");
        Call(game, "OnWordHit", word, 1);
        Require(ReferenceEquals(Get<object>(game, "lastHitWord"), entry),
            "The example list follows the last word hit by the player");
        Require((bool)Call(game, "HandlePauseToggleTouch",
            new Touch { fingerId = 91, phase = TouchPhase.Began }, tapPoint),
            "Tapping the gun toggles word pause");
        Require(Get<bool>(game, "wordsPaused"), "Gun tap pauses falling words");
        Call(game, "AdvanceGameplay", 0.5f);
        Require(Get<float>(word, "y") == initialY, "Falling words stay still while paused");
        Require(Get<string[]>(entry, "examples").Length == 5
            && Get<string[]>(entry, "examples")[0] == Get<string>(entry, "example"),
            "The pause panel has five prepared examples and preserves the primary example");
        Require(!(bool)Call(game, "HandlePauseToggleTouch",
            new Touch { fingerId = 92, phase = TouchPhase.Moved }, tapPoint),
            "Dragging a finger over the gun does not toggle word pause");
        Require(Get<bool>(game, "wordsPaused"), "Pause remains active without another tap");
        Call(game, "HandlePauseToggleTouch",
            new Touch { fingerId = 92, phase = TouchPhase.Began }, tapPoint);
        Require(!Get<bool>(game, "wordsPaused"), "Tapping the gun again resumes falling words");
        Require(Get<string[]>(entry, "examples")[0] == Get<string>(entry, "example"),
            "Resuming hides the additional examples and leaves the original first example unchanged");
        Call(game, "AdvanceGameplay", 0.5f);
        Require(Get<float>(word, "y") > initialY, "Falling words resume after the second gun tap");

        Set(game, "shotCooldown", 0f);
        Call(game, "TryFire");
        var projectiles = Get<IList>(game, "projectiles");
        var shotCount = projectiles.Count;
        var shotPosition = Get<Vector2>(projectiles[0], "position");
        Call(game, "HandlePauseToggleTouch",
            new Touch { fingerId = 93, phase = TouchPhase.Began }, tapPoint);
        Call(game, "TryFire");
        Call(game, "AdvanceGameplay", 0.5f);
        Require(projectiles.Count == shotCount
            && Get<Vector2>(projectiles[0], "position") == shotPosition,
            "Pause blocks new shots and freezes in-flight projectiles");

        Call(game, "ResetPronunciation");
        var pronunciationQueue = Get<Queue<AudioClip>>(game, "pronunciationQueue");
        var queuedClip = AudioClip.Create("pause-check", 128, 1, 44100, false);
        pronunciationQueue.Enqueue(queuedClip);
        Call(game, "UpdatePronunciation");
        Require(pronunciationQueue.Count == 1, "Pronunciation playback does not advance while paused");
        Call(game, "HandlePauseToggleTouch",
            new Touch { fingerId = 94, phase = TouchPhase.Began }, tapPoint);
        Require(!Get<bool>(game, "wordsPaused"), "Gun tap resumes the paused round");
        Call(game, "UpdatePronunciation");
        Require(pronunciationQueue.Count == 0,
            "Queued pronunciation resumes after the round is unpaused");

        Call(game, "StartRound", "A1");
        Require(!Get<bool>(game, "wordsPaused"), "Starting a new round clears word pause");
        UnityEngine.Object.DestroyImmediate(queuedClip);
    }

    private static void CheckBackNavigation(VocabularyShooterGame game)
    {
        Require((bool)CallStatic("IsBackSwipeOrigin", new Vector2(44f, 300f))
            && !(bool)CallStatic("IsBackSwipeOrigin", new Vector2(45f, 300f)),
            "Back navigation starts only at the left screen edge");
        Require((bool)CallStatic("IsBackSwipe", new Vector2(10f, 300f), new Vector2(90f, 360f))
            && !(bool)CallStatic("IsBackSwipe", new Vector2(10f, 300f), new Vector2(89f, 300f))
            && (bool)CallStatic("IsBackSwipe", new Vector2(10f, 300f), new Vector2(90f, 390f))
            && !(bool)CallStatic("IsBackSwipe", new Vector2(10f, 300f), new Vector2(90f, 391f)),
            "Back navigation requires a sufficiently long, mostly horizontal right swipe");
        Call(game, "UpdateLayout");
        var backArea = Property<Rect>(game, "BackSwipeTouchArea");
        Require(Mathf.Abs(backArea.height - Get<float>(game, "uiHeight") * 0.8f) < 0.01f,
            "Back swipe hit area matches the tall visible edge indicator");

        Call(game, "StartRound", "A1");
        Set(game, "backSwipeFingerId", 77);
        Call(game, "HandleShootingInput", 0.016f);
        Require(Get<IList>(game, "projectiles").Count == 0,
            "A recognized back-swipe gesture cannot fire the gun");
        Set(game, "backSwipeFingerId", -1);
        Call(game, "NavigateBack");
        Require(Get<bool>(game, "confirmingExit"),
            "Swiping back during gameplay asks for exit confirmation");
        Set(game, "confirmingExit", false);

        Set(game, "screenState", Enum.Parse(typeof(VocabularyShooterGame).GetNestedType(
            "ScreenState", BindingFlags.NonPublic), "Result"));
        Call(game, "NavigateBack");
        Require(Get<object>(game, "screenState").ToString() == "TopicSelection",
            "Swiping back from results returns to topic selection");

        Call(game, "NavigateBack");
        Require(Get<object>(game, "screenState").ToString() == "Menu",
            "Swiping back from topic selection returns to the main menu");
    }

    private static void CheckGunVolumeControlLayout(VocabularyShooterGame game)
    {
        var originalWidth = Get<float>(game, "uiWidth");
        try
        {
            foreach (var width in new[] { 320f, 390f, 430f })
            {
                Set(game, "uiWidth", width);
                var pivot = Property<Vector2>(game, "GunPosition");
                var pauseArea = Property<Rect>(game, "GunPauseTouchArea");
                var down = Property<Rect>(game, "GunVolumeDownRect");
                var up = Property<Rect>(game, "GunVolumeUpRect");
                Require(down.center.x < pivot.x && up.center.x > pivot.x,
                    "Gun volume decrease and increase controls sit on the corresponding gun sides");
                Require(down.yMin > pivot.y && up.yMin > pivot.y,
                    "Gun volume controls are below the cannon pivot");
                Require(!pauseArea.Overlaps(down) && !pauseArea.Overlaps(up),
                    "Gun volume controls do not trigger the gun pause area");
                Require(down.xMin >= 18f && up.xMax <= width - 18f,
                    "Gun volume controls stay inside compact screen widths");
                Require(down.yMax < Get<float>(game, "uiHeight") - 18f
                    && up.yMax < Get<float>(game, "uiHeight") - 18f,
                    "Gun volume controls stay above the bottom screen edge");
            }
        }
        finally
        {
            Set(game, "uiWidth", originalWidth);
        }
    }

    private static void CheckCompactControls(VocabularyShooterGame game)
    {
        var originalWidth = Get<float>(game, "uiWidth");
        var originalHeight = Get<float>(game, "uiHeight");
        Call(game, "PrepareStyles");
        Require(Get<GUIStyle>(game, "musicToggleStyle").fontSize == 13
            && Get<GUIStyle>(game, "backSwipeStyle").fontSize == 10,
            "Music toggle and Back arrow use their reduced font sizes");
        var wordWidth = (float)typeof(VocabularyShooterGame)
            .GetField("WordWidth", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
        var wordHeight = (float)typeof(VocabularyShooterGame)
            .GetField("WordHeight", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
        Require(Mathf.Abs(wordWidth - 91.2f) < 0.001f && wordHeight == 42f,
            "Word cards are twenty percent larger than their current dimensions");
        try
        {
            foreach (var width in new[] { 320f, 390f, 430f })
            {
                Set(game, "uiWidth", width);
                Set(game, "uiHeight", 700f);
                var pauseListen = Property<Rect>(game, "PausePronunciationRect");
                var volumeDown = Property<Rect>(game, "GunVolumeDownRect");
                var volumeUp = Property<Rect>(game, "GunVolumeUpRect");
                var music = Property<Rect>(game, "GunMusicToggleRect");
                var topic = Property<Rect>(game, "TopicHudRect");
                var score = Property<Rect>(game, "ScoreHudRect");
                Require(pauseListen.xMax < volumeDown.xMin && music.xMin > volumeUp.xMax,
                    "Pause pronunciation and music controls do not overlap gun volume controls");
                Require(topic.xMin == 60f && topic.y == score.y && topic.xMax < score.xMin
                    && Mathf.Abs(score.center.x - width * 0.5f) < 0.01f,
                    "Topic sits at the upper left and aligns with the centered score");
            }
        }
        finally
        {
            Set(game, "uiWidth", originalWidth);
            Set(game, "uiHeight", originalHeight);
        }
    }

    private static void CheckSingleGunOrigin(VocabularyShooterGame game)
    {
        var pivot = Property<Vector2>(game, "GunPosition");
        Require(pivot == new Vector2(195f, 635f), "Only firing pivot is centered near the bottom");
        foreach (var target in new[] { new Vector2(28f, 300f), new Vector2(195f, 300f), new Vector2(362f, 300f) })
        {
            Call(game, "SetAimTarget", target, true);
            Set(game, "shotCooldown", 0f);
            Call(game, "TryFire");
            var shots = Get<IList>(game, "projectiles");
            var shot = shots[shots.Count - 1];
            Require(Get<Vector2>(shot, "position") == Property<Vector2>(game, "MuzzlePosition"),
                "Every shot starts at the single cannon muzzle");
            Require(Mathf.Abs(Vector2.Distance(Get<Vector2>(shot, "position"), pivot) - 44f) < 0.001f,
                "Turning keeps shot origin attached to the same cannon");
            Require(Property<Vector2>(game, "GunPosition") == pivot, "Aiming never creates or moves another gun");
        }
    }

    private static void CheckWordHealth(VocabularyShooterGame game)
    {
        var entries = Property<Array>(game, "CurrentWordPool");
        foreach (var entry in entries)
        {
            CheckEntryHealth(game, entry);
        }
    }

    private static void CheckEntryHealth(VocabularyShooterGame game, object entry)
    {
        Call(game, "StartRound", "A1");
        var word = Spawn(game, 132f, 300f);
        var length = Get<string>(entry, "word").Length;
        Set(word, "entry", entry);
        Set(word, "hitsRemaining", length);
        var shotCount = Mathf.CeilToInt(length / 0.735f);
        for (var shot = 1; shot <= shotCount; shot++)
        {
            Set(game, "shotCooldown", 0f);
            Call(game, "TryFire");
            Call(game, "MoveProjectiles", 0.5f);
            var expectedHealth = Mathf.Max(0, length - Mathf.FloorToInt(shot * 0.735f + 0.0001f));
            var destroyed = expectedHealth == 0;
            Require(Get<int>(word, "hitsRemaining") == expectedHealth,
                "Reduced fractional projectile damage accumulates toward word health");
            Require(Get<IList>(game, "projectiles").Count == 0, "Hit consumes its projectile");
            Require(Get<int>(game, "score") == (destroyed ? 10 : 0),
                "Points only on final hit for every word");
            Require(Get<IList>(game, "explosions").Count == (destroyed ? 1 : 0),
                "Explosion only on final hit");
            Require(ReferenceEquals(Get<object>(game, "lastHitWord"), entry),
                "Every hit reveals the word's meaning and example");
        }
        Require(Get<IList>(game, "fallingWords").Count == 0, "Destroyed target removed");
        Require(ReferenceEquals(Get<object>(game, "lastHitWord"), entry), "Destroyed word retains its own meaning");
        Require(Get<int>(game, "destroyedCount") == 1, "Destroy count increments once");
    }

    private static void CheckMeaningOnHit(VocabularyShooterGame game)
    {
        Require(Get<object>(game, "lastHitWord") == null, "New round starts without a revealed meaning");
        var first = Spawn(game, 132f, 300f);
        Set(first, "hitsRemaining", 5);
        Call(game, "TryFire");
        Require(Get<object>(game, "lastHitWord") == null, "Firing alone does not reveal a meaning");
        Call(game, "MoveProjectiles", 0.5f);
        var firstEntry = Get<object>(first, "entry");
        Require(ReferenceEquals(Get<object>(game, "lastHitWord"), firstEntry), "First nonlethal impact reveals meaning");
        Require(Get<int>(first, "hitsRemaining") == 5 && Mathf.Abs(Get<float>(first, "damageCarry") - 0.735f) < 0.001f
            && Get<IList>(game, "fallingWords").Contains(first),
            "Meaning appears immediately while reduced damage accumulates on a live target");
        Require(Get<int>(game, "score") == 0 && Get<int>(game, "destroyedCount") == 0
            && Get<IList>(game, "explosions").Count == 0, "Revealing meaning gives no premature rewards or explosion");
        Set(first, "x", 18f);
        Set(first, "previousX", 18f);
        Set(game, "shotCooldown", 0f);
        Call(game, "TryFire");
        Call(game, "MoveProjectiles", 1f);
        Require(ReferenceEquals(Get<object>(game, "lastHitWord"), firstEntry), "Missed shot keeps the displayed meaning");
        var second = Spawn(game, 132f, 300f);
        Set(second, "hitsRemaining", 5);
        Require(!ReferenceEquals(Get<object>(second, "entry"), firstEntry), "Meaning-switch check uses distinct vocabulary");
        Set(game, "shotCooldown", 0f);
        Call(game, "TryFire");
        Call(game, "MoveProjectiles", 0.5f);
        Require(ReferenceEquals(Get<object>(game, "lastHitWord"), Get<object>(second, "entry")),
            "Hitting another word updates meaning and example before destruction");
        Call(game, "StartRound", "A1");
        Require(Get<object>(game, "lastHitWord") == null, "Replay clears meaning even without a destroyed word");
    }

    private static void CheckPronunciation(VocabularyShooterGame game)
    {
        var clips = Get<Dictionary<string, AudioClip>>(game, "pronunciations");
        Require(clips.Count == 50, "The selected topic loads only its offline pronunciations");
        foreach (var clip in clips.Values)
            Require(clip != null && clip.samples > 0 && clip.length > 0f, "Pronunciation audio is nonempty");
        var gun = Get<AudioSource>(game, "gunAudioSource");
        var music = Get<AudioSource>(game, "backgroundMusicSource");
        var voice = Get<AudioSource>(game, "pronunciationSource");
        var effects = Get<AudioSource>(game, "audioSource");
        var originalMusicEnabled = Get<bool>(game, "backgroundMusicEnabled");
        Call(game, "SetBackgroundMusicEnabled", true);
        Require(gun != voice && gun != effects && voice != effects && music != gun && music != voice && music != effects,
            "Gun, music, effects and speech use separate sources");
        Require(music.loop && music.clip != null,
            "Original background music is assigned and configured to loop");
        Require(voice.bypassEffects && voice.GetComponent<AudioEchoFilter>() == null,
            "Pronunciation is not distorted by gun filters");
        var baseline = gun.volume;
        var word = Spawn(game, 132f, 300f);
        Set(word, "maxHits", 4);
        Set(word, "hitsRemaining", 4);
        var wordClip = clips[Get<string>(Get<object>(word, "entry"), "word")];
        Call(game, "ApplyWordDamage", word, 0.5f);
        Require(voice.clip == wordClip && Get<Queue<AudioClip>>(game, "pronunciationQueue").Count == 0,
            "First contact immediately speaks the word once");
        Require(Mathf.Abs(gun.volume - baseline * 0.1f) < 0.0001f,
            "Gun volume drops while gameplay pronunciation is playing");
        var queue = Get<Queue<AudioClip>>(game, "pronunciationQueue");
        for (var hit = 1; hit < 4; hit++) Call(game, "ApplyWordDamage", word, 0.5f);
        Require(Get<int>(word, "hitsRemaining") == 2 && queue.Count == 1,
            "Crossing half health queues the second pronunciation");
        for (var hit = 0; hit < 4; hit++) Call(game, "ApplyWordDamage", word, 0.5f);
        Require(!Get<IList>(game, "fallingWords").Contains(word)
            && Get<IList>(game, "explosions").Count == 1 && queue.Count == 2,
            "Destroying the word queues exactly its third pronunciation");
        Require(Mathf.Abs(music.volume - 0.048f) < 0.0001f, "Background music ducks while a word is pronounced");
        Require(voice.volume == 1f && effects.volume == 0.7f, "Ducking affects only the gun");
        Call(game, "TryFire");
        Require(Mathf.Abs(gun.volume - baseline * 0.1f) < 0.0001f, "Firing during pronunciation preserves ducked volume");
        voice.Stop();
        Call(game, "UpdatePronunciation");
        Require(voice.clip == wordClip && queue.Count == 1, "Half-health pronunciation follows the first in FIFO order");
        Require(Mathf.Abs(gun.volume - baseline * 0.1f) < 0.0001f, "Gun stays ducked between queued words");
        voice.Stop();
        Call(game, "UpdatePronunciation");
        Require(voice.clip == wordClip && queue.Count == 0, "Explosion pronunciation follows the half-health pronunciation");
        voice.Stop();
        Call(game, "UpdatePronunciation");
        Require(voice.clip == null && gun.volume == baseline, "Gun returns to full original volume when speech finishes");

        var originalGunVolume = Get<float>(game, "gunVolume");
        Call(game, "AdjustGunVolume", -1f);
        Require(Get<float>(game, "gunVolume") == 0f && gun.volume == 0f,
            "Gun volume controls can mute firing");
        Call(game, "AdjustGunVolume", 1f);
        Require(Get<float>(game, "gunVolume") == 1f && gun.volume == 1f,
            "Gun volume controls can raise firing to full volume");
        Set(game, "gunVolume", originalGunVolume);
        PlayerPrefs.SetFloat("WordGalaxy.GunVolume", originalGunVolume);
        PlayerPrefs.Save();
        Call(game, "ApplyAudioLevels");
        Call(game, "SetBackgroundMusicEnabled", false);
        Require(!Get<bool>(game, "backgroundMusicEnabled") && music.volume == 0f
            && PlayerPrefs.GetInt("WordGalaxy.BackgroundMusicEnabled") == 0,
            "Background music can be muted and the choice persists");
        Call(game, "SetBackgroundMusicEnabled", true);
        Require(Get<bool>(game, "backgroundMusicEnabled")
            && Mathf.Abs(music.volume - 0.16f) < 0.0001f,
            "Background music can be restored at its configured volume");
        Call(game, "SetBackgroundMusicEnabled", originalMusicEnabled);
        Call(game, "AdjustGunVolume", 0.1f);
        Require(Mathf.Abs(Get<float>(game, "gunVolume") - Mathf.Min(1f, originalGunVolume + 0.1f)) < 0.0001f
            && Mathf.Abs(PlayerPrefs.GetFloat("WordGalaxy.GunVolume") - Get<float>(game, "gunVolume")) < 0.0001f,
            "Gun volume controls persist their 10% adjustment");
        Set(game, "gunVolume", originalGunVolume);
        PlayerPrefs.SetFloat("WordGalaxy.GunVolume", originalGunVolume);
        PlayerPrefs.Save();
        Call(game, "ApplyAudioLevels");
    }

    private static void CheckPronunciationReset(VocabularyShooterGame game)
    {
        var originalGunVolume = Get<float>(game, "gunVolume");
        var word = Spawn(game, 132f, 300f);
        Set(word, "hitsRemaining", 1);
        Call(game, "OnWordHit", word, 1);
        var voice = Get<AudioSource>(game, "pronunciationSource");
        var gun = Get<AudioSource>(game, "gunAudioSource");
        var music = Get<AudioSource>(game, "backgroundMusicSource");
        var next = Spawn(game, 132f, 300f);
        Set(next, "hitsRemaining", 1);
        Call(game, "OnWordHit", next, 1);
        Call(game, "EndRound");
        Require(voice.clip != null && Get<Queue<AudioClip>>(game, "pronunciationQueue").Count == 5,
            "Each word keeps all three pronunciation stages queued on the result screen");
        Call(game, "StartRound", "A1");
        Require(voice.clip == null && Get<Queue<AudioClip>>(game, "pronunciationQueue").Count == 0
            && gun.volume == originalGunVolume && Mathf.Abs(music.volume - 0.16f) < 0.0001f,
            "Replay clears speech and restores gun and music volumes");
        word = Spawn(game, 132f, 300f);
        Set(word, "hitsRemaining", 1);
        Call(game, "OnWordHit", word, 1);
        Call(game, "OnDisable");
        Require(voice.clip == null && gun.volume == originalGunVolume
            && Mathf.Abs(music.volume - 0.16f) < 0.0001f,
            "Disabling game stops speech and restores gun and music volumes");
    }

    private static void CheckContinuousHits(VocabularyShooterGame game)
    {
        var farther = Spawn(game, 132f, 230f);
        var nearer = Spawn(game, 132f, 390f);
        Set(nearer, "hitsRemaining", 1);
        Set(farther, "hitsRemaining", 2);
        Set(nearer, "damageCarry", 0.65f);
        Set(farther, "damageCarry", 0.65f);
        Call(game, "CaptureTouch", 7, new Vector2(195f, 300f));
        Call(game, "TryFire");
        Set(game, "shotCooldown", 0f);
        Call(game, "TryFire");
        Call(game, "MoveProjectiles", 0.5f);
        Require(Get<IList>(game, "fallingWords").Count == 1, "Explosion preserves other targets");
        Require(Get<int>(farther, "hitsRemaining") == 1, "Remaining projectiles keep processing after explosion");
        Require(Get<int>(game, "aimFingerId") == 7, "Explosion does not release aim input");
        Set(game, "shotCooldown", 0f);
        Call(game, "TryFire");
        Require(Get<IList>(game, "projectiles").Count == 1, "Can shoot while meaning is displayed");
        var previousY = Get<float>(farther, "y");
        Call(game, "MoveWords", 0.1f);
        Require(Get<float>(farther, "y") > previousY, "Other words keep falling during explosion");
    }

    private static void CheckMissedWords(VocabularyShooterGame game)
    {
        Spawn(game, 132f, 505f);
        Spawn(game, 132f, 510f);
        Spawn(game, 132f, 515f);
        Spawn(game, 132f, 520f);
        Call(game, "TryFire");
        Call(game, "RemoveMissedWords");
        Require(Get<IList>(game, "fallingWords").Count == 0, "All words crossing line are removed");
        Require(Get<object>(game, "screenState").ToString() == "Playing", "More than three misses never ends round");
        Require(Get<int>(game, "score") == 0 && Get<int>(game, "destroyedCount") == 0, "Misses cost no score and do not count as destruction");
        Require(((ICollection)Get<object>(game, "wordBag")).Count == 50, "Missed targets are all queued for another attempt");
        Require(Get<IList>(game, "projectiles").Count == 1, "Misses preserve projectiles");
    }

    private static void CheckRepeatedMeaningReview(VocabularyShooterGame game)
    {
        Call(game, "StartTopicRound", "A1", "problem_solving");
        object firstMeaning = null;
        object secondMeaning = null;
        foreach (var entry in Property<Array>(game, "CurrentWordPool"))
        {
            if (Get<string>(entry, "word") != "fix") continue;
            if (firstMeaning == null) firstMeaning = entry;
            else secondMeaning = entry;
        }

        Require(firstMeaning != null && secondMeaning != null, "Imported topic retains both meanings of a repeated term");
        var line = Property<Rect>(game, "Playfield").yMax;
        var firstTarget = Spawn(game, 100f, line);
        var secondTarget = Spawn(game, 220f, line);
        Set(firstTarget, "entry", firstMeaning);
        Set(secondTarget, "entry", secondMeaning);
        Call(game, "RemoveMissedWords");
        var missed = Get<IList>(game, "missedWords");
        Require(missed.Count == 2 && Get<string>(missed[0], "word") == Get<string>(missed[1], "word"),
            "Missed-word review keeps distinct meanings for the same term");
    }

    private static void CheckExplosionLifetime(VocabularyShooterGame game)
    {
        var word = Spawn(game, 132f, 300f);
        Set(word, "hitsRemaining", 1);
        Call(game, "OnWordHit", word, 1);
        var effects = Get<IList>(game, "explosions");
        Require(effects.Count == 1 && Get<float>(effects[0], "age") == 0f, "Explosion starts immediately");
        Require(ReferenceEquals(Get<object>(effects[0], "entry"), Get<object>(word, "entry")), "Each effect holds its own meaning");
        Call(game, "UpdateExplosions", 2.4f);
        Require(effects.Count == 1, "Meaning remains visible for reading");
        Call(game, "UpdateExplosions", 0.2f);
        Require(effects.Count == 0 && Get<object>(game, "lastHitWord") != null, "Effect expires but header retains meaning");
    }

    private static void CheckExplosionMeaningTypography(VocabularyShooterGame game)
    {
        Call(game, "PrepareStyles");
        var word = Spawn(game, 132f, 300f);
        var entry = Get<object>(word, "entry");
        var text = Get<string>(entry, "word");
        var visualScale = (float)typeof(VocabularyShooterGame)
            .GetField("WordVisualScale", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue();
        var width = (float)typeof(VocabularyShooterGame)
            .GetField("WordWidth", BindingFlags.Static | BindingFlags.NonPublic).GetRawConstantValue()
            - 8f * visualScale;
        var wordFontSize = (int)Call(game, "WordLabelFontSize", text, width);
        var meaningStyle = (GUIStyle)Call(game, "CreateExplosionMeaningStyle", entry);
        var meaning = Get<string>(entry, "meaning");
        var meaningContent = new GUIContent(meaning);
        Require(meaningStyle.fontSize == wordFontSize && meaningStyle.wordWrap
            && meaningStyle.padding.horizontal == 0 && meaningStyle.padding.vertical == 0,
            "Explosion meaning matches the word font and has no hidden layout padding");
        var textSize = (Vector2)Call(game, "ExplosionMeaningTextSize", entry);
        var maximumWidth = Get<float>(game, "uiWidth") - 24f;
        var expectedWidth = Mathf.Min(meaningStyle.CalcSize(meaningContent).x, maximumWidth);
        var expectedHeight = meaningStyle.CalcHeight(meaningContent, textSize.x) + 8f;
        Require(textSize.x < maximumWidth && Mathf.Abs(textSize.x - expectedWidth) < 0.1f,
            "A short meaning uses only its measured text width");
        Require(Mathf.Abs(textSize.y - expectedHeight) < 0.1f,
            "Meaning text height fits its wrapped content");

        var longestMeaning = string.Empty;
        foreach (var topic in VocabularyTopicData.Topics)
            foreach (var vocabularyWord in topic.Words)
                if (vocabularyWord.Meaning.Length > longestMeaning.Length)
                    longestMeaning = vocabularyWord.Meaning;
        var originalMeaning = Get<string>(entry, "meaning");
        try
        {
            Set(entry, "meaning", longestMeaning);
            var longStyle = (GUIStyle)Call(game, "CreateExplosionMeaningStyle", entry);
            var longSize = (Vector2)Call(game, "ExplosionMeaningTextSize", entry);
            var longContent = new GUIContent(longestMeaning);
            var longHeight = longStyle.CalcHeight(longContent, longSize.x);
            var longestWidth = Mathf.Min(longStyle.CalcSize(longContent).x, maximumWidth);
            Require(longSize.x <= maximumWidth + 0.1f
                && Mathf.Abs(longSize.x - longestWidth) < 0.1f
                && longSize.y >= longHeight + 8f - 0.1f,
                "The longest catalog meaning fits without a clipping frame");
        }
        finally
        {
            Set(entry, "meaning", originalMeaning);
        }
    }

    private static void CheckContinuousSpawning(VocabularyShooterGame game)
    {
        Call(game, "UpdateSpawning", 0.1f);
        var words = Get<IList>(game, "fallingWords");
        Require(words.Count == 1, "Round spawns without a question");
        Require(Get<int>(words[0], "hitsRemaining") == Get<string>(Get<object>(words[0], "entry"), "word").Length,
            "Spawned word health equals actual letter count");
        var swayFrequency = Get<float>(words[0], "swayFrequency");
        Require(swayFrequency >= 1.08f && swayFrequency <= 1.8f,
            "Each word receives a randomized sway frequency at forty percent of the previous rate");
        Call(game, "UpdateSpawning", 5.2f);
        Require(words.Count == 2, "Spawning continues without resolving previous word");
        Spawn(game, 18f, 250f);
        Spawn(game, 18f, 250f);
        Call(game, "UpdateSpawning", 2f);
        Require(words.Count == 4, "Active targets remain capped at four");
    }

    private static void CheckNearestTarget(VocabularyShooterGame game)
    {
        Spawn(game, 132f, 230f);
        var nearer = Spawn(game, 132f, 390f);
        var hit = Call(game, "FindHitWord", new Vector2(195f, 576f), new Vector2(195f, 180f));
        Require(ReferenceEquals(hit, nearer), "Nearest target absorbs shot regardless of list order");
    }

    private static void CheckMovingTarget(VocabularyShooterGame game)
    {
        var moving = Spawn(game, 132f, 400f);
        Set(moving, "x", 200f);
        Set(moving, "previousX", 100f);
        Set(moving, "previousY", 200f);
        var hit = Call(game, "FindHitWord", new Vector2(195f, 350f), new Vector2(195f, 250f));
        Require(ReferenceEquals(hit, moving), "Collision detects a target crossing horizontally and vertically between frames");
    }

    private static void CheckDeadlineCollision(VocabularyShooterGame game)
    {
        var word = Spawn(game, 132f, 500f);
        Set(word, "y", 506f);
        Set(word, "previousY", 506f);
        var late = Call(game, "FindHitWord", new Vector2(195f, 576f), new Vector2(195f, 400f));
        Require(late == null, "Cannot hit a word after it crosses the orange line");
        Set(word, "y", 500f);
        Set(word, "previousY", 480f);
        var early = Call(game, "FindHitWord", new Vector2(195f, 544f), new Vector2(195f, 400f));
        Require(ReferenceEquals(early, word), "Hit before crossing line still counts within same frame");
    }

    private static void CheckTouchControls(VocabularyShooterGame game)
    {
        var pivot = Property<Vector2>(game, "GunPosition");
        var target = new Vector2(300f, 300f);
        Call(game, "CaptureTouch", 7, target);
        Call(game, "UpdateTouch", new Touch { fingerId = 7, phase = TouchPhase.Moved }, target);
        var direction = Get<Vector2>(game, "aimDirection");
        Require(Get<int>(game, "aimFingerId") == 7, "Touch captures aim in the playfield");
        Require(Get<IList>(game, "projectiles").Count == 1, "Aiming touch fires automatically");
        Require(direction == Get<Vector2>(game, "aimDirection"), "Automatic fire preserves the touch aim");
        Require(pivot == Property<Vector2>(game, "GunPosition"), "Aiming never moves gun");
        Call(game, "UpdateTouch", new Touch { fingerId = 7, phase = TouchPhase.Canceled }, target);
        Call(game, "OnApplicationFocus", false);
        Require(Get<int>(game, "aimFingerId") == -1, "Cancel/focus loss releases touch aim");
    }

    private static void CheckKeyboardAim(VocabularyShooterGame game)
    {
        var pivot = Property<Vector2>(game, "GunPosition");
        var origin = Get<Vector2>(game, "aimTarget");
        var movements = new[] { Vector2.left, Vector2.right };
        foreach (var movement in movements)
        {
            Call(game, "SetAimTarget", origin, true);
            Call(game, "MoveKeyboardAim", movement, 0.1f);
            var target = Get<Vector2>(game, "aimTarget");
            Require(Vector2.Distance(target, origin + movement * 48f) < 0.001f, "Left/right aim speed is doubled to 480 units/sec");
            Require(Get<Vector2>(game, "aimDirection") == WordShooterBallistics.AimDirection(pivot, target),
                "Barrel follows keyboard crosshair");
        }
        Require(pivot == Property<Vector2>(game, "GunPosition"), "Keyboard aiming keeps gun fixed");
        var targetBeforeVerticalInput = Get<Vector2>(game, "aimTarget");
        Call(game, "MoveKeyboardAim", Vector2.up, 1f);
        Call(game, "MoveKeyboardAim", Vector2.down, 1f);
        Require(Get<Vector2>(game, "aimTarget") == targetBeforeVerticalInput, "Up/down input does not change aim");
        var direction = Get<Vector2>(game, "aimDirection");
        Call(game, "MoveKeyboardAim", Vector2.zero, 1f);
        Require(Get<Vector2>(game, "aimDirection") == direction, "Releasing arrows preserves aim");
        Call(game, "TryFire");
        var shot = Get<IList>(game, "projectiles")[0];
        Require(Get<Vector2>(shot, "velocity") == direction * 850f, "Space firing path uses keyboard direction");
    }

    private static void CheckKeyboardAimBounds(VocabularyShooterGame game)
    {
        var origin = Get<Vector2>(game, "aimTarget");
        Call(game, "MoveKeyboardAim", new Vector2(1f, -1f), 0.1f);
        Require(Vector2.Distance(origin + Vector2.right * 48f, Get<Vector2>(game, "aimTarget")) < 0.001f,
            "Vertical component is ignored without slowing horizontal aim");
        Call(game, "SetAimTarget", origin, true);
        Call(game, "MoveKeyboardAim", Vector2.right, 0.1f);
        var singleStep = Get<Vector2>(game, "aimTarget");
        Call(game, "SetAimTarget", origin, true);
        Call(game, "MoveKeyboardAim", Vector2.right, 0.05f);
        Call(game, "MoveKeyboardAim", Vector2.right, 0.05f);
        Require(Vector2.Distance(singleStep, Get<Vector2>(game, "aimTarget")) < 0.001f, "Aim speed is frame-rate independent");
        Call(game, "MoveKeyboardAim", new Vector2(-1f, -1f), 100f);
        var bounds = Property<Rect>(game, "Playfield");
        Require(Get<Vector2>(game, "aimTarget") == new Vector2(bounds.xMin + 10f, origin.y),
            "Crosshair stops at left edge without changing height");
        Call(game, "MoveKeyboardAim", new Vector2(1f, 1f), 100f);
        Require(Get<Vector2>(game, "aimTarget") == new Vector2(bounds.xMax - 10f, origin.y),
            "Crosshair stops at right edge without changing height");
    }

    private static void CheckAimInputSwitching(VocabularyShooterGame game)
    {
        var point = new Vector2(100f, 300f);
        Call(game, "SetAimTarget", point, false);
        Require(!Get<bool>(game, "keyboardAiming"), "Pointer aiming hides keyboard crosshair");
        Call(game, "MoveKeyboardAim", Vector2.right, 0.1f);
        Require(Get<bool>(game, "keyboardAiming"), "Arrow input shows keyboard crosshair");
        Require(Get<Vector2>(game, "aimTarget") == point + Vector2.right * 48f, "Keyboard continues from pointer target");
        Call(game, "StartRound", "A1");
        Require(Get<Vector2>(game, "aimTarget") == Property<Rect>(game, "Playfield").center,
            "Replay centers keyboard crosshair");
        Require(Get<bool>(game, "keyboardAiming"), "Keyboard aim is ready on replay");
    }

    private static void CheckRoundReset(VocabularyShooterGame game)
    {
        var word = Spawn(game, 132f, 300f);
        Set(word, "hitsRemaining", 1);
        Call(game, "OnWordHit", word, 1);
        Spawn(game, 18f, 300f);
        Call(game, "TryFire");
        Call(game, "StartRound", "A2");
        Require(Get<IList>(game, "projectiles").Count == 0 && Get<IList>(game, "fallingWords").Count == 0,
            "Replay clears targets and shots");
        Require(Get<IList>(game, "explosions").Count == 0 && Get<object>(game, "lastHitWord") == null,
            "Replay clears effects and meanings");
        Require(((ICollection)Get<object>(game, "wordBag")).Count == 50, "Replay replaces pending retries with the selected topic's full vocabulary");
        Require(Get<int>(game, "score") == 0 && Get<int>(game, "destroyedCount") == 0, "Replay resets progress");
        Require(Get<Vector2>(game, "aimDirection") == Vector2.down, "Replay resets aim upwards");
    }

    private static void CheckExitRound(VocabularyShooterGame game)
    {
        Spawn(game, 132f, 300f);
        Call(game, "TryFire");
        Set(game, "score", 120);
        Set(game, "confirmingExit", true);
        Call(game, "ExitRoundToTopicSelection");
        Require(Get<object>(game, "screenState").ToString() == "TopicSelection",
            "Exiting a round returns to topic selection");
        Require(!Get<bool>(game, "confirmingExit"), "Exiting clears the confirmation dialog");
        Require(Get<IList>(game, "projectiles").Count == 0 && Get<IList>(game, "fallingWords").Count == 0,
            "Exiting clears active shots and words");
        Require(((ICollection)Get<object>(game, "wordBag")).Count == 0,
            "Exiting clears the remaining vocabulary");
        Require(Get<int>(game, "score") == 0, "Exiting does not create a result score");
    }

    private static void CheckEndRound(VocabularyShooterGame game)
    {
        foreach (var level in new[] { "A1", "A2", "B1", "B2" })
            CheckCompleteVocabulary(game, level);
    }

    private static void CheckCompleteVocabulary(VocabularyShooterGame game, string level)
    {
        var key = "WordGalaxy.BestScore." + level + ".food";
        var hadRecord = PlayerPrefs.HasKey(key);
        var previousRecord = PlayerPrefs.GetInt(key);
        var legacyKey = "WordGalaxy.BestScore." + level;
        var hadLegacyRecord = PlayerPrefs.HasKey(legacyKey);
        var previousLegacyRecord = PlayerPrefs.GetInt(legacyKey);
        try
        {
            Call(game, "StartRound", level);
            var wordCount = Property<Array>(game, "CurrentWordPool").Length;
            var missed = Spawn(game, 132f, 510f);
            var missedEntry = Get<object>(missed, "entry");
            Call(game, "RemoveMissedWords");
            var destroyed = new HashSet<string>();
            for (var index = 0; index < wordCount - 1; index++)
            {
                var word = Spawn(game, 132f, 300f);
                Require(!ReferenceEquals(Get<object>(word, "entry"), missedEntry), "Missed word retries after pending new words");
                Require(destroyed.Add(Get<string>(Get<object>(word, "entry"), "word")), "Destroyed words never respawn");
                Call(game, "OnWordHit", word, Get<int>(word, "hitsRemaining"));
            }
            var completedBeforeFinal = wordCount - 1;
            Require(Property<string>(game, "RoundProgressText") == completedBeforeFinal + "/" + wordCount,
                "Progress counts unique destroyed vocabulary");
            for (var attempt = 0; attempt < 3; attempt++)
            {
                var retry = Spawn(game, 132f, 510f);
                Require(ReferenceEquals(Get<object>(retry, "entry"), missedEntry), "Only the unfinished word keeps returning");
                Call(game, "OnWordHit", retry, 1);
                Call(game, "RemoveMissedWords");
                Require(Get<int>(game, "destroyedCount") == completedBeforeFinal
                    && Get<int>(game, "score") == completedBeforeFinal * 10,
                    "Repeated nonlethal hits and misses do not duplicate rewards");
            }
            var final = Spawn(game, 132f, 300f);
            Require(Get<int>(final, "hitsRemaining") == Get<string>(missedEntry, "word").Length,
                "Returning targets start with full word health");
            Call(game, "UpdateSpawning", 10f);
            Require(Get<IList>(game, "fallingWords").Count == 1, "No new vocabulary appears when the initial bag is exhausted");
            Call(game, "AdvanceGameplay", 0f);
            Require(Property<bool>(game, "CanShoot"), "Round stays active until the final target is destroyed");
            Set(final, "hitsRemaining", 1);
            Set(final, "damageCarry", 0.5f);
            Call(game, "TryFire");
            Call(game, "MoveProjectiles", 0.5f);
            Call(game, "AdvanceGameplay", 0f);
            Require(Get<object>(game, "screenState").ToString() == "Result", "Destroying all vocabulary ends the round");
            Require(Property<string>(game, "RoundProgressText") == wordCount + "/" + wordCount
                && Get<int>(game, "score") == wordCount * 10,
                "Completion awards exactly fifty unique word rewards");
            Require(Get<IList>(game, "fallingWords").Count == 0, "Round end clears targets");
            Require(Get<AudioSource>(game, "pronunciationSource").clip != null, "Completion preserves pronunciation playback");
            Call(game, "TryFire");
            Require(Get<IList>(game, "projectiles").Count == 0, "Cannot shoot on result screen");
        }
        finally
        {
            if (hadRecord) PlayerPrefs.SetInt(key, previousRecord);
            else PlayerPrefs.DeleteKey(key);
            if (hadLegacyRecord) PlayerPrefs.SetInt(legacyKey, previousLegacyRecord);
            else PlayerPrefs.DeleteKey(legacyKey);
            PlayerPrefs.Save();
        }
    }

    private static void CheckNoTimeLimit(VocabularyShooterGame game)
    {
        Call(game, "AdvanceGameplay", 3600f);
        Require(Property<bool>(game, "CanShoot"), "Elapsed time alone cannot end a round");
        Require(Get<int>(game, "destroyedCount") == 0, "Elapsed time does not complete vocabulary");
    }

    private static void CheckSafeAreaCoordinates(VocabularyShooterGame game)
    {
        Call(game, "UpdateLayout");
        var safe = Screen.safeArea;
        var topLeft = (Vector2)Call(game, "ToGamePosition", new Vector2(safe.xMin, safe.yMax));
        var bottomRight = (Vector2)Call(game, "ToGamePosition", new Vector2(safe.xMax, safe.yMin));
        Require(topLeft.sqrMagnitude < 0.001f, "Safe-area top-left maps to game origin");
        Require(Vector2.Distance(bottomRight, new Vector2(Get<float>(game, "uiWidth"), Get<float>(game, "uiHeight"))) < 0.01f,
            "Input and rendering share scaled safe-area coordinates");
    }

    private static void CheckGameplayHudLayout(VocabularyShooterGame game)
    {
        var originalWidth = Get<float>(game, "uiWidth");
        try
        {
            foreach (var width in new[] { 320f, 390f, 430f })
            {
                Set(game, "uiWidth", width);
                var previous = new Rect(18f, 16f, 36f, 36f);
                var score = Property<Rect>(game, "ScoreHudRect");
                Require(Mathf.Abs(score.center.x - width * 0.5f) < 0.01f
                    && score.xMin > previous.xMax && score.yMin == 16f,
                    "Score remains centered at the top without overlapping the exit button");
                for (var index = 0; index < 3; index++)
                {
                    var rect = (Rect)Call(game, "GameplayHudLabelRect", index);
                    Require(rect.xMin >= previous.xMax + (index == 0 ? 5f : 0f)
                        && rect.xMax <= width - 18f
                        && rect.width > 0f,
                        "Gameplay HUD columns do not overlap or extend past the screen");
                    previous = rect;
                }
                Require(score.xMax < previous.xMin,
                    "Centered score stays clear of the right-side round progress label");
            }
        }
        finally
        {
            Set(game, "uiWidth", originalWidth);
        }
    }

    private static object Spawn(VocabularyShooterGame game, float x, float y)
    {
        Call(game, "SpawnWord");
        var words = Get<IList>(game, "fallingWords");
        var word = words[words.Count - 1];
        Set(word, "x", x);
        Set(word, "previousX", x);
        Set(word, "originX", x);
        Set(word, "swayAmplitude", 0f);
        Set(word, "swayPhase", 0f);
        Set(word, "y", y);
        Set(word, "previousY", y);
        Set(word, "speed", 100f);
        return word;
    }

    private static T Get<T>(object target, string name)
    {
        return (T)target.GetType().GetField(name, Members).GetValue(target);
    }

    private static void Set(object target, string name, object value)
    {
        target.GetType().GetField(name, Members).SetValue(target, value);
    }

    private static T Property<T>(object target, string name)
    {
        return (T)target.GetType().GetProperty(name, Members).GetValue(target);
    }

    private static object Call(object target, string name, params object[] args)
    {
        return target.GetType().GetMethod(name, Members).Invoke(target, args);
    }

    private static object CallStatic(string name, params object[] args)
    {
        return typeof(VocabularyShooterGame)
            .GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)
            .Invoke(null, args);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("WordShooter check failed: " + message);
        passed++;
    }
}
