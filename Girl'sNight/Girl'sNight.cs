// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable CS0168, CS0414, CS8907, IDE0090, GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

var world = new World();

foreach (var character in Night.Characters)
    world.Item($"-5 {character}", categories: world.Category(character), count: 7);

ImmutableArray<string> times = ["0:55", "1:50", "2:45", "Clear"];

foreach (var night in Night.Nights)
{
    var category = world.Category(night.Name);

    for (var i = 0; i < times.Length && times[i] is var time; i++)
    {
        var logic = night.Logic(world, i);
        var hardLogic = night.HardLogic(world, i);
        var hardEntrance = night.ExplicitHardName is "Gatekeep Gaslight" ? $"{night.Name} Hard" : null;
        world.Location($"{night.Name} {time}", logic, category);
        world.Location($"{night.HardName} {time}", hardLogic, category, hintEntrance: hardEntrance);
    }
}

var challenges = world.Category("Challenges");

var challengeLogic = world.Item("Disable Long Nights", categories: challenges) &
    world.Item("Disable Perma Door Break", categories: challenges) &
    world.Item("Disable Aggressive Swing", categories: challenges) &
    world.Item("Disable Aggressive Swave", categories: challenges) &
    world.Item("Disable Insistent Infection", categories: challenges);

for (var i = 0; i < times.Length && times[i] is var time; i++)
    world.Location(
        $"Girlboss {time}",
        world.Item("Girlboss", Priority.ProgressionUseful, world.AllCategories["Levels"]) &
        (new Yaml("Girlboss") > i | challengeLogic) &
        world.Location($"Catfight Crashout {time}").Logic &
        world.Location($"Gatekeep Gaslight {time}").Logic,
        world.Category("Girlboss"),
        options: time == times[^1] ? LocationOptions.Victory : LocationOptions.None
    );

var encouragement = world.Category("Words of Encouragement");

foreach (var line in Night.GirlQuotes)
    world.Item(line, Priority.Filler, encouragement);

var endless = world.Category("Endless");
world.Location("Complete a run in Endless (Hard)", categories: endless);
world.Location("Complete a run in Endless (Normal)", categories: endless);

await world.Game("Girl'sNight", "Emik", "Girlpower!", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);

// ReSharper disable ArrangeObjectCreationWhenTypeEvident NotAccessedPositionalProperty.Global UnusedMember.Local UnusedParameter.Local
readonly partial record struct Night(
    string Name,
    string? ExplicitHardName = null,
    int Swing = 0,
    int Shock = 0,
    int Swave = 0,
    int Pam = 0,
    int Nekuu = 0,
    int HadalFortune = 0,
    int BowtAndTurtlebot = 0,
    int DDub = 0,
    int DTurtle = 0
)
{
    static readonly SearchValues<char> s_yamlRemoval = SearchValues.Create("()' ");

    public static IEnumerable<string> Characters { get; } =
        ["Swing", "Shock", "Swave", "Pam", "Nekuu", "Hadal Fortune", "Bowt and Turtlebot", "DDub", "DTurtle"];

    public static IEnumerable<string> GirlQuotes { get; } =
    [
        "Fierce!",
        "Serve!",
        "Slay!",
        "Us girls stick together!",
        "You got this, queen!",
        "You got this, girl!",
        "You go, queen!",
        "You go, girl!",
        "Let's go, girliepop!",
        "Girl, what's your skincare routine?",
        "This girl is on fire!",
        "Look out, girl!",
        "Girl Dinner!",
        "Women's Rights!",
        "Women's Wrongs!",
        "God forbid, women do anything!",
        "Who runs the world... girls...",
        "So true, bestie!",
        "God is a woman!",
        "Estrogen",
    ];

    public static IEnumerable<Night> Nights { get; } =
    [
        new Night("Female Leads", Swing: 20, Shock: 15, Swave: 20, DDub: 15),
        new Night("Pamdemonium", Swing: 10, Swave: 25, Pam: 30, Nekuu: 15),
        new Night("Cootie Catastrophe", Shock: 20, Pam: 20, HadalFortune: 20, DDub: 20, DTurtle: 20),
        new Night("Camera Chaos", Swing: 5, Swave: 20, Pam: 15, Nekuu: 35, BowtAndTurtlebot: 15, DTurtle: 5),
        new Night("Shock Scoundrels", Shock: 30, Nekuu: 15, HadalFortune: 20, BowtAndTurtlebot: 30, DDub: 35),
        new Night("Freakshow", Nekuu: 25, HadalFortune: 35, BowtAndTurtlebot: 35, DDub: 15, DTurtle: 15),
        new Night("Middle(wo)men", Shock: 30, Swave: 30, Pam: 30, BowtAndTurtlebot: 35, DDub: 35),
        new Night("Cupcake Challenge", Swing: 20, Shock: 25, Pam: 25, Nekuu: 20, BowtAndTurtlebot: 30, DDub: 30),
        new Night("Flute Frenzy", Swing: 30, Shock: 20, Nekuu: 30, HadalFortune: 30, BowtAndTurtlebot: 25, DTurtle: 30),
        new Night(25, "Gal Pals"),
        new Night(30, "Bop's Quest for Women"),
        new Night(35, "Catfight Crashout", "Gatekeep Gaslight"),
    ];

    public string DisplayName => ExplicitHardName is null ? Name : $"{Name}/{ExplicitHardName}";

    public string HardName => ExplicitHardName ?? $"{Name} Hard";

    public Night(int all, string name, string? explicitHardName = null)
        : this(name, explicitHardName, all, all, all, all, all, all, all, all, all) { }

    public Logic HardLogic(World world, int iteration) => Logic(world, iteration, x => x.HardName);

    public Logic Logic(World world, int iteration) => Logic(world, iteration, x => x.Name);

    Night Diff(Night other) =>
        new Night(
            "Custom Night",
            "Custom Night",
            Swing - other.Swing,
            Shock - other.Shock,
            Swave - other.Swave,
            Pam - other.Pam,
            Nekuu - other.Nekuu,
            HadalFortune - other.HadalFortune,
            BowtAndTurtlebot - other.BowtAndTurtlebot,
            DDub - other.DDub,
            DTurtle - other.DTurtle
        );

    Logic Logic(World world, int iteration, Converter<Night, string> converter)
    {
        var ret = LocalLogic(world);

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var night in Nights)
            ret |= new Yaml(converter(night).SplitSpanOn(s_yamlRemoval).ToString()) > iteration &
                Diff(night).LocalLogic(world);

        return world.Item(DisplayName, Priority.ProgressionUseful, world.Category("Levels")) & ret;
    }

    Logic? LocalLogic(World world)
    {
        Logic? ret = null;

        foreach (var character in Characters)
            if (character switch
            {
                "Pam" => Pam,
                "DDub" => DDub,
                "Swing" => Swing,
                "Shock" => Shock,
                "Swave" => Swave,
                "Nekuu" => Nekuu,
                "DTurtle" => DTurtle,
                "Hadal Fortune" => HadalFortune,
                "Bowt and Turtlebot" => BowtAndTurtlebot,
                _ => throw Unreachable,
            } is > 0 and var amount)
                ret &= world.AllItemsWith(character).Single()[amount / 5];

        return ret;
    }
}
