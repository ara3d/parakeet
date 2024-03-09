namespace Ara3D.Parakeet.Grammars
{
    // Mostly follows this guideline:
    // https://www.markdownguide.org/basic-syntax/

    // See also:
    // https://github.com/jgm/lunamark/blob/master/lunamark/reader/markdown.lua
    // https://github.com/jgm/peg-markdown/blob/master/markdown_parser.leg
    // https://commonmark.org/help/
    // https://daringfireball.net/projects/markdown/syntax

    /// <summary>
    /// This is a grammar for inline elements within parsed Markdown blocks. 
    /// Markdown parsing is easier when broken down into two passes.    
    /// </summary>
    public class MarkdownInlineGrammar : BaseCommonGrammar
    {
        public static readonly MarkdownInlineGrammar Instance = new MarkdownInlineGrammar();
        public override Rule StartRule => Content;
        public override Rule WS => Named(SpaceOrTab.ZeroOrMore());

        public Rule EscapableChar => "\\`*-{}[]<>()#+-.!|".ToCharSetRule();
        public new Rule EscapedChar => Node("\\" + EscapableChar);
        public Rule HtmlTag => Node("<" + AbortOnFail + AnyCharUntilPast(">"));
        public Rule AltText => Node(Bracketed(AnyCharExcept(']').ZeroOrMore()));
        public Rule UrlLink => Node(("<" + Url + ">") | PlainTextUrl);
        public Rule EmailChar => Named(AnyCharExcept(">@\n\r "));
        public Rule Email => Node(EmailChar.ZeroOrMore() + '@' + EmailChar.ZeroOrMore());
        public Rule EmailLink => Node("<" + Email + ">");

        // TODO: support HTML but that is another grammar. 
        public Rule Img => Node("!" + AltText + Parenthesized(Url + WS + UrlTitle.Optional()));
        public Rule FormatChar => "*[<`~*_!\\".ToCharSetRule();
        public Rule PlainText => Node(AnyCharExcept(FormatChar).OneOrMore());
        public Rule Text => Recursive(nameof(InnerText));

        public Rule UrlChar => Named(DigitOrLetter | "_:?+%-#.$~!*'();:@&=+$,/?#[]".ToCharSetRule());
        public Rule Url => Node(UrlChar.ZeroOrMore());
        public Rule PlainTextUrl => Node("http://" + UrlChar.ZeroOrMore());
        public Rule UrlTitle => Node(DoubleQuoteBasicString);
        public Rule Link => Node("[" + Text + "]" + WS + "(" + Url + WS + UrlTitle + ")");

        public Rule FormattedText(string delimiter) 
            => Text.Except(delimiter).ZeroOrMore();

        public Rule FormattedSpan(string delimiter) 
            => delimiter + FormattedText(delimiter) + delimiter;

        public Rule Italic => Node(FormattedSpan("*") | FormattedSpan("_"));
        public Rule Bold => Node(FormattedSpan("**") | FormattedSpan("__"));
        public Rule Strikethrough => Node(FormattedSpan("~~"));
        public Rule BoldAndItalic => Node(FormattedSpan("***") | FormattedSpan("___"));
        public Rule Code => Node(FormattedSpan("`"));

        public Rule InnerText => Node(
            BoldAndItalic 
            | Strikethrough 
            | Bold
            | Italic 
            | Code
            | Link
            | Img
            | EmailLink
            | UrlLink
            | HtmlTag
            | EscapedChar
            | PlainText
            | AnyChar);

        public Rule Content => Node(InnerText.ZeroOrMore());
    }

    /// <summary>
    /// This is used for parsing the block structure of a Markdown document.
    /// Note: XML comments must be at the beginning of a line. 
    /// </summary>
    public class MarkdownBlockGrammar : BaseCommonGrammar
    {
        public static readonly MarkdownBlockGrammar Instance = new MarkdownBlockGrammar();

        public override Rule StartRule => Document;
        public override Rule WS => Named(SpaceOrTab.ZeroOrMore());
        public Rule WSToEndOfLine => Named(WS + (NewLine | EndOfInput));

        public Rule Document => Node(Block.ZeroOrMore());
        public Rule BlankLine => Node(WS + NewLine);
        public Rule RestOfLine => Recursive(nameof(Line));

        public Rule CodeBlockDelimiter => Named("```");
        public Rule CodeBlockLang => Node(Identifier.Optional() + AbortOnFail + WSToEndOfLine);
        public Rule CodeBlockText => Node(AnyCharUntilAt(CodeBlockDelimiter));
        public Rule CodeBlock => Node(CodeBlockDelimiter + AbortOnFail + CodeBlockLang + CodeBlockText + CodeBlockDelimiter);

        public Rule H1Underline => Node(IndentsOrQuoteMarkers + WS + TwoOrMore('=') + AbortOnFail + WSToEndOfLine);
        public Rule H2Underline => Node(IndentsOrQuoteMarkers + WS + TwoOrMore('-') + AbortOnFail + WSToEndOfLine);
        public Rule HeadingOperator => Node(OneOrMore('#'));
        public Rule HeadingWithOperator => Node(HeadingOperator + TextLine);
        public Rule HeadingUnderlined => Node(TextLine + (H1Underline | H2Underline));
        public Rule Heading => Node(HeadingWithOperator | HeadingUnderlined);

        public Rule HorizontalLine => Node(
            (ThreeOrMore('*') 
             | ThreeOrMore('-') 
             | ThreeOrMore('_')
             | ThreeOrMore('=')) + AbortOnFail + WSToEndOfLine);

        public Rule OrderedListItem => Node(Indents + WS + Digit.OneOrMore() + "." + WS + AbortOnFail + TextLine);
        public Rule UnorderedListItem => Node(Indents + WS + "+-*".ToCharSetRule() + WS + AbortOnFail + TextLine);

        public Rule Indents => Node(Indent.ZeroOrMore());
        public Rule Indent => Node(Tab | "    ");
        public Rule QuoteMarker => Node(">");
        public Rule IndentsOrQuoteMarkers => Node((Indent | QuoteMarker).ZeroOrMore());

        public Rule BlockQuotedLine => Node(Indents + QuoteMarker + RestOfLine);
        public Rule TextLine => Node(AnyCharExcept(NewLine) + AnyCharUntilNextLine);

        public Rule Comment => Node(XmlStyleComment);

        public Rule Line => Node(
            Heading 
            | HorizontalLine
            | UnorderedListItem 
            | OrderedListItem 
            | BlockQuotedLine
            | BlankLine
            | TextLine); 

        public Rule Block => Node(
            CodeBlock 
             | Comment 
             | Line);
    }
}