namespace Parakeet.Grammars
{
    // https://github.com/antlr/grammars-v4/blob/master/xml/XMLParser.g4
    // https://www.w3schools.com/xml/xml_syntax.asp
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
        public Rule AttrValue => Node(Identifier);
        public Rule Attr => Node(AttrName + Symbol("=") + AttrValue);
        public Rule AttrList => Node(Attr.ZeroOrMore());
        public Rule NSIdent => Node(Identifier + Optional(":" + Identifier));
        public Rule StartTag => Node(Symbol("<") + Identifier + AttrList + Symbol(">"));
        public Rule EmptyElement => Node(Symbol("<") + Identifier + AttrList + Symbol("/>"));
        public Rule EndTag => Node(Symbol("</") + Identifier + Symbol(">"));
        public Rule NonEmptyElement => Node(StartTag + Content + EndTag);
        public Rule Element => Node(EmptyElement | StartTag);
    }
}