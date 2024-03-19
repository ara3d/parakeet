using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests
{
    public static class MarkdownTests
    {
        public static IEnumerable<(string, Rule)> GetBlockLineInputs()
        {
            var g = MarkdownBlockGrammar.Instance;

            var tmp = new[]
            {
                ("# Header", g.Heading),
                ("## Header", g.Heading),
                ("Header\n--", g.Heading),
                ("Header\n==", g.Heading),
                ("- unordered line", g.UnorderedListItem),
                ("1. ordered line", g.OrderedListItem),
                ("> blockquoted line", g.BlockQuotedLine),
                ("  \n", g.BlankLine),
                ("****", g.HorizontalLine),
                ("abc *def* ghi", g.TextLine)
            };

            return tmp;
        }

        [Test]
        public static void FindEmptyMatchingLine()
        {
            var input = "";
            var g = MarkdownBlockGrammar.Instance;
            var rules = new[]
            {
                g.Heading,
                g.HorizontalLine,
                g.UnorderedListItem,
                g.OrderedListItem,
                g.BlockQuotedLine,
                g.BlankLine,
                g.TextLine,
            };
            foreach (var rule in rules)
            {
                Console.WriteLine($"Checking that Rule {rule} does not match empty string");
                var p = rule.Parse(input);
                Assert.IsNull(p);
            }
        }

        public static IEnumerable<(string, Rule)> GetTestInlineInputs()
        {
            var g = MarkdownInlineGrammar.Instance;
            var tmp = new[]
            {
                ("a", g.Content),
                ("abc", g.Content),

                ("** bold **", g.Bold),
                ("**a**", g.Bold),
                ("__a__", g.Bold),
                ("__ abc __", g.Bold),
                ("** _bold_ **", g.Bold),
                ("**a*b* **", g.Bold),
                ("__*a*__", g.Bold),
                ("__ **abc** __", g.Bold),
                
                ("* abc *", g.Italic),
                ("*abc*", g.Italic),
                ("*a*", g.Italic),
                ("_a_", g.Italic),
                ("_ abc _", g.Italic),
                ("_ abc _", g.Italic),
                ("* __abc__ *", g.Italic),

                ("~~abc~~", g.Strikethrough),
                ("~~ abc ~~", g.Strikethrough),

                ("`a`", g.Code),
                ("``", g.Code),
                ("` abc `", g.Code),
                ("` *abc* `", g.Code),

                ("***abc***", g.BoldAndItalic),
                ("___abc___", g.BoldAndItalic),

                ("<abc>", g.UrlLink),
                ("http://www.ara3d.com", g.UrlLink),

                ("[abc](abc)", g.Link),
                ("[abc](http://ara3d.com)", g.Link),
                ("[abc](http://wwww.ara3d.com?fubar&something#27%2C-more)", g.Link),
                ("[abc](http://ara3d.com \"and here is a title\")", g.Link),
                ("[abc def](abc)", g.Link),
                ("[abc def] (abc)", g.Link),
                ("[**bolded**](abc)", g.Link),
                ("[some **bolded** _text_](abc)", g.Link),
            };
            return tmp;
        }

        [TestCaseSource(nameof(GetTestInlineInputs))]
        public static void InlineMarkdownTests((string, Rule) tuple)
            => SimpleTest(tuple.Item1, tuple.Item2);

        [TestCaseSource(nameof(GetTestInlineInputs))]
        public static void InlineMarkdownTestsAsContent((string, Rule) tuple)
            => SimpleTest(tuple.Item1, MarkdownInlineGrammar.Instance.Content);

        [TestCaseSource(nameof(GetBlockLineInputs))]
        public static void BlockMarkdownTests((string, Rule) tuple)
            => SimpleTest(tuple.Item1, tuple.Item2);

        [TestCaseSource(nameof(GetBlockLineInputs))]
        public static void BlockMarkdownTestsBlock((string, Rule) tuple)
            => SimpleTest(tuple.Item1, MarkdownBlockGrammar.Instance.Block);

        public static string[] Snippets = new[]
        {
            "# h1\nsome text",
            "# h1\n\nsome text",
            "some\n\n# h1\n\ntext\n",
            "some\n# h1\ntext\n",
        };

        [TestCaseSource(nameof(Snippets))]
        public static void MultiBlockTest(string input)
            => SimpleTest(input, MarkdownBlockGrammar.Instance.StartRule);

        public static void SimpleTest(string input, Rule r)
        {
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Rule: {r}");
            var p = r.Parse(input, true);
            Assert.NotNull(p);
            Assert.IsNull(p.LastError);
            Assert.IsTrue(p.AtEnd());
        }

        public static void OutputParseResult(ParserState ps)
        {
            if (ps == null)
            {
                Console.WriteLine("Parsing failed irrecoverably");
                return;
            }

            if (ps.LastError != null)
            {
                Console.WriteLine("Errors");
                foreach (var err in ps.AllErrors())
                    Console.WriteLine(err);
            }

            var tree = ps.GetParseTree();
            Console.WriteLine($"Parse Tree: {tree.ToXml()}");
        }

        public static IEnumerable<FilePath> TestMarkdownFiles()
        {
            var rootFolder = Folders.SourceFolder.RelativeFolder("..", "..");
            return rootFolder.GetFiles("*.md", true).Where(fp => !fp.Value.Contains("unity"));
        }

        [TestCaseSource(nameof(TestMarkdownFiles))]
        public static void TestParseMarkdownFile(FilePath file)
        {
            var g = MarkdownBlockGrammar.Instance;
            var markdown = file.ReadAllText();
            var parserState = g.Parse(markdown);
            Assert.NotNull(parserState);
            var errors = parserState.AllErrors().ToList();
            if (errors.Count > 0)
            {
                Console.WriteLine($"Found {errors.Count} errors!");
                foreach (var e in errors)
                    Console.WriteLine(e);
            }
            Assert.IsNull(parserState.LastError);
            var tree = parserState.GetParseTree();
            Console.WriteLine(tree.ToXml());
        }

        [Test]
        public static void TestRuleNames()
        {
            var r = MarkdownBlockGrammar.Instance.TextLine;
            Assert.AreEqual("TextLine", r.GetName());
        }
    }
}
