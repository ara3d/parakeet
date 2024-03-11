using System.Linq;

namespace Ara3D.Parakeet.Grammars
{
    /// <summary>
    /// Common rules. All of which can be overridden. 
    /// </summary>
    public abstract class BaseCommonGrammar : Grammar
    {
        public override Rule StartRule => null;

        public override Rule WS => Spaces.Optional();

        // Common rules 
        public Rule EndOfInput => Named(EndOfInputRule.Default);
        public Rule AnyChar => Named(AnyCharRule.Default);
        public Rule AdvanceToEnd => AnyChar.ZeroOrMore();

        // Common named characters
        public Rule CarriageReturn => Named('\r');
        public Rule Cr => Named(CarriageReturn);
        public Rule LineFeed => Named('\n');
        public Rule Lf => Named(LineFeed);
        public Rule CrLf => Named(Cr + Lf);
        public Rule Space =>  Named(' ');
        public Rule Tab => Named('\t');
        public Rule SpaceOrTab => Named(Space | Tab);
        public Rule DQuote => Named('"');
        public Rule SQuote => Named('\'');
        public Rule ControlChar => Named(CharRange(0, 0x1F) | 0x7F);
        public Rule AsciiCharExceptNul => Named(CharRange(1, 0x7F));
        public Rule Octet => Named(CharRange(0, 0xFF));
        public Rule VisibleChar => Named(CharRange(0x21, 0x7E));

        // Identifier rules 
        public Rule LowerCaseLetter => Named('a'.To('z'));
        public Rule UpperCaseLetter => Named('A'.To('Z'));
        public Rule Letter => Named(LowerCaseLetter | UpperCaseLetter);
        public Rule DigitOrLetter => Named(Letter | Digit);
        public Rule IdentifierFirstChar => Named('_' | Letter);
        public Rule IdentifierChar => Named(IdentifierFirstChar | Digit);
        public Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());

        // Number rules
        public Rule Digit => Named('0'.To('9'));
        public Rule FractionalPart => Named("." + Digits);
        public Rule HexDigit => Named(Digit | 'a'.To('f') | 'A'.To('F'));
        public Rule BinDigit => Named('0'.To('1'));
        public Rule Sign => Named("+-".ToCharSetRule());
        public Rule ExponentPart => Named("eE".ToCharSetRule() + Sign.Optional() + Digits);
        public Rule Digits => Named(Digit.OneOrMore());
        public Rule Float => Named(Integer + ((FractionalPart + ExponentPart.Optional()) | ExponentPart));
        public Rule Integer => Named(Optional('-') + Digits);

        public Rule SpaceChars => Named(" \t\n\r\0\v\f".ToCharSetRule());
        public Rule Spaces => Named(SpaceChars.OneOrMore());
        public Rule NewLine => Named(Strings("\r\n", "\n"));

        // Common comment styles
        public Rule CppStyleSingleLineComment => Named("//" + AnyCharUntilNextLine);
        public Rule CStyleBlockComment => Named("/*" + AnyCharUntilPast("*/"));
        public Rule CppStyleComment => Named(CppStyleSingleLineComment | CStyleBlockComment);
        public Rule XmlStyleComment => Named("<!--" + AnyCharUntilPast("-->"));
        public Rule Comma => Named(Sym(","));

        // Repeated rules
        public Rule AnyCharExceptOneOf(string s) => AnyCharExcept(s.ToCharSetRule());
        public Rule AnyCharExcept(Rule r) => AnyChar.Except(r);
        public Rule AnyCharUntilAt(Rule r) => AnyChar.RepeatUntilAt(r);
        public Rule AnyCharUntilPast(Rule r) => AnyChar.RepeatUntilPast(r);
        public Rule AnyCharUntilNextLine => Named(AnyCharUntilAt(NewLine | EndOfInput) + NewLine.Optional());
        public Rule TwoOrMore(Rule r) => r.CountOrMore(2);
        public Rule ThreeOrMore(Rule r) => r.CountOrMore(3);

        // Delimited and list rules (assumes white-space before/after symbols, and by default does not recover), default separator is a comma
        public Rule ListOfAtLeastOne(Rule r, Rule sep = null) => (r + WS + ((sep ?? Comma) + r + WS).ZeroOrMore()).Then(Optional(sep ?? Comma));
        public Rule List(Rule r, Rule sep = null) => ListOfAtLeastOne(r, sep).Optional();
        public Rule Delimited(Rule startDelim, Rule endDelim, Rule r, Rule onFail = null) => startDelim + WS + (onFail ?? true) + r + WS + endDelim + WS;
        public Rule Parenthesized(Rule r, Rule onFail = null) => Delimited('(', ')', r, onFail);
        public Rule ParenthesizedList(Rule r, Rule sep = null, Rule onFail = null) => Parenthesized(List(r, sep), onFail);
        public Rule Bracketed(Rule r, Rule onFail = null) => Delimited('[', ']', r, onFail);
        public Rule BracketedList(Rule r, Rule sep = null, Rule onFail = null) => Bracketed(List(r, sep), onFail);
        public Rule Braced(Rule r, Rule onFail = null) => Delimited('{', '}', r, onFail);
        public Rule BracedList(Rule r, Rule sep = null, Rule onFail = null) => Braced(List(r, sep), onFail);
        public Rule AngleBracketed(Rule r, Rule onFail = null) => Delimited('<', '>', r, onFail);
        public Rule AngleBracketedList(Rule r, Rule sep = null, Rule onFail = null) => AngleBracketed(List(r, sep), onFail);

        // String rules that eat whitespace
        public Rule Keyword(string s) => Named(s + IdentifierChar.NotAt() + WS, $"Keyword('{s}')");
        public Rule Sym(string s) => Named(s + WS, $"Symbol('{s}')");
        public Rule Symbols(params string[] strings) => Choice(strings.OrderByDescending(x => x.Length).Select(Sym).ToArray());
        public Rule Keywords(params string[] strings) => Choice(strings.OrderByDescending(x => x.Length).Select(Keyword).ToArray());
        
        // Basic strings with escaping
        public Rule EscapedChar(char c) => $"\\{c}";
        public Rule EscapedDoubleQuote => EscapedChar('"');
        public Rule EscapedSingleQuote => EscapedChar('\'');
        public Rule StringFunc(Rule quote, Rule charRule) => Delimited(quote, quote, charRule.ZeroOrMore());
        public Rule SingleQuotedString(Rule charRule) => StringFunc('\'', charRule);
        public Rule DoubleQuotedString(Rule charRule) => StringFunc('"', charRule);
        public Rule BasicEscapedString(char delim, Rule escape) => Delimited(delim, delim, (escape | AnyChar).RepeatUntilPast(delim));
        public Rule DoubleQuoteBasicString => Named(BasicEscapedString('"', EscapedDoubleQuote));
        public Rule SingleQuoteBasicString => Named(BasicEscapedString('\'', EscapedSingleQuote));

        // By default, if an error occurs, will jump to the end of input.  
        public Rule AbortOnFail => OnFail(AdvanceToEnd);
    }
}