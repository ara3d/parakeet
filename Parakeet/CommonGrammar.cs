using System.Linq;

namespace Parakeet
{
    public class CommonGrammar : Grammar
    {
        public static CommonGrammar Instance = new CommonGrammar();

        public override Rule WS => Spaces.Optional();
        public override Rule Recovery => OnError(AdvanceToEnd);

        public Rule EndOfInput => EndOfInputRule.Default;
        public Rule AnyChar => Named(AnyCharRule.Default);
        public Rule AdvanceToEnd => AnyChar.ZeroOrMore();
        public Rule LowerCaseLetter => Named('a'.To('z'));
        public Rule UpperCaseLetter => Named('A'.To('Z'));
        public Rule Letter => Named(LowerCaseLetter | UpperCaseLetter);
        public Rule Digit => Named('0'.To('9'));
        public Rule DigitOrLetter => Named(Letter | Digit);
        public Rule IdentifierFirstChar => Named('_' | Letter);
        public Rule IdentifierChar => Named(IdentifierFirstChar | Digit);
        public Rule FractionalPart => Named("." + Digits);
        public Rule HexDigit => Named(Digit | 'a'.To('f') | 'A'.To('F'));
        public Rule BinDigit => Named('0'.To('1'));
        public Rule Sign => Named("+-".ToCharSetRule());
        public Rule ExponentPart => Named("eE".ToCharSetRule() + Sign.Optional() + Digits);
        public Rule Digits => Named(Digit.OneOrMore());
        public Rule SpaceChars => Named(" \t\n\r\0\v\f".ToCharSetRule());
        public Rule Spaces => Named(SpaceChars.OneOrMore());
        public Rule NewLine => Named(Strings("\r\n", "\n"));
        public Rule UntilNextLine => Named(AnyChar.Except(NewLine).ZeroOrMore().Then(NewLine.Optional()));
        public Rule CppStyleSingleLineComment => Named("//" + UntilNextLine);
        public Rule CStyleBlockComment => Named("/*" + UntilPast("*/"));
        public Rule CppStyleComment => Named(CppStyleSingleLineComment | CStyleBlockComment);

        public Rule UntilPast(Rule r) => RepeatUntilPast(AnyChar, r);
        public Rule RepeatUntilPast(Rule repeat, Rule delimiter) => delimiter.NotAt().Then(repeat).ZeroOrMore().Then(EndOfInput | delimiter);

        public Rule ListOfAtLeastOne(Rule r, Rule sep = null) => (r + WS + ((sep ?? Comma) + r + WS).ZeroOrMore()).Then(Optional(sep ?? Comma));
        public Rule List(Rule r, Rule sep = null) => ListOfAtLeastOne(r, sep).Optional();
        public Rule Parenthesized(Rule r) => Symbol("(") + r + Symbol(")");
        public Rule ParenthesizedList(Rule r, Rule sep = null) => Parenthesized(List(r, sep));
        public Rule Bracketed(Rule r) => Symbol("[") + r + Symbol("]");
        public Rule BracketedList(Rule r, Rule sep = null) => Bracketed(List(r, sep));
        public Rule Keyword(string s) => s + IdentifierChar.NotAt() + WS;
        public Rule Comma => Named(Symbol(","));
        public Rule Symbol(string s) => s + WS;
        public Rule Symbols(params string[] strings) => Choice(strings.OrderByDescending(x => x.Length).Select(Symbol).ToArray());
        public Rule Keywords(params string[] strings) => Choice(strings.OrderByDescending(x => x.Length).Select(Keyword).ToArray());
        public Rule Braced(Rule r) => Symbol("{") + Recovery + r + Symbol("}");
        public Rule BracedList(Rule r, Rule sep = null) => Braced(List(r, sep));
        public Rule AngledBracketList(Rule r, Rule sep = null) => Symbol("<") + List(r, sep) + Symbol(">");

        public Rule Float => Integer + ((FractionalPart + ExponentPart.Optional()) | ExponentPart);
        public Rule Integer => Optional('-') + Digits;
    }
}