using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Parakeet
{
    public abstract class Rule
    {
        protected abstract ParserState MatchImplementation(ParserState state, ParserCache cache);

        public ParserState Match(ParserState state, ParserCache cache)
        {
            // Set NO_CACHING to true to see performance of naive parser. 
#if NO_CACHING
        return MatchImplementation(state, cache);
#else
            // If we have already parsed this position and rule combination: return it. 
            var prev = cache.PreviousMatch(state.Position);
            if (prev?.Node?.Rule == this)
                return prev;

            try
            {
                var result = MatchImplementation(state, cache);
                cache.Update(result);
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Uncaught exception {e}");
                throw;
            }
#endif
        }

        public static Sequence operator +(Rule left, Rule right) => new Sequence(left, right);
        public static Choice operator |(Rule left, Rule right) => new Choice(left, right);
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
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache) => Rule.Match(state, cache);
        public override bool Equals(object obj) => obj is NamedRule other && other.Rule.Equals(Rule) && Name == other.Name;
        public override int GetHashCode() => Hash(Rule);
    }

    public class RecursiveRule : Rule
    {
        public Func<Rule> RuleFunc { get; }
        public RecursiveRule(Func<Rule> ruleFunc) => RuleFunc = ruleFunc;
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache) => RuleFunc().Match(state, cache);
        public override bool Equals(object obj) => obj is RecursiveRule other && other.RuleFunc == RuleFunc;
        public override int GetHashCode() => Hash(RuleFunc());
    }

    public class StringMatchRule : Rule
    {
        public string Pattern { get; }
        public StringMatchRule(string s) => Pattern = s;
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
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
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
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
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
            => state.AtEnd ? null : state.Current >= Low && state.Current <= High ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharRangeRule crr && crr.Low == Low && crr.High == High;
        public override int GetHashCode() => Hash(Low, High);
    }

    public class CharSetRule : Rule
    {
        public char[] Chars { get; }
        public CharSetRule(params char[] chars) => Chars = chars;
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache) => state.AtEnd ? null : Chars.Contains(state.Current) ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharSetRule csr && new string(Chars) == new string(csr.Chars);
        public override int GetHashCode() => Hash(new string(Chars));
    }

    public class EndOfInputRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache) => state.AtEnd ? state : null;
        public static EndOfInputRule Default => new EndOfInputRule();
        public override bool Equals(object obj) => obj is EndOfInputRule;
    }

    public class CharMatchRule : Rule
    {
        public char Ch { get; }
        public CharMatchRule(char ch) => Ch = ch;
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
            => state.AtEnd ? null : state.Current == Ch ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharMatchRule cmr && cmr.Ch == Ch;
        public override int GetHashCode() => Hash(Ch);
    }

    public class NodeRule : NamedRule
    {
        // Can be null
        public Rule Eat { get; }

        public NodeRule(Rule rule, Rule eat, string name) : base(rule, name) => Eat = eat;

        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
        {
            var result = Rule.Match(state, cache);
            if (result is null) return null;
            var node = new ParserNode(this, state.To(result), result.Node);
            var r = new ParserState(result.Input, result.Position, node);
            // Parse the data to eat 
            var tmp = Eat?.Match(r, cache);
            if (tmp != null) return tmp;
            return r;
        }

        public override bool Equals(object obj) => obj is NodeRule nr && Name == nr.Name && Rule.Equals(nr.Rule) && Eat.Equals(nr.Eat);
        public override int GetHashCode() => Hash(Rule, Eat, Name);
    }

    public class ZeroOrMore : Rule
    {
        public Rule Rule { get; }
        public ZeroOrMore(Rule rule) => Rule = rule;

        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
        {
            var curr = state;
            var next = Rule.Match(curr, cache);
            while (next != null)
            {
                curr = next;
                next = Rule.Match(curr, cache);
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
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
            => Rule.Match(state, cache) ?? state;
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
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
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
                    newState = rule.Match(prevState, cache);
                    if (newState == null)
                    {
                        if (onError != null)
                        {
                            var error = new ParseError(rule, this, state, prevState, msg);
                            cache.Errors.Add(error);
                            var recovery = onError.RecoveryRule;
                            return recovery.Match(prevState, cache);
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
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
        {
            foreach (var rule in Rules)
            {
                var newState = rule.Match(state, cache);
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
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
            => Rule.Match(state, cache) != null ? state : null;

        public override bool Equals(object obj) => obj is At at && Rule.Equals(at.Rule);
        public override int GetHashCode() => Hash(Rule);
    }

    public class NotAt : Rule
    {
        public Rule Rule { get; }
        public NotAt(Rule rule) => (Rule) = (rule);
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache)
            => Rule.Match(state, cache) == null ? state : null;
        public override bool Equals(object obj) => obj is NotAt notAt && Rule.Equals(notAt.Rule);
        public override int GetHashCode() => Hash(Rule);
    }

    public class OnError : Rule
    {
        public Rule RecoveryRule { get; }
        public OnError(Rule rule) => RecoveryRule = rule;
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache) => state;
        public override bool Equals(object obj) => obj is OnError rec && RecoveryRule.Equals(rec.RecoveryRule);
        public override int GetHashCode() => Hash(RecoveryRule);
    }

    public class LookBehind : Rule
    {
        public Rule Rule { get; }
        public LookBehind(Rule rule) => Rule = rule;
        protected override ParserState MatchImplementation(ParserState state, ParserCache cache) => Rule.Match(state.Reverse(), cache) != null ? state : null;
        public override bool Equals(object obj) => obj is LookBehind lb && Rule.Equals(lb.Rule);
        public override int GetHashCode() => Hash(Rule);
    }
}