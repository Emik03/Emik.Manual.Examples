// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

static string ItemName(int i) => $"Progressive Page for Chapter {i}";

static string JigsawLocationName(int i) => $"Complete Chapter {i}'s Jigsaw Puzzle";

static string LocationName(int i) => $"Complete {i % 10 + 1}/9 pages in Chapter {i / 10 + 1}";

static string MetaLocationName(int i) => $"Complete Chapter {i}'s Meta-Puzzle";

static string RegionName(int i) => $"Chapter {i}";

static string ReplayLocationName(int i) => $"Replay a book from Chapter {i}";

World world = new();

void MakeItem(int i) => world.Item(ItemName(i), null, world.Category(RegionName(i)), 9);

void MakeLocation(int i) =>
    world.Location(
        LocationName(i),
        world.AllItems[ItemName(i / 9 + 1)][i % 9 + 1],
        RegionName(i / 9 + 1),
        RegionName(i / 9 + 1)
    );

void MakeJigsawLocation(int i) =>
    world.Location(JigsawLocationName(i), world.AllItems[ItemName(i)].All, RegionName(i), RegionName(i));

void MakeMetaLocation(int i) =>
    world.Location(
        MetaLocationName(i),
        world.AllItems[ItemName(i)].All,
        RegionName(i),
        RegionName(i),
        i is 10 ? LocationOptions.Victory : LocationOptions.None
    );

void MakeRegion(int i) =>
    world.Region(
        RegionName(i),
        i is 1 ? null : world.AllItems[ItemName(i - 1)].All,
        i is 10 ? [] : (Region)RegionName(i + 1),
        i is 1
    );

void MakeReplayLocationName(int i) =>
    world.Location(ReplayLocationName(i), world.AllItems[ItemName(i)].All, RegionName(i), RegionName(i));

world.Item("Time Trap", Priority.Trap, world.Category("Traps"), 10);
world.Item("Orb (Puzzle Skip)", Priority.Useful, world.Category("Orbs"), 10);
(1..11).For().Lazily(MakeItem).Enumerate();
(11..1).For().Lazily(MakeRegion).Enumerate();
(1..11).For().Lazily(MakeReplayLocationName).Enumerate();
(1..11).For().Lazily(MakeJigsawLocation).Enumerate();
(1..11).For().Lazily(MakeMetaLocation).Enumerate();
90.For().Lazily(MakeLocation).Enumerate();
world.Location("Wake up", categories: RegionName(1));

await world.Game("Azada", "Emik", "One-Time Use Hint", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
