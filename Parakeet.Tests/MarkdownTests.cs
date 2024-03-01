using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests
{
    public static class MarkdownTests
    {
        public static void ExplainWhyRuleFailed(string input, Rule r)
        {
            r.Parse(input, true);
        }

        public static IEnumerable<(string, Rule)> GetTestInputs()
        {
            var g = MarkdownInlineGrammar.Instance;
            var tmp = new[]
            {
                ("a", g.PlainText),
                ("abc", g.PlainText),

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
            };
            return tmp;
        }

        [TestCaseSource(nameof(GetTestInputs))]
        public static void SmallMarkdownTest((string, Rule) tuple)
        {
            var (input, r) = tuple;
            var p = r.Parse(input);
            if (p == null)
                ExplainWhyRuleFailed(input, r);
            Assert.NotNull(p);
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
                    Console.WriteLine(err.Message);
            }

            var tree = ps.GetParseTree();
            Console.WriteLine($"Parse Tree: {tree.ToXml()}");
        }

        [Test]
        public static void ParseMarkdown()
        {
            var file = Folders.SourceFolder.RelativeFile("..", "readme.md");

            var lines = file.ReadAllLines();
            var g = MarkdownBlockGrammar.Instance;
            foreach (var line in lines)
            {
                var ps = g.Line.Parse(line);
                OutputParseResult(ps);
            }

            var markdown = file.ReadAllText();
            var parserState = g.Parse(markdown);
            Assert.NotNull(parserState);
            foreach (var e in parserState.AllErrors())
                Console.WriteLine(e.Message);
            Assert.IsNull(parserState.LastError);
            var tree = parserState.GetParseTree();
            Console.WriteLine(tree.ToXml());
        }
    }
}
