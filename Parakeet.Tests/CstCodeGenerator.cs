using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests;

public static class CstCodeGenerator
{
    public static IEnumerable<Grammar> Grammars
        => AllGrammars.Grammars;

    [Test, TestCaseSource(nameof(Grammars))]
    public static void OutputCstCode(Grammar g)
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