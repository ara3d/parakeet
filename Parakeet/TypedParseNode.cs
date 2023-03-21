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

    public class TypedParseNode : ITree<TypedParseNode>
    {
        public int Count => Children.Count;
        public TypedParseNode this[int index] => Children[index];
        public TypedParseNode(IReadOnlyList<TypedParseNode> children) => Children = children;
        public IReadOnlyList<TypedParseNode> Children { get; }
        public virtual TypedParseNode Transform(Func<TypedParseNode, TypedParseNode> f) => throw new NotImplementedException();
        public static implicit operator TypedParseNode(string text) => new TypedParseLeaf(text);
        public bool IsLeaf => this is TypedParseLeaf;
        public override string ToString() => $"[{GetType().Name}: {string.Join(" ", Children)}]";
    }

    public class TypedParseSequence : TypedParseNode
    {
        public TypedParseSequence(params TypedParseNode[] children) : base(children) { }
    }

    public class TypedParseChoice: TypedParseNode
    {
        public TypedParseChoice(params TypedParseNode[] children) : base(children) { }
        public TypedParseNode Node => Children[0];
    }

    public class TypedParseLeaf : TypedParseNode
    {
        public string Text { get; }
        public TypedParseLeaf(string text) : base(Array.Empty<TypedParseNode>()) => Text = text;
        public override string ToString() => Text;
    }

    public class TypedParseZeroOrMore<T> : TypedParseNode where T : TypedParseNode
    {
        public new T this[int index] => (T)Children[index];
        public TypedParseZeroOrMore(T node) : base(new[] { node }) { }
    }
   
    public class TypedParseOptional<T> : TypedParseNode where T : TypedParseNode
    {
        public T Node => (T)Children[0];
        public TypedParseOptional(T node) : base(new[] { node }) { }
        public static implicit operator T(TypedParseOptional<T> self) => self.Node;
    }
}
