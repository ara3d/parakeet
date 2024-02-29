namespace Ara3D.Parakeet.Grammars
{
    // https://datatracker.ietf.org/doc/html/rfc5322
    // Note: obsolete forms are omitted. 
    public class EmailGrammar : BaseIETFGrammar
    {
        public static readonly EmailGrammar Instance = new EmailGrammar();
        public override Rule StartRule => Address;  

        // Folded White-space
        public Rule FWS => Named((WSP.ZeroOrMore() + CRLF).Optional() + WSP.OneOrMore()); 
        
        // Printable ASCII characters except (,), and \
        public Rule CText => Node(CharRange(33, 39) | CharRange(42, 91) | CharRange(93, 126));

        // Comment or Folded-white-space
        public Rule CFWS => Node((FWS.Optional() + Comment).OneOrMore() + FWS.Optional() | FWS);
        
        public Rule Comment => Node("(" + (FWS.Optional() + Recursive(nameof(CContent))).ZeroOrMore() + FWS.Optional() + ")");
        public Rule CContent => Node(CText | QuotedPair | Comment);
        public Rule AText => Named(ALPHA | DIGIT | "!#$%&'*+-/=?^_`{|}~".ToCharSetRule());
        public Rule Atom => Node(CFWS.Optional() + AText.OneOrMore() + CFWS.Optional());
        public Rule DotAtomText => Node(AText.OneOrMore() + ("." + AText.OneOrMore()).ZeroOrMore());
        public Rule DotAtom => Node(CFWS.Optional() + DotAtomText + CFWS.Optional());
        public Rule Specials => Named("()<>[]:;@\\,.\"".ToCharSetRule());

        public Rule QuotedPair => Node("\\" + (VCHAR | WSP));
        
        // Printable US-ASCII characters not including "\" or the quote character
        public Rule QText => Named((char)33 | CharRange(35, 91) | CharRange(93, 126));
       
        public Rule QContent => Node(QText | QuotedPair);
        public Rule QuotedString => Node(CFWS.Optional() + DQuote + (FWS.Optional() + QContent).ZeroOrMore() + FWS.Optional() + DQuote + CFWS.Optional());
        public Rule Word => Node(Atom | QuotedString);
        public Rule Phrase => Node(Word.OneOrMore());
        public Rule Unstructured => Node((FWS.Optional() + VCHAR).ZeroOrMore() + WSP.ZeroOrMore());
        public Rule Address => Node(Mailbox | Group);
        public Rule Mailbox => Node(NameAddr | AddrSpec);
        public Rule NameAddr => Node(DisplayName.Optional() + AngleAddr);
        public Rule AngleAddr => Node(CFWS.Optional() + "<" + AddrSpec + ">" + CFWS.Optional());
        public Rule Group => Node(DisplayName + ":" + GroupList.Optional() + ";" + CFWS.Optional());
        public Rule DisplayName => Node(Phrase);
        public Rule MailboxList => Node(Mailbox + ("," + Mailbox).ZeroOrMore());
        public Rule AddressList => Node(Address + ("," + Address).ZeroOrMore());
        public Rule GroupList => Node(MailboxList | CFWS);

        public Rule AddrSpec => Node(LocalPart + "@" + Domain);
        public Rule LocalPart => Node(DotAtom | QuotedString);
        public Rule Domain => Node(DotAtom | DomainLiteral);

        public Rule DomainLiteral => Node(CFWS.Optional() + "[" + (FWS.Optional() + DText).ZeroOrMore() +
                                          FWS.Optional() + "]" + CFWS.Optional());

        // Printable US-ASCII characters not including "[", "]", or "\"
        public Rule DText => Node(CharRange(33, 90) | CharRange(94, 126)); 
    }
}