// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

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
    var found = Path.Join(Environment.CurrentDirectory)
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn(','));
}

static (int Seeds, int Lawnmowers) Amounts(ReadOnlySpan<char> category, ReadOnlySpan<char> level, Terrain terrain) =>
    category switch
    {
        "Adventure Mode (Classic)" => level switch
        {
            "Level 1" => (1, 0),
            "Level 27" => (0, 2),
            "Level 36" => (0, 3),
            "Level 45" => (0, 4),
            _ => (level.SplitWhitespace().Last.Into<int>().Min(9), (int)terrain),
        },
        "Adventure Mode (Snow)" => (level is "Level 9" ? 0 : level.SplitWhitespace().Last.Into<int>() / 2 + 9, 5),
        "Odyssey Adventure" => level switch
        {
            "Level 4" => (4, 0),
            _ => (level.SplitWhitespace().Last.Into<int>() / 3 + 8, (int)terrain / 3 + 4),
        },
        _ => (12, (int)terrain / 2 + 3),
    };
#pragma warning disable MA0051
static ImmutableArray<Chars> ToCounteringPlants(ReadOnlyMemory<char> zombie) =>
#pragma warning restore MA0051
    zombie.Span switch
    {
        "Armored Gargantuar" or "Kirov Zomppelin" => ["Cob-literation"],
        "Balloon Zombie" => ["Blover", "Cactus", "Cob Cannon", "Cattail Girl", "Cattail", "Melon Mortar"],
        "Bobsled Zomboni" or "Michael Zomboni" =>
        [
            "Chomper", "Gatling Pea", "Gloom-shroom", "Frenzy-shroom", "Scorched Threepeater", "Spikeweed",
            "Lumos Cactus", "Doom Cactus", "Starfruit", "Cracked Melon", "Corn-pult", "Winter Melon", "Summer Melon",
            "Cattail Girl", "Swordsage Starfruit/Swordmaster Starfruit", "Cattail", "Squash-nut", "Titan Pea Turret",
            "Chomper Maw",
        ],
        "Buck-nut Zombie" or
            "Cherry-nut Zombie" or
            "Clown-in-the-Box Zombie" or
            "Diamond-box Zombie" or
            "Football Zombie" or
            "Football-nut Zombie" or
            "Frost Legion Blockhead Zombie" or
            "Frost Legion Shieldbearer" or
            "Frost-nut Zombie" or
            "Grounded Sharkmarine" or
            "Jack-in-the-Box Zombie" or
            "Jalapeno Zombie" or
            "Michael Zombie" or
            "Peashooter Zombie" or
            "Scholar Zombie" or
            "Wall-nut Zombie" =>
            [
                "Gatling Pea", "Gloom-shroom", "Spike-ice", "Spike-hearth", "Cracked Melon", "Corn-pult",
                "Winter Melon", "Summer Melon", "Cattail", "Titan Pea Turret", "Chomper Maw",
            ],
        "Buckethead Zombie" =>
        [
            "Magnet-shroom", "Gatling Pea", "Gloom-shroom", "Spike-ice", "Spike-hearth", "Starfruit",
            "Cracked Melon", "Corn-pult", "Winter Melon", "Summer Melon", "Swordsage Starfruit/Swordmaster Starfruit",
            "Cattail", "Titan Pea Turret", "Chomper Maw",
        ],
        "Buckshooter Zombie" or "Buckshooter Buckethead Zombie" =>
        [
            "Gatling Pea", "Gloom-shroom", "Spike-ice", "Spike-hearth", "Starfruit", "Cracked Melon", "Corn-pult",
            "Winter Melon", "Summer Melon", "Swordsage Starfruit/Swordmaster Starfruit", "Cattail",
            "Titan Pea Turret", "Chomper Maw",
        ],
        "Bucknut-copter Zombie" or "Buckshoot-copter Zombie" => ["Cob Cannon", "Melon Mortar"],
        "Bungee Zombie" => ["Umbrella Leaf", "Chomper Maw"],
        "Catapult Zombie" => ["Umbrella Leaf", "Titan Pea Turret", "Chomper Maw"],
        "Cherryshooter Newspaper Zombie" or
            "Cherryshooter Zombie" or
            "Explod-o-shooter Zombie" or
            "Gatling Cherry Newspaper Zombie" or
            "Gatling Explod-o-rider Zombie" => ["Cherry-nut", "Cherry Pumpkin"],
        "Chronoporter Zombie" => ["Chrono-nut"],
        "Clown-in-the-Pogo Zombie" or "Giga Mecha-nut" =>
        [
            "Umbrella Kale", "Umbrella Husk", "Umbrella Rind", "Lumos Umbrella", "Umbrella Thorn", "Umbrella Shell",
            "Bombrella", "Cherrizilla",
        ],
        "Conehead Zombie" or
            "Zombie" or
            "Frost Legion Trident Thrower" or
            "Newspaper Zombie" or
            "Pole Vaulting Zombie" or
            "Trident Zombie" =>
            [
                "Peashooter", "Puff-shroom", "Fume-shroom", "Scaredy-shroom", "Threepeater", "Spikeweed", "Cactus",
                "Starfruit", "Cabbage-pult", "Kernel-pult", "Melon-pult", "Saw-me-not", "Spruce Sharpshooter",
                "Aloe Aqua", "Barley/Obsidian Barley", "Cattail Girl", "Swordsage Starfruit/Swordmaster Starfruit",
                "Cattail", "Queen Endoflame", "Amp-nion", "Doubleblast Passionfruit", "Pearmafrost", "Icetip Lily",
                "Squash-nut", "Bombrella", "Chomper Maw",
            ],
        "Digger Zombie" => ["Potato Mine", "Magnet-shroom", "Split Pea", "Starfruit", "Cattail Girl", "Cattail"],
        "Dolphin Rider Zombie" or "Dolphinhead Football Zombie" =>
            ["Tall-nut", "Kelp-spreader", "Leviathan-shroom", "Titan Pea Turret", "Chomper Maw"],
        "Elder Snowfur" => ["Twin Saw-me-not", "Titan Pea Turret", "Chomper Maw"],
        "Explod-o-pult Zombie" or "Mecha-nut Zombie" or "Pogo Melon Zombie" or "Trident Hwacha"
            => ["Umbrella Leaf"],
        "Frost Legion Snowball Launcher" => ["Snow Lotus", "Umbrella Rind", "Titan Pea Turret"],
        "Furling" => [],
        "Gargantuar" or
            "Giga Football-nut Zombie" or
            "Giga-gargantuar" or
            "Undying Wraith" or
            "Giga Trident-nut Zombie" =>
            [
                "Gatling Pea", "Cracked Melon", "Corn-pult", "Winter Melon", "Summer Melon", "Titan Pea Turret",
                "Chomper Maw", "Cherrizilla",
            ],
        "Giga Buckshot Commando" or "Pea Commando Football" => ["Obsidian Tall-nut", "Cherrizilla"],
        "Ice-borg Executrix Mk. Alpha" => ["Spruce Supershooter"],
        "Jackson Worldwide" =>
        [
            "Cherrizilla", "Cob-literation", "Titan Apeacalypse Minigun", "Helios Cabbage", "Quantum McCornics",
            "Empress-shroom", "Boreal Lifereaver", "Tyrannoflora Lux",
        ],
        "Ladder Zombie" or "Screen Door Zombie" => ["Magnet-shroom"],
        "Mecha Gargantuar" or "Mecha Giga-gargantuar" =>
        [
            "Magnet-shroom", "Gatling Pea", "Cracked Melon", "Corn-pult", "Winter Melon", "Summer Melon",
            "Titan Pea Turret", "Chomper Maw",
        ],
        "Nian Zombie" => ["Bamboom"],
        "Pogo Zombie" => ["Magnet-shroom", "Umbrella Leaf"],
        "Professor Cherryz" => ["Obsidian Spike-nut"],
        "Queen Jill-in-the-Box" or "Skystrider Mecha" or "Superstar Zombie" => ["Unstable Jalapeno"],
        "Screen Door Buckshooter Zombie" => ["Magnet Blover"],
        "Snorkel Zombie" => ["Squash", "Nyan Squash"],
        "Snowfur" => ["Saw-me-not"],
        "Ultra Mecha-nut" => ["Aegis Umbrella"],
        "Whale Rider Yeti" =>
            ["Kelp-spreader", "Leviathan-shroom", "Titan Pea Turret", "Chomper Maw"],
        "Yeti Zombie" => ["Jalapeno", "Torchwood", "Ice-shroom"],
        "Zombarine" => ["Kelp-spreader", "Leviathan-shroom", "Titan Pea Turret", "Chomper Maw"],
        "Zomboni" => ["Gatling Pea", "Spikeweed", "Snow Pea", "Titan Pea Turret"],
        "Zomppelin" => ["Blover", "Cob-literation"],
        _ => throw new FormatException(zombie.ToString()),
    };

Dictionary<string, ImmutableArray<ReadOnlyMemory<char>>> itemRequirements = new(StringComparer.Ordinal);
HashSet<string> odysseyPlants = new(StringComparer.Ordinal);

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

IEnumerable<Chars> Expand(ReadOnlyMemory<char> c) => itemRequirements[c.ToString()].SelectMany(Expand).Prepend(c);

World world = new();
world.Location("Beat Vasebreaker", categories: world.Category("Vasebreaker"));
world.Location("Beat Vasebreaker 2", categories: world.Category("Vasebreaker"));

await foreach (var (basic, (type, (name, requires))) in Read("Plants.csv"))
{
    var (count, priority) = name.Span switch
    {
        "Progressive Lawnmowers" => (6, Priority.ProgressionUseful),
        "Progressive Seed Slots" => (14, Priority.ProgressionUseful),
        _ when basic.Span is "Basic" && type.Span is not "Tools" and not "Pickups"
            => (2, Priority.ProgressionUseful),
        _ => (1, Priority.Progression),
    };

    world.Item(name, priority, world.Category($"{basic} ({type})"), count);
    itemRequirements[name.ToString()] = [..requires];

    if (type.Span.Contains("Odyssey", StringComparison.Ordinal))
        odysseyPlants.Add(name.Span.ToString());
}

await foreach (var (category, (level, (terrain, (waves, (zombies, (plants, _)))))) in Read("Levels.csv"))
{
    var count = int.Parse(waves.Span);
    var t = Enum.Parse<Terrain>(terrain.Span);
    var c = world.Category(category);

    Logic? ToLogic(ImmutableArray<Chars> x) =>
        x.Where(x => CanPlaceOn(x, t, category.Span.Contains("Odyssey", StringComparison.Ordinal)))
           .Select(x => Expand(x).Select(x => world.AllItems[x]).And())
           .Distinct()
           .Or();

    var (seeds, lawnmowers) = Amounts(category.Span, level.Span, t);

    var logic = plants.SplitOn('&').SelectMany(Expand).Select(x => world.AllItems[x]).Distinct().And() &
        (plants.Span.Contains('&')
            ? null
            : zombies.SplitOn('&')
               .Select(ToCounteringPlants)
               .Distinct(Equating<ImmutableArray<Chars>>((x, y) => x.SequenceEqual(y)))
               .Select(ToLogic)
               .And()) &
        ("Progressive Seed Slots", seeds) &
        ("Progressive Lawnmowers", lawnmowers) &
        (t is Terrain.Snow ? (Logic)"Firnace" : null) &
        (t is Terrain.Pool or Terrain.Fog ? (Logic)"Lily Pad" : null) &
        (t is Terrain.Fog ? (Logic)"Show Plant HP" & "Show Zombie HP" : null) &
        (category.Span is "Odyssey Adventure" ? (Logic)"Plant Gloves" : null) &
        (t is Terrain.Roof && level.Span is not "Level 37" ? (Logic)"Flower Pot" : null) &
        (category.Span is "Adventure Mode (Classic)" && level.Span is "Level 1" ? null : (Logic)"Sunflower");

    for (var i = 1; i <= count; i++)
        world.Location($"{category}, {level} - {Ordinal(i)} Flag", logic, c);

    world.Location($"{category}, {level} - Clear", logic, c);
    world.Location($"{category}, {level} - Trophy", logic, c);
}

world.Location(
    "Beat Odyssey Survival",
    odysseyPlants.Select(x => x.AsMemory()).SelectMany(Expand).Select(x => world.AllItems[x]).Distinct().And() &
    ("Progressive Seed Slots", 14) &
    ("Progressive Lawnmowers", 6) &
    "Plant Gloves" &
    "Zombie Gloves" &
    "Slow Mode" &
    "Show Plant HP" &
    "Show Zombie HP" &
    "Hammer" &
    "Shovel" &
    "Fertilizer" &
    "Coins",
    world.Category("Odyssey Adventure"),
    options: LocationOptions.Victory
);

await world.Game("PlantsVsZombiesFusion", "Emik", "Are You Sure?", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);

enum Terrain
{
    Day, Night, Pool, Fog, Roof, Snow,
}
