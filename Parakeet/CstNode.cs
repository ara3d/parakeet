using System;
using System.Collections.Generic;
using System.Linq;

namespace Parakeet
{
    public interface ITree<T> where T:ITree<T>
    {
        IReadOnlyList<T> Children { get; } 
    }

    public static class TreeHelpers
    {
        public static TAcc Aggregate<T, TAcc>(this ITree<T> tree, TAcc init, Func<TAcc, T, TAcc> func) where T:ITree<T>
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

        public string GetText()
        {
            if (this is CstLeaf leaf) return leaf.Text;
            return string.Join(" ", Children.Select(x => x.GetText()));
        }
    }

    public class CstSequence : CstNode
    {
        public CstSequence(params CstNode[] children) : base(children) { }
    }

    public class CstChoice: CstNode
    {
        public CstChoice(params CstNode[] children) : base(children) { }
        public CstNode Node => Children[0];
    }

    public class CstLeaf : CstNode
    {
        public string Text { get; }
        public CstLeaf(string text) : base(Array.Empty<CstNode>()) => Text = text;
        public override string ToString() => Text;
    }

    public class CstFilter<T> : CstNode where T : CstNode
    {
        public IReadOnlyList<T> Nodes => Children.OfType<T>().ToList();
        public CstFilter(IReadOnlyList<CstNode> children) : base(children) { }
        public T Node => Nodes.FirstOrDefault();
        public bool Present => Node != null;
    }

    public class CstZeroOrMore<T> : CstFilter<T> where T : CstNode
    {
        public new T this[int index] => (T)Children[index];
        public CstZeroOrMore(IReadOnlyList<CstNode> children) : base(children) { }
    }

    public class CstOneOrMore<T> : CstFilter<T> where T : CstNode
    {
        public CstOneOrMore(IReadOnlyList<T> children) : base(children)
        { if (Nodes.Count < 1) throw new Exception($"Expected at least one child of type {typeof(T)}"); }
    }

    public class CstOptional<T> : CstFilter<T> where T : CstNode
    {
        public CstOptional(IReadOnlyList<CstNode> children) : base(children) { }
        public static implicit operator T(CstOptional<T> self) => self.Node;
    }

    public class CstChoice<T> : CstFilter<T> where T : CstNode
    {
        public CstChoice(IReadOnlyList<CstNode> children) : base(children) { }
        public static implicit operator T(CstChoice<T> self) => self.Node;
    }
}
