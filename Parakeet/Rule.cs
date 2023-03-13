using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Parakeet
{
    public abstract class Rule
    {
        protected abstract ParserState MatchImplementation(ParserState state);

        public ParserState Match(ParserState state)
        {
            if (CompiledRule == null)
            {
                CompiledRule = Expression.Compile();
            }

            var tmp = CompiledRule.Invoke(state);
            //var tmp2 = MatchImplementation(state);
            //Debug.Assert(ReferenceEquals(tmp, tmp2) || tmp2.Equals(tmp));
            return tmp;
        }

        public Func<ParserState, ParserState> CompiledRule { get; private set; } 

        public static SequenceRule operator +(Rule left, Rule right)
        {
            var list = new List<Rule>();
            if (left is SequenceRule seq0)
                list.AddRange(seq0.Rules);
            else
                list.Add(left);
            if (right is SequenceRule seq1)
                list.AddRange(seq1.Rules);
            else
                list.Add(right);
            return new SequenceRule(list.ToArray());
        }

        public static ChoiceRule operator |(Rule left, Rule right)
        {
            var list = new List<Rule>();
            if (left is ChoiceRule ch0)
                list.AddRange(ch0.Rules);
            else
                list.Add(left);
            if (right is ChoiceRule ch1)
                list.AddRange(ch1.Rules);
            else
                list.Add(right);
            return new ChoiceRule(list.ToArray());
        }

        public static NotAtRule operator !(Rule rule) => new NotAtRule(rule);
        public static implicit operator Rule(string s) => new StringRule(s);
        public static implicit operator Rule(char c) => new CharRule(c);
        public static implicit operator Rule(char[] cs) => new CharSetRule(cs);
        public static implicit operator Rule(Func<Rule> f) => new RecursiveRule(f);
        
        public static bool operator ==(Rule r1, Rule r2) 
            => ReferenceEquals(r1, r2) || (r1?.Equals(r2) ?? false);

        public static bool operator !=(Rule r1, Rule r2)
            => !(r1 == r2);

        public static int Hash(params object[] objects)
        {
            var hashCode = -1669597463;
            foreach (var o in objects)
            {
                hashCode = hashCode * -1521134295 + o?.GetHashCode() ?? 0;
            }
            return hashCode;
        }

        public override bool Equals(object obj)
            => throw new NotImplementedException();

        public override int GetHashCode()
            => throw new NotImplementedException();

        public abstract Expression<Func<ParserState, ParserState>> Expression { get; }
    }

    public class NamedRule : Rule
    {
        public Rule Rule { get; }
        public string Name { get; }
        public NamedRule(Rule r, string name) => (Rule, Name) = (r, name);
        protected override ParserState MatchImplementation(ParserState state) => Rule.Match(state);
        public override bool Equals(object obj) => obj is NamedRule other && other.Rule.Equals(Rule) && Name == other.Name;
        public override int GetHashCode() => Hash(Rule);
        public override Expression<Func<ParserState, ParserState>> Expression 
            => Rule.Expression;
    }

    public class RecursiveRule : Rule
    {
        public Rule Rule => 
            CachedRule = CachedRule ?? (CachedRule = RuleFunc());
        private Rule CachedRule { get; set; }
        private Func<Rule> RuleFunc { get; }
        public RecursiveRule(Func<Rule> ruleFunc) => RuleFunc = ruleFunc;
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state);
        public override bool Equals(object obj) => obj is RecursiveRule other && other.RuleFunc == RuleFunc;
        public override int GetHashCode() => Hash(RuleFunc());
        public override Expression<Func<ParserState, ParserState>> Expression 
            => Rule.Expression;
    }

    public class StringRule : Rule
    {
        public string Pattern { get; }
        public StringRule(string s) => Pattern = s;
        protected override ParserState MatchImplementation(ParserState state)
            => state.Match(Pattern);
        public override bool Equals(object obj) => obj is StringRule smr && smr.Pattern == Pattern;
        public override int GetHashCode() => Hash(Pattern);
        public override Expression<Func<ParserState, ParserState>> Expression
            => state => state.Match(Pattern);
    }

    public class AnyCharRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state)
            => state.AtEnd ? null : state.Advance();
        public static AnyCharRule Default { get; } = new AnyCharRule();
        public override bool Equals(object obj) => obj is AnyCharRule;
        public override int GetHashCode() => nameof(AnyCharRule).GetHashCode();
        public override Expression<Func<ParserState, ParserState>> Expression
            => state => state.AtEnd ? null : state.Advance();
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
        public override Expression<Func<ParserState, ParserState>> Expression
            => state => state.AtEnd ? null : state.Current >= Low && state.Current <= High ? state.Advance() : null;
    }

    // TODO: this could be much more efficient, by using a table. 
    public class CharSetRule : Rule
    {
        public char[] Chars { get; }
        public CharSetRule(params char[] chars) => Chars = chars.Distinct().ToArray();
        protected override ParserState MatchImplementation(ParserState state) 
            => state.AtEnd ? null : Chars.Contains(state.Current) ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharSetRule csr && new string(Chars) == new string(csr.Chars);
        public override int GetHashCode() => Hash(new string(Chars));
        public override Expression<Func<ParserState, ParserState>> Expression
            => state => state.AtEnd ? null : Chars.Contains(state.Current) ? state.Advance() : null;
    }

    public class CharRule : Rule
    {
        public char Char { get; }
        public CharRule(char ch) => Char = ch;
        protected override ParserState MatchImplementation(ParserState state)
            => state.AtEnd ? null : state.Current == Char ? state.Advance() : null;
        public override bool Equals(object obj) => obj is CharRule csr && Char == csr.Char;
        public override int GetHashCode() => Hash(Char);
        public override Expression<Func<ParserState, ParserState>> Expression
            => state => state.AtEnd ? null : state.Current == Char ? state.Advance() : null;
    }

    public class EndOfInputRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state) 
            => state.AtEnd ? state : null;
        public static EndOfInputRule Default => new EndOfInputRule();
        public override bool Equals(object obj) => obj is EndOfInputRule;
        public override int GetHashCode() => nameof(EndOfInputRule).GetHashCode();
        public override Expression<Func<ParserState, ParserState>> Expression
            => state => state.AtEnd ? state : null;
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
        }

        public override bool Equals(object obj) => obj is NodeRule nr && Name == nr.Name && Rule.Equals(nr.Rule) 
            && (Eat == null ? nr.Eat == null : Eat.Equals(nr.Eat));

        public override int GetHashCode() => Hash(Rule, Eat, Name);

        public override Expression<Func<ParserState, ParserState>> Expression
            => state => MatchImplementation(state);
    }

    public class ZeroOrMoreRule : Rule
    {
        public Rule Rule { get; }
        public ZeroOrMoreRule(Rule rule) => Rule = rule;

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

        public override bool Equals(object obj) => obj is ZeroOrMoreRule z && z.Rule.Equals(Rule);
        public override int GetHashCode() => Hash(Rule);

        public override Expression<Func<ParserState, ParserState>> Expression
        {
            get
            {

                // https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/expression-trees/expression-trees-building
                var state = Parameter(typeof(ParserState), "state");
                var cur = Parameter(typeof(ParserState), "cur");
                var next = Parameter(typeof(ParserState), "next");
                var label = Label(typeof(ParserState));
                var block = Block(
                    new ParameterExpression[]
                    {
                        cur, next
                    },
                    new Expression[]
                    {
                        Assign(cur, state),
                        Assign(next, Invoke(Rule.Expression, cur)),
                        Loop(
                            IfThenElse(
                                NotEqual(next, Default(typeof(ParserState))),
                                Block(
                                    Assign(cur, next),
                                    Assign(next, Invoke(Rule.Expression, cur))
                                ),
                                Return(label, cur)),
                            label)
                    });
                return Lambda<Func<ParserState, ParserState>>(block, state);
            }
        }
    }

    public class OptionalRule : Rule
    {
        public Rule Rule { get; }
        public OptionalRule(Rule rule) => Rule = rule;

        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) ?? state;

        public override bool Equals(object obj) => obj is OptionalRule opt && opt.Rule.Equals(Rule);
        public override int GetHashCode() => Hash(Rule);

        public override Expression<Func<ParserState, ParserState>> Expression
        {
            get
            {
                var state = Parameter(typeof(ParserState), "state");
                var exprBody = Coalesce(Invoke(Rule.Expression, state), state);
                return Lambda<Func<ParserState, ParserState>>(exprBody, state);
            }
        }
    }

    public class SequenceRule : Rule
    {
        public Rule[] Rules { get; }
        public SequenceRule(params Rule[] rules) => Rules = rules;
        public int Count => Rules.Length;
        public Rule this[int index] => Rules[index];
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

        public override bool Equals(object obj) => obj is SequenceRule seq && Rules.SequenceEqual(seq.Rules);
        public override int GetHashCode() => Hash(Rules);

        public override Expression<Func<ParserState, ParserState>> Expression
        {
            get
            {
                var state = Parameter(typeof(ParserState), "state");
                var statements = Rules.Select(r => Assign(state,
                    Condition(
                        Equal(state, Default(typeof(ParserState))),
                        Default(typeof(ParserState)),
                        Invoke(r.Expression, state)))).Cast<Expression>().ToList();
                statements.Add(state);
                var block = Block(typeof(ParserState), statements);
                return Lambda<Func<ParserState, ParserState>>(block, state);
            }
        }
    }

    public class ChoiceRule : Rule
    {
        public Rule[] Rules { get; }
        public ChoiceRule(params Rule[] rules) => Rules = rules;
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

        public override bool Equals(object obj) => obj is ChoiceRule ch && Rules.SequenceEqual(ch.Rules);
        public override int GetHashCode() => Hash(Rules);

        public override Expression<Func<ParserState, ParserState>> Expression
        {
            get
            {
                var state = Parameter(typeof(ParserState), "state");
                var newState = Parameter(typeof(ParserState), "newState");
                var statements = new List<Expression>();
                var label = Label(typeof(ParserState));
                foreach (var r in Rules)
                {
                    statements.Add(Assign(newState, Invoke(r.Expression, state)));
                    statements.Add(IfThen(NotEqual(newState, Default(typeof(ParserState))), Return(label, newState)));
                }
                statements.Add(Label(label, Default(typeof(ParserState))));
                var block = Block(typeof(ParserState), new[] { newState }, statements);
                return Lambda<Func<ParserState, ParserState>>(block, state);
            }
        }
    }

    public class AtRule : Rule
    {
        public Rule Rule { get; }
        public AtRule(Rule rule) => (Rule) = (rule);
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) != null ? state : null;

        public override bool Equals(object obj) => obj is AtRule at && Rule.Equals(at.Rule);
        public override int GetHashCode() => Hash(Rule);

        public override Expression<Func<ParserState, ParserState>> Expression
        {
            get
            {
                var state = Parameter(typeof(ParserState), "state");
                var exprBody = Condition(
                    NotEqual(
                        Invoke(Rule.Expression, state), 
                        Default(typeof(ParserState))), 
                    state, 
                    Default(typeof(ParserState)));
                return Lambda<Func<ParserState, ParserState>>(exprBody, state);
            }
        }
    }

    public class NotAtRule : Rule
    {
        public Rule Rule { get; }
        public NotAtRule(Rule rule) => (Rule) = (rule);
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) == null ? state : null;
        public override bool Equals(object obj) => obj is NotAtRule notAt && Rule.Equals(notAt.Rule);
        public override int GetHashCode() => Hash(Rule);

        public override Expression<Func<ParserState, ParserState>> Expression
        {
            get
            {
                var state = Parameter(typeof(ParserState), "state");
                var exprBody = Condition(
                    NotEqual(
                        Invoke(Rule.Expression, state),
                        Default(typeof(ParserState))),
                    Default(typeof(ParserState)),
                    state);
                return Lambda<Func<ParserState, ParserState>>(exprBody, state);
            }
        }
    }

    public class OnError : Rule
    {
        public Rule RecoveryRule { get; }
        public OnError(Rule rule) => RecoveryRule = rule;
        protected override ParserState MatchImplementation(ParserState state) => state;
        public override bool Equals(object obj) => obj is OnError rec && RecoveryRule.Equals(rec.RecoveryRule);
        public override int GetHashCode() => Hash(RecoveryRule);

        public override Expression<Func<ParserState, ParserState>> Expression
            => state => state;
    }
}