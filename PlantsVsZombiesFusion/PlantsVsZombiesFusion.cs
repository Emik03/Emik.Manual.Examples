// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;
using static Emik.Manual.Domains.Priority;

// ReSharper disable HeuristicUnreachableCode RedundantLogicalConditionalExpressionOperand
#pragma warning disable CS0162
[Pure]
static string Ordinal(int i) =>
    $"{i}{(i % 10) switch
    {
        _ when i / 10 % 10 is 1 => "th",
        1 => "st",
        2 => "nd",
        3 => "rd",
        _ => "th",
    }}";

static IAsyncEnumerable<SplitMemory<char, char, MatchOne>> Read([Match("^[^<>:\"/\\\\|?*]+$")] string path)
{
    var found = Environment.CurrentDirectory
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn(','));
}

const string LongAdventure = "long_adventure";

static bool HasCobLiterator(Logic logic) =>
    logic.Left is { } left && HasCobLiterator(left) ||
    logic.Right is { } right && HasCobLiterator(right) ||
    logic.Name.Span is "Cob-literator";

static bool IsAlwaysPresent(Category x) => x.Yaml.All(x => x.Name.Span is not LongAdventure);
#pragma warning disable MA0051
static HashSet<string> ToCounteringPlants(ReadOnlyMemory<char> zombie) =>
#pragma warning restore MA0051
    zombie.Span switch
    {
        // ReSharper disable DuplicatedSwitchExpressionArms
        "Armored Gargantuar" => ["Cherrizilla"],
        "Balloon Zombie" => ["Blover", "Cactus"],
        "Bobsled Zomboni" => ["Spikeweed"],
        "Buck-nut Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Buckethead Zombie" => ["Magnet-shroom", "Split Pea"],
        "Bucknut-copter Zombie" => ["Cob Cannon", "Melon Mortar"],
        "Buckshoot-copter Zombie" => ["Cob Cannon", "Melon Mortar"],
        "Buckshooter Buckethead Zombie" => ["Chompzilla"],
        "Buckshooter Zombie" => ["Chompzilla"],
        "Bungee Zombie" => ["Umbrella Leaf"],
        "Catapult Zombie" => ["Umbrella Leaf"],
        "Cherry-nut Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Cherryshooter Newspaper Zombie" => ["Cherry-nut", "Cherry Pumpkin"],
        "Cherryshooter Zombie" => ["Cherry-nut", "Cherry Pumpkin"],
        "Chronoporter Zombie" => ["Chrono-nut"],
        "Clown-in-the-Box Zombie" => ["Gatling Pea"],
        "Clown-in-the-Pogo Zombie" => ["Umbrella Rind"],
        "Conehead Zombie" => ["Repeater"],
        "Diamond-box Zombie" => ["Gatling Pea"],
        "Digger Zombie" => ["Split Pea", "Starfruit"],
        "Dolphin Rider Zombie" => ["Tangle Kelp"],
        "Dolphinhead Football Zombie" => ["Kelp-spreader", "Kraken-shroom"],
        "Elder Snowfur" => ["Twin Saw-me-not"],
        "Explod-o-pult Zombie" => ["Umbrella Leaf"],
        "Explod-o-shooter Zombie" => ["Cherry-nut", "Cherry Pumpkin"],
        "Football Zombie" => ["Gatling Pea"],
        "Football-nut Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Frost Legion Blockhead Zombie" => ["Spruce Sharpshooter"],
        "Frost Legion Shieldbearer" => ["Spruce Sharpshooter"],
        "Frost Legion Snowball Launcher" => ["Snow Lotus", "Umbrella Rind"],
        "Frost Legion Trident Thrower" => ["Chompzilla"],
        "Frost-nut Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Furling" => ["Saw-me-not"],
        "Gargantuar" => ["Titan Pea Turret"],
        "Gatling Cherry Newspaper Zombie" => ["Cherry-nut", "Cherry Pumpkin"],
        "Gatling Explod-o-rider Zombie" => ["Cherry-nut", "Cherry Pumpkin"],
        "Giga Buckshot Commando" => ["Obsidian Tall-nut", "Cherrizilla"],
        "Giga Football-nut Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Giga Mecha-nut" => ["Umbrella Rind"],
        "Giga Trident-nut Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Giga-gargantuar" => ["Titan Pea Turret"],
        "Grounded Sharkmarine" => ["Gatling Cherry", "Blazer Snipea"],
        "Ice-borg Executrix Mk. Alpha" => ["Spruce Supershooter"],
        "Jack-in-the-Box Zombie" => ["Gatling Pea"],
        "Jackson Worldwide" => ["Cob-literator"],
        "Jalapeno Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Kirov Zomppelin" => ["Cob-literator"],
        "Ladder Zombie" => ["Magnet-shroom"],
        "Mecha Gargantuar" => ["Magnet-shroom", "Titan Pea Turret"],
        "Mecha Giga-gargantuar" => ["Magnet-shroom", "Titan Pea Turret"],
        "Mecha-nut Zombie" => ["Umbrella Leaf"],
        "Michael Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Michael Zomboni" => ["Titan Pea Turret", "Chomper Maw"],
        "Newspaper Zombie" => ["Split Pea"],
        "Nian Zombie" => ["Bamboom"],
        "Pea Commando Football" => ["Obsidian Tall-nut", "Cherrizilla"],
        "Peashooter Zombie" => ["Chompzilla"],
        "Pogo Melon Zombie" => ["Umbrella Leaf"],
        "Pogo Zombie" => ["Magnet-shroom", "Umbrella Leaf"],
        "Pole Vaulting Zombie" => ["Repeater"],
        "Professor Cherryz" => ["Obsidian Spike-nut"],
        "Queen Jill-in-the-Box" => ["Spicicle"],
        "Scholar Zombie" => ["Gatling Pea"],
        "Screen Door Buckshooter Zombie" => ["Magnet Blover"],
        "Screen Door Zombie" => ["Magnet-shroom"],
        "Skystrider Mecha" => ["Spicicle"],
        "Snorkel Zombie" => ["Squash", "Nyan Squash"],
        "Snowfur" => ["Saw-me-not"],
        "Superstar Zombie" => ["Spicicle"],
        "Trident Hwacha" => ["Umbrella Leaf"],
        "Trident Zombie" => ["Chompzilla"],
        "Ultra Mecha-nut" => ["Aegis Umbrella"],
        "Undying Wraith" => ["Titan Pea Turret"],
        "Wall-nut Zombie" => ["Gatling Cherry", "Blazer Snipea"],
        "Whale Rider Yeti" => ["Kelp-spreader", "Kraken-shroom"],
        "Yeti Zombie" => ["Jalapeno"],
        "Zombarine" => ["Kelp-spreader", "Kraken-shroom"],
        "Zombie" => ["Peashooter"],
        "Zomboni" => ["Spikeweed"],
        "Zomppelin" => ["Blover", "Cob-literator"],
        _ => throw new FormatException(zombie.ToString()),
    };

HashSet<string> early = ["Plant Gloves", "Zombie Gloves", "Hammer", "Shovel", "Peashooter", "Sunflower"];
Dictionary<string, ReadOnlyMemory<char>> itemRequirements = new(StringComparer.Ordinal);
HashSet<string> odysseyPlants = ["Queen Endoflame", "Snipea"];
World w = new();

bool CanPlaceOn(Logic? plant, Terrain terrain, bool isOdyssey) =>
    plant is null ||
    CanPlaceOn(plant.Left, terrain, isOdyssey) ||
    CanPlaceOn(plant.Right, terrain, isOdyssey) ||
    plant.Name.Span switch
    {
        "Potato Mine" => terrain is not Terrain.Pool and not Terrain.Fog,
        "Cattail Girl" or "Nyan Squash" or "Lily Pad" or "Tangle Kelp" or "Sea-shroom"
            => terrain is Terrain.Pool or Terrain.Fog,
        var x when !isOdyssey => !odysseyPlants.Contains(x.ToString()),
        _ => true,
    };

Logic Expand(string c) =>
    c &
    itemRequirements[c]
       .SplitOn('|')
       .Select(x => x.SplitOn('&').Select(x => Expand(x.ToString())).And())
       .Or();

Logic WithModifiers(Logic x) =>
    HasCobLiterator(x)
        ? x &
        w.AllItems["Cob-literator; Reload Time > 12 Seconds"] &
        w.AllItems["Cob-literator; Main Blast damage ×4"]
        : x;

IEnumerable<string> Recurse(string x) =>
    itemRequirements[x].SplitOnAny("|&").SelectMany(x => Recurse(x.ToString())).Prepend(x);

await foreach (var (basic, (type, (count, (version, (name, (requires, rest)))))) in Read("Plants.csv"))
{
    if (!rest.Body.IsEmpty)
        throw new UnreachableException($"Unexpected parameter: {rest}");

    var nameStr = name.ToString();
    itemRequirements[nameStr] = requires;

    var priority = nameStr switch
    {
        _ when basic.Span is "Basic" && type.Span is not "Tools" => ProgressionUseful,
        _ when type.Span is "Traps" => Trap,
        _ => Progression,
    };

    ImmutableArray<Yaml> yaml = type.Span is not ("Tools" or "Traps" or "Regular Odyssey" or "Strong Odyssey")
        ? [LongAdventure]
        : [];

    ArchipelagoArrayBuilder<Category> categories = w.Category($"{basic} ({type})", true, yaml);

    if (version.Span is "2.8" or "2.8.2")
        categories.Add(w.Category("2.8", true, ["version_2_8"]));

    var c = int.Parse(count.Span);
    w.Item(nameStr, priority, categories, c, early: early.Contains(name.ToString()) ? c : 0);

    if (basic.Span is "Fusion" && type.Span is "Regular Odyssey" or "Strong Odyssey")
        odysseyPlants.Add(nameStr);
}

w.Category("Garden Defense");
w.Category("Odyssey Adventure");

await foreach (var (category, (level, (terrain, (waves, (zombies, (plants, _)))))) in Read("Levels.csv"))
{
    Console.WriteLine($"{category}, {level}");
    var c = w.Category(category, [LongAdventure]);
    var count = int.Parse(waves.Span);
    var t = Enum.Parse<Terrain>(terrain.Span);

    var logic = plants.SplitOn('&')
           .Select(x => (HashSet<string>)[x.ToString()])
           .Concat(zombies.SplitOn('&').Select(ToCounteringPlants))
           .Distinct(Equating<HashSet<string>>((x, y) => x is null ? y is null : y is not null && x.SetEquals(y)))
           .Where(x => x.All(x => CanPlaceOn(Expand(x), t, category.Span is "Odyssey Adventure")))
           .Select(x => x.Select(Expand).Or())
           .And() &
        ((t is Terrain.Snow ? (Logic)"Firnace" : null) &
            (t is Terrain.Pool or Terrain.Fog ? (Logic)"Lily Pad" : null) &
            (t is Terrain.Roof && level.Span is not "Level 37" ? (Logic)"Flower Pot" : null) &
            (category.Span is "Garden Defense" or "Garden Defense (Long)" ? null : (Logic)"Sunflower")) &
        (category.Span is "Odyssey Adventure" ? (Logic)"Plant Gloves" : null);

    var region = w.Region($"{category}, {level}", logic.Opt(), true);

    for (var i = 1; i <= count; i++)
        w.Location($"{category}, {level} - {Ordinal(i)} Flag", null, c, region);

    w.Location($"{category}, {level} - Trophy", null, c, region);
    w.Location($"{category}, {level} - Clear", null, c, region);
}

ImmutableArray<string> goalPlants = ["Twin Solar-nut", "Gatling Icicle-shroom", "Cherrizilla", "Cob-literator"];
ImmutableArray<Logic?> goal = [..goalPlants.Select(Expand).Select(WithModifiers)];

Console.WriteLine("Odyssey Rush Mode");
var goals = w.Category("Goals");
var odysseyGoal = w.Region("Odyssey Goal", goal.And().Opt());
var odyssey = w.Region("Odyssey", (Logic)"Shovel" & "Plant Gloves", odysseyGoal, true);
// var odysseyRushMode = w.Category("Odyssey Rush Mode", [LongAdventure]);

// for (var i = 0; i < 7 && i >= goal.Length is var reuseOdysseyGoal; i++)
//     w.Location(
//         $"Odyssey Rush Mode - {Ordinal(i + 1)} Wave",
//         reuseOdysseyGoal ? null : goal.Take(i).And().Opt(),
//         odysseyRushMode,
//         reuseOdysseyGoal ? odysseyGoal : odyssey
//     );
//
// w.Location("Odyssey Rush Mode - Trophy", null, odysseyRushMode, odysseyGoal);
// w.Location("Odyssey Rush Mode - Clear", null, odysseyRushMode, odysseyGoal);
// w.Location("Odyssey Rush Mode - Goal", null, goals, odysseyGoal, LocationOptions.Victory);

Console.WriteLine("Odyssey Survival");
var odysseySurvival = w.Category("Odyssey Survival");

for (var i = 0; i < 21 && i / 3 >= goal.Length is var reuseOdysseyGoal; i++)
    w.Location(
        $"Odyssey Survival - {Ordinal(i + 1)} Wave",
        reuseOdysseyGoal ? null : goal.Take(i / 3).And().Opt(),
        odysseySurvival,
        reuseOdysseyGoal ? odysseyGoal : odyssey
    );

w.Location("Odyssey Survival - Trophy", null, odysseySurvival, odysseyGoal);
w.Location("Odyssey Survival - Clear", null, odysseySurvival, odysseyGoal);
w.Location("Odyssey Survival - Goal", null, goals, odysseyGoal, LocationOptions.Victory);

static bool Has(Logic? logic, Item item) =>
    logic is not null && (logic.Name.Equals(item.Name) || Has(logic.Left, item) || Has(logic.Right, item));

Console.WriteLine("Balancing priorities...");
World world = new();

bool AddItemWithInferredPriority(Item i)
{
    string Count((string Key, HashSet<string> Value) x) =>
        x.Value.Intersect(odysseyPlants, StringComparer.Ordinal).Count() is var odysseyCount &&
        odysseyCount is 0 ||
        odysseyCount == x.Value.Count
            ? x.Value.Count.ToString()
            : $"{odysseyCount}/{x.Value.Count}";

    string CategoryCounter((string Key, HashSet<string> Value) x) =>
        $"{w.AllItems[x.Key].Categories[0].Name} - {x.Key} ({Count(x)})";

    return world.AllItems.TryAdd(
        i with
        {
            Categories =
            [
                ..itemRequirements.Keys.Select(x => (Key: x, Value: Recurse(x).ToHashSet(StringComparer.Ordinal)))
                   .Where(x => x.Value.Count > 1 && x.Value.Contains(i.Name.ToString()))
                   .Select(CategoryCounter)
                   .Select(x => world.Category(x)),
                ..i.Categories,
            ],
            Priority = w.AllLocations.All(y => !Has(y.SelfLogic, i)) && w.AllRegions.All(y => !Has(y.SelfLogic, i))
                ? (i.Priority.Has(Progression) ? Useful : default) | ~Progression & i.Priority
                : i.Priority,
        }
    );
}

if (!w.AllCategories.All(world.AllCategories.TryAdd) |
    !w.AllLocations.All(world.AllLocations.TryAdd) |
    !w.AllRegions.All(world.AllRegions.TryAdd) |
    !w.AllItems.All(AddItemWithInferredPriority))
    throw new UnreachableException("Could not copy values");

var odysseyLocationCount = world.AllLocations.Count(x => x.Categories.All(IsAlwaysPresent));
var odysseyItemCount = world.AllItems.Sum(x => (x.Categories.All(IsAlwaysPresent) ? 1 : 0) * x.Count);
Console.WriteLine($"Odyssey only: {odysseyLocationCount}/{odysseyItemCount}");

await world.Game("PlantsVsZombiesFusion", "Emik", "Shovel 1 Plant", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);

enum Terrain { Day, Night, Pool, Fog, Roof, Snow }
