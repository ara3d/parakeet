namespace Parakeet.Grammars.WIP
{
    public class SExpressionGrammar : CommonGrammar
    {
        public override Rule WS => base.WS | Comment;

        public Rule RecExpr => Recursive(nameof(Expr));

        public Rule Comment => Named(";" + UntilNextLine);
        public Rule SymbolChar => Named(IdentifierFirstChar | '-' | '?' | '@' | '!' | '$');
        public Rule SymbolCharWithSpace => Named(SymbolChar | ' ');

        public new Rule Symbol => Node((SymbolChar | Digit).ZeroOrMore());
        public Rule SymbolWithSpaces => Node("|" + SymbolCharWithSpace.ZeroOrMore() + "|");
        public Rule Atom => Node(Symbol | SymbolWithSpaces | Integer | Float);
        public new Rule List => Node("(" + RecExpr.ZeroOrMore() + ")");
        public Rule Expr => Node(Atom | List);
    }
}