// DO NOT EDIT: Autogenerated file created on 2024-01-26 1:43:40 AM. 
using System;
using System.Linq;
using Ara3D.Parsing;
using System.Collections.Generic;

namespace Parakeet.Cst.SExpressionGrammar
{
    public class CstNodeFactory
    {
        public static Parakeet.Cst.SExpressionGrammarGrammar Grammar = new Parakeet.Cst.SExpressionGrammarGrammar();
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
                case "Atom": return new CstAtom(node.Children.Select(Create).ToArray());
                case "Document": return new CstDocument(node.Children.Select(Create).ToArray());
                case "Expr": return new CstExpr(node.Children.Select(Create).ToArray());
                case "Identifier": return new CstIdentifier(node.Contents);
                case "List": return new CstList(node.Children.Select(Create).ToArray());
                case "RecExpr": return new CstRecExpr(node.Children.Select(Create).ToArray());
                case "Document": return new CstDocument(node.Children.Select(Create).ToArray());
                case "Symbol": return new CstSymbol(node.Contents);
                case "SymbolWithSpaces": return new CstSymbolWithSpaces(node.Contents);
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
