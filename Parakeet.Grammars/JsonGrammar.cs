namespace Parakeet.Grammars
{
    public class JsonGrammar : CommonGrammar
    {
        public new static CSharpGrammar Instance = new CSharpGrammar();

        public Rule DoubleQuoted(Rule r) => '\"' + OnError(AdvanceToEnd) + r + '\"';

        public static readonly char CarriageReturn = '\r';
        public static readonly char LineFeed = '\n';
        public static readonly char Space = ' ';
        public static readonly char Tab = '\t';

        public override Rule WS => Named(CharSet(CarriageReturn, LineFeed, Space, Tab).ZeroOrMore());
        public Rule Exponent => Named(CharSet('e', 'E') + Sign.Optional() + Digits);
        public Rule Fraction => Named("." + Digits);
        public new Rule Integer => Named(Optional('-') + ("0" | Digits));
        public Rule EscapedChar => Named('\\' + (CharSet("\"\\/bfnrt") | 'u' + (HexDigit + HexDigit + HexDigit + HexDigit)));
        public Rule StringChar => Named(EscapedChar | AnyChar.Except('\"'));
        public Rule Number => Node(Integer + Fraction.Optional() + Exponent.Optional());
        public Rule String => Node(DoubleQuoted(StringChar.ZeroOrMore()));
        public Rule Constant => Node(Strings("true", "false", "null") + IdentifierChar.NotAt());
        public Rule Elements => Named(Element + WS + ("," + WS + Element + WS).ZeroOrMore());
        public Rule Array => Node("[" + OnError(AdvanceToEnd) + WS + Elements.Optional() + WS + "]");
        public Rule Member => Node(String + OnError(AdvanceToEnd) + WS + ":" + WS + Element);
        public Rule Members => Named(Member + OnError(AdvanceToEnd) + WS + ("," + WS + Member + WS).ZeroOrMore());
        public Rule Object => Node("{" + OnError(AdvanceToEnd) + WS + Members.Optional() + "}");
        public Rule Value => Named(Object | Array | String | Number | Constant);
        public Rule Element => Recursive(nameof(Value));
        public Rule Json => Node(WS + Element + WS);
    }
}
