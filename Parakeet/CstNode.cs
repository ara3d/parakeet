using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ara3D.Parakeet
{
    public class CstNode : ILocation
    {
        public ILocation Location { get; }
        public IReadOnlyList<CstNode> Children { get; }

        public ParserRange GetRange() 
            => Location?.GetRange();
        
        public int Count 
            => Children.Count;
        
        public CstNode this[int index] 
            => Children[index];

        public CstNode(ILocation location, IReadOnlyList<CstNode> children) 
            => (Location, Children) = (location, children);
        
        public bool IsLeaf 
            => this is CstNodeLeaf;
        
        public override string ToString() 
            => $"[{GetType().Name}: {string.Join(" ", Children)}]";

        public string Text
            => this is CstNodeLeaf leaf 
                ? leaf.Text 
                : string.Join(" ", Children.Select(x => x.Text));
    }

    public class CstNodeSequence : CstNode
    {
        public CstNodeSequence(ILocation location, params CstNode[] children) : base(location, children)
        {
        }
    }

    public class CstNodeChoice : CstNode
    {
        public CstNodeChoice(ILocation location, params CstNode[] children) : base(location, children)
        {
        }

        public CstNode Node => Children[0];
    }

    public class CstNodeLeaf : CstNode
    {
        public string Text { get; }
        public CstNodeLeaf(ILocation location, string text) : base(location, Array.Empty<CstNode>()) => Text = text;
        public override string ToString() => Text;
    }

    public class CstNodeFilter<T> 
        where T: CstNode
    {
        public T Node => Nodes.FirstOrDefault();
        public bool Present => Node != null;
        public int Count => Nodes.Count;
        public CstNode this[int index] => Nodes[index];
        public IReadOnlyList<T> Nodes { get; }
        public CstNodeFilter(IReadOnlyList<CstNode> nodes)
            => Nodes = nodes.OfType<T>().ToList();
    }

    public static class CstExtensions
    {
        public static StringBuilder ToXml(this CstNode node, StringBuilder sb = null, string indent = "")
        {
            sb = sb ?? new StringBuilder();
            var xs = node.GetType().Name;
            if (node.IsLeaf)
            {
                sb.AppendLine($"{indent}<{xs}>{node.Text}</{xs}>");
            }
            else
            {
                sb.AppendLine($"{indent}<{xs}>");
                foreach (var child in node.Children)
                    ToXml(child, sb, indent + "  ");
                sb.AppendLine($"{indent}</{xs}>");
            }

            return sb;
        }

        public static IEnumerable<CstNode> Descendants(this CstNode node)
        {
            yield return node;
            foreach (var c in node.Children)
            foreach (var d in c.Descendants())
                yield return d;
        }
    }
}
