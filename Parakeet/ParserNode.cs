using System.Collections.Generic;

namespace Ara3D.Parakeet
{
    /// <summary>
    /// A node generated from a "NodeRule". A "NodeRule" will create two nodes
    /// one when it starts (with no previous), and one when it ends.  
    /// These nodes are stored as a linked list, and can be converted 
    /// to a tree representation after all parsing is completed.
    /// </summary>
    public class ParserNode : ILocation
    {
        public readonly ParserRange Range;
        public int Start => Range.BeginPosition;
        public int End => Range.EndPosition;
        public int Length => Range.Length;
        public readonly string Name;
        public readonly ParserNode Previous;
        public string Contents => Range.Text;
        public bool IsBegin => Range.Begin == null;
        public bool IsEnd => Range.Begin != null;

        public ParserRange GetRange()
            => Range;

        public override string ToString()
            => $"({Name}:{Start}-{End}:{EllidedContents} end:{IsEnd})";

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
                if (prev.IsBegin)
                {
                    prev = prev.Previous;
                    continue;
                }
                (child, prev) = prev.ToParseTreeAndNode();
                children.Add(child);
            }
            children.Reverse();
            return (new ParserTreeNode(node, children), prev);
        }

        public IEnumerable<ParserNode> AllEndAllNodesReversed()
        {
            for (var node = this; node != null; node = node.Previous)
                if (node.IsEnd)
                    yield return node;
        }

        public bool IsParentOf(ParserNode other)
            => Start <= other.Start && End >= other.End;
    }
}