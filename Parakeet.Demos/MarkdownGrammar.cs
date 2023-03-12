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
}
