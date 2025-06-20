// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

static string Display(GameModes mode) =>
    mode switch
    {
        GameModes.None => nameof(GameModes.None),
        GameModes.All => nameof(GameModes.All),
        GameModes.ScoreAttack5Min => "Score Attack (5 min.)",
        GameModes.ScoreAttack10Min => "Score Attack (10 min.)",
        GameModes.GhostCombo => "Ghost Combo",
        GameModes.TimeTrial => "Time Trial",
        GameModes.TimeTrialShortTotal => "Time Trial (Short) Total",
        GameModes.Darkness => "Darkness",
        _ => throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(GameModes)),
    };

World world = new();

Dictionary<string, GameModes> locations = new()
{
    ["Championship II"] = GameModes.All,
    ["Highway"] = GameModes.All,
    ["Junction"] = GameModes.All,
    ["Spiral"] = GameModes.All,
    ["Manhattan"] = GameModes.All,
    ["Dungeon"] = GameModes.All,
    ["Championship I"] = GameModes.All,
    ["Half"] = GameModes.TimeTrial | GameModes.TimeTrialShortTotal | GameModes.Darkness,
    ["Mountain"] = GameModes.ScoreAttack5Min | GameModes.TimeTrial | GameModes.TimeTrialShortTotal,
    ["Big Eater"] = GameModes.ScoreAttack5Min | GameModes.TimeTrial | GameModes.TimeTrialShortTotal,
    ["Championship III"] = GameModes.ScoreAttack5Min | GameModes.TimeTrial,
    ["Highway II"] = GameModes.ScoreAttack5Min | GameModes.TimeTrial,
};

ImmutableArray<string> milestones = ["S Rank", "Top 500"];
var mapCount = 0;
var mapsCategory = world.Category("Maps");
var gameModesCategory = world.Category("Game Modes");

world.Item("Take a Shower!", Priority.Trap, world.Category("Showers"), count: 5);
world.Item("Progressive Bombs", Priority.Useful, world.Category("Bombs"), count: 15, early: 2);

foreach (var (map, gameModes) in locations)
{
    mapCount++;
    // var gameModeCount = 0;
    var mapCategory = world.Category(map);
    var mapItem = world.Item($"{map} ({mapCount})", categories: mapsCategory, count: mapCount);

    foreach (var gameMode in gameModes.AsBits())
    {
        // gameModeCount++;
        var gameModeCategory = world.Category(Display(gameMode));

        // var gameModeItem = world.Item(
        //     $"{Display(gameMode)} ({gameModeCount})",
        //     categories: gameModesCategory,
        //     count:
        //     gameModeCount
        // );

        foreach (var grade in milestones)
            world.Location(
                $"{map} {Display(gameMode)} - {grade}",
                mapItem.All,
                // mapItem.All & gameModeItem.All,
                [mapCategory, gameModeCategory]
            );
    }
}

world.Location(
    "Beat chaotic",
    world.AllItemsWithAny([mapsCategory, gameModesCategory]).Select(x => x.All).And(),
    options: LocationOptions.Victory
);

const string Filler = "Loosen leaderboard requirement by 100 (actually useful)";

await world.Game("PacManChampionshipEditionDXPlus", "RedsAndEmik", Filler, [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);

[Flags]
enum GameModes : byte
{
    None,
    ScoreAttack5Min,
    ScoreAttack10Min,
    TimeTrialShortTotal = 1 << 2,
    TimeTrial = 1 << 3,
    GhostCombo = 1 << 4,
    Darkness = 1 << 5,
    All = (1 << 6) - 1,
}
