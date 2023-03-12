namespace Parakeet.Tests
{

    public static class ParserTests
    {
        public static string ParserTestsDllPath => typeof(ParserTests).Assembly.Location;
        public static string TestsProjectFolder => Path.Combine(Path.GetDirectoryName(ParserTestsDllPath), "..", "..", "..");
        public static string DemosProjectFolder => Path.Combine(SolutionFolder, "Parakeet.Demos");
        public static string MainProjectFolder => Path.Combine(SolutionFolder, "Parakeet");
        public static string SolutionFolder => Path.Combine(TestsProjectFolder, "..");
        public static string ThisFile => Path.Combine(TestsProjectFolder, "ParserTests.cs");
        public static string InputFilesFolder => Path.Combine(ParserTests.TestsProjectFolder, "input");

        public static void OutputNodeCounts(ParserNode node)
        {
            var d = new Dictionary<string,int>();
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
                Console.WriteLine($"Parse error at {e.LastState} failed expected rule {e.Expected}, parent state is {e.ParentState}, message is {e.Message}");
                Console.WriteLine(e.LastState.CurrentLine);
                Console.WriteLine(e.LastState.Indicator);
            }
        }

        public static int ParseTest(ParserInput input, Rule rule, bool outputInput = true)
        {
            if (outputInput)
            {
                Console.WriteLine($"Testing Rule {rule.GetName()} with input {input}");
            }
            else
            {
                Console.WriteLine($"Testing Rule {rule.GetName()}");
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
            OutputParseErrors(ps);
            if (ps.LastError != null) 
            {
                return 0;
            }

            if (ps == null)
            {
                Console.WriteLine($"FAILED");
            }
            else if (ps.AtEnd)
            {
                Console.WriteLine($"PASSED");
            }
            else
            {
                Console.WriteLine($"PARTIAL PASSED: {ps.Position}/{ps.Input.Length}");
            }

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
                //Console.WriteLine($"Contents {tree.Contents}");

                //var ast = tree.ToNode();
                //Console.WriteLine($"Ast = {ast}");

                var expNodes = rule.OnlyNodes().Optimize();
                /*
                if (expNodes != null)
                {
                    Console.WriteLine("Expected parse tree is null");
                }
                else
                {
                    Console.WriteLine($"Expected parse tree = {expNodes.ToDefinition()}");
                }
                */
            }
            return ps.AtEnd ? 1 : 0;
        }


        [Test]
        public static void TestFolders()
        {
            var slnFile = Path.Combine(SolutionFolder, "Parakeet.sln");
            Assert.IsTrue(System.IO.File.Exists(slnFile));
            Assert.IsTrue(System.IO.File.Exists(ThisFile));
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
    }
}