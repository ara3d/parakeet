using System;
using System.Collections.Generic;
using System.Linq;

namespace Parakeet
{
    /// <summary>
    /// Used to identify ranges of text such as found in a node or a match.
    /// </summary>
    public class ParserRange : ILocation
    {
        public ParserState Begin { get; }
        public ParserState End { get; }

        public ParserRange(ParserState begin, ParserState end)
        {
            if (begin == null) throw new ArgumentNullException(nameof(begin));
            if (end == null) throw new ArgumentNullException(nameof(end));
            if (end.Input.Text != begin.Input.Text)
                throw new ArgumentException("End range and begin range do not refer to same string", nameof(end));
            if (end.Position < begin.Position) 
                throw new ArgumentException("End of range occurs before the beginning", nameof(end));
            if (end.Input.Text != begin.Input.Text)
                throw new ArgumentException("End range and begin range do not refer to same string", nameof(end));
            Begin = begin;
            End = end;
        }
            
        public int Length => End.Position - Begin.Position;
        public string Text => Begin.Input.Text.Substring(Begin.Position, Length);
        public ParserNode Node => End.Node;
        public IEnumerable<ParserNode> Nodes => Node.AllNodesReversed().Reverse();
    }
}