// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

static IAsyncEnumerable<SplitMemory<char, char, MatchOne>> Read([Match("^[^<>:\"/\\\\|?*]+$")] string path)
{
    var found = Environment.CurrentDirectory
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn(','));
}

World world = new();
world.Location("Finish a run of WHITE VANILLA");
world.Location("Reach loop 4 of ENDLESS DEFENDER");
world.Location("Reach loop 8 of ENDLESS DEFENDER");
world.Location("Reach loop 12 of ENDLESS DEFENDER");
world.Item("1-1", Priority.ProgressionUseful, world.Category("1-1"), count: 3);

await foreach (var (strat, _) in Read("Locations.csv"))
{
    var category = world.Category(strat.Span.SplitWhitespace().First.ToString());
    var strats = world.Category("Strats");

    var logic = world.Item(strat, Priority.Progression, [category, strats], 2)[1] &
        world.Item(strat.Span.SplitWhitespace().First.ToString(), Priority.ProgressionUseful, category, 3);

    world.Location($"{strat} once", logic, category);
}

await foreach (var (strat, _) in Read("Locations.csv"))
{
    var category = world.AllCategories[strat.Span.SplitWhitespace().First];

    world.Location(
        $"{strat} consistently",
        world.AllItemsWith(category).Select(x => strat == x.Name ? x.All : x[1]).And(),
        category
    );
}

Dictionary<string, int[]> levels = new()
{
    ["1-1"] = [48000, 51000, 52000],
    ["1-2"] = [88000, 105000, 115000],
    ["1-3"] = [160000, 180000, 200000],
    ["1-4"] = [240000, 300000, 356000],
    ["2-1"] = [80000, 100000, 120000],
    ["2-2"] = [120000, 140000, 170000],
    ["2-3"] = [128000, 170000, 200000],
    ["2-4"] = [100000, 130000, 170000],
};

foreach (var (level, scores) in levels)
    for (var i = 0; i < scores.Length; i++)
    {
        var j = i;

        world.Location(
            $"Score at least {scores[i]} in {level}",
            world.AllItems[level][j + 1] &
            world.AllItemsWith(level).Where(x => x.Categories.Contains("Strats")).Select(x => x[j]).And(),
            world.Category(level)
        );
    }

world.Location(
    "Score at least 1.3 million in a full run",
    world.AllItems.Where(x => x.Priority.Has(Priority.Progression)).Select(x => x.All).And(),
    options: LocationOptions.Victory
);

await world.Game("ZeroRangerScoring", "RedsAndEmik", "Enlightenment", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
