// DO NOT EDIT: Autogenerated file created on 2024-03-03 10:34:02 PM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Cst.MarkdownBlockGrammarNameSpace
{
    public class CstNodeFactory : INodeFactory
    {
        public static MarkdownBlockGrammar StaticGrammar = MarkdownBlockGrammar.Instance;
        public IGrammar Grammar { get; } = StaticGrammar;
        public Dictionary<CstNode, ParserTreeNode> Lookup { get;} = new Dictionary<CstNode, ParserTreeNode>();
        public CstNode Create(ParserTreeNode node)
        {
            var r = InternalCreate(node);
            Lookup.Add(r, node);
            return r;
        }
        public CstNode InternalCreate(ParserTreeNode node)
        {
            switch (node.Type)
            {
                case "BlankLine": return new CstBlankLine(node.Contents);
                case "Block": return new CstBlock(node.Children.Select(Create).ToArray());
                case "BlockQuotedLine": return new CstBlockQuotedLine(node.Children.Select(Create).ToArray());
                case "CodeBlock": return new CstCodeBlock(node.Children.Select(Create).ToArray());
                case "CodeBlockLang": return new CstCodeBlockLang(node.Children.Select(Create).ToArray());
                case "CodeBlockText": return new CstCodeBlockText(node.Contents);
                case "Comment": return new CstComment(node.Contents);
                case "Document": return new CstDocument(node.Children.Select(Create).ToArray());
                case "H1Underline": return new CstH1Underline(node.Contents);
                case "H2Underline": return new CstH2Underline(node.Contents);
                case "Heading": return new CstHeading(node.Children.Select(Create).ToArray());
                case "HeadingOperator": return new CstHeadingOperator(node.Contents);
                case "HeadingUnderlined": return new CstHeadingUnderlined(node.Children.Select(Create).ToArray());
                case "HeadingWithOperator": return new CstHeadingWithOperator(node.Children.Select(Create).ToArray());
                case "HorizontalLine": return new CstHorizontalLine(node.Contents);
                case "Identifier": return new CstIdentifier(node.Contents);
                case "Indent": return new CstIndent(node.Contents);
                case "IndentedLine": return new CstIndentedLine(node.Children.Select(Create).ToArray());
                case "Line": return new CstLine(node.Children.Select(Create).ToArray());
                case "OrderedListItem": return new CstOrderedListItem(node.Children.Select(Create).ToArray());
                case "TextLine": return new CstTextLine(node.Contents);
                case "UnorderedListItem": return new CstUnorderedListItem(node.Children.Select(Create).ToArray());
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
