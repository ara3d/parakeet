namespace Parakeet.Tests
{
    public static class CstCodeGenerator
    {
        [Test, Explicit]
        public static void OutputCSharpCst()
        {
            OutputCstCode(CSharpTests.Grammar, "CSharp");
        }


        [Test, Explicit]
        public static void OutputJsonCst()
        {
            OutputCstCode(JsonTests.Grammar, "Json");
        }

        [Test, Explicit]
        public static void OutputPlatoCst()
        {
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
}