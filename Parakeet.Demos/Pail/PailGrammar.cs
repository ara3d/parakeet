using Parakeet.Demos.CSharp;

namespace Parakeet.Demos.PAIL
{
    /// <summary>
    /// Plato Abstract Intermediate Language (PAIL)
    /// is a textual representation of the abstract syntax tree used
    /// in the Plato compiler.
    /// </summary>
    public class PailGrammar : CSharpGrammar
    {
        public Rule InnerExpr => Break | Continue | Noop | Loop | Return | Conditional | VarDef | Block | Lambda | ParenthesizedExpr | Constant | Invoke | Assign | VarRef;
        public Rule Expr => Recursive(nameof(InnerExpr));   
        public Rule ParenthesizedExpr => Node(Parenthesized(Expr));
        public Rule Args => Node(ParenthesizedList(Expr));
        public Rule Invoke => Node(VarRef + Args);
        public Rule Assign => Node(VarRef + Symbol("=") + Recovery + Expr);
        public Rule VarRef => Node(Identifier);
        public Rule VarDef => Node(Keyword("let") + Recovery + Identifier + Keyword("=") + Expr);
        public Rule Conditional => Node(Keyword("if") + Recovery + Expr + Keyword("then") + Expr + Keyword("else") + Expr);
        public Rule Loop => Node(Keyword("while") + Recovery + Expr + Keyword("do") + Expr);
        public Rule Block => Node(Symbol("{") + Recovery + Expr.Then(Symbol(";")).ZeroOrMore() + Symbol("}"));
        public Rule Constant => Node(Literal);
        public Rule Break => Node(Keyword("break"));        
        public Rule Continue => Node(Keyword("continue"));
        public Rule Return => Node(Keyword("return") + Recovery + Expr);
        public Rule Noop => Node(Keyword("_"));
        public Rule Parameters => Node(ParenthesizedList(Identifier));
        public Rule Lambda => Node(Parameters + Symbol("=>") + Recovery + Expr);
    }
}   