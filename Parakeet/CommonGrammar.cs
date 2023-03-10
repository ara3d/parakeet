namespace Parakeet
{
    public class CommonGrammar : Grammar
    {
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
        public Rule FractionalPart => Named("." + Digits.Optional());
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
        public Rule RepeatUntilPast(Rule repeat, Rule delimiter) => delimiter.NotAt().Then(repeat).ZeroOrMore().Then(delimiter);
    }
}