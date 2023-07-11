using Parakeet.Demos;

namespace Parakeet.Tests;

public static class PlatoTests
{
    public static PlatoGrammar Grammar = new PlatoGrammar();

    [Test]
    [TestCase("C:\\Users\\cdigg\\git\\plato\\PlatoStandardLibrary\\modules.plato")]
    [TestCase("C:\\Users\\cdigg\\git\\plato\\PlatoStandardLibrary\\types.plato")]
    [TestCase("C:\\Users\\cdigg\\git\\plato\\PlatoStandardLibrary\\concepts.plato")]
    public static void TestFile(string file)
    {
        var pi = ParserInput.FromFile(file);
        ParserTests.ParseTest(pi, Grammar.File);
    }


    public static string[] Literals =
    {
        "42",
        "\"Hello world\"",
        "'q'",
        "true",
        "false",
        "3.14e-10",
        "-3.14E+10",
        "0.001",
    };

    public static string[] Identifiers =
    {
        "a",
        "abc",
        "ab_cd",
    };

    public static string[] Expressions =
    {
        "f()",
        "abc(123)",
        "f(g(), h())",
    };

    public static string[] Statements =
    {
        "a = x",
        "let x = 12",
        "break",
        "continue",
        "return x",
        "if x then a else b",
        "_",
        "x",
        "f()",
        "f(g(h(123)))",
        "{}",
        "{ }",
        "{ {}; {}; }",
        "{ break; }",
        "{ break; continue; let x = _; { }; return x; f(); }",
        "if true then { } else { }",
        "while true do { }",
        "while false do f()",
        "if if x then y else z then a else b",
        "while x do { a; b(); x = 12; }",
    };

    [Test]
    [TestCaseSource(nameof(Identifiers))]
    [TestCaseSource(nameof(Literals))]
    [TestCaseSource(nameof(Expressions))]
    public static void TestExpressions(string input)
        => ParserTests.SingleParseTest(input, Grammar.Expression);

    [Test]
    public static void TestThings()
    {
        ParserTests.SingleParseTest($"field F: Type;", Grammar.FieldDeclaration);
        ParserTests.SingleParseTest($"field Field: Element<T>;", Grammar.FieldDeclaration);
        ParserTests.SingleParseTest($"function f();", Grammar.MethodDeclaration);
        ParserTests.SingleParseTest($"function f(a);", Grammar.MethodDeclaration);
        ParserTests.SingleParseTest($"function f(a: Number): String;", Grammar.MethodDeclaration);
        ParserTests.SingleParseTest($"function f(a) = 42;", Grammar.MethodDeclaration);
        ParserTests.SingleParseTest($"function f(a: Number): String = a + a;", Grammar.MethodDeclaration);
    }
}