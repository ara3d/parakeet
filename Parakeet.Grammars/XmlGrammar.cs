using System;
using System.Linq;

namespace Ara3D.Parakeet.Grammars
{
    // https://github.com/antlr/grammars-v4/blob/master/xml/XMLParser.g4
    // https://www.w3schools.com/xml/xml_syntax.asp
    // https://www.w3.org/TR/REC-xml
    public class XmlGrammar : BaseCommonGrammar
    {
        public override Rule StartRule => Document;
        public static readonly XmlGrammar Instance = new XmlGrammar();

        public static CharRangeRule CharClass(string s)
        {
            s = s.Trim();
            if (!s.StartsWith("[") || !s.EndsWith("]"))
                throw new Exception("Not a valid character class");
            var n = s.IndexOf('-');
            if (n == -1)
                throw new Exception("Not a valid character class");
            var from = s.Substring(1, n - 1);
            var to = s.Substring(n + 1, s.Length - n - 2);
            if (!from.StartsWith("#"))
                throw new Exception("Only numeric ranges supported");
            if (!to.StartsWith("#"))
                throw new Exception("Only numeric ranges supported");
            var a = Convert.ToInt32($"0{from.Substring(1)}", 16);
            var b = Convert.ToInt32($"0{to.Substring(1)}", 16);
            if (a > short.MaxValue || b > short.MaxValue)
                return null;
            return new CharRangeRule((char)a, (char)b);
        }

        public static Rule CharClasses(string s)
        {
            return Choice(s.Split('|').Select(CharClass).Where(c => c != null));
        }

        public Rule NL => Optional('\r') + '\n';
        public Rule S => CharSet((char)0x20, (char)0x9, (char)0xD, (char)0xA).ZeroOrMore();

        public override Rule WS => Named((NL | ' ' | '\t' | XmlStyleComment).ZeroOrMore());

        public Rule DTDInternalSubset => Node("[" + AnyCharUntilPast("]"));
        public Rule DTD => Node("<!DOCTYPE" + AnyCharExceptOneOf("[]>").ZeroOrMore() + DTDInternalSubset.Optional() + S + ">");
        public Rule PI => Node("<?" + AnyCharUntilPast("?>"));
        public Rule CDStart => Named("<![CDATA[");
        public Rule CDEnd => Named("]]>");
        public Rule CData => Node(AnyChar.Except(CDEnd).ZeroOrMore());
        public Rule CDSect => Node(CDStart + CData + CDEnd);
        public Rule XmlDecl => Node("<?xml" + S + Recursive(nameof(AttrList)) + S + "?>");

        public Rule NameStartChar => Letter | ':' | '_' | CharClasses("[#xC0-#xD6] | [#xD8-#xF6] | [#xF8-#x2FF] | [#x370-#x37D] | [#x37F-#x1FFF] | [#x200C-#x200D] | [#x2070-#x218F] | [#x2C00-#x2FEF] | [#x3001-#xD7FF] | [#xF900-#xFDCF] | [#xFDF0-#xFFFD] | [#x10000-#xEFFFF]");
        public Rule NameChar => NameStartChar | '-' | '.' | Digit | (char)0xB7 | CharClasses("[#x0300-#x036F] | [#x203F-#x2040]");

        public Rule Name => Node(NameStartChar + NameChar.ZeroOrMore());

        public Rule Comment => XmlStyleComment;
        public Rule Misc => Node(Comment | PI | S);
        public Rule Prolog => Node(XmlDecl.Optional() + Misc.ZeroOrMore() + DTD.Optional() + Misc.ZeroOrMore());
        public Rule Document => Node(Prolog.Optional() + WS + Element + EndOfInput);

        public Rule Identifier => Node(IdentifierFirstChar + IdentifierChar.ZeroOrMore());

        // https://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references
        public Rule HexEntityValue => Node("x" + HexDigit.ZeroOrMore());
        public Rule NumericEntityValue => Node(Digit.ZeroOrMore());
        public Rule Entity => Node("&" + (Identifier | HexEntityValue | NumericEntityValue) + ";");
        public Rule AttrValue => Node(Identifier);
        public Rule Attr => Node(Name + S + "=" + S + AttrValue);
        public Rule AttrList => Node(Attr.ZeroOrMore());
        public Rule NSIdent => Node(Identifier + Optional(":" + Identifier));
        public Rule StartTag => Node("<" + Identifier + AttrList + Sym(">"));
        public Rule EmptyElementTag => Node(Sym("<") + Identifier + AttrList + Sym("/>"));
        public Rule EndTag => Node(Sym("</") + Identifier + Sym(">"));
        public Rule Tag => Node(EndTag | EmptyElementTag | StartTag);
        public Rule NonEmptyElement => Node(StartTag + Content + EndTag);
        public Rule Text => Node(AnyCharExceptOneOf("<&").OneOrMore());
        
        public Rule Content => Node(Text | XmlStyleComment | CDSect | DTD | Recursive(nameof(Element)));
        public Rule Element => Node(EmptyElementTag | (StartTag + Content + EndTag));


        /*

[1]   	document	   ::=   	prolog element Misc*

[2]   	Char	   ::=   	#x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]	// any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
[3]   	S	   ::=   	(#x20 | #x9 | #xD | #xA)+

Names and Tokens
[4]   	NameStartChar	   ::=   	":" | [A-Z] | "_" | [a-z] | [#xC0-#xD6] | [#xD8-#xF6] | [#xF8-#x2FF] | [#x370-#x37D] | [#x37F-#x1FFF] | [#x200C-#x200D] | [#x2070-#x218F] | [#x2C00-#x2FEF] | [#x3001-#xD7FF] | [#xF900-#xFDCF] | [#xFDF0-#xFFFD] | [#x10000-#xEFFFF]
[4a]   	NameChar	   ::=   	NameStartChar | "-" | "." | [0-9] | #xB7 | [#x0300-#x036F] | [#x203F-#x2040]
[5]   	Name	   ::=   	NameStartChar (NameChar)*
[6]   	Names	   ::=   	Name (#x20 Name)*
[7]   	Nmtoken	   ::=   	(NameChar)+
[8]   	Nmtokens	   ::=   	Nmtoken (#x20 Nmtoken)*

Literals
[9]   	EntityValue	   ::=   	'"' ([^%&"] | PEReference | Reference)* '"'
|  "'" ([^%&'] | PEReference | Reference)* "'"
[10]   	AttValue	   ::=   	'"' ([^<&"] | Reference)* '"'
|  "'" ([^<&'] | Reference)* "'"
[11]   	SystemLiteral	   ::=   	('"' [^"]* '"') | ("'" [^']* "'")
[12]   	PubidLiteral	   ::=   	'"' PubidChar* '"' | "'" (PubidChar - "'")* "'"
[13]   	PubidChar	   ::=   	#x20 | #xD | #xA | [a-zA-Z0-9] | [-'()+,./:=?;!*#@$_%]

Character Data
[14]   	CharData	   ::=   	[^<&]* - ([^<&]* ']]>' [^<&]*)

Comments
[15]   	Comment	   ::=   	'<!--' ((Char - '-') | ('-' (Char - '-')))* '-->'

2.6 Processing Instructions
[Definition: Processing instructions (PIs) allow documents to contain instructions for applications.]

Processing Instructions
[16]   	PI	   ::=   	'<?' PITarget (S (Char* - (Char* '?>' Char*)))? '?>'
[17]   	PITarget	   ::=   	Name - (('X' | 'x') ('M' | 'm') ('L' | 'l'))

CDATA Sections
[18]   	CDSect	   ::=   	CDStart CData CDEnd
[19]   	CDStart	   ::=   	'<![CDATA['
[20]   	CData	   ::=   	(Char* - (Char* ']]>' Char*))
[21]   	CDEnd	   ::=   	']]>'

Prolog
[22]   	prolog	   ::=   	XMLDecl? Misc* (doctypedecl Misc*)?
[23]   	XMLDecl	   ::=   	'<?xml' VersionInfo EncodingDecl? SDDecl? S? '?>'
[24]   	VersionInfo	   ::=   	S 'version' Eq ("'" VersionNum "'" | '"' VersionNum '"')
[25]   	Eq	   ::=   	S? '=' S?
[26]   	VersionNum	   ::=   	'1.' [0-9]+
[27]   	Misc	   ::=   	Comment | PI | S

Document Type Definition

[28]   	doctypedecl	   ::=   	'<!DOCTYPE' S Name (S ExternalID)? S? ('[' intSubset ']' S?)? '>'	[VC: Root Element Type]
[WFC: External Subset]
[28a]   	DeclSep	   ::=   	PEReference | S	[WFC: PE Between Declarations]
[28b]   	intSubset	   ::=   	(markupdecl | DeclSep)*
[29]   	markupdecl	   ::=   	elementdecl | AttlistDecl | EntityDecl | NotationDecl | PI | Comment	[VC: Proper Declaration/PE Nesting]
[WFC: PEs in Internal Subset]

External Subset

[30]   	extSubset	   ::=   	TextDecl? extSubsetDecl
[31]   	extSubsetDecl	   ::=   	( markupdecl | conditionalSect | DeclSep)*

Standalone Document Declaration

[32]   	SDDecl	   ::=   	S 'standalone' Eq (("'" ('yes' | 'no') "'") | ('"' ('yes' | 'no') '"'))	[VC: Standalone Document Declaration]

Element

[39]   	element	   ::=   	EmptyElemTag
| STag content ETag	[WFC: Element Type Match]
[VC: Element Valid]

Start-tag

[40]   	STag	   ::=   	'<' Name (S Attribute)* S? '>'	[WFC: Unique Att Spec]
[41]   	Attribute	   ::=   	Name Eq AttValue	[VC: Attribute Value Type]
[WFC: No External Entity References]
[WFC: No < in Attribute Values]

End-tag

[42]   	ETag	   ::=   	'</' Name S? '>'

Tags for Empty Elements

[44]   	EmptyElemTag	   ::=   	'<' Name (S Attribute)* S? '/>'	[WFC: Unique Att Spec]

Element Type Declaration

[45]   	elementdecl	   ::=   	'<!ELEMENT' S Name S contentspec S? '>'	[VC: Unique Element Type Declaration]
[46]   	contentspec	   ::=   	'EMPTY' | 'ANY' | Mixed | children

Element-content Models

[47]   	children	   ::=   	(choice | seq) ('?' | '*' | '+')?
[48]   	cp	   ::=   	(Name | choice | seq) ('?' | '*' | '+')?
[49]   	choice	   ::=   	'(' S? cp ( S? '|' S? cp )+ S? ')'	[VC: Proper Group/PE Nesting]
[50]   	seq	   ::=   	'(' S? cp ( S? ',' S? cp )* S? ')'	[VC: Proper Group/PE Nesting]

Mixed-content Declaration

[51]   	Mixed	   ::=   	'(' S? '#PCDATA' (S? '|' S? Name)* S? ')*'
| '(' S? '#PCDATA' S? ')'	[VC: Proper Group/PE Nesting]
[VC: No Duplicate Types]

Attribute-list Declaration

[52]   	AttlistDecl	   ::=   	'<!ATTLIST' S Name AttDef* S? '>'
[53]   	AttDef	   ::=   	S Name S AttType S DefaultDecl

Attribute Types

[54]   	AttType	   ::=   	StringType | TokenizedType | EnumeratedType
[55]   	StringType	   ::=   	'CDATA'
[56]   	TokenizedType	   ::=   	'ID'	[VC: ID]
[VC: One ID per Element Type]
[VC: ID Attribute Default]
| 'IDREF'	[VC: IDREF]
| 'IDREFS'	[VC: IDREF]
| 'ENTITY'	[VC: Entity Name]
| 'ENTITIES'	[VC: Entity Name]
| 'NMTOKEN'	[VC: Name Token]
| 'NMTOKENS'	[VC: Name Token]

Enumerated Attribute Types

[57]   	EnumeratedType	   ::=   	NotationType | Enumeration
[58]   	NotationType	   ::=   	'NOTATION' S '(' S? Name (S? '|' S? Name)* S? ')'	[VC: Notation Attributes]
[VC: One Notation Per Element Type]
[VC: No Notation on Empty Element]
[VC: No Duplicate Tokens]
[59]   	Enumeration	   ::=   	'(' S? Nmtoken (S? '|' S? Nmtoken)* S? ')'	[VC: Enumeration]
[VC: No Duplicate Tokens]

Attribute Defaults

[60]   	DefaultDecl	   ::=   	'#REQUIRED' | '#IMPLIED'
| (('#FIXED' S)? AttValue)	[VC: Required Attribute]
[VC: Attribute Default Value Syntactically Correct]
[WFC: No < in Attribute Values]
[VC: Fixed Attribute Default]
[WFC: No External Entity References]

Conditional Section

[61]   	conditionalSect	   ::=   	includeSect | ignoreSect
[62]   	includeSect	   ::=   	'<![' S? 'INCLUDE' S? '[' extSubsetDecl ']]>'	[VC: Proper Conditional Section/PE Nesting]
[63]   	ignoreSect	   ::=   	'<![' S? 'IGNORE' S? '[' ignoreSectContents* ']]>'	[VC: Proper Conditional Section/PE Nesting]
[64]   	ignoreSectContents	   ::=   	Ignore ('<![' ignoreSectContents ']]>' Ignore)*
[65]   	Ignore	   ::=   	Char* - (Char* ('<![' | ']]>') Char*)

Character Reference

[66]   	CharRef	   ::=   	'&#' [0-9]+ ';'
| '&#x' [0-9a-fA-F]+ ';'	[WFC: Legal Character]

Entity Reference

[67]   	Reference	   ::=   	EntityRef | CharRef
[68]   	EntityRef	   ::=   	'&' Name ';'	[WFC: Entity Declared]
[VC: Entity Declared]
[WFC: Parsed Entity]
[WFC: No Recursion]
[69]   	PEReference	   ::=   	'%' Name ';'	[VC: Entity Declared]
[WFC: No Recursion]
[WFC: In DTD]

Entity Declaration
[70]   	EntityDecl	   ::=   	GEDecl | PEDecl
[71]   	GEDecl	   ::=   	'<!ENTITY' S Name S EntityDef S? '>'
[72]   	PEDecl	   ::=   	'<!ENTITY' S '%' S Name S PEDef S? '>'
[73]   	EntityDef	   ::=   	EntityValue | (ExternalID NDataDecl?)
[74]   	PEDef	   ::=   	EntityValue | ExternalID

External Entity Declaration
[75]   	ExternalID	   ::=   	'SYSTEM' S SystemLiteral
| 'PUBLIC' S PubidLiteral S SystemLiteral
[76]   	NDataDecl	   ::=   	S 'NDATA' S Name	[VC: Notation Declared]

Text Declaration
[77]   	TextDecl	   ::=   	'<?xml' VersionInfo? EncodingDecl S? '?>'

Well-Formed External Parsed Entity
[78]   	extParsedEnt	   ::=   	TextDecl? content

Encoding Declaration
[80]   	EncodingDecl	   ::=   	S 'encoding' Eq ('"' EncName '"' | "'" EncName "'" )
[81]   	EncName	   ::=   	[A-Za-z] ([A-Za-z0-9._] | '-')*

Notation Declarations
[82]   	NotationDecl	   ::=   	'<!NOTATION' S Name S (ExternalID | PublicID) S? '>'	[VC: Unique Notation Name]
[83]   	PublicID	   ::=   	'PUBLIC' S PubidLiteral
*/
            }
}