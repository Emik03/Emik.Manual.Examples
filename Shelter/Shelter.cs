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
var nodes = world.Category("Nodes");

ArchipelagoArrayBuilder<Category> Categories(ReadOnlyMemory<char> x)
{
    ArchipelagoArrayBuilder<Category> ret = [nodes];

    switch (x.Span)
    {
        case "Node2D":
            ret.Add(world.Category("2D", ["two_dimensional"]));
            break;
        case "Node3D":
            ret.Add(world.Category("3D", ["three_dimensional"]));
            break;
    }

    return ret;
}

await foreach (var (isAbstract, requires) in Read("Nodes.csv"))
{
    Item ToItem(ReadOnlyMemory<char> x, int i) =>
        world.Item(x, categories: [..requires[i..].SelectMany(x => Categories(x)).Distinct()]);

    var logic = requires.Select(ToItem).And();

    if (!bool.Parse(isAbstract.Span))
        world.Location($"Use {requires.First}", logic, [..world.AllItems[requires.First].Categories]);
}

world.Item("Item Detector", Priority.Useful, world.Category("Item Detectors"), count: 5);

var tokens = world.Category("Tokens");
world.Item("3 Mesh Tokens", categories: [tokens, world.AllCategories["3D"]], count: 15);
world.Item("3 Actor Tokens", categories: tokens, count: 15);
world.Item("3 Audio Tokens", categories: tokens, count: 15);
world.Item("3 Image Tokens", categories: tokens, count: 15);
world.Item("3 Script Tokens", categories: tokens, count: 15);
world.Item("3 Shader Tokens", categories: tokens, count: 15);

var bills = world.Category("Pay Checks");
world.Item("25$ Pay Check", Priority.Useful, bills, count: 8);
world.Item("Trap Card", Priority.Trap, world.Category("Traps"), count: 10);

var key = world.Item("Key", categories: world.Category("Keys"), count: 11);

const int ExportedItemCount = 200;
const int ItemCountPerShop = 3;
const int ShopCount = 10;
const int ItemShopCount = ItemCountPerShop * ShopCount;

for (var item = 0; item < ItemShopCount && (int)((float)item / ItemShopCount * ShopCount) + 1 is var count; item++)
    world.Location($"Item #{item % ItemCountPerShop + 1} at Shop #{count}", key[count], world.Category("Shops"));

const int UpgradeStationCount = ExportedItemCount - ItemShopCount;

Dictionary<string, HashSet<string>> mapping = new(StringComparer.Ordinal);

await foreach (var (from, to) in Read("Inheritance.csv"))
    mapping.Add(from.ToString(), to.Select(x => x.ToString()).ToSet());

FrozenSet<string> starting = ["Node", "Node2D", "Node3D", "Sprite3D"];

foreach (var item in world.AllItemsWith(nodes)
   .Omit(x => starting.Contains(x.Name.ToString()))
   .Shuffle()
   .Take(UpgradeStationCount))
    world.Location(
        $"Upgrade Station - {item}",
        mapping[item.Name.ToString()].Select(x => world.AllItems[x]).Or(),
        [..item.Categories],
        allowList: item
    );

world.Item("Ruler Mode", Priority.Filler);
world.Item("Use Grid Snap", Priority.Filler);
world.Item("Lock Selected Nodes", Priority.Filler);
world.Item("Groups", Priority.Useful);
world.Item("Run Current Scene", Priority.Useful);
ImmutableArray<string> editorFeatures = ["Input Map", "Globals", "Move Mode", "Rotate Mode", "Scale Mode"];

Console.WriteLine(
    $"2D: {world.AllLocations.Count(x => x.Categories.IsDefaultOrEmpty || x.Categories.All(x => x.Name.Span is not "3D"))
    }/{world.AllItems.Where(x => x.Categories.IsDefaultOrEmpty || x.Categories.All(x => x.Name.Span is not "3D")).Sum(x => x.Count)}"
);

Console.WriteLine(
    $"3D: {world.AllLocations.Count(x => x.Categories.IsDefaultOrEmpty || x.Categories.All(x => x.Name.Span is not "2D"))
    }/{world.AllItems.Where(x => x.Categories.IsDefaultOrEmpty || x.Categories.All(x => x.Name.Span is not "2D")).Sum(x => x.Count)}"
);

world.Location(
    "Export Game Successfully",
    editorFeatures.Select(x => world.Item(x)).And(),
    world.Category("Goal"),
    options: LocationOptions.Victory
);

ImmutableArray<StartingItemBlock> startingItems =
[
    world.AllItems["Node"],
    new(world.AllItems["Node2D"], Yaml: ["2D"]),
    new(world.AllItems["Node3D"], Yaml: ["3D"]),
    new(world.AllItems["Sprite3D"], Yaml: ["3D"]),
];

await world.Game("Shelter", "KAUTARUMAAndEmik", "25$ Pay Check", startingItems)
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
