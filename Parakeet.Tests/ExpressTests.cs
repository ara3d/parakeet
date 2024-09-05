using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests;

public static class ExpressTests
{
    public static PlatoGrammar Grammar = PlatoGrammar.Instance;
    public static DirectoryPath InputFolder = PathUtil.GetCallerSourceFolder().RelativeFolder("..", "input", "exp");

    [Test]
    [TestCase("IFC2X3.exp")]
    [TestCase("IFC4X3.exp")]
    public static void TestFile(string file)
    {
        var pi = ParserInput.FromFile(InputFolder.RelativeFile(file));
        ParserTests.ParseTest(pi, Grammar.File);
    }
}