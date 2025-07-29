// SPDX-License-Identifier: MPL-2.0
// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

[Pure]
static string Ordinal(int i) =>
    $"{i}{(i % 10) switch
    {
        _ when i / 10 % 10 is 1 => "th",
        1 => "st",
        2 => "nd",
        3 => "rd",
        _ => "th",
    }}";

static IAsyncEnumerable<SplitMemory<char, char, MatchOne>> Read([Match("^[^<>:\"/\\\\|?*]+$")] string path)
{
    var found = Path.Join(Environment.CurrentDirectory)
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn(','));
}

World world = new();
var bossCell = world.Item("Boss Cell", categories: world.Category("Boss Cell"), count: 9);
var passages = world.Category("Passages");

void Connect(string regionName, params string[] connectsTo) =>
    world.Region(
        regionName,
        null,
        [
            ..connectsTo.Select(
                x => new Passage(
                    world.AllRegions[x],
                    world.Item($"{regionName} → {x}", Priority.ProgressionUseful, passages)
                )
            ),
        ],
        regionName is "Prisoners' Quarters" or "Bank"
    );

Connect("Crown");
Connect("Throne Room");
Connect("Master's Keep");
Connect("Lighthouse", "Crown");
Connect("Infested Shipwreck", "Lighthouse");
Connect("Derelict Distillery", "Lighthouse", "Throne Room");
Connect("High Peak Castle", "Throne Room", "Master's Keep");
Connect("Dracula's Castle (Late)", "Master's Keep");
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

Connect("Undying Shores", "Mausoleum");
Connect("Clock Tower", "Clock Room");
Connect("Forgotten Sepulcher", "Clock Room", "Guardian's Haven");
Connect("Cavern", "Mausoleum", "Guardian's Haven");
Connect("Fractured Shrines", "Undying Shores", "Clock Tower", "Forgotten Sepulcher");
Connect("Stilt Village", "Undying Shores", "Clock Tower", "Forgotten Sepulcher");
Connect("Slumbering Sanctuary", "Cavern", "Clock Tower", "Forgotten Sepulcher");
Connect("Graveyard", "Cavern", "Undying Shores", "Forgotten Sepulcher");
Connect("Nest", "Fractured Shrines", "Stilt Village", "Graveyard");
Connect("Black Bridge", "Fractured Shrines", "Stilt Village", "Slumbering Sanctuary");
Connect("Insufferable Crypt", "Graveyard", "Slumbering Sanctuary");
Connect("Defiled Necropolis", "Stilt Village", "Slumbering Sanctuary", "Graveyard");
Connect("Morass of the Banished", "Nest");
Connect("Ossuary", "Black Bridge", "Defiled Necropolis");
Connect("Ramparts", "Black Bridge");
Connect("Ancient Sewers", "Insufferable Crypt");
Connect("Dracula's Castle (Early)", "Black Bridge", "Defiled Necropolis");
Connect("Prison Depths", "Morass of the Banished", "Ossuary", "Ancient Sewers");
Connect("Corrupted Prison", "Ramparts", "Ancient Sewers", "Dracula's Castle (Early)");
Connect("Dilapidated Arboretum", "Prison Depths", "Morass of the Banished", "Ramparts");
Connect("Promenade of the Condemned", "Morass of the Banished", "Ossuary", "Ramparts", "Prison Depths");
Connect("Toxic Sewers", "Corrupted Prison", "Ramparts", "Ancient Sewers", "Dracula's Castle (Early)");
Connect("Castle Outskirts", "Ossuary", "Dracula's Castle (Early)", "Corrupted Prison");

Connect(
    "Prisoners' Quarters",
    "Castle Outskirts",
    "Toxic Sewers",
    "Promenade of the Condemned",
    "Dilapidated Arboretum"
);

Connect("Bank");

// ReSharper disable once UnusedLocalFunctionReturnValue
Location Biome(string name, int i, int count, Logic? logic = null, string? with = null)
{
    var item = world.Item(name, categories: world.Category("Completions"), count: count);
    logic &= item[i - 1];
    var category = world.Category(name);
    var region = world.AllRegions[name];

    if (with is null)
    {
        world.Location($"Beat {name} for the {Ordinal(i)} time - LOGIC", logic, category, region, allowList: item);
        return world.Location($"Beat {name} for the {Ordinal(i)} time", logic, category, region);
    }

    world.Location($"Beat {name} with {with} - LOGIC LOGIC", logic, category, region, allowList: bossCell);
    world.Location($"Beat {name} with {with} - LOGIC", logic, category, region, allowList: item);
    return world.Location($"Beat {name} with {with}", logic, category, region);
}

foreach (var category in (ImmutableArray<string>)["Melee", "Ranged", "Shields", "Skills"])
    world.Category(category, []);

var startingItems = ImmutableArray.CreateBuilder<StartingItemBlock>();

await foreach (var (itemName, (category, (starting, _))) in Read("DeadCellsItems.csv"))
{
    var item = world.Item(itemName, Priority.Progression, [category], giveItems: [("Weapon", 1)]);

    if (starting.Span is "Starting")
        startingItems.Add(new(item));
}

var mutations = world.Category("Mutations");

await foreach (var (mutation, _) in Read("DeadCellsMutations.csv"))
    world.Item(mutation, Priority.Useful, mutations);

var reduceCurses = world.Category("Reduce Curses");
world.Item("Reduce Curse by 1", Priority.Useful, reduceCurses, 10);
var cells = world.Category("Cells");

/*
 * Trap,3,Next run must be Expert
 * Trap,2,Next run must be Nightmare
 * Trap,1,Next run must be Hell
 */
// ReSharper disable once UseRawString
@"
Trap,10,Win a run with Enable Malaise
Trap,1,Win a run with FOLLOW THE LIGHT
Trap,1,Win a run with THE CHERRY ON THE CAKE
Trap,1,Win a run with TRAPPED DOORS
Trap,1,Win a run with BLOODLUST
Trap,1,Win a run with VENOMOUS
Trap,1,Win a run with HITCHCOCK
Trap,1,Win a run with FOG
Trap,1,Win a run with SHARP SHOOTERS
Trap,1,Win a run with SPIKERS
Trap,1,Win a run with ADAPTATION
".SplitLines()
   .Select(
        x => x.SplitOn(',') is var (priority, (amount, (rest, _)))
            ? world.Item(rest, Enum.Parse<Priority>(priority.Span), [], int.Parse(amount.Span))
            : throw Unreachable
    )
   .Enumerate();

for (var i = 0; i < 56; i++)
    world.Location($"Get {i * 10 + 50} Cells", Logic.ItemValue("Weapon", i * 3), cells);

const int PrisonersQuarters = 1,
    Bank = 13,
    DilapidatedArboretum = 13,
    PrisonDepths = 12,
    MorassOfTheBanished = 11,
    Nest = 9,
    FracturedShrines = 8,
    UndyingShores = 7,
    Mausoleum = 6,
    InfestedShipwreck = 5,
    Lighthouse = 3,
    Crown = 3;

for (var i = 1; i <= PrisonersQuarters; i++)
    Biome("Prisoners' Quarters", i, PrisonersQuarters);

for (var i = 1; i <= Bank; i++)
    Biome("Bank", i, Bank, Logic.ItemValue("Weapon", i * 3 + 65) & bossCell[3]);

for (var i = 1; i <= DilapidatedArboretum; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 5) & (i > 6 ? bossCell[3] : null);
    Biome("Dilapidated Arboretum", i, DilapidatedArboretum, logic);
    Biome("Promenade of the Condemned", i, DilapidatedArboretum, logic);
    Biome("Toxic Sewers", i, DilapidatedArboretum, logic);
    Biome("Castle Outskirts", i, DilapidatedArboretum, logic);
}

for (var i = 1; i <= PrisonDepths; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 15);
    Biome("Prison Depths", i, PrisonDepths, logic);
    Biome("Corrupted Prison", i, PrisonDepths, logic);
}

for (var i = 1; i <= MorassOfTheBanished; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 7) & (i > 5 ? bossCell[3] : null);
    Biome("Morass of the Banished", i, MorassOfTheBanished, logic);
    Biome("Ossuary", i, MorassOfTheBanished, logic);
    Biome("Ramparts", i, MorassOfTheBanished, logic);
    Biome("Ancient Sewers", i, MorassOfTheBanished, logic);
    Biome("Dracula's Castle (Early)", i, MorassOfTheBanished, logic);
}

for (var i = 1; i <= Nest; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 10) & bossCell[i / 3 * 3];
    Biome("Nest", i, Nest, logic);
    Biome("Black Bridge", i, Nest, logic);
    Biome("Insufferable Crypt", i, Nest, logic);
    Biome("Defiled Necropolis", i, Nest, logic);
}

for (var i = 1; i <= FracturedShrines; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 13) &
        i switch
        {
            1 or 2 => null,
            3 or 4 or 5 => bossCell[3],
            6 or 7 or 8 => bossCell[6],
            _ => throw Unreachable,
        };

    Biome("Fractured Shrines", i, FracturedShrines, logic);
    Biome("Stilt Village", i, FracturedShrines, logic);
    Biome("Slumbering Sanctuary", i, FracturedShrines, logic);
    Biome("Graveyard", i, FracturedShrines, logic);
}

for (var i = 1; i <= UndyingShores; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 17) &
        i switch
        {
            1 or 2 => null,
            3 or 4 => bossCell[3],
            5 or 6 or 7 => bossCell[6],
            _ => throw Unreachable,
        };

    Biome("Undying Shores", i, UndyingShores, logic);
    Biome("Clock Tower", i, UndyingShores, logic);
    Biome("Forgotten Sepulcher", i, UndyingShores, logic);
    Biome("Cavern", i, UndyingShores, logic);
}

for (var i = 1; i <= Mausoleum; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 25) &
        i switch
        {
            1 or 2 => null,
            3 or 4 => bossCell[3],
            5 or 6 => bossCell[6],
            _ => throw Unreachable,
        };

    Biome("Mausoleum", i, Mausoleum, logic);
    Biome("Clock Room", i, Mausoleum, logic);
    Biome("Guardian's Haven", i, Mausoleum, logic);
}

for (var i = 1; i <= InfestedShipwreck; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 35) &
        i switch
        {
            1 => null,
            2 or 3 => bossCell[3],
            4 or 5 => bossCell[6],
            _ => throw Unreachable,
        };

    Biome("Infested Shipwreck", i, InfestedShipwreck, logic);
    Biome("Derelict Distillery", i, InfestedShipwreck, logic);
    Biome("High Peak Castle", i, InfestedShipwreck, logic);
    Biome("Dracula's Castle (Late)", i, InfestedShipwreck, logic);
}

for (var i = 1; i <= Lighthouse; i++)
{
    var logic = Logic.ItemValue("Weapon", i * 55) &
        i switch
        {
            1 => null,
            2 => bossCell[3],
            3 => bossCell[6],
            _ => throw Unreachable,
        };

    Biome("Lighthouse", i, Lighthouse, logic);
}

foreach (var method in (ImmutableArray<string>)["Brutality", "Tactics", "Survival"])
{
    static Logic? RandomWeaponAmount() => Logic.ItemValue("Weapon", Random.Shared.Next(90, 151));

    Biome("Crown", 1, Crown, RandomWeaponAmount(), method);
    Biome("Throne Room", 1, Crown, RandomWeaponAmount(), method);
    Biome("Master's Keep", 1, Crown, RandomWeaponAmount(), method);
}

world.Location("Defeat all final bosses with all colors", bossCell[9], options: LocationOptions.Victory);

await world.Game("DeadCells", "RedsAndEmik", "Increase starting gold by 1000", startingItems.DrainToImmutable())
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
