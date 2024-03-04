// DO NOT EDIT: Autogenerated file created on 2024-03-03 10:34:02 PM. 

namespace Ara3D.Parakeet.Cst.EmailGrammarNameSpace
{
    /// <summary>
    /// Rule = Address ::= ((Mailbox|Group)+(Spaces)?)
    /// Nodes = (Mailbox|Group)
    /// </summary>
    public class CstAddress : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Address;
        public CstAddress(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstMailbox> Mailbox => new CstNodeFilter<CstMailbox> (Children);
        public CstNodeFilter<CstGroup> Group => new CstNodeFilter<CstGroup> (Children);
    }

    /// <summary>
    /// Rule = AddressList ::= ((Address+((','+Address))*)+(Spaces)?)
    /// Nodes = (Address)+
    /// </summary>
    public class CstAddressList : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.AddressList;
        public CstAddressList(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstAddress> Address => new CstNodeFilter<CstAddress> (Children);
    }

    /// <summary>
    /// Rule = AddrSpec ::= ((LocalPart+'@'+Domain)+(Spaces)?)
    /// Nodes = (LocalPart+Domain)
    /// </summary>
    public class CstAddrSpec : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.AddrSpec;
        public CstAddrSpec(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstLocalPart> LocalPart => new CstNodeFilter<CstLocalPart> (Children);
        public CstNodeFilter<CstDomain> Domain => new CstNodeFilter<CstDomain> (Children);
    }

    /// <summary>
    /// Rule = AngleAddr ::= (((CFWS)?+'<'+AddrSpec+'>'+(CFWS)?)+(Spaces)?)
    /// Nodes = ((CFWS)?+AddrSpec+(CFWS)?)
    /// </summary>
    public class CstAngleAddr : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.AngleAddr;
        public CstAngleAddr(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCFWS> CFWS => new CstNodeFilter<CstCFWS> (Children);
        public CstNodeFilter<CstAddrSpec> AddrSpec => new CstNodeFilter<CstAddrSpec> (Children);
    }

    /// <summary>
    /// Rule = Atom ::= (((CFWS)?+(AText)++(CFWS)?)+(Spaces)?)
    /// Nodes = ((CFWS)?+(CFWS)?)
    /// </summary>
    public class CstAtom : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Atom;
        public CstAtom(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCFWS> CFWS => new CstNodeFilter<CstCFWS> (Children);
    }

    /// <summary>
    /// Rule = CContent ::= ((CText|QuotedPair|Comment)+(Spaces)?)
    /// Nodes = (CText|QuotedPair|Comment)
    /// </summary>
    public class CstCContent : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CContent;
        public CstCContent(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCText> CText => new CstNodeFilter<CstCText> (Children);
        public CstNodeFilter<CstQuotedPair> QuotedPair => new CstNodeFilter<CstQuotedPair> (Children);
        public CstNodeFilter<CstComment> Comment => new CstNodeFilter<CstComment> (Children);
    }

    /// <summary>
    /// Rule = CFWS ::= ((((((FWS)?+Comment))++(FWS)?)|FWS)+(Spaces)?)
    /// Nodes = (Comment)+
    /// </summary>
    public class CstCFWS : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CFWS;
        public CstCFWS(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstComment> Comment => new CstNodeFilter<CstComment> (Children);
    }

    /// <summary>
    /// Rule = Comment ::= (('('+(((FWS)?+CContent))*+(FWS)?+')')+(Spaces)?)
    /// Nodes = (CContent)*
    /// </summary>
    public class CstComment : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Comment;
        public CstComment(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCContent> CContent => new CstNodeFilter<CstCContent> (Children);
    }

    /// <summary>
    /// Rule = CText ::= (([!-']|[*-[]|[]-~])+(Spaces)?)
    /// Nodes = 
    /// </summary>
    public class CstCText : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CText;
        public CstCText(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = DisplayName ::= (Phrase+(Spaces)?)
    /// Nodes = Phrase
    /// </summary>
    public class CstDisplayName : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.DisplayName;
        public CstDisplayName(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstPhrase> Phrase => new CstNodeFilter<CstPhrase> (Children);
    }

    /// <summary>
    /// Rule = Domain ::= ((DotAtom|DomainLiteral)+(Spaces)?)
    /// Nodes = (DotAtom|DomainLiteral)
    /// </summary>
    public class CstDomain : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Domain;
        public CstDomain(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDotAtom> DotAtom => new CstNodeFilter<CstDotAtom> (Children);
        public CstNodeFilter<CstDomainLiteral> DomainLiteral => new CstNodeFilter<CstDomainLiteral> (Children);
    }

    /// <summary>
    /// Rule = DomainLiteral ::= (((CFWS)?+'['+(((FWS)?+DText))*+(FWS)?+']'+(CFWS)?)+(Spaces)?)
    /// Nodes = ((CFWS)?+(DText)*+(CFWS)?)
    /// </summary>
    public class CstDomainLiteral : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.DomainLiteral;
        public CstDomainLiteral(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCFWS> CFWS => new CstNodeFilter<CstCFWS> (Children);
        public CstNodeFilter<CstDText> DText => new CstNodeFilter<CstDText> (Children);
    }

    /// <summary>
    /// Rule = DotAtom ::= (((CFWS)?+DotAtomText+(CFWS)?)+(Spaces)?)
    /// Nodes = ((CFWS)?+DotAtomText+(CFWS)?)
    /// </summary>
    public class CstDotAtom : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.DotAtom;
        public CstDotAtom(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCFWS> CFWS => new CstNodeFilter<CstCFWS> (Children);
        public CstNodeFilter<CstDotAtomText> DotAtomText => new CstNodeFilter<CstDotAtomText> (Children);
    }

    /// <summary>
    /// Rule = DotAtomText ::= (((AText)++(('.'+(AText)+))*)+(Spaces)?)
    /// Nodes = 
    /// </summary>
    public class CstDotAtomText : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.DotAtomText;
        public CstDotAtomText(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = DText ::= (([!-Z]|[^-~])+(Spaces)?)
    /// Nodes = 
    /// </summary>
    public class CstDText : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.DText;
        public CstDText(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Group ::= ((DisplayName+':'+(GroupList)?+';'+(CFWS)?)+(Spaces)?)
    /// Nodes = (DisplayName+(GroupList)?+(CFWS)?)
    /// </summary>
    public class CstGroup : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Group;
        public CstGroup(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDisplayName> DisplayName => new CstNodeFilter<CstDisplayName> (Children);
        public CstNodeFilter<CstGroupList> GroupList => new CstNodeFilter<CstGroupList> (Children);
        public CstNodeFilter<CstCFWS> CFWS => new CstNodeFilter<CstCFWS> (Children);
    }

    /// <summary>
    /// Rule = GroupList ::= ((MailboxList|CFWS)+(Spaces)?)
    /// Nodes = (MailboxList|CFWS)
    /// </summary>
    public class CstGroupList : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.GroupList;
        public CstGroupList(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstMailboxList> MailboxList => new CstNodeFilter<CstMailboxList> (Children);
        public CstNodeFilter<CstCFWS> CFWS => new CstNodeFilter<CstCFWS> (Children);
    }

    /// <summary>
    /// Rule = Identifier ::= ((IdentifierFirstChar+(IdentifierChar)*)+(Spaces)?)
    /// Nodes = 
    /// </summary>
    public class CstIdentifier : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Identifier;
        public CstIdentifier(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = LocalPart ::= ((DotAtom|QuotedString)+(Spaces)?)
    /// Nodes = (DotAtom|QuotedString)
    /// </summary>
    public class CstLocalPart : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.LocalPart;
        public CstLocalPart(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDotAtom> DotAtom => new CstNodeFilter<CstDotAtom> (Children);
        public CstNodeFilter<CstQuotedString> QuotedString => new CstNodeFilter<CstQuotedString> (Children);
    }

    /// <summary>
    /// Rule = Mailbox ::= ((NameAddr|AddrSpec)+(Spaces)?)
    /// Nodes = (NameAddr|AddrSpec)
    /// </summary>
    public class CstMailbox : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Mailbox;
        public CstMailbox(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstNameAddr> NameAddr => new CstNodeFilter<CstNameAddr> (Children);
        public CstNodeFilter<CstAddrSpec> AddrSpec => new CstNodeFilter<CstAddrSpec> (Children);
    }

    /// <summary>
    /// Rule = MailboxList ::= ((Mailbox+((','+Mailbox))*)+(Spaces)?)
    /// Nodes = (Mailbox)+
    /// </summary>
    public class CstMailboxList : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.MailboxList;
        public CstMailboxList(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstMailbox> Mailbox => new CstNodeFilter<CstMailbox> (Children);
    }

    /// <summary>
    /// Rule = NameAddr ::= (((DisplayName)?+AngleAddr)+(Spaces)?)
    /// Nodes = ((DisplayName)?+AngleAddr)
    /// </summary>
    public class CstNameAddr : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.NameAddr;
        public CstNameAddr(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDisplayName> DisplayName => new CstNodeFilter<CstDisplayName> (Children);
        public CstNodeFilter<CstAngleAddr> AngleAddr => new CstNodeFilter<CstAngleAddr> (Children);
    }

    /// <summary>
    /// Rule = Phrase ::= ((Word)++(Spaces)?)
    /// Nodes = (Word)+
    /// </summary>
    public class CstPhrase : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Phrase;
        public CstPhrase(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstWord> Word => new CstNodeFilter<CstWord> (Children);
    }

    /// <summary>
    /// Rule = QContent ::= ((QText|QuotedPair)+(Spaces)?)
    /// Nodes = QuotedPair
    /// </summary>
    public class CstQContent : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.QContent;
        public CstQContent(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstQuotedPair> QuotedPair => new CstNodeFilter<CstQuotedPair> (Children);
    }

    /// <summary>
    /// Rule = QuotedPair ::= (('\'+(VCHAR|WSP))+(Spaces)?)
    /// Nodes = 
    /// </summary>
    public class CstQuotedPair : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.QuotedPair;
        public CstQuotedPair(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = QuotedString ::= (((CFWS)?+DQuote+(((FWS)?+QContent))*+(FWS)?+DQuote+(CFWS)?)+(Spaces)?)
    /// Nodes = ((CFWS)?+(QContent)*+(CFWS)?)
    /// </summary>
    public class CstQuotedString : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.QuotedString;
        public CstQuotedString(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCFWS> CFWS => new CstNodeFilter<CstCFWS> (Children);
        public CstNodeFilter<CstQContent> QContent => new CstNodeFilter<CstQContent> (Children);
    }

    /// <summary>
    /// Rule = Unstructured ::= (((((FWS)?+VCHAR))*+(WSP)*)+(Spaces)?)
    /// Nodes = 
    /// </summary>
    public class CstUnstructured : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Unstructured;
        public CstUnstructured(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Word ::= ((Atom|QuotedString)+(Spaces)?)
    /// Nodes = (Atom|QuotedString)
    /// </summary>
    public class CstWord : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Word;
        public CstWord(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstAtom> Atom => new CstNodeFilter<CstAtom> (Children);
        public CstNodeFilter<CstQuotedString> QuotedString => new CstNodeFilter<CstQuotedString> (Children);
    }

}