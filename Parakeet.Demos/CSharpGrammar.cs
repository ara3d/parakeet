using System.Linq;
using System.Runtime.CompilerServices;

namespace Parakeet
{
    // This is currently a parser for a subset of the C# language 
    // See: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure
    // The goal is to separate this into a C# and A Plato grammar.
    // First step though is to test this on my own source code. 

    public class CSharpGrammar : CommonGrammar
    {
        public CSharpGrammar()
            => WhitespaceRule = WS;

        // 
        public Rule Recovery => OnError(TokenOrStructure.Except(EOS).ZeroOrMore() + (EOS | "}" | "]" | ")" | EndOfInput));

        // Helper functions 
        public Rule List(Rule r, Rule sep = null) => (r + WS + ((sep ?? Comma) + r + WS).ZeroOrMore()).Optional();
        public Rule Parenthesized(Rule r) => Symbol("(") + r + Symbol(")");
        public Rule ParenthesizedList(Rule r, Rule sep = null) => Parenthesized(List(r, sep));
        public Rule Bracketed(Rule r) => Symbol("[") + Recovery + r + Symbol("]");
        public Rule BracketedList(Rule r, Rule sep = null) => Bracketed(List(r, sep));
        public Rule Keyword(string s) => s + IdentifierChar.NotAt() + WS;
        public Rule IntegerSuffix => Named(Strings("ul", "UL", "u", "U", "l", "L", "lu", "lU", "Lu", "LU"));
        public Rule FloatSuffix => Named("fFdDmM".ToCharSetRule());
        public Rule Comma => Named(Symbol(","));
        public Rule Symbol(string s) => s + WS; 
        public Rule Symbols(params string[] strings) => Choice(strings.OrderByDescending(x => x.Length).Select(Symbol).ToArray());
        public Rule Keywords(params string[] strings) => Choice(strings.OrderByDescending(x => x.Length).Select(Keyword).ToArray());
        public Rule Braced(Rule r) => Symbol("{") + Recovery + r + Symbol("}");
        public Rule BracedList(Rule r, Rule sep = null) => Braced(List(r, sep));
        public Rule AngledBracketList(Rule r, Rule sep = null) => Symbol("<") + List(r, sep) + Symbol(">");

        // Basic 
        public Rule WS => Named((Spaces | CppStyleComment).ZeroOrMore());

        // Literals 
        public Rule EscapedLiteralChar => Named('\\' + AnyChar); // TODO: handle special codes like \u codes and \x
        public Rule StringLiteralChar => Named(EscapedLiteralChar | AnyChar.Except('"'));
        public Rule CharLiteralChar => Named(EscapedLiteralChar | AnyChar.Except('\''));

        public Rule FloatLiteral => Node(Digits + FractionalPart.Optional() + ExponentPart.Optional() + FloatSuffix.Optional());
        public Rule HexLiteral => Node(Strings("0x", "0X") + HexDigit.OneOrMore() + IntegerSuffix.Optional());
        public Rule BinaryLiteral => Node("0b" | "0B" + BinDigit.OneOrMore() + IntegerSuffix.Optional());
        public Rule IntegerLiteral => Node(Digits.ThenNot(".fFdDmM".ToCharSetRule()) + IntegerSuffix.Optional());
        public Rule StringLiteral => Node('"' + StringLiteralChar.ZeroOrMore() + '"');
        public Rule CharLiteral => Node('\'' + CharLiteralChar + '\'');
        public Rule BooleanLiteral => Node(Keyword("true") | Keyword("false"));
        public Rule NullLiteral => Node(Keyword("null"));
        public Rule ValueLiteral => Node(Keyword("value"));

        public Rule Literal => Node(
            HexLiteral
            | BinaryLiteral
            | FloatLiteral
            | IntegerLiteral
            | StringLiteral
            | CharLiteral
            | BooleanLiteral
            | NullLiteral
            | ValueLiteral);

        // Operators 
        public Rule BinaryOperator => Node(Symbols(
            "+", "-", "*", "/", "%", ">>>", ">>", "<<", "&&", "||", "&", "|", "^",
            "+=", "-=", "*=", "/=", "%=", ">>>=", ">>=", "<<=", "&&=", "||=", "&=", "|=", "^=",
            "=", "<", ">", "<=", ">=", "==", "!=",
            "??", "?="
            ));

        public Rule UnaryOperator => Node(Symbols("++", "--", "!", "-", "+", "~"));
        public Rule Indexer => Node(Bracketed(Expression));
        public Rule OverloadableOperator => Node(Symbols("+", "-", "!", "~", "++", "--", "*", "/", "%", "&", "|", "^", "<<", ">>", ">>>", "==", "!=", "<", ">", "<=", ">="));
        
        public Rule PostfixOperator => Node(
            Symbols("!", "?", "++", "--")    
            | MemberAccess
            | ConditionalMemberAccess
            | FunctionArgs
            | Indexer
            | TernaryOperation
            | BinaryOperation
            | IsOperation
            | AsOperation
            );

        // Expressions 
        public Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());

        public Rule BinaryOperation => Node(Not("=>") + BinaryOperator + Recovery + Expression);
        public Rule TernaryOperation => Node(Symbol("?") + Recovery + Expression + Symbol(":") + Expression);
        public Rule ParenthesizedExpression => Node(ParenthesizedList(Expression));       

        public Rule ThrowExpression => Node(Keyword("throw") + Recovery + Expression);
        public Rule LambdaParameter => Node((TypeExpr + Identifier) | Identifier);
        public Rule LambdaParameters => Node(LambdaParameter | ParenthesizedList(LambdaParameter));
        public Rule LambdaBody => Node(CompoundStatement | Expression);
        public Rule LambdaExpr => Node(LambdaParameters + Symbol("=>") + Recovery + LambdaBody);
        public Rule MemberAccess => Node(Symbol(".") + Identifier);
        public Rule ConditionalMemberAccess => Node(Symbol("?.") + Identifier);
        public Rule TypeOf => Node(Keyword("typeof") + Recovery + Parenthesized(TypeExpr));
        public Rule NameOf => Node(Keyword("nameof") + Recovery + Parenthesized(Expression));
        public Rule Default => Node(Keyword("default") + Recovery + Parenthesized(TypeExpr).Optional());
        public Rule InitializerClause => Node((Identifier + Symbol("=") + Recovery + Expression) | Expression);
        public Rule Initializer => Node(BracedList(InitializerClause));
        public Rule ArraySizeSpecifier => Node(Bracketed(Expression));
        public Rule NewOperation => Node(Keyword("new") + Recovery + TypeExpr + FunctionArgs.Optional() + ArraySizeSpecifier.Optional() + Initializer.Optional());
        public Rule IsOperation => Node(Keyword("is") + Recovery + TypeExpr + Identifier.Optional());
        public Rule AsOperation => Node(Keyword("as") + Recovery + TypeExpr + Identifier.Optional());
        public Rule StringInterpolationContent => Node(Braced(Expression) | StringLiteralChar);
        public Rule StringInterpolation => Node("$" + Recovery + "\"" + StringInterpolationContent.ZeroOrMore() + "\"");
        public Rule FunctionArgKeyword => Node(Keywords("ref", "out", "in", "params"));
        public Rule FunctionArg => Node(FunctionArgKeyword.ZeroOrMore() + Expression);
        public Rule FunctionArgs => Node(ParenthesizedList(FunctionArg));

        public Rule LeafExpression => Node( 
            LambdaExpr
            | ParenthesizedExpression
            | ThrowExpression
            | Literal
            | TypeOf
            | NameOf
            | Default
            | NewOperation
            | StringInterpolation
            | Identifier
            );

        public Rule Expression => Node(Recursive(() =>
            UnaryOperator.ZeroOrMore()
            + LeafExpression
            + PostfixOperator.ZeroOrMore())
            );

        // Statements 
        public Rule EOS => Named(Symbol(";"));
        public Rule ExpressionStatement => Node(Expression + Recovery + EOS);
        public Rule ElseClause => Node(Keyword("else") + Recovery + Statement);
        public Rule IfStatement => Node(Keyword("if") + Recovery + ParenthesizedExpression + Statement + ElseClause.Optional());
        public Rule WhileStatement => Node(Keyword("while") + Recovery + ParenthesizedExpression + Statement);
        public Rule DoWhileStatement => Node(Keyword("do") + Recovery + Statement + Keyword("while") + ParenthesizedExpression + EOS);
        public Rule ReturnStatement => Node(Keyword("return") + Recovery + Expression.Optional() + EOS);
        public Rule BreakStatement => Node(Keyword("break") + Recovery + EOS);
        
        public Rule YieldStatement => Node(Keyword("yield") + Recovery + (YieldReturn | YieldBreak));
        public Rule YieldReturn => Node(Keyword("return") + Recovery + Expression + EOS);
        public Rule YieldBreak => Node(Keyword("break") + Recovery + EOS);
        public Rule ContinueStatement => Node(Keyword("continue") + Recovery + EOS);

        public Rule CompoundStatement => Node(Braced(Statement.ZeroOrMore()));
        public Rule CatchClause => Node(Keyword("catch") + Recovery + Parenthesized(VarDecl) + CompoundStatement);
        public Rule FinallyClause => Node(Keyword("finally") + CompoundStatement);
        public Rule CaseClause => Node((Keyword("default") | (Keyword("case") + Recovery + Expression)).Then(Statement));
        public Rule SwitchStatement => Node(Keyword("switch") + Recovery + Braced(CaseClause.ZeroOrMore()));
        public Rule TryStatement => Node(Keyword("try") + Recovery + CompoundStatement + CatchClause.Optional() + FinallyClause.Optional());
        public Rule ForEachStatement => Node(Keyword("foreach") + Recovery + Symbol("(") + VarDecl + Keyword("in") + Expression + Symbol(")") + Statement);

        public Rule InitializationClause => Node((VarDecl + Initialization).Optional());
        public Rule InvariantClause => Node(Expression.Optional());
        public Rule VariantClause => Node(Expression.Optional());
        public Rule ForStatement => Node(Keyword("for") + Recovery + Symbol("(") + InitializationClause + EOS + InvariantClause + EOS + VariantClause + Symbol(")") + Statement);
        public Rule Initialization => Node((Symbol("=") + Recovery + Expression).Optional());
        public Rule VarDecl => Node(TypeExpr + Identifier);
        public Rule VarDeclStatement => Node(VarDecl + Initialization + EOS);

        public Rule Statement => Node(Recursive(() => 
            EOS 
            | CompoundStatement
            | IfStatement 
            | WhileStatement 
            | DoWhileStatement 
            | ReturnStatement 
            | BreakStatement 
            | ContinueStatement 
            | ForStatement 
            | ForEachStatement
            | VarDeclStatement 
            | TryStatement
            | ExpressionStatement
        )); 

        public Rule QualifiedIdentifier => Node(List(Identifier, Symbol(".")));
        public Rule Static => Node(Keyword("static").Optional());
        public Rule UsingDirective => Node(Keyword("using") + Recovery + Static + QualifiedIdentifier + EOS);

        public Rule Modifier => Node(Keywords("static", "sealed", "partial", "readonly", "const", "ref", "abstract", "virtual"));
        public Rule AccessSpecifier => Node(Keywords("public", "private", "protected", "internal"));
        public Rule ModifiersAndSpecifiers => (Modifier | AccessSpecifier).ZeroOrMore();
        public Rule Attribute => Node(Bracketed(List(Identifier + FunctionArgs.Optional())));
        public Rule AttributeList => Node(Attribute.ZeroOrMore());
        public Rule DeclarationPreamble => Node(AttributeList + ModifiersAndSpecifiers);
        public Rule TypeVariance => Node(Keywords("in", "out").Optional());
        public Rule TypeParameter => Node(TypeVariance + Identifier);
        public Rule TypeParameterList => Node(AngledBracketList(TypeParameter).Optional());
        public Rule BaseClassList => Node((Symbol(":") + List(TypeExpr)).Optional());
        public Rule Constraint => Node(Keyword("class") | Keyword("struct") | TypeExpr); 
        public Rule ConstraintClause => Node(Keyword("where") + Identifier + Symbol(":") + TypeExpr);
        public Rule ConstraintList => Node(ConstraintClause.ZeroOrMore());

        public Rule Kind => Node(Keywords("class", "struct", "interface", "enum"));
        public Rule TypeDeclaration => Node(Recursive(() => Kind + Recovery + Identifier + TypeParameterList + BaseClassList + ConstraintList + Braced(MemberDeclaration.ZeroOrMore())));
        public Rule TypeDeclarationWithPreamble => Node(DeclarationPreamble + TypeDeclaration);

        public Rule FunctionParameterKeywords => Node(Keywords("ref", "out", "in", "params").Optional());
        public Rule DefaultValue => Node((Symbol("=") + Expression).Optional());
        public Rule FunctionParameter => Node(AttributeList + FunctionParameterKeywords + TypeExpr + Identifier + DefaultValue);
        public Rule FunctionParameterList => Node(ParenthesizedList(FunctionParameter));
        public Rule ExpressionBody => Node(Symbol("=>") + Recovery + Expression);
        public Rule FunctionBody => Node(Recursive(() => ExpressionBody | CompoundStatement));
        public Rule BaseCall => Node(Keyword("base") + Recovery + ParenthesizedExpression);
        public Rule ThisCall => Node(Keyword("this") + Recovery + ParenthesizedExpression);
        public Rule BaseOrThisCall => Node((Symbol(":") + (BaseCall | ThisCall)).Optional());
        public Rule ConstructorDeclaration => Node(Identifier + FunctionParameterList + BaseOrThisCall + FunctionBody);
        public Rule MethodDeclaration => Node(TypeExpr + Identifier + FunctionParameterList + FunctionBody);
        public Rule FieldDeclaration => Node(TypeExpr + Identifier + Initialization.Optional());

        public Rule Getter => Node(Keyword("get") + Recovery + FunctionBody);
        public Rule Setter => Node(Keyword("set") + Recovery + FunctionBody);
        public Rule Initter => Node(Keyword("init") + Recovery + FunctionBody);
        public Rule PropertyClauses => Node((Getter | Setter | Initter).ZeroOrMore());
        public Rule PropertyBody => Node(ExpressionBody | Braced(PropertyClauses));
        public Rule PropertyDeclaration => Node(TypeExpr + Identifier + PropertyBody);
        public Rule IndexerDeclaration => Node(TypeExpr + Keyword("this") + Recovery + Bracketed(FunctionParameter) + PropertyBody);
        public Rule OperatorDeclaration => Node(TypeExpr + Keyword("operator") + Recovery + OverloadableOperator + FunctionParameterList + FunctionBody);
        public Rule ImplicitOrExplicit => Node(Keywords("implicit", "explicit"));
        public Rule ConverterDeclaration => Node(TypeExpr + ImplicitOrExplicit + Keyword("operator") + TypeExpr + FunctionBody);

        public Rule MemberDeclaration => Node(DeclarationPreamble
            + ConstructorDeclaration
            | MethodDeclaration
            | FieldDeclaration
            | OperatorDeclaration
            | ConverterDeclaration
            | TypeDeclaration
            | IndexerDeclaration
            | PropertyDeclaration);

        public Rule NamespaceDeclaration => Node(Keyword("namespace") + QualifiedIdentifier + Braced(TopDeclaration.ZeroOrMore()));
        public Rule FileScopedNamespace => Node(Keyword("namespace") + QualifiedIdentifier + EOS);
        public Rule TopDeclaration => Node(Recursive(() => NamespaceDeclaration | TypeDeclarationWithPreamble));
        public Rule File => Node(UsingDirective.ZeroOrMore() + FileScopedNamespace.Optional() + TopDeclaration.ZeroOrMore());

        public Rule ArrayRankSpecifier => Node(Bracketed(Comma.ZeroOrMore()));
        public Rule ArrayRankSpecifiers => Node(ArrayRankSpecifier.ZeroOrMore());
        public Rule TypeArgList => Node(AngledBracketList(TypeExpr));
        public Rule Nullable => Node(Symbol("?").Optional());
        public Rule TypeExpr => Node(Recursive(() => QualifiedIdentifier + TypeArgList.Optional() + ArrayRankSpecifiers));

        // Tokenization pass 
        public Rule OperatorChar => "!%^&|*?+-=/><:@#~$".ToCharSetRule();
        public Rule OperatorToken => OperatorChar.OneOrMore();
        public Rule Separator => Node(";,.".ToCharSetRule() | TypeKeyword | StatementKeyword);
        public Rule Delimiter => "[]{}()".ToCharSetRule();
        public Rule TypeKeyword => Node(Keywords("class", "struct", "interface", "enum"));
        public Rule StatementKeyword => Node(Keywords("for", "if", "return", "break", "continue", "do", "foreach", "throw", "switch", "try", "catch", "finally", "using", "case", "default"));
        public new Rule Token => Node(Separator | CppStyleComment | Spaces | OperatorToken | Identifier | Literal);
        public Rule TokenOrStructure => Named(Token | Structure);
        public Rule Tokenizer => Token.ZeroOrMore() + OnError(AdvanceToEnd) + EndOfInput;

        // Structural pass 
        public Rule TokenGroup => Node((Token | Separator).Except(Delimiter).OneOrMore());
        public Rule TypeStructure => Node(TypeKeyword + TokenGroup + BracedStructure);
        public Rule StatementStructure => Node(StatementKeyword + TokenGroup + BracedStructure.Optional());
        public Rule Element => Node(Structure | TokenGroup);
        public Rule BracedStructure => Node("{" + Element.ZeroOrMore() + "}");
        public Rule BracketedStructure => Node("[" + Element.ZeroOrMore() + "]");
        public Rule ParenthesizedStructure => Node("(" + Element.ZeroOrMore() + ")");
        public Rule Structure => Node(Recursive(() => BracketedStructure | ParenthesizedStructure | BracedStructure));
        public Rule FileStructure => Node(Element.ZeroOrMore());

        // Some C# features not supported:
        // goto
        // label
        // LINQ natural query syntax
        // async
        // await
        // fixed
        // lock
        // events
        // finalizer
        // unsafe
        // checked / unchecked
        // pointers
        // stackalloc
        // delegates
        // pattern matching
        // switch expressions
        // preprocessor directives 
        // records
        // with
        // sizeof
        // verbatim strings (to-do)
    }
}