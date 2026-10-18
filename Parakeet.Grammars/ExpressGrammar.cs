namespace Ara3D.Parakeet.Grammars
{
    /// <summary>
    /// EXPRESS grammar for generating schema types (IFC and the STEP application protocols):
    /// - Parses ENTITY name, the supertype expression, SUBTYPE OF (...), explicit attributes
    ///   (including SELF\entity.attribute redeclarations), and DERIVE attribute names.
    /// - Skips WHERE, INVERSE, UNIQUE, FUNCTION and RULE contents.
    /// Rules after an AbortOnFail recover by jumping to the end and record ParserState.LastError,
    /// so a caller must check LastError, not only AtEnd, to know a block was read in full.
    /// </summary>
    public class ExpressGrammar : BaseCommonGrammar
    {
        public static readonly ExpressGrammar Instance = new ExpressGrammar();

        // IFC EXPRESS uses (* ... *) block comments (often multiline)
        public Rule Comment => Named("(*" + AnyCharUntilPast("*)"));

        // The STEP long forms also use -- line comments (the AP214 header has one with an apostrophe)
        public Rule LineComment => Named("--" + AnyCharUntilNextLine);

        // -----------------------------
        // Blocks 
        // These are used for parsing out chunks of the express file at a time.
        // Separately from code generation. 
        // -----------------------------

        // A block starts only at a whole keyword outside comments and strings: whole words,
        // comments and quoted strings are skipped, so SUBTYPE, TYPEOF and 'X.TYPE' never start a
        // TYPE block; skipping whole words also stops the scan from rescanning to the end of the file at each.
        // The keyword tests are inline, not KeywordNoWS, because Named caches rules by name and
        // KeywordNoWS("TYPE") would replace Keyword("TYPE") in TypeDecl, or the reverse.
        public Rule SkippedWord => Named(IdentifierFirstChar + IdentifierChar.ZeroOrMore());
        public Rule SkippedString => Named("'" + StringLiteralChar.ZeroOrMore() + "'");
        public Rule SkippedInBlockScan => Named(SkippedWord | Comment | LineComment | SkippedString | AnyChar);
        public Rule EntityBlock => Node("ENTITY" + IdentifierChar.NotAt() + AnyCharUntilPast("END_ENTITY;"));
        public Rule TypeBlock => Node("TYPE" + IdentifierChar.NotAt() + AnyCharUntilPast("END_TYPE;"));
        public Rule EntityBlocks => Named(ZeroOrMore(EntityBlock | SkippedInBlockScan));
        public Rule TypeBlocks => Named(ZeroOrMore(TypeBlock | SkippedInBlockScan));

        // -----------------------------
        // Whitespace / Skipping
        // -----------------------------
        // Treat FUNCTION/RULE blocks as "skippable noise" (like comments).
        // This makes the file parse succeed even if we don't model those constructs.

        // Skippable should never generate a "Node"
        public Rule FunctionBlock => Named(KeywordNoWS("FUNCTION") + AnyCharUntilPast("END_FUNCTION;"));
        public Rule RuleBlock => Named(KeywordNoWS("RULE") + AnyCharUntilPast("END_RULE;"));
        public Rule SkippedSection => Named(FunctionBlock | RuleBlock);

        public override Rule WS => Named((SpaceChars | Comment | LineComment | SkippedSection).ZeroOrMore());

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
        // A bound is ?, an integer, or an expression such as segments or ypr_index(roll)
        public Rule BoundTerm => Named(Integer | SkippedWord | Parenthesized(Recursive(nameof(BoundExpr))) | CharSet('+', '-', '*', '/'));
        public Rule BoundExpr => Named((BoundTerm + WS).OneOrMore());
        public Rule Bound => Named(QMark | BoundExpr);
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
            + OptionalKeyword.Optional()
            + UniqueKeyword.Optional()
            + Recursive(nameof(TypeExpr)));

        // Type expression we care about for IFC entities:
        // OPTIONAL? (AggregationType | NamedType)
        public Rule TypeExpr 
            => Node(OptionalKeyword.Optional() + (AggregationType | Identifier + StringWidth.Optional()));

        // STRING(255), BINARY(32) FIXED: the width is not modelled.
        public Rule StringWidth
            => Named(Parenthesized(Integer) + Keyword("FIXED").Optional());

        // -----------------------------
        // TYPE parsing (for enums, selects, aliases)
        // -----------------------------

        public Rule EndType 
            => Named(Keyword("END_TYPE") + Eos);

        public Rule TypeWhere
            => Named(Keyword("WHERE") + AnyCharUntilAt(Keyword("END_TYPE")));

        // TYPE IfcFoo = <TypeDef> ; END_TYPE;
        public Rule TypeDecl => Node(
            Keyword("TYPE")
            + Identifier
            + Sym("=") + AbortOnFail
            + TypeDef
            + Eos
            + TypeWhere.Optional()
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

        // The supertype expression constrains which subtypes may be combined in one instance;
        // it has no effect on attribute order. Precedence: AND binds tighter than ANDOR.
        // SUPERTYPE OF (ONEOF(a, b) ANDOR c), SUPERTYPE OF ((ONEOF(a, b)) AND (ONEOF(c, d)))
        public Rule OneOfSupertype 
            => Node(Keyword("ONEOF") + ParenthesizedList(Recursive(nameof(SupertypeExpr))));

        public Rule SupertypeTerm
            => Node(OneOfSupertype | Parenthesized(Recursive(nameof(SupertypeExpr))) | Identifier);

        public Rule SupertypeAnd
            => Node(SupertypeTerm + (Keyword("AND") + SupertypeTerm).ZeroOrMore());

        public Rule SupertypeExpr
            => Node(SupertypeAnd + (Keyword("ANDOR") + SupertypeAnd).ZeroOrMore());

        // ABSTRACT SUPERTYPE; ABSTRACT SUPERTYPE OF (...); SUPERTYPE OF (...)
        public Rule SupertypeHeader 
            => Node(Keyword("ABSTRACT").Optional() + Keyword("SUPERTYPE") + (Keyword("OF") + Parenthesized(SupertypeExpr)).Optional());

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

        // SELF\supertype.attribute [RENAMED name]: an inherited attribute redeclared with a
        // narrower type (explicit) or as computed (DERIVE). It adds no position to a record.
        public Rule RedeclaredAttribute
            => Node(Keyword("SELF") + Sym("\\") + Identifier + Sym(".") + Identifier + (Keyword("RENAMED") + Identifier).Optional());

        public Rule AttributeName => Named(RedeclaredAttribute | Identifier);

        // Attribute declaration:
        //   Name : TypeExpr;
        //   SELF\supertype.Name : TypeExpr;
        public Rule AttributeDecl => Node(AttributeName + Sym(":") + AbortOnFail + TypeExpr + Eos);

        // DERIVE attributes: only the name is modelled; the type and expression are skipped.
        public Rule DeriveExpressionText => Named((SkippedString | Not(";") + AnyChar).ZeroOrMore());
        public Rule DerivedAttribute => Node(AttributeName + Sym(":") + DeriveExpressionText + Eos);
        // Named, not a node: a node here would start where an empty EntityBody ends, and the parse tree
        // would then make the empty EntityBody its child (see ParserNode.IsParentOf). The DerivedAttribute
        // nodes are children of the Entity.
        public Rule DeriveSection => Named(Keyword("DERIVE") + DerivedAttribute.ZeroOrMore());

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
        public Rule Entity => Node(EntityHeader + AbortOnFail + EntityBody + DeriveSection.Optional() + EntityInnerSections + EndEntity);

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
