// DO NOT EDIT: Autogenerated file created on 2024-03-02 9:43:36 PM. 
using System;
using System.Linq;

namespace Ara3D.Parakeet.Cst.MarkdownInlineGrammarNameSpace
{
    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstAltText : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.AltText;
        public CstAltText(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (InnerText)*
    /// </summary>
    public class CstBold : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Bold;
        public CstBold(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerText> InnerText => new CstNodeFilter<CstInnerText> (Children);
    }

    /// <summary>
    /// Nodes = (InnerText)*
    /// </summary>
    public class CstBoldAndItalic : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.BoldAndItalic;
        public CstBoldAndItalic(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerText> InnerText => new CstNodeFilter<CstInnerText> (Children);
    }

    /// <summary>
    /// Nodes = (InnerText)*
    /// </summary>
    public class CstCode : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Code;
        public CstCode(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerText> InnerText => new CstNodeFilter<CstInnerText> (Children);
    }

    /// <summary>
    /// Nodes = (InnerText)*
    /// </summary>
    public class CstContent : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Content;
        public CstContent(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerText> InnerText => new CstNodeFilter<CstInnerText> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstEmail : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Email;
        public CstEmail(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Email
    /// </summary>
    public class CstEmailLink : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.EmailLink;
        public CstEmailLink(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstEmail> Email => new CstNodeFilter<CstEmail> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstEscapedChar : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.EscapedChar;
        public CstEscapedChar(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstHtmlTag : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.HtmlTag;
        public CstHtmlTag(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstIdentifier : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Identifier;
        public CstIdentifier(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (AltText+Url+(UrlTitle)?)
    /// </summary>
    public class CstImg : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Img;
        public CstImg(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstAltText> AltText => new CstNodeFilter<CstAltText> (Children);
        public CstNodeFilter<CstUrl> Url => new CstNodeFilter<CstUrl> (Children);
        public CstNodeFilter<CstUrlTitle> UrlTitle => new CstNodeFilter<CstUrlTitle> (Children);
    }

    /// <summary>
    /// Nodes = (BoldAndItalic|Strikethrough|Bold|Italic|Code|Link|Img|EmailLink|UrlLink|HtmlTag|EscapedChar|PlainText)
    /// </summary>
    public class CstInnerText : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.InnerText;
        public CstInnerText(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstBoldAndItalic> BoldAndItalic => new CstNodeFilter<CstBoldAndItalic> (Children);
        public CstNodeFilter<CstStrikethrough> Strikethrough => new CstNodeFilter<CstStrikethrough> (Children);
        public CstNodeFilter<CstBold> Bold => new CstNodeFilter<CstBold> (Children);
        public CstNodeFilter<CstItalic> Italic => new CstNodeFilter<CstItalic> (Children);
        public CstNodeFilter<CstCode> Code => new CstNodeFilter<CstCode> (Children);
        public CstNodeFilter<CstLink> Link => new CstNodeFilter<CstLink> (Children);
        public CstNodeFilter<CstImg> Img => new CstNodeFilter<CstImg> (Children);
        public CstNodeFilter<CstEmailLink> EmailLink => new CstNodeFilter<CstEmailLink> (Children);
        public CstNodeFilter<CstUrlLink> UrlLink => new CstNodeFilter<CstUrlLink> (Children);
        public CstNodeFilter<CstHtmlTag> HtmlTag => new CstNodeFilter<CstHtmlTag> (Children);
        public CstNodeFilter<CstEscapedChar> EscapedChar => new CstNodeFilter<CstEscapedChar> (Children);
        public CstNodeFilter<CstPlainText> PlainText => new CstNodeFilter<CstPlainText> (Children);
    }

    /// <summary>
    /// Nodes = (InnerText)*
    /// </summary>
    public class CstItalic : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Italic;
        public CstItalic(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerText> InnerText => new CstNodeFilter<CstInnerText> (Children);
    }

    /// <summary>
    /// Nodes = (InnerText+Url+UrlTitle)
    /// </summary>
    public class CstLink : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Link;
        public CstLink(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerText> InnerText => new CstNodeFilter<CstInnerText> (Children);
        public CstNodeFilter<CstUrl> Url => new CstNodeFilter<CstUrl> (Children);
        public CstNodeFilter<CstUrlTitle> UrlTitle => new CstNodeFilter<CstUrlTitle> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstPlainText : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.PlainText;
        public CstPlainText(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstPlainTextUrl : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.PlainTextUrl;
        public CstPlainTextUrl(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (InnerText)*
    /// </summary>
    public class CstStrikethrough : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Strikethrough;
        public CstStrikethrough(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerText> InnerText => new CstNodeFilter<CstInnerText> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstUrl : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Url;
        public CstUrl(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (Url|PlainTextUrl)
    /// </summary>
    public class CstUrlLink : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.UrlLink;
        public CstUrlLink(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstUrl> Url => new CstNodeFilter<CstUrl> (Children);
        public CstNodeFilter<CstPlainTextUrl> PlainTextUrl => new CstNodeFilter<CstPlainTextUrl> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstUrlTitle : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.UrlTitle;
        public CstUrlTitle(string text) : base(text) { }
        // No children
    }

}
