namespace Parakeet.Demos.WIP
{
    public class CGrammar : CommonGrammar
    {
        // https://learn.microsoft.com/en-us/cpp/c-language/lexical-grammar?view=msvc-170
        // https://www.lysator.liu.se/c/ANSI-C-grammar-y.html
    }

    public class XMLGrammar : CommonGrammar
    {
        // https://www.w3schools.com/xml/xml_syntax.asp
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

    // https://en.wikipedia.org/wiki/S-expression
    // https://en.wikipedia.org/wiki/Lisp_(programming_language)
    // https://en.wikipedia.org/wiki/Scheme_(programming_language)
    // https://gist.github.com/Idorobots/3378676

    public class CsvGrammar : CommonGrammar
    {
        /*
        public Rule TextData => Node(AnyCharRule.);
        public Rule Csv = Node()
        m.notChar('\n\r"' + delimiter);    
        this.quoted     = m.doubleQuoted(m.notChar('"').or('""').zeroOrMore);
        this.field      = this.textdata.or(this.quoted).zeroOrMore.ast;
        this.record     = this.field.delimited(delimiter).ast;
        this.file       = this.record.delimited(m.newLine).ast;        
         */
    }

    public class MarkdownGrammar : CommonGrammar
    {
        // https://github.com/jgm/lunamark/blob/master/lunamark/reader/markdown.lua
        // https://github.com/jgm/peg-markdown/blob/master/markdown_parser.leg
        // https://commonmark.org/help/
        // https://www.markdownguide.org/basic-syntax/
        // https://daringfireball.net/projects/markdown/syntax

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
        public Rule Bold2 => "__" + Text + "__";
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
}