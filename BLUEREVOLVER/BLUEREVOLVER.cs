// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

World world = new();

world.Location("JAM TO THE OST", categories: world.Category("IT'S TIME TO ENGAGE!"));

ImmutableArray<string> filters =
    ["DB16", "SWEETIE", "PICO-8", "JPC16", "HEADACHE", "EGA", "CAPTAIN", "GOOD TV", "SMART TV"];

foreach (var filter in filters)
    world.Item($"SET FILTER TO {filter}", Priority.Trap, world.Category("FILTERS"));

world.Item("PLAY ON PARALLEL MODE", Priority.Trap, count: 4);

for (var i = 1; i <= 5; i++)
{
    world.Item($"PRACTICE STAGE {i} START", Priority.Useful);

    switch (i)
    {
        case 5:
            world.Item("PRACTICE STAGE 5 MIDBOSS 1", Priority.Useful);
            world.Item("PRACTICE STAGE 5 MIDBOSS 2", Priority.Useful);
            world.Item("PRACTICE STAGE 5 DESCENT", Priority.Useful);
            world.Item("PRACTICE STAGE 5 MIDBOSS 3", Priority.Useful);
            world.Item("PRACTICE STAGE 5 MIDBOSS 4", Priority.Useful);
            break;
        case 3:
            world.Item("PRACTICE STAGE 3 AFTER MIDBOSS", Priority.Useful);
            goto case 0;
        case 0:
        default:
            world.Item($"PRACTICE STAGE {i} MIDBOSS", Priority.Useful);
            break;
    }

    world.Item($"PRACTICE STAGE {i} BOSS", Priority.Useful);
}

var val = world.Category("VAL");
world.Item("VAL-ARCCASTER", categories: val);
world.Item("VAL-HYPERLASER", categories: val);
world.Item("VAL-STASISFIELD", categories: val);
world.Item("VAL-CLUSTERMISSILE", categories: val);

var mae = world.Category("MAE");
world.Item("MAE-PLASMALANCER", categories: mae);
world.Item("MAE-HYPERLASER", categories: mae);
world.Item("MAE-VORTEXBARRIER", categories: mae);
world.Item("MAE-CLUSTERMISSILE", categories: mae);

var dee = world.Category("DEE");
world.Item("DEE-FRACTALSALVO", categories: dee);
world.Item("DEE-PIERCELANCER", categories: dee);
world.Item("DEE-GUNBUCKLER", categories: dee);
world.Item("DEE-CLUSTERMISSILE", categories: dee);

ImmutableArray<Category> characters = [val, mae, dee];

foreach (var character in characters)
{
    var cStage5 = world.Region($"{character} STAGE 5", character[2]);
    ArchipelagoArrayBuilder<Region> cConnection = [cStage5];
    var cStage1Through4 = world.Region($"{character} STAGE 1-4", character[1], cConnection, true);

    for (var i = 1; i <= 5; i++)
        world.Location($"COMPLETE STAGE {i} FOR {character}", null, character, i is 5 ? cStage5 : cStage1Through4);
}

ImmutableArray<int> breaks = [3, 6, 6, 6, 6];

var stage5 = world.Region("STAGE 5 BREAKS", val[2] | mae[2] | dee[2]);
ArchipelagoArrayBuilder<Region> connection = [stage5];
var stage1Through4 = world.Region("STAGE 1-4 BREAKS", val[1] | mae[1] | dee[1], connection, true);

for (var i = 0; i < breaks.Length && breaks[i] is var br; i++)
    for (var j = 1; j <= br; j++)
        world.Location($"STAGE {i + 1} BREAK {j}", null, world.Category("BREAKS"), i is 4 ? stage5 : stage1Through4);

ImmutableArray<int> scores = [15, 40, 80, 120];

foreach (var score in scores)
    world.Location($"GET {score} MILLION SCORE", null, world.Category("SCORES"), score > 40 ? stage5 : stage1Through4);

world.Location("COMPLETE EVERY OTHER LOCATION", val[2] & mae[2] & dee[2], [], stage5, LocationOptions.Victory);

await world.Game("BLUEREVOLVER", "RedsAndEmik", "FLOURISH", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
