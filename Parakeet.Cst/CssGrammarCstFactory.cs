// DO NOT EDIT: Autogenerated file created on 2024-06-29 2:12:45 AM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Cst.CssGrammarNameSpace
{
    public class CstNodeFactory : INodeFactory
    {
        public static CssGrammar StaticGrammar = CssGrammar.Instance;
        public IGrammar Grammar { get; } = StaticGrammar;
        public CstNode Create(ParserTreeNode node)
        {
            switch (node.Type)
            {
                case "Attrib": return new CstAttrib(node, node.Children.Select(Create).ToArray());
                case "AttribOperator": return new CstAttribOperator(node, node.Contents);
                case "AttribValue": return new CstAttribValue(node, node.Contents);
                case "CharSet": return new CstCharSet(node, node.Contents);
                case "Class": return new CstClass(node, node.Contents);
                case "Combinator": return new CstCombinator(node, node.Contents);
                case "CombinedSelector": return new CstCombinedSelector(node, node.Children.Select(Create).ToArray());
                case "Content": return new CstContent(node, node.Children.Select(Create).ToArray());
                case "Contents": return new CstContents(node, node.Children.Select(Create).ToArray());
                case "Declaration": return new CstDeclaration(node, node.Children.Select(Create).ToArray());
                case "Declarations": return new CstDeclarations(node, node.Children.Select(Create).ToArray());
                case "ElementName": return new CstElementName(node, node.Contents);
                case "Expr": return new CstExpr(node, node.Children.Select(Create).ToArray());
                case "Function": return new CstFunction(node, node.Children.Select(Create).ToArray());
                case "HexColor": return new CstHexColor(node, node.Contents);
                case "Identifier": return new CstIdentifier(node, node.Contents);
                case "Import": return new CstImport(node, node.Children.Select(Create).ToArray());
                case "Imports": return new CstImports(node, node.Children.Select(Create).ToArray());
                case "MediaList": return new CstMediaList(node, node.Children.Select(Create).ToArray());
                case "Medium": return new CstMedium(node, node.Contents);
                case "Operator": return new CstOperator(node, node.Contents);
                case "Page": return new CstPage(node, node.Children.Select(Create).ToArray());
                case "PageDeclarations": return new CstPageDeclarations(node, node.Children.Select(Create).ToArray());
                case "Prio": return new CstPrio(node, node.Contents);
                case "Property": return new CstProperty(node, node.Contents);
                case "Pseudo": return new CstPseudo(node, node.Children.Select(Create).ToArray());
                case "PseudoPage": return new CstPseudoPage(node, node.Contents);
                case "RuleSet": return new CstRuleSet(node, node.Children.Select(Create).ToArray());
                case "Selector": return new CstSelector(node, node.Children.Select(Create).ToArray());
                case "SelectorPart": return new CstSelectorPart(node, node.Children.Select(Create).ToArray());
                case "Selectors": return new CstSelectors(node, node.Children.Select(Create).ToArray());
                case "SimpleSelector": return new CstSimpleSelector(node, node.Children.Select(Create).ToArray());
                case "StyleSheet": return new CstStyleSheet(node, node.Children.Select(Create).ToArray());
                case "Term": return new CstTerm(node, node.Children.Select(Create).ToArray());
                case "UnaryOperator": return new CstUnaryOperator(node, node.Contents);
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
