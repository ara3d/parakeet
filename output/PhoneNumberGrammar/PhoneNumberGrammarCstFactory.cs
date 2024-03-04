// DO NOT EDIT: Autogenerated file created on 2024-01-26 1:43:40 AM. 
using System;
using System.Linq;
using Ara3D.Parsing;
using System.Collections.Generic;

namespace Parakeet.Cst.PhoneNumberGrammar
{
    public class CstNodeFactory
    {
        public static Parakeet.Cst.PhoneNumberGrammarGrammar Grammar = new Parakeet.Cst.PhoneNumberGrammarGrammar();
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
                case "Identifier": return new CstIdentifier(node.Contents);
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}