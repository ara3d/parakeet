using Parakeet.Demos;

namespace Parakeet.Tests
{
    public static class JsonTests
    {
        public static JsonGrammar Grammar = new JsonGrammar();

        [Test]
        [TestCase("1", nameof(JsonGrammar.Value))]
        [TestCase("true", nameof(JsonGrammar.Value))]
        [TestCase("false", nameof(JsonGrammar.Value))]
        [TestCase("null", nameof(JsonGrammar.Value))]
        [TestCase("\"Hello world\"", nameof(JsonGrammar.Value))]
        [TestCase("1", nameof(JsonGrammar.Number))]
        [TestCase("-1", nameof(JsonGrammar.Number))]
        [TestCase("0", nameof(JsonGrammar.Number))]
        [TestCase("123", nameof(JsonGrammar.Number))]
        [TestCase("123.45", nameof(JsonGrammar.Number))]
        [TestCase("123.45e00", nameof(JsonGrammar.Number))]
        [TestCase("123.43e+123", nameof(JsonGrammar.Number))]
        [TestCase("123e-123", nameof(JsonGrammar.Number))]
        [TestCase("\"\"", nameof(JsonGrammar.String))]
        [TestCase("\"a\"", nameof(JsonGrammar.String))]
        [TestCase("\"a b c\"", nameof(JsonGrammar.String))]
        [TestCase("\"a \\\" c\"", nameof(JsonGrammar.String))]
        [TestCase("\"a \\\\ c\"", nameof(JsonGrammar.String))]
        [TestCase("\"\\t\"", nameof(JsonGrammar.String))]
        [TestCase("[]", nameof(JsonGrammar.Array))]
        [TestCase("[1]", nameof(JsonGrammar.Array))]
        [TestCase("[1,2]", nameof(JsonGrammar.Array))]
        [TestCase("[ 1, 2,  3 , 4]", nameof(JsonGrammar.Array))]
        [TestCase("[1,[], 2]", nameof(JsonGrammar.Array))]
        [TestCase("[[]]", nameof(JsonGrammar.Array))]
        [TestCase("[ [ ] ]", nameof(JsonGrammar.Array))]
        [TestCase("[[],[],[[]]]", nameof(JsonGrammar.Array))]
        [TestCase("[[1],[2],[[3],4]]", nameof(JsonGrammar.Array))]
        [TestCase("{}", nameof(JsonGrammar.Object))]
        [TestCase("{ }", nameof(JsonGrammar.Object))]
        [TestCase("{\"abc\":42}", nameof(JsonGrammar.Object))]
        [TestCase("{ \"abc\" : 42 }", nameof(JsonGrammar.Object))]
        public static void TargetedTest(string input, string name)
        {
            var rule = Grammar.GetRuleFromName(name);
            var result = ParserTests.ParseTest(input, rule);
            Assert.IsTrue(result == 1);
        }


        [Test]
        public static void JsonTest()
        {
            var file = Path.Combine(ParserTests.InputFilesFolder, "twitter.json");
            var input = ParserInput.FromFile(file);
            var ps = input.Parse(Grammar.Json);
            Assert.IsNotNull(ps);
            var node = ps.Node;
            Assert.IsNotNull(node);
            var tree = node.ToParseTree();
            Assert.IsNotNull(tree);
            tree.OutputTree();
        }
    }
}