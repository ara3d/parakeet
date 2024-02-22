namespace Ara3D.Parakeet.Grammars
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Joy_(programming_language)
    /// </summary>
    public class JoyGrammar : BaseCommonGrammar
    {
        public static readonly JoyGrammar Instance = new JoyGrammar();
        public override Rule StartRule => JoyProgram;
        public Rule Operator => Node(CharSet("+=-<>&|*/^%@$~!").ZeroOrMore() | Identifier);
        public Rule Literal => Node(Float | Integer | DoubleQuoteBasicString);
        public Rule Quotation => Node(Sym("[") + Recursive(nameof(Expr)).ZeroOrMore() + Sym("]"));
        public Rule Expr => Node(Quotation | Literal | Operator | Identifier);
        public Rule Def => Node(Sym("DEFINE") + Operator + Sym("==") + Expr.ZeroOrMore() + ".");
        public Rule JoyProgram => Def.ZeroOrMore() + Expr.ZeroOrMore();
    }
}