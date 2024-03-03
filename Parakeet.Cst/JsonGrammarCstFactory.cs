// DO NOT EDIT: Autogenerated file created on 2024-03-02 9:43:36 PM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Cst.JsonGrammarNameSpace
{
    public class CstNodeFactory
    {
        public static readonly JsonGrammar Grammar = new JsonGrammar();
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
                case "Array": return new CstArray(node.Contents);
                case "Constant": return new CstConstant(node.Contents);
                case "Identifier": return new CstIdentifier(node.Contents);
                case "Json": return new CstJson(node.Contents);
                case "Member": return new CstMember(node.Children.Select(Create).ToArray());
                case "Number": return new CstNumber(node.Contents);
                case "Object": return new CstObject(node.Contents);
                case "String": return new CstString(node.Contents);
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
