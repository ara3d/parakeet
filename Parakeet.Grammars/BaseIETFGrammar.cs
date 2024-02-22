namespace Ara3D.Parakeet.Grammars
{
    // https://datatracker.ietf.org/doc/html/rfc5234#appendix-B
    public class BaseIETFGrammar : BaseCommonGrammar
    {
        public Rule ALPHA => Named(Letter);
        public Rule BIT => Named('0'.To('1'));
        public Rule CHAR => Named(AsciiCharExceptNul);
        public Rule CR => Named(Cr);
        public Rule LF => Named(Lf);
        public Rule CRLF => Named(CrLf);
        public Rule CTL => Named(ControlChar);
        public Rule DIGIT => Named(Digit);
        public Rule WSP => Named(' ' | '\t');
        public Rule HEXDIG => Named(DIGIT | CharRange('A', 'Z'));
        public Rule HTAB => Named(Tab);
        public Rule LWSP => Named((WSP | (CRLF + WSP)).ZeroOrMore());
        public Rule OCTET => Named(Octet);
        public Rule SP => Named(' ');
        public Rule VCHAR => Named(VisibleChar);
    }
}