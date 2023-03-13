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

        public bool AtEnd => Position < 0 || Position >= Input.Length;
        public char Current => Input[Position];

        public ParserState(ParserInput input, int position = 0, ParserNode node = null, ParserError error = null)
            => (Input, Position, Node, LastError) = (input, position, node, error);

        public ParserState Advance(int amount)
            => new ParserState(Input, Position + amount, Node,  LastError);

        public ParserState Advance()
            => new ParserState(Input, Position + 1, Node, LastError);

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
    }
}