using System;
using System.Collections.Generic;

internal static class LegacyFoodExampleData
{
    private static readonly Dictionary<string, string[]> Examples =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["apple"] = new[]
            {
                "A crisp apple and a handful of almonds make an easy snack.",
                "She grated an apple into the warm bowl of oatmeal.",
                "The baker used a tart apple to balance the sweet filling.",
                "Keep each apple in the refrigerator to make it last longer."
            },
            ["bread"] = new[]
            {
                "Fresh bread is best with a little olive oil.",
                "He toasted a slice of bread to serve with the soup.",
                "The bread dough rose slowly in the cool kitchen.",
                "We bought a loaf of whole-grain bread at the market."
            },
            ["cheese"] = new[]
            {
                "A sharp cheese gives the macaroni sauce plenty of flavor.",
                "She crumbled cheese over the roasted beet salad.",
                "Store soft cheese in a sealed container in the refrigerator.",
                "The sandwich tastes better with melted cheese and tomato."
            },
            ["chicken"] = new[]
            {
                "Roast the chicken with lemon and rosemary for Sunday lunch.",
                "She added shredded chicken to the vegetable soup.",
                "Marinate the chicken before placing it on the grill.",
                "Leftover chicken makes a quick filling for wraps."
            },
            ["grapes"] = new[]
            {
                "Wash the grapes just before serving them with the cheese.",
                "She froze a few grapes to chill her afternoon drink.",
                "Red grapes added a burst of sweetness to the salad.",
                "The children packed grapes in their lunch boxes."
            },
            ["noodles"] = new[]
            {
                "The noodles soaked up the fragrant broth.",
                "Toss the noodles with sesame oil so they do not stick together.",
                "She topped the noodles with crisp vegetables and tofu.",
                "Fresh noodles cook faster than dried ones."
            },
            ["orange"] = new[]
            {
                "An orange adds bright flavor to the fennel salad.",
                "He squeezed an orange over the fruit bowl.",
                "The orange peel scented the cake batter.",
                "We shared an orange after our walk."
            },
            ["rice"] = new[]
            {
                "Rinse the rice until the water runs clear.",
                "The curry was served over steamed rice.",
                "She used leftover rice to make a quick lunch.",
                "Brown rice has a pleasantly nutty flavor."
            },
            ["soup"] = new[]
            {
                "The soup simmered with carrots, beans, and thyme.",
                "Serve the soup with warm bread for a simple supper.",
                "A squeeze of lemon lifted the flavor of the soup.",
                "He packed the soup in a thermos for the hike."
            },
            ["water"] = new[]
            {
                "Add a little water if the sauce becomes too thick.",
                "The cook brought a pot of water to a gentle boil.",
                "She drank a glass of water with her spicy meal.",
                "Use cold water to rinse the rice before cooking it."
            },
            ["banana"] = new[]
            {
                "Mash a ripe banana into the pancake batter.",
                "She packed a banana with her lunch for a quick snack.",
                "A sliced banana sweetened the plain yogurt.",
                "Freeze a banana before blending it into a creamy smoothie."
            },
            ["mango"] = new[]
            {
                "Dice the mango and fold it into the fresh salsa.",
                "A ripe mango makes a refreshing addition to breakfast.",
                "She blended mango with yogurt for a cool drink.",
                "The mango added sweetness to the spicy rice salad."
            },
            ["pear"] = new[]
            {
                "A baked pear with cinnamon makes a simple dessert.",
                "He sliced a pear into his lunch salad.",
                "Choose a firm pear if you plan to poach it.",
                "The soft pear paired nicely with blue cheese."
            },
            ["peach"] = new[]
            {
                "Grill each peach half until the cut side is caramelized.",
                "She added a ripe peach to her morning yogurt.",
                "A peach pie tastes especially good while it is still warm.",
                "The peach made the green smoothie naturally sweet."
            },
            ["plum"] = new[]
            {
                "The cook simmered each plum into a fragrant compote.",
                "A ripe plum is a juicy snack on a hot afternoon.",
                "She sliced a plum over her breakfast cereal.",
                "The tart plum filling balanced the buttery pastry."
            },
            ["cherry"] = new[]
            {
                "Add a cherry to the top of each small cake.",
                "She stirred chopped cherry into the chocolate batter.",
                "The cherry sauce complemented the roast duck.",
                "A spoonful of cherry compote made a colorful dessert."
            },
            ["lemon"] = new[]
            {
                "Squeeze lemon over the grilled fish just before serving.",
                "The lemon zest gave the biscuits a fresh aroma.",
                "She stirred lemon juice into the salad dressing.",
                "A wedge of lemon brightened the iced tea."
            },
            ["lime"] = new[]
            {
                "A squeeze of lime brings the black bean soup to life.",
                "He added lime juice to the avocado dressing.",
                "Grate a little lime zest over the fruit salad.",
                "The lime kept the cut avocado from turning brown."
            },
            ["melon"] = new[]
            {
                "Chilled melon is refreshing after a spicy meal.",
                "She cut the melon into cubes for the picnic.",
                "A slice of melon made a light breakfast.",
                "The melon paired well with salty cured ham."
            },
            ["papaya"] = new[]
            {
                "Serve ripe papaya with a squeeze of lime.",
                "The papaya added a tropical sweetness to the smoothie.",
                "She diced papaya for a bright fruit salad.",
                "Unripe papaya can be shredded into a crisp salad."
            },
            ["coconut"] = new[]
            {
                "Stir coconut milk into the curry near the end of cooking.",
                "Toasted coconut added crunch to the rice pudding.",
                "She blended coconut with banana for a creamy smoothie.",
                "The cake was topped with flakes of dried coconut."
            },
            ["kiwi"] = new[]
            {
                "Peel the kiwi and slice it over the breakfast bowl.",
                "A ripe kiwi adds a tangy note to fruit salad.",
                "She blended kiwi with apple for a refreshing drink.",
                "The kiwi's bright color made the tart look inviting."
            },
            ["pineapple"] = new[]
            {
                "Grilled pineapple caramelizes beautifully over the fire.",
                "She added pineapple chunks to the sweet-and-sour sauce.",
                "Fresh pineapple gave the salsa a juicy, tart flavor.",
                "The baker used pineapple to keep the cake moist."
            },
            ["strawberry"] = new[]
            {
                "Slice a strawberry over the pancakes just before serving.",
                "She mashed strawberry into a quick sauce for the yogurt.",
                "A ripe strawberry added sweetness to the garden salad.",
                "The children dipped each strawberry in melted chocolate."
            },
            ["blueberry"] = new[]
            {
                "Fold each blueberry gently into the muffin batter.",
                "A spoonful of blueberry compote brightened the bowl of oatmeal.",
                "She simmered blueberry with a little lemon for a sauce.",
                "The blueberry topping turned the pancakes a deep purple."
            },
            ["avocado"] = new[]
            {
                "Mash avocado with lime and salt for a quick sandwich spread.",
                "She added sliced avocado to the grain bowl.",
                "Ripe avocado gives the smoothie a silky texture.",
                "The avocado was perfectly soft for the fresh salsa."
            },
            ["tomato"] = new[]
            {
                "Roast the tomato with garlic for a rich pasta sauce.",
                "He sliced a tomato to add freshness to the sandwich.",
                "A ripe tomato makes the best base for summer gazpacho.",
                "She chopped tomato into the bean and corn salad."
            },
            ["potato"] = new[]
            {
                "Bake the potato until its center is soft and fluffy.",
                "The potato absorbed the spices in the vegetable curry.",
                "She grated potato to make crisp breakfast pancakes.",
                "A small potato thickened the homemade soup."
            },
            ["carrot"] = new[]
            {
                "Grate a carrot into the salad for extra crunch.",
                "The carrot softened in the simmering lentil stew.",
                "She roasted each carrot with cumin and honey.",
                "A carrot adds natural sweetness to tomato sauce."
            },
            ["onion"] = new[]
            {
                "Cook the onion slowly until it turns golden and sweet.",
                "He diced an onion to start the bean stew.",
                "The onion gave the broth a rich savory base.",
                "A slice of red onion added bite to the burger."
            },
            ["garlic"] = new[]
            {
                "Crush the garlic and stir it into the warm olive oil.",
                "A little garlic gave the roasted vegetables a fragrant edge.",
                "She rubbed garlic on the toasted bread before serving it.",
                "The garlic softened as it simmered in the tomato sauce."
            },
            ["ginger"] = new[]
            {
                "Fresh ginger brought warmth to the carrot soup.",
                "She grated ginger into the marinade for the tofu.",
                "A slice of ginger steeped in hot tea with honey.",
                "The cook added ginger to the stir-fried vegetables."
            },
            ["cabbage"] = new[]
            {
                "Shred the cabbage finely for a crisp slaw.",
                "The cabbage softened in the pot with smoked beans.",
                "She wrapped rice and vegetables in a tender cabbage leaf.",
                "A splash of vinegar kept the cabbage salad bright."
            },
            ["lettuce"] = new[]
            {
                "Wash the lettuce and dry each leaf before making the salad.",
                "Crisp lettuce added freshness to the grilled chicken wrap.",
                "She tucked lettuce beneath the tomato in the sandwich.",
                "The lettuce stayed crunchy in the chilled lunch bowl."
            },
            ["spinach"] = new[]
            {
                "Wilt the spinach in the pan with a little garlic.",
                "She folded spinach into the cheese omelet.",
                "Fresh spinach makes a tender base for the warm lentil salad.",
                "Add spinach at the end so it keeps its vivid green color."
            },
            ["broccoli"] = new[]
            {
                "Steam the broccoli until it is bright green and tender.",
                "The roasted broccoli developed crisp, savory edges.",
                "She chopped broccoli into the creamy pasta bake.",
                "A squeeze of lemon made the broccoli taste fresh."
            },
            ["cucumber"] = new[]
            {
                "Slice cucumber thinly and toss it with rice vinegar.",
                "The cool cucumber balanced the heat of the chili noodles.",
                "She grated cucumber into a yogurt and dill dip.",
                "A crisp cucumber added crunch to the lunch salad."
            },
            ["pumpkin"] = new[]
            {
                "Roast the pumpkin until its edges begin to caramelize.",
                "She stirred pumpkin puree into the pancake batter.",
                "A bowl of pumpkin soup made a warming autumn lunch.",
                "The pumpkin pie was flavored with cinnamon and nutmeg."
            },
            ["corn"] = new[]
            {
                "Grill the corn and brush it with lime butter.",
                "She stirred sweet corn into the chowder.",
                "A handful of corn added color to the rice salad.",
                "The corn kernels were tender and sweet after steaming."
            },
            ["peas"] = new[]
            {
                "Stir the peas into the risotto just before serving.",
                "Frozen peas make a quick addition to vegetable soup.",
                "She mashed peas with mint for a bright side dish.",
                "The peas stayed sweet after a brief steam."
            },
            ["beans"] = new[]
            {
                "Soak the beans overnight before cooking them.",
                "The beans simmered with tomatoes and smoked paprika.",
                "She mashed beans into a creamy dip for warm flatbread.",
                "Black beans and rice make a satisfying weeknight meal."
            },
            ["mushroom"] = new[]
            {
                "Sauté each mushroom until its moisture has evaporated.",
                "A wild mushroom added an earthy flavor to the sauce.",
                "She filled the pastry with mushroom and caramelized onion.",
                "The mushroom slices browned nicely in the hot pan."
            },
            ["beef"] = new[]
            {
                "Braise the beef slowly until it falls apart with a fork.",
                "He sliced the beef thinly for the noodle soup.",
                "A little ginger made the beef stir-fry fragrant.",
                "The beef rested before the cook carved it for dinner."
            },
            ["pork"] = new[]
            {
                "Roast the pork with apples and fresh sage.",
                "She added shredded pork to the warm tortillas.",
                "The pork chops browned quickly in the cast-iron pan.",
                "A slow-cooked pork shoulder made a generous family meal."
            },
            ["fish"] = new[]
            {
                "Bake the fish with lemon slices and fresh dill.",
                "The fish was flaky and tender after a short time in the oven.",
                "She added fish to the coconut broth just before serving.",
                "Fresh fish from the market made a simple, delicious supper."
            },
            ["shrimp"] = new[]
            {
                "Toss the shrimp with garlic and chili in a hot pan.",
                "She added shrimp to the cold noodle salad.",
                "The shrimp turned pink as it cooked in the broth.",
                "Grilled shrimp taste delicious with lime and cilantro."
            },
            ["crab"] = new[]
            {
                "The cook folded fresh crab into the delicate omelet.",
                "She served crab with warm bread and lemon butter.",
                "A little chili gave the crab salad a lively kick.",
                "We shared a bowl of crab bisque for lunch."
            },
            ["squid"] = new[]
            {
                "Grill the squid briefly so it stays tender.",
                "She sliced squid into rings for the seafood stew.",
                "The squid was served with a bright lemon dressing.",
                "Fresh squid cooks quickly in a hot wok."
            },
            ["egg"] = new[]
            {
                "Beat an egg into the flour mixture until smooth.",
                "She fried an egg to top the rice bowl.",
                "A soft-boiled egg made the salad more filling.",
                "The egg bound the breadcrumbs to the vegetable patties."
            },
            ["milk"] = new[]
            {
                "Warm the milk gently before adding it to the cocoa.",
                "She poured milk over the cereal and sliced fruit.",
                "A splash of milk made the mashed potatoes creamy.",
                "The recipe uses milk to keep the bread soft."
            },
            ["butter"] = new[]
            {
                "Melt the butter in the pan before adding the mushrooms.",
                "She spread butter on the warm toast.",
                "A knob of butter gave the sauce a glossy finish.",
                "Keep the butter chilled until you are ready to bake."
            },
            ["yogurt"] = new[]
            {
                "Plain yogurt makes a cooling sauce for spicy kebabs.",
                "She topped the fruit with a spoonful of yogurt.",
                "The yogurt marinated the chicken and kept it tender.",
                "Stir honey into the yogurt for a simple breakfast."
            },
            ["cake"] = new[]
            {
                "The orange cake stayed moist for several days.",
                "She decorated the cake with fresh berries.",
                "Let the cake cool before spreading on the frosting.",
                "A slice of cake went perfectly with afternoon tea."
            },
            ["cookie"] = new[]
            {
                "The cookie turned crisp around the edges in the oven.",
                "He packed a homemade cookie for the afternoon break.",
                "Add chopped walnuts to the cookie dough for extra crunch.",
                "She shared a warm cookie with a glass of milk."
            },
            ["pizza"] = new[]
            {
                "We topped the pizza with mushrooms and fresh basil.",
                "The pizza crust baked until it was crisp and golden.",
                "She made a small pizza with tomato and goat cheese.",
                "Leftover pizza reheats best in a hot skillet."
            },
            ["sandwich"] = new[]
            {
                "The sandwich was filled with roast vegetables and hummus.",
                "She packed a sandwich for the train journey.",
                "Toast the bread before assembling the sandwich.",
                "A pickle added a sharp crunch to his sandwich."
            },
            ["pasta"] = new[]
            {
                "Cook the pasta until it is just tender.",
                "She tossed the pasta with basil pesto and cherry tomatoes.",
                "A little pasta water helped the sauce cling to the noodles.",
                "The baked pasta was bubbling when it came out of the oven."
            },
            ["honey"] = new[]
            {
                "Drizzle honey over the warm cornbread.",
                "She stirred honey into the tea to sweeten it gently.",
                "A spoonful of honey balanced the tart berry sauce.",
                "The granola was lightly coated with honey before baking."
            },
            ["tea"] = new[]
            {
                "Brew the tea for three minutes before adding milk.",
                "She served mint tea with the lemon cake.",
                "A cup of tea is a comforting end to the meal.",
                "The tea steeped with ginger and a slice of orange."
            },
            ["coffee"] = new[]
            {
                "Freshly ground coffee filled the kitchen with its rich aroma.",
                "He served coffee with small squares of dark chocolate.",
                "A splash of oat milk softened the strong coffee.",
                "The cake batter included a little coffee to deepen its flavor."
            },
            ["ingredient"] = new[]
            {
                "A ripe tomato is the key ingredient in her summer salad.",
                "Check each ingredient before you start mixing the batter.",
                "Fresh ingredient choices can transform a simple soup.",
                "The recipe lists olive oil as its first ingredient."
            },
            ["recipe"] = new[]
            {
                "I found a recipe for lentil stew in my grandmother's notebook.",
                "This recipe uses ripe bananas instead of refined sugar.",
                "The chef tested the recipe several times before sharing it.",
                "Add the herbs near the end of the recipe to keep their flavor."
            },
            ["dessert"] = new[]
            {
                "We served baked pears for dessert after the family meal.",
                "The dessert menu includes lemon tart and fresh berries.",
                "A small dessert can be a lovely finish to a rich dinner.",
                "She prepared a chocolate dessert the night before the party."
            },
            ["vinegar"] = new[]
            {
                "A splash of vinegar brightens the flavor of bean soup.",
                "Whisk vinegar with mustard to make a quick salad dressing.",
                "The pickled onions need enough vinegar to cover them.",
                "He added vinegar to balance the sweetness of the sauce."
            },
            ["seasoning"] = new[]
            {
                "Taste the broth before you adjust the seasoning.",
                "A pinch of seasoning brought out the flavor of the roasted squash.",
                "The cook mixed a simple seasoning of salt, pepper, and herbs.",
                "Good seasoning makes the beans taste lively without masking them."
            },
            ["portion"] = new[]
            {
                "The café serves a generous portion of rice with each curry.",
                "Save a portion of the soup for tomorrow's lunch.",
                "She ordered a smaller portion so she could try the dessert too.",
                "Each portion of the casserole contains plenty of vegetables."
            },
            ["nutritious"] = new[]
            {
                "Chickpeas make a nutritious addition to a green salad.",
                "The children packed a nutritious lunch with fruit and yogurt.",
                "A bowl of oatmeal is a nutritious way to start the morning.",
                "They chose nutritious snacks made from nuts and dried fruit."
            },
            ["leftovers"] = new[]
            {
                "We turned last night's leftovers into a quick vegetable fried rice.",
                "Pack the leftovers in shallow containers and refrigerate them promptly.",
                "She used the roast chicken leftovers to fill warm sandwiches.",
                "A little stock made the leftovers taste fresh again."
            },
            ["groceries"] = new[]
            {
                "We bought groceries for a week of simple home-cooked meals.",
                "Put the chilled groceries away as soon as you get home.",
                "He checked the pantry before writing the groceries list.",
                "The market delivers fresh groceries to our neighborhood."
            },
            ["supermarket"] = new[]
            {
                "The supermarket keeps fresh herbs beside the vegetables.",
                "I stopped at the supermarket to buy flour and eggs.",
                "A new supermarket opened near the farmers' market.",
                "She compares prices at the supermarket before choosing olive oil."
            },
            ["bakery"] = new[]
            {
                "The bakery fills the street with the smell of warm bread.",
                "We ordered a fruit tart from the neighborhood bakery.",
                "A small bakery sells sourdough loaves every morning.",
                "He picked up fresh rolls at the bakery on his way home."
            },
            ["seafood"] = new[]
            {
                "The restaurant serves local seafood with lemon and herbs.",
                "Keep raw seafood separate from ready-to-eat food in the fridge.",
                "We bought fresh seafood from the harbor market.",
                "A light broth lets the natural flavor of the seafood shine."
            },
            ["poultry"] = new[]
            {
                "Store raw poultry on the lowest shelf of the refrigerator.",
                "The butcher sells free-range poultry from nearby farms.",
                "Cook poultry thoroughly before serving it to your guests.",
                "She seasoned the poultry with garlic, thyme, and lemon."
            },
            ["sausage"] = new[]
            {
                "He sliced a smoked sausage into the bean stew.",
                "The sausage browned quickly in the hot pan.",
                "We served grilled sausage with mustard and warm bread.",
                "A spicy sausage gave the tomato sauce a deeper flavor."
            },
            ["cereal"] = new[]
            {
                "She poured oat cereal into a bowl and added sliced peaches.",
                "Choose a cereal with whole grains and little added sugar.",
                "The children ate cereal before catching the school bus.",
                "This crunchy cereal tastes good with plain yogurt."
            },
            ["flour"] = new[]
            {
                "Sift the flour to remove any small lumps.",
                "Whole-wheat flour gives the bread a nutty flavor.",
                "She dusted the counter with flour before rolling the pastry.",
                "The baker measured the flour carefully for the cake."
            },
            ["pepper"] = new[]
            {
                "Freshly ground pepper adds warmth to the mushroom soup.",
                "Roast the peppers until their skins begin to blister.",
                "A little black pepper balanced the creamy cheese sauce.",
                "He stuffed each pepper with rice, herbs, and lentils."
            },
            ["grilled"] = new[]
            {
                "We put grilled eggplant and peppers into warm flatbread.",
                "The grilled fish came with a bright herb salsa.",
                "She served grilled peaches with a spoonful of yogurt.",
                "A grilled cheese sandwich makes an easy lunch."
            },
            ["boiled"] = new[]
            {
                "The boiled eggs and spinach make a filling breakfast.",
                "The boiled potatoes absorbed the garlic dressing beautifully.",
                "We added boiled noodles to the fragrant soup.",
                "A few boiled beets gave the salad a deep color."
            },
            ["baked"] = new[]
            {
                "We served baked apples with a cinnamon aroma after dinner.",
                "The café sells baked rolls fresh from the oven.",
                "He brought baked salmon with dill to the picnic.",
                "The cook paired baked sweet potatoes with black beans."
            },
            ["spicy"] = new[]
            {
                "The spicy lentil curry warmed us on a rainy evening.",
                "Add coconut milk if the spicy sauce is too intense.",
                "She prefers spicy noodles with fresh lime.",
                "A spicy pepper gave the salsa a lingering heat."
            },
            ["sour"] = new[]
            {
                "The sour yogurt worked well in the cucumber dip.",
                "A sour cherry filling gives the pie a pleasant sharpness.",
                "The soup tasted sour, so the cook stirred in a little cream.",
                "He likes sour pickles alongside a rich sandwich."
            },
            ["bitter"] = new[]
            {
                "The bitter greens tasted milder after a quick sauté.",
                "A square of dark chocolate added a bitter note to the cake.",
                "The marmalade has a pleasantly bitter orange finish.",
                "She balanced the bitter coffee with a little warm milk."
            },
            ["savory"] = new[]
            {
                "The savory tart was filled with spinach and goat cheese.",
                "We baked savory muffins with cheddar and chives.",
                "A savory broth made the dumplings especially comforting.",
                "He chose a savory pastry instead of a sweet one."
            },
            ["sweetener"] = new[]
            {
                "The tea needs no sweetener when the peaches are ripe.",
                "She used maple syrup as a natural sweetener for the granola.",
                "Check the label to see which sweetener is in the jam.",
                "A small amount of sweetener softened the sharp lemon flavor."
            },
            ["cuisine"] = new[]
            {
                "The coastal cuisine features fish, citrus, and fresh herbs.",
                "We explored regional cuisine through a family-run cooking class.",
                "Her restaurant brings traditional cuisine to a modern menu.",
                "Rice and fragrant spices are central to this cuisine."
            },
            ["culinary"] = new[]
            {
                "The culinary students practiced knife skills with ripe tomatoes.",
                "A culinary tour introduced us to the city's street food.",
                "He enrolled in a culinary course to learn pastry techniques.",
                "The cookbook offers culinary ideas for using seasonal produce."
            },
            ["waiter"] = new[]
            {
                "The waiter recommended the grilled trout with lemon.",
                "Our waiter brought extra bread for the table.",
                "A friendly waiter explained which dishes contained nuts.",
                "The waiter noticed our water glasses were empty."
            },
            ["chef"] = new[]
            {
                "The chef finished the soup with chopped parsley.",
                "A local chef showed us how to fold dumplings by hand.",
                "The chef tasted the sauce before adding more salt.",
                "Their chef changes the lunch menu with the seasons."
            },
            ["menu"] = new[]
            {
                "The menu marks dishes that can be made without dairy.",
                "We studied the menu while the server brought us water.",
                "Today's menu features roasted vegetables and barley.",
                "The restaurant updates its menu every few months."
            },
            ["marinate"] = new[]
            {
                "You can marinate the tofu in ginger and soy sauce before baking it.",
                "She lets the chicken marinate overnight in lemon and herbs.",
                "For a stronger flavor, marinate the mushrooms briefly in garlic dressing.",
                "We marinate firm fish in citrus for a fresh, bright flavor."
            },
            ["simmer"] = new[]
            {
                "Let the tomato sauce simmer while the pasta cooks.",
                "The beans need to simmer gently until they are tender.",
                "She left the broth to simmer with ginger and scallions.",
                "Allow the fruit and spices to simmer into a fragrant compote."
            },
            ["saute"] = new[]
            {
                "For the best texture, saute the sliced mushrooms until their edges turn golden.",
                "He will saute the onions before adding the lentils.",
                "To keep their color and texture, saute the greens briefly.",
                "I saute garlic in olive oil to start the pasta sauce."
            },
            ["poach"] = new[]
            {
                "For dessert, poach the pears in spiced tea until they are tender.",
                "She likes to poach eggs for a quick weekend brunch.",
                "At a gentle heat, poach the salmon in court-bouillon with fresh dill.",
                "The cook will poach peaches in a light vanilla syrup."
            },
            ["knead"] = new[]
            {
                "You should knead the dough until it feels elastic and smooth.",
                "He kneads the bread dough on a lightly floured board.",
                "By hand, knead the pasta dough for several minutes to develop its texture.",
                "The baker taught us to knead dough without adding too much flour."
            },
            ["whisk"] = new[]
            {
                "Before pouring them into the pan, whisk the eggs with milk.",
                "She used a fork to whisk the dressing until it emulsified.",
                "To avoid dry pockets, whisk the cocoa into the warm batter.",
                "I whisk olive oil into the lemon juice a little at a time."
            },
            ["dice"] = new[]
            {
                "For a smooth sauce, dice the onion finely so it softens quickly.",
                "He will dice ripe mango for the black bean salsa.",
                "For an even roast, dice the potatoes into pieces of similar size.",
                "The recipe asks you to dice the carrots before adding them to the stew."
            },
            ["grate"] = new[]
            {
                "For a warm aroma, grate fresh nutmeg over the rice pudding.",
                "She likes to grate Parmesan onto the tomato soup.",
                "Before making the fritters, grate the zucchini into the batter.",
                "I grate lemon zest into the cake mixture for a citrus aroma."
            },
            ["drizzle"] = new[]
            {
                "Just before serving, drizzle honey over the baked figs.",
                "He will drizzle olive oil across the roasted vegetables.",
                "A little tahini sauce is enough to drizzle over the falafel.",
                "For a finishing touch, drizzle melted chocolate over the chilled fruit tart."
            },
            ["tender"] = new[]
            {
                "The slow-cooked lamb became tender enough to cut with a spoon.",
                "Simmer the carrots until they are tender but still bright.",
                "A tender piece of fish flakes easily under a fork.",
                "The baker kept the pastry tender by handling it gently."
            },
            ["succulent"] = new[]
            {
                "The succulent roast chicken stayed moist beneath its crisp skin.",
                "We shared succulent peaches picked at the farm that morning.",
                "A succulent pork shoulder needs slow cooking and plenty of time.",
                "The succulent grilled prawns were served with garlic butter."
            },
            ["aromatic"] = new[]
            {
                "The aromatic cardamom and cinnamon warmed the rice pudding.",
                "The aromatic broth filled the kitchen as it slowly cooked.",
                "She crushed aromatic herbs just before stirring them into the sauce.",
                "The aromatic coffee beans gave the cake a rich fragrance."
            },
            ["bland"] = new[]
            {
                "The vegetable soup seemed bland until we added fresh thyme.",
                "A squeeze of lime can rescue bland rice and beans.",
                "He seasoned the bland sauce with roasted garlic and black pepper.",
                "The potatoes tasted bland beside the boldly spiced fish."
            },
            ["tangy"] = new[]
            {
                "The tangy yogurt dressing cut through the richness of the lamb.",
                "She added orange zest to make the salsa more tangy.",
                "A tangy cheese brought a lively flavor to the salad.",
                "The cook served tangy pickled cabbage with the fried fish."
            },
            ["mouthwatering"] = new[]
            {
                "The market stall displayed a mouthwatering tray of cinnamon buns.",
                "A mouthwatering aroma drifted from the bakery's open door.",
                "He prepared a mouthwatering mushroom pie for the holiday meal.",
                "The menu photo showed a mouthwatering bowl of noodles."
            },
            ["wholesome"] = new[]
            {
                "The café serves wholesome grain bowls with seasonal vegetables.",
                "She made a wholesome breakfast of oats, nuts, and fruit.",
                "A wholesome lentil stew kept everyone warm after the hike.",
                "They packed wholesome snacks for the long train journey."
            },
            ["perishable"] = new[]
            {
                "Keep perishable dairy products cold during the trip home.",
                "The market labels each perishable item with its use-by date.",
                "We stored the perishable berries in the refrigerator right away.",
                "Foods that are perishable should not sit on the counter all afternoon."
            },
            ["vegetarian"] = new[]
            {
                "The restaurant serves a vegetarian lasagna with roasted zucchini.",
                "He chose a vegetarian lunch of chickpea soup and flatbread.",
                "A vegetarian filling of mushrooms and lentils made the pie hearty.",
                "Our vegetarian guests enjoyed the grilled vegetable platter."
            },
            ["vegan"] = new[]
            {
                "The bakery makes a vegan chocolate cake with oat milk.",
                "She prepared vegan tacos filled with beans and avocado.",
                "This vegan soup gets its creamy texture from blended white beans.",
                "We ordered a vegan meal with roasted cauliflower and tahini."
            },
            ["condiment"] = new[]
            {
                "Chili crisp is a popular condiment for noodle dishes.",
                "The table offered a condiment made from ginger and scallions.",
                "He chose mustard as the condiment for his roast beef sandwich.",
                "A spoonful of this tangy condiment adds brightness to grilled corn."
            },
            ["fermentation"] = new[]
            {
                "Natural fermentation gives sourdough its airy texture and sharp flavor.",
                "The cook explained how fermentation turns cabbage into sauerkraut.",
                "Slow fermentation helped the dough develop a deeper aroma.",
                "Temperature affects fermentation when you make yogurt at home."
            },
            ["preserve"] = new[]
            {
                "We preserve ripe tomatoes in jars for winter meals.",
                "A little sugar and lemon help preserve the fruit's color.",
                "They preserve fresh herbs in olive oil and freeze the mixture.",
                "Canning is one practical way to preserve a garden harvest."
            },
            ["refrigeration"] = new[]
            {
                "Proper refrigeration keeps milk fresh for longer.",
                "The picnic cooler provided safe refrigeration for the cheese.",
                "Proper refrigeration slows the growth of bacteria in cooked rice.",
                "This dessert needs refrigeration until it is ready to serve."
            },
            ["nutrition"] = new[]
            {
                "Shoppers can use nutrition labels to compare breakfast cereals.",
                "The dietitian discussed nutrition while planning balanced meals.",
                "Good nutrition can come from a varied diet of ordinary foods.",
                "The school improved lunch nutrition by adding fresh vegetables."
            },
            ["delicacy"] = new[]
            {
                "Smoked eel is a local delicacy served during the festival.",
                "The market sells a regional delicacy made with sweet chestnuts.",
                "Visitors often try this seafood delicacy with lemon and herbs.",
                "The chef prepared the traditional delicacy for a special occasion."
            },
            ["casserole"] = new[]
            {
                "The vegetable casserole baked until its cheese topping turned golden.",
                "She brought a chicken casserole to the neighborhood potluck.",
                "A bean casserole makes an easy meal for a busy evening.",
                "We reheated the casserole gently and served it with salad."
            },
            ["vinaigrette"] = new[]
            {
                "A mustard vinaigrette gave the lentil salad a sharp finish.",
                "She shook lemon juice and olive oil into a quick vinaigrette.",
                "The roasted beet salad was dressed with orange vinaigrette.",
                "Pour the vinaigrette over the greens just before serving."
            },
            ["appetizer"] = new[]
            {
                "We shared grilled artichokes as an appetizer before dinner.",
                "The appetizer was a small bowl of chilled cucumber soup.",
                "A platter of olives makes a simple appetizer for guests.",
                "He ordered the shrimp appetizer while choosing a main course."
            },
            ["palatable"] = new[]
            {
                "A squeeze of lemon made the rich bean stew more palatable.",
                "The chef adjusted the bitterness until the greens were palatable.",
                "Roasting the vegetables made their flavor more palatable to the children.",
                "A smooth sauce can make dry grains more palatable."
            },
            ["saturated"] = new[]
            {
                "The nutrition label lists the saturated fat in each serving.",
                "She replaced some butter to reduce saturated fat in the pastry.",
                "Many baked goods contain saturated fat from shortening.",
                "The nutritionist explained how saturated fat appears in dairy foods."
            }
        };

    public static string[] GetAdditionalExamples(string word)
    {
        return word != null && Examples.TryGetValue(word, out var examples)
            ? examples
            : Array.Empty<string>();
    }
}
