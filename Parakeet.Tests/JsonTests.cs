using System.Diagnostics;
using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;
using Newtonsoft.Json.Linq;

namespace Ara3D.Parakeet.Tests
{
    public static class JsonTests
    {
        public static DirectoryPath InputFolder = Folders.InputFolder("json");

        public static JsonGrammar Grammar = new JsonGrammar();

        public static string[] Numbers = new[]
        {
            "0",
            "1",
            "101",
            "-1",
            "-222",
            "9999999999",
            "-123.123",
            "123.123",
            "123e+12",
            "123E+12",
            "123e-12",
            "123E-12",
            "123e12",
            "123E12",
            "123.99e+12",
            "123.99E+12",
            "123.99e-12",
            "123.99E-12",
            "123.99e12",
            "123.99E12",
        };

        public static string[] Strings = new[]
        {
            "\"\"",
            "\"A\"",
            "\"abc\"",
            "\" A \"",
            "\"\\\n\"",
            "\"\\\n\"",
            "\"\\\\\"",
            "\"\\uFFFF\"",
            "\"\\uffff\"",
            "\"\\u9999\"",
            "\"ab\\ncd\""
        };

        public static string[] ConstantValues = new[]
        {
            "true",
            "false",
            "null"
        };

        public static string[] Arrays = new[]
        {
            "[]",
            "[ ]",
            "[[]]",
            "[[],[]]",
            "[ [ ] , [ ] ]",
            "[ [ ] , [ ] ]",
            "[ 1 ]",
            "[ 1 , 2 ]",
            "[\n1\n,\n2\n,3]",
            "[1]",
            "[1,2]",
            "[1,[],3]",
            "[1,\"abc\",3]",
            "[true,false,null]",
            "[ true , false , null ]"
        };

        public static string[] Objects = new[] {
            "{}",
            "{\"A\":1}",
            "{ \"A\" : 1 }",
            "{ \"abc\" : \"\\n\" }",
            "{ \"abc\" : true}",
            "{ \"abc\" : false}",
            "{ \"abc\" : null}",
            "{\"A\":[]}",
            "{\"A\":[1]}",
            "{\"A\":{}}",
            "{\"A\":{\"B\":99}}",
            "{\"Id\" :789 ,\"Name\":\"Albert\\tSimple\",\"Status\":\"Married\",\"Address\": \"Planet Earth\", \"Scores\":[1,2,3,4,5,6,7,8,9,10],\"Data\":null}"
        };

        public static string[] Spaces = new[]
        {
            "",
            " ",
            "\n",
            "\t",
            "\r",
            " \n ",
            " \t ",
            " \r ",
            "\r\n",
            "\n\r",
        };

        public static IEnumerable<TestCaseData> ParseTestData()
        {
            foreach (var x in Strings)
            {
                yield return new TestCaseData(x, Grammar.String);
                yield return new TestCaseData(x, Grammar.Value);
                yield return new TestCaseData(x, Grammar.Json);
            }
            foreach (var x in Arrays)
            {
                yield return new TestCaseData(x, Grammar.Array);
                yield return new TestCaseData(x, Grammar.Value);
                yield return new TestCaseData(x, Grammar.Json);
            }
            foreach (var x in Objects)
            {
                yield return new TestCaseData(x, Grammar.Object);
                yield return new TestCaseData(x, Grammar.Value);
                yield return new TestCaseData(x, Grammar.Json);
            }
            foreach (var x in Numbers)
            {
                yield return new TestCaseData(x, Grammar.Number);
                yield return new TestCaseData(x, Grammar.Value);
                yield return new TestCaseData(x, Grammar.Json);
            }
            foreach (var x in ConstantValues)
            {
                yield return new TestCaseData(x, Grammar.Value);
                yield return new TestCaseData(x, Grammar.Json);
            }
            foreach (var x in Spaces)
            {
                yield return new TestCaseData(x, Grammar.WS);
            }
        }

        [Test]
        public static void WhiteSpaceWeirdness()
        {
            foreach (var x in Objects)
            {
                foreach (var s in Spaces)
                {
                    var input = s + x + s;
                    Assert.AreEqual(1, ParserTests.ParseTest(input, Grammar.Json));
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(ParseTestData))]
        public static void TestJsonRule(string input, Rule rule)
        {
            Assert.AreEqual(1, ParserTests.ParseTest(input, rule));
        }

        public static IEnumerable<string> JsonFiles()
        {
            return InputFolder.GetFiles().Select(f => f.Value);
        }

        [Test, Explicit]
        [TestCaseSource(nameof(JsonFiles))]
        public static void JsonFileTest(string file)
        {
            var input = ParserInput.FromFile(file);
            Assert.AreEqual(1, ParserTests.ParseTest(input, Grammar.Json, false));
        }

        public static int CountInnerNodes(ParserTreeNode treeNode)
        {
            return 1 + treeNode.Children.Sum(t => CountInnerNodes(t));
        }

        [Test]
        public static void CompareToNewtonsoft()
        {
            var file = InputFolder.RelativeFile("59ba4880.json");
            var text = File.ReadAllText(file);
            Console.WriteLine($"File size = {text.Length / 1000}k");
            {
                var sw = Stopwatch.StartNew();
                JObject.Parse(text);
                Console.WriteLine($"It took {sw.Elapsed} to parse using Newtonsoft");
            }

            {
                var sw = Stopwatch.StartNew();
                var ps = Grammar.Json.Parse(text);
                Console.WriteLine($"It took {sw.Elapsed} to parse using Parakeet");

                var tree = ps.Node.ToParseTree();
                Console.WriteLine($"Inner tree nodes = {CountInnerNodes(tree)}");
                Assert.NotNull(ps);
                Assert.IsTrue(ps.AtEnd());
            }
        }
    }
}