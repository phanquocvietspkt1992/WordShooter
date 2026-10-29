using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class VocabularyShooterGame : MonoBehaviour
{
    private static VocabularyShooterGame activeGame;

    private enum ScreenState
    {
        Menu,
        TopicSelection,
        Playing,
        Result
    }

    [Serializable]
    private sealed class WordEntry
    {
        public string word;
        public string meaning;
        public string example;
        public string[] examples;
        public string partOfSpeech;
        public string level;
        public string topic;

        public WordEntry(string word, string meaning, string example, string level = "A1", string topic = "food",
            string partOfSpeech = null, string[] additionalExamples = null)
        {
            this.word = word;
            this.meaning = meaning;
            this.example = example;
            examples = new string[1 + (additionalExamples == null ? 0 : additionalExamples.Length)];
            examples[0] = example;
            if (additionalExamples != null)
                Array.Copy(additionalExamples, 0, examples, 1, additionalExamples.Length);
            this.level = level;
            this.topic = topic;
            this.partOfSpeech = partOfSpeech ?? VocabularyTopicData.InferPartOfSpeech(word, example);
        }
    }

    private static bool IsBackSwipeOrigin(Vector2 point)
    {
        return point.x <= 44f;
    }

    private static bool IsBackSwipe(Vector2 start, Vector2 current)
    {
        return current.x - start.x >= 80f
            && Mathf.Abs(current.y - start.y) <= 90f;
    }

    private sealed class FallingWord
    {
        public WordEntry entry;
        public float x;
        public float previousX;
        public float originX;
        public float swayAmplitude;
        public float swayFrequency;
        public float swayPhase;
        public float y;
        public float previousY;
        public float speed;
        public int maxHits;
        public int hitsRemaining;
        public float damageCarry;
        public float hitFlash;
        public float age;
        public bool firstPronunciationQueued;
        public bool halfHealthPronunciationQueued;
    }

    private sealed class Projectile
    {
        public Vector2 position;
        public Vector2 velocity;
        public float damage;
        public int level;
    }

    private sealed class WordExplosion
    {
        public WordEntry entry;
        public Vector2 position;
        public float age;
    }

    private sealed class HitSpark
    {
        public Vector2 position;
        public float age;
    }

    private sealed class ShotPuff
    {
        public Vector2 position;
        public Vector2 direction;
        public float age;
    }

    private const float WordVisualScale = 1.2f;
    private const float WordWidth = 76f * WordVisualScale;
    private const float WordHeight = 35f * WordVisualScale;
    private const float BackSwipeZoneWidth = 44f;
    private const float BackSwipeZoneHeightRatio = 0.8f;
    private const float ProjectileRadius = 5f;
    private const float ShotRateMultiplier = 1.35f;
    private const float ShotInterval = 0.22f / 0.7f / 0.4f / 1.2f / ShotRateMultiplier;
    private const float ProjectileSpeed = 850f;
    private const float BarrelLength = 44f;
    private const float KeyboardAimSpeed = 480f;
    private const float ExplosionDuration = 2.5f;
    private const float WordSwayFrequency = 1.8f;
    private const float WordSwaySpeedMultiplier = 0.4f;
    private const float PlasmaLevelSpeedStep = 0.08f;
    private static readonly int[] PlasmaThresholds = { 0, 100, 220, 360, 450 };
    private static readonly string[] PlasmaNames = { "I", "II", "III", "IV", "V" };

    private static readonly WordEntry[] FoodWords =
    {
        new WordEntry("apple", "quả táo", "I eat an apple every morning."),
        new WordEntry("bread", "bánh mì", "We bought fresh bread."),
        new WordEntry("cheese", "phô mai", "This cheese is delicious."),
        new WordEntry("chicken", "thịt gà", "She cooked chicken for dinner."),
        new WordEntry("grapes", "quả nho", "The grapes are very sweet."),
        new WordEntry("noodles", "mì", "He likes noodles for lunch."),
        new WordEntry("orange", "quả cam", "Would you like an orange?"),
        new WordEntry("rice", "cơm, gạo", "We eat rice with vegetables."),
        new WordEntry("soup", "món súp", "The soup is hot."),
        new WordEntry("water", "nước", "Drink a glass of water."),
        new WordEntry("banana", "quả chuối", "Peel the banana before you eat it."),
        new WordEntry("mango", "quả xoài", "This mango is ripe and sweet."),
        new WordEntry("pear", "quả lê", "She sliced a pear for her snack."),
        new WordEntry("peach", "quả đào", "The peach has soft skin."),
        new WordEntry("plum", "quả mận", "There is a plum in my lunch box."),
        new WordEntry("cherry", "quả anh đào", "A cherry sits on top of the cake."),
        new WordEntry("lemon", "quả chanh vàng", "Add a slice of lemon to the tea."),
        new WordEntry("lime", "quả chanh xanh", "Squeeze a lime over the salad."),
        new WordEntry("melon", "quả dưa", "We shared a melon after lunch."),
        new WordEntry("papaya", "quả đu đủ", "The papaya has orange flesh."),
        new WordEntry("coconut", "quả dừa", "We drank water from a coconut."),
        new WordEntry("kiwi", "quả kiwi", "The kiwi is green inside."),
        new WordEntry("pineapple", "quả dứa", "Cut the pineapple into small pieces."),
        new WordEntry("strawberry", "quả dâu tây", "I put a strawberry on my yogurt."),
        new WordEntry("blueberry", "quả việt quất", "A blueberry fell from my spoon."),
        new WordEntry("avocado", "quả bơ", "Spread avocado on your toast."),
        new WordEntry("tomato", "quả cà chua", "Slice a tomato for the sandwich."),
        new WordEntry("potato", "củ khoai tây", "Bake the potato until it is soft."),
        new WordEntry("carrot", "củ cà rốt", "The rabbit is eating a carrot."),
        new WordEntry("onion", "củ hành tây", "Chop the onion for the soup."),
        new WordEntry("garlic", "tỏi", "Garlic adds flavor to this dish.", "A2"),
        new WordEntry("ginger", "gừng", "Ginger tea keeps me warm.", "A2"),
        new WordEntry("cabbage", "bắp cải", "We need cabbage for the salad.", "A2"),
        new WordEntry("lettuce", "rau xà lách", "Wash the lettuce before serving.", "A2"),
        new WordEntry("spinach", "rau cải bó xôi", "She added spinach to the soup.", "A2"),
        new WordEntry("broccoli", "bông cải xanh", "Steam the broccoli for dinner.", "A2"),
        new WordEntry("cucumber", "quả dưa chuột", "This cucumber tastes fresh.", "A2"),
        new WordEntry("pumpkin", "quả bí đỏ", "We made pumpkin soup.", "A2"),
        new WordEntry("corn", "ngô, bắp", "We grilled corn at the picnic.", "A2"),
        new WordEntry("peas", "đậu Hà Lan", "Add some peas to the rice.", "A2"),
        new WordEntry("beans", "đậu, đỗ", "These beans are full of protein.", "A2"),
        new WordEntry("mushroom", "nấm", "There is a mushroom in the soup.", "A2"),
        new WordEntry("beef", "thịt bò", "He ordered beef with rice.", "A2"),
        new WordEntry("pork", "thịt lợn, thịt heo", "We cooked pork for lunch.", "A2"),
        new WordEntry("fish", "cá", "The fish is fresh today.", "A2"),
        new WordEntry("shrimp", "tôm", "She likes shrimp with noodles.", "A2"),
        new WordEntry("crab", "cua", "We bought a crab at the market.", "A2"),
        new WordEntry("squid", "mực", "The grilled squid tastes good.", "A2"),
        new WordEntry("egg", "trứng", "Boil an egg for breakfast.", "A2"),
        new WordEntry("milk", "sữa", "Pour some milk into the glass.", "A2"),
        new WordEntry("butter", "bơ sữa", "Melt the butter in the pan.", "A2"),
        new WordEntry("yogurt", "sữa chua", "I eat yogurt with fruit.", "A2"),
        new WordEntry("cake", "bánh ngọt", "We baked a cake for her birthday.", "A2"),
        new WordEntry("cookie", "bánh quy", "Would you like a cookie?", "A2"),
        new WordEntry("pizza", "bánh pizza", "They shared a pizza for dinner.", "A2"),
        new WordEntry("sandwich", "bánh mì kẹp", "I made a sandwich for lunch.", "A2"),
        new WordEntry("pasta", "mì Ý", "Serve the pasta with tomato sauce.", "A2"),
        new WordEntry("honey", "mật ong", "Add honey to your tea.", "A2"),
        new WordEntry("tea", "trà", "She drinks tea every afternoon.", "A2"),
        new WordEntry("coffee", "cà phê", "He likes coffee with milk.", "A2")
    };

    private static readonly WordEntry[] B1Words =
    {
        new WordEntry("ingredient", "nguyên liệu", "Fresh ingredients make a better meal.", "B1"),
        new WordEntry("recipe", "công thức nấu ăn", "This recipe serves four people.", "B1"),
        new WordEntry("dessert", "món tráng miệng", "Fruit is a light dessert.", "B1"),
        new WordEntry("vinegar", "giấm", "Add a little vinegar to the dressing.", "B1"),
        new WordEntry("seasoning", "gia vị nêm", "Taste the soup before adding seasoning.", "B1"),
        new WordEntry("portion", "khẩu phần", "The restaurant serves a large portion.", "B1"),
        new WordEntry("nutritious", "bổ dưỡng", "Beans are a nutritious source of protein.", "B1"),
        new WordEntry("leftovers", "thức ăn thừa", "We used the leftovers for lunch.", "B1"),
        new WordEntry("groceries", "thực phẩm đi chợ", "I bought groceries for the week.", "B1"),
        new WordEntry("supermarket", "siêu thị", "The supermarket sells fresh fish.", "B1"),
        new WordEntry("bakery", "tiệm bánh mì", "The bakery opens early each morning.", "B1"),
        new WordEntry("seafood", "hải sản", "This market has fresh seafood.", "B1"),
        new WordEntry("poultry", "thịt gia cầm", "Poultry should be cooked thoroughly.", "B1"),
        new WordEntry("sausage", "xúc xích", "He grilled a sausage for dinner.", "B1"),
        new WordEntry("cereal", "ngũ cốc ăn sáng", "She eats cereal with milk.", "B1"),
        new WordEntry("flour", "bột mì", "Mix the flour with water.", "B1"),
        new WordEntry("pepper", "hạt tiêu", "Season the vegetables with pepper.", "B1"),
        new WordEntry("grilled", "được nướng", "We ordered grilled vegetables.", "B1"),
        new WordEntry("boiled", "được luộc", "Boiled potatoes go well with fish.", "B1"),
        new WordEntry("baked", "được nướng lò", "They served baked potatoes.", "B1"),
        new WordEntry("spicy", "cay", "This curry is too spicy for me.", "B1"),
        new WordEntry("sour", "chua", "The lemon tastes very sour.", "B1"),
        new WordEntry("bitter", "đắng", "Dark chocolate can taste bitter.", "B1"),
        new WordEntry("savory", "đậm đà, mặn", "I prefer a savory breakfast.", "B1"),
        new WordEntry("sweetener", "chất tạo ngọt", "Use a natural sweetener in the tea.", "B1"),
        new WordEntry("cuisine", "ẩm thực", "The city is famous for its local cuisine.", "B1"),
        new WordEntry("culinary", "thuộc về nấu ăn", "She is studying culinary arts.", "B1"),
        new WordEntry("waiter", "nhân viên phục vụ", "The waiter brought us the menu.", "B1"),
        new WordEntry("chef", "đầu bếp", "The chef prepared a special meal.", "B1"),
        new WordEntry("menu", "thực đơn", "There are several vegetarian dishes on the menu.", "B1")
    };

    private static readonly WordEntry[] B2Words =
    {
        new WordEntry("marinate", "ướp", "Marinate the chicken before grilling it.", "B2"),
        new WordEntry("simmer", "đun liu riu", "Let the sauce simmer for ten minutes.", "B2"),
        new WordEntry("saute", "xào nhanh", "Saute the onions in a little oil.", "B2"),
        new WordEntry("poach", "chần, luộc nhẹ", "Poach the eggs in gently boiling water.", "B2"),
        new WordEntry("knead", "nhào bột", "Knead the dough until it feels smooth.", "B2"),
        new WordEntry("whisk", "đánh đều", "Whisk the eggs with a fork.", "B2"),
        new WordEntry("dice", "thái hạt lựu", "Dice the carrots into small cubes.", "B2"),
        new WordEntry("grate", "bào, nạo", "Grate some cheese over the pasta.", "B2"),
        new WordEntry("drizzle", "rưới nhẹ", "Drizzle olive oil over the salad.", "B2"),
        new WordEntry("tender", "mềm, mềm nhừ", "Cook the beef until it is tender.", "B2"),
        new WordEntry("succulent", "mọng nước, mềm ngon", "The succulent roast was full of flavor.", "B2"),
        new WordEntry("aromatic", "thơm nức", "Aromatic herbs filled the kitchen.", "B2"),
        new WordEntry("bland", "nhạt nhẽo", "The soup tastes bland without seasoning.", "B2"),
        new WordEntry("tangy", "chua dịu, đậm vị", "The dressing has a tangy citrus flavor.", "B2"),
        new WordEntry("mouthwatering", "ngon đến chảy nước miếng", "The menu showed a mouthwatering pie.", "B2"),
        new WordEntry("wholesome", "lành mạnh, bổ dưỡng", "They serve wholesome home-cooked meals.", "B2"),
        new WordEntry("perishable", "dễ hỏng", "Keep perishable food in the refrigerator.", "B2"),
        new WordEntry("vegetarian", "người ăn chay", "The cafe offers a vegetarian option.", "B2"),
        new WordEntry("vegan", "thuần chay", "This vegan soup contains no dairy.", "B2"),
        new WordEntry("condiment", "gia vị ăn kèm", "Mustard is a popular sandwich condiment.", "B2"),
        new WordEntry("fermentation", "sự lên men", "Fermentation gives yogurt its distinctive taste.", "B2"),
        new WordEntry("preserve", "bảo quản", "Salt was once used to preserve food.", "B2"),
        new WordEntry("refrigeration", "việc làm lạnh", "Refrigeration keeps fresh food safe.", "B2"),
        new WordEntry("nutrition", "dinh dưỡng", "Good nutrition supports healthy growth.", "B2"),
        new WordEntry("delicacy", "món đặc sản", "The region is known for a seafood delicacy.", "B2"),
        new WordEntry("casserole", "món hầm đút lò", "We shared a vegetable casserole.", "B2"),
        new WordEntry("vinaigrette", "sốt dầu giấm", "She made a lemon vinaigrette for the salad.", "B2"),
        new WordEntry("appetizer", "món khai vị", "We ordered soup as an appetizer.", "B2"),
        new WordEntry("palatable", "ngon miệng, dễ ăn", "A little salt makes the dish more palatable.", "B2"),
        new WordEntry("saturated", "bão hòa", "Butter contains saturated fat.", "B2")
    };

    private static readonly string[] LevelIds = { "A1", "A2", "B1", "B2" };
    private static readonly string[] LevelNames = { "BEGINNER", "ELEMENTARY", "INTERMEDIATE", "UPPER-INT." };
    private static readonly string[] PlanetNames = { "EARTH", "MARS", "SATURN", "NEPTUNE" };
    private static readonly string[] PlanetEnglishNames = { "EARTH", "MARS", "SATURN", "NEPTUNE" };
    private static readonly float[] LevelSpeeds = { 23.4f, 26.4f, 29.4f, 32.4f };
    private static readonly string[] TopicIds =
    {
        "food", "home", "school", "travel", "jobs", "body", "nature",
        "tech", "sports", "work", "office", "it", "work_it", "meeting", "technical",
        "business", "finance", "hr", "customer_service", "office_workplace",
        "people_colleagues", "daily_work", "communication", "meetings",
        "discussion_opinions", "planning_priorities", "project_management",
        "tasks_deadlines", "problem_solving", "critical_thinking",
        "software_development", "programming_code_review", "database_api",
        "system_architecture", "testing_quality", "deployment_devops",
        "incidents_troubleshooting", "jobs_career", "performance_growth"
    };
    private static readonly string[] TopicShortNames =
    {
        "FOOD", "HOME", "SCHOOL", "TRAVEL", "JOBS", "BODY", "NATURE",
        "TECH", "SPORT", "WORK", "OFFICE", "IT", "WORK IT", "MEETING", "ENGINEER",
        "BIZ", "FINANCE", "HR", "SERVICE", "OFFICE", "PEOPLE", "DAILY",
        "COMMS", "MEET", "DISCUSS", "PLAN", "PROJECT", "TASKS", "SOLVE",
        "DECIDE", "SOFTWARE", "CODE", "DB/API", "SYSTEM", "TEST", "DEVOPS",
        "INCIDENT", "CAREER", "GROWTH"
    };
    private const float TopicRowHeight = 56f;
    private const float TopicRowGap = 6f;
    private const float TopicDragThreshold = 14f;
    private static readonly WordEntry[][][] TopicLevelWordPools = BuildTopicLevelWordPools();
    private static readonly WordEntry[][] LevelWordPools = BuildLevelWordPools();
    private static readonly WordEntry[] AllWords = BuildAllWords();

    private static WordEntry[] BuildAllWords()
    {
        var entries = new List<WordEntry>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var topic in VocabularyTopicData.Topics)
        {
            foreach (var word in topic.Words)
            {
                if (seen.Add(word.Text))
                    entries.Add(new WordEntry(word.Text, word.Meaning, word.Example, "A1", topic.Id,
                        word.PartOfSpeech, word.AdditionalExamples));
            }
        }
        foreach (var entry in FoodWords)
            if (seen.Add(entry.word))
                entries.Add(new WordEntry(entry.word, entry.meaning, entry.example, entry.level, entry.topic,
                    VocabularyTopicData.InferPartOfSpeech(entry.word, entry.example),
                    FindFoodAdditionalExamples(entry.word)));
        foreach (var entry in B1Words)
            if (seen.Add(entry.word))
                entries.Add(new WordEntry(entry.word, entry.meaning, entry.example, entry.level, entry.topic,
                    VocabularyTopicData.InferPartOfSpeech(entry.word, entry.example),
                    FindFoodAdditionalExamples(entry.word)));
        foreach (var entry in B2Words)
            if (seen.Add(entry.word))
                entries.Add(new WordEntry(entry.word, entry.meaning, entry.example, entry.level, entry.topic,
                    VocabularyTopicData.InferPartOfSpeech(entry.word, entry.example),
                    FindFoodAdditionalExamples(entry.word)));
        return entries.ToArray();
    }

    private static WordEntry[][][] BuildTopicLevelWordPools()
    {
        var topics = new WordEntry[LevelIds.Length][][];
        for (var levelIndex = 0; levelIndex < LevelIds.Length; levelIndex++)
        {
            topics[levelIndex] = new WordEntry[TopicIds.Length][];
            for (var topicIndex = 0; topicIndex < TopicIds.Length; topicIndex++)
            {
                var definition = VocabularyTopicData.Topics[topicIndex];
                if (definition.Id != TopicIds[topicIndex])
                    throw new InvalidOperationException("Topic catalog order does not match the game menu.");
                if (definition.Words == null || definition.Words.Length != 50)
                    throw new InvalidOperationException("Topic must contain exactly 50 words: " + definition.Id);
                var uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var uniqueCatalogEntries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var preserveRepeatedTerms = topicIndex >= 19;
                var topicWords = new List<WordEntry>(definition.Words.Length);
                if (topicIndex == 0)
                {
                    AppendLegacyFoodWords(topicWords, uniqueWords, FoodWords, LevelIds[levelIndex]);
                    AppendLegacyFoodWords(topicWords, uniqueWords, B1Words, LevelIds[levelIndex]);
                    AppendLegacyFoodWords(topicWords, uniqueWords, B2Words, LevelIds[levelIndex]);
                }
                for (var wordIndex = 0; wordIndex < definition.Words.Length; wordIndex++)
                {
                    var word = definition.Words[wordIndex];
                    if (word == null || string.IsNullOrWhiteSpace(word.Text)
                        || string.IsNullOrWhiteSpace(word.Meaning) || string.IsNullOrWhiteSpace(word.Example))
                        throw new InvalidOperationException("Incomplete vocabulary entry in topic: " + definition.Id);
                    var catalogKey = word.Text + "\n" + word.Meaning;
                    if ((!preserveRepeatedTerms && !uniqueCatalogEntries.Add(catalogKey))
                        || !IsValidVocabularyTerm(word.Text))
                        throw new InvalidOperationException("Invalid or duplicate word in topic "
                            + definition.Id + ": " + word.Text);
                    if (preserveRepeatedTerms)
                    {
                        topicWords.Add(new WordEntry(word.Text, word.Meaning, word.Example,
                            LevelIds[levelIndex], definition.Id, word.PartOfSpeech, word.AdditionalExamples));
                    }
                    else if (uniqueWords.Add(word.Text) && topicWords.Count < 50)
                    {
                        topicWords.Add(new WordEntry(word.Text, word.Meaning, word.Example,
                            LevelIds[levelIndex], definition.Id, word.PartOfSpeech, word.AdditionalExamples));
                    }
                }
                if (topicWords.Count != 50)
                    throw new InvalidOperationException("Not enough unique vocabulary for level "
                        + LevelIds[levelIndex] + " in topic " + definition.Id);
                topics[levelIndex][topicIndex] = topicWords.ToArray();
            }
        }
        return topics;
    }

    private static bool IsValidVocabularyTerm(string term)
    {
        if (term.Length < 3 || term.Length > 40 || term[0] == ' ' || term[term.Length - 1] == ' ')
        {
            return false;
        }

        foreach (var character in term)
        {
            if (!char.IsLetterOrDigit(character) && character != ' ' && character != '-' && character != '/'
                && character != '&' && character != '+' && character != '#' && character != '.' && character != '('
                && character != ')')
            {
                return false;
            }
        }

        return true;
    }

    private static void AppendLegacyFoodWords(
        List<WordEntry> destination,
        HashSet<string> seen,
        WordEntry[] source,
        string level)
    {
        foreach (var entry in source)
            if (entry.level == level && seen.Add(entry.word))
                destination.Add(new WordEntry(entry.word, entry.meaning, entry.example, level, "food",
                    VocabularyTopicData.InferPartOfSpeech(entry.word, entry.example),
                    FindFoodAdditionalExamples(entry.word)));
    }

    private static string[] FindFoodAdditionalExamples(string word)
    {
        foreach (var entry in VocabularyTopicData.GetTopic("food").Words)
            if (string.Equals(entry.Text, word, StringComparison.OrdinalIgnoreCase))
                return entry.AdditionalExamples;
        return LegacyFoodExampleData.GetAdditionalExamples(word);
    }

    private static WordEntry[][] BuildLevelWordPools()
    {
        var pools = new WordEntry[LevelIds.Length][];
        for (var levelIndex = 0; levelIndex < LevelIds.Length; levelIndex++)
            pools[levelIndex] = TopicLevelWordPools[levelIndex][0];
        return pools;
    }

    private static int LevelIndex(string level)
    {
        for (var index = 0; index < LevelIds.Length; index++)
            if (LevelIds[index] == level) return index;
        throw new ArgumentException("Unknown vocabulary level: " + level, nameof(level));
    }

    private readonly List<FallingWord> fallingWords = new List<FallingWord>();
    private readonly List<Projectile> projectiles = new List<Projectile>();
    private readonly List<WordExplosion> explosions = new List<WordExplosion>();
    private readonly List<HitSpark> hitSparks = new List<HitSpark>();
    private readonly List<ShotPuff> shotPuffs = new List<ShotPuff>();
    private readonly Queue<WordEntry> wordBag = new Queue<WordEntry>();
    private readonly List<WordEntry> missedWords = new List<WordEntry>();
    private readonly HashSet<string> missedWordSet = new HashSet<string>();
    private readonly Dictionary<string, AudioClip> pronunciations = new Dictionary<string, AudioClip>();
    private readonly Queue<AudioClip> pronunciationQueue = new Queue<AudioClip>();

    private ScreenState screenState;
    private string currentLevel = "A1";
    private string currentTopic = "food";
    private int selectedLevelIndex;
    private Vector2 topicScrollPosition;
    private int topicTouchFingerId = -1;
    private Vector2 topicTouchStartPosition;
    private float topicTouchLastY;
    private bool topicTouchDragging;
    private int topicTouchStartIndex = -1;
    private int backSwipeFingerId = -1;
    private Vector2 backSwipeStartPosition;
    private int bestScore;
    private bool newRecord;
    private bool confirmingExit;
    private bool wordsPaused;
    private Vector2 reviewScrollPosition;
    private float spawnTimer;
    private int score;
    private int destroyedCount;
    private WordEntry lastHitWord;
    private Vector2 aimDirection = Vector2.down;
    private Vector2 aimTarget;
    private bool keyboardAiming;
    private float shotCooldown;
    private float roundCompletedAt;
    private int aimFingerId = -1;
    private bool mouseAiming;
    private float muzzleFlash;
    private float upgradeTimer;
    private Texture2D gunTexture;
    private Texture2D glowTexture;
    private Texture2D ringTexture;
    private Texture2D wordCardTexture;

    private Vector2 GunPosition => new Vector2(uiWidth * 0.5f, uiHeight - 65f);
    private Rect GunPauseTouchArea =>
        new Rect(GunPosition.x - 28f, GunPosition.y - 48f, 56f, 78f);
    private Rect GunVolumeDownRect =>
        new Rect(GunPosition.x - 91f, GunPosition.y + 5f, 42f, 38f);
    private Rect GunVolumeUpRect =>
        new Rect(GunPosition.x + 49f, GunPosition.y + 5f, 42f, 38f);
    private Rect GunMusicToggleRect =>
        new Rect(uiWidth - 57f, GunPosition.y + 5f, 42f, 38f);
    private Rect PausePronunciationRect =>
        new Rect(14f, GunPosition.y + 5f, 44f, 38f);
    private Rect BackSwipeTouchArea =>
        new Rect(0f, uiHeight * (1f - BackSwipeZoneHeightRatio) * 0.5f,
            BackSwipeZoneWidth, uiHeight * BackSwipeZoneHeightRatio);
    private Vector2 MuzzlePosition => GunPosition + aimDirection * BarrelLength;
    private Rect Playfield => new Rect(18f, 178f, uiWidth - 36f, uiHeight - 338f);
    private bool CanShoot => screenState == ScreenState.Playing;
    private Rect ScoreHudRect
    {
        get
        {
            var width = Mathf.Min(130f, uiWidth * 0.34f);
            return new Rect((uiWidth - width) * 0.5f, 16f, width, 36f);
        }
    }
    private Rect TopicHudRect
    {
        get
        {
            const float left = 60f;
            return new Rect(left, 16f, Mathf.Max(0f, ScoreHudRect.xMin - left - 4f), 36f);
        }
    }
    private Rect GameplayHudLabelRect(int index)
    {
        const float left = 60f;
        const float right = 18f;
        const float gap = 2f;
        if (index < 0 || index > 2)
            throw new ArgumentOutOfRangeException(nameof(index));

        var columnWidth = (uiWidth - left - right - gap * 2f) / 3f;
        return new Rect(left + index * (columnWidth + gap), 16f, columnWidth, 36f);
    }
    private int PlasmaLevel => score >= 450 ? 5 : score >= 360 ? 4 : score >= 220 ? 3 : score >= 100 ? 2 : 1;
    private float PlasmaDamageMultiplier => (1f + (PlasmaLevel - 1) * 0.2f) * 0.35f * 1.25f * 1.4f * 1.2f;
    private float PlasmaSpeedMultiplier => 1f + (PlasmaLevel - 1) * PlasmaLevelSpeedStep;
    private float UpgradeProgress => PlasmaLevel == 5 ? 1f :
        Mathf.InverseLerp(PlasmaThresholds[PlasmaLevel - 1], PlasmaThresholds[PlasmaLevel], score);

    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle smallStyle;
    private GUIStyle hudStyle;
    private GUIStyle wordStyle;
    private GUIStyle buttonStyle;
    private GUIStyle musicToggleStyle;
    private GUIStyle backSwipeStyle;
    private GUIStyle feedbackStyle;
    private GUIStyle pauseExampleStyle;
    private GUIStyle wordDetailStyle;
    private float uiScale;
    private float uiWidth;
    private float uiHeight;
    private AudioSource audioSource;
    private AudioSource gunAudioSource;
    private AudioSource backgroundMusicSource;
    private AudioSource pronunciationSource;
    private const float DefaultGunVolume = 0.7f;
    private const float GunVolumeStep = 0.1f;
    private const float BackgroundMusicVolume = 0.16f;
    private const string GunVolumeSettingKey = "WordGalaxy.GunVolume";
    private const string BackgroundMusicSettingKey = "WordGalaxy.BackgroundMusicEnabled";
    private float gunVolume = DefaultGunVolume;
    private bool backgroundMusicEnabled = true;
    private AudioClip tapSound;
    private AudioClip roundStartSound;
    private AudioClip roundEndSound;
    private AudioClip upgradeSound;
    private Rect ReviewPanelRect => new Rect(42f, uiHeight * 0.48f, uiWidth - 84f, uiHeight * 0.18f);
    private Rect TopicSelectionViewport => new Rect(20f, uiHeight * 0.19f, uiWidth - 40f, uiHeight * 0.65f);
    private int CurrentLevelIndex => LevelIndex(currentLevel);
    private WordEntry[] CurrentWordPool => TopicLevelWordPools[CurrentLevelIndex][TopicIndex(currentTopic)];
    private string RoundProgressText => destroyedCount + "/" + CurrentWordPool.Length;
    private Color CurrentPlanetColor => WordShooterArt.PlanetColor(selectedLevelIndex);

    private void Awake()
    {
        if (Application.isPlaying && !RegisterGame())
        {
            return;
        }

        Screen.orientation = ScreenOrientation.Portrait;
        Application.targetFrameRate = 60;

        if (FindObjectOfType<AudioListener>() == null)
        {
            gameObject.AddComponent<AudioListener>();
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = DefaultGunVolume;
        gunAudioSource = gameObject.AddComponent<AudioSource>();
        gunAudioSource.playOnAwake = false;
        gunAudioSource.spatialBlend = 0f;
        gunVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(GunVolumeSettingKey, DefaultGunVolume));
        gunAudioSource.volume = gunVolume;
        tapSound = LoadSound("laser1");
        backgroundMusicEnabled = PlayerPrefs.GetInt(BackgroundMusicSettingKey, 1) != 0;
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource.playOnAwake = false;
        backgroundMusicSource.spatialBlend = 0f;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.volume = BackgroundMusicVolume;
        backgroundMusicSource.clip = LoadSound("WordGalaxyArcadeTheme");
        if (Application.isPlaying && backgroundMusicEnabled && backgroundMusicSource.clip != null)
            backgroundMusicSource.Play();
        InitializePronunciation();

        var distortion = gameObject.AddComponent<AudioDistortionFilter>();
        distortion.distortionLevel = 0.08f;

        var echo = gameObject.AddComponent<AudioEchoFilter>();
        echo.delay = 58f;
        echo.decayRatio = 0.14f;
        echo.dryMix = 1f;
        echo.wetMix = 0.18f;

        roundStartSound = LoadSound("highUp");
        roundEndSound = LoadSound("lowThreeTone");
        upgradeSound = LoadSound("highUp");
        gunTexture = WordShooterArt.LoadGun();
        glowTexture = WordShooterArt.CreateRadial(false);
        ringTexture = WordShooterArt.CreateRadial(true);
        wordCardTexture = WordShooterArt.CreateWordCard();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetActiveGame()
    {
        activeGame = null;
    }

    private bool RegisterGame()
    {
        if (activeGame != null && activeGame != this)
        {
            enabled = false;
            Destroy(this);
            return false;
        }

        activeGame = this;
        return true;
    }

    private void OnDestroy()
    {
        if (activeGame == this) activeGame = null;
        ReleaseTexture(glowTexture);
        ReleaseTexture(ringTexture);
        ReleaseTexture(wordCardTexture);
    }

    private void InitializePronunciation()
    {
        var voice = new GameObject("VocabularyVoice");
        voice.transform.SetParent(transform, false);
        pronunciationSource = voice.AddComponent<AudioSource>();
        pronunciationSource.playOnAwake = false;
        pronunciationSource.spatialBlend = 0f;
        pronunciationSource.volume = 1f;
        pronunciationSource.bypassEffects = true;
    }

    private void LoadPronunciationsForCurrentPool()
    {
        foreach (var entry in CurrentWordPool)
        {
            if (pronunciations.ContainsKey(entry.word)) continue;
            var resourceName = VocabularyTopicData.PronunciationResourceName(entry.word);
            var clip = Resources.Load<AudioClip>("Audio/Pronunciation/" + resourceName);
            if (clip == null) throw new InvalidOperationException("Missing pronunciation audio: " + entry.word);
            pronunciations.Add(entry.word, clip);
        }
    }

    private void QueuePronunciation(WordEntry entry)
    {
        pronunciationQueue.Enqueue(pronunciations[entry.word]);
        ApplyAudioLevels();
        if (pronunciationSource.clip == null) UpdatePronunciation();
    }

    private void UpdatePronunciation()
    {
        if (wordsPaused) return;
        if (pronunciationSource.isPlaying) return;
        if (pronunciationQueue.Count > 0)
        {
            pronunciationSource.clip = pronunciationQueue.Dequeue();
            pronunciationSource.Play();
            ApplyAudioLevels();
            return;
        }
        pronunciationSource.clip = null;
        ApplyAudioLevels();
    }

    private void SetWordsPaused(bool paused)
    {
        if (wordsPaused == paused) return;

        wordsPaused = paused;
        if (paused)
        {
            PauseAudioSource(audioSource);
            PauseAudioSource(gunAudioSource);
            PauseAudioSource(backgroundMusicSource);
            PauseAudioSource(pronunciationSource);
        }
        else
        {
            ResumeAudioSource(audioSource);
            ResumeAudioSource(gunAudioSource);
            ResumeAudioSource(backgroundMusicSource);
            ResumeAudioSource(pronunciationSource);
            if (backgroundMusicEnabled && backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
                backgroundMusicSource.Play();
            ApplyAudioLevels();
        }
    }

    private static void PauseAudioSource(AudioSource source)
    {
        if (source != null) source.Pause();
    }

    private static void ResumeAudioSource(AudioSource source)
    {
        if (source != null) source.UnPause();
    }

    private void AdjustGunVolume(float amount)
    {
        gunVolume = Mathf.Clamp01(gunVolume + amount);
        PlayerPrefs.SetFloat(GunVolumeSettingKey, gunVolume);
        PlayerPrefs.Save();
        ApplyAudioLevels();
    }

    private void SetBackgroundMusicEnabled(bool enabled)
    {
        backgroundMusicEnabled = enabled;
        PlayerPrefs.SetInt(BackgroundMusicSettingKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
        if (backgroundMusicSource != null)
        {
            if (enabled && !wordsPaused && !backgroundMusicSource.isPlaying)
                backgroundMusicSource.Play();
            else if (!enabled)
                backgroundMusicSource.Stop();
        }
        ApplyAudioLevels();
    }

    private void ApplyAudioLevels()
    {
        var duckAudio = pronunciationQueue.Count > 0
            || pronunciationSource != null && pronunciationSource.clip != null;
        if (gunAudioSource != null)
            gunAudioSource.volume = gunVolume * (duckAudio ? 0.1f : 1f);
        if (backgroundMusicSource != null)
            backgroundMusicSource.volume = (backgroundMusicEnabled ? BackgroundMusicVolume : 0f)
                * (duckAudio ? 0.3f : 1f);
    }

    private void OnDisable()
    {
        SetWordsPaused(false);
        ResetPronunciation();
        if (backgroundMusicSource != null) backgroundMusicSource.Stop();
    }

    private void OnEnable()
    {
        if (Application.isPlaying && backgroundMusicEnabled && backgroundMusicSource != null
            && backgroundMusicSource.clip != null
            && !backgroundMusicSource.isPlaying)
            backgroundMusicSource.Play();
    }

    private void ResetPronunciation()
    {
        pronunciationQueue.Clear();
        if (pronunciationSource != null)
        {
            pronunciationSource.Stop();
            pronunciationSource.clip = null;
        }
        ApplyAudioLevels();
    }

    private static void ReleaseTexture(Texture2D texture)
    {
        if (texture == null) return;
        if (Application.isPlaying) Destroy(texture);
        else DestroyImmediate(texture);
    }

    private static AudioClip LoadSound(string clipName)
    {
        return Resources.Load<AudioClip>("Audio/" + clipName);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateGameIfNeeded()
    {
        if (FindObjectOfType<VocabularyShooterGame>() != null)
        {
            return;
        }

        var gameObject = new GameObject("VocabularyShooterGame");
        gameObject.AddComponent<VocabularyShooterGame>();
    }

    private void Update()
    {
        UpdateLayout();
        HandleBackSwipeTouches();
        if (screenState == ScreenState.TopicSelection)
        {
            HandleTopicSelectionTouches();
        }

        if (screenState == ScreenState.Playing && Input.GetKeyDown(KeyCode.Escape))
        {
            confirmingExit = !confirmingExit;
            ResetShootingInput();
        }

        UpdatePronunciation();
        if (screenState != ScreenState.Playing || confirmingExit)
        {
            return;
        }

        AdvanceGameplay(Time.deltaTime);
    }

    private void AdvanceGameplay(float deltaTime)
    {
        if (!CanShoot) return;
        if (wordsPaused)
        {
            HandlePausedInput();
            return;
        }

        shotCooldown = Mathf.Max(0f, shotCooldown - deltaTime);
        UpdateShotEffects(deltaTime);
        UpdateExplosions(deltaTime);
        HandleShootingInput(deltaTime);
        MoveProjectiles(deltaTime);
        MoveWords(deltaTime);
        RemoveMissedWords();
        UpdateSpawning(deltaTime);
        if (destroyedCount == CurrentWordPool.Length)
        {
            EndRound();
            return;
        }
    }

    private void UpdateExplosions(float deltaTime)
    {
        for (var index = explosions.Count - 1; index >= 0; index--)
        {
            explosions[index].age += deltaTime;
            if (explosions[index].age >= ExplosionDuration) explosions.RemoveAt(index);
        }
    }

    private void UpdateShotEffects(float deltaTime)
    {
        upgradeTimer = Mathf.Max(0f, upgradeTimer - deltaTime);
        muzzleFlash = Mathf.Max(0f, muzzleFlash - deltaTime);
        for (var index = hitSparks.Count - 1; index >= 0; index--)
        {
            hitSparks[index].age += deltaTime;
            if (hitSparks[index].age >= 0.25f) hitSparks.RemoveAt(index);
        }
        for (var index = shotPuffs.Count - 1; index >= 0; index--)
        {
            shotPuffs[index].age += deltaTime;
            if (shotPuffs[index].age >= 0.4f) shotPuffs.RemoveAt(index);
        }
    }

    private void MoveWords(float deltaTime)
    {
        for (var index = fallingWords.Count - 1; index >= 0; index--)
        {
            var fallingWord = fallingWords[index];
            fallingWord.previousX = fallingWord.x;
            fallingWord.previousY = fallingWord.y;
            fallingWord.y += fallingWord.speed * PlasmaSpeedMultiplier * deltaTime;
            fallingWord.age += deltaTime;
            fallingWord.x = fallingWord.originX
                + Mathf.Sin(fallingWord.age * fallingWord.swayFrequency + fallingWord.swayPhase)
                    * fallingWord.swayAmplitude;
            fallingWord.hitFlash = Mathf.Max(0f, fallingWord.hitFlash - deltaTime);
        }
    }

    private void RemoveMissedWords()
    {
        for (var index = fallingWords.Count - 1; index >= 0; index--)
        {
            var fallingWord = fallingWords[index];
            if (fallingWord.y + WordHeight >= Playfield.yMax)
            {
                var reviewKey = fallingWord.entry.word + "\n" + fallingWord.entry.meaning;
                if (missedWordSet.Add(reviewKey)) missedWords.Add(fallingWord.entry);
                fallingWords.RemoveAt(index);
                wordBag.Enqueue(fallingWord.entry);
            }
        }
    }

    private void UpdateSpawning(float deltaTime)
    {
        if (fallingWords.Count < 4 && wordBag.Count > 0)
        {
            spawnTimer -= deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnWord();
                spawnTimer = UnityEngine.Random.Range(4.2f, 5f) / PlasmaSpeedMultiplier;
            }
        }
    }

    private void HandleShootingInput(float deltaTime)
    {
        if (wordsPaused || backSwipeFingerId != -1) return;
        if (Input.touchCount > 0)
        {
            HandleTouches();
        }
        else
        {
            aimFingerId = -1;
            HandleMouse();
        }

        HandleKeyboardAim(deltaTime);
        if (Input.GetKey(KeyCode.Space))
        {
            TryFire();
        }
    }

    private void HandleKeyboardAim(float deltaTime)
    {
        var movement = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow)) movement.x--;
        if (Input.GetKey(KeyCode.RightArrow)) movement.x++;
        MoveKeyboardAim(movement, deltaTime);
    }

    private void MoveKeyboardAim(Vector2 movement, float deltaTime)
    {
        movement.y = 0f;
        if (movement == Vector2.zero)
        {
            return;
        }

        var target = aimTarget + Vector2.ClampMagnitude(movement, 1f) * KeyboardAimSpeed * deltaTime;
        var bounds = Playfield;
        target.x = Mathf.Clamp(target.x, bounds.xMin + 10f, bounds.xMax - 10f);
        target.y = Mathf.Clamp(target.y, bounds.yMin + 10f, bounds.yMax - 10f);
        SetAimTarget(target, true);
    }

    private void SetAimTarget(Vector2 target, bool useKeyboard)
    {
        aimTarget = target;
        aimDirection = WordShooterBallistics.AimDirection(GunPosition, target);
        keyboardAiming = useKeyboard;
    }

    private void HandleTouches()
    {
        mouseAiming = false;
        for (var index = 0; index < Input.touchCount; index++)
        {
            var touch = Input.GetTouch(index);
            if (touch.fingerId == backSwipeFingerId) continue;
            var point = ToGamePosition(touch.position);
            if (touch.phase == TouchPhase.Began)
            {
                if (HandlePauseToggleTouch(touch, point)) return;
                if (HandleGunControlTouch(touch, point)) return;
                CaptureTouch(touch.fingerId, point);
            }

            UpdateTouch(touch, point);
        }
    }

    private bool HandlePauseToggleTouch(Touch touch, Vector2 point)
    {
        if (touch.phase != TouchPhase.Began || !GunPauseTouchArea.Contains(point))
            return false;

        SetWordsPaused(!wordsPaused);
        ResetShootingInput();
        return true;
    }

    private bool HandleGunControlTouch(Touch touch, Vector2 point)
    {
        if (touch.phase != TouchPhase.Began) return false;
        if (GunVolumeDownRect.Contains(point)) AdjustGunVolume(-GunVolumeStep);
        else if (GunVolumeUpRect.Contains(point)) AdjustGunVolume(GunVolumeStep);
        else if (GunMusicToggleRect.Contains(point)) SetBackgroundMusicEnabled(!backgroundMusicEnabled);
        else if (wordsPaused && PausePronunciationRect.Contains(point) && lastHitWord != null)
            PlayReviewPronunciation(lastHitWord);
        else return false;
        return true;
    }

    private void HandlePausedInput()
    {
        for (var index = 0; index < Input.touchCount; index++)
        {
            var touch = Input.GetTouch(index);
            if (touch.fingerId == backSwipeFingerId) continue;
            var point = ToGamePosition(touch.position);
            if (HandlePauseToggleTouch(touch, point) || HandleGunControlTouch(touch, point)) return;
        }

        if (Input.GetMouseButtonDown(0)
            && GunPauseTouchArea.Contains(ToGamePosition(Input.mousePosition)))
        {
            SetWordsPaused(false);
            ResetShootingInput();
        }
    }

    private void CaptureTouch(int fingerId, Vector2 point)
    {
        if (Playfield.Contains(point) && aimFingerId == -1)
        {
            aimFingerId = fingerId;
        }
    }

    private void UpdateTouch(Touch touch, Vector2 point)
    {
        var released = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
        if (touch.fingerId == aimFingerId)
        {
            if (!released)
            {
                SetAimTarget(point, false);
                TryFire();
            }
            else aimFingerId = -1;
        }
    }

    private void HandleMouse()
    {
        var point = ToGamePosition(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (GunPauseTouchArea.Contains(point))
            {
                SetWordsPaused(!wordsPaused);
                ResetShootingInput();
                return;
            }
            mouseAiming = Playfield.Contains(point);
        }

        if (!Input.GetMouseButton(0))
        {
            mouseAiming = false;
        }

        if (mouseAiming)
        {
            SetAimTarget(point, false);
            TryFire();
        }
    }

    private Vector2 ToGamePosition(Vector2 screenPosition)
    {
        var safeArea = Screen.safeArea;
        var topInset = Screen.height - safeArea.yMax;
        return new Vector2(screenPosition.x - safeArea.xMin, Screen.height - screenPosition.y - topInset) / uiScale;
    }

    private void TryFire()
    {
        if (!CanShoot || wordsPaused || shotCooldown > 0f)
        {
            return;
        }

        projectiles.Add(new Projectile
        {
            position = MuzzlePosition,
            velocity = aimDirection * ProjectileSpeed,
            damage = PlasmaDamageMultiplier,
            level = PlasmaLevel
        });
        shotCooldown = ShotInterval;
        muzzleFlash = 0.09f;
        shotPuffs.Add(new ShotPuff { position = MuzzlePosition, direction = aimDirection });
        if (tapSound != null) gunAudioSource.PlayOneShot(tapSound);
    }

    private void MoveProjectiles(float deltaTime)
    {
        for (var index = projectiles.Count - 1; index >= 0; index--)
        {
            var projectile = projectiles[index];
            var end = projectile.position + projectile.velocity * deltaTime;
            var hit = FindHitWord(projectile.position, end);
            if (hit != null)
            {
                projectiles.RemoveAt(index);
                ApplyWordDamage(hit, projectile.damage);
                continue;
            }

            projectile.position = end;
            if (end.y < Playfield.yMin || end.x < 0f || end.x > uiWidth) projectiles.RemoveAt(index);
        }
    }

    private FallingWord FindHitWord(Vector2 start, Vector2 end)
    {
        FallingWord closest = null;
        var closestFraction = float.PositiveInfinity;
        foreach (var word in fallingWords)
        {
            // Sweep relative to the falling target so fast shots cannot skip over it.
            var relativeStart = start + new Vector2(word.x - word.previousX, word.y - word.previousY);
            var bounds = new Rect(word.x, word.y, WordWidth, WordHeight);
            if (WordShooterBallistics.TryHitWord(relativeStart, end, bounds, ProjectileRadius, out var fraction)
                && fraction < closestFraction
                && Mathf.Lerp(word.previousY, word.y, fraction) + WordHeight <= Playfield.yMax)
            {
                closest = word;
                closestFraction = fraction;
            }
        }

        return closest;
    }

    private void ResetShootingInput()
    {
        aimFingerId = -1;
        mouseAiming = false;
    }

    private void HandleTopicSelectionTouches()
    {
        for (var index = 0; index < Input.touchCount; index++)
        {
            var touch = Input.GetTouch(index);
            if (touch.fingerId == backSwipeFingerId) continue;
            HandleTopicSelectionTouch(touch, ToGamePosition(touch.position));
        }
    }

    private void HandleBackSwipeTouches()
    {
        for (var index = 0; index < Input.touchCount; index++)
        {
            var touch = Input.GetTouch(index);
            var point = ToGamePosition(touch.position);
            if (touch.phase == TouchPhase.Began)
            {
                if (screenState != ScreenState.Menu && BackSwipeTouchArea.Contains(point)
                    && IsBackSwipeOrigin(point))
                {
                    backSwipeFingerId = touch.fingerId;
                    backSwipeStartPosition = point;
                }
                continue;
            }

            if (touch.fingerId != backSwipeFingerId) continue;
            if (IsBackSwipe(backSwipeStartPosition, point))
            {
                NavigateBack();
                backSwipeFingerId = -1;
                return;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                backSwipeFingerId = -1;
        }
    }

    private void NavigateBack()
    {
        switch (screenState)
        {
            case ScreenState.TopicSelection:
                screenState = ScreenState.Menu;
                ResetTopicSelectionTouch();
                break;
            case ScreenState.Playing:
                confirmingExit = true;
                ResetShootingInput();
                break;
            case ScreenState.Result:
                screenState = ScreenState.TopicSelection;
                break;
        }
    }

    private void HandleTopicSelectionTouch(Touch touch, Vector2 point)
    {
        if (screenState != ScreenState.TopicSelection)
        {
            ResetTopicSelectionTouch();
            backSwipeFingerId = -1;
            return;
        }

        if (touch.phase == TouchPhase.Began)
        {
            if (topicTouchFingerId == -1 && TopicSelectionViewport.Contains(point))
            {
                topicTouchFingerId = touch.fingerId;
                topicTouchStartPosition = point;
                topicTouchLastY = point.y;
                topicTouchDragging = false;
                topicTouchStartIndex = TopicIndexAtPoint(point);
            }
            return;
        }

        if (touch.fingerId != topicTouchFingerId)
        {
            return;
        }

        var wasDragging = topicTouchDragging;
        if (!topicTouchDragging
            && Vector2.Distance(topicTouchStartPosition, point) >= TopicDragThreshold)
        {
            topicTouchDragging = true;
        }

        if (topicTouchDragging)
        {
            var scrollDelta = wasDragging
                ? topicTouchLastY - point.y
                : topicTouchStartPosition.y - point.y;
            topicScrollPosition.y += scrollDelta;
            topicScrollPosition.y = Mathf.Clamp(topicScrollPosition.y, 0f, TopicScrollMax);
        }

        topicTouchLastY = point.y;
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (touch.phase == TouchPhase.Ended
                && !topicTouchDragging
                && topicTouchStartIndex >= 0
                && TopicIndexAtPoint(point) == topicTouchStartIndex)
            {
                TryStartTopic(TopicIds[topicTouchStartIndex]);
            }
            ResetTopicSelectionTouch();
        }
    }

    private int TopicIndexAtPoint(Vector2 point)
    {
        var viewport = TopicSelectionViewport;
        if (!viewport.Contains(point)) return -1;

        var contentY = point.y - viewport.yMin + topicScrollPosition.y;
        var rowStride = TopicRowHeight + TopicRowGap;
        var index = Mathf.FloorToInt(contentY / rowStride);
        if (index < 0 || index >= TopicIds.Length || contentY - index * rowStride >= TopicRowHeight)
            return -1;
        return index;
    }

    private float TopicScrollMax =>
        Mathf.Max(0f, TopicIds.Length * (TopicRowHeight + TopicRowGap) - TopicSelectionViewport.height);

    private void ResetTopicSelectionTouch()
    {
        topicTouchFingerId = -1;
        topicTouchDragging = false;
        topicTouchStartIndex = -1;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            ResetShootingInput();
            ResetTopicSelectionTouch();
        }
    }

    private void OnGUI()
    {
        PrepareStyles();
        var previousMatrix = GUI.matrix;
        var safeArea = Screen.safeArea;
        var topInset = Screen.height - safeArea.yMax;
        GUI.matrix = Matrix4x4.TRS(
            new Vector3(safeArea.xMin, topInset, 0f),
            Quaternion.identity,
            new Vector3(uiScale, uiScale, 1f));

        DrawBackground();
        switch (screenState)
        {
            case ScreenState.Menu:
                DrawMenu();
                break;
            case ScreenState.TopicSelection:
                DrawTopicSelection();
                break;
            case ScreenState.Playing:
                DrawGameplay();
                if (confirmingExit) DrawExitConfirmation();
                break;
            case ScreenState.Result:
                DrawResult();
                break;
        }

        if (screenState != ScreenState.Menu && !confirmingExit) DrawBackSwipeHint();

        GUI.matrix = previousMatrix;
    }

    private void PrepareStyles()
    {
        UpdateLayout();

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 34,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };
        titleStyle.normal.textColor = WordShooterArt.SpaceText;

        bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };
        bodyStyle.normal.textColor = WordShooterArt.SpaceText;

        smallStyle = new GUIStyle(bodyStyle) { fontSize = 15 };
        smallStyle.normal.textColor = new Color(0.6f, 0.76f, 0.9f);

        hudStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            clipping = TextClipping.Clip,
            padding = new RectOffset(2, 2, 0, 0)
        };
        hudStyle.normal.textColor = WordShooterArt.SpaceText;

        wordStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 21,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            padding = new RectOffset(0, 0, 0, 0),
            normal = { textColor = Color.white },
            hover = { textColor = Color.white },
            active = { textColor = Color.white }
        };

        buttonStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 21,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            hover = { textColor = Color.white },
            active = { textColor = Color.white }
        };
        musicToggleStyle = new GUIStyle(buttonStyle) { fontSize = 13 };
        backSwipeStyle = new GUIStyle(bodyStyle) { fontSize = 10, alignment = TextAnchor.MiddleCenter };

        feedbackStyle = new GUIStyle(bodyStyle)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter
        };
        pauseExampleStyle = new GUIStyle(smallStyle)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 14,
            padding = new RectOffset(8, 8, 1, 1)
        };
        wordDetailStyle = new GUIStyle(smallStyle)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 11,
            wordWrap = false,
            clipping = TextClipping.Clip,
            padding = new RectOffset(0, 0, 0, 0)
        };
    }

    private void UpdateLayout()
    {
        var safeArea = Screen.safeArea;
        uiScale = Mathf.Max(0.01f, Mathf.Min(safeArea.width / 390f, safeArea.height / 700f));
        uiWidth = safeArea.width / uiScale;
        uiHeight = safeArea.height / uiScale;
    }

    private void DrawBackground()
    {
        var planet = screenState == ScreenState.Playing || screenState == ScreenState.Result
            ? WordShooterArt.PlanetColor(CurrentLevelIndex)
            : CurrentPlanetColor;
        DrawRectangle(new Rect(0f, 0f, uiWidth, uiHeight), WordShooterArt.SpaceBackground);
        var drift = new Vector2(Mathf.Sin(Time.time * 0.04f) * 50f, Mathf.Cos(Time.time * 0.03f) * 35f);
        DrawTextureTinted(new Rect(-uiWidth * 0.5f + drift.x, -100f + drift.y, uiWidth * 1.7f, uiHeight),
            glowTexture, new Color(0.4f, 0.12f, 0.85f, 0.45f));
        DrawTextureTinted(new Rect(uiWidth * 0.15f - drift.x, uiHeight * 0.15f - drift.y, uiWidth * 1.5f, uiHeight * 0.9f),
            glowTexture, new Color(planet.r * 0.2f, planet.g * 0.2f, planet.b * 0.2f, 0.5f));
        DrawSpiralGalaxy();
        DrawBackgroundDetails();
        DrawMeteors();
    }

    private void DrawSpiralGalaxy()
    {
        var center = WordShooterArt.GalaxyCenter(new Vector2(uiWidth, uiHeight), Time.time);
        var radius = Mathf.Min(uiWidth * 0.65f, 320f);
        for (var arm = 0; arm < 3; arm++)
        {
            for (var index = 1; index <= 28; index++)
            {
                var progress = index / 28f;
                var angle = arm * Mathf.PI * 2f / 3f + progress * 4.5f + Time.time * 0.035f;
                var point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle) * 0.48f) * radius * progress;
                var tint = Color.Lerp(new Color(0.55f, 0.35f, 1f), WordShooterArt.Plasma, progress);
                tint.a = (1f - progress) * 0.18f;
                DrawGlow(point, 20f + progress * 40f, tint);
            }
        }
        DrawGlow(center, 95f, new Color(0.65f, 0.6f, 1f, 0.32f));
        DrawGlow(center, 24f, new Color(0.8f, 0.9f, 1f, 0.45f));
    }

    private void DrawMeteors()
    {
        var size = new Vector2(uiWidth, uiHeight);
        var direction = new Vector2(size.x * 0.7f + 150f, size.y + 220f).normalized;
        for (var index = 0; index < 4; index++)
        {
            var head = WordShooterArt.MeteorPosition(index, size, Time.time, out var opacity);
            if (opacity <= 0f) continue;
            for (var segment = 0; segment < 12; segment++)
            {
                var fade = 1f - segment / 12f;
                var point = head - direction * segment * 9f;
                DrawStreak(point, direction, 10f, 0.5f + fade * 2f,
                    new Color(0.45f, 0.75f, 1f, opacity * fade * fade));
            }
            DrawGlow(head, 28f, new Color(0.4f, 0.8f, 1f, opacity));
            DrawRectangle(new Rect(head.x - 1.5f, head.y - 1.5f, 3f, 3f),
                new Color(0.9f, 1f, 1f, opacity));
        }
    }

    private void DrawBackgroundDetails()
    {
        for (var index = 0; index < 90; index++)
        {
            var point = WordShooterArt.StarPosition(index, new Vector2(uiWidth, uiHeight), Time.time);
            var size = 0.8f + index % 3 * 0.55f;
            var alpha = 0.35f + 0.25f * Mathf.Sin(Time.time * 0.8f + index);
            DrawRectangle(new Rect(point.x, point.y, size, size), new Color(0.75f, 0.9f, 1f, alpha));
            if (index % 9 == 0) DrawGlow(point, 15f, new Color(0.4f, 0.7f, 1f, alpha * 0.7f));
        }
        DrawTextureTinted(new Rect(uiWidth * 0.5f - 150f, uiHeight - 200f, 300f, 240f),
            glowTexture, new Color(0.05f, 0.8f, 0.7f, 0.22f));
    }

    private void DrawMenu()
    {
        GUI.Label(new Rect(28f, uiHeight * 0.08f, uiWidth - 56f, 58f), "WORD GALAXY", titleStyle);
        GUI.Label(new Rect(34f, uiHeight * 0.18f, uiWidth - 68f, 30f), "Choose a level", bodyStyle);

        var rowHeight = 58f;
        var rowGap = 12f;
        var startY = uiHeight * 0.27f;
        for (var index = 0; index < LevelIds.Length; index++)
        {
            var rect = new Rect(24f, startY + index * (rowHeight + rowGap), uiWidth - 48f, rowHeight);
            DrawLevelCard(rect, index);
        }
    }

    private void DrawLevelCard(Rect rect, int levelIndex)
    {
        var tint = WordShooterArt.PlanetColor(levelIndex);
        var label = LevelIds[levelIndex] + "  •  " + LevelNames[levelIndex];
        if (DrawButton(rect, label, tint))
        {
            selectedLevelIndex = levelIndex;
            currentLevel = LevelIds[levelIndex];
            topicScrollPosition = Vector2.zero;
            screenState = ScreenState.TopicSelection;
        }
    }

    private void DrawTopicSelection()
    {
        GUI.Label(new Rect(24f, uiHeight * 0.065f, uiWidth - 48f, 46f),
            "CHOOSE A TOPIC", titleStyle);
        GUI.Label(new Rect(28f, uiHeight * 0.13f, uiWidth - 56f, 28f),
            currentLevel + "  •  " + PlanetEnglishName(selectedLevelIndex), bodyStyle);

        var viewport = TopicSelectionViewport;
        var contentWidth = viewport.width - 14f;
        var contentHeight = TopicIds.Length * (TopicRowHeight + TopicRowGap);
        if (Application.isMobilePlatform)
        {
            GUI.BeginGroup(viewport);
            DrawTopicRows(contentWidth, false, topicScrollPosition.y);
            GUI.EndGroup();
            DrawTopicScrollIndicator(viewport, contentHeight);
        }
        else
        {
            var content = new Rect(0f, 0f, contentWidth, contentHeight);
            topicScrollPosition = GUI.BeginScrollView(viewport, topicScrollPosition, content, false, true);
            DrawTopicRows(contentWidth, true, 0f);
            GUI.EndScrollView();
        }

        if (DrawButton(new Rect(40f, uiHeight - 72f, uiWidth - 80f, 48f),
            "Back to levels", Color32(126, 87, 220)))
        {
            screenState = ScreenState.Menu;
        }
    }

    private void DrawTopicRows(float contentWidth, bool allowButtonClicks, float verticalOffset)
    {
        for (var index = 0; index < TopicIds.Length; index++)
        {
            var rect = new Rect(0f,
                index * (TopicRowHeight + TopicRowGap) - verticalOffset, contentWidth, TopicRowHeight);
            var tint = WordShooterArt.PlanetColor(selectedLevelIndex);
            var clicked = false;
            if (allowButtonClicks)
            {
                clicked = DrawButton(rect, string.Empty, tint);
            }
            else
            {
                DrawButtonBackground(rect, tint);
            }

            if (clicked && TryStartTopic(TopicIds[index]))
            {
                return;
            }

            var topicName = TopicName(index);
            var nameRect = new Rect(rect.x + 16f, rect.y + 5f, rect.width - 32f, rect.height - 10f);
            GUI.Label(nameRect, topicName, CreateTopicNameStyle(topicName, nameRect.width));
        }
    }

    private void DrawTopicScrollIndicator(Rect viewport, float contentHeight)
    {
        if (TopicScrollMax <= 0f) return;

        var track = new Rect(viewport.xMax - 5f, viewport.y + 4f, 3f, viewport.height - 8f);
        var thumbHeight = Mathf.Max(24f, track.height * viewport.height / contentHeight);
        var thumbY = track.y + (track.height - thumbHeight) * topicScrollPosition.y / TopicScrollMax;
        DrawPill(track, new Color(0.3f, 0.48f, 0.65f, 0.35f));
        DrawPill(new Rect(track.x, thumbY, track.width, thumbHeight), WordShooterArt.Plasma);
    }

    private void DrawPlanetIcon(Vector2 center, int levelIndex)
    {
        var color = WordShooterArt.PlanetColor(levelIndex);
        if (levelIndex == 2)
            DrawTextureTinted(CenteredRect(center, 44f), ringTexture, new Color(color.r, color.g, color.b, 0.85f));
        DrawGlow(center, 33f, new Color(color.r, color.g, color.b, 0.95f));
        DrawGlow(center + new Vector2(-4f, -5f), 17f, new Color(0.8f, 0.92f, 1f, 0.85f));
    }

    private void DrawGameplay()
    {
        if (DrawButton(new Rect(18f, 16f, 36f, 36f), "×", Color32(190, 62, 82)))
        {
            confirmingExit = true;
            ResetShootingInput();
        }
        DrawHudLabel(TopicHudRect, TopicShortNames[TopicIndex(currentTopic)], TextAnchor.MiddleLeft);
        DrawHudLabel(ScoreHudRect, "SCORE " + score, TextAnchor.MiddleCenter);
        DrawHudLabel(GameplayHudLabelRect(2), RoundProgressText + " WORDS", TextAnchor.MiddleRight);

        DrawHologramPanel(new Rect(20f, 65f, uiWidth - 40f, 100f));
        DrawMeaningPanel();

        DrawShootingRange();
        DrawFallingWords();
        DrawProjectiles();
        DrawShotPuffs();
        DrawGun();
        DrawExplosions();
        DrawHitSparks();
        DrawKeyboardCrosshair();
        DrawUpgradeNotification();
        if (wordsPaused) DrawPausedWordExamples();
    }

    private void DrawHudLabel(Rect rect, string text, TextAnchor alignment)
    {
        GUI.Label(rect, text, CreateFittedHudStyle(text, alignment, rect.width));
    }

    private GUIStyle CreateFittedHudStyle(string text, TextAnchor alignment, float width)
    {
        var style = new GUIStyle(hudStyle) { alignment = alignment };
        while (style.fontSize > 9 && style.CalcSize(new GUIContent(text)).x > width - 4f)
        {
            style.fontSize--;
        }
        return style;
    }

    private GUIStyle CreateFittedSmallStyle(string text, float width)
    {
        var style = new GUIStyle(smallStyle) { wordWrap = false, clipping = TextClipping.Clip };
        while (style.fontSize > 9 && style.CalcSize(new GUIContent(text)).x > width - 4f)
            style.fontSize--;
        return style;
    }

    private GUIStyle CreateTopicNameStyle(string text, float width)
    {
        var style = new GUIStyle(smallStyle)
        {
            wordWrap = false,
            clipping = TextClipping.Clip,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(0, 0, 0, 0)
        };
        while (style.fontSize > 9 && style.CalcSize(new GUIContent(text)).x > width - 4f)
            style.fontSize--;
        return style;
    }

    private void DrawBackSwipeHint()
    {
        var touchArea = BackSwipeTouchArea;
        var rect = new Rect(touchArea.x, touchArea.y, 8f, touchArea.height);
        DrawPill(rect, new Color(0.03f, 0.12f, 0.2f, 0.62f));
        DrawPill(new Rect(rect.xMax - 2f, rect.y + 8f, 1f, rect.height - 16f),
            new Color(0.35f, 0.85f, 1f, 0.72f));
        GUI.Label(new Rect(rect.xMax - 1f, rect.center.y - 12f, 20f, 24f), "→", backSwipeStyle);
    }

    private void DrawExitConfirmation()
    {
        DrawRectangle(new Rect(0f, 0f, uiWidth, uiHeight), new Color(0f, 0f, 0f, 0.72f));
        var panelWidth = Mathf.Min(uiWidth - 40f, 340f);
        var panel = new Rect((uiWidth - panelWidth) * 0.5f, uiHeight * 0.5f - 112f, panelWidth, 224f);
        DrawHologramPanel(panel);
        GUI.Label(new Rect(panel.x + 12f, panel.y + 20f, panel.width - 24f, 46f),
            "Exit this round?", bodyStyle);
        GUI.Label(new Rect(panel.x + 18f, panel.y + 70f, panel.width - 36f, 48f),
            "Progress from this round will not be saved.", smallStyle);

        var buttonWidth = panel.width - 44f;
        if (DrawButton(new Rect(panel.x + 22f, panel.y + 128f, buttonWidth, 40f),
            "Resume", Color32(42, 143, 112)))
        {
            confirmingExit = false;
        }
        if (DrawButton(new Rect(panel.x + 22f, panel.y + 176f, buttonWidth, 36f),
            "Exit to topics", Color32(126, 87, 220)))
        {
            ExitRoundToTopicSelection();
        }
    }

    private void ExitRoundToTopicSelection()
    {
        SetWordsPaused(false);
        ResetPronunciation();
        pronunciations.Clear();
        ResetShootingInput();
        confirmingExit = false;
        fallingWords.Clear();
        projectiles.Clear();
        explosions.Clear();
        hitSparks.Clear();
        shotPuffs.Clear();
        wordBag.Clear();
        lastHitWord = null;
        missedWords.Clear();
        missedWordSet.Clear();
        score = 0;
        destroyedCount = 0;
        newRecord = false;
        shotCooldown = 0f;
        muzzleFlash = 0f;
        upgradeTimer = 0f;
        screenState = ScreenState.TopicSelection;
    }

    private static Color PlasmaColor(int level)
    {
        if (level >= 5) return new Color(1f, 0.22f, 0.35f);
        if (level >= 4) return new Color(1f, 0.72f, 0.24f);
        if (level == 3) return new Color(0.65f, 0.32f, 1f);
        if (level == 2) return new Color(0.28f, 1f, 0.48f);
        return WordShooterArt.Plasma;
    }

    private static int ProjectileTrailCount(int level)
    {
        if (level <= 1) return 3;
        return 4 + Mathf.Clamp(level, 1, 5) * 2;
    }

    private static int ProjectileSparkCount(int level)
    {
        if (level <= 1) return 0;
        return Mathf.Clamp(level, 1, 5) + 1;
    }

    private static float ProjectilePowerScale(int level)
    {
        if (level <= 1) return 1f;
        return 1.15f + (Mathf.Clamp(level, 1, 5) - 1) * 0.25f;
    }

    private static int MuzzleStreakCount(int level)
    {
        if (level <= 1) return 3;
        return 2 + Mathf.Clamp(level, 1, 5) * 2;
    }

    private void DrawUpgradeNotification()
    {
        if (upgradeTimer <= 0f) return;
        var previousColor = GUI.color;
        var alpha = Mathf.Min(1f, upgradeTimer / 0.4f);
        var tint = PlasmaColor(PlasmaLevel);
        GUI.color *= new Color(1f, 1f, 1f, alpha);
        DrawGlow(new Vector2(uiWidth * 0.5f, 214f), 110f + PlasmaLevel * 20f,
            new Color(tint.r, tint.g, tint.b, alpha * 0.32f));
        DrawHologramPanel(new Rect(30f, 184f, uiWidth - 60f, 60f));
        GUI.Label(new Rect(36f, 188f, uiWidth - 72f, 50f),
            "GUN LEVEL " + PlasmaNames[PlasmaLevel - 1] + "!\nPOWER INCREASED", feedbackStyle);
        GUI.color = previousColor;
    }

    private void DrawKeyboardCrosshair()
    {
        if (!keyboardAiming)
        {
            return;
        }

        var color = WordShooterArt.Plasma;
        DrawRectangle(new Rect(aimTarget.x - 12f, aimTarget.y - 1f, 8f, 2f), color);
        DrawRectangle(new Rect(aimTarget.x + 4f, aimTarget.y - 1f, 8f, 2f), color);
        DrawRectangle(new Rect(aimTarget.x - 1f, aimTarget.y - 12f, 2f, 8f), color);
        DrawRectangle(new Rect(aimTarget.x - 1f, aimTarget.y + 4f, 2f, 8f), color);
    }

    private void DrawMeaningPanel()
    {
        var meaning = lastHitWord == null
            ? "Hit any word to see its meaning"
            : lastHitWord.word + " (" + lastHitWord.partOfSpeech + ") = " + lastHitWord.meaning;
        var textWidth = uiWidth - 64f;
        GUI.Label(new Rect(32f, 72f, textWidth, 40f), meaning, bodyStyle);
        var detail = lastHitWord == null ? "Word health equals its letter count. Score points to upgrade!" : lastHitWord.example;
        GUI.Label(new Rect(32f, 112f, textWidth, 45f), detail, smallStyle);
    }

    private void DrawPausedWordExamples()
    {
        var panelWidth = Mathf.Min(uiWidth - 40f, 360f);
        var panelHeight = 302f;
        var panel = new Rect((uiWidth - panelWidth) * 0.5f,
            Mathf.Clamp(uiHeight * 0.34f, 178f, uiHeight - panelHeight - 150f),
            panelWidth, panelHeight);
        DrawHologramPanel(panel);
        if (lastHitWord != null && DrawControlButton(PausePronunciationRect, "▶", Color32(42, 143, 112)))
            PlayReviewPronunciation(lastHitWord);

        if (lastHitWord == null)
        {
            GUI.Label(new Rect(panel.x + 12f, panel.y + 8f, panel.width - 24f, 44f),
                "WORDS PAUSED  •  TAP THE GUN TO RESUME", smallStyle);
            return;
        }

        GUI.Label(new Rect(panel.x + 14f, panel.y + 5f, panel.width - 28f, 32f),
            lastHitWord.word + "  •  EXAMPLES", smallStyle);
        var examples = lastHitWord.examples;
        var count = Mathf.Min(5, examples.Length);
        var rowHeight = 49f;
        for (var index = 0; index < count; index++)
        {
            var row = new Rect(panel.x + 10f, panel.y + 39f + index * rowHeight,
                panel.width - 20f, rowHeight - 2f);
            DrawTextureTinted(row, wordCardTexture, new Color(0.05f, 0.2f, 0.3f, 0.65f));
            var label = (index + 1) + ". " + examples[index];
            pauseExampleStyle.fontSize = 14;
            while (pauseExampleStyle.fontSize > 10
                && pauseExampleStyle.CalcHeight(new GUIContent(label), row.width - 12f) > row.height - 2f)
                pauseExampleStyle.fontSize--;
            GUI.Label(row, label, pauseExampleStyle);
        }
        GUI.Label(new Rect(panel.x + 8f, panel.yMax - 17f, panel.width - 16f, 15f),
            "TAP THE GUN TO CONTINUE", pauseExampleStyle);
    }

    private void DrawFallingWords()
    {
        for (var index = 0; index < fallingWords.Count; index++)
        {
            var fallingWord = fallingWords[index];
            DrawFallingTrail(fallingWord);
            DrawWordCard(fallingWord);
            DrawWordHealth(fallingWord);
        }
    }

    private int WordLabelFontSize(string text, float width)
    {
        var maxWordFontSize = 13f * WordVisualScale;
        return Mathf.RoundToInt(Mathf.Min(maxWordFontSize,
            maxWordFontSize * width / Mathf.Max(1f, wordStyle.CalcSize(new GUIContent(text)).x)));
    }

    private void DrawWordCard(FallingWord word)
    {
        var rect = new Rect(word.x, word.y, WordWidth, WordHeight);
        var wordTint = WordTint(word.entry);
        var tint = Color.Lerp(WordTint(word.entry) * 0.28f, new Color(0.15f, 0.65f, 0.8f), word.hitFlash / 0.12f);
        var opacity = Mathf.SmoothStep(0f, 1f, word.age / 0.22f);
        var wingColor = new Color(wordTint.r * 0.55f, wordTint.g * 0.55f, wordTint.b * 0.55f, opacity);
        DrawPill(new Rect(rect.x - 4f * WordVisualScale, rect.y + 14f * WordVisualScale,
            9f * WordVisualScale, 6f * WordVisualScale), wingColor);
        DrawPill(new Rect(rect.xMax - 5f * WordVisualScale, rect.y + 14f * WordVisualScale,
            9f * WordVisualScale, 6f * WordVisualScale), wingColor);
        DrawPill(new Rect(rect.x - 3f * WordVisualScale, rect.y + 16f * WordVisualScale,
            6f * WordVisualScale, WordVisualScale), new Color(wordTint.r, wordTint.g, wordTint.b, opacity));
        DrawPill(new Rect(rect.xMax - 3f * WordVisualScale, rect.y + 16f * WordVisualScale,
            6f * WordVisualScale, WordVisualScale), new Color(wordTint.r, wordTint.g, wordTint.b, opacity));
        DrawTextureTinted(new Rect(rect.x, rect.y + 2f * WordVisualScale, rect.width, rect.height),
            wordCardTexture, new Color(0.06f, 0.12f, 0.2f, 0.25f * opacity));
        tint.a = opacity;
        DrawTextureTinted(rect, wordCardTexture, tint);
        DrawCapsuleTrim(rect, WordTint(word.entry), opacity);
        var label = new Rect(rect.x + 4f * WordVisualScale, rect.y + WordVisualScale,
            rect.width - 8f * WordVisualScale, 18f * WordVisualScale);
        wordStyle.fontSize = WordLabelFontSize(word.entry.word, label.width);
        var previousColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, opacity);
        GUI.Label(label, word.entry.word, wordStyle);
        GUI.color = previousColor;
        GUI.Label(new Rect(rect.x + 3f * WordVisualScale, rect.y + 18f * WordVisualScale,
            rect.width - 6f * WordVisualScale, 10f * WordVisualScale),
            "(" + word.entry.partOfSpeech + ")", wordDetailStyle);
        wordStyle.fontSize = 21;
        DrawCardShimmer(word, opacity);
    }

    private void DrawCapsuleTrim(Rect rect, Color tint, float opacity)
    {
        tint.a = opacity * 0.85f;
        DrawPill(new Rect(rect.x + 14f, rect.y + 2f, rect.width - 28f, 2f), tint);
        DrawPill(new Rect(rect.x + 14f, rect.yMax - 3f, rect.width - 28f, 2f), tint);
        DrawPill(new Rect(rect.x + 3f, rect.y + 17f, 2f, rect.height - 34f), tint);
        DrawPill(new Rect(rect.xMax - 5f, rect.y + 17f, 2f, rect.height - 34f), tint);
    }

    private void DrawHologramPanel(Rect rect)
    {
        DrawTextureTinted(rect, wordCardTexture, new Color(0.035f, 0.09f, 0.2f, 0.95f));
        DrawCapsuleTrim(rect, WordShooterArt.Plasma, 0.6f);
        for (var y = rect.y + 8f; y < rect.yMax - 8f; y += 6f)
            DrawRectangle(new Rect(rect.x + 12f, y, rect.width - 24f, 1f), new Color(0.2f, 0.7f, 1f, 0.035f));
    }

    private void DrawCardShimmer(FallingWord word, float opacity)
    {
        var power = PlasmaLevel - 1;
        var phase = Mathf.Repeat(word.age * 0.45f, 1f);
        var point = new Vector2(word.x + 14f * WordVisualScale + phase * (WordWidth - 28f * WordVisualScale),
            word.y + 7f * WordVisualScale);
        DrawGlow(point, (26f + power * 4f) * WordVisualScale,
            new Color(0.85f, 1f, 1f, Mathf.Sin(phase * Mathf.PI) * (0.5f + power * 0.08f) * opacity));
        if (word.hitFlash > 0f)
            DrawGlow(new Vector2(word.x + WordWidth / 2f, word.y + WordHeight / 2f),
                (120f + power * 18f) * WordVisualScale,
                new Color(0.3f, 0.85f, 1f, word.hitFlash * (3f + power * 0.5f)));
    }

    private void DrawFallingTrail(FallingWord word)
    {
        var power = PlasmaLevel - 1;
        var tint = WordTint(word.entry);
        var center = new Vector2(word.x + WordWidth / 2f, word.y + WordHeight / 2f);
        var glowTint = Color.Lerp(tint, PlasmaColor(PlasmaLevel), power * 0.08f);
        var effectScale = 1f + power * 0.12f;
        DrawGlow(center, 140f * effectScale,
            new Color(glowTint.r, glowTint.g, glowTint.b, 0.14f + power * 0.035f));
        var swayDirection = Mathf.Cos(word.age * word.swayFrequency + word.swayPhase);
        var trailSide = swayDirection >= 0f ? -1f : 1f;
        for (var streak = 0; streak < 3; streak++)
        {
            var point = new Vector2(center.x + trailSide * (WordWidth * 0.5f + 1f), center.y - 12f + streak * 12f);
            var streakAlpha = 0.18f + Mathf.Abs(swayDirection) * 0.16f;
            DrawStreak(point, new Vector2(trailSide, 0.25f).normalized,
                12f + Mathf.Abs(swayDirection) * 10f, 1.5f,
                new Color(glowTint.r, glowTint.g, glowTint.b, streakAlpha));
            DrawGlow(point + new Vector2(trailSide * 10f, 2f), 9f,
                new Color(glowTint.r, glowTint.g, glowTint.b, streakAlpha * 0.4f));
        }
        var engineCount = 2 + power;
        for (var engine = 0; engine < engineCount; engine++)
        {
            var nozzleX = word.x + WordWidth * (0.2f + 0.6f * engine / (engineCount - 1f));
            var flicker = 0.75f + Mathf.Sin(word.age * 24f + engine * Mathf.PI) * 0.25f;
            var plumeLength = (18f + flicker * 14f) * (1f + power * 0.08f);
            DrawRectangle(new Rect(nozzleX - 1.5f, word.y + 5f - plumeLength, 3f + power * 0.35f, plumeLength),
                new Color(glowTint.r, glowTint.g, glowTint.b, (0.5f + power * 0.06f) * flicker));
            DrawGlow(new Vector2(nozzleX, word.y + 5f - plumeLength * 0.65f), 12f + power * 2f,
                new Color(glowTint.r, glowTint.g, glowTint.b, (0.2f + power * 0.035f) * flicker));
        }
        for (var index = 0; index < 5 + power; index++)
        {
            var phase = Mathf.Repeat(word.age * 0.8f + index * 0.2f, 1f);
            var x = center.x + Mathf.Sin(index * 2.4f + word.age * 1.6f) * (45f + power * 4f);
            var y = word.y - 8f - phase * (48f + power * 6f);
            if (y < Playfield.yMin + 4f) continue;
            var size = 3f + (1f - phase) * (5f + power);
            DrawGlow(new Vector2(x, y), size * 3f,
                new Color(glowTint.r, glowTint.g, glowTint.b, (1f - phase) * (0.7f + power * 0.04f)));
        }
        for (var index = 0; index < 4 + power * 2; index++)
        {
            var phase = Mathf.Repeat(word.age * (1.2f + power * 0.08f) + index * 0.25f, 1f);
            var side = index % 2 == 0 ? -1f : 1f;
            var x = word.x + (side < 0f ? -8f : WordWidth + 8f)
                + Mathf.Sin(word.age * 3f + index) * (4f + power);
            var y = word.y + 10f - phase * (42f + power * 5f);
            var sparkleAlpha = (1f - phase) * (0.65f + power * 0.06f);
            DrawGlow(new Vector2(x, y), 13f + power * 2f,
                new Color(0.7f, 0.9f, 1f, sparkleAlpha * 0.45f));
            DrawRectangle(new Rect(x - 3f, y - 0.75f, 6f, 1.5f), new Color(0.9f, 0.98f, 1f, sparkleAlpha));
            DrawRectangle(new Rect(x - 0.75f, y - 3f, 1.5f, 6f), new Color(0.9f, 0.98f, 1f, sparkleAlpha));
        }
        if (word.age < 0.5f)
            DrawTextureTinted(CenteredRect(center, 45f + word.age * 170f), ringTexture,
                new Color(tint.r, tint.g, tint.b, (1f - word.age / 0.5f) * 0.6f));
    }

    private static Color WordTint(WordEntry entry)
    {
        return Color.Lerp(WordShooterArt.Plasma, new Color(0.8f, 0.45f, 1f),
            (entry.word.Length % 5) / 4f);
    }

    private void DrawWordHealth(FallingWord word)
    {
        var characterCount = VocabularyTopicData.LetterCount(word.entry.word);
        var width = (WordWidth - 20f * WordVisualScale) / characterCount;
        var opacity = Mathf.SmoothStep(0f, 1f, word.age / 0.22f);
        DrawPill(new Rect(word.x + 8f * WordVisualScale, word.y + 28f * WordVisualScale,
            WordWidth - 16f * WordVisualScale, 4f * WordVisualScale),
            new Color(0.03f, 0.09f, 0.18f, 0.35f * opacity));
        for (var index = 0; index < characterCount; index++)
        {
            var color = index < word.hitsRemaining ? new Color(0.9f, 1f, 0.96f, opacity)
                : new Color(1f, 1f, 1f, 0.13f * opacity);
            DrawPill(new Rect(word.x + 10f * WordVisualScale + index * width,
                word.y + 29f * WordVisualScale, width - 2f * WordVisualScale, 2f * WordVisualScale), color);
        }
    }

    private static void DrawPill(Rect rect, Color color)
    {
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, color, 0f, rect.height / 2f);
    }

    private GUIStyle CreateExplosionMeaningStyle(WordEntry entry)
    {
        return new GUIStyle(feedbackStyle)
        {
            fontSize = WordLabelFontSize(entry.word, WordWidth - 8f * WordVisualScale),
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(0, 0, 0, 0),
            clipping = TextClipping.Clip
        };
    }

    private Vector2 ExplosionMeaningTextSize(WordEntry entry)
    {
        const float horizontalMargin = 12f;
        const float verticalSafetyMargin = 16f;
        var maximumWidth = Mathf.Max(1f, uiWidth - horizontalMargin * 2f);
        var labelStyle = CreateExplosionMeaningStyle(entry);
        var content = new GUIContent(entry.meaning);
        // Measure using the same width that the label will receive. Measuring a
        // narrower, single-line CalcSize result can underestimate wrapped text
        // and clip the second or third line inside the label rectangle.
        var layoutWidth = maximumWidth;
        var textHeight = labelStyle.CalcHeight(content, layoutWidth);
        return new Vector2(layoutWidth, textHeight + verticalSafetyMargin);
    }

    private void DrawExplosions()
    {
        foreach (var explosion in explosions)
        {
            if (explosion.age < 0.65f) DrawExplosionBurst(explosion);
            var textSize = ExplosionMeaningTextSize(explosion.entry);
            var meaningStyle = CreateExplosionMeaningStyle(explosion.entry);
            var x = Mathf.Clamp(explosion.position.x - textSize.x * 0.5f,
                12f, Mathf.Max(12f, uiWidth - textSize.x - 12f));
            var y = Mathf.Clamp(explosion.position.y - textSize.y - 16f - explosion.age * 6f,
                178f, Mathf.Max(178f, uiHeight - textSize.y - 12f));
            var previousColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01((ExplosionDuration - explosion.age) / 0.5f));
            GUI.Label(new Rect(x, y, textSize.x, textSize.y), explosion.entry.meaning, meaningStyle);
            GUI.color = previousColor;
        }
    }

    private void DrawExplosionBurst(WordExplosion explosion)
    {
        var progress = explosion.age / 0.65f;
        DrawGlow(explosion.position, 150f * (1f + progress), new Color(0.4f, 0.6f, 1f, (1f - progress) * 0.9f));
        DrawTextureTinted(CenteredRect(explosion.position, 30f + 180f * progress), ringTexture,
            new Color(0.1f, 0.75f, 0.7f, 1f - progress));
        DrawTextureTinted(CenteredRect(explosion.position, 20f + 120f * progress), ringTexture,
            new Color(0.7f, 0.4f, 1f, (1f - progress) * 0.7f));
        DrawExplosionSmoke(explosion, progress);
        var size = Mathf.Lerp(14f, 3f, progress);
        for (var index = 0; index < 20; index++)
        {
            var angle = index * Mathf.PI / 10f;
            var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (16f + (60f + index % 4 * 15f) * progress);
            offset.y += 50f * progress * progress;
            var point = explosion.position + offset;
            var tint = index % 3 == 0 ? WordTint(explosion.entry) : WordShooterArt.Plasma;
            tint.a = 1f - progress;
            DrawGlow(point, size * 3f, tint);
            DrawStreak(point, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)),
                size * 1.5f, size * 0.4f, tint);
        }
    }

    private void DrawExplosionSmoke(WordExplosion explosion, float progress)
    {
        for (var index = 0; index < 6; index++)
        {
            var angle = index * Mathf.PI / 3f;
            var point = explosion.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * progress * 45f;
            point.y -= progress * 20f;
            DrawGlow(point, 30f + progress * 60f, new Color(0.2f, 0.3f, 0.38f, (1f - progress) * 0.25f));
        }
    }

    private void DrawHitSparks()
    {
        foreach (var spark in hitSparks)
        {
            var progress = spark.age / 0.25f;
            DrawGlow(spark.position, 55f, new Color(0.3f, 0.8f, 1f, 1f - progress));
            DrawTextureTinted(CenteredRect(spark.position, 12f + progress * 50f), ringTexture,
                new Color(0.4f, 0.8f, 1f, (1f - progress) * 0.65f));
            for (var index = 0; index < 6; index++)
            {
                var angle = index * Mathf.PI / 3f;
                var point = spark.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (8f + progress * 30f);
                DrawStreak(point, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)),
                    10f * (1f - progress), 2f, new Color(0.4f, 0.8f, 1f, 1f - progress));
            }
        }
    }

    private void DrawShotPuffs()
    {
        foreach (var puff in shotPuffs)
        {
            var progress = puff.age / 0.4f;
            var perpendicular = new Vector2(-puff.direction.y, puff.direction.x);
            for (var index = 0; index < 3; index++)
            {
                var point = puff.position + puff.direction * (8f + progress * 32f);
                point += perpendicular * (index - 1) * progress * 12f;
                DrawGlow(point, 12f + progress * 25f, new Color(0.4f, 0.5f, 0.55f, (1f - progress) * 0.25f));
            }
        }
    }

    private static void DrawStreak(Vector2 point, Vector2 direction, float length, float width, Color tint)
    {
        var previousMatrix = GUI.matrix;
        GUI.matrix = WordShooterArt.RotateAtPivot(previousMatrix, point,
            Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg);
        DrawRectangle(new Rect(point.x - width / 2f, point.y, width, length), tint);
        GUI.matrix = previousMatrix;
    }

    private void DrawShootingRange()
    {
        DrawRectangle(new Rect(18f, Playfield.yMax, uiWidth - 36f, 2f), new Color(0.2f, 0.7f, 1f, 0.4f));
        var muzzle = MuzzlePosition;
        for (var distance = 18f; distance < 125f; distance += 20f)
        {
            var point = muzzle + aimDirection * distance;
            DrawRectangle(new Rect(point.x - 2f, point.y - 2f, 4f, 4f), new Color(0.2f, 0.8f, 1f, 0.5f));
        }
    }

    private void DrawPlasmaProgress()
    {
        var next = PlasmaLevel == 5 ? "MAX" : score + "/" + PlasmaThresholds[PlasmaLevel];
        GUI.Label(new Rect(18f, uiHeight - 157f, uiWidth - 36f, 27f),
            "PLASMA " + PlasmaNames[PlasmaLevel - 1] + "  •  DAMAGE ×" + PlasmaLevel + "  •  " + next, smallStyle);
        var rect = new Rect(30f, uiHeight - 126f, uiWidth - 60f, 5f);
        DrawPill(rect, new Color(0.15f, 0.25f, 0.4f));
        if (UpgradeProgress > 0f)
            DrawPill(new Rect(rect.x, rect.y, rect.width * UpgradeProgress, rect.height), PlasmaColor(PlasmaLevel));
    }

    private void DrawProjectiles()
    {
        foreach (var projectile in projectiles)
        {
            var point = projectile.position;
            var tint = PlasmaColor(projectile.level);
            var tier = Mathf.Clamp(projectile.level, 1, 5);
            var powerScale = ProjectilePowerScale(tier);
            var direction = projectile.velocity.normalized;
            var trailCount = ProjectileTrailCount(tier);
            for (var index = 1; index <= trailCount; index++)
            {
                var trailDistance = tier == 1 ? 9f : 8f + tier * 1.5f;
                var trailSize = tier == 1 ? 22f - index * 3f : 16f + tier * 3f - index * 0.65f;
                var trailAlpha = tier == 1 ? 0.5f - index * 0.06f : 0.72f - index * 0.045f;
                DrawGlow(point - direction * index * trailDistance, trailSize,
                    new Color(tint.r, tint.g, tint.b, Mathf.Max(0.06f, trailAlpha)));
            }
            DrawGlow(point, tier == 1 ? 32f : 38f * powerScale, tint);
            if (tier > 1)
                DrawTextureTinted(CenteredRect(point, 22f * powerScale), ringTexture,
                    new Color(tint.r, tint.g, tint.b, 0.8f));
            var perpendicular = new Vector2(-direction.y, direction.x);
            var sparkCount = ProjectileSparkCount(tier);
            for (var spark = 0; spark < sparkCount; spark++)
            {
                var side = spark - (sparkCount - 1) * 0.5f;
                var sparkPoint = point - direction * (10f + Mathf.Abs(side) * 4f) + perpendicular * side * 6f;
                DrawGlow(sparkPoint, 9f + tier * 2f,
                    new Color(tint.r, tint.g, tint.b, 0.9f - Mathf.Abs(side) * 0.1f));
            }
            var previousMatrix = GUI.matrix;
            GUI.matrix = WordShooterArt.RotateAtPivot(previousMatrix, point,
                Mathf.Atan2(projectile.velocity.x, -projectile.velocity.y) * Mathf.Rad2Deg);
            DrawRectangle(new Rect(point.x - 3f * powerScale, point.y, 6f * powerScale, 28f * powerScale),
                new Color(tint.r, tint.g, tint.b, 0.3f));
            DrawRectangle(new Rect(point.x - 2f * powerScale, point.y - 5f, 4f * powerScale, 14f), Color.white);
            GUI.matrix = previousMatrix;
        }
    }

    private void DrawGun()
    {
        var pivot = GunPosition;
        DrawStationDeck(pivot);
        DrawGunEnergy(pivot);
        DrawGlow(pivot + new Vector2(0f, 25f), 115f, new Color(0.05f, 0.6f, 0.55f, 0.55f));
        var previousMatrix = GUI.matrix;
        var angle = Mathf.Atan2(aimDirection.x, -aimDirection.y) * Mathf.Rad2Deg;
        GUI.matrix = WordShooterArt.RotateAtPivot(previousMatrix, pivot, angle);
        // The source image's rotation center is 190 pixels below its muzzle.
        var scale = BarrelLength / 190f;
        var width = gunTexture.width * scale * 1.2f;
        var armor = new Color(0.04f, 0.13f, 0.22f, 0.95f);
        var armorGlow = PlasmaColor(PlasmaLevel);
        armorGlow.a = 0.9f;
        DrawPill(new Rect(pivot.x - width * 0.66f, pivot.y - 1f, width * 0.3f, 21f), armor);
        DrawPill(new Rect(pivot.x + width * 0.36f, pivot.y - 1f, width * 0.3f, 21f), armor);
        DrawPill(new Rect(pivot.x - width * 0.55f, pivot.y + 2f, 2f, 14f), armorGlow);
        DrawPill(new Rect(pivot.x + width * 0.53f, pivot.y + 2f, 2f, 14f), armorGlow);
        GUI.DrawTexture(new Rect(pivot.x - width / 2f, pivot.y - BarrelLength,
            width, gunTexture.height * scale), gunTexture);
        if (muzzleFlash > 0f) DrawMuzzleFlash(pivot);
        GUI.matrix = previousMatrix;
        GUI.Label(new Rect(pivot.x - 26f, pivot.y - BarrelLength - 22f, 52f, 16f),
            "LV " + PlasmaLevel, CreateFittedSmallStyle("LV " + PlasmaLevel, 52f));

        GUI.Label(new Rect(pivot.x - 48f, pivot.y + 45f, 96f, 16f),
            "GUN " + Mathf.RoundToInt(gunVolume * 100f) + "%", smallStyle);
        if (DrawControlButton(GunVolumeDownRect, "−", Color32(64, 82, 132)))
            AdjustGunVolume(-GunVolumeStep);
        if (DrawControlButton(GunVolumeUpRect, "+", Color32(42, 143, 112)))
            AdjustGunVolume(GunVolumeStep);
        GUI.Label(new Rect(uiWidth - 64f, pivot.y + 43f, 56f, 16f), "MUSIC", smallStyle);
        if (DrawControlButton(GunMusicToggleRect, backgroundMusicEnabled ? "ON" : "OFF",
            backgroundMusicEnabled ? Color32(42, 143, 112) : Color32(126, 87, 100), musicToggleStyle))
            SetBackgroundMusicEnabled(!backgroundMusicEnabled);
    }

    private void DrawGunEnergy(Vector2 pivot)
    {
        var tint = PlasmaColor(PlasmaLevel);
        var pulse = 0.5f + Mathf.Sin(Time.time * (4f + PlasmaLevel)) * 0.5f;
        DrawGlow(pivot, 58f + PlasmaLevel * 14f + pulse * 8f,
            new Color(tint.r, tint.g, tint.b, 0.24f + PlasmaLevel * 0.08f));
        DrawTextureTinted(CenteredRect(pivot, 52f + PlasmaLevel * 8f + pulse * 5f), ringTexture,
            new Color(tint.r, tint.g, tint.b, 0.28f + PlasmaLevel * 0.08f));
        for (var index = 0; index < 3; index++)
        {
            var angle = Time.time * (1.1f + PlasmaLevel * 0.18f) + index * Mathf.PI * 2f / 3f;
            var orbit = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var point = pivot + orbit * (31f + PlasmaLevel * 2f);
            var nodeAlpha = 0.55f + pulse * 0.35f;
            DrawGlow(point, 15f + PlasmaLevel * 2f, new Color(tint.r, tint.g, tint.b, nodeAlpha * 0.55f));
            DrawRectangle(new Rect(point.x - 2f, point.y - 2f, 4f, 4f),
                new Color(0.88f, 0.98f, 1f, nodeAlpha));
        }
        if (upgradeTimer > 0f)
        {
            var progress = 1f - upgradeTimer / 2f;
            DrawTextureTinted(CenteredRect(pivot, 35f + progress * 170f), ringTexture,
                new Color(tint.r, tint.g, tint.b, 1f - progress));
        }
    }

    private void DrawStationDeck(Vector2 pivot)
    {
        DrawPill(new Rect(pivot.x - 48f, pivot.y + 13f, 96f, 19f), new Color(0.08f, 0.15f, 0.26f));
        DrawPill(new Rect(pivot.x - 38f, pivot.y + 25f, 76f, 3f), WordShooterArt.Plasma);
        for (var index = 0; index < 5; index++)
            DrawRectangle(new Rect(pivot.x - 26f + index * 12f, pivot.y + 17f, 5f, 3f),
                new Color(0.3f, 0.8f, 1f, 0.7f));
    }

    private void DrawMuzzleFlash(Vector2 pivot)
    {
        var strength = muzzleFlash / 0.09f;
        var tint = PlasmaColor(PlasmaLevel);
        var muzzle = pivot + Vector2.down * BarrelLength;
        DrawGlow(muzzle, 72f + PlasmaLevel * 12f, new Color(tint.r, tint.g, tint.b, strength));
        DrawTextureTinted(CenteredRect(muzzle, 16f + (1f - strength) * 42f), ringTexture,
            new Color(tint.r, tint.g, tint.b, strength * 0.8f));
        DrawTextureTinted(CenteredRect(muzzle, 30f + (1f - strength) * 54f), ringTexture,
            new Color(0.78f, 0.96f, 1f, strength * 0.65f));
        DrawGlow(muzzle, 24f + PlasmaLevel * 5f, new Color(0.92f, 1f, 1f, strength * 0.9f));
        var streakCount = MuzzleStreakCount(PlasmaLevel);
        for (var index = 0; index < streakCount; index++)
        {
            var angle = index * Mathf.PI * 2f / streakCount;
            var direction = new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle));
            DrawStreak(muzzle, direction,
                (PlasmaLevel == 1 ? 22f + PlasmaLevel * 6f : 24f + PlasmaLevel * 9f) * strength,
                PlasmaLevel == 1 ? 2.5f + strength : 2.5f + PlasmaLevel * 0.45f,
                new Color(tint.r, tint.g, tint.b, strength));
        }
        DrawTextureTinted(new Rect(muzzle.x - 10f, muzzle.y - 32f * strength, 20f, 36f * strength),
            glowTexture, new Color(tint.r, tint.g, tint.b, strength));
        DrawRectangle(new Rect(muzzle.x - 2f, muzzle.y - 12f * strength, 4f, 14f * strength),
            new Color(0.9f, 1f, 1f, strength));
    }

    private void DrawGlow(Vector2 center, float size, Color color)
    {
        DrawTextureTinted(CenteredRect(center, size), glowTexture, color);
    }

    private static Rect CenteredRect(Vector2 center, float size)
    {
        return new Rect(center.x - size / 2f, center.y - size / 2f, size, size);
    }

    private void DrawResultFireworks()
    {
        var elapsed = Time.time - roundCompletedAt;
        if (elapsed < 0f || elapsed > 7.5f) return;
        var colors = new[]
        {
            new Color(1f, 0.5f, 0.3f),
            new Color(0.35f, 0.85f, 1f),
            new Color(1f, 0.8f, 0.3f)
        };
        for (var burst = 0; burst < 3; burst++)
        {
            var phase = Mathf.Repeat((elapsed + burst * 0.43f) / 1.3f, 1f);
            if (phase > 0.68f) continue;
            var progress = phase / 0.68f;
            var center = new Vector2(uiWidth * (burst == 1 ? 0.82f : 0.18f), uiHeight * 0.18f);
            var tint = colors[(burst + Mathf.FloorToInt(elapsed / 1.3f)) % colors.Length];
            DrawGlow(center, 18f + progress * 35f, new Color(tint.r, tint.g, tint.b, (1f - progress) * 0.8f));
            for (var spark = 0; spark < 16; spark++)
            {
                var angle = spark * Mathf.PI / 8f + burst * 0.31f;
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var point = center + direction * (8f + progress * (30f + spark % 3 * 6f));
                var sparkColor = new Color(tint.r, tint.g, tint.b, 1f - progress);
                DrawGlow(point, 8f * (1f - progress) + 2f, sparkColor);
                DrawStreak(point, direction, 8f * (1f - progress), 1.5f, sparkColor);
            }
        }
    }

    private static void DrawTextureTinted(Rect rect, Texture2D texture, Color color)
    {
        var previousColor = GUI.color;
        GUI.color = previousColor * color;
        GUI.DrawTexture(rect, texture);
        GUI.color = previousColor;
    }

    private static void DrawRectangle(Rect rect, Color color)
    {
        var previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }

    private void DrawResult()
    {
        DrawResultFireworks();
        GUI.Label(new Rect(25f, uiHeight * 0.09f, uiWidth - 50f, 58f),
            "ROUND COMPLETE  •  " + currentLevel, titleStyle);
        var panel = new Rect(24f, uiHeight * 0.18f, uiWidth - 48f, uiHeight * 0.49f);
        DrawHologramPanel(panel);
        GUI.Label(new Rect(38f, panel.y + 8f, uiWidth - 76f, 36f), "SCORE  " + score, titleStyle);
        GUI.Label(new Rect(38f, panel.y + 45f, uiWidth - 76f, 24f),
            TopicName(TopicIndex(currentTopic)), smallStyle);
        GUI.Label(new Rect(38f, panel.y + 68f, uiWidth - 76f, 26f),
            "Words destroyed: " + destroyedCount + "/" + CurrentWordPool.Length, bodyStyle);
        GUI.Label(new Rect(38f, panel.y + 96f, uiWidth - 76f, 26f),
            newRecord ? "NEW PERSONAL BEST!" : "Personal best: " + bestScore, smallStyle);
        GUI.Label(new Rect(38f, panel.y + 124f, uiWidth - 76f, 26f),
            missedWords.Count == 0 ? "No missed words. Great work!" : "MISSED WORDS  •  Tap to listen", smallStyle);
        DrawMissedWordReview(new Rect(panel.x + 16f, panel.y + 152f, panel.width - 32f, panel.height - 164f));

        var buttonY = uiHeight * 0.705f;
        if (DrawButton(new Rect(40f, buttonY, uiWidth - 80f, 54f), "Play Again  •  " + currentLevel, Color32(42, 143, 112)))
            StartTopicRound(currentLevel, currentTopic);
        if (DrawButton(new Rect(40f, buttonY + 62f, uiWidth - 80f, 48f), "Choose Another Topic", Color32(126, 87, 220)))
            screenState = ScreenState.TopicSelection;
        if (DrawButton(new Rect(40f, buttonY + 118f, uiWidth - 80f, 44f), "Choose a Level", Color32(64, 82, 132)))
            screenState = ScreenState.Menu;
    }

    private bool TryStartTopic(string topic)
    {
        currentTopic = topic;
        StartTopicRound(currentLevel, topic);
        return true;
    }

    private static int BestScoreForLevel(string level)
    {
        var best = PlayerPrefs.GetInt(BestScoreKey(level, "food"), PlayerPrefs.GetInt(BestScoreKey(level), 0));
        foreach (var topic in TopicIds)
            best = Mathf.Max(best, PlayerPrefs.GetInt(BestScoreKey(level, topic), 0));
        return best;
    }

    private void DrawMissedWordReview(Rect rect)
    {
        if (missedWords.Count == 0)
        {
            GUI.Label(rect, "All words cleared!", smallStyle);
            return;
        }

        var content = new Rect(0f, 0f, rect.width - 16f, missedWords.Count * 34f);
        reviewScrollPosition = GUI.BeginScrollView(rect, reviewScrollPosition, content, false, true);
        for (var index = 0; index < missedWords.Count; index++)
        {
            var entry = missedWords[index];
            var row = new Rect(0f, index * 34f, content.width, 31f);
            DrawTextureTinted(row, wordCardTexture, new Color(0.05f, 0.2f, 0.3f, 0.75f));
            if (GUI.Button(row, GUIContent.none, GUIStyle.none)) PlayReviewPronunciation(entry);
            GUI.Label(new Rect(row.x + 6f, row.y, row.width - 12f, row.height),
                entry.word + " (" + entry.partOfSpeech + ")  —  " + entry.meaning + "   ▶", smallStyle);
        }
        GUI.EndScrollView();
    }

    private void PlayReviewPronunciation(WordEntry entry)
    {
        pronunciationQueue.Clear();
        pronunciationSource.Stop();
        pronunciationSource.clip = pronunciations[entry.word];
        pronunciationSource.Play();
        ApplyAudioLevels();
    }

    private bool DrawButton(Rect rect, string label, Color color)
    {
        DrawButtonBackground(rect, color);
        var clicked = GUI.Button(rect, GUIContent.none, GUIStyle.none);
        GUI.Label(rect, label, buttonStyle);
        return clicked;
    }

    private bool DrawControlButton(Rect rect, string label, Color color)
    {
        return DrawControlButton(rect, label, color, buttonStyle);
    }

    private bool DrawControlButton(Rect rect, string label, Color color, GUIStyle labelStyle)
    {
        DrawButtonBackground(rect, color);
        var clicked = !Application.isMobilePlatform && GUI.Button(rect, GUIContent.none, GUIStyle.none);
        GUI.Label(rect, label, labelStyle);
        return clicked;
    }

    private void DrawButtonBackground(Rect rect, Color color)
    {
        DrawTextureTinted(rect, wordCardTexture, color * 0.5f + new Color(0f, 0f, 0f, 0.5f));
        DrawCapsuleTrim(rect, color, 1f);
    }

    private static string BestScoreKey(string level)
    {
        return "WordGalaxy.BestScore." + level;
    }

    private static string BestScoreKey(string level, string topic)
    {
        return "WordGalaxy.BestScore." + level + "." + topic;
    }

    private static int TopicIndex(string topic)
    {
        for (var index = 0; index < TopicIds.Length; index++)
            if (TopicIds[index] == topic) return index;
        throw new ArgumentException("Unknown vocabulary topic: " + topic, nameof(topic));
    }

    private static string TopicName(int topicIndex)
    {
        return VocabularyTopicData.Topics[topicIndex].DisplayName;
    }

    private static string PlanetEnglishName(int levelIndex)
    {
        return PlanetEnglishNames[levelIndex];
    }

    private void StartRound(string selectedLevel)
    {
        StartTopicRound(selectedLevel, "food");
    }

    private void StartTopicRound(string selectedLevel, string selectedTopic)
    {
        SetWordsPaused(false);
        ResetPronunciation();
        pronunciations.Clear();
        PlaySound(roundStartSound);
        currentLevel = selectedLevel;
        currentTopic = selectedTopic;
        TopicIndex(currentTopic);
        selectedLevelIndex = CurrentLevelIndex;
        LoadPronunciationsForCurrentPool();
        var topicBestKey = BestScoreKey(currentLevel, currentTopic);
        var legacyFoodBest = currentTopic == "food" ? PlayerPrefs.GetInt(BestScoreKey(currentLevel), 0) : 0;
        bestScore = PlayerPrefs.GetInt(topicBestKey, legacyFoodBest);
        if (currentTopic == "food" && !PlayerPrefs.HasKey(topicBestKey) && legacyFoodBest > 0)
        {
            PlayerPrefs.SetInt(topicBestKey, legacyFoodBest);
            PlayerPrefs.Save();
        }
        newRecord = false;
        score = 0;
        destroyedCount = 0;
        roundCompletedAt = 0f;
        lastHitWord = null;
        missedWords.Clear();
        missedWordSet.Clear();
        fallingWords.Clear();
        aimDirection = Vector2.down;
        aimTarget = Playfield.center;
        keyboardAiming = true;
        screenState = ScreenState.Playing;
        projectiles.Clear();
        explosions.Clear();
        hitSparks.Clear();
        shotPuffs.Clear();
        wordBag.Clear();
        FillWordBag();
        reviewScrollPosition = Vector2.zero;
        muzzleFlash = 0f;
        upgradeTimer = 0f;
        shotCooldown = 0f;
        ResetShootingInput();
        spawnTimer = 0.05f;
    }

    private void SpawnWord()
    {
        var entry = NextWord();
        var swayAmplitude = UnityEngine.Random.Range(28f, 42f);
        var minX = 18f + swayAmplitude;
        var maxX = Mathf.Max(minX, uiWidth - WordWidth - 18f - swayAmplitude);
        var originX = UnityEngine.Random.Range(minX, maxX);
        var swayPhase = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        var swayFrequency = UnityEngine.Random.Range(
            WordSwayFrequency * 1.5f * WordSwaySpeedMultiplier,
            WordSwayFrequency * 2.5f * WordSwaySpeedMultiplier);
        var x = originX + Mathf.Sin(swayPhase) * swayAmplitude;
        var speed = LevelSpeeds[CurrentLevelIndex];
        fallingWords.Add(new FallingWord
        {
            entry = entry,
            x = x,
            previousX = x,
            originX = originX,
            swayAmplitude = swayAmplitude,
            swayFrequency = swayFrequency,
            swayPhase = swayPhase,
            y = 178f,
            previousY = 178f,
            speed = speed * UnityEngine.Random.Range(0.9f, 1.15f),
            maxHits = VocabularyTopicData.LetterCount(entry.word),
            hitsRemaining = VocabularyTopicData.LetterCount(entry.word)
        });
    }

    private void FillWordBag()
    {
        foreach (var entry in CurrentWordPool) wordBag.Enqueue(entry);
    }

    private WordEntry NextWord()
    {
        return wordBag.Dequeue();
    }

    private void OnWordHit(FallingWord fallingWord, int damage)
    {
        ApplyWordDamage(fallingWord, damage);
    }

    private void ApplyWordDamage(FallingWord fallingWord, float damage)
    {
        lastHitWord = fallingWord.entry;
        if (!fallingWord.firstPronunciationQueued)
        {
            fallingWord.firstPronunciationQueued = true;
            QueuePronunciation(fallingWord.entry);
        }
        var accumulatedDamage = fallingWord.damageCarry + damage;
        var wholeDamage = Mathf.FloorToInt(accumulatedDamage + 0.0001f);
        fallingWord.damageCarry = Mathf.Max(0f, accumulatedDamage - wholeDamage);
        fallingWord.hitsRemaining = Mathf.Max(0, fallingWord.hitsRemaining - wholeDamage);
        fallingWord.hitFlash = 0.12f;
        var remainingHealth = fallingWord.hitsRemaining - fallingWord.damageCarry;
        if (!fallingWord.halfHealthPronunciationQueued && remainingHealth <= fallingWord.maxHits * 0.5f)
        {
            fallingWord.halfHealthPronunciationQueued = true;
            QueuePronunciation(fallingWord.entry);
        }
        hitSparks.Add(new HitSpark
        {
            position = new Vector2(fallingWord.x + WordWidth / 2f, fallingWord.y + WordHeight)
        });
        if (fallingWord.hitsRemaining > 0)
        {
            return;
        }

        fallingWords.Remove(fallingWord);
        QueuePronunciation(fallingWord.entry);
        var previousLevel = PlasmaLevel;
        score += 10;
        if (PlasmaLevel > previousLevel)
        {
            upgradeTimer = 2f;
            PlaySound(upgradeSound);
        }
        destroyedCount++;
        explosions.Add(new WordExplosion
        {
            entry = fallingWord.entry,
            position = new Vector2(fallingWord.x + WordWidth / 2f, fallingWord.y + WordHeight / 2f)
        });
    }

    private void EndRound()
    {
        roundCompletedAt = Time.time;
        PlaySound(roundEndSound);
        if (score > bestScore)
        {
            bestScore = score;
            newRecord = true;
            PlayerPrefs.SetInt(BestScoreKey(currentLevel, currentTopic), bestScore);
            if (currentTopic == "food") PlayerPrefs.SetInt(BestScoreKey(currentLevel), bestScore);
            PlayerPrefs.Save();
        }
        screenState = ScreenState.Result;
        fallingWords.Clear();
        projectiles.Clear();
        explosions.Clear();
        hitSparks.Clear();
        shotPuffs.Clear();
        muzzleFlash = 0f;
        upgradeTimer = 0f;
        ResetShootingInput();
    }

    private static Color Color32(byte red, byte green, byte blue)
    {
        return new Color32(red, green, blue, 255);
    }
}
