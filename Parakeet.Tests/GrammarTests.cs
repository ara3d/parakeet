using System.Text;
using System.Xml.Linq;
using Ara3D.Utils;
using Parakeet.Grammars;

namespace Parakeet.Tests;

public static class GrammarTests
{
    public static IEnumerable<Grammar> Grammars
        => AllGrammars.Grammars;

    [Test, TestCaseSource(nameof(Grammars))]
    public static void OptimizerTest(Grammar g)
    {

        Console.WriteLine("Rule Optimization");
        var ro = new RuleOptimizer(g);

        foreach (var kv in ro.OptimizedRules)
        {
            Console.WriteLine("Original");
            Console.WriteLine(kv.Key);

            Console.WriteLine("Optimized");
            Console.WriteLine(kv.Value);
        }
    }

    public static string GetGrammarDef(Grammar g)
    {
        var sb = new StringBuilder();
        foreach (var r in g.GetRules())
        {
            sb.AppendLine(r.GetName() + " := " + r.Body().ToDefinition());
        }
        return sb.ToString();
    }

    [Test, TestCaseSource(nameof(Grammars))]
    public static void OutputDefinitions(Grammar g)
    {
        var name = g.GetType().Name;
        var folder = Folders.BaseOutputFolder.RelativeFolder(name).Create();
        {
            var s = GetGrammarDef(g);
            Console.WriteLine(s);
            folder.RelativeFile("grammar.txt").WriteAllText(s);
        }
    }

    [Test, TestCaseSource(nameof(Grammars))]
    public static void OutputCstCode(Grammar g)
    {
        var name = g.GetType().Name;
        var folder = Folders.CstOutputFolder;

        var nameSpace = $"Parakeet.Cst.{name}NameSpace";
        {
            var cb = new CodeBuilder();
            CstCodeBuilder.OutputCstClassesFile(cb, g, nameSpace);
            var path = folder.RelativeFile($"{name}Cst.cs");
            var text = cb.ToString();
            Console.WriteLine(text);
            File.WriteAllText(path, text);
        }
        {
            var cb = new CodeBuilder();
            CstCodeBuilder.OutputCstFactoryFile(cb, g, nameSpace);
            var path = folder.RelativeFile($"{name}CstFactory.cs");
            var text = cb.ToString();
            Console.WriteLine(text);
            File.WriteAllText(path, text);
        }
    }

}