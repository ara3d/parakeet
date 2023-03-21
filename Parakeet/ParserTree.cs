using System.Collections.Generic;

namespace Parakeet
{
    /// <summary>
    /// Created from parse nodes. 
    /// </summary>
    public class ParserTree
    {
        public ParserNode Node { get; }
        public string Type => Node.Name;
        public IReadOnlyList<ParserTree> Children { get; }
        public ParserTree(ParserNode node, IReadOnlyList<ParserTree> children)
            => (Node, Children) = (node, children);
        public string Contents => Node.Contents;
        public override string ToString()
            => $"({Type} {string.Join(" ", Children)})";
    }
}