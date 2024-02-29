namespace Ara3D.Parakeet.Grammars
{
    public class SExpressionGrammar : BaseCommonGrammar
    {
        public override Rule StartRule => Document;
        public static readonly SExpressionGrammar Instance = new SExpressionGrammar();

        public override Rule WS => base.WS | Comment;
        
        public Rule Comment => Named(";" + AnyCharUntilNextLine);
        public Rule SymbolChar => Named(IdentifierFirstChar | '-' | '?' | '@' | '!' | '$');
        public Rule SymbolCharWithSpace => Named(SymbolChar | ' ');

        public Rule Symbol => Node((SymbolChar | Digit).ZeroOrMore());
        public Rule SymbolWithSpaces => Node("|" + SymbolCharWithSpace.ZeroOrMore() + "|");
        public Rule Atom => Node(Symbol | SymbolWithSpaces | Integer | Float);
        public new Rule List => Node("(" + Recursive(nameof(Expr)).ZeroOrMore() + ")");
        public Rule Expr => Node(Atom | List);
        public Rule Document => Node(WS + Expr.ZeroOrMore() + EndOfInput);
    }
}