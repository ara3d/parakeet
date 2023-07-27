namespace Parakeet.Tests;

public static class CstCodeGenerator
{
    [Test, Explicit]
    public static void OutputCst()
    {
        OutputCstCode(CSharpTests.Grammar, "CSharp");
        OutputCstCode(JsonTests.Grammar, "Json");
        OutputCstCode(PlatoTests.Grammar, "Plato");
    }

    public static void OutputCstCode(Grammar g, string name)
    {
        {
            var cb = new CodeBuilder();
            CstCodeBuilder.OutputCstClassesFile(cb, $"Parakeet.Demos.{name}", g.GetRules());
            var path = Path.Combine(ParserTests.DemosProjectFolder, "_generated", $"{name}Cst.cs");
            var text = cb.ToString();
            Console.WriteLine(text);
            File.WriteAllText(path, text);
        }
        {
            var cb = new CodeBuilder();
            CstCodeBuilder.OutputCstFactoryFile(cb, $"Parakeet.Demos.{name}", g.GetRules());
            var path = Path.Combine(ParserTests.DemosProjectFolder, "_generated", $"{name}CstFactory.cs");
            var text = cb.ToString();
            Console.WriteLine(text);
            File.WriteAllText(path, text);
        }
    }
}