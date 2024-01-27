// DO NOT EDIT: Autogenerated file created on 2024-01-27 2:01:53 AM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Parakeet.Grammars;

namespace Parakeet.Cst.CsvGrammarNameSpace
{
    public class CstNodeFactory
    {
        public static readonly CsvGrammar Grammar = new CsvGrammar();
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
                case "Field": return new CstField(node.Children.Select(Create).ToArray());
                case "File": return new CstFile(node.Children.Select(Create).ToArray());
                case "Identifier": return new CstIdentifier(node.Contents);
                case "Row": return new CstRow(node.Children.Select(Create).ToArray());
                case "String": return new CstString(node.Contents);
                case "Text": return new CstText(node.Contents);
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
