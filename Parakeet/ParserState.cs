using System.Runtime.CompilerServices;

namespace Parakeet
{
    /// <summary>
    /// A class that represents the state of the parser, and the parse tree. 
    /// </summary>
    public class ParserState
    {
        public ParserInput Input { get; }
        public int Position { get; }
        public ParserNode Node { get; }
        public ParserError LastError { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AtEnd() 
            => Position >= Input.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char GetCurrent() 
            => Input[Position];

        public ParserState(ParserInput input, int position = 0, ParserNode node = null, ParserError error = null)
            => (Input, Position, Node, LastError) = (input, position, node, error);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserState Advance(int amount)
            => new ParserState(Input, Position + amount, Node,  LastError);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserState Advance()
            => new ParserState(Input, Position + 1, Node, LastError);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserState AdvanceIf(char c)
            => AtEnd() ? null : GetCurrent() == c ? Advance() : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserState AdvanceIfNotAtEnd()
            => AtEnd() ? null : new ParserState(Input, Position + 1, Node, LastError);

        public ParserState JumpToEnd()
            => Advance(CharsLeft);

        public override string ToString()
            => $"Parse state: line {CurrentLineIndex} column {CurrentColumn} position {Position}/{Input.Length} node = {Node}";

        public int CurrentLineIndex
            => Input.GetLineIndex(Position);

        public int CurrentColumn
            => Input.GetColumn(Position);

        public string CurrentLine
            => Input.GetLine(CurrentLineIndex);

        public string Indicator
            => Input.GetIndicator(Position);

        public ParserState ClearNodes()
            => new ParserState(Input, Position);

        public ParserRange To(ParserState other)
            => new ParserRange(this, other);

        public ParserState WithError(ParserError error)
            => new ParserState(Input, Position, Node, error);

        public int CharsLeft
            => Input.Length - Position;

        public ParserState Match(string s)
        {
            var n = s.Length;
            if (CharsLeft < n) 
                return null;
            for (int i = 0, j = Position; i < n; i++, j++)
            {
                if (s[i] != Input[j])
                    return null;
            }
            return Advance(n);
        }

        public override bool Equals(object obj)
        {
            return obj is ParserState ps && Equals(ps);
        }

        public bool Equals(ParserState state)
        {
            if (state == null) return false;
            return Position == state.Position;
        }

        public ParserState AddNode(string name, ParserState prev)
            => new ParserState(Input, Position, new ParserNode(name, prev.To(this), Node), LastError);

        public override int GetHashCode()
            => Position.GetHashCode();
        }
}