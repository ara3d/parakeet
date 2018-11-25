using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Parakeet
{
    public struct ParserInternalState
    {
        public ParserInternalState(int pos, int nodeCount)
        {
            Pos = pos;
            NodeCount = nodeCount;
        }
        public readonly int Pos;
        public readonly int NodeCount;
    }

    public struct ParseNode
    {
        public int RuleId { get { return _ruleId >= 0 ? _ruleId : -_ruleId; } }
        public bool IsEnd {  get { return _ruleId < 0; } }
        public readonly int Pos;
        readonly int _ruleId;
        public Rule Rule
        {
            get { return Rule.RuleLookup[RuleId]; }
        }
        public ParseNode(int ruleId, int pos)
        {
            _ruleId = ruleId;
            Pos = pos;
        }
    }

    public class ParserState
    {
        public string Input;
        public int Pos;
        public List<ParseNode> Nodes = new List<ParseNode>();
        public readonly Dictionary<string, int> ruleIds = new Dictionary<string, int>();
        public readonly Stack<Rule> Rules = new Stack<Rule>();
        public int NodeCount;
        
        public List<ParseNode> AllParseNodes()
        {
            if (Nodes.Count > NodeCount)
            {
                Nodes.RemoveRange(NodeCount, Nodes.Count - NodeCount);
            }
            return Nodes;
        }

        public ParserInternalState GetState()
        {
            return new ParserInternalState(Pos, NodeCount);
        }

        public void RestoreState(ParserInternalState state)
        {
            Pos = state.Pos;
            NodeCount = state.NodeCount;
        }

        public string Context
        {
            get
            {
                //int before = Pos - 5;
                //if (before < 0) before = 0;
                var before = Pos;
                var len = 20;
                if (before + len > Input.Length)
                    len = Input.Length - before;
                return Input.Substring(before, len);
            }
        }

        public override string ToString()
        {
            return $"{Pos}/{Input.Length}: {Context}";
        }
            
        public ParserInternalState AddNode(Rule r, bool beginOrEnd)
        {
            var state = GetState();
            var n = NodeCount++;
            Debug.Assert(n <= Nodes.Count, "NodeCount should never be more than the number of valid nodes");
            var index = beginOrEnd ? r.Index : -r.Index;
            var node = new ParseNode(index, Pos);
            if (n == Nodes.Count)
                Nodes.Add(node);
            else
                Nodes[n] = node;
            return state;
        }
    }
}
