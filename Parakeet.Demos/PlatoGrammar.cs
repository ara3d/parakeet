namespace Parakeet.Demos
{
    public class PlatoGrammar : CommonGrammar
    {
        public static PlatoGrammar Instance = new PlatoGrammar();

        // Recovery on error 
        public Rule Recovery => OnError(
            TokenOrStructure.Except(EOS).ZeroOrMore()
            + (EOS | EndOfInput) | AdvanceToEnd);

        // Helper functions 
        public Rule IntegerSuffix => Named(Strings("ul", "UL", "u", "U", "l", "L", "lu", "lU", "Lu", "LU"));
        public Rule FloatSuffix => Named("fFdDmM".ToCharSetRule());

        // Basic 
        public override Rule WS => Named((SpaceChars | CppStyleComment).ZeroOrMore());

        // Literals 
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
            "and", "or", "+", "-", "*", "/", "%", ">>>", ">>", "<<", "&&", "||", "&", "|", "^",
            "+=", "-=", "*=", "/=", "%=", ">>>=", ">>=", "<<=", "&&=", "||=", "&=", "|=", "^=",
            "=", "<", ">", "<=", ">=", "==", "!=",
            "??", "?="
        ));

        public Rule CastExpression => Node(Parenthesized(TypeExpr) + Expression);
        public Rule PrefixOperator => Node(Symbols("++", "--", "!", "-", "+", "~"));
        public Rule Indexer => Node(Bracketed(Expression));

        public Rule PostfixOperator => Node(
            Symbols("!", "++", "--")
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
        public Rule LambdaParameter => Node(Identifier + TypeAnnotation);
        public Rule LambdaParameters => Node(LambdaParameter | ParenthesizedList(LambdaParameter));
        public Rule LambdaBody => Node(CompoundStatement | Expression);
        public Rule LambdaExpr => Node(LambdaParameters + Symbol("=>") + Recovery + LambdaBody);
        public Rule MemberAccess => Node(Symbol(".") + Identifier);
        public Rule ConditionalMemberAccess => Node(Symbol("?.") + Identifier);
        public Rule TypeOf => Node(Keyword("typeof") + Recovery + Parenthesized(TypeExpr));
        public Rule NameOf => Node(Keyword("nameof") + Recovery + Parenthesized(Expression));
        public Rule Default => Node(Keyword("default") + Recovery + Parenthesized(TypeExpr).Optional());
        public Rule InitializerClause => Node(Identifier + Symbol("=") + Recovery + Expression | Expression);
        public Rule Initializer => Node(BracedList(InitializerClause));
        public Rule ArraySizeSpecifier => Node(Bracketed(Expression));

        public Rule NewOperation => Node(Keyword("new") + Recovery + TypeExpr.Optional() + FunctionArgs.Optional() +
                                         ArraySizeSpecifier.Optional() + Initializer.Optional());

        public Rule IsOperation => Node(Keyword("is") + Recovery + TypeExpr + Identifier.Optional());
        public Rule AsOperation => Node(Keyword("as") + Recovery + TypeExpr + Identifier.Optional());
        public Rule StringInterpolationContent => Node(Braced(Expression) | StringLiteralChar);
        public Rule StringInterpolation => Node("$" + Recovery + "\"" + StringInterpolationContent.ZeroOrMore() + "\"");
        public Rule FunctionArgKeyword => Node(Keywords("ref", "out", "in", "params"));
        public Rule FunctionArg => Node(FunctionArgKeyword.ZeroOrMore() + Expression);
        public Rule FunctionArgs => Node(ParenthesizedList(FunctionArg));

        public Rule LeafExpression => Node(
            LambdaExpr
            | CastExpression
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

        public Rule InnerExpression => Named(
            PrefixOperator.ZeroOrMore()
            + LeafExpression
            + PostfixOperator.ZeroOrMore());

        public Rule Expression => Recursive(nameof(InnerExpression));

        // Statements 
        public Rule EOS => Named(Symbol(";"));
        public Rule ExpressionStatement => Node(Expression + Recovery + EOS);
        public Rule ElseClause => Node(Keyword("else") + Recovery + Statement);

        public Rule IfStatement =>
            Node(Keyword("if") + Recovery + ParenthesizedExpression + Statement + ElseClause.Optional());

        public Rule WhileStatement => Node(Keyword("while") + Recovery + ParenthesizedExpression + Statement);

        public Rule DoWhileStatement =>
            Node(Keyword("do") + Recovery + Statement + Keyword("while") + ParenthesizedExpression + EOS);

        public Rule ReturnStatement => Node(Keyword("return") + Recovery + Expression.Optional() + EOS);
        public Rule BreakStatement => Node(Keyword("break") + Recovery + EOS);

        public Rule YieldStatement => Node(Keyword("yield") + Recovery + (YieldReturn | YieldBreak));
        public Rule YieldReturn => Node(Keyword("return") + Recovery + Expression + EOS);
        public Rule YieldBreak => Node(Keyword("break") + Recovery + EOS);
        public Rule ContinueStatement => Node(Keyword("continue") + Recovery + EOS);

        public Rule CompoundStatement => Node(Braced(Statement.ZeroOrMore()));
        public Rule CatchClause => Node(Keyword("catch") + Recovery + Parenthesized(VarDecl) + CompoundStatement);
        public Rule FinallyClause => Node(Keyword("finally") + CompoundStatement);
        public Rule CaseClause => Node((Keyword("default") | Keyword("case") + Recovery + Expression).Then(Statement));
        public Rule SwitchStatement => Node(Keyword("switch") + Recovery + Braced(CaseClause.ZeroOrMore()));

        public Rule TryStatement => Node(Keyword("try") + Recovery + CompoundStatement + CatchClause.Optional() +
                                         FinallyClause.Optional());

        public Rule ForEachStatement => Node(Keyword("foreach") + Recovery + Symbol("(") + TypeExpr + Identifier +
                                             Keyword("in") + Expression + Symbol(")") + Statement);

        public Rule FunctionDeclStatement => Node(Keyword("static").Optional() + MethodDeclaration);
        public Rule ForLoopInit => Node(VarDecl.Optional());
        public Rule ForLoopInvariant => Node(Expression.Optional());
        public Rule ForLoopVariant => Node(List(Expression));

        public Rule ForStatement => Node(Keyword("for") + Recovery + Symbol("(") + ForLoopInit + EOS +
                                         ForLoopInvariant + EOS + ForLoopVariant + Symbol(")") + Statement);

        public Rule ArrayInitializationValue => Node(BracedList(Expression));
        public Rule InitializationValue => Node(ArrayInitializationValue | Expression);
        public Rule Initialization => Node((Symbol("=") + Recovery + InitializationValue).Optional());
        public Rule VarWithInitialization => Node(Identifier + Initialization);
        public Rule VarDecl => Node(TypeExpr + ListOfAtLeastOne(VarWithInitialization));
        public Rule VarDeclStatement => Node(VarDecl + EOS);

        public Rule InnerStatement => Named(
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
            | FunctionDeclStatement
            | VarDeclStatement
            | TryStatement
            | YieldStatement
            | SwitchStatement
            | ExpressionStatement
        );

        public Rule Statement => Recursive(nameof(InnerStatement));

        public Rule QualifiedIdentifier => Node(List(Identifier, Symbol(".")));
        public Rule TypeParameter => Node(Identifier + TypeAnnotation);
        public Rule TypeParameterList => Node(AngledBracketList(TypeParameter).Optional());
        

        public Rule ImplementsList => Node(Optional(Keyword("implements") + Recovery + List(TypeExpr)));
        public Rule InheritsList => Node(Optional(Keyword("inherits") + Recovery + List(TypeExpr)));

        public Rule Type => Node(Keyword("type") + Recovery + Identifier + TypeParameterList + ImplementsList + Braced(FieldDeclaration.ZeroOrMore()));
        public Rule Concept => Node(Keyword("concept") + Recovery + Identifier + TypeParameterList + InheritsList + Braced(MethodDeclaration.ZeroOrMore()));
        public Rule Library => Node(Keyword("library") + Recovery + Identifier + Braced(MethodDeclaration.ZeroOrMore()));
        
        public Rule TopLevelDeclaration => Node(Library | Concept | Type);

        public Rule FunctionParameter => Node(Identifier + TypeAnnotation);
        public Rule FunctionParameterList => Node(ParenthesizedList(FunctionParameter));
        public Rule ExpressionBody => Node(Symbol("=>") + Recovery + Expression + EOS);
        public Rule FunctionBody => Node(ExpressionBody | CompoundStatement | EOS);
        public Rule TypeAnnotation => Node(Optional(Symbol(":") + Recovery + TypeExpr));
        public Rule MethodDeclaration => Node(Identifier + FunctionParameterList + Recovery + TypeAnnotation + FunctionBody);
        public Rule FieldDeclaration => Node(Identifier + Symbol(":") + TypeExpr + EOS);
        public Rule MemberDeclaration => Node(MethodDeclaration | FieldDeclaration);

        public Rule ArrayRankSpecifier => Node(Bracketed(Comma.ZeroOrMore()));
        public Rule ArrayRankSpecifiers => Node(ArrayRankSpecifier.ZeroOrMore());
        public Rule TypeArgList => Node(AngledBracketList(TypeExpr));
        public Rule SimpleTypExpr => Node(QualifiedIdentifier);
        public Rule CompoundTypeExpr => Node(ParenthesizedList(TypeExpr));
        public Rule CompoundOrSimpleTypeExpr => Node(CompoundTypeExpr | SimpleTypExpr);
        public Rule InnerTypeExpr => Node(CompoundOrSimpleTypeExpr + TypeArgList.Optional() + ArrayRankSpecifiers);
        public Rule TypeExpr => Recursive(nameof(InnerTypeExpr));

        public Rule File => Node(WS + TopLevelDeclaration.ZeroOrMore());

        // Tokenization pass 
        public Rule OperatorChar => "!%^&|*?+-=/><:@#~$".ToCharSetRule();
        public Rule OperatorToken => OperatorChar.OneOrMore();
        public Rule Separator => Node(";,.".ToCharSetRule() | TypeKeyword | StatementKeyword);
        public Rule Delimiter => "[]{}()".ToCharSetRule();
        public Rule TypeKeyword => Node(Keywords("concept", "library", "type"));
        public Rule StatementKeyword => Node(Keywords("for", "if", "return", "break", "continue", "do", "foreach", "throw", "switch", "try", "catch", "finally", "using", "case", "default"));
        public Rule Token => Node(Separator | CppStyleComment | Spaces | OperatorToken | Identifier | Literal);
        public Rule TokenOrStructure => Named(Token | Structure);
        public Rule Tokenizer => TokenOrStructure.ZeroOrMore() + OnError(AdvanceToEnd) + EndOfInput;

        // Structural pass 
        public Rule TokenGroup => Node((Token | Separator).Except(Delimiter).OneOrMore());
        public Rule Element => Node(Structure | TokenGroup);
        public Rule BracedStructure => Node("{" + Element.ZeroOrMore() + "}");
        public Rule BracketedStructure => Node("[" + Element.ZeroOrMore() + "]");
        public Rule ParenthesizedStructure => Node("(" + Element.ZeroOrMore() + ")");
        public Rule InnerStructure => Named(BracketedStructure | ParenthesizedStructure | BracedStructure);
        public Rule Structure => Recursive(nameof(InnerStructure));
    }
}