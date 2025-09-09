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

static async IAsyncEnumerable<(SplitMemory<char, char, MatchOne>, SplitMemory<char, char, MatchOne>)> W(
    IAsyncEnumerable<SplitMemory<char, char, MatchOne>> enumerable
)
{
    SplitMemory<char, char, MatchOne> previous = default;

    await foreach (var next in enumerable)
    {
        yield return (previous, next);

        previous = next;
    }

    yield return (previous, default);
}

World world = new();

Logic? ToItem(ReadOnlyMemory<char> next) => world.Item(next, null, world.Category(next.SplitWhitespace().Last), 2)[1];

await foreach (var ((_, (starts, (name, items))), (_, (nStarts, (nName, _)))) in W(Read("Rooms.csv")).Skip(1).Reverse())
    world.Location(
        name,
        null,
        world.Category(name.SplitWhitespace()[..2].Body),
        world.Region(
            name,
            items.Select(ToItem).And(),
            nStarts.IsEmpty || bool.Parse(nStarts.Span) ? default(ArchipelagoArrayBuilder<Region>) : [nName],
            bool.Parse(starts.Span)
        )
    );

await foreach (var (name, (_, (region, _))) in Read("Achievements.csv"))
    world.Location(name, null, world.Category("Achievements"), world.AllRegions[region]);

world.Item("Down Arrows", Priority.Useful, "Arrows", 2);
world.Item("Light Stars", Priority.Useful, "Stars", 2);

world.Location(
    "Credits",
    null,
    world.AllCategories["Core Stage"],
    world.AllRegions["Core Stage Save 7/7 Clear"],
    LocationOptions.Victory
);

await world.Game("IWannaDestroyTheNeedleverse", "Emik", "F2", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
