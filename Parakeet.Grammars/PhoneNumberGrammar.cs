namespace Parakeet.Grammars
{
    // https://stackoverflow.com/questions/16699007/regular-expression-to-match-standard-10-digit-phone-number
    // https://github.com/google/libphonenumber
    // https://github.com/twcclegg/libphonenumber-csharp
    public class PhoneNumberGrammar : BaseCommonGrammar
    {
        public static readonly PhoneNumberGrammar Instance = new PhoneNumberGrammar();
        public override Rule StartRule => PhoneNumber;
        public Rule CountryCode => Named(Optional('+' + Spaces) + Digit.Counted(1, 3));
        public Rule Separators => Named(".- ".ToCharSetRule().ZeroOrMore());
        public Rule AreaCodeDigits => Named(Digit.Counted(3));
        public Rule AreaCode => Named(Parenthesized(AreaCodeDigits) | AreaCodeDigits);
        public Rule Exchange => Named(Digit.Counted(3));
        public Rule Subscriber => Named(Digit.Counted(4));
        public Rule PhoneNumber => Named((CountryCode + Separators).Optional() + AreaCode + Separators + Exchange + Separators + Subscriber);
    }
}