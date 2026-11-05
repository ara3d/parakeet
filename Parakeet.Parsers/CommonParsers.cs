using Ara3D.Logging;
using Ara3D.Parakeet;
using Ara3D.Parakeet.Cst;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parsing
{
    public static class CommonParsers
    {
        // A grammar's rule property rebuilds its whole expression on every read before it returns the rule it
        // cached the first time. Plato parses hundreds of files per run, so its start rules are read once, here
        // in a nested class so that no other grammar is built with them.
        private static class PlatoRules
        {
            public static readonly Rule Start = PlatoGrammar.Instance.StartRule;
            public static readonly Rule TokenStart = PlatoTokenGrammar.Instance.StartRule;
        }

        public static Parser PlatoParser(ParserInput input, ILogger logger = null)
            => new Parser(input, PlatoRules.Start, AllCstFactories.Plato, logger);

        public static Parser PlatoTokenizer(ParserInput input, ILogger logger = null)
            => new Parser(input, PlatoRules.TokenStart, AllCstFactories.Plato, logger);

        public static Parser MarkdownBlockParser(ParserInput input, ILogger logger = null)
            => new Parser(input, MarkdownBlockGrammar.Instance.StartRule, AllCstFactories.MarkdownBlock, logger);

        public static Parser MarkdownInlineParser(ParserInput input, ILogger logger = null)
            => new Parser(input, MarkdownInlineGrammar.Instance.StartRule, AllCstFactories.MarkdownInline, logger);
            
        public static Parser JsonParser(ParserInput input, ILogger logger = null)
            => new Parser(input, JsonGrammar.Instance.StartRule, null, logger);
    }
}