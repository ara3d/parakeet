namespace Ara3D.Parakeet.Grammars
{
    public class JsonGrammar : BaseCommonGrammar
    {
        public static readonly JsonGrammar Instance = new JsonGrammar();
        public override Rule StartRule => Json;

        // TODO: maybe this rule can be moved to the common grammar 
        public Rule DoubleQuoted(Rule r) => '\"' + OnFail(AdvanceToEnd) + r + '\"';

        public override Rule WS => Named((CarriageReturn | LineFeed | Space | Tab).ZeroOrMore());
        public Rule Exponent => Named(CharSet('e', 'E') + Sign.Optional() + Digits);
        public Rule Fraction => Named("." + Digits);
        public new Rule Integer => Named(Optional('-') + ("0" | Digits));
        public new Rule EscapedChar => Named('\\' + (CharSet("\"\\/bfnrt") | 'u' + (HexDigit + HexDigit + HexDigit + HexDigit)));
        public Rule StringChar => Named(EscapedChar | AnyChar.Except('\"'));
        public Rule Number => Node(Integer + Fraction.Optional() + Exponent.Optional());
        public Rule String => Node(DoubleQuoted(StringChar.ZeroOrMore()));
        public Rule Constant => Node(Strings("true", "false", "null") + IdentifierChar.NotAt());
        public Rule Elements => Named(Element + WS + ("," + WS + Element + WS).ZeroOrMore());
        public Rule Array => Node("[" + OnFail(AdvanceToEnd) + WS + Elements.Optional() + WS + "]");
        public Rule Member => Node(String + OnFail(AdvanceToEnd) + WS + ":" + WS + Element);
        public Rule Members => Named(Member + OnFail(AdvanceToEnd) + WS + ("," + WS + Member + WS).ZeroOrMore());
        public Rule Object => Node("{" + OnFail(AdvanceToEnd) + WS + Members.Optional() + "}");
        public Rule Value => Named(Object | Array | String | Number | Constant);
        public Rule Element => Node(Recursive(nameof(Value)));
        public Rule Json => Node(WS + Element + WS);
    }
}
