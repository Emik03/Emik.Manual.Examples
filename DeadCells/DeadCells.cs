// SPDX-License-Identifier: MPL-2.0
// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
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
var passages = world.Category("Passages");

ref readonly Region Connect(string regionName, params string[] connectsTo) =>
    ref world.Region(
        regionName,
        null,
        [
            ..connectsTo.Select(
                x => new Passage(
                    world.AllRegions[x],
                    world.Item($"{regionName} -> {x}", Priority.ProgressionUseful, passages)
                )
            ),
        ],
        regionName is "Prisoners' Quarters" or "Bank"
    );

var observatory = Connect("Observatory");
var astrolab = Connect("Astrolab", "Observatory");

ImmutableArray<Passage> throneRoomToAstrolab =
[
    (
        astrolab,
        ((Logic)"Mausoleum -> Infested Shipwreck" |
            "Clock Room -> Infested Shipwreck" |
            "Guardian's Haven -> Infested Shipwreck") &
        ((Logic)"Mausoleum -> Dracula's Castle (Late)" |
            "Clock Room -> Dracula's Castle (Late)" |
            "Guardian's Haven -> Dracula's Castle (Late)") &
        "Lighthouse -> Crown" &
        "Infested Shipwreck -> Lighthouse" &
        "Dracula's Castle (Late) -> Master's Keep"
    ),
];

world.AllRegions.TryAdd(new Region("Throne Room", null, [astrolab], Exits: throneRoomToAstrolab));
Connect("Crown");
Connect("Master's Keep");
Connect("Lighthouse", "Crown");
var infestedShipwreck = Connect("Infested Shipwreck", "Lighthouse");
var derelictDistillery = Connect("Derelict Distillery", "Lighthouse", "Throne Room");
var highPeakCastle = Connect("High Peak Castle", "Throne Room", "Master's Keep");
var draculasCastleLate = Connect("Dracula's Castle (Late)", "Master's Keep");
Connect("Mausoleum", "Infested Shipwreck", "Derelict Distillery", "High Peak Castle", "Dracula's Castle (Late)");
Connect("Clock Room", "Infested Shipwreck", "Derelict Distillery", "High Peak Castle", "Dracula's Castle (Late)");

Connect(
    "Guardian's Haven",
    "Infested Shipwreck",
    "Derelict Distillery",
    "High Peak Castle",
    "Dracula's Castle (Late)",
    "Throne Room"
);

var undyingShores = Connect("Undying Shores", "Mausoleum");
var clockTower = Connect("Clock Tower", "Clock Room");
var forgottenSepulcher = Connect("Forgotten Sepulcher", "Clock Room", "Guardian's Haven");
var cavern = Connect("Cavern", "Mausoleum", "Guardian's Haven");
var fracturedShrines = Connect("Fractured Shrines", "Undying Shores", "Clock Tower", "Forgotten Sepulcher");
var stiltVillage = Connect("Stilt Village", "Undying Shores", "Clock Tower", "Forgotten Sepulcher");
var slumberingSanctuary = Connect("Slumbering Sanctuary", "Cavern", "Clock Tower", "Forgotten Sepulcher");
var graveyard = Connect("Graveyard", "Cavern", "Undying Shores", "Forgotten Sepulcher");
Connect("Nest", "Fractured Shrines", "Stilt Village", "Graveyard");
Connect("Black Bridge", "Fractured Shrines", "Stilt Village", "Slumbering Sanctuary");
Connect("Insufferable Crypt", "Graveyard", "Slumbering Sanctuary");
Connect("Defiled Necropolis", "Stilt Village", "Slumbering Sanctuary", "Graveyard");
var morassOfTheBanished = Connect("Morass of the Banished", "Nest");
var ossuary = Connect("Ossuary", "Black Bridge", "Defiled Necropolis");
var ramparts = Connect("Ramparts", "Black Bridge");
var ancientSewers = Connect("Ancient Sewers", "Insufferable Crypt");
var draculasCastleEarly = Connect("Dracula's Castle (Early)", "Black Bridge", "Defiled Necropolis");
var prisonDepths = Connect("Prison Depths", "Morass of the Banished", "Ossuary", "Ancient Sewers");
var corruptedPrison = Connect("Corrupted Prison", "Ramparts", "Ancient Sewers", "Dracula's Castle (Early)");
var dilapidatedArboretum = Connect("Dilapidated Arboretum", "Prison Depths", "Morass of the Banished", "Ramparts");

var promenadeOfTheCondemned = Connect(
    "Promenade of the Condemned",
    "Morass of the Banished",
    "Ossuary",
    "Ramparts",
    "Prison Depths"
);

var toxicSewers = Connect("Toxic Sewers", "Corrupted Prison", "Ramparts", "Ancient Sewers", "Dracula's Castle (Early)");
var castleOutskirts = Connect("Castle Outskirts", "Ossuary", "Dracula's Castle (Early)", "Corrupted Prison");

var prisonersQuarters = Connect(
    "Prisoners' Quarters",
    "Castle Outskirts",
    "Toxic Sewers",
    "Promenade of the Condemned",
    "Dilapidated Arboretum"
);

Connect("Bank");

foreach (var biome in (ImmutableArray<Region>)
[
    prisonDepths, corruptedPrison, morassOfTheBanished, ossuary,
    fracturedShrines, slumberingSanctuary, graveyard, forgottenSepulcher,
])
    world.Location($"Clear curse in {biome}", region: biome);

foreach (var biome in (ImmutableArray<Region>)
[
    prisonersQuarters, dilapidatedArboretum, promenadeOfTheCondemned, toxicSewers, castleOutskirts, prisonDepths,
    corruptedPrison, morassOfTheBanished, ossuary, ramparts, ancientSewers, draculasCastleEarly, fracturedShrines,
    stiltVillage, slumberingSanctuary, graveyard, undyingShores, clockTower, forgottenSepulcher, cavern,
    infestedShipwreck, derelictDistillery, highPeakCastle, draculasCastleLate,
])
    world.Location($"Purchase in {biome}", region: biome);

void Biome(string name, Logic? logic = null)
{
    var category = world.Category(name);
    var region = world.AllRegions[name];
    world.Location($"Reach {name}", logic, category, region);
    world.Location($"Beat {name}", logic, category, region);
}

foreach (var category in (ImmutableArray<string>)["Melee", "Ranged", "Shields", "Deployable", "Grenades", "Powers"])
    world.Category(category, []);

var startingItems = ImmutableArray.CreateBuilder<StartingItemBlock>();

await foreach (var (itemName, (category, (starting, _))) in Read("DeadCellsItems.csv"))
    if (starting.Span is "Starting")
        startingItems.Add(new(world.Item(itemName, Priority.Progression, [category], giveItems: [("Weapon", 1)])));

await foreach (var items in Read("DeadCellsItems.csv").Shuffle().GroupBy(x => x[1].ToString()).Select(x => x.Chunk(3)))
    foreach (var bundle in items)
        world.Item(
            bundle.Select(x => x[0]).Conjoin(),
            Priority.Progression,
            [bundle[0][1]],
            giveItems: [("Weapon", bundle.Length)]
        );

await foreach (var mutations in Read("DeadCellsMutations.csv").Shuffle().Chunk(3))
    world.Item(mutations.Select(x => x[0]).Conjoin(), Priority.Useful, world.Category("Mutations"));

Biome("Prisoners' Quarters");
Biome("Bank", Logic.ItemValue("Weapon", 40));

{
    var logic = Logic.ItemValue("Weapon", 15);
    Biome("Dilapidated Arboretum", logic);
    Biome("Promenade of the Condemned", logic);
    Biome("Toxic Sewers", logic);
    Biome("Castle Outskirts", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 45);
    Biome("Prison Depths", logic);
    Biome("Corrupted Prison", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 25);
    Biome("Morass of the Banished", logic);
    Biome("Ossuary", logic);
    Biome("Ramparts", logic);
    Biome("Ancient Sewers", logic);
    Biome("Dracula's Castle (Early)", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 50);
    Biome("Nest", logic);
    Biome("Black Bridge", logic);
    Biome("Insufferable Crypt", logic);
    Biome("Defiled Necropolis", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 60);
    Biome("Fractured Shrines", logic);
    Biome("Stilt Village", logic);
    Biome("Slumbering Sanctuary", logic);
    Biome("Graveyard", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 75);
    Biome("Undying Shores", logic);
    Biome("Clock Tower", logic);
    Biome("Forgotten Sepulcher", logic);
    Biome("Cavern", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 100);
    Biome("Mausoleum", logic);
    Biome("Clock Room", logic);
    Biome("Guardian's Haven", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 110);
    Biome("Infested Shipwreck", logic);
    Biome("Derelict Distillery", logic);
    Biome("High Peak Castle", logic);
    Biome("Dracula's Castle (Late)", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 130);
    Biome("Lighthouse", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 130);
    Biome("Crown", logic);
    Biome("Throne Room", logic);
    Biome("Master's Keep", logic);
}

{
    var logic = Logic.ItemValue("Weapon", 150);
    Biome("Astrolab", logic);
    Biome("Observatory", logic);
    world.Location("Beat the Collector", logic, region: observatory, options: LocationOptions.Victory);
}

var cells = world.Category("Cells");
var canAccessSecondRegion = castleOutskirts | toxicSewers | dilapidatedArboretum | promenadeOfTheCondemned;

for (var i = 0; i <= 60; i++)
    world.Location(
        $"Get {i * 5 + 50} Cells",
        Logic.ItemValue("Weapon", i / 5 * 15) & (i >= 5 ? canAccessSecondRegion : null),
        cells
    );

var game = world.Game("DeadCells", "RedsAndEmik2", "Use random outfit", startingItems.DrainToImmutable());
await game.DisplayExported(Console.WriteLine).ZipAsync(Path.GetTempPath(), listChecks: true, expand: true);
