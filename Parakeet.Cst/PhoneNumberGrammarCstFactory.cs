// DO NOT EDIT: Autogenerated file created on 2024-06-03 7:58:23 PM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Cst.PhoneNumberGrammarNameSpace
{
    public class CstNodeFactory : INodeFactory
    {
        public static PhoneNumberGrammar StaticGrammar = PhoneNumberGrammar.Instance;
        public IGrammar Grammar { get; } = StaticGrammar;
        public CstNode Create(ParserTreeNode node)
        {
            switch (node.Type)
            {
                case "AreaCode": return new CstAreaCode(node, node.Children.Select(Create).ToArray());
                case "AreaCodeDigits": return new CstAreaCodeDigits(node, node.Contents);
                case "CountryCode": return new CstCountryCode(node, node.Contents);
                case "Exchange": return new CstExchange(node, node.Contents);
                case "Identifier": return new CstIdentifier(node, node.Contents);
                case "PhoneNumber": return new CstPhoneNumber(node, node.Children.Select(Create).ToArray());
                case "Subscriber": return new CstSubscriber(node, node.Contents);
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
