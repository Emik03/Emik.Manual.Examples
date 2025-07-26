// ReSharper disable ArgumentsStyleLiteral RedundantExplicitParamsArrayCreation RedundantUsingDirective
#pragma warning disable GlobalUsingsAnalyzer, MA0047, RCS1110
using Emik.Manual;
using Emik.Manual.Domains;

World world = new();

await world.Game("Template", "Emik", "(filler)", []).DisplayExported(Console.WriteLine).ZipAsync(Path.GetTempPath());
