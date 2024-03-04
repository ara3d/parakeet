// DO NOT EDIT: Autogenerated file created on 2024-03-03 10:34:02 PM. 
using System;
using System.Linq;

namespace Ara3D.Parakeet.Cst.MarkdownBlockGrammarNameSpace
{
    /// <summary>
    /// Rule = BlankLine ::= ((WS+NewLine)+WS)
    /// Nodes = 
    /// </summary>
    public class CstBlankLine : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.BlankLine;
        public CstBlankLine(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Block ::= ((CodeBlock|Comment|Line)+WS)
    /// Nodes = (CodeBlock|Comment|Line)
    /// </summary>
    public class CstBlock : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Block;
        public CstBlock(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCodeBlock> CodeBlock => new CstNodeFilter<CstCodeBlock> (Children);
        public CstNodeFilter<CstComment> Comment => new CstNodeFilter<CstComment> (Children);
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

    /// <summary>
    /// Rule = BlockQuotedLine ::= (('>'+Line)+WS)
    /// Nodes = Line
    /// </summary>
    public class CstBlockQuotedLine : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.BlockQuotedLine;
        public CstBlockQuotedLine(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

    /// <summary>
    /// Rule = CodeBlock ::= ((CodeBlockDelimiter+_RECOVER_+CodeBlockLang+CodeBlockText+CodeBlockDelimiter)+WS)
    /// Nodes = (CodeBlockLang+CodeBlockText)
    /// </summary>
    public class CstCodeBlock : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CodeBlock;
        public CstCodeBlock(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCodeBlockLang> CodeBlockLang => new CstNodeFilter<CstCodeBlockLang> (Children);
        public CstNodeFilter<CstCodeBlockText> CodeBlockText => new CstNodeFilter<CstCodeBlockText> (Children);
    }

    /// <summary>
    /// Rule = CodeBlockLang ::= (((Identifier)?+_RECOVER_+WSToEndOfLine)+WS)
    /// Nodes = (Identifier)?
    /// </summary>
    public class CstCodeBlockLang : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CodeBlockLang;
        public CstCodeBlockLang(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
    }

    /// <summary>
    /// Rule = CodeBlockText ::= (((!(CodeBlockDelimiter)+AnyChar))*+WS)
    /// Nodes = 
    /// </summary>
    public class CstCodeBlockText : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.CodeBlockText;
        public CstCodeBlockText(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Comment ::= (XmlStyleComment+WS)
    /// Nodes = 
    /// </summary>
    public class CstComment : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Comment;
        public CstComment(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Document ::= ((Block)*+WS)
    /// Nodes = (Block)*
    /// </summary>
    public class CstDocument : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Document;
        public CstDocument(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstBlock> Block => new CstNodeFilter<CstBlock> (Children);
    }

    /// <summary>
    /// Rule = H1Underline ::= ((('='){2,2147483647}+_RECOVER_+WSToEndOfLine)+WS)
    /// Nodes = 
    /// </summary>
    public class CstH1Underline : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.H1Underline;
        public CstH1Underline(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = H2Underline ::= ((('-'){2,2147483647}+_RECOVER_+WSToEndOfLine)+WS)
    /// Nodes = 
    /// </summary>
    public class CstH2Underline : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.H2Underline;
        public CstH2Underline(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Heading ::= ((HeadingWithOperator|HeadingUnderlined)+WS)
    /// Nodes = (HeadingWithOperator|HeadingUnderlined)
    /// </summary>
    public class CstHeading : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Heading;
        public CstHeading(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstHeadingWithOperator> HeadingWithOperator => new CstNodeFilter<CstHeadingWithOperator> (Children);
        public CstNodeFilter<CstHeadingUnderlined> HeadingUnderlined => new CstNodeFilter<CstHeadingUnderlined> (Children);
    }

    /// <summary>
    /// Rule = HeadingOperator ::= (('#')++WS)
    /// Nodes = 
    /// </summary>
    public class CstHeadingOperator : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HeadingOperator;
        public CstHeadingOperator(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = HeadingUnderlined ::= ((TextLine+(H1Underline|H2Underline))+WS)
    /// Nodes = (TextLine+(H1Underline|H2Underline))
    /// </summary>
    public class CstHeadingUnderlined : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HeadingUnderlined;
        public CstHeadingUnderlined(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstTextLine> TextLine => new CstNodeFilter<CstTextLine> (Children);
        public CstNodeFilter<CstH1Underline> H1Underline => new CstNodeFilter<CstH1Underline> (Children);
        public CstNodeFilter<CstH2Underline> H2Underline => new CstNodeFilter<CstH2Underline> (Children);
    }

    /// <summary>
    /// Rule = HeadingWithOperator ::= ((HeadingOperator+Line)+WS)
    /// Nodes = (HeadingOperator+Line)
    /// </summary>
    public class CstHeadingWithOperator : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HeadingWithOperator;
        public CstHeadingWithOperator(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstHeadingOperator> HeadingOperator => new CstNodeFilter<CstHeadingOperator> (Children);
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

    /// <summary>
    /// Rule = HorizontalLine ::= (((('*'){3,2147483647}|('-'){3,2147483647}|('_'){3,2147483647})+_RECOVER_+WSToEndOfLine)+WS)
    /// Nodes = 
    /// </summary>
    public class CstHorizontalLine : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.HorizontalLine;
        public CstHorizontalLine(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Identifier ::= ((IdentifierFirstChar+(IdentifierChar)*)+WS)
    /// Nodes = 
    /// </summary>
    public class CstIdentifier : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Identifier;
        public CstIdentifier(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = Indent ::= ((Tab|"\x20\x20\x20\x20")+WS)
    /// Nodes = 
    /// </summary>
    public class CstIndent : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Indent;
        public CstIndent(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = IndentedLine ::= ((Indent+_RECOVER_+Line)+WS)
    /// Nodes = (Indent+Line)
    /// </summary>
    public class CstIndentedLine : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.IndentedLine;
        public CstIndentedLine(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIndent> Indent => new CstNodeFilter<CstIndent> (Children);
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

    /// <summary>
    /// Rule = Line ::= ((Heading|HorizontalLine|UnorderedListItem|OrderedListItem|IndentedLine|BlockQuotedLine|BlankLine|TextLine)+WS)
    /// Nodes = (Heading|HorizontalLine|UnorderedListItem|OrderedListItem|IndentedLine|BlockQuotedLine|BlankLine|TextLine)
    /// </summary>
    public class CstLine : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.Line;
        public CstLine(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstHeading> Heading => new CstNodeFilter<CstHeading> (Children);
        public CstNodeFilter<CstHorizontalLine> HorizontalLine => new CstNodeFilter<CstHorizontalLine> (Children);
        public CstNodeFilter<CstUnorderedListItem> UnorderedListItem => new CstNodeFilter<CstUnorderedListItem> (Children);
        public CstNodeFilter<CstOrderedListItem> OrderedListItem => new CstNodeFilter<CstOrderedListItem> (Children);
        public CstNodeFilter<CstIndentedLine> IndentedLine => new CstNodeFilter<CstIndentedLine> (Children);
        public CstNodeFilter<CstBlockQuotedLine> BlockQuotedLine => new CstNodeFilter<CstBlockQuotedLine> (Children);
        public CstNodeFilter<CstBlankLine> BlankLine => new CstNodeFilter<CstBlankLine> (Children);
        public CstNodeFilter<CstTextLine> TextLine => new CstNodeFilter<CstTextLine> (Children);
    }

    /// <summary>
    /// Rule = OrderedListItem ::= (((Digit)*+'.'+WS+_RECOVER_+Line)+WS)
    /// Nodes = Line
    /// </summary>
    public class CstOrderedListItem : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.OrderedListItem;
        public CstOrderedListItem(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

    /// <summary>
    /// Rule = TextLine ::= ((!(NewLine)+AnyChar+AnyCharUntilNextLine)+WS)
    /// Nodes = 
    /// </summary>
    public class CstTextLine : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.TextLine;
        public CstTextLine(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Rule = UnorderedListItem ::= (([\x2A+\x2D]+WS+_RECOVER_+Line)+WS)
    /// Nodes = Line
    /// </summary>
    public class CstUnorderedListItem : CstNode
    {
        public static Rule Rule = CstNodeFactory.StaticGrammar.UnorderedListItem;
        public CstUnorderedListItem(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstLine> Line => new CstNodeFilter<CstLine> (Children);
    }

}
