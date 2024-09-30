namespace Ara3D.Parakeet.Grammars
{
    public class PlatoTokenGrammar : BaseCommonGrammar
    {
        public override Rule StartRule => Tokenizer;
        public static PlatoTokenGrammar Instance  = new PlatoTokenGrammar();

        // Tokenization pass 
        public override Rule WS => true;

        // Literals 
        public Rule IntegerSuffix => Named(Strings("ul", "UL", "u", "U", "l", "L", "lu", "lU", "Lu", "LU"));
        public Rule FloatSuffix => Named("fFdDmM".ToCharSetRule());
        public Rule EscapedLiteralChar => Named('\\' + AnyChar); // TODO: handle special codes like \u codes and \x
        public Rule StringLiteralChar => Named(EscapedLiteralChar | "\"\"" | AnyChar.Except('"'));
        public Rule CharLiteralChar => Named(EscapedLiteralChar | AnyChar.Except('\''));
        public Rule FloatLiteral => Node(Float + FloatSuffix.Optional());
        public Rule HexLiteral => Node(Strings("0x", "0X") + HexDigit.OneOrMore() + IntegerSuffix.Optional());
        public Rule BinaryLiteral => Node("0b" | "0B" + BinDigit.OneOrMore() + IntegerSuffix.Optional());
        public Rule IntegerLiteral => Node(Digits.ThenNot("fFdDmM".ToCharSetRule()) + IntegerSuffix.Optional());
        public Rule StringLiteral => Node(Optional('@') + '"' + StringLiteralChar.ZeroOrMore() + '"');
        public Rule CharLiteral => Node('\'' + CharLiteralChar + '\'');
        public Rule BooleanLiteral => Node(Keyword("true") | Keyword("false"));
        public Rule NullLiteral => Node(Keyword("null"));

        public Rule Literal => Node(
            HexLiteral
            | BinaryLiteral
            | FloatLiteral
            | IntegerLiteral
            | StringLiteral
            | CharLiteral
            | BooleanLiteral
            | NullLiteral);

        // Operators 
        public Rule BinaryOperator => Node(Symbols(
            "and", "or", "+", "-", "*", "/", "%", "<<", "&&", "||", "&", "|", "^",
            "+=", "-=", "*=", "/=", "%=", ">>>=", ">>=", "<<=", "&&=", "||=", "&=", "|=", "^=",
            "=", "<", ">", "<=", ">=", "==", "!="
        ));

        public Rule Operator => Node(OperatorChar.OneOrMore());
        public Rule Separator => Node(";,.".ToCharSetRule() | TypeKeyword | StatementKeyword);
        public Rule Delimiter => Node("[]{}()".ToCharSetRule());
        public Rule TypeKeyword => Node(Keywords("concept", "library", "type"));
        public Rule StatementKeyword => Node(Keywords("for", "if", "return", "break", "continue", "do", "foreach", "throw", "switch", "try", "catch", "finally", "using", "case", "default"));
        public Rule Unknown => Node(AnyChar);
        public Rule ParameterName => Node(Identifier);
        public Rule FunctionName => Node(Identifier);
        public Rule FieldName => Node(Identifier);
        public Rule TypeName => Node(Identifier);
        public Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());

        public Rule Tok(Rule r) => r + CommentOrSpaces;
        
        public Rule OperatorChar => "!%^&|*?+-=/><:@#~$".ToCharSetRule();
        public Rule TypeParameterToken => Node(Tok(Identifier) + Recursive(nameof(TypeAnnotationToken)));
        public Rule TypeParametersToken => Node(Tok("<") + Tok(TypeParameterToken) + ZeroOrMore(Tok(",") + TypeParameterToken) + Tok(">"));
        public Rule DeclaredType => Named(Tok(TypeKeyword) + Tok(TypeName));
        public Rule TypeAnnotationToken => Named(Tok(":") + Tok(TypeName) + Optional(Tok(TypeParametersToken)));

        public Rule Parameter => Named(Tok(ParameterName) + Tok(TypeAnnotationToken));
        public Rule EmptyParameterList => Named(Tok("(") + Tok(")"));
        public Rule ParameterList => Named(Tok("(") + Parameter + ZeroOrMore(Tok(",") + Parameter) + Tok(")"));
        public Rule Parameters => Named(EmptyParameterList | ParameterList);

        public Rule MemberDelimiter => Named(Tok("{;".ToCharSetRule()));
        public Rule DeclaredFunction => Named(MemberDelimiter + Tok(FunctionName) + Parameters + Optional(TypeAnnotationToken));
        public Rule DeclaredField => Named(MemberDelimiter + Tok(FieldName) + Tok(TypeAnnotationToken));
        public Rule Comment => Named(CppStyleComment);
        public Rule CommentOrSpaces => Named((Comment | Spaces).ZeroOrMore());

        public Rule Token => Named(
            DeclaredType 
            | DeclaredField 
            | DeclaredFunction 
            | Separator 
            | Delimiter 
            | Comment
            | Spaces
            | Operator 
            | Identifier 
            | HexLiteral
            | BinaryLiteral
            | FloatLiteral
            | IntegerLiteral
            | StringLiteral
            | CharLiteral
            | BooleanLiteral
            | NullLiteral
            | Unknown);

        public Rule BracedTokenGroup => Node(Braced(Token));
        public Rule BracketedTokenGroup => Node(Bracketed(Token));
        public Rule ParenthesizedTokenGroup => Node(Parenthesized(Token));

        public Rule TokenGroup => Node((BracedTokenGroup | BracketedTokenGroup | ParenthesizedTokenGroup | Token).OneOrMore());

        public Rule Tokenizer => Named(Token.ZeroOrMore() + EndOfInput);
    }
}