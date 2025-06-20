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

ImmutableArray<(string Name, int Count)> itemsWithCount =
[
    ("Secrets of the Deep", 11),
    ("Breach the Electric Tower", 10),
    ("Calling for Help", 14),
    ("Reveal the Unseen", 15),
    ("Walk the Path of Destruction", 12),
];

Logic? ParseSpan(ReadOnlyMemory<char> memory) =>
    memory.Span switch
    {
        "Precise Movement" => new Yaml("PreciseMovement").Enabled,
        "null" => null,
        _ when itemsWithCount.Any(x => memory.Span.Equals(x.Name, StringComparison.Ordinal))
            => world.AllCategories[memory].All,
        _ => new(world.AllItems[memory]),
    };

Logic? ParseLogic(ReadOnlyMemory<char> memory) =>
    memory.SplitOn('|').Select(x => x.SplitOn('&').Select(ParseSpan).And()).Or();

ArchipelagoDictionaryValues<string> hints = new([]);
var regionalKeyItems = world.Category("Regional Key Items");

foreach (var (name, count) in itemsWithCount)
    world.Item($"{name} ({count})", categories: [world.Category(name), regionalKeyItems], count: count);

await foreach (var (line, _) in Read("StrawberryLabyrinthEntities.csv"))
    world.Item(line, Priority.Progression | Priority.Useful, world.Category("Entities"));

// 1 excludes 'A01' as a potential starting location, change it to 0 to include it.
var startingLocationIndex = Random.Shared.Next(1, 12);

await foreach (var (room, (hint, (logic, rest))) in Read("StrawberryLabyrinthRooms.csv"))
{
    var l = ParseLogic(logic);
    ImmutableArray<Region> connectsTo = [..rest.Select(x => new Region(x))];
    var isStarting = l is null && connectsTo.Length >= 2 && room.Span is not ['D', ..] && startingLocationIndex-- is 0;
    Region region = new(room, l, connectsTo, isStarting);
    var hasAdded = world.AllRegions.TryAdd(region);
    Trace.Assert(hasAdded);
    hints.TryAdd(room.ToString(), hint.ToString());
}

var maps = world.Item("Found a Map", categories: world.Category("Maps"), count: 4);
var teleporters = world.Item("Found a Teleporter", categories: world.Category("Teleporters"), count: 10);

await foreach (var (room, (type, (logic, (effectiveRegion, _)))) in Read("StrawberryLabyrinthLocations.csv"))
{
    var l = ParseLogic(logic);
    var displayedRegion = room.SplitWhitespace().First;
    var region = world.AllRegions[effectiveRegion.IsEmpty ? displayedRegion : effectiveRegion];
    ArchipelagoListBuilder<Category> categories = [world.Category(type), world.Category(displayedRegion)];
    var hintEntrance = hints[displayedRegion];
    var location = world.Location($"{room} {type}", l, categories, region, hintEntrance: hintEntrance);

    if (type.Span is "Map" or "Teleporter" && (type.Span is "Map" ? maps : teleporters) is var item)
        world.Location($"{location.Name} LOGIC", l, categories, region, allowList: item, hintEntrance: hintEntrance);
}

var endings = world.Category("Endings");

for (var i = 1; i <= 2; i++)
{
    var options = i is 1 ? LocationOptions.Victory : LocationOptions.None;

    world.Location(
        i is 1 ? "Normal End" : "Normal End (Extra)",
        new Logic("Dash Fuse Boxes") & "Dash Refills" & "Springs" & "Touch Switches" & "Zip Lines",
        endings,
        world.AllRegions["C20"],
        options
    );

    world.Location(i is 1 ? "Good End" : "Good End (Extra)", null, endings, world.AllRegions["D09"], options);

    world.Location(
        i is 1 ? "Find all maps" : "Good End (Extra)",
        maps.All,
        [world.AllCategories["Maps"], endings],
        options: options
    );
}

var startingLocations = world.Category("Starting Locations");
world.Location("Start", categories: startingLocations);
world.Location("Do a Screen Transition", categories: startingLocations);

var regionalKeyItemMilestones = world.Category("Regional Key Item Milestones");

itemsWithCount.Aggregate(
    0,
    (a, n) =>
    {
        // var (mid, sum) = (a + n.Count / 2, a + n.Count);
        var sum = a + n.Count;
        // world.Location($"Get {mid} Regional Key Items", regionalKeyItems[mid], regionalKeyItemMilestones);
        world.Location($"Get {sum} Regional Key Items", regionalKeyItems[sum], regionalKeyItemMilestones);
        return sum;
    }
);

world.Location(
    "Find all teleporters",
    teleporters.All,
    [world.AllCategories["Teleporters"], endings],
    options: LocationOptions.Victory
);

world.Location(
    "Get from A01 to any ending without dying",
    world.AllItems["Teleporters"] & world.AllLocations["Normal End"].Logic,
    endings,
    world.AllRegions["D09"],
    LocationOptions.Victory
);

var startingRegion = world.AllRegions.Single(x => x.IsStarting);

var startingItem = world.Item(
    $"Starting location is {startingRegion.Name} ({hints[startingRegion.Name]})",
    Priority.Progression | Priority.Useful,
    world.Category("Starting Location", true)
);

await world.Game("StrawberryLabyrinth", "Emik", "Strawberry", [new(startingItem, 1)])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
