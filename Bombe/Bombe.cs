// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

const int StartingGameSpeed = 5;
World world = new();

static ImmutableArray<(Chars PhantomItemName, int Amount)> Brainpower(int amount) =>
    amount is 0 ? default : [(nameof(Brainpower), amount)];

ImmutableArray<Item> digits =
[
    ..10.For(
        x => world.Item(
            $"Digit {x}",
            categories: world.Category("Digits"),
            giveItems: Brainpower(
                x switch
                {
                    0 => 0,
                    1 => 320,
                    2 or 3 => 240,
                    4 or 5 or 6 => 160,
                    7 or 8 or 9 => 80,
                    _ => throw new ArgumentOutOfRangeException(nameof(x), x, null),
                }
            )
        )
    ),
];

ImmutableArray<(string Name, int Count, int Brainpower)> variableNames =
    [("x", 3, 450), ("y", 6, 600), ("k", 9, 630), ("pi", 12, 840), ("h", 15, 750)];

var items = ImmutableArray.CreateRange(
    variableNames,
    x => world.Item(
        $"Variable {x.Name} (requires {x.Count - 1})",
        categories: world.Category("Variables"),
        count: x.Count
    )
);

foreach (var item in items)
{
    var i = item.Name.Memory.SplitWhitespace()[1];

    world.Location(
        $"Unlock {i}",
        item[^1],
        world.Category("Logic"),
        allowList: world.Item(
            $"Unlocked {i}",
            categories: world.Category("Variables"),
            giveItems: Brainpower(
                variableNames.Single(x => x.Name.AsSpan().Equals(i.Span, StringComparison.Ordinal)).Brainpower
            )
        )
    );
}

ImmutableArray<(string Name, int Early, int Brainpower)> ruleNames =
[
    ("x", 1, 230), ("x-", 1, 190), ("x/x+1", 0, 120), ("x/x+2", 0, 80), ("x/x+3", 0, 30), ("!x", 0, 5), ("x+", 1, 190),
    ("x/x+1/x+2", 0, 60), ("x/x+2/x+4", 0, 30), ("x+x2*", 0, 40),
];

_ = ImmutableArray.CreateRange(
    ruleNames,
    x => world.Item(
        $"Rule {x.Name}",
        categories: world.Category("Rules"),
        giveItems: Brainpower(x.Brainpower),
        early: x.Early
    )
);

ImmutableArray<(string Name, Priority Priority, int Brainpower, bool IsStarting)> actionName =
[
    ("Don't Care", Priority.Progression, 115, false), ("Clear", Priority.Progression, 0, true),
    ("Bomb", Priority.Progression, 0, true), ("Hidden", Priority.Progression, 40, false),
    ("Trash", Priority.Progression, 80, false), ("Hint", Priority.Useful, 0, false),
];

_ = ImmutableArray.CreateRange(
    actionName,
    x => world.Item($"Action {x.Name}", x.Priority, world.Category("Actions"), giveItems: Brainpower(x.Brainpower))
);

var ruleCount = world.Item("+5 Rules", null, world.Category("Rules"), 20);

Logic LogicOf(Tile tile, int count)
{
    var difficulty = (int)tile * 1000 + count;

    // f\left(x\right)=\operatorname{floor}\left(\min\left(\max\left(\left(\frac{x}{d}\right)^{a},1\right),20\right)\right)
    // where d = 600
    // where a = 1.5
    var logic = ruleCount[(int)(difficulty / 600.0).Pow(1.5).Clamp(1, 20)] &
        Logic.ItemValue(Brainpower(1)[0].PhantomItemName, difficulty) &
        digits[0];

    return logic;
}

_ = world.Item("+100 Auto-Solve Maximum Regions", Priority.Useful, world.Category("Auto-Solve Regions"), 10);
var gameSpeed = world.Item("Progressive Game Speed", Priority.Useful, world.Category("Game Speed"), 17);
_ = world.Item("+0.2 Robots (rounded down)", Priority.Useful, world.Category("Robots"), 20);
_ = world.Item("Reset Levels", Priority.Trap, world.Category("Traps"), 4);
_ = world.Item("Enable Colors", Priority.Filler, count: 3);
_ = world.Item("Filter", Priority.Useful);
_ = world.Item("Paint", Priority.Useful);

ImmutableArray<(Tile Tile, int Count)> puzzles =
[
    (Tile.Hexagon, 5000),
    (Tile.Square, 4000),
    (Tile.Triangle, 3000),
    (Tile.Infinite, 2000),
];

world.Location($"Set game speed to {StartingGameSpeed}", categories: world.Category("Start"));
world.Location("Turn off colors", categories: world.Category("Start"));
world.Location("Reset Levels", categories: world.Category("Start"));
world.Location("Reset Rules", categories: world.Category("Start"));

foreach (var (tile, count) in puzzles)
    for (var i = 100; i <= count; i += 100)
        world.Location(
            $"Solve {i} {tile} Puzzles",
            LogicOf(tile, i),
            world.Category(tile.ToString()),
            options: tile is Tile.Infinite && i is 2000 ? LocationOptions.Victory : LocationOptions.None
        );

ImmutableArray<StartingItemBlock> startingItems =
[
    new(ruleCount, 1), new(gameSpeed, StartingGameSpeed),
    ..actionName.Where(x => x.IsStarting).Select(x => world.AllItems[$"Action {x.Name}"]),
];

Console.WriteLine(
    $"Maximum Brainpower: {world.AllItems.Sum(x => x.Count * x.GiveItems.OrEmpty().Sum(x => x.Amount))}\n\t{
        world.AllCategories
           .Select(world.AllItemsWith)
           .Select(x => x.ToArray())
           .Where(x => x is not [])
           .Select(x => $"{x[0].Categories[0]}: {x.Sum(static x => x.Count * x.GiveItems.OrEmpty().Sum(x => x.Amount))}")
           .Conjoin("\n\t")}"
);

await world.Game("Bombe", "RedsAndEmik", "BOOM", startingItems)
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);

enum Tile
{
    Hexagon,
    Square,
    Triangle,
    Infinite,
}
