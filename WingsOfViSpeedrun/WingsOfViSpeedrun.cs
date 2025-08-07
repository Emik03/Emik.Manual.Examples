// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

// ReSharper disable WrongIndentSize
static ImmutableArray<(Chars PhantomItemName, int Amount)> TimeItemValue(int seconds) => [("Time Saved", seconds)];

static Logic? TimeLogic(int minutes) => Logic.ItemValue("Time Saved", (114 - minutes) * 15);

World world = new();

const Priority MinorSkips = Priority.Useful,
   Skips = Priority.Progression,
   MajorSkips = Priority.ProgressionUseful;

var theFallenKeep = world.Category("The Fallen Keep");
world.Item("Pre-Flutter Grabber Damage Boost", MinorSkips, theFallenKeep);
world.Item("Double Turret Damage Boost", MinorSkips, theFallenKeep);
world.Item("Keep 2 Damage Boost", MinorSkips, theFallenKeep);

var terrestrialVale = world.Category("Terrestrial Vale");
world.Item("Falling Skip", MajorSkips, terrestrialVale, giveItems: TimeItemValue(110));
world.Item("Electram Checkpoint Skip", Skips, terrestrialVale, giveItems: TimeItemValue(25));
world.Item("Vale 2 First Checkpoint Damage Boost", MinorSkips, terrestrialVale);
world.Item("Vale 2 Skip", MajorSkips, terrestrialVale, giveItems: TimeItemValue(65));
world.Item("Windmill Damage Boost", MinorSkips, terrestrialVale);
world.Item("Cave Skip", Skips, terrestrialVale, giveItems: TimeItemValue(30));
world.Item("Amethyst Checkpoint Skip", Skips, terrestrialVale, giveItems: TimeItemValue(15));

var sinisterGrotto = world.Category("Sinister Grotto");
world.Item("Grotto Mine Damage Boost", Skips, sinisterGrotto, giveItems: TimeItemValue(20));
world.Item("Spiders Damage Boost", MinorSkips, sinisterGrotto);
world.Item("Grotto Plant Damage Boost", Skips, sinisterGrotto, giveItems: TimeItemValue(15));
world.Item("Dark Annihilator Wrath Skip", Skips, sinisterGrotto, giveItems: TimeItemValue(130));

var theUnderworld = world.Category("The Underworld");
world.Item("Underworld Top Split Checkpoint Skip", Skips, theUnderworld, giveItems: TimeItemValue(35));
world.Item("Underworld Right Split Checkpoint Skip", MajorSkips, theUnderworld, giveItems: TimeItemValue(120));
world.Item("Underworld Bottom Split Checkpoint Skip", Skips, theUnderworld, giveItems: TimeItemValue(60));
world.Item("Ancient Constructs Skip", MajorSkips, theUnderworld, giveItems: TimeItemValue(90));

var acropolisOfAnguish = world.Category("Acropolis of Anguish");
world.Item("Library Start Damage Boost", MinorSkips, acropolisOfAnguish);
world.Item("Library Entry Damage Boost", MinorSkips, acropolisOfAnguish);
world.Item("Library Skip", Skips, acropolisOfAnguish, giveItems: TimeItemValue(90));
world.Item("Myougi 2 Enrage Skip", MajorSkips, acropolisOfAnguish, giveItems: TimeItemValue(60));

var theChambers = world.Category("The Chambers");
world.Item("The Chambers Entry Damage Boost", MinorSkips, theChambers);
world.Item("The Summoning Chamber Checkpoint Skip", Skips, theChambers, giveItems: TimeItemValue(30));
world.Item("Twin Orgs Fast Kill", MajorSkips, theChambers, giveItems: TimeItemValue(65));

var pathOfDecay = world.Category("Path of Decay");
world.Item("Path of Decay Entry Damage Boost", MinorSkips, pathOfDecay);

world.AllItems.Where(x => x.Priority is MajorSkips)
   .Lazily(x => world.Location($"Get consistent at {x}", x, world.Category("Consistency")))
   .Enumerate();

var runs = world.Category("Runs");
world.Location("Finish a Run", null, runs);

for (var i = 120; i >= 51; i -= 3)
   world.Location($"Get a sub-{i.ToString(CultureInfo.InvariantCulture)} run", TimeLogic(i), runs);

world.Location("Get a sub-50 run", TimeLogic(50), runs, options: LocationOptions.Victory);

await world.Game("WingsOfViSpeedrun", "RedsAndEmik", "Change Outfit", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
