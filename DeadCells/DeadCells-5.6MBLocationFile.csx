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

static IAsyncEnumerable<SplitMemory<char, char, MatchOne>> Read(string path)
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

// ReSharper disable once UnusedLocalFunctionReturnValue
Location Biome(string name, int i, int count, ImmutableArray<Chars> entries, Logic? logic = null, string? with = null)
{
    const Priority PassagePriority = Priority.ProgressionUseful;
    var item = world.Item(name, categories: world.Category("Completions"), count: count);
    logic &= item[i - 1];

    Logic? All(Chars item) =>
        world.AllItemsWith(passages)
           .Where(x => x.Name.Span.EndsWith(item.Memory.Span))
           .Select(x => All(x.Name.Memory.SplitOn(" → ").First) & x)
           .Or();

    if (!entries.IsDefaultOrEmpty)
        logic &= entries.Select(x => All(x) & world.Item($"{x} → {name}", PassagePriority, passages)).Or();

    var category = world.Category(name);

    if (with is null)
    {
        world.Location(
            $"Beat {name} for the {Ordinal(i)} time - LOGIC",
            logic,
            category,
            allowList: item
        );

        return world.Location($"Beat {name} for the {Ordinal(i)} time", logic, category);
    }

    world.Location($"Beat {name} with {with} - LOGIC LOGIC", logic, category, allowList: bossCell);

    world.Location(
        $"Beat {name} with {with} - LOGIC",
        logic,
        category,
        allowList: item
    );

    return world.Location($"Beat {name} with {with}", logic, category);
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
    world.Location($"Get {i * 10 + 50} Cells", Logic.OfItemValue("Weapon", i * 3), cells);

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
    Biome("Prisoners' Quarters", i, PrisonersQuarters, []);

for (var i = 1; i <= Bank; i++)
    Biome("Bank", i, Bank, [], bossCell[3]);

for (var i = 1; i <= DilapidatedArboretum; i++)
{
    ImmutableArray<Chars> entries = ["Prisoners' Quarters"];
    var logic = Logic.OfItemValue("Weapon", i * 5) & (i > 6 ? bossCell[3] : null);
    Biome("Dilapidated Arboretum", i, DilapidatedArboretum, entries, logic);
    Biome("Promenade of the Condemned", i, DilapidatedArboretum, entries, logic);
    Biome("Toxic Sewers", i, DilapidatedArboretum, entries, logic);
    Biome("Castle Outskirts", i, DilapidatedArboretum, entries, logic);
}

for (var i = 1; i <= PrisonDepths; i++)
{
    ImmutableArray<Chars> entries =
        ["Dilapidated Arboretum", "Promenade of the Condemned", "Toxic Sewers", "Castle Outskirts"];

    var logic = Logic.OfItemValue("Weapon", i * 15);
    Biome("Prison Depths", i, PrisonDepths, entries[..2], logic);
    Biome("Corrupted Prison", i, PrisonDepths, entries[2..], logic);
}

for (var i = 1; i <= MorassOfTheBanished; i++)
{
    ImmutableArray<Chars> entries =
    [
        "Dilapidated Arboretum", "Promenade of the Condemned", "Toxic Sewers",
        "Castle Outskirts", "Prison Depths", "Corrupted Prison",
    ];

    var logic = Logic.OfItemValue("Weapon", i * 7) & (i > 5 ? bossCell[3] : null);
    Biome("Morass of the Banished", i, MorassOfTheBanished, [..entries[..2], entries[4]], logic);
    Biome("Ossuary", i, MorassOfTheBanished, [entries[1], ..entries[3..5]], logic);
    Biome("Ramparts", i, MorassOfTheBanished, [..entries[..3], entries[5]], logic);
    Biome("Ancient Sewers", i, MorassOfTheBanished, [entries[2], ..entries[4..6]], logic);
    Biome("Dracula's Castle (Early)", i, MorassOfTheBanished, [..entries[2..4], entries[5]], logic);
}

for (var i = 1; i <= Nest; i++)
{
    ImmutableArray<Chars> entries =
        ["Morass of the Banished", "Ossuary", "Ramparts", "Ancient Sewers", "Dracula's Castle (Early)"];

    var logic = Logic.OfItemValue("Weapon", i * 10) & bossCell[i / 3 * 3];
    Biome("Nest", i, Nest, entries[..1], logic);
    Biome("Black Bridge", i, Nest, [..entries[1..3], entries[4]], logic);
    Biome("Insufferable Crypt", i, Nest, entries[2..4], logic);
    Biome("Defiled Necropolis", i, Nest, [entries[1], entries[4]], logic);
}

for (var i = 1; i <= FracturedShrines; i++)
{
    var logic = Logic.OfItemValue("Weapon", i * 13) &
        i switch
        {
            1 or 2 => null,
            3 or 4 or 5 => bossCell[3],
            6 or 7 or 8 => bossCell[6],
            _ => throw Unreachable,
        };

    ImmutableArray<Chars> entries = ["Nest", "Black Bridge", "Insufferable Crypt", "Defiled Necropolis"];
    Biome("Fractured Shrines", i, FracturedShrines, entries[..2], logic);
    Biome("Stilt Village", i, FracturedShrines, [..entries[..2], entries[3]], logic);
    Biome("Slumbering Sanctuary", i, FracturedShrines, entries[1..], logic);
    Biome("Graveyard", i, FracturedShrines, [entries[0], ..entries[2..]], logic);
}

for (var i = 1; i <= UndyingShores; i++)
{
    var logic = Logic.OfItemValue("Weapon", i * 17) &
        i switch
        {
            1 or 2 => null,
            3 or 4 => bossCell[3],
            5 or 6 or 7 => bossCell[6],
            _ => throw Unreachable,
        };

    ImmutableArray<Chars> entries = ["Fractured Shrines", "Stilt Village", "Slumbering Sanctuary", "Graveyard"];
    Biome("Undying Shores", i, UndyingShores, [..entries[..2], entries[3]], logic);
    Biome("Clock Tower", i, UndyingShores, entries[..3], logic);
    Biome("Forgotten Sepulcher", i, UndyingShores, entries, logic);
    Biome("Cavern", i, UndyingShores, [entries[3]], logic);
}

for (var i = 1; i <= Mausoleum; i++)
{
    var logic = Logic.OfItemValue("Weapon", i * 25) &
        i switch
        {
            1 or 2 => null,
            3 or 4 => bossCell[3],
            5 or 6 => bossCell[6],
            _ => throw Unreachable,
        };

    ImmutableArray<Chars> entries = ["Undying Shores", "Clock Tower", "Forgotten Sepulcher", "Cavern"];
    Biome("Mausoleum", i, Mausoleum, [entries[0], entries[3]], logic);
    Biome("Clock Room", i, Mausoleum, entries[1..3], logic);
    Biome("Guardian's Haven", i, Mausoleum, [entries[3]], logic);
}

for (var i = 1; i <= InfestedShipwreck; i++)
{
    var logic = Logic.OfItemValue("Weapon", i * 35) &
        i switch
        {
            1 => null,
            2 or 3 => bossCell[3],
            4 or 5 => bossCell[6],
            _ => throw Unreachable,
        };

    ImmutableArray<Chars> entries = ["Mausoleum", "Clock Room", "Guardian's Haven"];
    Biome("Infested Shipwreck", i, InfestedShipwreck, entries, logic);
    Biome("Derelict Distillery", i, InfestedShipwreck, entries, logic);
    Biome("High Peak Castle", i, InfestedShipwreck, entries, logic);
    Biome("Dracula's Castle (Late)", i, InfestedShipwreck, entries, logic);
}

for (var i = 1; i <= Lighthouse; i++)
{
    var logic = Logic.OfItemValue("Weapon", i * 55) &
        i switch
        {
            1 => null,
            2 => bossCell[3],
            3 => bossCell[6],
            _ => throw Unreachable,
        };

    Biome("Lighthouse", i, Lighthouse, ["Infested Shipwreck", "Derelict Distillery"], logic);
}

foreach (var method in (ImmutableArray<string>)["Brutality", "Tactics", "Survival"])
{
    ImmutableArray<Chars> entries =
        ["Guardian's Haven", "Derelict Distillery", "High Peak Castle", "Dracula's Castle (Late)"];

    var logic = Logic.OfItemValue("Weapon", 125);
    Biome("Crown", 1, Crown, ["Lighthouse"], logic, method);
    Biome("Throne Room", 1, Crown, entries[..3], logic, method);
    Biome("Master's Keep", 1, Crown, entries[2..], logic, method);
}

world.Location("Defeat all final bosses with all colors", bossCell[9], options: LocationOptions.Victory);
var game = world.Game("DeadCells", "RedsAndEmik", "Increase starting gold by 1000", startingItems.DrainToImmutable());
Console.WriteLine($"{game.ExportedLocationCount()}/{game.ExportedItemCount()}");
await game.ZipAsync(Path.GetTempPath());
