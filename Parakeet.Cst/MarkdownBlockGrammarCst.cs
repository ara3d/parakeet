// DO NOT EDIT: Autogenerated file created on 2024-06-03 7:58:22 PM. 
using System;
using System.Linq;

namespace Ara3D.Parakeet.Cst.MarkdownBlockGrammarNameSpace
{
    /// <summary>This interface exists to make it easy to auto-generate type switches</summary>
    public interface IMarkdownBlockCstNode { }

    /// <summary>
    /// Rule = BlankLine ::= ((WS+NewLine)+WS)
    /// Nodes = 
    /// </summary>
    public class CstBlankLine : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.BlankLine;
        public CstBlankLine(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = Block ::= ((CodeBlock|Comment|Line)+WS)
    /// Nodes = (CodeBlock|Comment|Line)
    /// </summary>
    public class CstBlock : CstNodeChoice, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Block;
        public CstBlock(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstCodeBlock> CodeBlock => new CstNodeFilter<CstCodeBlock> (Children);
        public CstNodeFilter<CstComment> Comment => new CstNodeFilter<CstComment> (Children);
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

    /// <summary>
    /// Rule = BlockQuotedLine ::= ((Indents+QuoteMarker+RestOfLine)+WS)
    /// Nodes = (Indents+QuoteMarker+RestOfLine)
    /// </summary>
    public class CstBlockQuotedLine : CstNodeSequence, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.BlockQuotedLine;
        public CstBlockQuotedLine(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIndents> Indents => new CstNodeFilter<CstIndents> (Children);
        public CstNodeFilter<CstQuoteMarker> QuoteMarker => new CstNodeFilter<CstQuoteMarker> (Children);
        public CstNodeFilter<CstRestOfLine> RestOfLine => new CstNodeFilter<CstRestOfLine> (Children);
    }

    /// <summary>
    /// Rule = CodeBlock ::= ((CodeBlockDelimiter+_RECOVER_+CodeBlockLang+CodeBlockText+CodeBlockDelimiter)+WS)
    /// Nodes = (CodeBlockLang+CodeBlockText)
    /// </summary>
    public class CstCodeBlock : CstNodeSequence, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CodeBlock;
        public CstCodeBlock(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstCodeBlockLang> CodeBlockLang => new CstNodeFilter<CstCodeBlockLang> (Children);
        public CstNodeFilter<CstCodeBlockText> CodeBlockText => new CstNodeFilter<CstCodeBlockText> (Children);
    }

    /// <summary>
    /// Rule = CodeBlockLang ::= (((Identifier)?+_RECOVER_+BlankLine)+WS)
    /// Nodes = ((Identifier)?+BlankLine)
    /// </summary>
    public class CstCodeBlockLang : CstNodeSequence, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CodeBlockLang;
        public CstCodeBlockLang(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
        public CstNodeFilter<CstBlankLine> BlankLine => new CstNodeFilter<CstBlankLine> (Children);
    }

    /// <summary>
    /// Rule = CodeBlockText ::= (((!(CodeBlockDelimiter)+AnyChar))*+WS)
    /// Nodes = 
    /// </summary>
    public class CstCodeBlockText : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CodeBlockText;
        public CstCodeBlockText(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = Comment ::= ((XmlStyleComment+TextLine)+WS)
    /// Nodes = TextLine
    /// </summary>
    public class CstComment : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Comment;
        public CstComment(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstTextLine> TextLine => new CstNodeFilter<CstTextLine> (Children);
    }

    /// <summary>
    /// Rule = Document ::= ((Block)*+WS)
    /// Nodes = (Block)*
    /// </summary>
    public class CstDocument : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Document;
        public CstDocument(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstBlock> Block => new CstNodeFilter<CstBlock> (Children);
    }

    /// <summary>
    /// Rule = H1Underline ::= ((IndentsOrQuoteMarkers+WS+('='){2,2147483647}+_RECOVER_+EatWsToNextLine)+WS)
    /// Nodes = IndentsOrQuoteMarkers
    /// </summary>
    public class CstH1Underline : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.H1Underline;
        public CstH1Underline(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIndentsOrQuoteMarkers> IndentsOrQuoteMarkers => new CstNodeFilter<CstIndentsOrQuoteMarkers> (Children);
    }

    /// <summary>
    /// Rule = H2Underline ::= ((IndentsOrQuoteMarkers+WS+('-'){2,2147483647}+_RECOVER_+EatWsToNextLine)+WS)
    /// Nodes = IndentsOrQuoteMarkers
    /// </summary>
    public class CstH2Underline : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.H2Underline;
        public CstH2Underline(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIndentsOrQuoteMarkers> IndentsOrQuoteMarkers => new CstNodeFilter<CstIndentsOrQuoteMarkers> (Children);
    }

    /// <summary>
    /// Rule = Heading ::= ((HeadingWithOperator|HeadingUnderlined)+WS)
    /// Nodes = (HeadingWithOperator|HeadingUnderlined)
    /// </summary>
    public class CstHeading : CstNodeChoice, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Heading;
        public CstHeading(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstHeadingWithOperator> HeadingWithOperator => new CstNodeFilter<CstHeadingWithOperator> (Children);
        public CstNodeFilter<CstHeadingUnderlined> HeadingUnderlined => new CstNodeFilter<CstHeadingUnderlined> (Children);
    }

    /// <summary>
    /// Rule = HeadingOperator ::= (('#')++WS)
    /// Nodes = 
    /// </summary>
    public class CstHeadingOperator : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HeadingOperator;
        public CstHeadingOperator(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = HeadingUnderlined ::= ((TextLine+(H1Underline|H2Underline))+WS)
    /// Nodes = (TextLine+(H1Underline|H2Underline))
    /// </summary>
    public class CstHeadingUnderlined : CstNodeSequence, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HeadingUnderlined;
        public CstHeadingUnderlined(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstTextLine> TextLine => new CstNodeFilter<CstTextLine> (Children);
        public CstNodeFilter<CstH1Underline> H1Underline => new CstNodeFilter<CstH1Underline> (Children);
        public CstNodeFilter<CstH2Underline> H2Underline => new CstNodeFilter<CstH2Underline> (Children);
    }

    /// <summary>
    /// Rule = HeadingWithOperator ::= ((HeadingOperator+TextLine)+WS)
    /// Nodes = (HeadingOperator+TextLine)
    /// </summary>
    public class CstHeadingWithOperator : CstNodeSequence, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HeadingWithOperator;
        public CstHeadingWithOperator(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstHeadingOperator> HeadingOperator => new CstNodeFilter<CstHeadingOperator> (Children);
        public CstNodeFilter<CstTextLine> TextLine => new CstNodeFilter<CstTextLine> (Children);
    }

    /// <summary>
    /// Rule = HorizontalLine ::= (((('*'){3,2147483647}|('-'){3,2147483647}|('_'){3,2147483647}|('='){3,2147483647})+_RECOVER_+BlankLine)+WS)
    /// Nodes = BlankLine
    /// </summary>
    public class CstHorizontalLine : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HorizontalLine;
        public CstHorizontalLine(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstBlankLine> BlankLine => new CstNodeFilter<CstBlankLine> (Children);
    }

    /// <summary>
    /// Rule = Identifier ::= ((IdentifierFirstChar+(IdentifierChar)*)+WS)
    /// Nodes = 
    /// </summary>
    public class CstIdentifier : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Identifier;
        public CstIdentifier(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = Indent ::= ((Tab|"\x20\x20\x20\x20")+WS)
    /// Nodes = 
    /// </summary>
    public class CstIndent : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Indent;
        public CstIndent(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = Indents ::= ((Indent)*+WS)
    /// Nodes = (Indent)*
    /// </summary>
    public class CstIndents : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Indents;
        public CstIndents(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIndent> Indent => new CstNodeFilter<CstIndent> (Children);
    }

    /// <summary>
    /// Rule = IndentsOrQuoteMarkers ::= (((Indent|QuoteMarker))*+WS)
    /// Nodes = ((Indent|QuoteMarker))*
    /// </summary>
    public class CstIndentsOrQuoteMarkers : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.IndentsOrQuoteMarkers;
        public CstIndentsOrQuoteMarkers(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIndent> Indent => new CstNodeFilter<CstIndent> (Children);
        public CstNodeFilter<CstQuoteMarker> QuoteMarker => new CstNodeFilter<CstQuoteMarker> (Children);
    }

    /// <summary>
    /// Rule = Line ::= ((Heading|HorizontalLine|UnorderedListItem|OrderedListItem|BlockQuotedLine|BlankLine|NonEmptyTextLine)+WS)
    /// Nodes = (Heading|HorizontalLine|UnorderedListItem|OrderedListItem|BlockQuotedLine|BlankLine|NonEmptyTextLine)
    /// </summary>
    public class CstLine : CstNodeChoice, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Line;
        public CstLine(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstHeading> Heading => new CstNodeFilter<CstHeading> (Children);
        public CstNodeFilter<CstHorizontalLine> HorizontalLine => new CstNodeFilter<CstHorizontalLine> (Children);
        public CstNodeFilter<CstUnorderedListItem> UnorderedListItem => new CstNodeFilter<CstUnorderedListItem> (Children);
        public CstNodeFilter<CstOrderedListItem> OrderedListItem => new CstNodeFilter<CstOrderedListItem> (Children);
        public CstNodeFilter<CstBlockQuotedLine> BlockQuotedLine => new CstNodeFilter<CstBlockQuotedLine> (Children);
        public CstNodeFilter<CstBlankLine> BlankLine => new CstNodeFilter<CstBlankLine> (Children);
        public CstNodeFilter<CstNonEmptyTextLine> NonEmptyTextLine => new CstNodeFilter<CstNonEmptyTextLine> (Children);
    }

    /// <summary>
    /// Rule = NonEmptyTextLine ::= ((!(NewLine)+AnyChar+AnyCharUntilNextLine)+WS)
    /// Nodes = 
    /// </summary>
    public class CstNonEmptyTextLine : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.NonEmptyTextLine;
        public CstNonEmptyTextLine(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = OrderedListItem ::= ((Indents+WS+(Digit)++'.'+WS+_RECOVER_+TextLine)+WS)
    /// Nodes = (Indents+TextLine)
    /// </summary>
    public class CstOrderedListItem : CstNodeSequence, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.OrderedListItem;
        public CstOrderedListItem(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIndents> Indents => new CstNodeFilter<CstIndents> (Children);
        public CstNodeFilter<CstTextLine> TextLine => new CstNodeFilter<CstTextLine> (Children);
    }

    /// <summary>
    /// Rule = QuoteMarker ::= ('>'+WS)
    /// Nodes = 
    /// </summary>
    public class CstQuoteMarker : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.QuoteMarker;
        public CstQuoteMarker(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = RestOfLine ::= (Line+WS)
    /// Nodes = Line
    /// </summary>
    public class CstRestOfLine : CstNode, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.RestOfLine;
        public CstRestOfLine(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

    /// <summary>
    /// Rule = TextLine ::= (AnyCharUntilNextLine+WS)
    /// Nodes = 
    /// </summary>
    public class CstTextLine : CstNodeLeaf, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.TextLine;
        public CstTextLine(ILocation location, string text) : base(location, text) { }
    }

    /// <summary>
    /// Rule = UnorderedListItem ::= ((Indents+WS+[\x2A+\x2D]+WS+_RECOVER_+TextLine)+WS)
    /// Nodes = (Indents+TextLine)
    /// </summary>
    public class CstUnorderedListItem : CstNodeSequence, IMarkdownBlockCstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.UnorderedListItem;
        public CstUnorderedListItem(ILocation location, params CstNode[] children) : base(location, children) { }
        public CstNodeFilter<CstIndents> Indents => new CstNodeFilter<CstIndents> (Children);
        public CstNodeFilter<CstTextLine> TextLine => new CstNodeFilter<CstTextLine> (Children);
    }

}
