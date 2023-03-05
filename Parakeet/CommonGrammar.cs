namespace Parakeet
{
    public class CommonGrammar : Grammar
    {
        public Rule EndOfInput => EndOfInputRule.Default;
        public Rule AnyChar => Token(AnyCharRule.Default);
        public Rule AdvanceToEnd => AnyChar.ZeroOrMore();
        public Rule LowerCaseLetter => Token('a'.To('z'));
        public Rule UpperCaseLetter => Token('A'.To('Z'));
        public Rule Letter => Token(LowerCaseLetter | UpperCaseLetter);
        public Rule Digit => Token('0'.To('9'));
        public Rule DigitOrLetter => Token(Letter | Digit);
        public Rule IdentifierFirstChar => Token('_' | Letter);
        public Rule IdentifierChar => Token(IdentifierFirstChar | Digit);
        public Rule FractionalPart => Token("." + Digits.Optional());
        public Rule HexDigit => Token(Digit | 'a'.To('f') | 'A'.To('F'));
        public Rule BinDigit => Token('0'.To('1'));
        public Rule Sign => Token("+-".ToCharSetRule());
        public Rule ExponentPart => Token("eE".ToCharSetRule() + Sign.Optional() + Digits);
        public Rule Digits => Token(Digit.OneOrMore());
        public Rule SpaceChars => Token(" \t\n\r\0\v\f".ToCharSetRule());
        public Rule Spaces => Token(SpaceChars.OneOrMore());
        public Rule NewLine => Token(new[] { "\r\n", "\n" });
        public Rule UntilNextLine => Token(AnyChar.Except(NewLine).ZeroOrMore().Then(NewLine.Optional()));
        public Rule CppStyleSingleLineComment => Token("//" + UntilNextLine);
        public Rule CStyleBlockComment => Token("/*" + UntilPast("*/"));
        public Rule CppStyleComment => Token(CppStyleSingleLineComment | CStyleBlockComment);

        public Rule UntilPast(Rule r) => RepeatUntilPast(AnyChar, r);
        public Rule RepeatUntilPast(Rule repeat, Rule delimiter) => delimiter.NotAt().Then(repeat).ZeroOrMore().Then(delimiter);
    }
}