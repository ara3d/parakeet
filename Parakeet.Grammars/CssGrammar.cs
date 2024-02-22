
namespace Ara3D.Parakeet.Grammars
{
    /// <summary>
    /// https://www.w3.org/TR/CSS21/grammar.html
    /// https://www.w3schools.com/css/css_syntax.asp
    /// https://developer.mozilla.org/en-US/docs/Web/CSS/Syntax
    /// </summary>
    public class CssGrammar : BaseCommonGrammar
    {
        public static readonly CssGrammar Instance = new CssGrammar();
        public override Rule StartRule => StyleSheet;

        public Rule Space => " \t\r\n\f".ToCharSetRule();
        public Rule S => Named(Space.OneOrMore());
        public override Rule WS => Named(S.Optional());

        // Token rules 
        public Rule H => Named(HexDigit);
        public Rule NonAscii => Named(new CharRangeRule((char)240, (char)377));
        public Rule Unicode => Named('\\' + H.Counted(1, 6) + ("\r\n" | Space).Optional());
        public Rule Escape => Named(Unicode | '\\' + Not("\r\n\f".ToCharSetRule() | HexDigit));
        public Rule Nmstart => Named(IdentifierFirstChar | NonAscii | Escape);
        public Rule Nmchar => Named(Nmstart | Digit | '-');
        public Rule NL => Named(new StringRule("\n") | "\r\n" | "\r" | "\f");
        public Rule String1 => Named(DoubleQuotedString(AnyCharExceptOneOf("\n\r\f\"\\") | "\\" + NewLine | Escape));
        public Rule String2 => Named(SingleQuotedString(AnyCharExceptOneOf("\n\r\f\'\\") | "\\" + NewLine | Escape));
        public Rule Ident => Named(Optional("-") + Nmstart + Nmchar.ZeroOrMore());
        public Rule Name => Named(Nmchar.OneOrMore());
        public Rule Num => Named(Digit.ZeroOrMore() + "." + Digit.OneOrMore() | Digits);
        public Rule String => Named(String1 | String2);
        public Rule Url => Named(("!#$%&*-~".ToCharSetRule() | NonAscii | Escape).ZeroOrMore());
        public Rule Comment => Named(CStyleBlockComment);
        
        public Rule Cdo => Named("<!--");
        public Rule Cdc => Named("-->");
        public Rule Includes => Named("~=");
        public Rule DashMatch => Named("|=");
        public Rule Hash => Named("#" + Name);
        public Rule ImportSym => Named(CaseInvariant("@import"));
        public Rule PageSym => Named(CaseInvariant("@page"));
        public Rule MediaSym => Named(CaseInvariant("@media"));
        public Rule CharSetSym => Named(CaseInvariant("@charset"));
        public Rule ImportantSym => Named("!" + (WS | Comment).ZeroOrMore() + CaseInvariant("important"));
        public Rule Ems => Named(Num + CaseInvariant("em"));
        public Rule Exs => Named(Num + CaseInvariant("ex"));
        public Rule Length => Named(Num + CaseInvariant("cm") | CaseInvariant("mm") | CaseInvariant("in") | CaseInvariant("pt") | CaseInvariant("pc"));
        public Rule Angle => Named(Num + CaseInvariant("deg") | CaseInvariant("rad") | CaseInvariant("grad"));
        public Rule Time => Named(Num + CaseInvariant("ms") | CaseInvariant("s"));
        public Rule Freq => Named(Num + CaseInvariant("hz") | CaseInvariant("khz"));
        public Rule Dimension => Named(Num + Ident);
        public Rule Percentage => Named(Num + "%");
        public Rule Number => Named(Num);
        public Rule Uri => Named("url(" + WS + (String | Url) + WS + ")");
        public Rule CharSet => Node(CharSetSym + String + ";");
        
        public Rule Medium => Node(Ident);
        public Rule MediaList => Node(List(Medium)); 
        public Rule Import => Node(ImportSym + WS + (String | Uri) + WS + MediaList.Optional() + ";");

        public Rule CommentParts => Named((Cdo + WS | Cdc + WS).ZeroOrMore());
        public Rule Imports => Node((Import + CommentParts).ZeroOrMore());

        public Rule Prio => Node(ImportantSym);
        public Rule Expr => Node(ListOfAtLeastOne(Recursive(nameof(Term)), Operator.Optional()));
        public Rule Declaration => Node(Property + Sym(":") + Expr + Prio.Optional());
        public Rule HexColor => Node("#" + HexDigit.Counted(6) | HexDigit.Counted(3));
        public Rule Function => Node(Ident + Parenthesized(Expr.Optional()));
        public Rule Term => Node(UnaryOperator.Optional() +
                                 (Percentage | Length | Ems | Exs | Angle | Time | Freq | String | Ident | Number | Uri | HexColor | Function));
        public Rule Class => Node("." + Ident);
        public Rule AttribOperator => Node("=" | Includes | DashMatch);
        public Rule AttribValue => Node(Ident | String);
        public Rule Attrib => Node(Bracketed(Ident + AttribOperator + AttribValue.Optional()));
        public Rule ElementName => Node(Ident | "*");
        public Rule Pseudo => Node(":" + (Function | Ident));
        public Rule SelectorPart => Node(Hash | Class | Attrib | Pseudo);
        public Rule SimpleSelector => Node(ElementName + SelectorPart.ZeroOrMore() | SelectorPart.OneOrMore());
        public Rule CombinedSelector => Node(Combinator.Optional() + Recursive(nameof(Selector)));
        public Rule Selector => Node(SimpleSelector + CombinedSelector.Optional());
        public Rule Declarations => Node(List(Selector, Sym(";")));
        public Rule Selectors => Node(List(Selector));
        public Rule RuleSet => Node(Selectors + Braced(Declarations));
        public Rule Property => Node(Ident);
        public Rule UnaryOperator => Node(new StringRule("+") | "-");
        public Rule Combinator => Node(Symbols("+", ">"));
        public Rule Operator => Node(Symbols("//", ","));
        public Rule PseudoPage => Node(":" + Ident);
        public Rule PageDeclarations => Node(BracedList(Declaration, Sym(";")));
        public Rule Page => Node(PageSym + WS + PseudoPage.Optional() + PageDeclarations);
        public Rule Content => Node(RuleSet | MediaList | Page);
        public Rule Contents => Node((Content + CommentParts).ZeroOrMore());
        public Rule StyleSheet => Node(CharSet.Optional() + (S | Cdo | Cdc).ZeroOrMore() + Imports + Contents);
    }

    /*
     * stylesheet
  : [ CHARSET_SYM STRING ';' ]?
    [S|CDO|CDC]* [ import [ CDO S* | CDC S* ]* ]*
    [ [ ruleset | media | page ] [ CDO S* | CDC S* ]* ]*
  ;
import
  : IMPORT_SYM S*
    [STRING|URI] S* media_list? ';' S*
  ;
media
  : MEDIA_SYM S* media_list '{' S* ruleset* '}' S*
  ;
media_list
  : medium [ COMMA S* medium]*
  ;
medium
  : IDENT S*
  ;
page
  : PAGE_SYM S* pseudo_page?
    '{' S* declaration? [ ';' S* declaration? ]* '}' S*
  ;
pseudo_page
  : ':' IDENT S*
  ;
operator
  : '/' S* | ',' S*
  ;
combinator
  : '+' S*
  | '>' S*
  ;
unary_operator
  : '-' | '+'
  ;
property
  : IDENT S*
  ;
ruleset
  : selector [ ',' S* selector ]*
    '{' S* declaration? [ ';' S* declaration? ]* '}' S*
  ;
selector
  : simple_selector [ combinator selector | S+ [ combinator? selector ]? ]?
  ;
simple_selector
  : element_name [ HASH | class | attrib | pseudo ]*
  | [ HASH | class | attrib | pseudo ]+
  ;
class
  : '.' IDENT
  ;
element_name
  : IDENT | '*'
  ;
attrib
  : '[' S* IDENT S* [ [ '=' | INCLUDES | DASHMATCH ] S*
    [ IDENT | STRING ] S* ]? ']'
  ;
pseudo
  : ':' [ IDENT | FUNCTION S* [IDENT S*]? ')' ]
  ;
declaration
  : property ':' S* expr prio?
  ;
prio
  : IMPORTANT_SYM S*
  ;
expr
  : term [ operator? term ]*
  ;
term
  : unary_operator?
    [ NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* |
      TIME S* | FREQ S* ]
  | STRING S* | IDENT S* | URI S* | hexcolor | function
  ;
function
  : FUNCTION S* expr ')' S*
  ;

 // There is a constraint on the color that it must
 // have either 3 or 6 hex-digits (i.e., [0-9a-fA-F])
 // after the "#"; e.g., "#000" is OK, but "#abcd" is not.

    hexcolor
        : HASH S*
        ;

h		[0-9a-f]
nonascii	[\240-\377]
unicode		\\{h}{1,6}(\r\n|[ \t\r\n\f])?
escape		{unicode}|\\[^\r\n\f0-9a-f]
nmstart		[_a-z]|{nonascii}|{escape}
nmchar		[_a-z0-9-]|{nonascii}|{escape}
string1		\"([^\n\r\f\\"]|\\{nl}|{escape})*\"
string2		\'([^\n\r\f\\']|\\{nl}|{escape})*\'
badstring1      \"([^\n\r\f\\"]|\\{nl}|{escape})*\\?
badstring2      \'([^\n\r\f\\']|\\{nl}|{escape})*\\?
badcomment1     \/\*[^*]*\*+([^/*][^*]*\*+)*
badcomment2     \/\*[^*]*(\*+[^/*][^*]*)*
baduri1         url\({w}([!#$%&*-\[\]-~]|{nonascii}|{escape})*{w}
baduri2         url\({w}{string}{w}
baduri3         url\({w}{badstring}
comment		\/\*[^*]*\*+([^/*][^*]*\*+)*\/
ident		-?{nmstart}{nmchar}*
name		{nmchar}+
num		[0-9]+|[0-9]*"."[0-9]+
string		{string1}|{string2}
badstring       {badstring1}|{badstring2}
badcomment      {badcomment1}|{badcomment2}
baduri          {baduri1}|{baduri2}|{baduri3}
url		([!#$%&*-~]|{nonascii}|{escape})*
s		[ \t\r\n\f]+
w		{s}?
nl		\n|\r\n|\r|\f


    "#"{name}		{return HASH;}

    @{I}{M}{P}{O}{R}{T}	{return IMPORT_SYM;}
    @{P}{A}{G}{E}		{return PAGE_SYM;}
    @{M}{E}{D}{I}{A}	{return MEDIA_SYM;}
    "@charset "		{return CHARSET_SYM;}

    "!"({w}|{comment})*{I}{M}{P}{O}{R}{T}{A}{N}{T}	{return IMPORTANT_SYM;}

    {num}{E}{M}		{return EMS;}
    {num}{E}{X}		{return EXS;}
    {num}{P}{X}		{return LENGTH;}
    {num}{C}{M}		{return LENGTH;}
    {num}{M}{M}		{return LENGTH;}
    {num}{I}{N}		{return LENGTH;}
    {num}{P}{T}		{return LENGTH;}
    {num}{P}{C}		{return LENGTH;}
    {num}{D}{E}{G}		{return ANGLE;}
    {num}{R}{A}{D}		{return ANGLE;}
    {num}{G}{R}{A}{D}	{return ANGLE;}
    {num}{M}{S}		{return TIME;}
    {num}{S}		{return TIME;}
    {num}{H}{Z}		{return FREQ;}
    {num}{K}{H}{Z}		{return FREQ;}
    {num}{ident}		{return DIMENSION;}

    {num}%			{return PERCENTAGE;}
    {num}			{return NUMBER;}

    "url("{w}{string}{w}")" {return URI;}
    "url("{w}{url}{w}")"    {return URI;}
    {baduri}                {return BAD_URI;}
    {ident}"("		{return FUNCTION;}  
    */

}