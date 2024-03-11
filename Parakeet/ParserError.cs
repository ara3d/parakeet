namespace Ara3D.Parakeet
{
    /// <summary>
    /// Parse errors are created within a sequence when a rule following an "OnFail" rule fails. 
    /// They can occur because previous tokens in a sequence might be unambiguous about what 
    /// is expected to follow. For example, in some languages an "if" keyword indicate that a parenthesized
    /// expression must follow, and then another statement, with an optional else clause. 
    /// </summary>
    public class ParserError
    {
        public ParserError(Rule seqRule, ParserState seqState, Rule rule, ParserState state, ParserError previous)
        {
            SeqRule = seqRule;
            SeqState = seqState;
            Rule = rule;
            State = state;
            Previous = previous;
        }

        public string NodeName => Node?.Name ?? "_INPUT_";
        public ParserNode Node => SeqState.Node;
        public ParserInput Input => SeqState.Input;
        public ParserRange Range => ParserRange.Create(SeqState, State);

        public readonly ParserState SeqState;
        public readonly ParserState State;
        public readonly Rule SeqRule;
        public readonly Rule Rule;
        public readonly ParserError Previous;
        
        public override string ToString()
            => $"Error at {Range} while parsing {NodeName}, expected Rule failed {Rule}";
    }
}