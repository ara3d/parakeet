using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Runtime.InteropServices.ComTypes;

namespace Parakeet.Grammars.WIP
{
    public class CGrammar : CommonGrammar
    {
      
    }
	

    public class HTMLGrammar : CommonGrammar
    {
    }

    public class PythonGrammar : CommonGrammar
    {
        // https://docs.python.org/3/reference/grammar.html
    }

    public class ExpressionTreeGrammar : CommonGrammar
    {

    }

    public class GlslGrammar : CommonGrammar
    {
        // https://github.com/nnesse/glsl-parser
        // https://github.com/nnesse/glsl-parser/blob/master/glsl.y
        // https://registry.khronos.org/OpenGL/specs/es/2.0/GLSL_ES_Specification_1.00.pdf
        // https://registry.khronos.org/OpenGL/specs/es/3.0/GLSL_ES_Specification_3.00.pdf
    }

    public class JavaScriptGrammar : CommonGrammar
    {

    }

    public class LogoGrammar : CommonGrammar
    {
        // https://ia800907.us.archive.org/5/items/Apple_Logo_II_Reference_Manual/Apple_Logo_II_Reference_Manual.pdf
        // https://dspace.mit.edu/bitstream/handle/1721.1/6226/AIM-313.pdf
    }

    public class PrologGrammar : CommonGrammar
    {
        // https://en.wikipedia.org/wiki/Prolog
    }

    // https://en.wikipedia.org/wiki/Lambda_calculus
    public class LambdaGrammar : CommonGrammar
    {
        public Rule Variable => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());
        public Rule Parameter => Node("\\" + Variable);
        public Rule Expression => Node(Variable | Abstraction | Application);
        public Rule Abstraction => Node("(" + Parameter.Then(".").ZeroOrMore() + Expression + ")");
        public Rule Application => Node("(" + Expression + Expression + ")");
    }

    public class NasmGrammar : CommonGrammar
    {
        // https://www.cs.uaf.edu/2017/fall/cs301/reference/x86_64.html
        // https://www.nasm.us/index.php
        // https://en.wikipedia.org/wiki/Netwide_Assembler
        // https://en.wikipedia.org/wiki/Executable_and_Linkable_Format
    }


    // https://en.wikipedia.org/wiki/Lisp_(programming_language)
    // https://en.wikipedia.org/wiki/Scheme_(programming_language)
    // https://gist.github.com/Idorobots/3378676


    // https://github.com/antlr/grammars-v4/blob/master/csv/CSV.g4

    // https://www.w3schools.com/cssref/css_selectors.php
    // https://www.w3.org/TR/CSS21/grammar.html
    // https://www.w3schools.com/css/css_syntax.asp
    // https://developer.mozilla.org/en-US/docs/Web/CSS/Syntax
    public class CssGrammar : CommonGrammar
    {
        public Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());
        public Rule Comment => "/*" + RepeatUntilPast(AnyChar, "*/");
        public Rule Property;
        public Rule Value;
        public Rule Declaration => Node(Property + ":" + Value + ";");
        public Rule DeclarationBlock => Node("{" + Recovery + Declaration.ZeroOrMore() + "}");
        public Rule IdSelector => Node("#" + Identifier);
        public Rule ClassSelector => Node("." + Identifier);
        public Rule ElementClassSelector => Node(Identifier + ClassSelector);

        // https://www.w3schools.com/css/css_combinators.asp
        // https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_selectors
        public Rule Combinator => Node("+" | "~" | ">" | "|");
        
        // https://www.w3schools.com/css/css_pseudo_elements.asp
        public Rule PseudoElement => 
        public Rule PseudoClass => Node(":" + Recovery + Identifier);
        public Rule QuotedValue; // => Node(StringLiteral);
        public Rule AttributeOperator => Node("*=", "~=", "^=", "$=", "|=", "=");
        public Rule AttributeSelector => Node("[" + Recovery + Identifier + AttributeOperator + QuotedValue + "]");
        public Rule UniversalSelector => Node("*");
        public Rule Selector => IdentifierChar;
        public Rule SelectorList => Node(Selector + ("," + Recovery + Selector).ZeroOrMore());
        public Rule RuleSet => SelectorList + DeclarationBlock;
        public Rule AtRule => Node("@" + Identifier.ZeroOrMore());
        public Rule Statement => Node((RuleSet | AtRule).ZeroOrMore());
    }

    // https://en.wikipedia.org/wiki/Joy_(programming_language)
    public class JoyGrammar : CommonGrammar
    {
        public Rule Operator => Node(CharSet("+=-<>&|*/^%@$~!").ZeroOrMore() | Identifier);
        public Rule Literal => Node(Float | Integer | String);
        public Rule RecExpr => Recursive(nameof(Expr));
        public Rule Quotation => Node(Symbol("[") + RecExpr.ZeroOrMore() + Symbol("]"));
		public Rule Expr => Node(Quotation | Literal | Operator | Identifier);
        public Rule Def => Node(Symbol("DEFINE") + Operator + "==" + Expr.ZeroOrMore() + ".");
    }

    // https://github.com/antlr/grammars-v4/blob/master/xml/XMLParser.g4
    //  https://www.w3schools.com/xml/xml_syntax.asp
    public class XmlGrammar : CommonGrammar
    {
        public Rule NL => Optional('\r') + '\n';
        public override Rule WS => Named(OneOrMore(NL | ' ' | '\t'));

        public Rule DTD => Node("<!")
        public Rule CDATA => Node("<![CDATA['" + UntilPast("]]>"));
        public Rule Prolog => "<?xml" + WS + Recursive(nameof(AttrList)) + "?>";
        public Rule Comment => "<!--" + Recovery + RepeatUntilPast(AnyChar, "-->");
        public Rule Document => Prolog.Optional() + Element;
        public Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());
        // https://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references
        public Rule HexEntityValue => Node("x" + HexDigit.ZeroOrMore());
        public Rule NumericEntityValue => Node(Digit.ZeroOrMore());
        public Rule Entity => Node("&" + Recovery + (Identifier | HexEntityValue | NumericEntityValue) + ";");

        public Rule AttrName => Node(Identifier);
		public Rule AttrValue => Node((Identifier));
        public Rule Attr => Node(AttrName + Symbol("=") + AttrValue);
        public Rule AttrList => Node(Attr.ZeroOrMore());
        public Rule NSIdent => Node(Identifier + Optional(":" + Identifier));
        public Rule StartTag => Node(Symbol("<") + Identifier + AttrList + Symbol(">"));
        public Rule EmptyElement => Node(Symbol("<") + Identifier + AttrList + Symbol("/>"));
        public Rule EndTag => Node(Symbol("</") + Identifier + Symbol(">"));
        public Rule NonEmptyElement => Node(StartTag + Content + EndTag);
        public Rule Element => Node(EmptyElement | StartTag);
    }

    public class MarkdownGrammar : CommonGrammar
    {
        // https://github.com/jgm/lunamark/blob/master/lunamark/reader/markdown.lua
        // https://github.com/jgm/peg-markdown/blob/master/markdown_parser.leg
        // https://commonmark.org/help/
        // https://www.markdownguide.org/basic-syntax/
        // https://daringfireball.net/projects/markdown/syntax

        public Rule SpaceOrTab => " \t\r".ToCharSetRule();
        public Rule BlankLine => SpaceOrTab.ZeroOrMore();
        public Rule LineText;
        public Rule AnyCharUntilEor;
        public Rule AtxHeader => "#" + AnyChar;
        public Rule SetExtHeader;
        public Rule Paragraph;

        /*
        public Rule ContentLine => ;
        public Rule BlockQuotedParagraph => OneOrMore(">") + Paragraph;
        public Rule Paragraph => ContentLine.OneOrMore() + BlankLine.AtRule() | EndOfInput;
        public Rule BlankLine => WSToEndOfLine();
        public Rule Text;
        public Rule WS => ZeroOrMoreRule(Spaces);
        public Rule WSToEndOfLine => ZeroOrMoreRule(" \t\r".ToCharSetRule()) + NewLine; 
        public Rule LineBegin => After(NewLine);
        public Rule Line => LineBegin;
        public Rule Heading => LineBegin + OneOrMore('#') + OptionalRule(' ') + HeadingContent + NewLine;
        public Rule NonHeadingText => LineBegin + NotAtRule("-") + Text;
        public Rule Heading1Underlined => NonHeadingText + NewLine + "==" + ZeroOrMoreRule('=') + UntilNextLine;
        public Rule Heading2Underlined => NonHeadingText + NewLine + "--" + ZeroOrMoreRule('-') + UntilNextLine;
        public Rule Bold1 => "**" + Text + "**";
        public Rule Bold2 => "__" + Text + "        __";
        public Rule Italic1 => "*" + Text + "*";
        public Rule Italic2 => "_" + Text + "_";
        public Rule LineBreak => "  " + NewLine;
        public Rule Mixed =>
            ("***" + Text + "***")
            | ("___" + Text + "___")
            | ("__*" + Text + "*__")
            | ("_**" + Text + "**_")
            | ("*__" + Text + "__*")
            | ("**_" + Text + "_**");
        public Rule InlineCode => "`" + Text + "`";
        public Rule LanguageIdentifier => Node(Identifier);
        public Rule CodeBlock => "```" + LanguageIdentifier + UntilNextLine + UntilPast("```");
        public Rule BlankLine => LineBegin + WS + NewLine;
        public Rule BlockquotedLine => ">" + Line;
        public Rule UnorderedListItemLine => CharSet("-*+") + Line;
        public Rule OrderedListItemLine => Digits + "." + Line;
        public Rule Inline => ;
        public Rule EscapedChar => '\\' + EscapableChar;
        public Rule EscapableChar => CharSet("\`*_{}[]<>()#+-.!");
        public Rule LinkedText => Text;
        public Rule URL => Text; // TODO:
        public Rule UrlTitle => '"' + AnyChar.Except('"').ZeroOrMoreRule() + '"';
        public Rule InlineUrl => "<" + URL + ">";
        public Rule ReferenceStyleLink => "[" + LinkedText + "]" + OptionalRule(' ') + "[" + ReferenceLink + "]";
        public Rule Image => "!" + Link;
        public Rule LinkedImage => ? 
        public Rule Link => "[" + LinkedText + "]" + WS + "(" + URL + WS + UrlTitle.OptionalRule() + ")";
        public Rule HorizontalLine => (LineBegin + ThreeOrMore("*") | ThreeOrMore("-") | ThreeOrMore("_") + WSToEndOfLine; 
    */
    }
    // https://github.com/cdiggins/cat-language
    public class CatGrammar : CommonGrammar
    {
        public Rule Definition { get; }
        public Rule Quotation { get; }
        public Rule Term { get; }
        public Rule Terms { get; }
        public Rule Program { get; }
        public Rule Extern { get; }
        public Rule TypeSignature { get; }
        public Rule TypeVar { get; }
        public Rule TypeConstant { get; }
        public Rule TypeFunc { get; }
        public Rule TypeInput { get; }
        public Rule TypeOutput { get; }
        public Rule TypeArray { get; }
        public new Rule Integer { get; }
        public new Rule Float { get; }
        public Rule String { get; }
        public Rule Boolean { get; }
        public Rule Number { get; }
    }

    public class MathGrammar
    {
        public Rule Number { get; }
        public Rule DecimalNumber { get; }
        public Rule WholeNumber { get; }
        public Rule Operator { get; }
        public Rule Function { get; }
    }

    /*   // https://learn.microsoft.com/en-us/cpp/c-language/lexical-grammar?view=msvc-170
        // https://www.lysator.liu.se/c/ANSI-C-grammar-y.html
    public class AnsiCGrammar
    {

        public Rule IDENTIFIER;
        public Rule CONSTANT;
        public Rule STRING_LITERAL;

        public Rule rec_expression => new RecursiveRule(() => expression);
		public Rule primary_expression => 
	        IDENTIFIER | CONSTANT | STRING_LITERAL | ('(' + rec_expression + ')');

		public Rule postfix_expression => 
	        primary_expression
	        | (postfix_expression + "[" + rec_expression + ']')
	        | (postfix_expression + "(" + ")")
	        | (postfix_expression + '(' + argument_expression_list + ')')
	        | (postfix_expression + '.' + IDENTIFIER)
	        | (postfix_expression + "->"+ IDENTIFIER)
	        | (postfix_expression + "++")
	        | (postfix_expression + "--")
            ;

        public Rule argument_expression_list
	        => assignment_expression
	        | argument_expression_list + ',' + assignment_expression;
		

        public Rule unary_expression
	=> postfix_expression
	| ("++" + unary_expression)
	| ("--" + unary_expression)
	| (unary_operator + cast_expression)
	| ("sizeof" + unary_expression)
	| ("sizeof" + '(' + type_name + ')');

unary_operator
	: '&'
	| '*'
	| '+'
	| '-'
	| '~'
	| '!'
	;

cast_expression
	: unary_expression
	| '(' type_name ')' cast_expression
	;

multiplicative_expression
	: cast_expression
	| multiplicative_expression '*' cast_expression
	| multiplicative_expression '/' cast_expression
	| multiplicative_expression '%' cast_expression
	;

additive_expression
	: multiplicative_expression
	| additive_expression '+' multiplicative_expression
	| additive_expression '-' multiplicative_expression
	;

shift_expression
	: additive_expression
	| shift_expression LEFT_OP additive_expression
	| shift_expression RIGHT_OP additive_expression
	;

relational_expression
	: shift_expression
	| relational_expression '<' shift_expression
	| relational_expression '>' shift_expression
	| relational_expression LE_OP shift_expression
	| relational_expression GE_OP shift_expression
	;

equality_expression
	: relational_expression
	| equality_expression EQ_OP relational_expression
	| equality_expression NE_OP relational_expression
	;

and_expression
	: equality_expression
	| and_expression '&' equality_expression
	;

exclusive_or_expression
	: and_expression
	| exclusive_or_expression '^' and_expression
	;

inclusive_or_expression
	: exclusive_or_expression
	| inclusive_or_expression '|' exclusive_or_expression
	;

logical_and_expression
	: inclusive_or_expression
	| logical_and_expression AND_OP inclusive_or_expression
	;

logical_or_expression
	: logical_and_expression
	| logical_or_expression OR_OP logical_and_expression
	;

conditional_expression
	: logical_or_expression
	| logical_or_expression '?' expression ':' conditional_expression
	;

assignment_expression
	: conditional_expression
	| unary_expression assignment_operator assignment_expression
	;

assignment_operator
	: '='
	| MUL_ASSIGN
	| DIV_ASSIGN
	| MOD_ASSIGN
	| ADD_ASSIGN
	| SUB_ASSIGN
	| LEFT_ASSIGN
	| RIGHT_ASSIGN
	| AND_ASSIGN
	| XOR_ASSIGN
	| OR_ASSIGN
	;

expression
	: assignment_expression
	| expression ',' assignment_expression
	;

constant_expression
	: conditional_expression
	;

declaration
	: declaration_specifiers ';'
	| declaration_specifiers init_declarator_list ';'
	;

        declaration_specifiers
	: storage_class_specifier
	| storage_class_specifier declaration_specifiers
	| type_specifier
	| type_specifier declaration_specifiers
	| type_qualifier
	| type_qualifier declaration_specifiers
    ;

        init_declarator_list
	: init_declarator
	| init_declarator_list ',' init_declarator
	;

init_declarator
	: declarator
	| declarator '=' initializer
	;

storage_class_specifier
	: TYPEDEF
	| EXTERN
	| STATIC
	| AUTO
	| REGISTER
	;

type_specifier
	: VOID
	| CHAR
	| SHORT
	| INT
	| LONG
	| FLOAT
	| DOUBLE
	| SIGNED
	| UNSIGNED
	| struct_or_union_specifier
	| enum_specifier
	| TYPE_NAME
	;

struct_or_union_specifier
	: struct_or_union IDENTIFIER '{' struct_declaration_list '}'
	| struct_or_union '{' struct_declaration_list '}'
	| struct_or_union IDENTIFIER
    ;

        struct_or_union
	: STRUCT
	| UNION
	;

struct_declaration_list
	: struct_declaration
	| struct_declaration_list struct_declaration
    ;

        struct_declaration
	: specifier_qualifier_list struct_declarator_list ';'
	;

        specifier_qualifier_list
	: type_specifier specifier_qualifier_list
	| type_specifier
	| type_qualifier specifier_qualifier_list
	| type_qualifier
	;

struct_declarator_list
	: struct_declarator
	| struct_declarator_list ',' struct_declarator
	;

struct_declarator
	: declarator
	| ':' constant_expression
	| declarator ':' constant_expression
	;

enum_specifier
	: ENUM '{' enumerator_list '}'
	| ENUM IDENTIFIER '{' enumerator_list '}'
	| ENUM IDENTIFIER
    ;

        enumerator_list
	: enumerator
	| enumerator_list ',' enumerator
	;

enumerator
	: IDENTIFIER
	| IDENTIFIER '=' constant_expression
	;

type_qualifier
	: CONST
	| VOLATILE
	;

declarator
	: pointer direct_declarator
	| direct_declarator
	;

direct_declarator
	: IDENTIFIER
	| '(' declarator ')'
	| direct_declarator '[' constant_expression ']'
	| direct_declarator '[' ']'
	| direct_declarator '(' parameter_type_list ')'
	| direct_declarator '(' identifier_list ')'
	| direct_declarator '(' ')'
	;

pointer
	: '*'
	| '*' type_qualifier_list
	| '*' pointer
	| '*' type_qualifier_list pointer
    ;

        type_qualifier_list
	: type_qualifier
	| type_qualifier_list type_qualifier
    ;


        parameter_type_list
	: parameter_list
	| parameter_list ',' ELLIPSIS
	;

parameter_list
	: parameter_declaration
	| parameter_list ',' parameter_declaration
	;

parameter_declaration
	: declaration_specifiers declarator
	| declaration_specifiers abstract_declarator
	| declaration_specifiers
	;

identifier_list
	: IDENTIFIER
	| identifier_list ',' IDENTIFIER
	;

type_name
	: specifier_qualifier_list
	| specifier_qualifier_list abstract_declarator
    ;

        abstract_declarator
	: pointer
	| direct_abstract_declarator
	| pointer direct_abstract_declarator
    ;

        direct_abstract_declarator
	: '(' abstract_declarator ')'
	| '[' ']'
	| '[' constant_expression ']'
	| direct_abstract_declarator '[' ']'
	| direct_abstract_declarator '[' constant_expression ']'
	| '(' ')'
	| '(' parameter_type_list ')'
	| direct_abstract_declarator '(' ')'
	| direct_abstract_declarator '(' parameter_type_list ')'
	;

initializer
	: assignment_expression
	| '{' initializer_list '}'
	| '{' initializer_list ',' '}'
	;

initializer_list
	: initializer
	| initializer_list ',' initializer
	;

statement
	: labeled_statement
	| compound_statement
	| expression_statement
	| selection_statement
	| iteration_statement
	| jump_statement
	;

labeled_statement
	: IDENTIFIER ':' statement
	| CASE constant_expression ':' statement
	| DEFAULT ':' statement
	;

compound_statement
	: '{' '}'
	| '{' statement_list '}'
	| '{' declaration_list '}'
	| '{' declaration_list statement_list '}'
	;

        declaration_list
	: declaration
	| declaration_list declaration
    ;

        statement_list
	: statement
	| statement_list statement
    ;

        expression_statement
	: ';'
	| expression ';'
	;

selection_statement
	: IF '(' expression ')' statement
	| IF '(' expression ')' statement ELSE statement
	| SWITCH '(' expression ')' statement
	;

iteration_statement
	: WHILE '(' expression ')' statement
	| DO statement WHILE '(' expression ')' ';'
	| FOR '(' expression_statement expression_statement ')' statement
	| FOR '(' expression_statement expression_statement expression ')' statement
	;

jump_statement
	: GOTO IDENTIFIER ';'
	| CONTINUE ';'
	| BREAK ';'
	| RETURN ';'
	| RETURN expression ';'
	;

        translation_unit
	: external_declaration
	| translation_unit external_declaration
    ;

        external_declaration
	: function_definition
	| declaration
	;

function_definition
	: declaration_specifiers declarator declaration_list compound_statement
	| declaration_specifiers declarator compound_statement
	| declarator declaration_list compound_statement
	| declarator compound_statement
    ;

    }
	*/
}