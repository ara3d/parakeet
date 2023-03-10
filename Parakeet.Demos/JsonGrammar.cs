namespace Parakeet.Demos
{
    public class JsonGrammar : CommonGrammar
    {
        public Rule DoubleQuoted(Rule r) => '\"' + r + '\"';

        public static readonly char CarriageReturn = '\r';
        public static readonly char LineFeed = '\n';
        public static readonly char Space = ' ';
        public static readonly char Tab = '\t';

        public Rule WS => Named(CharSet(CarriageReturn, LineFeed, Space, Tab).ZeroOrMore());
        public Rule Exponent => Named(CharSet('e', 'E') + Sign.Optional() + Digits);
        public Rule Fraction => Named("." + Digits);
        public Rule Integer => Named(CharSet('-').Optional() + ("0" | Digits));
        public Rule EscapedChar => Named('\\' + (CharSet("\"\\/bfnrt") | 'u' + (HexDigit + HexDigit + HexDigit + HexDigit)));
        public Rule StringChar => Named(EscapedChar | AnyChar.Except('\"'));
        public Rule Number => Node(Integer + Fraction.Optional() + Exponent.Optional());
        public Rule String => Node('\"' + OnError(AdvanceToEnd) + StringChar.ZeroOrMore() + '\"');
        public Rule True => Node("true");
        public Rule False => Node("false");    
        public Rule Null => Node("null");
        public Rule Elements => Node(Element + WS + ("," + WS + Element + WS).ZeroOrMore());
        public Rule Array => Node("[" + OnError(AdvanceToEnd) + WS + Elements.Optional() + WS + "]");
        public Rule Member => Node(String + OnError(AdvanceToEnd) + WS + ":" + WS + Element);
        public Rule Members => Node(Member + OnError(AdvanceToEnd) + WS + ("," + WS + Member + WS).ZeroOrMore());
        public Rule Object => Node("{" + OnError(AdvanceToEnd) + WS + Members.Optional      () + "}");
        public Rule Value => Node(Object | Array | String | Number | True | False | Null);
        public Rule Element => Recursive(nameof(Value));
        public Rule Json => Node(WS + Element + WS);
    }
}
