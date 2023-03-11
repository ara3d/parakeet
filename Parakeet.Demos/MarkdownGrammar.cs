namespace Parakeet.Demos
{
    public class Transformer
    {
        // Strip trailing "#" from header. 
    }

    // Paragraph
    // Block 
    // 

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
        public Rule Paragraph => ContentLine.OneOrMore() + BlankLine.At() | EndOfInput;
        public Rule BlankLine => WSToEndOfLine();
        public Rule Text;
        public Rule WS => ZeroOrMore(Spaces);
        public Rule WSToEndOfLine => ZeroOrMore(" \t\r".ToCharSetRule()) + NewLine; 
        public Rule LineBegin => After(NewLine);
        public Rule Line => LineBegin;
        public Rule Heading => LineBegin + OneOrMore('#') + Optional(' ') + HeadingContent + NewLine;
        public Rule NonHeadingText => LineBegin + NotAt("-") + Text;
        public Rule Heading1Underlined => NonHeadingText + NewLine + "==" + ZeroOrMore('=') + UntilNextLine;
        public Rule Heading2Underlined => NonHeadingText + NewLine + "--" + ZeroOrMore('-') + UntilNextLine;
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
        public Rule UrlTitle => '"' + AnyChar.Except('"').ZeroOrMore() + '"';
        public Rule InlineUrl => "<" + URL + ">";
        public Rule ReferenceStyleLink => "[" + LinkedText + "]" + Optional(' ') + "[" + ReferenceLink + "]";
        public Rule Image => "!" + Link;
        public Rule LinkedImage => ? 
        public Rule Link => "[" + LinkedText + "]" + WS + "(" + URL + WS + UrlTitle.Optional() + ")";
        public Rule HorizontalLine => (LineBegin + ThreeOrMore("*") | ThreeOrMore("-") | ThreeOrMore("_") + WSToEndOfLine; 
    */
    }
}
