using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Tests
{
    public static class CSharpTests
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

        public static string[] Members = new[]
        {
            "public static int x;",
            "int x;",
            "public int x;",
            "private int y;",
            "int x = 12;",
            "public int x = 12;",
            "public int x { get; } = 12;",
            "public Dictionary<string, IFunctionValue> Functions { get; } = new();",
            "void f() { }",
            "public static void f(int x) { return x; }",
            "int x {get;}",
            "int y {get;set;}",
            "int x { get; set; }",
            "int x => 42;",
            "readonly int[] x;",
            "int x() => 12;",
            "int x { get { return x; } set { y = value; } }",
            "int x { get => x; set => y = value; }",
            "public static int x => { return 42; }",
            "public static string[] Literals = new[] { a, b, c };",
            "[Attr] public int x;",
            "[Attr] public static void f() {}",
            "[Attr, Attr()] public int x;",
            "[Attr] [Attr(nameof(something), t)] private int ffg(int x) { return x; }",
            "public static IEnumerable<(Rule, string[], string[])> CoreTestData() { }",
            "public static IEnumerable<ParserRange> BetweenMatches(this IEnumerable<ParserRange> ranges) {}",
            "public abstract void Test();",
            "int[] xs = { 1, 2, 3 };",
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
            "(int, float)", 
            "List<(int, float)>",
            "(int, (a, b))",
            "(xs[], List<int>)",
            "IEnumerable<(Rule, string[], string[])>",
            "(Rule, string[], string[])[]",
        };

        public static string[] Expressions = new[]
        {
            "\"abc\"",
            "\"\"",
            "\"\\n\"",
            "\'a\'",
            "\'\\n'",
            @"@""ABC
            DEF""",
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
            "new T() { a=1, b = 3 }",
            "nameof(x)",
            "nameof(abc.def)",
            "1.seconds()",
            "x?.a",
            "x ?. a",
            "x += x = 2",
            "x += x + 2",
            "x = x += 3",
            "x=y=z",
            "ps == null ? ps2 == null : ps2 != null",
            "ps.AtEnd() ? 1 : 0",
            "(float)x",
            "(int)(int)(int)y",
            "((float)x + (int)y)",
            "(int)x * (y)",
            "(float)((int)x)",
            "new (Rule, string[], string[])[] { }",
            "new int[] { 1, 2, 3, }",
            "x * 1e-24",
            "1e-24",
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
            "f()()();",
            "((a()));",
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
            "int x = 12, y = 13;",
            "for(int x=12,y=13; b < 12; ++i, ++j);",
            "for(int x=12,y=13; b < 12; ++i, ++j);",
        };

        public static string[] FailingStatements = new[]
        {
            "return x x;",
            "if ;",
            "f(;",
            "f);",
            "if (a b);",
            "if (12 34);",
            "for ( ) }",
            "for { }",
            "continue 12;",
            "f() g();",
            "var var var var;",
        };

        public static string[] FailingExpressions =
        {
            "",
            "+",
            "<<",
            "<< 1",
            "1+",
            "a b c",
            "a[",
            "f(",
            "(",
            "))",
            "f())",
            "a <<<< b",
            "a << << b",
            "a === b",
            "a ^!= b",
            "a!+",
        };

        public static string[] Classes = new[]
        {
            "class C { }",
            "class C : B { }",
            "class C<T> { }",
            "class C<T> where T: class { }",
            "class C { int x = 12; }",
            "class C { public int x = 12; }",
            "class C { public int x = 12; public int y = 13; }",
            "class C { private int x = 12; }",
            "class C { public static int x = 12; }",
            "class C { public int x => 12; }",
            "class C { public int x { get; } = 12; }",
            "class C { public int x { get; set; } }",
            "class C { public int x { get { return 12; } set { _x = value; } } }",
            "class C { public int x() { return 12; } }",
            "class C { public int x() => 12; }",
            "class C { public int x() { return 12; } private float x() { return 99; } }",
            "public static class CSharpTests\r\n    {\r\n\r\n        public static string[] Spaces = new[]\r\n{\r\n            \"\",\r\n            \" \",\r\n            \"\\t\",\r\n            \"\\n \\t\",\r\n            \"// abc\",\r\n            \"/* abc */\",\r\n            @\"/*\r\nabc\r\n*/\",\r\n            @\"// abc\r\n\",\r\n            \"/* */ /* */\",\r\n        }; }",
            "public static class CSharpTests\r\n    {\r\n\r\n        public static string[] Spaces = new[]\r\n{}; }",
            "public abstract class Rule\r\n    {\r\n        protected abstract ParserState MatchImplementation(ParserState state);\r\n\r\n        [MethodImpl(MethodImplOptions.AggressiveInlining)]\r\n        public ParserState Match(ParserState state)\r\n        {\r\n            return MatchImplementation(state);\r\n        } }",
        };

        public static string TestDigits = "0123456789";
        public static string TestNumbersThenUpperCaseLetters = "0123ABC";
        public static string HelloWorld = "Hello world!";
        public static string MathEquation = "(1.23 + (4.56 / 7.9) - 0.8)";
        public static string SomeCode = "var x = 123; x += 23; f(1, a);";

        public static CSharpGrammar Grammar = new CSharpGrammar();

        [Test, TestCaseSource(nameof(Spaces))] 
        public static void TestSpaces(string input) => ParserTests.SingleParseTest(input, Grammar.WS);
        
        [Test, TestCaseSource(nameof(Literals))] public static void TestLiterals(string input) => ParserTests.SingleParseTest(input, Grammar.Literal);
        
        [Test, TestCaseSource(nameof(Literals))] public static void TestLiteralExpressions(string input) => ParserTests.SingleParseTest(input, Grammar.Expression);
        
        [Test, TestCaseSource(nameof(Types))] 
        public static void TestTypes(string input) => ParserTests.SingleParseTest(input, Grammar.TypeExpr);
        
        [Test, TestCaseSource(nameof(Statements))] 
        public static void TestStatements(string input) => ParserTests.SingleParseTest(input, Grammar.Statement);
        
        [Test, TestCaseSource(nameof(Expressions))] 
        public static void TestExpressions(string input) => ParserTests.SingleParseTest(input, Grammar.Expression);
        
        [Test, TestCaseSource(nameof(Identifiers))] 
        public static void TestIdentifiers(string input) => ParserTests.SingleParseTest(input, Grammar.QualifiedIdentifier);

        [Test, TestCaseSource(nameof(Members))]
        public static void TestMemberDeclarations(string input) => ParserTests.SingleParseTest(input, Grammar.MemberDeclaration);

        [Test, TestCaseSource(nameof(FailingStatements))]
        public static void TestFailingStatements(string input)
        {
            Assert.AreEqual(0, ParserTests.ParseTest(input, Grammar.Statement));
        }

        [Test, TestCaseSource(nameof(FailingExpressions))]
        public static void TestFailingExpressions(string input)
        {
            Assert.AreEqual(0, ParserTests.ParseTest(input, Grammar.Expression));
        }

        [Test, TestCaseSource(nameof(Classes))] 
        public static void TestClasses(string input) => ParserTests.SingleParseTest(input, Grammar.TypeDeclarationWithPreamble);
        
        public static void TestInputsAndRule(string[] inputs, Rule r)
        {
            var pass = 0;
            var total = 0;
            foreach (var input in inputs)
            {
                pass += ParserTests.ParseTest(input, r);
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
        [TestCase("", nameof(CSharpGrammar.ForLoopInit))]
        [TestCase("", nameof(CSharpGrammar.ForLoopVariant))]
        [TestCase("", nameof(CSharpGrammar.ForLoopInvariant))]
        [TestCase("var x=12", nameof(CSharpGrammar.ForLoopInit))]
        [TestCase("b < 12", nameof(CSharpGrammar.ForLoopInvariant))]
        [TestCase("++i", nameof(CSharpGrammar.ForLoopInvariant))]
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
        [TestCase("{}", nameof(CSharpGrammar.BracedStructure))]
        [TestCase("{ }", nameof(CSharpGrammar.BracedStructure))]
        [TestCase("{a}", nameof(CSharpGrammar.BracedStructure))]
        [TestCase("{ /* */ }", nameof(CSharpGrammar.BracedStructure))]
        [TestCase("{a b c;}", nameof(CSharpGrammar.BracedStructure))]
        [TestCase("a", nameof(CSharpGrammar.Token))]
        [TestCase("/* */", nameof(CSharpGrammar.Token))]
        [TestCase(" \n ", nameof(CSharpGrammar.Token))]
        [TestCase("/* */", nameof(CSharpGrammar.CppStyleComment))]
        [TestCase(" \n ", nameof(CSharpGrammar.Spaces))]
        [TestCase("a", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("/* */", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c;", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c;", nameof(CSharpGrammar.TokenGroup))]
        [TestCase("a b c;", nameof(CSharpGrammar.Element))]
        [TestCase("a", nameof(CSharpGrammar.Element))]
        [TestCase("{ a }", nameof(CSharpGrammar.Element))]
        [TestCase("namespace X.Y { }", nameof(CSharpGrammar.NamespaceDeclaration))]
        [TestCase("namespace A { class B {} class C {} }", nameof(CSharpGrammar.NamespaceDeclaration))]
        [TestCase("public static internal readonly ref", nameof(CSharpGrammar.DeclarationPreamble))]
        [TestCase("= {1,2,3}", nameof(CSharpGrammar.Initialization))]
        [TestCase("= new()", nameof(CSharpGrammar.Initialization))]
        [TestCase("int x {get;}", nameof(CSharpGrammar.PropertyDeclaration))]
        [TestCase("int x { get; set; }", nameof(CSharpGrammar.PropertyDeclaration))]
        [TestCase("int x { get; } = 12;", nameof(CSharpGrammar.PropertyDeclaration))]
        [TestCase("= new()", nameof(CSharpGrammar.Initialization))]
        [TestCase("Velocity PerSecond(Length l) => l / 1.Seconds();", nameof(CSharpGrammar.MethodDeclaration))]
        [TestCase("=> l / 1.Seconds();", nameof(CSharpGrammar.ExpressionBody))]
        [TestCase("l / 1.Seconds()", nameof(CSharpGrammar.Expression))]
        [TestCase("1.Seconds()", nameof(CSharpGrammar.Expression))]
        [TestCase("=> { }", nameof(CSharpGrammar.ExpressionBody))]
        [TestCase(", ", nameof(CSharpGrammar.Comma))]
        [TestCase("{}", nameof(CSharpGrammar.Initializer))]
        [TestCase("{ 1 }", nameof(CSharpGrammar.Initializer))]
        [TestCase("{x=1}", nameof(CSharpGrammar.Initializer))]
        [TestCase("{ x = 1, y = 2 }", nameof(CSharpGrammar.Initializer))]
        [TestCase("x=1", nameof(CSharpGrammar.InitializerClause))]
        [TestCase("x", nameof(CSharpGrammar.InitializerClause))]
        [TestCase("abc = 2.4", nameof(CSharpGrammar.InitializerClause))]
        public static void TargetedTest(string input, string name)
        {
            var rule = Grammar.GetRuleFromName(name);
            var result = ParserTests.ParseTest(input, rule);
            Assert.IsTrue(result == 1);
        }

        public static IEnumerable<TestCaseData> SourceFiles()
        {
            return Directory.GetFiles(Folders.SourceFolder, "*.cs").Select(f => new TestCaseData(f));
        }

        [Test]
        [TestCaseSource(nameof(SourceFiles))]
        public static void TestFileTokenizer(string file)
        {
            Assert.AreEqual(1, ParserTests.ParseTest(ParserInput.FromFile(file), Grammar.Tokenizer, false));
        }

        [Test]
        [TestCaseSource(nameof(SourceFiles))]
        public static void TestFileParser(string file)
        {
            Assert.AreEqual(1, ParserTests.ParseTest(ParserInput.FromFile(file), Grammar.File, false));
        }

        [Test, TestCaseSource(nameof(SourceFiles))]
        public static void TestIdentifierMatches(string file)
            => OutputMatches(file, Grammar.Identifier);

        [Test, TestCaseSource(nameof(SourceFiles))]
        public static void TestStatementMatches(string file)
            => OutputMatches(file, Grammar.Statement);

        [Test, TestCaseSource(nameof(SourceFiles))]
        public static void TestTypeDeclarationMatches(string file)
            => OutputMatches(file, Grammar.TypeDeclaration);

        [Test, TestCaseSource(nameof(SourceFiles))]
        public static void TestExpressionMatches(string file)
            => OutputMatches(file, Grammar.Expression);

        public static void OutputMatches(string file, Rule rule)
        {
            var text = ParserInput.FromFile(file);
            var prs = text.GetMatches(rule | Grammar.Token | Grammar.Delimiter);
            var between = prs.BetweenMatches();
            foreach (var range in between)
            {
                Console.WriteLine($"Failed to match at {range.Begin.Position} = {range.Text}");
            }
            Assert.IsFalse(between.Any());
            foreach (var range in prs)
            {
                if (range.Node?.Name == rule.GetName())
                {
                    Assert.IsTrue(range.Text.Length > 0);
                    Console.WriteLine($"[{range.Node.EllidedContents}]");
                }
            }
        }
    }
}