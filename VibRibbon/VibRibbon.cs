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

await foreach (var (category, (song, _)) in Read("CustomSongs.csv"))
{
    var c = world.Category(category);
    var i = world.Item(song, categories: c);
    world.Location(song, category.Span is "Song Link" or "Variety" ? i : world.AllItemsWith(c).And(), c);
}

var finale = world.AllCategories["Finale"];
world.Location("Complete all finale songs", finale[world.AllItemsWith("Finale").Count()], finale);

await world.Game("VibRibbon", "Emik", "Play a random song, no reward (Trap)", [new(world.AllCategories["Variety"], 2)])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
