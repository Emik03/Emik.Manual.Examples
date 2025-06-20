// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

World world = new(Priority.Useful);
6.For(i => world.Location($"Apply for {(i + 1).Conjugate("job")}")).Enumerate();
world.Location("Submit activity report", options: LocationOptions.Victory);
world.Item("LinkedIn", Priority.Trap);
world.Item("Do household chores");
world.Item("Movement");
world.Item("Pet cats");

await world.Game("Employment", "Emik", "Email check", [])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
