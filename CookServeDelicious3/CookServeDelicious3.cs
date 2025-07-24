// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
// ReSharper disable NullableWarningSuppressionIsUsed
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

static IAsyncEnumerable<SplitMemory<char, char, MatchAll>> Read([Match("^[^<>:\"/\\\\|?*]+$")] string path)
{
    var found = Path.Join(Environment.CurrentDirectory)
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn(", "));
}

World world = new();

const string Progressives =
    """
    7,Progression,Progressive Extra Prep Station
    4,Useful,Progressive Extra Holding Station
    3,Useful,Progressive Heat Lamps
    3,Useful,Progressive Cooking Time Regulator
    4,Useful,Progressive Rejuvenator
    3,Useful,Progressive Special System Analyzer
    6,Useful,Progressive Serving Cloner
    4,Useful,Progressive Food Truck Reinforcements
    4,Filler,Progressive Aroma Blower
    """;

foreach (var (count, (priority, (name, _))) in Progressives.SplitLines().Select(x => x.SplitOn(',')))
    world.Item(name, Enum.Parse<Priority>(priority.Span), world.Category("Truck Upgrades"), int.Parse(count.Span));

Dictionary<int, (string HS, string SO)> categoryIdToCategory = [];
Dictionary<int, List<string>> itemIdToCategories = [];
var foodJsonArray = JsonSerializer.Deserialize<JsonArray>(await File.ReadAllTextAsync("foodgloss.json"))!;
var foods = foodJsonArray.Where(x => x?["7"]?.ToString() is "Entree").OfType<JsonObject>().Select(Food.From).ToArray();
world.Category("Random Foods HS", true);
world.Category("Random Foods SO", true);

await foreach (var (id, (name, rest)) in Read("foods.csv"))
{
    categoryIdToCategory[int.Parse(id.Span)] = ($"{name} HS", $"{name} SO");

    switch (rest)
    {
        case ({ Span: "-1002" }, var (special, fields)): // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var food in foods)
                if (fields.Any(x => food.Match(special, x)))
                    (CollectionsMarshal.GetValueRefOrAddDefault(itemIdToCategories, food.Id, out _) ??= [])
                       .Add(food.Mode(name));

            break;
        case ({ Span: "-1003" or "-1005" }, _):
            foreach (var food in foods)
                (CollectionsMarshal.GetValueRefOrAddDefault(itemIdToCategories, food.Id, out _) ??= [])
                   .Add(food.Mode(name));

            break;
        default:
            foreach (var element in rest)
            {
                var foodId = int.Parse(element.Span);
                var food = foods.Single(x => x.Id == foodId);

                (CollectionsMarshal.GetValueRefOrAddDefault(itemIdToCategories, foodId, out _) ??= [])
                   .Add(food.Mode(name));
            }

            break;
    }
}

foreach (var (_, (hs, _)) in categoryIdToCategory)
    world.Category(hs);

foreach (var food in foods)
    world.Item(
        food.Name,
        Priority.Progression,
        [..itemIdToCategories[food.Id].Select(x => world.Category(x))]
    );

var territories = JsonSerializer.Deserialize<JsonArray>(await File.ReadAllTextAsync("route.json"))!;
Item previous = default;

for (var i = 0; i < territories.Count && territories[i] is var territory; i++)
{
    var shortName = territory!["shortname"]!.GetValue<string>();

    var routes = territory["routes"]!.AsArray()
       .Where(x => x?.GetValueKind() is not JsonValueKind.Null and not null)
       .ToList();

    for (var j = 0; j < routes.Count && routes[j] is var route; j++)
    {
        var days = route!["days"]!.AsArray();

        for (var k = 0; k < days.Count && days[k] is var day; k++)
        {
            var slot = int.Parse(day!["slot"]!.GetValue<string>());
            var serveOnlySlots = slot / 1000;
            var holdOnlySlots = slot % 10 + slot / 100 % 10;

            var categories = ((IEnumerable<JsonNode?>)[day["a"], day["b"], day["c"]])
               .Where(x => x?.GetValueKind() is not JsonValueKind.Null)
               .Select(x => x?.GetValue<int>())
               .Filter()
               .Where(x => x >= 0)
               .Select(x => categoryIdToCategory[x])
               .ToArray();

            var hs = categories is []
                ? world.AllCategories["Entire Menu HS"][holdOnlySlots]
                : categories.Select(x => world.AllCategories[x.HS][holdOnlySlots]).Or();

            var so = categories is []
                ? world.AllCategories["Entire Menu SO"][serveOnlySlots]
                : categories.Select(x => world.AllCategories[x.SO][serveOnlySlots]).Or();

            // ReSharper disable once UnusedVariable
            var chef = day["chef"] is { } c && c.GetValueKind() is not JsonValueKind.Null
                ? -c.GetValue<int>() % 100
                : 0;

            var minPrep = day["minprep"]!.GetValue<int>();
            var name = $"{shortName}; Route {(char)('A' + j)} Day {k + 1}";

            world.Location(
                name,
                world.AllItems["Progressive Extra Prep Station"][minPrep] &
                (previous.Name.IsEmpty ? (Logic?)null : previous) &
                (hs & so),
                [world.Category(shortName), world.Category($"{shortName}, Route {(char)('A' + j)}")]
            );

            if (k + 1 == days.Count && j + 1 == routes.Count)
                world.Location(
                    $"{name} LOGIC",
                    world.AllLocations[name].SelfLogic,
                    [..world.AllLocations[name].Categories],
                    options: LocationOptions.Victory,
                    allowList: previous = world.Item(name, Priority.Progression, world.Category("Completion", true))
                );
        }
    }
}

world.Item("Progressive One Third of a Day Skip", Priority.Useful, world.Category("Thirds of a Day Skip"), 129);

var game = world.Game(
    "CookServeDelicious3",
    "EmikAndBagels",
    "Cook a meal IRL!",
    [..foods.Where(x => x.Difficulty is 0).Select(x => new StartingItemBlock(world.AllItems[x.Name]))]
);

await game.DisplayExported(Console.WriteLine).ZipAsync(Path.GetTempPath(), listChecks: true);

sealed record Food(string Name, int Difficulty, HoldingStation Station, int Id, string PriorGames, bool AutoServe)
{
    public static Food From(JsonObject obj) =>
        new(
            obj["0"]!.GetValue<string>(),
            obj["5"]!.GetValue<int>(),
            Enum.Parse<HoldingStation>(obj["8"]!.GetValue<string>()),
            obj["9"]!.GetValue<int>(),
            obj["27"]!.GetValue<string>(),
            obj.TryGetPropertyValue("50", out _)
        );

    public bool Match(ReadOnlyMemory<char> special, ReadOnlyMemory<char> next) =>
        special.Span switch
        {
            "5" => Difficulty == int.Parse(next.Span),
            "50" => AutoServe == next.Span is "autoserve",
            "27" => PriorGames.AsSpan().Equals(next.Span, StringComparison.Ordinal),
            _ => throw new ArgumentOutOfRangeException(nameof(special), special, null),
        };

    public string Mode(ReadOnlyMemory<char> memory) => $"{memory} {(Station is HoldingStation.None ? "SO" : "HS")}";
}

enum HoldingStation
{
    None,
    HSRequired,
    HSOptional,
}
