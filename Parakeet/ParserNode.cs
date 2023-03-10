using System.Collections.Generic;
using System.Diagnostics;

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
        public Rule Rule { get; }
        public int Start => Range.Begin.Position;
        public int End => Range.End.Position;
        public string Name => Rule.GetName();
        public ParserNode Previous { get; }
        public string Contents => Range.Text;
        
        public override string ToString()
            => $"Node {Name}:{Start}-{End}:{EllidedContents}";

        public const int MaxLength = 20;

        public string EllidedContents
            => Contents.Length < MaxLength            
            ? Contents : $"{Contents.Substring(0, MaxLength - 1)}...";

        public ParserNode(Rule rule, ParserRange range, ParserNode previous = null)
            => (Rule, Range, Previous) = (rule, range, previous);

        public ParseTree ToParseTree()
            => ToParseTreeAndNode().Item1;

        public (ParseTree, ParserNode) ToParseTreeAndNode()
        {
            var node = this;
            if (node == null) return (null, null);
            var prev = node.Previous;
            var children = new List<ParseTree>();
            while (prev != null && node.IsParentOf(prev))
            {
                ParseTree child;
                (child, prev) = prev.ToParseTreeAndNode();
                children.Add(child);
            }
            children.Reverse();
            return (new ParseTree(node, children), prev);
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
        {
            var node = this;
            if (node == null || other == null) return false;
            if (other.Start >= node.End) return false;
            if (other.End <= node.Start) return false;

            // In this case it was a child
            if (other.Start >= node.Start)
            {
                Debug.Assert(node.End >= node.End);
                return true;
            }

            // Otherwise it was a sibling
            Debug.Assert(other.End <= node.Start);
            return false;
        }
    }
}