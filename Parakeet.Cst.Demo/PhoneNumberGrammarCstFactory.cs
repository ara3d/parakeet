// DO NOT EDIT: Autogenerated file created on 2024-01-27 2:01:54 AM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Parakeet.Grammars;

namespace Parakeet.Cst.PhoneNumberGrammarNameSpace
{
    public class CstNodeFactory
    {
        public static readonly PhoneNumberGrammar Grammar = new PhoneNumberGrammar();
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
