// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, MA0051, RCS1110
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

const int MinimumGates = 4, MaximumGates = 8;

// ReSharper disable UnusedVariable
const int A = 5, Ab = 7, Abc = 10, Abcd = 13;

// 1 excludes 'A01' as a potential starting location, change it to 0 to include it.
// The upper bound can be 12 to include Voltic Citadel as a starting location.
var startingLocationIndex = Random.Shared.Next(1, A);

ImmutableArray<string> gates =
[
    "Secrets of the Deep (West)",
    "Secrets of the Deep (East)",
    "Breach the Electric Tower (West)",
    "Breach the Electric Tower (East)",
    "Calling for Help",
    "Reveal the Unseen",
    "Walk the Path of Destruction",
];

foreach (var gate in gates)
    world.Item($"{gate} ({MinimumGates})", categories: [world.Category(gate)], count: MaximumGates);

Logic? ParseSpan(ReadOnlyMemory<char> memory) =>
    memory.Span switch
    {
        "null" => null,
        "Precise Movement" => new Yaml("PreciseMovement").Enabled,
        _ when gates.Any(x => memory.Span.Equals(x, StringComparison.Ordinal))
            => world.AllItems[$"{memory} ({MinimumGates})"][MinimumGates],
        _ => new(world.AllItems[memory]),
    };

Logic? ParseLogic(ReadOnlyMemory<char> memory) =>
    memory.SplitOn('|').Select(x => x.SplitOn('&').Select(ParseSpan).And()).Or();

ArchipelagoDictionaryValues<string> hints = new([]);

await foreach (var (line, _) in Read("StrawberryLabyrinthEntities.csv"))
    world.Item(line, Priority.Progression | Priority.Useful, world.Category("Entities"));

await foreach (var (room, (hint, (logic, rest))) in Read("StrawberryLabyrinthRooms.csv"))
{
    var l = ParseLogic(logic);
    ImmutableArray<Region> connectsTo = [..rest.Select(x => new Region(x))];
    var isStartingCandidate = l is null && connectsTo.Length >= 2;
    var isStarting = l is null && connectsTo.Length >= 2 && startingLocationIndex-- is 0;
    Region region = new(room, l, connectsTo, isStarting);
    var hasAdded = world.AllRegions.TryAdd(region);
    Trace.Assert(hasAdded);
    hints.TryAdd(room.ToString(), hint.ToString());
}

await foreach (var (room, (type, (logic, (effectiveRegion, _)))) in Read("StrawberryLabyrinthLocations.csv"))
{
    var l = ParseLogic(logic);
    var displayedRegion = room.SplitWhitespace().First;
    var region = world.AllRegions[effectiveRegion.IsEmpty ? displayedRegion : effectiveRegion];
    ArchipelagoListBuilder<Category> categories = [world.Category(type), world.Category(displayedRegion)];
    var hintEntrance = hints[displayedRegion];
    world.Location($"{room} {type}", l, categories, region, hintEntrance: hintEntrance);
}

var endings = world.Category("Endings");

foreach (var b in (ImmutableArray<bool>)[true, false])
{
    var options = b ? LocationOptions.Victory : LocationOptions.None;

    world.Location(
        b ? "Normal End" : "Normal End (Extra)",
        new Logic("Dash Fuse Boxes") & "Dash Refills" & "Springs" & "Touch Switches" & "Zip Lines",
        endings,
        world.AllRegions["C20"],
        options
    );

    world.Location(b ? "Good End" : "Good End (Extra)", null, endings, world.AllRegions["D09"], options);
}

var startingRegion = world.AllRegions.Single(x => x.IsStarting);

var startingItem = world.Item(
    $"Starting location is {startingRegion.Name} ({hints[startingRegion.Name]})",
    Priority.Progression | Priority.Useful,
    world.Category("Starting Location")
);

await world.Game("StrawberryLabyrinth", "Emik", "Strawberry", [new(startingItem, 1)])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
