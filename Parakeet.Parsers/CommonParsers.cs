using Ara3D.Logging;
using Ara3D.Parakeet;
using Ara3D.Parakeet.Cst;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parsing
{
    public static class CommonParsers
    {
        public static Parser PlatoParser(ParserInput input, ILogger logger = null)
            => new Parser(input, PlatoGrammar.Instance.StartRule, AllCstFactories.Plato, logger);

        public static Parser PlatoTokenizer(ParserInput input, ILogger logger = null)
            => new Parser(input, PlatoTokenGrammar.Instance.StartRule, AllCstFactories.Plato, logger);

        public static Parser MarkdownBlockParser(ParserInput input, ILogger logger = null)
            => new Parser(input, MarkdownBlockGrammar.Instance.StartRule, AllCstFactories.MarkdownBlock, logger);

        public static Parser MarkdownInlineParser(ParserInput input, ILogger logger = null)
            => new Parser(input, MarkdownInlineGrammar.Instance.StartRule, AllCstFactories.MarkdownInline, logger);
            
        public static Parser JsonParser(ParserInput input, ILogger logger = null)
            => new Parser(input, JsonGrammar.Instance.StartRule, null, logger);
    }
}