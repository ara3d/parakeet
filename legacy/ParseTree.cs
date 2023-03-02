using System.Collections.Generic;
using System.Linq;

namespace Parakeet
{
    /// <summary>
    /// This is the data structure stored every time a parse node is created. 
    /// </summary>
    public struct ParseNode
    {
        public int RuleId;
        public int Pos;
        public int Next;
        public int End;
        public ParseNode(int ruleId, int pos)
        {
            RuleId = ruleId;
            Pos = pos;
            Next = -1;
            End = pos;
        }
        public Rule Rule => Rule.RuleLookup[RuleId];
        public int Length => End - Pos;
    }

    /// <summary>
    /// Encapsulates the list of nodes, and the input string.
    /// </summary>
    public class ParseTree
    {
        public readonly IReadOnlyList<ParseNode> Nodes;
        public readonly string Input;

        public ParseTree(string input, IReadOnlyList<ParseNode> nodes)
        {
            Input = input;
            Nodes = nodes;
        }

        public IEnumerable<Node> GetRoots()
        {
            var node = new Node(this, 0);
            while (node.IsValid)
            {
                yield return node;
                node = node.Sibling;
            }            
        }
    }

    /// <summary>
    /// This is a Node in the parse tree, intended to be consumed by the users.
    /// </summary>
    public readonly struct Node
    {
        public readonly ParseTree Tree;
        public readonly int Id;

        public Node(ParseTree tree, int id)
        {
            Tree = tree;
            Id = id;
        }

        /// <summary>
        /// Returns child nodes
        /// </summary>
        public IEnumerable<Node> Nodes
        {
            get
            {
                var id = ParseNode.Next;
                var tmp = FirstChild;
                while (tmp.IsValid && tmp.Id != id)
                {
                    yield return tmp;
                    tmp = tmp.Sibling;
                }
            }
        }

        /// <summary>
        /// Returns true if this node has any children
        /// </summary>
        public bool HasChild => ParseNode.Next != Id + 1;

        /// <summary>
        /// Returns the first child in the list (could be invalid, or the sibling) 
        /// </summary>
        public Node FirstChild => new Node(Tree, Id + 1);

        /// <summary>
        /// Returns the low-level data structure used in the list.
        /// </summary>
        public ParseNode ParseNode => Tree.Nodes[Id];

        /// <summary>
        /// Returns true if this node points to a valid ParseNode
        /// </summary>
        public bool IsValid => Id >= 0 && Id < Tree.Nodes.Count;

        /// <summary>
        /// Returns the next sibling
        /// </summary>
        public Node Sibling => new Node(Tree, ParseNode.Next);

        /// <summary>
        /// Length of associated text. 
        /// </summary>
        public int Length => ParseNode.Length;

        /// <summary>
        /// Text associated with the parse result.
        /// </summary>
        public string Text => Tree.Input.Substring(ParseNode.Pos, Length); 

        /// <summary>
        /// Returns the nth child node.
        /// </summary>
        public Node this[int n] => Nodes.ElementAt(n);

        /// <summary>
        /// Returns the number of child nodes.
        /// </summary>
        public int Count => Nodes.Count();

        /// <summary>
        /// Returns a string representation. 
        /// </summary>
        public override string ToString()
        {
            return $"{Rule.Name}:{Text}";
        }

        /// <summary>
        /// Get the Rule that was associated with this node.
        /// </summary>
        public Rule Rule => Rule.RuleLookup[ParseNode.RuleId];

        /// <summary>
        /// Returns the name of the associated rule.
        /// </summary>
        public string Label => Rule.Name;        

        /// <summary>
        /// Returns the first node with the given label
        /// </summary>
        public Node this[string name]
        {
            get { return Nodes.FirstOrDefault(n => n.Label == name); } 
        }

        /// <summary>
        /// Compares whether the two nodes point to the same structure. 
        /// </summary>
        public bool Equals(Node other)
        {
            return other.Id == Id;
        }

        /// <summary>
        /// Returns true if this node is associated with the given rule
        /// </summary>
        public bool IsRule(Rule r)
        {
            return ParseNode.RuleId == r.Id;
        }
    }
}

