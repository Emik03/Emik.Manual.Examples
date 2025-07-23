// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, MA0051, RCS1110
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

const string Minerals =
    """
    Bismor,100,Dense Biozone
    Croppa,100,Azure Weald|Fungus Bogs
    Enor pearl,100,Salt Pits|Sandblasted Corridors
    Jadiz,100,Crystalline Caverns|Hollow Bough
    Magnite,100,Glacial Strata|Magma Core
    Umanite,100,Radioactive Exclusion Zone
    Bittergem,1,Crystalline Caverns&Salt Pits&Fungus Bogs&Radioactive Exclusion Zone&Dense Biozone&Glacial Strata&Hollow Bough&Azure Weald&Magma Core&Sandblasted Corridors
    ERR://23¤Y%/,1,Crystalline Caverns&Salt Pits&Fungus Bogs&Radioactive Exclusion Zone&Dense Biozone&Glacial Strata&Hollow Bough&Azure Weald&Magma Core&Sandblasted Corridors
    """;

static async Task<World> CreateGame(int seed)
{
    World world = new();

    Dictionary<string, List<string>> weaponDictionary = [];
    List<(string Name, ArchipelagoBuilder<Category> Categories)> overclockList = [];
    var overclockCategory = world.Category("Overclocks");
    var biomes = world.Category("Biomes");
    var minerals = world.Category("Minerals");
    ImmutableArray<string> explicitStartingItems = seed is 0 ? ["Deepcore GK2", "Jury-Rigged Boomstick"] : [];

    await foreach (var (line, _) in Read("DeepRockGalacticBiomes.csv"))
        world.Item(line, categories: biomes, early: seed is 0 ? 1 : 0);

    foreach (var (mineral, (amount, logic)) in Minerals.SplitLines().Select(x => x.SplitOn(',')))
    {
        var l = logic.SelectMany(x => x.SplitOn('|')).Select(x => x.SplitOn('&').Select(x => new Logic(x)).And()).Or();
        world.Location(amount.Span is "1" ? $"Get {mineral}" : $"Get {amount} {mineral} total", l, minerals);
    }

    await foreach (var (d, (type, (name, overclocks))) in Read("DeepRockGalacticWeapons.csv"))
    {
        var dwarf = d.ToString();
        weaponDictionary.TryAdd(dwarf, []);

        if (name.ToString() is var nameStr && !explicitStartingItems.Contains(nameStr, StringComparer.Ordinal))
            weaponDictionary[dwarf].AddRange(Enumerable.Repeat(nameStr, type.Span is "Throwables" ? 1 : 6));

        if (seed is not 0)
            overclockList.AddRange(
                overclocks.Select(
                    overclock => ($"{overclock} ({name})",
                        (ArchipelagoBuilder<Category>)[world.Category(name), overclockCategory])
                )
            );
    }

    if (seed is not 0)
        overclockList.Shuffle()
           .Chunk(4)
           .Select(
                x => world.Item(
                    $"Overclock Bundle - {x.Select(x => x.Name).Conjoin()}",
                    categories: [..x.SelectMany(x => x.Categories).DistinctBy(x => x.Name)]
                )
            )
           .Enumerate();

    var fun = world.Category("Fun");
    world.Location("Pet a lootbug", categories: fun);
    world.Location("Drink Hidden Dwarf", categories: fun);
    world.Location("Drink Randomweisser", categories: fun);
    world.Location("Drink Blackout Stout", categories: fun);
    world.Location("Mushroom!", world.AllItems["Fungus Bogs"], fun);
    world.Location("Play soccer and get first to 10", categories: fun);
    world.Location("Kick 4 Barrels inside the Launch Bay", categories: fun);
    world.Location("We're rich!", Logic.Percent(biomes, 50), fun);
    world.Location("Get 1500 Points in the Barrel Throw game", categories: fun);
    world.Location("See a rare structure", world.AllItems["Fungus Bogs"] | world.AllItems["Azure Weald"], fun);
    var weaponStacks = weaponDictionary.Values.Select(x => new Stack<string>(x.Shuffle())).ToArray();
    var weaponCategory = world.Category("Weapons");
    var bundleNumber = 0;

    ImmutableArray<StartingItemBlock> ExplicitStartingItems(ImmutableArray<string> explicitStartingItems) =>
    [
        ..explicitStartingItems.Select(
            x => world.Item(
                x,
                Priority.ProgressionUseful,
                weaponCategory
            )
        ),
    ];

    while (weaponStacks.Any(x => x.Count is not 0))
    {
        var weapons = weaponStacks.Where(x => x.Count is not 0).Take(seed is 0 ? 2 : 4).Select(x => x.Pop()).ToArray();

        world.Item(
            $"Progressive Weapon Bundle {++bundleNumber} - {weapons.Conjoin()}",
            Priority.ProgressionUseful,
            weaponCategory,
            giveItems: [..weapons.Select(x => ((Chars)x, 1))]
        );
    }

    // ReSharper disable once NotAccessedVariable
    Chars previousObjective = default;
    var index = 0;

    await foreach (var (type, (name, _)) in Read("DeepRockGalacticMissions.csv"))
    {
        var times = type.Span switch
        {
            "Warnings" => 2,
            "Milestones" => 1,
            "Objectives" => 4,
            "Eradications" => 1,
            _ => throw new FormatException(type.ToString()),
        };

        Item previousClear = null;

        for (var i = 1; i <= times; i++)
        {
            // ReSharper disable once PossibleLossOfFraction
            var logic = (type.Span is "Objectives" ? Logic.Percent(biomes, index / 2 * 20) : null) &
                (type.Span is "Warnings" or "Eradications"
                    ? Logic.Percent(weaponCategory, type.Span is "Eradications" ? 70 : 40)
                    : null) &
                (type.Span is "Milestones"
                    ? Logic.Percent(weaponCategory, 80) &
                    (seed is 0 ? null : Logic.Percent(overclockCategory, 50))
                    : null) &
                (previousClear.Name.IsEmpty ? (Logic?)null : previousClear);
            // &
            //  (type.Span is "Objectives" && !previousObjective.IsEmpty
            //      ? world.Item($"Beat {previousObjective} for the 1st time")
            //      : (Logic?)null);

            var locationName = times is 1 ? name.ToString() : $"Beat {name} for the {Ordinal(i)} time";
            var options = type.Span is "Milestones" ? LocationOptions.Victory : LocationOptions.None;

            world.Location(
                $"{locationName} LOGIC",
                logic,
                world.Category(type),
                allowList: previousClear = world.Item(locationName)
            );

            world.Location(locationName, logic, world.Category(type), options: options);
        }

        if (type.Span is "Objectives")
        {
            world.Location(
                $"Beat {name} at a high hazard level",
                Logic.Percent(weaponCategory, 60) &
                (seed is 0 ? null : Logic.Percent(overclockCategory, 40))
            );

            // ReSharper disable once RedundantAssignment
            previousObjective = name;
        }

        index++;
    }

    var startingItems = seed is 0 ? ExplicitStartingItems(explicitStartingItems) : [new(weaponCategory, 1)];

    await world.Game($"DeepRockGalactic{seed}", "RedsAndEmik", "Rock and Stone", startingItems)
       .DisplayExported(Console.WriteLine)
       .ZipAsync(Path.GetTempPath(), listChecks: true);

    return world;
}

var worlds = await Task.WhenAll(CreateGame(0), CreateGame(1));
Console.WriteLine($"{worlds.Length} games created.");
