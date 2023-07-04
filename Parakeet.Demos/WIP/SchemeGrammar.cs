namespace Parakeet.Demos.WIP
{
    // https://en.wikipedia.org/wiki/S-expression
    // https://en.wikipedia.org/wiki/Lisp_(programming_language)
    // https://en.wikipedia.org/wiki/Scheme_(programming_language)
    // https://gist.github.com/Idorobots/3378676
    // https://www.gnu.org/software/mit-scheme/documentation/stable/mit-scheme-ref/Strings.html#:~:text=Strings%20are%20written%20as%20sequences,start%20with%20a%20backslash%20(%20%5C%20)%3A
    // https://www.gnu.org/software/mit-scheme/documentation/stable/mit-scheme-ref/Characters.html
    // https://www.cs.cmu.edu/Groups/AI/html/r4rs/r4rs_9.html#SEC67

    public class SchemeGrammar : CommonGrammar
    {
        public Rule Token => Node(IdentifierChar | Boolean | Number | Character | String | "(" | ")" | "#(" | "'" | "`{" | "," | ".");
        public Rule Delimiter => Node(Whitespace | "(" | ")" | '"' | ";");
        public Rule Whitespace => SpaceChars;
        public Rule Comment => Node(";" + UntilNextLine);
        public Rule Atmosphere => Node(Whitespace | Comment);
        public Rule IntertokenSpace => Atmosphere.ZeroOrMore();
        public Rule Identifier => PeculiarIdentifier | Initial + Subsequent.ZeroOrMore();
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

        /*
          cs.cmu.edu/Groups/AI/html/r4rs/r4rs_9.html#SEC70

         * <expression> ==> <variable>
     | <literal>
     | <procedure call>
     | <lambda expression>
     | <conditional>
     | <assignment>
     | <derived expression>

<literal> ==> <quotation> | <self-evaluating>
<self-evaluating> ==> <boolean> | <number>
     | <character> | <string>
<quotation> ==> '<datum> | (quote <datum>)
<procedure call> ==> (<operator> <operand>*)
<operator> ==> <expression>
<operand> ==> <expression>

<lambda expression> ==> (lambda <formals> <body>)
<formals> ==> (<variable>*) | <variable>
     | (<variable>+ . <variable>)
<body> ==> <definition>* <sequence>
<sequence> ==> <command>* <expression>
<command> ==> <expression>

<conditional> ==> (if <test> <consequent> <alternate>)
<test> ==> <expression>
<consequent> ==> <expression>
<alternate> ==> <expression> | <empty>

<assignment> ==> (set! <variable> <expression>)

<derived expression> ==>
      (cond <cond clause>+)
     | (cond <cond clause>* (else <sequence>))
     | (case <expression>
        <case clause>+)
     | (case <expression>
        <case clause>*
        (else <sequence>))
     | (and <test>*)
     | (or <test>*)
     | (let (<binding spec>*) <body>)
     | (let <variable> (<binding spec>*) <body>)
     | (let* (<binding spec>*) <body>)
     | (letrec (<binding spec>*) <body>)
     | (begin <sequence>)
     | (do (<iteration spec>*)
          (<test> <sequence>)
        <command>*)
     | (delay <expression>)
     | <quasiquotation>

<cond clause> ==> (<test> <sequence>)
     | (<test>)
     | (<test> => <recipient>)
<recipient> ==> <expression>
<case clause> ==> ((<datum>*) <sequence>)

<binding spec> ==> (<variable> <expression>)
<iteration spec> ==> (<variable> <init> <step>)
     | (<variable> <init>)
<init> ==> <expression>
<step> ==> <expression>
         */
    }
}