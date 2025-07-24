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

foreach (var allowedName in (ImmutableArray<string>)["Ships", "Systems"])
    world.Category(allowedName);

await foreach (var line in Read("FTL-Shops.csv"))
{
    var (categoryName, (firstPriority, (secondPriority, rest))) = line;
    var priority = Enum.Parse<Priority>(firstPriority.Span) | Enum.Parse<Priority>(secondPriority.Span);
    var category = world.Category(categoryName);

    foreach (var item in rest)
        world.Item(item, priority, [category]);
}

await foreach (var line in Read("FTL-Progressives.csv"))
{
    var (priority, (name, (quantity, (category, _)))) = line;
    world.Item(name, Enum.Parse<Priority>(priority.Span), [category], int.Parse(quantity.Span));
}

await foreach (var line in Read("FTL-Ships.csv"))
{
    var (ship, (a, (b, (c, _)))) = line;
    ImmutableArray<ReadOnlyMemory<char>> ships = c.IsEmpty ? [a, b] : [a, b, c];
    var category = world.Category(ship);
    var shipIt = world.Item($"Progressive {ship}", Priority.ProgressionUseful, "Ships", ships.Length);

    for (var i = 2; i <= 8; i += 2)
        for (var j = 0; j < ships.Length; j++)
        {
            var shipItem = shipIt[j + 1];

            var logic = i switch
            {
                2 or 4 => shipItem,
                6 => shipItem &
                    (new Logic("Repairs") | "Fuel") &
                    (new Logic("Progressive Shield", 2) |
                        ("Progressive Engine", 4) |
                        ("Progressive Weapon Control", 5) |
                        ("Progressive Drone Control", 4) |
                        ("Progressive Cloaking", 2) |
                        ("Progressive Crew Teleporter", 2)),
                8 => shipItem &
                    "Fuel" &
                    "Repairs" &
                    ("Progressive Shield", 3) &
                    ("Progressive Engine", 3) &
                    ("Progressive Weapon Control", 5) &
                    (new Logic("Progressive Mind Control", 1) |
                        ("Progressive Drone Control", 1) |
                        ("Progressive Crew Teleporter", 1) |
                        ("Progressive Hacking", 1) |
                        ("Progressive Cloaking", 1)),
                _ => throw Unreachable,
            };

            world.Location($"{ships[j]} Reach Sector {i}", logic, [category]);
        }

    for (var j = 0; j < ships.Length; j++)
        for (var k = 1; k <= 2; k++)
            world.Location(
                $"{ships[j]} Victory {k}",
                shipIt[j + 1] &
                "Fuel" &
                "Repairs" &
                ("Progressive Shield", 4) &
                ("Progressive Engine", 4) &
                ("Progressive Weapon Control", 5) &
                (new Logic("Progressive Cloaking", 1) |
                    ("Progressive Hacking", 2) |
                    ("Progressive Drone Control", 4) |
                    ("Progressive Crew Teleporter", 1)),
                [category]
            );
}

var anyShipUnlocked = world.AllItemsWithAny(["Ships"]).Select(x => new Logic(x)).Aggregate((x, y) => x | y);
var hardProgression = world.Category("Hard Progression");

foreach (var difficulty in (ImmutableArray<string>)["Normal", "Hard"])
{
    for (var i = 2; i <= 8; i += 2)
    {
        var logic = i switch
        {
            2 => anyShipUnlocked,
            4 => anyShipUnlocked & "Repairs" & ("Progressive Shield", 2),
            6 => anyShipUnlocked & "Fuel" & "Repairs" & ("Progressive Shield", 3) & ("Progressive Weapon Control", 4),
            8 => anyShipUnlocked &
                "Fuel" &
                "Repairs" &
                ("Progressive Shield", 3) &
                ("Progressive Weapon Control", 4) &
                (new Logic("Progressive Cloaking", 1) | ("Progressive Hacking", 1)),
            _ => throw Unreachable,
        };

        world.Location($"{difficulty} Mode Reach Sector {i}", logic, [hardProgression]);
    }

    for (var j = 1; j <= 2; j++)
        world.Location(
            $"{difficulty} Mode Victory {j}",
            anyShipUnlocked &
            "Fuel" &
            "Repairs" &
            ("Progressive Shield", 4) &
            ("Progressive Engine", 6) &
            ("Progressive Weapon Control", 7) &
            ("Progressive Reactor", 12) &
            ("Progressive Cloaking", 2) &
            ("Progressive Hacking", 2) &
            ("Progressive Drone Control", 4) &
            ("Progressive Crew Teleporter", 2),
            [hardProgression]
        );
}

world.Location(
    "Win with All Layouts",
    world.AllItemsWithAny(["Ships"]).Select(x => x[^0]).Aggregate((x, y) => x & y),
    options: LocationOptions.Victory
);

world.Category("Achievements");
Dictionary<string, int> dict = [];

void Achievement(
    string name,
    string ship,
    Logic? logic
) =>
    world.Location(
        $"{name} ({ship} ACH {(dict.ContainsKey(name) ? dict[name]++ : dict[name] = 1)})",
        world.Item($"Progressive {ship}")[1] & logic,
        ["Achievements"]
    );

Achievement(
    "The United Federation",
    "Kestrel",
    world.Location("The Kestrel (Kestrel A) Reach Sector 6").SelfLogic
);

Achievement(
    "Full Arsenal",
    "Kestrel",
    new Logic("Progressive Cloaking", 1) &
    ("Progressive Hacking", 1) &
    ("Progressive Drone Control", 1) &
    ("Progressive Mind Control", 1) &
    ("Progressive Backup Battery", 1)
);

Achievement("Tough Little Ship", "Kestrel", "Repairs");
Achievement("Robotic Warfare", "Engi", ("Progressive Drone Control", 5));
Achievement("I hardly lifted a finger", "Engi", ("Progressive Drone Control", 5));

Achievement(
    "The guns... They've stopped",
    "Engi",
    ("Progressive Weapon Control", 5) & (new Logic("Heavy Ion") & "Ion Blast" | "Ion Blast II")
);

Achievement("Master of Patience", "Fed", ("Progressive Artillery Beam", 3));
Achievement("Diplomatic Immunity", "Fed", world.Location("The Osprey (Fed A) Reach Sector 6").SelfLogic);

Achievement(
    "Artillery Mastery",
    "Fed",
    ("Progressive Artillery Beam", 3) & world.Location("The Osprey (Fed A) Reach Sector 6").SelfLogic
);

Achievement("Shields Holding", "Zoltan", ("Progressive Cloaking", 1));
Achievement("Givin' her all she's got, Captain!", "Zoltan", ("Progressive Reactor", 17));
Achievement("Manpower", "Zoltan", world.Location("The Adjudicator (Zoltan A) Reach Sector 6").SelfLogic);
Achievement("Take no prisoners!", "Mantis", world.Location("The Gila Monster (Mantis A) Reach Sector 6").SelfLogic);

Achievement(
    "Avast, ye scurvy dogs!",
    "Mantis",
    world.Location("The Gila Monster (Mantis A) Reach Sector 8").SelfLogic & ("Progressive Shield", 3)
);

Achievement("Battle Royale", "Mantis", world.Location("The Gila Monster (Mantis A) Reach Sector 6").SelfLogic);
Achievement("We're in Position!", "Slug", ("Progressive Crew Teleporter", 1));
Achievement("Home Sweet Home", "Slug", world.Location("Man of War (Slug A) Reach Sector 8").SelfLogic);
Achievement("Disintegration Ray", "Slug", world.Location("Man of War (Slug A) Reach Sector 8").SelfLogic);

Achievement(
    "Is it warm in here?",
    "Rock",
    (new Logic("Fire Beam") | "Fire Bomb") &
    ("Progressive Crew Teleporter", 1)
);

Achievement(
    "Defense Drones Don't Do D'anything!",
    "Rock",
    new Logic("Defense Scrambler") |
    "Hermes Missile" |
    "Breach Missiles" |
    "Hull Missile" |
    "Swarm Missiles" |
    "Pegasus Missile"
);

Achievement(
    "Ancestry",
    "Rock",
    new Logic("Progressive Rock", 3) & world.Location("Tektite (Rock C) Reach Sector 8").SelfLogic
);

Achievement(
    "Bird of Prey",
    "Stealth",
    ("Progressive Stealth", 2)
);

Achievement(
    "Phase Shift",
    "Stealth",
    world.Location("The Nesasio (Stealth A) Reach Sector 8").SelfLogic & ("Progressive Cloaking", 2)
);

Achievement(
    "Tactical Approach",
    "Stealth",
    world.Location("The Nesasio (Stealth A) Reach Sector 8").SelfLogic
);

Achievement(
    "Advanced Mastery",
    "Lanius",
    new Logic("Progressive Hacking", 1) & ("Progressive Mind Control", 1) & ("Progressive Backup Battery", 1)
);

Achievement(
    "Scrap Hoarder",
    "Lanius",
    world.Location("Kruos (Lanius A) Reach Sector 8").SelfLogic & "Scrap Recovery Arm"
);

Achievement(
    "Loss of Cabin Pressure",
    "Lanius",
    world.Location("Kruos (Lanius A) Reach Sector 8").SelfLogic
);

Achievement("Sweet Revenge", "Crystal", null);
Achievement("No Escape", "Crystal", world.Location("Bravais (Crystal A) Reach Sector 6").SelfLogic);
Achievement("Clash of the Titans", "Crystal", world.Location("Bravais (Crystal A) Reach Sector 8").SelfLogic);

await world.Game("FTL", "RedsAndEmik", "Free One-Time System Upgrade", [new(world.AllCategories["Ships"], 1)])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
