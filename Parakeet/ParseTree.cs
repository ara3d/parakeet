﻿using System.Collections.Generic;

namespace Parakeet
{
    /// <summary>
    /// Created from parse nodes. 
    /// </summary>
    public class ParseTree
    {
        public ParseNode Node { get; }
        public string Type => Node.Name;
        public IReadOnlyList<ParseTree> Children { get; }
        public ParseTree(ParseNode node, IReadOnlyList<ParseTree> children)
            => (Node, Children) = (node, children);
        public string Contents => Node.Contents;
        public override string ToString()
            => $"({Type} {string.Join(" ", Children)})";
    }
}