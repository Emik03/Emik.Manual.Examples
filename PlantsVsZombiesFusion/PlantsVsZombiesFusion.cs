// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

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

static bool IsAlwaysPresent(Category x) => x.Yaml.All(x => x.Name.Span is not LongAdventure);

static int SeedSlots(Logic? l) =>
    l switch
    {
        { Type: Logic.Kind.Or } => SeedSlots(l.Left).Max(SeedSlots(l.Right)),
        { Type: Logic.Kind.And } => SeedSlots(l.Left) + SeedSlots(l.Right),
        { Type: Logic.Kind.Item } => 1,
        { Type: Logic.Kind.OptOne or Logic.Kind.OptAll } => SeedSlots(l.Left),
        _ => throw new UnreachableException(l?.ToString()),
    };
#pragma warning disable MA0051
static ImmutableArray<Chars> ToCounteringPlants(ReadOnlyMemory<char> zombie) =>
#pragma warning restore MA0051
    zombie.Span switch
    {
        // ReSharper disable DuplicatedSwitchExpressionArms
        "Armored Gargantuar" => ["Cob-literator"],
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
        "Dolphinhead Football Zombie" => ["Kelp-spreader", "Leviathan-shroom"],
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
        "Whale Rider Yeti" => ["Kelp-spreader", "Leviathan-shroom"],
        "Yeti Zombie" => ["Jalapeno"],
        "Zombarine" => ["Kelp-spreader", "Leviathan-shroom"],
        "Zombie" => ["Peashooter"],
        "Zomboni" => ["Spikeweed"],
        "Zomppelin" => ["Blover", "Cob-literator"],
        _ => throw new FormatException(zombie.ToString()),
    };

Dictionary<string, ImmutableArray<ReadOnlyMemory<char>>> itemRequirements = new(StringComparer.Ordinal);
HashSet<string> odysseyPlants = new(StringComparer.Ordinal) { "Queen Endoflame", "Snipea" };

bool CanPlaceOn(ReadOnlyMemory<char> plant, Terrain terrain, bool isOdyssey) =>
    Expand(plant)
       .All(
            x => x.Span switch
            {
                "Potato Mine" => terrain is not Terrain.Pool and not Terrain.Fog,
                "Cattail Girl" or "Nyan Squash" or "Lily Pad" or "Tangle Kelp" or "Sea-shroom"
                    => terrain is Terrain.Pool or Terrain.Fog,
                _ when !isOdyssey => !odysseyPlants.Contains(plant.Span.ToString()),
                _ => true,
            }
        );

IEnumerable<Chars> Expand(Chars c) => itemRequirements[c.ToString()].SelectMany(x => Expand(x)).Prepend(c);

World world = new();

world.Location(
    "Take care of your Zen Garden!",
    categories: [world.Category("Mini-games"), world.Category("Start", [LongAdventure])]
);

ImmutableArray<string> minigames = ["Beghouled", "Vasebreaker", "Vasebreaker 2", "Chain Reaction"];

foreach (var minigame in minigames)
{
    world.Location($"{minigame} - Clear", categories: world.Category("Mini-games"));
    world.Location($"{minigame} - Trophy", categories: world.Category("Mini-games"));
}

HashSet<string> notStrictlyNecessary = new(StringComparer.Ordinal);

await foreach (var element in Read("NotStrictlyNecessary.csv"))
    notStrictlyNecessary.Add(element.ToString());

await foreach (var (basic, (type, (count, (version, (name, requires))))) in Read("Plants.csv"))
{
    var nameStr = name.ToString();

    var priority = nameStr switch
    {
        "Imitater" => Priority.Progression,
        "Progressive Lawnmowers" => Priority.Useful,
        "Progressive Seed Slots" => Priority.ProgressionUseful,
        _ when type.Span is "Traps" or "Tough Traps" => Priority.Trap,
        _ when basic.Span is "Basic" && type.Span is not "Tools" and not "Pickups"
            => notStrictlyNecessary.Contains(name.ToString()) ? Priority.Useful : Priority.ProgressionUseful,
        _ => notStrictlyNecessary.Contains(name.ToString()) ? Priority.Useful : Priority.Progression,
    };

    ImmutableArray<Yaml> yaml = type.Span is not ("Tools" or "Traps" or "Pickups" or "Weak Odyssey" or "Strong Odyssey")
        ? [LongAdventure]
        : [];

    ArchipelagoArrayBuilder<Category> categories = world.Category($"{basic} ({type})", yaml);

    if (version.Span is "2.8")
        categories.Add(world.Category("2.8", true, ["version_2_8"]));

    world.Item(nameStr, priority, categories, int.Parse(count.Span));
    itemRequirements[nameStr] = [..requires];

    if (type.Span is "Weak Odyssey" or "Strong Odyssey")
        odysseyPlants.Add(nameStr);
}

world.Category("Odyssey Adventure");

await foreach (var (category, (level, (terrain, (waves, (zombies, (plants, _)))))) in Read("Levels.csv"))
{
    Console.WriteLine($"{category}, {level}");
    var c = world.Category(category, [LongAdventure]);
    var count = int.Parse(waves.Span);
    var t = Enum.Parse<Terrain>(terrain.Span);

    var plantLogic = (level.Span is "Level 10"
            ? [["Alchemist Umbrella"]]
            : plants.SplitOn('&').Select(x => ImmutableArray.Create<Chars>(x)))
       .Concat(zombies.SplitOn('&').Select(ToCounteringPlants))
       .Distinct(Equating<ImmutableArray<Chars>>((x, y) => x.SequenceEqual(y)))
       .Where(x => x.All(x => CanPlaceOn(x, t, category.Span is "Odyssey Adventure")))
       .Select(x => x.Select(Expand).Select(x => x.Select(x => world.AllItems[x]).And()).Or())
       .And() &
        ((t is Terrain.Snow ? (Logic)"Firnace" : null) &
            (t is Terrain.Pool or Terrain.Fog ? (Logic)"Lily Pad" : null) &
            (t is Terrain.Roof && level.Span is not "Level 37" ? (Logic)"Flower Pot" : null) &
            (category.Span is "Adventure Mode (Classic)" && level.Span is "Level 1" ? null : (Logic)"Sunflower"));

    var logic = plantLogic &
        (t is Terrain.Fog ? (Logic)"Show Plant HP" & "Show Zombie HP" : null) &
        (category.Span is "Odyssey Adventure" ? (Logic)"Plant Gloves" : null) &
        world.AllItems["Progressive Seed Slots"][SeedSlots(plantLogic).Min(14)];

    var region = world.Region($"{category}, {level}", logic.Opt(), true);

    for (var i = 1; i <= count; i++)
        world.Location($"{category}, {level} - {Ordinal(i)} Flag", null, c, region);

    world.Location($"{category}, {level} - Trophy", null, c, region);
    world.Location($"{category}, {level} - Clear", null, c, region);
}

Console.WriteLine("Odyssey Survival");

ImmutableArray<Chars> goalPlants =
    ["Twin Solar-nut", "Apeacalypse Minigun", "Cob-literator", "Obsidian Tall-nut", "Cherrizilla"];

ImmutableArray<Logic?> goal =
    [..goalPlants.Select(Expand).Select(x => x.Select(x => world.AllItems[x]).Distinct().And())];

var goalRegion = world.Region(
    "Odyssey Survival (Completion)",
    ((Logic)("Progressive Seed Slots", 14)).Opt() &
    ((Logic)"Plant Gloves" & "Show Plant HP" & "Show Zombie HP" & "Shovel"),
    true
);

var goalCategory = world.Category("Odyssey Survival");

for (var i = 0; i < 21; i++)
    world.Location($"Odyssey Survival - {Ordinal(i + 1)} Wave", goal.Take(i / 3).And().Opt(), goalCategory, goalRegion);

world.Location("Odyssey Survival - Trophy", goal.And().Opt(), goalCategory, goalRegion);
world.Location("Odyssey Survival - Clear", goal.And().Opt(), goalCategory, goalRegion);
world.Location("Odyssey Survival - Goal", goal.And().Opt(), goalCategory, goalRegion, LocationOptions.Victory);

// static bool Contains(Logic? logic, Item item) =>
//     logic is not null && (logic.Name.Equals(item.Name) || Contains(logic.Left, item) || Contains(logic.Right, item));
//
// // ReSharper disable once LoopCanBePartlyConvertedToQuery
// foreach (var item in world.AllItems)
//     if (world.AllLocations.All(x => !Contains(x.SelfLogic, item)) &&
//         world.AllRegions.All(x => !Contains(x.SelfLogic, item)))
//         Console.WriteLine(item);

var odysseyLocationCount = world.AllLocations.Count(x => x.Categories.All(IsAlwaysPresent));
var odysseyItemCount = world.AllItems.Sum(x => (x.Categories.All(IsAlwaysPresent) ? 1 : 0) * x.Count);
Console.WriteLine($"Odyssey only: {odysseyLocationCount}/{odysseyItemCount}");

await world.Game("PlantsVsZombiesFusion", "Emik", "Shovel 1 Plant", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);

enum Terrain
{
    Day, Night, Pool, Fog, Roof, Snow,
}
