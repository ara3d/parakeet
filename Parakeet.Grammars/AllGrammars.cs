namespace Ara3D.Parakeet.Grammars
{
    public static class AllGrammars
    {
        public static Grammar[] Grammars =
        {
            CombinatorCalculusGrammar.Instance,
            CSharpGrammar.Instance,
            CssGrammar.Instance,
            CsvGrammar.Instance,
            EmailGrammar.Instance,
            JoyGrammar.Instance,
            JsonGrammar.Instance,
            MarkdownInlineGrammar.Instance,
            MarkdownBlockGrammar.Instance,
            MustacheGrammar.Instance,
            PhoneNumberGrammar.Instance,
            PlatoGrammar.Instance,
            PlatoTokenGrammar.Instance,
            SchemeGrammar.Instance,
            SExpressionGrammar.Instance,
            SimpleLambdaCalculusGrammar.Instance,
            StepGrammar.Instance,
            XmlGrammar.Instance,
        };
    }
}
