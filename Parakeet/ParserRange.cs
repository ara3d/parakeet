using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Utils;

namespace Ara3D.Parakeet
{
    /// <summary>
    /// Used to identify ranges of text such as found in a node or a match.
    /// </summary>
    public class ParserRange : ILocation
    {
        // May be null
        public readonly ParserState Begin;

        // Cannot be null 
        public readonly ParserState End;

        public ParserRange(ParserState begin, ParserState end)
        {
            Begin = begin;
            End = end;
            if (end == null) 
                throw new ArgumentNullException(nameof(end));
            if (end.Position < BeginPosition) 
                throw new ArgumentException("End of range occurs before the beginning", nameof(end));
        }

        public ParserRange GetRange()
            => this;

        public string InputText => End.Input.Text;
        public int BeginPosition => Begin?.Position ?? EndPosition;
        public int EndPosition => End.Position;
        public int Length => End.Position - BeginPosition;
        public string Text => InputText.Substring(BeginPosition, Length);
        public ParserNode Node => End.Node;
        public IEnumerable<ParserNode> Nodes => Node.AllEndAllNodesReversed().Reverse();

        public int BeginLineIndex => Begin?.LineIndex ?? 0;
        public int BeginColumn => Begin?.Column ?? 0;
        public int EndLineIndex => End.LineIndex;
        public int EndColumn => End.Column;

        public override string ToString()
            => $"({BeginLineIndex},{BeginColumn},{EndLineIndex},{EndColumn})";

        public static ParserRange Create(ParserState begin, ParserState end)
            => new ParserRange(begin, end);

        public ParserInput Input 
            => End.Input;

        public FilePath FilePath
            => Input?.File;
    }
}