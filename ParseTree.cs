using System;
using System.Collections.Generic;
using System.Linq;

namespace Parakeet
{
    public class Node 
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Node()
        {
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public Node(int begin, int id, string input)
        {
            Begin = begin;
            RuleId = id;
            Input = input;
        }

        /// <summary>
        /// Input string used to create AST node.
        /// </summary>
        public string Input;

        /// <summary>
        /// Index where AST content starts within Input.
        /// </summary>
        public int Begin;

        /// <summary>
        /// Index where AST content ends within Input .
        /// </summary>
        public int End;

        /// <summary>
        /// The id of the associated rule.
        /// </summary>
        public int RuleId;

        /// <summary>
        /// List of child nodes. Made public for the transformer. 
        /// TODO: prevent the transformer from accessing this list directly.
        /// </summary>
        public List<Node> _nodes = NoNodes;

        /// <summary>
        /// Default list of no nodes
        /// </summary>
        public static List<Node> NoNodes = new List<Node>();

        /// <summary>
        /// Safe accessor to the list of nodes.
        /// </summary>
        public IReadOnlyList<Node> Nodes => _nodes ?? NoNodes;

        /// <summary>
        /// Length of associated text. 
        /// </summary>
        public int Length { get { return End > Begin ? End - Begin : 0; } }

        /// <summary>
        /// Text associated with the parse result.
        /// </summary>
        public string Text { get { return Input.Substring(Begin, Length); } }

        /// <summary>
        /// Text associated with the parse result.
        /// </summary>
        public string AbbreviatedText { get { return Length > 20 ? Input.Substring(Begin, 20) + "..." : Input.Substring(Begin, Length); } }

        /// <summary>
        /// Indicates whether there are any children nodes or not. 
        /// </summary>
        public bool IsLeaf { get { return Count == 0; } }                    

        /// <summary>
        /// Returns the nth child node.
        /// </summary>
        public Node this[int n]
        {
            get { return _nodes[n]; }
        }

        /// <summary>
        /// Returns the number of child nodes.
        /// </summary>
        public int Count
        {
            get { return Nodes.Count; }
        }

        /// <summary>
        /// Returns a string representation. 
        /// </summary>
        public override string ToString()
        {
            return $"{Rule.Name}:{AbbreviatedText}";
        }

        /// <summary>
        /// Get the Rule that was associated with this node.
        /// </summary>
        public Rule Rule
        {
            get { return Rule.RuleLookup[RuleId]; }
        }

        /// <summary>
        /// Returns all the decenendant nodes.
        /// </summary>
        public IEnumerable<Node> Descendants
        {
            get
            {               
                foreach (var n in Nodes)
                {
                    foreach (var d in n.Descendants)
                        yield return d;
                    yield return n;
                }
            }
        }

        /// <summary>
        /// Adds a node to the tree. 
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Node node)
        {
            if (_nodes == NoNodes)
                _nodes = new List<Node> { node };
            else
                _nodes.Add(node);
        }

        /// <summary>
        /// Returns the name of the associated rule.
        /// </summary>
        public string Label
        {
            get { return Rule.Name; }
        }

        /// <summary>
        /// Returns the first node with the given label
        /// </summary>
        public Node this[string name]
        {
            get { return Nodes.FirstOrDefault(n => n.Label == name); } 
        }
    }
}

