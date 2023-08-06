// DO NOT EDIT: Autogenerated file created on 2023-08-06 4:17:11 PM. 
using System;
using System.Linq;
using Parakeet;
using System.Collections.Generic;

namespace Parakeet.Demos.Json
{
    public class CstNodeFactory
    {
        public static Parakeet.Demos.JsonGrammar Grammar = new Parakeet.Demos.JsonGrammar();
        public Dictionary<ParserTreeNode, CstNode> Lookup { get;} = new Dictionary<ParserTreeNode, CstNode>();
        public CstNode Create(ParserTreeNode node)
        {
            var r = InternalCreate(node);
            Lookup.Add(node, r);
            return r;
        }
        public CstNode InternalCreate(ParserTreeNode node)
        {
            switch (node.Type)
            {
                case "Number": return new CstNumber(node.Contents);
                case "String": return new CstString(node.Contents);
                case "Constant": return new CstConstant(node.Contents);
                case "Array": return new CstArray(node.Contents);
                case "Member": return new CstMember(node.Children.Select(Create).ToArray());
                case "Object": return new CstObject(node.Contents);
                case "Element": return new CstElement(node.Children.Select(Create).ToArray());
                case "Json": return new CstJson(node.Children.Select(Create).ToArray());
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
