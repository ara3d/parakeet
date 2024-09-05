namespace Ara3D.Parakeet.Grammars
{
    public class ExpressGrammar : BaseCommonGrammar
    {
        public static readonly ExpressGrammar Instance = new ExpressGrammar();

        //             + WS + Entity.ZeroOrMore() + "END_SCHEMA" + WS + ";");

        public Rule Comment => Node("(*" + AnyCharUntilPast("*)"));
        public override Rule WS => Spaces | Comment;


        public Rule Operator => CharSet("+-*/=<>!&|:").OneOrMore() ;
        public Rule Token => Identifier | Float | Integer | SingleQuoteBasicString | Operator; 
        public Rule TokenGroup => ParenthesizedList(Token) | BracketedList(Token);

        public Rule Schema => Node(Keyword("SCHEMA") + Identifier + WS + Eos);
        public Rule TypeDef => Node(Keyword("TYPE") + Identifier + WS + Keyword("=") + TypeExpr);
        public Rule Eos => Sym(";");
        public Rule Enum => Node(Keyword("ENUMERATION") + Keyword("OF") + EnumOptions + Eos);
        public Rule EnumOptions => Node(Sym("(") + ParenthesizedList(Identifier) + Sym(")"));
        public Rule DimValue => Node(Integer | '?');
        public Rule Dim => Node(Sym("[") + DimValue + Sym(":") + DimValue + Sym("]"));
        public Rule ListType => Node(Keyword("LIST") + Dim + Keyword("OF") + TypeExpr + Eos);
        public Rule SetType => Node(Keyword("LIST") + Dim + Keyword("OF") + TypeExpr + Eos);

        public const string SCHEMA = nameof(SCHEMA);

        public const string FUNCTION = nameof(FUNCTION);
        public const string ENTITY = nameof(ENTITY);
        public const string TYPE = nameof(TYPE);
        public const string RULE = nameof(RULE);

        public Rule Entity => Node(Keyword(ENTITY) + Identifier + WS + Eos);

        //public Rule SuperType

        public Rule EndSchema => Node(Keyword($"END_{SCHEMA}") + WS + Eos);
        
        public Rule EndFunction => Node(Keyword($"END_{FUNCTION}") + WS + Eos);
        public Rule EndEntity => Node(Keyword($"END_{ENTITY}") + WS + Eos);
        public Rule EndType => Node(Keyword($"END_{TYPE}") + WS + Eos);
        public Rule EndRule => Node(Keyword($"END_{RULE}") + WS + Eos);

        public Rule Optional => Node(Keyword("OPTIONAL").Optional());
        public Rule TypeExpr => Node(Optional + Identifier);
        
        //public Rule Set

        public Rule StartSection => Keyword(FUNCTION) | Keyword(ENTITY) | Keyword(TYPE) | Keyword(RULE);
        public Rule EndSection => EndFunction | EndEntity | EndType | EndRule;

        public Rule Property => Node(Identifier + Sym(":") + AbortOnFail + TypeExpr + Eos);
        public Rule DerivedProperty => Node(Identifier + Sym(":") + AbortOnFail + TypeExpr + Eos);
        public Rule WhereConstraint => Node(Identifier + Sym(":") + AbortOnFail + TypeExpr + ";");
        public Rule InverseRelation => Node(Identifier + Sym(":") + AbortOnFail + TypeExpr + ";");

        public Rule SimpleTypeList => Node(ParenthesizedList(Identifier));
        public Rule SuperTypeList => Node(Parenthesized(Keyword("ONEOF") + SimpleTypeList));
        public Rule SubTypes => Node(Keyword("SUBTYPE") + AbortOnFail + Keyword("OF") + SimpleTypeList) + Eos;

        public Rule SuperTypes => Node(Keyword("ABSTRACT").Optional() + Keyword("SUPERTYPE") + Keyword("OF") + SuperTypeList);
        
        public Rule Where => Node(Keyword("WHERE") + WhereConstraint.ZeroOrMore());
        public Rule Derive => Node(Keyword("DERIVE") + DerivedProperty.ZeroOrMore());
        public Rule Inverse => Node(Keyword("INVERSE") + InverseRelation.ZeroOrMore());


    }
}