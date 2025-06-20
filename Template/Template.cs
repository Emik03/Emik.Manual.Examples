// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

World world = new();

var game = world.Game("Template", "Emik", "(filler)", []);
await game.ZipAsync(Path.GetTempPath());
Console.WriteLine($"{game.ExportedLocationCount()}/{game.ExportedItemCount()}");
