    namespace Parakeet.Demos
{
    public class PlatoGrammar : PlatoTokenGrammar
    {
        public new static PlatoGrammar Instance = new PlatoGrammar();

        // Recovery on error 

        public override Rule Recovery => OnError(RepeatUntilPast(Token, EOS | "}"));

        // Basic 
        public override Rule WS => Named((SpaceChars | CppStyleComment).ZeroOrMore());

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
        public new Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());

        public Rule BinaryOperation => Node(Not("=>") + BinaryOperator + Recovery + Expression);
        public Rule TernaryOperation => Node(Symbol("?") + Recovery + Expression + Symbol(":") + Expression);
        public Rule ParenthesizedExpression => Node(ParenthesizedList(Expression));

        public Rule ThrowExpression => Node(Keyword("throw") + Recovery + Expression);
        public Rule LambdaParameter => Node(Identifier);
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

        public Rule FunctionDeclStatement => Node(MethodDeclaration);
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
        public new Rule TypeParameter => Node(Identifier + TypeAnnotation);
        public Rule TypeParameterList => Node(AngledBracketList(TypeParameter).Optional());

        public Rule ImplementsList => Node(Optional(Keyword("implements") + Recovery + List(TypeExpr)));
        public Rule InheritsList => Node(Optional(Keyword("inherits") + Recovery + List(TypeExpr)));

        public Rule Type => Node(Keyword("type") + Recovery + Identifier + TypeParameterList + ImplementsList +
                                 Braced(FieldDeclaration.ZeroOrMore()));

        public Rule Concept => Node(Keyword("concept") + Recovery + Identifier + TypeParameterList + InheritsList +
                                    Braced(MethodDeclaration.ZeroOrMore()));

        public Rule Library =>
            Node(Keyword("library") + Recovery + Identifier + Braced(MethodDeclaration.ZeroOrMore()));

        public Rule TopLevelDeclaration => Node(Library | Concept | Type);

        public Rule FunctionParameter => Node(Identifier + TypeAnnotation);
        public Rule FunctionParameterList => Node(ParenthesizedList(FunctionParameter));
        public Rule ExpressionBody => Node(Symbol("=>") + Recovery + Expression + EOS);
        public Rule FunctionBody => Node(ExpressionBody | CompoundStatement | EOS);
        public new Rule TypeAnnotation => Node(Symbol(":") + Recovery + TypeExpr);

        public Rule MethodDeclaration =>
            Node(Identifier + FunctionParameterList + Recovery + TypeAnnotation + FunctionBody);

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
    }
}