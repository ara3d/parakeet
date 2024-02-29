namespace Ara3D.Parakeet.Grammars
{
    public class MarkdownGrammar : BaseCommonGrammar
    {
        // https://github.com/jgm/lunamark/blob/master/lunamark/reader/markdown.lua
        // https://github.com/jgm/peg-markdown/blob/master/markdown_parser.leg
        // https://commonmark.org/help/
        // https://www.markdownguide.org/basic-syntax/
        // https://daringfireball.net/projects/markdown/syntax
        public static readonly MarkdownGrammar Instance = new MarkdownGrammar();
        public override Rule StartRule => Document;
        public override Rule WS => SpaceOrTab;
        public Rule SpaceOrTab => " \t".ToCharSetRule();
        public Rule BlankLine => SpaceOrTab.ZeroOrMore();
        public Rule HeadingOperator => Node(new CountedRule('#', 1, 6));
        public Rule Heading => Node(HeadingOperator + Text);
        public Rule WSToEndOfLine => Node(WS.ZeroOrMore() + NewLine);
        public Rule HorizontalLine => Node((ThreeOrMore('*') | ThreeOrMore('-') | ThreeOrMore('_')) + WSToEndOfLine);
        public Rule Heading1Underlined => Node(TwoOrMore('=') + WSToEndOfLine);
        public Rule Heading2Underlined => Node(TwoOrMore('-') + WSToEndOfLine);
        public Rule Comment => Node(XmlStyleComment);
        public Rule UrlChar => Named(IdentifierChar | "/:?+%-#".ToCharSetRule());
        public Rule Url => Node(UrlChar.ZeroOrMore());
        public Rule UrlTitle => Node(DoubleQuoteBasicString);
        public Rule Link => "[" + Text + "]" + WS + "(" + Url + WS + UrlTitle + ")";
        public Rule Bold1 => Named("**" + Text + "**");
        public Rule Bold2 => Named("__" + Text + "__");
        public Rule Bold => Node(Bold1 | Bold2);
        public Rule Italic1 => Named("*" + Text + "*");
        public Rule Italic2 => Named("_" + Text + "_");
        public Rule Italic => Node(Italic1 | Italic2);
        public Rule Code => "`" + Not('`') + Text + "`";
        public Rule LangIdentifier => Node(AnyCharUntilNextLine);
        public Rule CodeBlock => "```" + LangIdentifier + AnyCharUntilPast("```");
        public new Rule EscapedChar => Node("\\" + "\\`*-{}[]<>()#+-.!|".ToCharSetRule());
        public Rule BlockQuotedLine => Node("> " + Text);
        public Rule Break => Node("<br" + Optional("/") + ">");
        public Rule OrderedListItem => Node(Digit.ZeroOrMore() + "." + WS + Text);
        public Rule UnorderedListItem => Node("+-*".ToCharSetRule() + WS + Text);
        public Rule AltText => Node(Bracketed(AnyCharExcept(']').ZeroOrMore()));
        public Rule UrlLink => Node("<" + Url + ">");
        public Rule EmailChar => Named(AnyCharExcept(">@\n\r "));
        public Rule Email => Node(EmailChar.ZeroOrMore() + '@' + EmailChar.ZeroOrMore());
        public Rule EmailLink => Node("<" + Email + ">");
        // TODO: support HTML but that is another grammar. 
        public Rule Img => Node("!" + AltText + Parenthesized(Url + WS + UrlTitle.Optional()));
        public Rule FormattedText => Node(
            Link | 
            Bold | 
            Italic | 
            Code | 
            EmailLink | 
            UrlLink | 
            Img | 
            Break | 
            Comment | 
            EscapedChar);

        public Rule PlainText => Node(AnyCharExcept(NewLine | FormattedText).ZeroOrMore());
        public Rule InnerText => Node(FormattedText | PlainText);
        public Rule Text => Recursive(nameof(InnerText));

        public Rule Indent => Node(Space.ZeroOrMore());
        public Rule Line => Node(Indent + (
            CodeBlock | 
            OrderedListItem |
            UnorderedListItem |
            Heading |
            Heading1Underlined |
            Heading2Underlined |
            BlockQuotedLine |
            HorizontalLine |
            BlankLine | 
            Text));

        public Rule Document => Node(Line.ZeroOrMore());
    }
}