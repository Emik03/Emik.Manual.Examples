// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

static IAsyncEnumerable<SplitMemory<char, char, MatchOne>> Read([Match("^[^<>:\"/\\\\|?*]+$")] string path)
{
    var found = Path.Join(Environment.CurrentDirectory)
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn(','));
}

World world = new();

ImmutableArray<(string Name, int Count)> arms =
    [("RAPID SHOT", 1), ("SEARCH LASER", 1), ("WAVE", 1), ("LOCK-ON", 4), ("GRAVITY", 2), ("ROUND", 1), ("CLASSIC", 4)];

var armCategory = world.Category("ARMS");
var starterArmCategory = world.Category("STARTER ARMS", true);

foreach (var (name, count) in arms)
    world.Item(
        $"PROGRESSIVE {name} ({count})",
        count is 1 ? Priority.Useful : Priority.Progression,
        [world.Category(name), armCategory, ..count is 1 ? (ReadOnlySpan<Category>)[starterArmCategory] : []],
        count
    );

var settings = world.Category("SETTINGS");
world.Item("TOGGLE COMMENT", Priority.Filler, settings, 7);
var autoShot = world.Item("AUTO SHOT", Priority.Progression, settings);
var stocks = world.Item("PROGRESSIVE STOCKS (+1 LIFE)", null, settings, 8);
var strongestArms = world.Item("LOCK-ON").All | world.Item("CLASSIC").All;
var strongArms = world.Item("GRAVITY").All | strongestArms;

await foreach (var (location, (categoryName, (hintEntrance, (meta, _)))) in Read("InfinityGeneChecks.csv"))
    world.Location(
        location,
        categoryName.Span switch
        {
            "LEVEL 0" => stocks[0],
            "LEVEL 1" => stocks[0],
            "LEVEL 2" => stocks[0],
            "LEVEL 3" => strongArms & stocks[2],
            "EXTRA LEVEL 1" => strongArms & stocks[4],
            "EXTRA LEVEL 2" => strongestArms & stocks[6],
            "EXTRA LEVEL 3" => strongestArms & stocks[8] & autoShot,
            _ => throw new UnreachableException(categoryName.ToString()),
        } &
        (world.AllLocations.TryGetValue(meta, out var inheritedLogic) ? inheritedLogic.Logic : null) &
        world.Item(categoryName, Priority.Progression | Priority.Useful, world.Category("LEVELS")),
        [world.Category(categoryName), world.Category(location)],
        null,
        meta.Span is "VICTORY" ? LocationOptions.Victory : LocationOptions.None,
        hintEntrance: hintEntrance
    );

await world.Game("InfinityGene", "Emik", "EVOLUTION", [world.AllItems["LEVEL 0"], new(starterArmCategory, 1)])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
