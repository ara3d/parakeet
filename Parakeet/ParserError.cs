namespace Parakeet
{
    /// <summary>
    /// Parse errors are only created within sequence when a "OnError" node fails. 
    /// They occur because previous tokens in a sequence are unambiguous about what 
    /// is expected to follow. For example, an "if" keyword indicate that a parenthesized
    /// expression must follow, and then another statement, with an optional else clause. 
    /// </summary>
    public class ParserError
    {
        public ParserError(Rule expected, Rule parent, ParserState parentState, ParserState state, string message, ParserError previous)
        {
            Expected = expected;
            Parent = parent;
            ParentState = parentState;
            State = state;
            Message = message;
            Previous = previous;
        }

        public Rule Expected { get; }
        public Rule Parent { get; }
        public ParserState ParentState { get; }
        public ParserState State { get; }
        public string Message { get; }
        public ParserError Previous { get; }
    }
}