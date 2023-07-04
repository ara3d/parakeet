using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parakeet
{
    public interface ITree<T> where T : ITree<T>
    {
        IReadOnlyList<T> Children { get; }
    }

    public static class TreeHelpers
    {
        public static TAcc Aggregate<T, TAcc>(this ITree<T> tree, TAcc init, Func<TAcc, T, TAcc> func)
            where T : ITree<T>
            => tree.Children.Aggregate(init, (acc, curr) => curr.Aggregate(acc, func));
    }

    public class CstNode : ITree<CstNode>
    {
        public int Count => Children.Count;
        public CstNode this[int index] => Children[index];
        public CstNode(IReadOnlyList<CstNode> children) => Children = children;
        public IReadOnlyList<CstNode> Children { get; }
        public static implicit operator CstNode(string text) => new CstLeaf(text);
        public bool IsLeaf => this is CstLeaf;
        public override string ToString() => $"[{GetType().Name}: {string.Join(" ", Children)}]";

        public string Text
        {
            get
            {
                if (this is CstLeaf leaf) return leaf.Text;
                return string.Join(" ", Children.Select(x => x.Text));
            }
        }
    }

    public class CstSequence : CstNode
    {
        public CstSequence(params CstNode[] children) : base(children)
        {
        }
    }

    public class CstChoice : CstNode
    {
        public CstChoice(params CstNode[] children) : base(children)
        {
        }

        public CstNode Node => Children[0];
    }

    public class CstLeaf : CstNode
    {
        public string Text { get; }
        public CstLeaf(string text) : base(Array.Empty<CstNode>()) => Text = text;
        public override string ToString() => Text;
    }


    public class CstFilter : CstNode
    {
        public CstFilter(IReadOnlyList<CstNode> children) : base(children)
        {
        }
    }

    public class CstFilter<T> : CstFilter
    {
        public IReadOnlyList<T> Nodes => Children.OfType<T>().ToList();

        public CstFilter(IReadOnlyList<CstNode> children) : base(children)
        {
        }

        public T Node => Nodes.FirstOrDefault();
        public bool Present => Node != null;
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
