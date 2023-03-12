using System;
using System.Collections.Generic;
using System.Linq;

namespace Parakeet
{
    public abstract class Rule
    {
        protected abstract ParserState MatchImplementation(ParserState state);

        public ParserState Match(ParserState state)
            => MatchImplementation(state);

        public static Sequence operator +(Rule left, Rule right)
        {
            var list = new List<Rule>();
            if (left is Sequence seq0)
                list.AddRange(seq0.Rules);
            else
                list.Add(left);
            if (right is Sequence seq1)
                list.AddRange(seq1.Rules);
            else
                list.Add(right);
            return new Sequence(list.ToArray());
        }

        public static Choice operator |(Rule left, Rule right)
        {
            var list = new List<Rule>();
            if (left is Choice ch0)
                list.AddRange(ch0.Rules);
            else
                list.Add(left);
            if (right is Choice ch1)
                list.AddRange(ch1.Rules);
            else
                list.Add(right);
            return new Choice(list.ToArray());
        }

        public static NotAt operator !(Rule rule) => new NotAt(rule);
        public static implicit operator Rule(string s) => new StringMatchRule(s);
        public static implicit operator Rule(char c) => new CharMatchRule(c);
        public static implicit operator Rule(char[] cs) => new CharSetRule(cs);
        public static implicit operator Rule(Func<Rule> f) => new RecursiveRule(f);
        public static int Hash(params object[] objects)
        {
            var hashCode = -1669597463;
            foreach (var o in objects)
            {
                hashCode = hashCode * -1521134295 + o?.GetHashCode() ?? 0;
            }
            return hashCode;
        }
    }

    public class NamedRule : Rule
    {
        public Rule Rule { get; }
        public string Name { get; }
        public NamedRule(Rule r, string name) => (Rule, Name) = (r, name);
        protected override ParserState MatchImplementation(ParserState state) => Rule.Match(state);
        public override bool Equals(object obj) => obj is NamedRule other && other.Rule.Equals(Rule) && Name == other.Name;
        public override int GetHashCode() => Hash(Rule);
    }

    public class RecursiveRule : Rule
    {
        public Rule Rule => 
            CachedRule = CachedRule ?? (CachedRule = RuleFunc());
        private Rule CachedRule { get; set; }
        private Func<Rule> RuleFunc { get; }
        public RecursiveRule(Func<Rule> ruleFunc) => RuleFunc = ruleFunc;
        protected override ParserState MatchImplementation(ParserState state)
        {
            return Rule.Match(state);
        }
        public override bool Equals(object obj) => obj is RecursiveRule other && other.RuleFunc == RuleFunc;
        public override int GetHashCode() => Hash(RuleFunc());
    }

    public class StringMatchRule : Rule
    {
        public string Pattern { get; }
        public StringMatchRule(string s) => Pattern = s;
        protected override ParserState MatchImplementation(ParserState state)
        {
            for (var i = 0; i < Pattern.Length; ++i)
            {
                if (state.AtEnd || state?.Current != Pattern[i])
                    return null;
                var tmp = state?.Advance();
                if (tmp == null) return null;
                state = tmp;
            }
            return state;
        }
        public override bool Equals(object obj) => obj is StringMatchRule smr && smr.Pattern == Pattern;
        public override int GetHashCode() => Hash(Pattern);
    }

    public class AnyCharRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state)
            => state.AtEnd ? null : state.Advance();
        public static AnyCharRule Default { get; } = new AnyCharRule();
        public override bool Equals(object obj) => obj is AnyCharRule;
        public override int GetHashCode() => nameof(AnyCharRule).GetHashCode();
    }

    public class CharRangeRule : Rule
    {
        public char Low { get; }
        public char High { get; }
        public CharRangeRule(char low, char high) => (Low, High) = (low, high);
        protected override ParserState MatchImplementation(ParserState state)
            => state.AtEnd ? null : state.Current >= Low && state.Current <= High ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharRangeRule crr && crr.Low == Low && crr.High == High;
        public override int GetHashCode() => Hash(Low, High);
    }

    public class CharSetRule : Rule
    {
        public char[] Chars { get; }
        public CharSetRule(params char[] chars) => Chars = chars;
        protected override ParserState MatchImplementation(ParserState state) => state.AtEnd ? null : Chars.Contains(state.Current) ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharSetRule csr && new string(Chars) == new string(csr.Chars);
        public override int GetHashCode() => Hash(new string(Chars));
    }

    public class EndOfInputRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state) => state.AtEnd ? state : null;
        public static EndOfInputRule Default => new EndOfInputRule();
        public override bool Equals(object obj) => obj is EndOfInputRule;
        public override int GetHashCode() => nameof(EndOfInputRule).GetHashCode();
    }

    public class CharMatchRule : Rule
    {
        public char Ch { get; }
        public CharMatchRule(char ch) => Ch = ch;
        protected override ParserState MatchImplementation(ParserState state)
            => state.AtEnd ? null : state.Current == Ch ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharMatchRule cmr && cmr.Ch == Ch;
        public override int GetHashCode() => Hash(Ch);
    }

    public class NodeRule : NamedRule
    {
        // Can be null
        public Rule Eat { get; }

        public NodeRule(Rule rule, Rule eat, string name) : base(rule, name) => Eat = eat;

        protected override ParserState MatchImplementation(ParserState state)
        {
            var result = Rule.Match(state);
            if (result is null)
                return null;
            
            var node = new ParserNode(this, state.To(result), result.Node);
            result = new ParserState(result.Input, result.Position, node);

            // Eat whitespace 
            var tmp = Eat?.Match(result);
            if (tmp != null) 
                result = tmp;
            
            return result;

            /*
            // If we have already parsed this position and rule combination: return it. 
            var prev = cache.PreviousMatch(state.Position);
            if (prev?.Node?.Rule == this)
                return prev;

            var result = Rule.Match(state, cache);
            if (result is null) 
                return null;
            var node = new ParserNode(this, state.To(result), result.Node);
            result = new ParserState(result.Input, result.Position, node);
                
            // Eat whitespace 
            var tmp = Eat?.Match(result, cache);
            if (tmp != null) result = tmp;

            // Cache the result and return it
            cache.Update(result);
            return result;
            */
        }

        public override bool Equals(object obj) => obj is NodeRule nr && Name == nr.Name && Rule.Equals(nr.Rule) 
            && (Eat == null ? nr.Eat == null : Eat.Equals(nr.Eat));

        public override int GetHashCode() => Hash(Rule, Eat, Name);
    }

    public class ZeroOrMore : Rule
    {
        public Rule Rule { get; }
        public ZeroOrMore(Rule rule) => Rule = rule;

        protected override ParserState MatchImplementation(ParserState state)
        {
            var curr = state;
            var next = Rule.Match(curr);
            while (next != null)
            {
                curr = next;
                next = Rule.Match(curr);
                if (next != null)
                {
                    if (next.Position <= curr.Position)
                    {
                        throw new ParserException(curr, "Parser is no longer making progress");
                    }
                }
            }
            return curr;
        }

        public override bool Equals(object obj) => obj is ZeroOrMore z && z.Rule.Equals(Rule);
        public override int GetHashCode() => Hash(Rule);
    }

    public class Optional : Rule
    {
        public Rule Rule { get; }
        public Optional(Rule rule) => Rule = rule;
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) ?? state;
        public override bool Equals(object obj) => obj is Optional opt && opt.Rule.Equals(Rule);
        public override int GetHashCode() => Hash(Rule);
    }

    public class Sequence : Rule
    {
        public Rule[] Rules { get; }
        public Sequence(params Rule[] rules) => Rules = rules;
        public int Count => Rules.Count();
        public Rule this[int index] => Rules[index];
        public Rule Head => this[0];
        public Rule Tail => new Sequence(Rules.Skip(1).ToArray());
        protected override ParserState MatchImplementation(ParserState state)
        {
            var newState = state;
            OnError onError = null;
            foreach (var rule in Rules)
            {
                if (rule is OnError)
                {
                    onError = (OnError)rule;
                }
                else
                {
                    var prevState = newState;
                    var msg = string.Empty;
                    newState = rule.Match(prevState);
                    if (newState == null)
                    {
                        if (onError != null)
                        {
                            var error = new ParserError(rule, this, state, prevState, msg, prevState.LastError);
                            prevState = prevState.WithError(error);
                            var recovery = onError.RecoveryRule;
                            return recovery.Match(prevState);
                        }
                        return null;
                    }
                }
            }

            return newState;
        }

        public override bool Equals(object obj) => obj is Sequence seq && Rules.SequenceEqual(seq.Rules);
        public override int GetHashCode() => Hash(Rules);
    }

    public class Choice : Rule
    {
        public Rule[] Rules { get; }
        public Choice(params Rule[] rules) => Rules = rules;
        public int Count => Rules.Count();
        public Rule this[int index] => Rules[index];
        protected override ParserState MatchImplementation(ParserState state)
        {
            foreach (var rule in Rules)
            {
                var newState = rule.Match(state);
                if (newState != null) return newState;
            }

            return null;
        }

        public override bool Equals(object obj) => obj is Choice ch && Rules.SequenceEqual(ch.Rules);
        public override int GetHashCode() => Hash(Rules);
    }

    public class At : Rule
    {
        public Rule Rule { get; }
        public At(Rule rule) => (Rule) = (rule);
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) != null ? state : null;

        public override bool Equals(object obj) => obj is At at && Rule.Equals(at.Rule);
        public override int GetHashCode() => Hash(Rule);
    }

    public class NotAt : Rule
    {
        public Rule Rule { get; }
        public NotAt(Rule rule) => (Rule) = (rule);
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) == null ? state : null;
        public override bool Equals(object obj) => obj is NotAt notAt && Rule.Equals(notAt.Rule);
        public override int GetHashCode() => Hash(Rule);
    }

    public class OnError : Rule
    {
        public Rule RecoveryRule { get; }
        public OnError(Rule rule) => RecoveryRule = rule;
        protected override ParserState MatchImplementation(ParserState state) => state;
        public override bool Equals(object obj) => obj is OnError rec && RecoveryRule.Equals(rec.RecoveryRule);
        public override int GetHashCode() => Hash(RecoveryRule);
    }

    public class LookBehind : Rule
    {
        public Rule Rule { get; }
        public LookBehind(Rule rule) => Rule = rule;
        protected override ParserState MatchImplementation(ParserState state) => Rule.Match(state.Reverse()) != null ? state : null;
        public override bool Equals(object obj) => obj is LookBehind lb && Rule.Equals(lb.Rule);
        public override int GetHashCode() => Hash(Rule);
    }
}