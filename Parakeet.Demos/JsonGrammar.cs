using System;
using System.Collections.Generic;
using System.Text;

namespace Parakeet.Demos
{
    public class JsonGrammar : CommonGrammar
    {
        public Rule DoubleQuoted(Rule r) => '\"' + r + '\"';

        public Rule WS => Token(CharSet((char)0x20, (char)0x0A, (char)0x0D, (char)0x09).ZeroOrMore());
        public Rule Exponent => Token(CharSet('e', 'E') + Sign.Optional() + Digits);
        public Rule Fraction => Token("." + Digits);
        public Rule Integer => Token(CharSet('-').Optional() + ("0" | Digits));
        public Rule EscapedChar => Token('\\' + CharSet("\"\\/bfnrt") | 'u' + (HexDigit + HexDigit + HexDigit + HexDigit));
        public Rule StringChar => Token(EscapedChar | AnyChar.Except('\"'));
        public Rule Number => Node(Integer + Fraction.Optional() + Exponent.Optional());
        public Rule String => Node(DoubleQuoted(StringChar.ZeroOrMore()));
        public Rule True => Node("true");
        public Rule False => Node("false");    
        public Rule Null => Node("null");
        public Rule Elements => Node(Element + WS + ("," + WS + Element + WS).ZeroOrMore());
        public Rule Array => Node("[" + WS + Elements.Optional() + WS + "]");
        public Rule Member => Node(String + WS + ":" + WS + Element);
        public Rule Members => Node(Member + WS + ("," + WS + Member + WS).ZeroOrMore());
        public Rule Object => Node("{" + WS + Members.Optional      () + "}");
        public Rule Value => Node(Object | Array | String | Number | True | False | Null);
        public Rule Element => Node(Recursive(() => Value + WS));
        public Rule Json => Node(Element);
    }
}
