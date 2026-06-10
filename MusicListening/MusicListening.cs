// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

static IAsyncEnumerable<SplitMemory<char, char, MatchOne>> Read(string path)
{
    var found = Environment.CurrentDirectory
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn('\t'));
}

World world = new();
var albums = world.Category("Albums");
var artists = world.Category("Artists");
var goals = world.Category("Goals", true);

await foreach (var all in Read("July2026APList.tsv").OrderByDescending(x => x.Last.Span is "GOAL"))
{
    var (track, (album, (artist, (goal, _)))) = all;
    (track, album, artist, goal) = (track.Trim(), album.Trim(), artist.Trim(), goal.Trim());
    Console.WriteLine(track);
    var priority = goal.Span is "GOAL" ? Priority.ProgressionUseful : Priority.Progression;
    var albumItem = world.Item(album, priority, goal.Span is "GOAL" ? [albums, goals] : albums, count: 12);

    var artistItem = artist.Span is "[Starter]"
        ? (Item?)null
        : world.Item(artist, priority, goal.Span is "GOAL" ? [artists, goals] : artists, count: 12);

    world.Location(track, albumItem[8] & (artistItem is { } a ? a[8] : null), world.Category(album));
}

await foreach (var all in Read("one wayne g").Take(10))
    world.Location(all.Body, null, world.Category("One Wayne G"));

world.Location("Goal", goals.All, options: LocationOptions.Victory);

await world.Game("MusicListening", "CrawAndEmik", "One Wayne G", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
