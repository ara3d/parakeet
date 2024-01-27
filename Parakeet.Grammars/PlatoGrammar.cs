    namespace Parakeet.Grammars
{
    public class PlatoGrammar : PlatoTokenGrammar
    {
        public new static readonly PlatoGrammar Instance = new PlatoGrammar();
        public override Rule StartRule => File;

        // Recovery on error 

        public Rule NoRecovery => OnError(Token.RepeatUntilPast(EOS | "}"));

        // Basic 
        public override Rule WS => Named((SpaceChars | Comment).ZeroOrMore());

        public Rule Comment => Named(CppStyleComment);
        public Rule CastExpression => Node(Parenthesized(TypeExpr) + Expression);
        public Rule PrefixOperator => Node(Symbols("++", "--", "!", "-", "+", "~"));
        public Rule Indexer => Node(Bracketed(Expression));

        public Rule PostfixOperator => Node(
            Symbols("++", "--")
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

        public Rule BinaryOperation => Node(Not("=>") + BinaryOperator + NoRecovery + Expression);
        public Rule TernaryOperation => Node(Sym("?") + NoRecovery + Expression + Sym(":") + Expression);
        public Rule ParenthesizedExpression => Node(ParenthesizedList(Expression));

        public Rule ThrowExpression => Node(Keyword("throw") + NoRecovery + Expression);
        public Rule LambdaParameter => Node(Identifier);
        public Rule LambdaParameters => Node(LambdaParameter | ParenthesizedList(LambdaParameter));
        public Rule LambdaBody => Node(CompoundStatement | Expression);
        public Rule LambdaExpr => Node(LambdaParameters + Sym("=>") + NoRecovery + LambdaBody);
        public Rule MemberAccess => Node(Sym(".") + Identifier);
        public Rule ConditionalMemberAccess => Node(Sym("?.") + Identifier);
        public Rule TypeOf => Node(Keyword("typeof") + NoRecovery + Parenthesized(TypeExpr));
        public Rule NameOf => Node(Keyword("nameof") + NoRecovery + Parenthesized(Expression));
        public Rule Default => Node(Keyword("default") + NoRecovery + Parenthesized(TypeExpr).Optional());
        public Rule InitializerClause => Node(Identifier + Sym("=") + NoRecovery + Expression | Expression);
        public Rule Initializer => Node(BracedList(InitializerClause));
        public Rule ArraySizeSpecifier => Node(Bracketed(Expression));

        public Rule NewOperation => Node(Keyword("new") + NoRecovery + TypeExpr.Optional() + FunctionArgs.Optional() +
                                         ArraySizeSpecifier.Optional() + Initializer.Optional());

        public Rule IsOperation => Node(Keyword("is") + NoRecovery + TypeExpr + Identifier.Optional());
        public Rule AsOperation => Node(Keyword("as") + NoRecovery + TypeExpr + Identifier.Optional());
        public Rule StringInterpolationContent => Node(Braced(Expression) | StringLiteralChar);
        public Rule StringInterpolation => Node("$" + NoRecovery + "\"" + StringInterpolationContent.ZeroOrMore() + "\"");
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

        public Rule Expression => Node(Recursive(nameof(InnerExpression)));

        // Statements 
        public Rule EOS => Named(Sym(";"));
        public Rule ExpressionStatement => Node(Expression + NoRecovery + EOS);
        public Rule ElseClause => Node(Keyword("else") + NoRecovery + Statement);

        public Rule IfStatement =>
            Node(Keyword("if") + NoRecovery + ParenthesizedExpression + Statement + ElseClause.Optional());

        public Rule WhileStatement => Node(Keyword("while") + NoRecovery + ParenthesizedExpression + Statement);

        public Rule DoWhileStatement =>
            Node(Keyword("do") + NoRecovery + Statement + Keyword("while") + ParenthesizedExpression + EOS);

        public Rule ReturnStatement => Node(Keyword("return") + NoRecovery + Expression.Optional() + EOS);
        public Rule BreakStatement => Node(Keyword("break") + NoRecovery + EOS);

        public Rule YieldStatement => Node(Keyword("yield") + NoRecovery + (YieldReturn | YieldBreak));
        public Rule YieldReturn => Node(Keyword("return") + NoRecovery + Expression + EOS);
        public Rule YieldBreak => Node(Keyword("break") + NoRecovery + EOS);
        public Rule ContinueStatement => Node(Keyword("continue") + NoRecovery + EOS);

        public Rule CompoundStatement => Node(Braced(Statement.ZeroOrMore()));
        public Rule CatchClause => Node(Keyword("catch") + NoRecovery + Parenthesized(VarDecl) + CompoundStatement);
        public Rule FinallyClause => Node(Keyword("finally") + CompoundStatement);
        public Rule CaseClause => Node((Keyword("default") | Keyword("case") + NoRecovery + Expression).Then(Statement));
        public Rule SwitchStatement => Node(Keyword("switch") + NoRecovery + Braced(CaseClause.ZeroOrMore()));

        public Rule TryStatement => Node(Keyword("try") + NoRecovery + CompoundStatement + CatchClause.Optional() +
                                         FinallyClause.Optional());

        public Rule ForEachStatement => Node(Keyword("foreach") + NoRecovery + Sym("(") + TypeExpr + Identifier +
                                             Keyword("in") + Expression + Sym(")") + Statement);

        public Rule FunctionDeclStatement => Node(MethodDeclaration);
        public Rule ForLoopInit => Node(VarDecl.Optional());
        public Rule ForLoopInvariant => Node(Expression.Optional());
        public Rule ForLoopVariant => Node(List(Expression));

        public Rule ForStatement => Node(Keyword("for") + NoRecovery + Sym("(") + ForLoopInit + EOS +
                                         ForLoopInvariant + EOS + ForLoopVariant + Sym(")") + Statement);

        public Rule ArrayInitializationValue => Node(BracedList(Expression));
        public Rule InitializationValue => Node(ArrayInitializationValue | Expression);
        public Rule Initialization => Node((Sym("=") + NoRecovery + InitializationValue).Optional());
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

        public Rule QualifiedIdentifier => Node(List(Identifier, Sym(".")));
        public Rule TypeParameter => Node(Identifier);
        public Rule TypeParameterList => Node(AngleBracketedList(TypeParameter).Optional());

        public Rule ImplementsList => Node(Optional(Keyword("implements") + NoRecovery + List(TypeExpr)));
        public Rule InheritsList => Node(Optional(Keyword("inherits") + NoRecovery + List(TypeExpr)));
        public Rule Constraint => Node(Identifier + TypeAnnotation);
        public Rule ConstraintList => Node(Optional(Keyword("where") + NoRecovery + List(Constraint)));

        public Rule Type => Node(Keyword("type") + NoRecovery + Identifier + TypeParameterList + ImplementsList +
                                 Braced(FieldDeclaration.ZeroOrMore()));

        public Rule Concept => Node(Keyword("concept") + NoRecovery + Identifier + TypeParameterList + ConstraintList + InheritsList +
                                    Braced(MethodDeclaration.ZeroOrMore()));

        public Rule Library =>
            Node(Keyword("library") + NoRecovery + Identifier + Braced(MethodDeclaration.ZeroOrMore()));

        public Rule TopLevelDeclaration => Node(Library | Concept | Type);

        public Rule FunctionParameter => Node(Identifier + TypeAnnotation);
        public Rule FunctionParameterList => Node(ParenthesizedList(FunctionParameter));
        public Rule ExpressionBody => Node(Sym("=>") + NoRecovery + Expression + EOS);
        public Rule FunctionBody => Node(ExpressionBody | CompoundStatement | EOS);
        public Rule TypeAnnotation => Node(Sym(":") + NoRecovery + TypeExpr);

        public Rule MethodDeclaration =>
            Node(Identifier + FunctionParameterList + NoRecovery + TypeAnnotation + FunctionBody);

        public Rule FieldDeclaration => Node(Identifier + Sym(":") + TypeExpr + EOS);
        public Rule MemberDeclaration => Node(MethodDeclaration | FieldDeclaration);

        public Rule ArrayRankSpecifier => Node(Bracketed(Comma.ZeroOrMore()));
        public Rule ArrayRankSpecifiers => Node(ArrayRankSpecifier.ZeroOrMore());
        public Rule TypeArgList => Node(AngleBracketedList(TypeExpr));
        public Rule TypeVar => Node('$' + Identifier);
        public Rule SimpleTypeExpr => Node(Identifier);
        public Rule CompoundTypeExpr => Node(ParenthesizedList(TypeExpr));
        public Rule CompoundOrSimpleTypeExpr => Node(CompoundTypeExpr | SimpleTypeExpr | TypeVar);
        public Rule InnerTypeExpr => Node(CompoundOrSimpleTypeExpr + TypeArgList.Optional() + ArrayRankSpecifiers);
        public Rule TypeExpr => Node(Recursive(nameof(InnerTypeExpr)));

        public Rule File => Node(WS + TopLevelDeclaration.ZeroOrMore() + EndOfInput);
    }
}