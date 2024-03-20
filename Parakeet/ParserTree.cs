using System.Collections.Generic;

namespace Ara3D.Parakeet
{
    /// <summary>
    /// Created from parse nodes. 
    /// </summary>
    public class ParserTreeNode : ILocation
    {
        public readonly ParserNode Node;
        public string Type => Node.Name;
        public IReadOnlyList<ParserTreeNode> Children { get; }
        public ParserTreeNode(ParserNode node, IReadOnlyList<ParserTreeNode> children)
            => (Node, Children) = (node, children);
        public string Contents => Node.Contents;
        public override string ToString()
            => $"({Type} {string.Join(" ", Children)})";

        public ParserRange GetRange()
            => Node.GetRange();
    }
}