namespace Ara3D.Parakeet.Cst
{
    public static class AllCstFactories
    {
        public static CstNode CombinatorCalculus(ParserTreeNode input) => (new CombinatorCalculusGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode CSharp(ParserTreeNode input) => (new CSharpGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Css(ParserTreeNode input) => (new CssGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Csv(ParserTreeNode input) => (new CsvGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Email(ParserTreeNode input) => (new EmailGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Joy(ParserTreeNode input) => (new JoyGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Json(ParserTreeNode input) => (new JsonGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode MarkdownInline(ParserTreeNode input) => (new MarkdownInlineGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode MarkdownBlock(ParserTreeNode input) => (new MarkdownBlockGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Mustache(ParserTreeNode input) => (new MustacheGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode PhoneNumber(ParserTreeNode input) => (new PhoneNumberGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Plato(ParserTreeNode input) => (new PlatoGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode PlatoToken(ParserTreeNode input) => (new PlatoTokenGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Scheme(ParserTreeNode input) => (new SchemeGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode SExpression(ParserTreeNode input) => (new SExpressionGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode SimpleLambdaCalculus(ParserTreeNode input) => (new SimpleLambdaCalculusGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Step(ParserTreeNode input) => (new StepGrammarNameSpace.CstNodeFactory()).Create(input);
        public static CstNode Xml(ParserTreeNode input) => (new XmlGrammarNameSpace.CstNodeFactory()).Create(input);
    }
}
