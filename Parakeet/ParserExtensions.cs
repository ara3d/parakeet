﻿using System.Collections.Generic;
using System.Diagnostics;

namespace Parakeet
{
    public static class ParserExtensions
    {
        public static ParserState Parse(this ParserInput input, Rule r, ParserCache results)
            => r.Match(new ParserState(input), results);

        public static ParserState Parse(this ParserInput input, Rule r)
            => r.Match(new ParserState(input), new ParserCache(input.Length));

        public static ParserState Parse(this string s, Rule r, ParserCache results)
            => r.Match(new ParserState(s), results);

        public static ParserState Parse(this string s, Rule r)
            => s.Parse(r, new ParserCache(s.Length));

        public static ParseNode CreateParseRoot(this ParseNode node)
            => new ParseNode(node.Input, null, 0, node.Input.Length, node);

        public static ParseTree ToParseTree(this ParseNode node)
            => node.ToParseTreeAndNode().Item1;

        public static (ParseTree, ParseNode) ToParseTreeAndNode(this ParseNode node)
        {
            if (node == null) return (null, null);
            var prev = node.Previous;
            var children = new List<ParseTree>();
            while (prev != null && IsAParent(node, prev))
            {
                ParseTree child;
                (child, prev) = ToParseTreeAndNode(prev);
                children.Add(child);
            }
            children.Reverse();
            return (new ParseTree(node, children), prev);
        }

        public static List<ParseNode> AllNodes(this ParseNode node)
        {
            var r = new List<ParseNode>();
            while (node != null)
            {
                r.Add(node);
                node = node.Previous;
            }
            r.Reverse();
            return r;
        }

        public static bool IsAParent(this ParseNode node, ParseNode other)
        {
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