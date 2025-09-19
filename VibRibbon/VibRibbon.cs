// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, MA0051, RCS1110
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
Dictionary<string, IList<Item>> lookup = [];
IList<ReadOnlyMemory<char>> varietySongs = [];
var varietyCategory = world.Category("Variety");

await foreach (var (category, (song, _)) in Read("CustomSongs.csv"))
{
    if (category.Span is "Variety")
    {
        varietySongs.Add(song);
        continue;
    }

    var c = world.Category(category);

    var early = category.Span is not "Hello (BPM)" &&
        (category.Span is "Song Link" || world.AllItemsWith(category).Skip(1).Any())
            ? 0
            : 1;

    var i = world.Item(song, null, c, early: early);
    world.Location(song, category.Span is "Song Link" ? i : world.AllItemsWith(c).And(), c);
}

for (var i = 0; i < varietySongs.Count; i++)
{
    IList<string> take = [..varietySongs.Concat(varietySongs).Skip(i).Take(3).Select(x => x.ToString())];
    var it = world.Item(take.Conjoin(" & "), null, varietyCategory, early: i >= 2 ? 0 : 1);

    foreach (var x in take)
    {
        lookup.TryAdd(x, []);
        lookup[x].Add(it);
    }
}

foreach (var (key, value) in lookup)
    world.Location(key, value.Or(), varietyCategory);

var finale = world.AllCategories["Finale"];
world.Location("Complete all finale songs", finale[world.AllItemsWith("Finale").Count()], finale);

await world.Game("VibRibbon", "Emik", "Play a random song, no reward (Trap)", [new(world.AllCategories["Variety"], 1)])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
