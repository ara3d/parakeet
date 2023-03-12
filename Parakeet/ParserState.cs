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
        public bool Backwards { get; }

        public bool AtEnd => Position < 0 || Position >= Input.Length;
        public char Current => Input[Position];

        public ParserState(ParserInput input, int position = 0, ParserNode node = null, bool backwards = false, ParserError error = null)
            => (Input, Position, Node, Backwards, LastError) = (input, position, node, backwards, error);

        public ParserState Reverse()
            => new ParserState(Input, Position, Node, !Backwards, LastError);

        public ParserState Advance()
            => AtEnd ? null : new ParserState(Input, Backwards ? Position - 1 : Position + 1, Node, Backwards, LastError);

        public ParserState JumpToEnd()
            => new ParserState(Input, Input.Length, Node, Backwards, LastError);

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
            => new ParserState(Input, Position, Node, Backwards, error);
    }
}