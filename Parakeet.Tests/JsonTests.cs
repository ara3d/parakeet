using NUnit.Framework;
using Parakeet.Demos;
using System;

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
        [TestCase("{\"Id\" :789 ,\"Name\":\"Albert\\tSimple\",\"Status\":\"Married\",\"Address\": \"Planet Earth\", \"Scores\":[1,2,3,4,5,6,7,8,9,10],\"Data\":null}\r\n", nameof(JsonGrammar.Json))]
        public static void TargetedTest(string input, string name)
        {
            var rule = Grammar.GetRuleFromName(name);
            var result = ParserTests.ParseTest(input, rule);
            Assert.IsTrue(result == 1);
        }

        [TestCase("twitter.json")]
        [TestCase("1900b0aE.dag.json")]
        [TestCase("59ba4880.json")]
        [TestCase("7D0144EC2C5F43E42EF6587E214E857ABF59718F.json")]
        [TestCase("basic2.json")]
        [TestCase("boon-small.json")]
        [TestCase("catalog.json")]
        [TestCase("DataWarehouseActiveCollectionControllerMPCore.servicehub.service.json")]
        [TestCase("dicos.json")]
        [TestCase("eucjp.json")]
        [TestCase("events.json")]
        [TestCase("fathers.json")]
        [TestCase("func.deps.json")]
        [TestCase("func.runtimeconfig.json")]
        [TestCase("index.json")]
        [TestCase("large.json")]
        [TestCase("Microsoft.ServiceHub.Framework.AlwaysUnauthorizedService.deps.json")]
        [TestCase("package.json")]
        [TestCase("project.assets.json")]
        [TestCase("small.json")]
        [TestCase("tiny.json")]
        [TestCase("twitter.json")]
        [TestCase("yan-cui-10k-simple-objects.json")]
        [TestCase("_oj-highly-nested.json")]        
        public static void JsonFileTest(string fileName)
        {
            var file = Path.Combine(ParserTests.InputFilesFolder, fileName);
            var input = ParserInput.FromFile(file);
            var ps = input.Parse(Grammar.Json);
            Assert.IsNotNull(ps);
            var node = ps.Node;
            Assert.IsNotNull(node);
            var tree = node.ToParseTree();
            Assert.IsNotNull(tree);
            //tree.OutputTree();
        }
    }
}