using System.Collections.Generic;

namespace Parakeet
{
    /// <summary>
    /// Created from parse nodes. 
    /// </summary>
    public class ParserTreeNode
    {
        public ParserNode Node { get; }
        public string Type => Node.Name;
        public IReadOnlyList<ParserTreeNode> Children { get; }
        public ParserTreeNode(ParserNode node, IReadOnlyList<ParserTreeNode> children)
            => (Node, Children) = (node, children);
        public string Contents => Node.Contents;
        public override string ToString()
            => $"({Type} {string.Join(" ", Children)})";
    }
}