// DO NOT EDIT: Autogenerated file created on 2024-03-02 9:43:36 PM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Cst.JoyGrammarNameSpace
{
    public class CstNodeFactory
    {
        public static readonly JoyGrammar Grammar = new JoyGrammar();
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
                case "Def": return new CstDef(node.Children.Select(Create).ToArray());
                case "Expr": return new CstExpr(node.Children.Select(Create).ToArray());
                case "Identifier": return new CstIdentifier(node.Contents);
                case "Literal": return new CstLiteral(node.Contents);
                case "Operator": return new CstOperator(node.Children.Select(Create).ToArray());
                case "Quotation": return new CstQuotation(node.Children.Select(Create).ToArray());
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
