using System.Collections.Generic;

namespace Parakeet
{
    /// <summary>
    /// A node generated from successful "NodeRule" parses. 
    /// These nodes are stored as a linked list, and can be converted 
    /// to a tree representation after all parsing is completed. 
    /// </summary>
    public class ParserNode
    {
        public ParserRange Range { get; }
        public int Start => Range.Begin.Position;
        public int End => Range.End.Position;
        public int Length => Range.Length;
        public string Name { get; }
        public ParserNode Previous { get; }
        public string Contents => Range.Text;
        
        public override string ToString()
            => $"Node {Name}:{Start}-{End}:{EllidedContents}";

        public const int MaxLength = 20;

        public string EllidedContents
            => Contents.Length < MaxLength            
            ? Contents : $"{Contents.Substring(0, MaxLength - 1)}...";

        public ParserNode(string name, ParserRange range, ParserNode previous = null)
            => (Name, Range, Previous) = (name, range, previous);

        public ParserTreeNode ToParseTree()
            => ToParseTreeAndNode().Item1;

        public (ParserTreeNode, ParserNode) ToParseTreeAndNode()
        {
            var node = this;
            var prev = node.Previous;
            var children = new List<ParserTreeNode>();
            while (prev != null && node.IsParentOf(prev))
            {
                ParserTreeNode child;
                (child, prev) = prev.ToParseTreeAndNode();
                children.Add(child);
            }
            children.Reverse();
            return (new ParserTreeNode(node, children), prev);
        }

        public IEnumerable<ParserNode> AllNodesReversed()
        {
            for (var node = this; node != null; node = node.Previous)
                yield return node;
        }

        public IEnumerable<ParserNode> SelfAndSiblings()
        {
            yield return this;
            var current = this;
            foreach (var node in AllNodesReversed())
            {
                if (node.End < current.Start)
                {
                    current = node;
                    yield return current;
                }
            }
        }

        public bool IsChildOf(ParserNode parent)
            => parent.IsParentOf(this);

        public bool IsParentOf(ParserNode other)
            => Start <= other.Start && End >= other.End;
    }
}