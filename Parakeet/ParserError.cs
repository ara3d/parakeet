namespace Ara3D.Parakeet
{
    /// <summary>
    /// Parse errors are only created within sequence when a "OnError" node fails. 
    /// They occur because previous tokens in a sequence are unambiguous about what 
    /// is expected to follow. For example, an "if" keyword indicate that a parenthesized
    /// expression must follow, and then another statement, with an optional else clause. 
    /// </summary>
    public class ParserError
    {
        public ParserError(Rule expected, ParserState parentState, ParserState state, string message, ParserError previous)
        {
            Expected = expected;
            ParentState = parentState;
            State = state;
            Message = message;
            Previous = previous;
        }

        public readonly Rule Expected;
        public readonly ParserState ParentState;
        public readonly ParserState State;
        public readonly string Message;
        public readonly ParserError Previous;
    }
}