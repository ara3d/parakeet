using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ara3D.Parakeet.Grammars;
using Ara3D.Utils;

namespace Ara3D.Parakeet.Tests
{
    public static class MarkdownTests
    {
        public static IEnumerable<(string, Rule)> GetTests()
        {
            var g = MarkdownGrammar.Instance;
            var tmp = new[]
            {
                ("** bold **", g.Bold),
                ("**a**", g.Bold),
                ("__a__", g.Bold),
                ("__ abc __", g.Bold),
            };
            return tmp;
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
            var g = MarkdownGrammar.Instance;
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
