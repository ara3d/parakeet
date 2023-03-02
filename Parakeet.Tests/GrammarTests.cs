using Parakeet.Demos;

namespace Parakeet.Tests
{
    public static class GrammarTests
    {
        public static string[] Spaces = new[]
        {
            "",
            " ",
            "\t",
            "\n \t",
            "// abc",
            "/* abc */",
            @"/*
abc
*/",
            @"// abc
",
            "/* */ /* */",
        };

        public static string[] Literals = new[]
        {
            "1",
            "123",
            "0xFF",
            "0xff",
            "12.34",
            "1.23e41",
            "'a'",
            "'\\n'",
            "\"\"",
            "\"abc\"",
            "true",
            "false",
            "null",
        };

        public static string[] Identifiers = new[]
        {
            "a",
            "_",
            "A_B",
            "A123",
            "A1_22_0x",
            "_0x",
            "___",
            "a.b",
            "a . b",
            "abc._123.DEF"
        };

        public static string[] Types = new[]
        {
            "int",
            "System.Object",
            "int[]",
            "x",
            "List<int>",
            "List<int,int>",
            "System.List<int>",
            "List<float>[]",
            "float[][]",
            "List<float[]>",
            "List<a,b,c>",
            "list<a, b<int, float>, c[]>",
            "list<system.int>",
        };

        public static string[] Expressions = new[]
        {
            "x",
            "++x",
            "3 + 1",
            "(x)",
            "(3 - 3)",
            "+x",
            "x++",
            "x()",
            "(x, y)",
            "x(1)",
            "x()()",
            "xs[1]",
            "xs[1](2)",
            "(x) => 42",
            "(int x) => 42",
            "x => 42",
            "x => { return 42; }",
            "(int x, float y) => x + y",
            "(x,y) => x + y",
            "x + y + z",
            "x(1) + y(2)",
            "x = z++",
            "++x++",
            "++++x",
            "x.ToString()",
            "x.abc",
            "f()",
            "f(1, 2)",
            "f()()",
            "f(f())",
            "f(f(f(),1),f())",
            "throw x",
            "typeof(int)",
            "typeof(T<a,b>)",
            "typeof(T[])",
            "default",
            "default(T)",
            "default (T<a,b>)",
            "default (T[])",
            "new T()",
            "new()",
            "new T[] { 1,2,3}",
            "new T<int,int>()",
            "new T(1, a)",
            "new() { 1, 2, 3 }",
            "new T() { a=1, b = 3 }",
            "nameof(x)",
            "nameof(abc.def)",
            "x?.a",
            "x ?. a",
            "x += x = 2",
            "x += x + 2",
            "x = x += 3",
            "x=y=z",
        };

        public static string[] Statements = new[]
        {
            "var x;",
            ";",
            "var x = 12;",
            "int x;",
            "T<A> x;",
            "T<A,B> x;",
            "f();",
            "f(1, 2, 3);",
            "if(b);",
            "return;",
            "break;",
            "return 12;",
            "continue;", 
            "if(b);else;",
            "if(b)break;else continue;",
            "for(;;);",
            "for(var x; b < 12; ++i);",
            "{}",
            "{;}",
            "{continue;}",
            "{{}}",
            "{;{};}",
            "++x;",
            "do{}while(b);",
            "foreach(var x in xs);",
            "try{}catch(var e){}",
            "try{}finally{}",
            "try{}catch(Exception e){}finally{}",
        };


        [Test]
        public static void OutputAstCode()
        {
            var cb = new CodeBuilder();
            AstClassBuilder.OutputAstFile(cb, "PlatoParser", Grammar.GetRules());
            var path = @"C:\Users\cdigg\git\plato\PlatoParser\CSharpAst.cs";
            var text = cb.ToString();
            Console.WriteLine(text);
            System.IO.File.WriteAllText(path, text);
        }

        [Test]
        public static void GrammarDefinition()
        {
            Grammar.OutputDefinitions();
        }

        public static string TestDigits = "0123456789";
        public static string TestNumbersThenUpperCaseLetters = "0123ABC";
        public static string HelloWorld = "Hello world!";
        public static string MathEquation = "(1.23 + (4.56 / 7.9) - 0.8)";
        public static string SomeCode = "var x = 123; x += 23; f(1, a);";

        public static CSharpGrammar Rules = new CSharpGrammar();

        public static void OutputNodeCounts(ParseNode node)
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

        public static void OutputParseErrors(ParserCache cache)
        {
            foreach (var e in cache.Errors)
            {
                Console.WriteLine($"Parse error at {e.LastState} failed expected rule {e.Expected}, parent state is {e.ParentState}, message is {e.Message}");
                Console.WriteLine(e.LastState.CurrentLine);
                Console.WriteLine(e.LastState.Indicator);
            }
        }

        public static int ParseTest(ParserInput input, Rule rule, bool outputInput = true)
        {
            var cache = new ParserCache(input.Length);
            if (outputInput)
            {
                Console.WriteLine($"Testing Rule {rule.Name} with input {input}");
            }
            else
            {
                Console.WriteLine($"Testing Rule {rule.Name}");
            }

            ParserState ps = null;
            try
            {
                ps = input.Parse(rule, cache);
            }
            catch (ParserException pe)
            {
                Console.WriteLine($"Parsing exception {pe.Message} occured at {pe.LastValidState} ");            
            }
            OutputParseErrors(cache);

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

                var ast = tree.ToNode();
                Console.WriteLine($"Ast = {ast}");

                var expNodes = rule.OnlyNodes().Simplify();
                if (expNodes != null)
                {
                    Console.WriteLine("Expected parse tree is null");
                }
                else
                {
                    Console.WriteLine($"Expected parse tree = {expNodes.ToDefinition()}");
                }
            }
            return ps.AtEnd ? 1 : 0;
        }

        public static CSharpGrammar Grammar = new CSharpGrammar();

        [Test] public static void TestSpaces() => TestInputsAndRule(Spaces, Grammar.WS);
        [Test] public static void TestLiterals() => TestInputsAndRule(Literals, Grammar.Literal);
        [Test] public static void TestLiteralExpressions() => TestInputsAndRule(Literals, Grammar.Expression);
        [Test] public static void TestTypes() => TestInputsAndRule(Types, Grammar.TypeExpr);
        [Test] public static void TestStatements() => TestInputsAndRule(Statements, Grammar.Statement);
        [Test] public static void TestExpressions() => TestInputsAndRule(Expressions, Grammar.Expression);
        [Test] public static void TestIdentifiers() => TestInputsAndRule(Identifiers, Grammar.QualifiedIdentifier);

        public static void TestInputsAndRule(string[] inputs, Rule r)
        {
            var pass = 0;
            var total = 0;
            foreach (var input in inputs)
            {
                pass += ParseTest(input, r);
                total++;
            }
            Assert.AreEqual(total, pass);
        }

        [Test]
        [TestCase("<T,T>", nameof(CSharpGrammar.TypeArgList))]
        [TestCase("<T1, T2>", nameof(CSharpGrammar.TypeArgList))]
        [TestCase("?", nameof(CSharpGrammar.Nullable))]
        [TestCase("[]", nameof(CSharpGrammar.ArrayRankSpecifier))]
        [TestCase("[,,]", nameof(CSharpGrammar.ArrayRankSpecifier))]
        [TestCase("[ , , ] ", nameof(CSharpGrammar.ArrayRankSpecifier))]
        [TestCase("= 12", nameof(CSharpGrammar.Initialization))]
        [TestCase("", nameof(CSharpGrammar.InitializationClause))]
        [TestCase("", nameof(CSharpGrammar.VariantClause))]
        [TestCase("", nameof(CSharpGrammar.InvariantClause))]
        [TestCase("var x=12", nameof(CSharpGrammar.InitializationClause))]
        [TestCase("b < 12", nameof(CSharpGrammar.InvariantClause))]
        [TestCase("++i", nameof(CSharpGrammar.InvariantClause))]
        [TestCase("(a)", nameof(CSharpGrammar.LambdaParameters))]
        [TestCase("a", nameof(CSharpGrammar.LambdaParameters))]
        [TestCase("()", nameof(CSharpGrammar.LambdaParameters))]
        [TestCase("(a,b)", nameof(CSharpGrammar.LambdaParameters))]
        [TestCase("(int a,int b)", nameof(CSharpGrammar.LambdaParameters))]
        [TestCase("(list<int, int> a,int[] b)", nameof(CSharpGrammar.LambdaParameters))]
        [TestCase("a", nameof(CSharpGrammar.LambdaParameter))]
        [TestCase("int a", nameof(CSharpGrammar.LambdaParameter))]
        [TestCase("List<int> a", nameof(CSharpGrammar.LambdaParameter))]
        [TestCase("List<int, int> a", nameof(CSharpGrammar.LambdaParameter))]
        [TestCase("int[] a", nameof(CSharpGrammar.LambdaParameter))]
        [TestCase("{}", nameof(BracedStructure))]
        [TestCase("{ }", nameof(BracedStructure))]
        [TestCase("{a}", nameof(BracedStructure))]
        [TestCase("{ /* */ }", nameof(BracedStructure))]
        [TestCase("{a b c;}", nameof(BracedStructure))]
        [TestCase("a", nameof(CSharpGrammar.Token))]
        [TestCase("/* */", nameof(CSharpGrammar.Token))]
        [TestCase(" \n ", nameof(CSharpGrammar.Token))]
        [TestCase("/* */", nameof(CSharpGrammar.Comment))]
        [TestCase(" \n ", nameof(Spaces))]
        [TestCase("a", nameof(TokenGroup))]
        [TestCase("/* */", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c;", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c;", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c;", nameof(Element))]
        [TestCase("a", nameof(Element))]
        [TestCase("{ a }", nameof(Element))]
        public static void TargetedTest(string input, string name)
        {
            var rule = Grammar.GetRuleFromName(name);
            var result = ParseTest(input, rule);
            Assert.IsTrue(result == 1);
        }
       
        [Test]
        public static void TestFiles()
        {
            var fs = Directory.GetFiles(@"C:\Users\cdigg\git\Plato.Collections\src", "*.cs");
            foreach (var f in fs)
            {
                Console.WriteLine($"Parsing file {f}");
                var input = ParserInput.FromFile(f);
                ParseTest(input, Grammar.Tokenizer, false);
                Console.WriteLine();
            }
        }

        /*
        [Test]
        public static void TestToken()
        {           
            {
                var ps = SomeCode.Parse(Rules.Element.ZeroOrMore());
                ps.OutputState();
            }
            {
                var ps = MathEquation.Parse(Rules.Element.ZeroOrMore());
                ps.OutputState();
            }
        }*/
    }
}