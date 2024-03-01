namespace Ara3D.Parakeet.Grammars
{
    // This is currently a parser for a subset of the C# language 
    // See: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure
    // First step though is to test this on my own source code. 

    public class CSharpGrammar : BaseCommonGrammar
    {
        public static CSharpGrammar Instance = new CSharpGrammar();
        public override Rule StartRule => File;

        // Recovery on error 
        public Rule RecoverEos => OnFail(
            TokenOrStructure.Except(EOS).ZeroOrMore()
            + (EOS | EndOfInput) | AdvanceToEnd);

        // Helper functions 
        public Rule IntegerSuffix => Named(Strings("ul", "UL", "u", "U", "l", "L", "lu", "lU", "Lu", "LU"));
        public Rule FloatSuffix => Named("fFdDmM".ToCharSetRule());

        // Basic 
        public override Rule WS => Named((SpaceChars | CppStyleComment | Directive).ZeroOrMore());

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
            "+", "-", "*", "/", "%", ">>>", ">>", "<<", "&&", "||", "&", "|", "^",
            "+=", "-=", "*=", "/=", "%=", ">>>=", ">>=", "<<=", "&&=", "||=", "&=", "|=", "^=",
            "=", "<", ">", "<=", ">=", "==", "!=",
            "??", "?="
            ));

        public Rule CastExpression => Node(Parenthesized(TypeExpr) + Expression);
        public Rule PrefixOperator => Node(Symbols("++", "--", "!", "-", "+", "~"));
        public Rule Indexer => Node(Bracketed(Expression));
        public Rule OverloadableOperator => Node(Symbols("+", "-", "!", "~", "++", "--", "*", "/", "%", "&", "|", "^", "<<", ">>", ">>>", "==", "!=", "<", ">", "<=", ">="));

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

        public Rule BinaryOperation => Node(Not("=>") + BinaryOperator + RecoverEos + Expression);
        public Rule TernaryOperation => Node(Sym("?") + RecoverEos + Expression + Sym(":") + Expression);
        public Rule ParenthesizedExpression => Node(ParenthesizedList(Expression));

        public Rule ThrowExpression => Node(Keyword("throw") + RecoverEos + Expression);
        public Rule LambdaParameter => Node(TypeExpr + Identifier | Identifier);
        public Rule LambdaParameters => Node(LambdaParameter | ParenthesizedList(LambdaParameter));
        public Rule LambdaBody => Node(CompoundStatement | Expression);
        public Rule LambdaExpr => Node(LambdaParameters + Sym("=>") + RecoverEos + LambdaBody);
        public Rule MemberAccess => Node(Sym(".") + Identifier);
        public Rule ConditionalMemberAccess => Node(Sym("?.") + Identifier);
        public Rule TypeOf => Node(Keyword("typeof") + RecoverEos + Parenthesized(TypeExpr));
        public Rule NameOf => Node(Keyword("nameof") + RecoverEos + Parenthesized(Expression));
        public Rule Default => Node(Keyword("default") + RecoverEos + Parenthesized(TypeExpr).Optional());
        public Rule InitializerClause => Node((Identifier + Sym("=") + RecoverEos + Expression) | Expression);
        public Rule Initializer => Node(BracedList(InitializerClause));
        public Rule ArraySizeSpecifier => Node(Bracketed(Expression));
        public Rule NewOperation => Node(Keyword("new") + RecoverEos + TypeExpr.Optional() + FunctionArgs.Optional() + ArraySizeSpecifier.Optional() + Initializer.Optional());
        public Rule IsOperation => Node(Keyword("is") + RecoverEos + TypeExpr + Identifier.Optional());
        public Rule AsOperation => Node(Keyword("as") + RecoverEos + TypeExpr + Identifier.Optional());
        public Rule StringInterpolationContent => Node(Braced(Expression) | StringLiteralChar);
        public Rule StringInterpolation => Node("$" + RecoverEos + "\"" + StringInterpolationContent.ZeroOrMore() + "\"");
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

        public Rule InnerExpression => Node(
            PrefixOperator.ZeroOrMore()
            + LeafExpression
            + PostfixOperator.ZeroOrMore());

        public Rule Expression => Node(Recursive(nameof(InnerExpression)));

        // Statements 
        public Rule EOS => Named(Sym(";"));
        public Rule ExpressionStatement => Node(Expression + RecoverEos + EOS);
        public Rule ElseClause => Node(Keyword("else") + RecoverEos + Statement);
        public Rule IfStatement => Node(Keyword("if") + RecoverEos + ParenthesizedExpression + Statement + ElseClause.Optional());
        public Rule WhileStatement => Node(Keyword("while") + RecoverEos + ParenthesizedExpression + Statement);
        public Rule DoWhileStatement => Node(Keyword("do") + RecoverEos + Statement + Keyword("while") + ParenthesizedExpression + EOS);
        public Rule ReturnStatement => Node(Keyword("return") + RecoverEos + Expression.Optional() + EOS);
        public Rule BreakStatement => Node(Keyword("break") + RecoverEos + EOS);

        public Rule YieldStatement => Node(Keyword("yield") + RecoverEos + (YieldReturn | YieldBreak));
        public Rule YieldReturn => Node(Keyword("return") + RecoverEos + Expression + EOS);
        public Rule YieldBreak => Node(Keyword("break") + RecoverEos + EOS);
        public Rule ContinueStatement => Node(Keyword("continue") + RecoverEos + EOS);

        public Rule CompoundStatement => Node(Braced(Statement.ZeroOrMore()));
        public Rule CatchClause => Node(Keyword("catch") + RecoverEos + Parenthesized(VarDecl) + CompoundStatement);
        public Rule FinallyClause => Node(Keyword("finally") + CompoundStatement);
        public Rule CaseClause => Node((Keyword("default") | Keyword("case") + RecoverEos + Expression).Then(Statement));
        public Rule SwitchStatement => Node(Keyword("switch") + RecoverEos + Braced(CaseClause.ZeroOrMore()));
        public Rule TryStatement => Node(Keyword("try") + RecoverEos + CompoundStatement + CatchClause.Optional() + FinallyClause.Optional());
        public Rule ForEachStatement => Node(Keyword("foreach") + RecoverEos + Sym("(") + TypeExpr + Identifier + Keyword("in") + Expression + Sym(")") + Statement);
        public Rule FunctionDeclStatement => Node(Keyword("static").Optional() + MethodDeclaration);
        public Rule ForLoopInit => Node(VarDecl.Optional());
        public Rule ForLoopInvariant => Node(Expression.Optional());
        public Rule ForLoopVariant => Node(List(Expression));
        public Rule ForStatement => Node(Keyword("for") + RecoverEos + Sym("(") + ForLoopInit + EOS + ForLoopInvariant + EOS + ForLoopVariant + Sym(")") + Statement);
        public Rule ArrayInitializationValue => Node(BracedList(Expression));
        public Rule InitializationValue => Node(ArrayInitializationValue | Expression);
        public Rule Initialization => Node((Sym("=") + RecoverEos + InitializationValue).Optional());
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

        public Rule Statement => Node(Recursive(nameof(InnerStatement)));

        public Rule Directive =>
            ((Rule)"#region" | "#endregion" | "#error" | "#warning" | "#line" | "#pragma" | "#if" | "#else" | "#elif" | "#endif" | "#define" | "#nullable" | "#undef")
            + AnyCharUntilNextLine;

        public Rule QualifiedIdentifier => Node(List(Identifier, Sym(".")));
        public Rule Static => Node(Keyword("static").Optional());
        public Rule UsingDirective => Node(Keyword("global").Optional() + Keyword("using") + RecoverEos + Static + QualifiedIdentifier + Optional(Sym("=") + QualifiedIdentifier) + EOS);

        public Rule Modifier => Node(Keywords("static", "sealed", "partial", "readonly", "const", "ref", "abstract", "virtual"));
        public Rule AccessSpecifier => Node(Keywords("public", "private", "protected", "internal"));
        public Rule ModifiersAndSpecifiers => (Modifier | AccessSpecifier).ZeroOrMore();
        public Rule Attribute => Node(Identifier + FunctionArgs.Optional());
        public Rule AttributeGroup => Node(Bracketed(List(Attribute)));
        public Rule AttributeList => Node(AttributeGroup.ZeroOrMore());
        public Rule DeclarationPreamble => Node(AttributeList + ModifiersAndSpecifiers);
        public Rule TypeVariance => Node(Keywords("in", "out").Optional());
        public Rule TypeParameter => Node(TypeVariance + Identifier);
        public Rule TypeParameterList => Node(AngleBracketedList(TypeParameter).Optional());
        public Rule BaseClassList => Node((Sym(":") + List(TypeExpr)).Optional());
        public Rule Constraint => Node(Keyword("class") | Keyword("struct") | TypeExpr);
        public Rule ConstraintClause => Node(Keyword("where") + Identifier + Sym(":") + TypeExpr);
        public Rule ConstraintList => Node(ConstraintClause.ZeroOrMore());

        public Rule Kind => Node(Keywords("class", "struct", "interface", "enum"));
        public Rule InnerTypeDeclaration => Named(Kind + RecoverEos + Identifier + TypeParameterList + BaseClassList + ConstraintList + Braced(MemberDeclaration.ZeroOrMore()));
        public Rule TypeDeclaration => Node(Recursive(nameof(InnerTypeDeclaration)));
        public Rule TypeDeclarationWithPreamble => Node(DeclarationPreamble + TypeDeclaration);

        public Rule FunctionParameterKeywords => Node(Keywords("this", "ref", "out", "in", "params").Optional());
        public Rule FunctionParameterDefaultValue => Node((Sym("=") + Expression).Optional());
        public Rule FunctionParameter => Node(AttributeList + FunctionParameterKeywords + TypeExpr + Identifier + FunctionParameterDefaultValue);
        public Rule FunctionParameterList => Node(ParenthesizedList(FunctionParameter));
        public Rule ExpressionBody => Node(Sym("=>") + RecoverEos + ((Expression + EOS) | CompoundStatement));
        public Rule FunctionBody => Node(ExpressionBody | CompoundStatement | EOS);
        public Rule BaseCall => Node(Keyword("base") + RecoverEos + ParenthesizedExpression);
        public Rule ThisCall => Node(Keyword("this") + RecoverEos + ParenthesizedExpression);
        public Rule BaseOrThisCall => Node((Sym(":") + (BaseCall | ThisCall)).Optional());
        public Rule ConstructorDeclaration => Node(Identifier + FunctionParameterList + RecoverEos + BaseOrThisCall + FunctionBody);
        public Rule MethodDeclaration => Node(TypeExpr + Identifier + FunctionParameterList + RecoverEos + FunctionBody);
        public Rule FieldDeclaration => Node(VarDeclStatement);

        public Rule Getter => Node(Keyword("get") + RecoverEos + (EOS | FunctionBody));
        public Rule Setter => Node(Keyword("set") + RecoverEos + (EOS | FunctionBody));
        public Rule Initter => Node(Keyword("init") + RecoverEos + (EOS | FunctionBody));
        public Rule PropertyClauses => Node((Getter | Setter | Initter).OneOrMore());
        public Rule PropertyWithClauses => Node(Braced(PropertyClauses) + Optional(Initialization + EOS));
        public Rule PropertyExpression => Node(ExpressionBody);
        public Rule PropertyBody => Node(PropertyExpression | PropertyWithClauses);
        public Rule PropertyDeclaration => Node(TypeExpr + Identifier + PropertyBody);
        public Rule IndexerDeclaration => Node(TypeExpr + Keyword("this") + RecoverEos + Bracketed(FunctionParameter) + PropertyBody);
        public Rule OperatorDeclaration => Node(TypeExpr + Keyword("operator") + RecoverEos + OverloadableOperator + FunctionParameterList + FunctionBody);
        public Rule ImplicitOrExplicit => Node(Keywords("implicit", "explicit"));
        public Rule ConverterDeclaration => Node(TypeExpr + ImplicitOrExplicit + Keyword("operator") + TypeExpr + FunctionBody);

        public Rule MemberDeclaration => Node(DeclarationPreamble
            + (ConstructorDeclaration
            | MethodDeclaration
            | IndexerDeclaration
            | PropertyDeclaration
            | FieldDeclaration
            | OperatorDeclaration
            | ConverterDeclaration
            | TypeDeclaration
            ));

        public Rule TypesAndDirectives => Named((UsingDirective | TypeDeclarationWithPreamble).ZeroOrMore());
        public Rule ImplicitlyOrExplicitlyScopedTypes => Named(EOS + TypesAndDirectives) | Braced(TypesAndDirectives);
        public Rule NamespaceDeclaration => Node(Keyword("namespace") + QualifiedIdentifier + ImplicitlyOrExplicitlyScopedTypes);
        public Rule File => Node(WS + (EndOfInput.NotAt() + UsingDirective.ZeroOrMore() + NamespaceDeclaration.Optional()).ZeroOrMore());

        public Rule ArrayRankSpecifier => Node(Bracketed(Comma.ZeroOrMore()));
        public Rule ArrayRankSpecifiers => Node(ArrayRankSpecifier.ZeroOrMore());
        public Rule TypeArgList => Node(AngleBracketedList(TypeExpr));
        public Rule Nullable => Node(Sym("?").Optional());
        public Rule SimpleTypExpr => Node(QualifiedIdentifier);
        public Rule CompoundTypeExpr => Node(ParenthesizedList(TypeExpr));
        public Rule CompoundOrSimpleTypeExpr => Node(CompoundTypeExpr | SimpleTypExpr);
        public Rule InnerTypeExpr => Node(CompoundOrSimpleTypeExpr + TypeArgList.Optional() + ArrayRankSpecifiers);
        public Rule TypeExpr => Node(Recursive(nameof(InnerTypeExpr)));

        // Tokenization pass 
        public Rule OperatorChar => "!%^&|*?+-=/><:@#~$".ToCharSetRule();
        public Rule OperatorToken => OperatorChar.OneOrMore();
        public Rule Separator => Node(";,.".ToCharSetRule() | TypeKeyword | StatementKeyword);
        public Rule Delimiter => "[]{}()".ToCharSetRule();
        public Rule TypeKeyword => Node(Keywords("class", "struct", "interface", "enum"));
        public Rule StatementKeyword => Node(Keywords("for", "if", "return", "break", "continue", "do", "foreach", "throw", "switch", "try", "catch", "finally", "using", "case", "default"));
        public Rule Token => Node(Separator | CppStyleComment | Spaces | OperatorToken | Identifier | Literal);
        public Rule TokenOrStructure => Named(Token | Structure);
        public Rule Tokenizer => TokenOrStructure.ZeroOrMore() + OnFail(AdvanceToEnd) + EndOfInput;

        // Structural pass 
        public Rule TokenGroup => Node((Token | Separator).Except(Delimiter).OneOrMore());
        public Rule TypeStructure => Node(TypeKeyword + TokenGroup + BracedStructure);
        public Rule StatementStructure => Node(StatementKeyword + TokenGroup + BracedStructure.Optional());
        public Rule Element => Node(Structure | TokenGroup);
        public Rule BracedStructure => Node("{" + Element.ZeroOrMore() + "}");
        public Rule BracketedStructure => Node("[" + Element.ZeroOrMore() + "]");
        public Rule ParenthesizedStructure => Node("(" + Element.ZeroOrMore() + ")");
        public Rule InnerStructure => Named(BracketedStructure | ParenthesizedStructure | BracedStructure);
        public Rule Structure => Node(Recursive(nameof(InnerStructure)));
        public Rule FileStructure => Node(Element.ZeroOrMore());

        // Script rule
        public Rule TypeDirectiveOrStatement => Node(UsingDirective | TypeDeclarationWithPreamble | Statement);
        public Rule Script => Node(WS + TypeDirectiveOrStatement.ZeroOrMore());

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