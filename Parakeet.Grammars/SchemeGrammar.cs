namespace Ara3D.Parakeet.Grammars
{
    // https://en.wikipedia.org/wiki/S-expression
    // https://en.wikipedia.org/wiki/Lisp_(programming_language)
    // https://en.wikipedia.org/wiki/Scheme_(programming_language)
    // https://gist.github.com/Idorobots/3378676
    // https://www.gnu.org/software/mit-scheme/documentation/stable/mit-scheme-ref/Strings.html#:~:text=Strings%20are%20written%20as%20sequences,start%20with%20a%20backslash%20(%20%5C%20)%3A
    // https://www.gnu.org/software/mit-scheme/documentation/stable/mit-scheme-ref/Characters.html
    // https://www.cs.cmu.edu/Groups/AI/html/r4rs/r4rs_9.html#SEC67
    // Skipping QuasiQuotation
    public class SchemeGrammar : BaseCommonGrammar
    {
        public static readonly SchemeGrammar Instance = new SchemeGrammar();
        public override Rule StartRule => Program;

        public Rule InnerDatum => Node(SimpleDatum | CompoundDatum);
        public Rule Datum => Node(Recursive(nameof(InnerDatum)));
        public Rule SimpleDatum => Node(Boolean | Number | Character | String | Symbol);
        public Rule Symbol => Node(Identifier);
        public Rule CompoundDatum => Node(List | Vector);

        public new Rule List => Node(Parenthesized(Datum.ZeroOrMore())
                                     | Parenthesized(Datum.OneOrMore() + Sym(".") + Datum)
                                     | Abbreviation);

        public Rule Abbreviation => Node(AbbrevPrefix + Datum);
        public Rule AbbrevPrefix => Node("'`,".ToCharSetRule() | ",@");
        public Rule Vector => Node('#' + Parenthesized(Datum.ZeroOrMore()));

        public Rule Token => Node(IdentifierChar | Boolean | Number | Character | String | "(" | ")" | "#(" | "'" |
                                  "`{" | "," | ".");

        public Rule Delimiter => Node(WS | "(" | ")" | '"' | ";");
        public override Rule WS => Named(SpaceChars | Comment);
        public Rule Comment => Named(";" + AnyCharUntilNextLine);
        public new Rule Identifier => Node(PeculiarIdentifier | Initial + Subsequent.ZeroOrMore());
        public Rule Initial => Letter | SpecialInitial;
        public Rule SpecialInitial => "!$%&*/:<=>?_^";
        public Rule Subsequent => Initial | Digit | SpecialSubsequent;
        public Rule SpecialSubsequent => CharSet(".+-");
        public Rule PeculiarIdentifier => Keywords("+", "-", "...") + EndOfWord;
        public Rule EndOfWord => Not(Subsequent);

        public Rule SyntaxKeyword => (ExpressionKeyword | "else" | "=>" | "define" | "unquote" | "unquote-splicing") +
                                     EndOfWord;

        public Rule ExpressionKeyword => Keywords("quote", "lambda", "if", "set!", "begin",
            "cond", "and", "or", "case", "let", "let*", "letrec", "do", "delay", "quasiquote");

        public Rule Boolean => Keywords("#t", "#f") + EndOfWord;
        public Rule Character => "\\#" + CharacterName | "\\#" + AnyChar;
        public Rule CharacterName => Keywords("space", "newline");
        public Rule Number => Digit.ZeroOrMore();
        public Rule String => '\"' + AnyChar.Except(CharSet("\"\\")).ZeroOrMore() + '\"';

        public Rule Variable => Node(Identifier);

        public Rule InnerExpression => Node(
            Variable 
            | Literal 
            | ProcedureCall 
            | LambdaExpression 
            | Conditional 
            | Assignment 
            | DerivedExpression);

        public Rule Expression => Node(Recursive(nameof(InnerExpression)));

        public Rule Literal => Node(Quotation | SelfEvaluating);

        public Rule SelfEvaluating => Node(Boolean | Number | Character | String);

        public Rule Quotation => Node(("'" + Datum) | (Keyword("quote") + Datum));

        public Rule ProcedureCall => Node(Parenthesized(Operator + Operand.ZeroOrMore()));

        public Rule Operator => Node(Expression);
        public Rule Operand => Node(Expression);

        public Rule LambdaExpression => Node(Parenthesized(Keyword("lambda") + Formals + Body));

        public Rule Formals => Node(
            Parenthesized(Variable.ZeroOrMore())
            | Variable
            | Parenthesized(Variable.OneOrMore() + Sym(".") + Variable));

        public Rule Sequence => Node(Command.ZeroOrMore() + Expression);
        public Rule Command => Node(Expression);
        public Rule Body => Node(Definition.ZeroOrMore() + Sequence);

        public Rule Conditional => Node(Keyword("if") + Test + Consequent + Alternate);

        public Rule Test => Node(Expression);
        public Rule Consequent => Node(Expression);
        public Rule Alternate => Node(Expression.Optional());

        public Rule Assignment => Node(Keyword("set!") + Variable + Expression);

        public Rule CondKeyword => Node(Keyword("cond"));
        public Rule CaseKeyword => Node(Keyword("case"));
        public Rule ElseKeyword => Node(Keyword("else"));
        public Rule LetKeyword => Node(Keyword("let"));
        public Rule LetStarKeyword => Node(Keyword("let*"));
        public Rule LetRecKeyword => Node(Keyword("letrec"));
        public Rule AndKeyword => Node(Keyword("and"));
        public Rule OrKeyword => Node(Keyword("or"));
        public Rule BeginKeyword => Node(Keyword("begin"));
        public Rule DoKeyword => Node(Keyword("do"));
        public Rule DelayKeyword => Node(Keyword("delay"));
        public Rule ElseClause => Node(Parenthesized(ElseKeyword + Sequence));

        public Rule DerivedExpression => Node(Parenthesized(
            CondKeyword + CondClause.OneOrMore()
            | CondKeyword + CondClause.ZeroOrMore() + ElseClause)
            | CaseKeyword + Expression + CaseClause.OneOrMore()
            | CaseKeyword + CaseClause.ZeroOrMore() + ElseClause
            | AndKeyword + Test.ZeroOrMore()
            | OrKeyword + Test.ZeroOrMore()
            | LetKeyword + Parenthesized(BindingSpec.ZeroOrMore()) + Body
            | LetKeyword + Variable + Parenthesized(BindingSpec.ZeroOrMore()) + Body
            | LetStarKeyword + Parenthesized(BindingSpec.ZeroOrMore()) + Body
            | LetRecKeyword + Parenthesized(BindingSpec.ZeroOrMore()) + Body
            | BeginKeyword + Sequence
            | DoKeyword + Parenthesized(IterationSpec.ZeroOrMore()) + Parenthesized(Test + Sequence) + Command.ZeroOrMore()
            | DelayKeyword + Expression);

        public Rule CondClause => Node(Parenthesized(Test + Sequence) |
                                       Parenthesized(Test) |
                                       Parenthesized(Test + Sym("=>") + Recipient));

        public Rule Recipient => Node(Expression);
        public Rule CaseClause => Node(Parenthesized(Parenthesized(Datum.ZeroOrMore()) + Sequence));
        public Rule BindingSpec => Node(Parenthesized(Variable + Expression));
        public Rule IterationSpec => Node(Parenthesized(Variable + Init + Step) | Parenthesized((Variable + Init)));
        public Rule Init => Node(Expression);
        public Rule Step => Node(Expression);

        public Rule Program => Node((Command | Definition).ZeroOrMore());

        public Rule Definition 
            => Node(Recursive(nameof(InnerDefinition)));

        public Rule InnerDefinition => Node(
            Parenthesized(Keyword("define") + Variable + Expression)
            | Parenthesized(Keyword("define") + Parenthesized(Variable + DefFormals) + Body)
            | Parenthesized(Keyword("begin") + Recursive(nameof(Definition)).ZeroOrMore()));

        public Rule DefFormals => Node(Variable.ZeroOrMore() | Variable.OneOrMore() + Sym(".") + Variable);
    }
}