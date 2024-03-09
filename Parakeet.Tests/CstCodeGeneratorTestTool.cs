using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;
using System.Xml.Linq;

namespace Ara3D.Parakeet.Tests;

public static class CstCodeGeneratorTestTool
{
    public static IEnumerable<Grammar> Grammars
        => AllGrammars.Grammars;

    /// <summary>
    /// Generates the source files for the CST project.
    /// This includes CstNode factories: which convert from AST nodes to CST nodes
    /// and the definitions for the various CstNode classes. 
    /// </summary>
    [Test, TestCaseSource(nameof(Grammars))]
    public static void GenerateCstCode(Grammar g)
    {       
        var name = g.GetType().Name;
        var folder = Folders.CstOutputFolder;

        var nameSpace = $"Ara3D.Parakeet.Cst.{name}NameSpace";
        {
            var cb = new CodeBuilder();
            CstCodeBuilder.OutputCstClassesFile(cb, g, nameSpace);
            var path = folder.RelativeFile($"{name}Cst.cs");
            var text = cb.ToString();
            Console.WriteLine(text);
            path.WriteAllText(text);
        }
        {
            var cb = new CodeBuilder();
            CstCodeBuilder.OutputCstFactoryFile(cb, g, nameSpace);
            var path = folder.RelativeFile($"{name}CstFactory.cs");
            var text = cb.ToString();
            Console.WriteLine(text);
            path.WriteAllText(text);
        }
    }

    [Test]
    public static void GenerateAllCstFactories()
    {
        var cb = new CodeBuilder();
        var path = Folders.CstOutputFolder.RelativeFile($"AllCstFactories.cs");
        cb.WriteLine($"namespace Ara3D.Parakeet.Cst");
        cb.WriteStartBlock();
        cb.WriteLine($"public static class AllCstFactories");
        cb.WriteStartBlock();
        foreach (var g in Grammars)
        {
            var name = g.GetType().Name;
            var nameSpace = $"{name}NameSpace";
            name = name.RemoveSuffix("Grammar");
            cb.WriteLine($"public static CstNode {name}(ParserTreeNode input) => (new {nameSpace}.CstNodeFactory()).Create(input);");
        }
        cb.WriteEndBlock();
        cb.WriteEndBlock();
        var text = cb.ToString();
        Console.WriteLine(text);
        path.WriteAllText(text);
    }
}