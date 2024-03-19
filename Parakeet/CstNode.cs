using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ara3D.Parakeet
{
    public class CstNode : ILocation
    {
        public int Count => Children.Count;
        public ILocation Location { get; set; }
        public CstNode this[int index] => Children[index];
        public CstNode(IReadOnlyList<CstNode> children) => Children = children;
        public IReadOnlyList<CstNode> Children { get; }
        public static implicit operator CstNode(string text) => new CstNodeLeaf(text);
        public bool IsLeaf => this is CstNodeLeaf;
        public override string ToString() => $"[{GetType().Name}: {string.Join(" ", Children)}]";

        public string Text
        {
            get
            {
                if (this is CstNodeLeaf leaf) return leaf.Text;
                return string.Join(" ", Children.Select(x => x.Text));
            }
        }
    }

    public class CstNodeSequence : CstNode
    {
        public CstNodeSequence(params CstNode[] children) : base(children)
        {
        }
    }

    public class CstNodeChoice : CstNode
    {
        public CstNodeChoice(params CstNode[] children) : base(children)
        {
        }

        public CstNode Node => Children[0];
    }

    public class CstNodeLeaf : CstNode
    {
        public string Text { get; }
        public CstNodeLeaf(string text) : base(Array.Empty<CstNode>()) => Text = text;
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
    }
}
