namespace Ara3D.Parakeet.Grammars
{
    /// <summary>
    /// A Parakeet grammar for a variant of the Mustache and CTemplate template languages
    /// This grammar works with any template delimiters defaulting to "{{" and "}}"
    /// - http://mustache.github.io/mustache.5.html
    /// - https://github.com/olafvdspek/ctemplate
    /// According to the mustache documentation:
    /// - `#` indicates a start section
    /// - `/` indicates an end section
    /// - `^` indicates an inverted section
    /// - `!` indicates a comment
    /// - `&` or `{` indicate an unescaped variable 
    /// - `>` indicates a *partial* which is effectively a file include with run-time expansion.
    /// </summary>
    public class MustacheGrammar : BaseCommonGrammar
    {
        public static MustacheGrammar Instance = new MustacheGrammar();
        public override Rule StartRule => this.Document;

        public Rule Start => Named("{{");
        public Rule End => Named("}}");

        public Rule Key => Node(AnyCharUntilAt(End));

        public Rule Tag(Rule tagType) => Start + tagType + AbortOnFail + Space.ZeroOrMore() + Key + End;
        public Rule RestOfLine => Node(SpaceOrTab.ZeroOrMore() + NewLine | EndOfInput);
        public Rule StartSection => Node(Tag('#') + RestOfLine);
        public Rule EndSection => Node(Tag('/'));
        public Rule StartInvertedSection => Node(Tag('^') + RestOfLine);
        public Rule Comment => Node(Tag('!'));
        public Rule UnescapedVariable => Node((Tag('{') + Optional('}')) | Tag('&'));
        public Rule Partial => Node(Tag('>'));
        public Rule Variable => Node(Tag(!"#/^!{&<".ToCharSetRule()));
        public Rule PlainText => Node(AnyChar.RepeatUntilAt(Start | EndOfInput));
        public Rule SectionContent => Recursive(nameof(Content));
        public Rule InvertedSection => Node(StartInvertedSection + AbortOnFail + SectionContent + EndSection);
        public Rule Section => Node(StartSection + AbortOnFail + SectionContent + EndSection);
        public Rule Block => Node(InvertedSection | Section | Comment | Partial | Variable | UnescapedVariable | PlainText);
        public Rule Content => Node(Block.ZeroOrMore());
        public Rule Document => Node(Content);
    }
}
