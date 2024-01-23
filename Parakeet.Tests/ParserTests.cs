#pragma warning disable NUnit2005
namespace Parakeet.Tests
{
    public static class ParserTests
    {
        public static string ParserTestsDllPath => typeof(ParserTests).Assembly.Location;
        public static string TestsProjectFolder => Path.Combine(Path.GetDirectoryName(ParserTestsDllPath), "..", "..", "..");
        public static string DemosProjectFolder => Path.Combine(SolutionFolder, "src", "Ara3D.Parsing.Grammars");
        public static string MainProjectFolder => Path.Combine(SolutionFolder, "Parakeet");
        public static string SolutionFolder => Path.Combine(TestsProjectFolder, "..", "..");
        public static string ThisFile => Path.Combine(TestsProjectFolder, "ParserTests.cs");
        public static string InputFilesFolder => Path.Combine(ParserTests.TestsProjectFolder, "input");

        public static void OutputNodeCounts(ParserNode node)
        {
            var d = new Dictionary<string, int>();
            var sum = 0;
            while (node != null)
            {
                if (d.ContainsKey(node.Name))
                    d[node.Name] += 1;
                else 
                    d[node.Name] = 1;
                sum++;
                node = node.Previous;
            }

            Console.WriteLine($"Total number of nodes = {sum}");
            foreach (var kv in d.OrderBy(kv => kv.Key))
                Console.WriteLine($"Node {kv.Key} = {kv.Value}");
        }

        public static void OutputParseErrors(ParserState state)
        {
            for (var e = state.LastError; e != null; e = e.Previous)
            {
                Console.WriteLine($"Parse error at {e.State} failed expected rule {e.Expected}, parent state is {e.ParentState}, message is {e.Message}");
                Console.WriteLine(e.State.CurrentLine);
                Console.WriteLine(e.State.Indicator);
            }
        }

        public static void SingleParseTest(string input, Rule r)
            => Assert.AreEqual(1, ParseTest(input, r));

        public static int ParseTest(ParserInput input, Rule rule, bool outputInput = true)
        {
            if (outputInput)
            {
                Console.WriteLine($"Testing Rule {rule} with input {input}");
            }
            else
            {
                Console.WriteLine($"Testing Rule {rule}");
            }

            ParserState ps = null;
            try
            {
                ps = input.Parse(rule);
            }
            catch (ParserException pe)
            {
                Console.WriteLine($"Parsing exception {pe.Message} occured at {pe.LastValidState} ");            
            }

            if (ps != null)
            {
                OutputParseErrors(ps);

                if (ps.LastError != null)
                {
                    return 0;
                }
            }

            if (ps == null)
            {
                Console.WriteLine($"FAILED");
            }
            else if (ps.AtEnd())
            {
                Console.WriteLine($"PASSED");
            }
            else
            {
                Console.WriteLine($"PARTIAL PASSED: {ps.Position}/{ps.Input.Length}");
            }

            // Check that optimized rules produce the same output 
            var optimizedRule = rule.Optimize();
            var ps2 = input.Parse(optimizedRule);

            Assert.IsTrue(ps == null ? ps2 == null : ps2 != null);

            if (ps == null || ps2 == null)
                return 0;
            
            Assert.AreEqual(ps.Position, ps2.Position);

            if (ps == null)
                return 0;

            if (rule is NodeRule)
            {
                if (ps.Node == null)
                {
                    Console.WriteLine($"No parse node created");
                    return 0;
                }
                Console.WriteLine($"Node {ps.Node}");

                OutputNodeCounts(ps.Node);

                var treeAndNode = ps.Node.ToParseTreeAndNode();
                var tree = treeAndNode.Item1;
                if (tree == null)
                {
                    Console.WriteLine($"No parse tree created");
                    return 0;
                }
                Console.WriteLine($"Tree {tree}");
            }
            return ps.AtEnd() ? 1 : 0;
        }


        [Test]
        public static void TestFolders()
        {
            var slnFile = Path.Combine(SolutionFolder, "Parakeet.sln");
            Assert.IsTrue(File.Exists(slnFile));
            Assert.IsTrue(File.Exists(ThisFile));
        }

        public static IEnumerable<ParserRange> BetweenMatches(this IEnumerable<ParserRange> ranges)
        {
            ParserRange prev = null;
            foreach (var range in ranges)
            {
                if (prev != null)
                {
                    if (range.Begin.Position > prev.End.Position)
                    {
                        yield return prev.End.To(range.Begin);
                    }
                }
                prev = range;
            }
        }

        [Test, TestCaseSource(nameof(CoreTestDataSource))]
        public static void BasicTests(Rule rule, string[] inputs, string[] badInputs)
        {
            foreach (var input in inputs)
            {

                Assert.AreEqual(0, ParseTest("", rule));
                Assert.AreEqual(1, ParseTest("", rule.Optional()));
                Assert.AreEqual(1, ParseTest("", rule.ZeroOrMore()));

                Assert.AreEqual(1, ParseTest(input, rule));
                Assert.AreEqual(1, ParseTest(input, rule.Optional()));
                Assert.AreEqual(1, ParseTest(input, rule.ZeroOrMore()));

                Assert.AreEqual(0, ParseTest(input + input, rule));
                Assert.AreEqual(0, ParseTest(input + input, rule.Optional()));
                Assert.AreEqual(1, ParseTest(input + input, rule.ZeroOrMore()));

                Assert.AreEqual(1, ParseTest(input + input, rule + rule));
                Assert.AreEqual(1, ParseTest(input, rule | rule.Optional()));
                Assert.AreEqual(1, ParseTest(input, rule + rule.Optional()));
                Assert.AreEqual(1, ParseTest(input, rule.Optional() + rule.Optional()));
                Assert.AreEqual(1, ParseTest(input, rule + rule.ZeroOrMore()));
                Assert.AreEqual(1, ParseTest(input, rule.ZeroOrMore() + rule.ZeroOrMore()));
                Assert.AreEqual(1, ParseTest(input, rule.At() + rule));
                Assert.AreEqual(1, ParseTest(input, rule.At() + rule.At() + rule));
                Assert.AreEqual(1, ParseTest(input, rule + rule.NotAt()));

                Assert.AreEqual(1, ParseTest("a" + input, 'a' + rule));
                Assert.AreEqual(1, ParseTest("a" + input, "a" + rule));
                Assert.AreEqual(1, ParseTest("a" + input, new[] { 'a' } + rule));

                Assert.AreEqual(1, ParseTest("a", 'a' | rule));
                Assert.AreEqual(1, ParseTest("a", "a" | rule));
                Assert.AreEqual(1, ParseTest("a", new[] { 'a' } | rule));

                Assert.AreEqual(1, ParseTest("a" + input, ((Rule)'a' | 'b') + rule));
                Assert.AreEqual(1, ParseTest("b" + input, ((Rule)'a' | 'b') + rule));

                Assert.AreEqual(1, ParseTest("ab" + input, (Rule)'a' + 'b' + rule));
                Assert.AreEqual(1, ParseTest("ab" + input, (Rule)"a" + 'b' + rule));
                Assert.AreEqual(1, ParseTest("ab" + input, (Rule)"a" + "b" + rule));
                Assert.AreEqual(1, ParseTest("ab" + input, (Rule)"ab" + rule));
                Assert.AreEqual(1, ParseTest("ab" + input, AnyCharRule.Default + "b" + rule));
                Assert.AreEqual(1, ParseTest("ab" + input, new CharSetRule('a') + new CharSetRule('b') + rule));
            }

            foreach (var input in badInputs)
            {
                Assert.AreEqual(0, ParseTest(input, rule));
            }
        }

        public static IEnumerable<TestCaseData> CoreTestDataSource()
            => CoreTestData().Select(abc => new TestCaseData(abc.Item1, abc.Item2, abc.Item3));

        public static IEnumerable<(Rule, string[], string[])> CoreTestData()
        {
            var a = new CharRule('a');
            var b = new CharRule('b');
            var ab = new StringRule("ab");
            var ba = new StringRule("ba");
            var any = new AnyCharRule();

            return new (Rule, string[], string[])[]
            {
                (a, new[] { "a" }, new[] { "b" }),
                (b, new[] { "b" }, new[] { "a" }),
                (a + b, new [] { "ab" }, new [] { "a", "b", "aba", "abab"}),
                (a + b + a, new [] { "aba" }, new [] { "a", "b" }),
                (a + any + a, new [] { "aba", "aaa" }, new [] { "a", "b" }),
                (a | b, new [] { "a", "b" }, new [] { "ab", "ba", "aba", "abab"}),
                ((a | b) + a, new [] { "aa", "ba" }, new [] { "a", "b", "ab", "aba", "abab"}),
                (a + (a | b), new [] { "aa", "ab" }, new [] { "a", "b", "ba", "aba", "abab"}),
                ((a | b) + (a | b), new [] { "aa", "ab", "ba", "bb" }, new [] { "a", "b", "aba", "abab"}),
                (a + b.ZeroOrMore(), new [] { "a", "ab", "abb" }, new [] { "b", "aba" }),
                (a + ab + ab, new [] { "aabab" }, new [] { "b", "aba" }),
                (a + b.ZeroOrMore() + a, new [] { "aa", "aba", "abba" }, new [] { "b" }),
            };
        }

        public static IEnumerable<Grammar> Grammars()
        {
            yield return CSharpTests.Grammar;
            yield return JsonTests.Grammar;
        }

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

        [Test]
        public static void OutputDefinitions()
        {
            foreach (var g in Grammars())
                g.OutputDefinitions(false);
        }
    }
}