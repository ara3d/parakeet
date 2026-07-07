namespace Ara3D.Parakeet.Grammars
{
    /// <summary>
    /// IFC-focused EXPRESS grammar:
    /// - Parses ENTITY name, optional SUBTYPE OF (...), and explicit attributes.
    /// - Skips FUNCTION and RULE blocks entirely.
    /// - Robust WS/comment handling.
    /// </summary>
    public class ExpressGrammar : BaseCommonGrammar
    {
        public static readonly ExpressGrammar Instance = new ExpressGrammar();

        // IFC EXPRESS uses (* ... *) block comments (often multiline)
        public Rule Comment => Named("(*" + AnyCharUntilPast("*)"));

        // -----------------------------
        // Blocks 
        // These are used for parsing out chunks of the express file at a time.
        // Separately from code generation. 
        // -----------------------------

        public Rule EntityBlock => Node("ENTITY" + AnyCharUntilPast("END_ENTITY;"));
        public Rule TypeBlock => Node("TYPE" + AnyCharUntilPast("END_TYPE;"));
        public Rule EntityBlocks => Named(ZeroOrMore(EntityBlock | AnyChar));
        public Rule TypeBlocks => Named(ZeroOrMore(TypeBlock | AnyChar));

        // -----------------------------
        // Whitespace / Skipping
        // -----------------------------
        // Treat FUNCTION/RULE blocks as "skippable noise" (like comments).
        // This makes the file parse succeed even if we don't model those constructs.

        // Skippable should never generate a "Node"
        public Rule FunctionBlock => Named(KeywordNoWS("FUNCTION") + AnyCharUntilPast("END_FUNCTION;"));
        public Rule RuleBlock => Named(KeywordNoWS("RULE") + AnyCharUntilPast("END_RULE;"));
        public Rule SkippedSection => Named(FunctionBlock | RuleBlock);

        public override Rule WS => Named((SpaceChars | Comment | SkippedSection).ZeroOrMore());

        // -----------------------------
        // Tokens / identifiers
        // -----------------------------

        public new Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());
        public Rule Eos => Named(Sym(";"));
        public Rule StringLiteralChar => Named("''" | Not("'") + AnyChar);
        public Rule StringLiteral => Node("'" + StringLiteralChar.ZeroOrMore() + "'");

        // -----------------------------
        // Bounds and aggregation
        // -----------------------------

        public Rule QMark => Named(Sym("?")); 
        public Rule Bound => Named(Integer | QMark);
        public Rule Dim => Named(Sym("[") + Bound + Sym(":") + Bound + Sym("]"));
        public Rule AggregationKeyword => Keywords("SET", "LIST", "BAG", "ARRAY");
        public Rule UniqueKeyword => Keyword("UNIQUE");
        public Rule OptionalKeyword => Keyword("OPTIONAL");
        public Rule OfKeyword => Keyword("OF");

        // Aggregation: SET [1:?] OF <TypeExpr>
        // Also supports "SET OF ..." (no dimension) which appears in local declarations sometimes.
        // Also supports "OF UNIQUE <TypeExpr>" which EXPRESS allows in some contexts.
        public Rule AggregationType => Node(
            AggregationKeyword
            + Dim.Optional()
            + OfKeyword 
            + UniqueKeyword.Optional()
            + Recursive(nameof(TypeExpr)));

        // Type expression we care about for IFC entities:
        // OPTIONAL? (AggregationType | NamedType)
        public Rule TypeExpr 
            => Node(OptionalKeyword.Optional() + (AggregationType | Identifier));

        // -----------------------------
        // TYPE parsing (for enums, selects, aliases)
        // -----------------------------

        public Rule EndType 
            => Named(Keyword("END_TYPE") + Eos);

        // TYPE IfcFoo = <TypeDef> ; END_TYPE;
        public Rule TypeDecl => Node(
            Keyword("TYPE")
            + Identifier
            + Sym("=") + AbortOnFail
            + TypeDef
            + Eos
            + EndType);

        // What can appear on the RHS of "TYPE X = ...;"
        public Rule TypeDef => Node(
            EnumerationType
            | SelectType
            | TypeExpr   
        );

        // ENUMERATION OF (A, B, C)
        public Rule EnumerationType 
            => Node(Keyword("ENUMERATION") + Keyword("OF") + IdentifierList);

        // SELECT (A, B, C)
        public Rule SelectType
            => Node(Keyword("SELECT") + IdentifierList);
        
        // -----------------------------
        // ENTITY parsing 
        // -----------------------------

        // Header clauses WITHOUT trailing semicolon
        public Rule SubtypeHeader 
            => Node(Keyword("SUBTYPE") + Keyword("OF") + IdentifierList);

        // SUPERTYPE OF(ONEOF (A, B))
        public Rule OneOfSupertype 
            => Node(Parenthesized(Keyword("ONEOF") + WS + IdentifierList));

        public Rule SupertypeHeader 
            => Node(Keyword("ABSTRACT").Optional() + Keyword("SUPERTYPE") + Keyword("OF") + (OneOfSupertype | IdentifierList));

        // Entity header ends at the FIRST semicolon after ENTITY name and optional headers.
        public Rule EntityHeader => Node(
            Keyword("ENTITY")
            + AbortOnFail
            + Identifier
            + SupertypeHeader.Optional()
            + SubtypeHeader.Optional() 
            + Eos);
        
        // -----------------------------
        // ENTITY headers: abstract/supertype/subtype
        // -----------------------------

        // (A, B, C)
        public Rule IdentifierList 
            => Node(ParenthesizedList(Identifier));

        // Attribute declaration:
        //   Name : TypeExpr;
        public Rule AttributeDecl => Node(Identifier + Sym(":") + AbortOnFail + TypeExpr + Eos);

        // Sections we mostly ignore (but must not break parse):
        // DERIVE ... WHERE ... INVERSE ... UNIQUE ... etc.
        // We'll just "eat" them safely until END_ENTITY.
        public Rule EntityInnerSectionHeader 
            => Named(Keyword("DERIVE") | Keyword("WHERE") | Keyword("INVERSE") | Keyword("UNIQUE") | Keyword("LOCAL") | Keyword("END_ENTITY"));

        public Rule EntityInnerSections 
            => Named(EntityInnerSectionHeader.At() + AnyCharUntilAt(Keyword("END_ENTITY")));

        // Explicit attribute lines appear before DERIVE/WHERE/INVERSE (typically).
        // We'll parse as many AttributeDecl as possible, then skip other sections until END_ENTITY.
        public Rule EntityBody => Node(AttributeDecl.ZeroOrMore());

        public Rule EndEntity => Named(Keyword("END_ENTITY") + Eos);

        // ENTITY IfcFoo; [SubtypeClause]? [SupertypeNoise]? <AttributeDecl...> END_ENTITY;
        public Rule Entity => Node(EntityHeader + AbortOnFail + EntityBody + EntityInnerSections + EndEntity);

        // -----------------------------
        // Top-level (file) parsing
        // -----------------------------

        // Optional SCHEMA header/footer (IFC has them)
        public Rule SchemaHeader => Node(Keyword("SCHEMA") + Identifier + Eos);
        public Rule EndSchema => Node(Keyword("END_SCHEMA") + Eos);

        // Keep skipping FUNCTION/RULE blocks and comments
        public Rule OtherTopLevelNoise => Named(SkippedSection | Comment);

        // If you still want to tolerate unknown blocks, you can keep a “fallback eater”
        public Rule UnknownTopLevelChunk => Named(AnyChar); // last resort

        public Rule TopLevelDecl => Named(
            TypeDecl
            | Entity
            | OtherTopLevelNoise
            | UnknownTopLevelChunk);

        public Rule File => Node(
            WS
            + SchemaHeader.Optional() + WS
            + TopLevelDecl.ZeroOrMore() + WS
            + EndSchema.Optional() + WS
            + EndOfInput
        );

        public override Rule StartRule => File;
    }
}
