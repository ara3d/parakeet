namespace Parakeet.Demos
{
    public class JsonGrammar : CommonGrammar
    {
        public Rule DoubleQuoted(Rule r) => '\"' + r + '\"';

        public static readonly char CarriageReturn = '\r';
        public static readonly char LineFeed = '\n';
        public static readonly char Space = ' ';
        public static readonly char Tab = '\t';

        public Rule WS => Token(CharSet(CarriageReturn, LineFeed, Space, Tab).ZeroOrMore());
        public Rule Exponent => Token(CharSet('e', 'E') + Sign.Optional() + Digits);
        public Rule Fraction => Token("." + Digits);
        public Rule Integer => Token(CharSet('-').Optional() + ("0" | Digits));
        public Rule EscapedChar => Token('\\' + (CharSet("\"\\/bfnrt") | 'u' + (HexDigit + HexDigit + HexDigit + HexDigit)));
        public Rule StringChar => Token(EscapedChar | AnyChar.Except('\"'));
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
        public Rule Element => Node(Recursive(() => Value));
        public Rule Json => Node(WS + Element + WS);
    }
}
