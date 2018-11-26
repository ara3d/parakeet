using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Parakeet
{
    public unsafe readonly struct ParserInternalState
    {
        public ParserInternalState(char* ptr, int nodeCount)
        {
            Ptr = ptr;
            NodeCount = nodeCount;
        }
        public readonly char* Ptr;
        public readonly int NodeCount;
    }

    public readonly struct ParseNode
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

    public unsafe class ParserState
    {
        public string Input;
        public GCHandle Handle;
        public char* Ptr;
        public char* End;
        public char* Begin;
        public List<ParseNode> Nodes = new List<ParseNode>();
        public readonly Dictionary<string, int> ruleIds = new Dictionary<string, int>();
        public readonly Stack<Rule> Rules = new Stack<Rule>();
        public int NodeCount;

        public ParserState(string input)
        {
            Input = input;
            Handle = GCHandle.Alloc(Input, GCHandleType.Pinned);
            Begin = (char*)Handle.AddrOfPinnedObject();
            End = Begin + Input.Length;
            Ptr = Begin;
        }
        
        public bool AtEnd
        {
            get { return Ptr >= End; }
        }

        public void Advance()
        {
            Ptr++;
        }
        
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
            return new ParserInternalState(Ptr, NodeCount);
        }

        public void RestoreState(in ParserInternalState state)
        {
            Ptr = state.Ptr;
            NodeCount = state.NodeCount;
        }

        public int Index
        {
            get { return (int)(Ptr - Begin); }
        }

        public string Context
        {
            get
            {
                //int before = Pos - 5;
                //if (before < 0) before = 0;
                var before = Index;
                var len = 20;
                if (before + len > Input.Length)
                    len = Input.Length - before;
                return Input.Substring(before, len);
            }
        }

        public override string ToString()
        {
            return $"{Index}/{Input.Length}: {Context}";
        }
            
        public ParserInternalState AddNode(Rule r, bool beginOrEnd)
        {
            var state = GetState();
            var n = NodeCount++;
            Debug.Assert(n <= Nodes.Count, "NodeCount should never be more than the number of valid nodes");
            var index = beginOrEnd ? r.Index : -r.Index;
            var node = new ParseNode(index, Index);
            if (n == Nodes.Count)
                Nodes.Add(node);
            else
                Nodes[n] = node;
            return state;
        }
    }
}
