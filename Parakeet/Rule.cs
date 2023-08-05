using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Parakeet
{
    public abstract class Rule
    {
        protected abstract ParserState MatchImplementation(ParserState state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserState Match(ParserState state)
        {
            return MatchImplementation(state);
        }

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

        public static NotAtRule operator !(Rule rule) 
            => new NotAtRule(rule);
        
        public static implicit operator Rule(string s) 
            => s.Length == 1 ? (Rule)s[0] : new StringRule(s);
        
        public static implicit operator Rule(char c) 
            => new CharRule(c);

        public static implicit operator Rule(bool b)
            => new BooleanRule(b);

        public static implicit operator Rule(char[] cs) 
            => cs.Length == 1 ? (Rule)cs[0] : new CharSetRule(cs);
        
        public static implicit operator Rule(Func<Rule> f) 
            => new RecursiveRule(f);
        
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

        public virtual IReadOnlyList<Rule> Children 
            => Array.Empty<Rule>();

        public override string ToString()
            => $"{this.GetName()} ::= {this.Body().ToDefinition()}";
    }

    public class NamedRule : Rule
    {
        public Rule Rule { get; }
        public string Name { get; }
        
        public NamedRule(Rule r, string name) 
            => (Rule, Name) = (r, name);
        
        protected override ParserState MatchImplementation(ParserState state) 
            => Rule.Match(state);
        
        public override bool Equals(object obj) 
            => obj is NamedRule other && other.Rule.Equals(Rule) && Name == other.Name;
        
        public override int GetHashCode() 
            => Hash(Rule, Name);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    public class RecursiveRule : Rule
    {
        public Rule Rule 
            => CachedRule = CachedRule ?? (CachedRule = RuleFunc());
        
        private Rule CachedRule { get; set; }
        private Func<Rule> RuleFunc { get; }
        
        public RecursiveRule(Func<Rule> ruleFunc) 
            => RuleFunc = ruleFunc;
        
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state);
        
        public override bool Equals(object obj) 
            => obj is RecursiveRule other && other.RuleFunc == RuleFunc;
        
        public override int GetHashCode() 
            => Hash(RuleFunc);
    }

    public class StringRule : Rule
    {
        public string Pattern { get; }
        
        public StringRule(string s) 
            => Pattern = s;
        
        protected override ParserState MatchImplementation(ParserState state)
            => state.Match(Pattern);
        
        public override bool Equals(object obj) 
            => obj is StringRule smr && smr.Pattern == Pattern;
        
        public override int GetHashCode() 
            => Hash(Pattern);
    }

    public class AnyCharRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state)
            => state.AdvanceIfNotAtEnd();
        
        public static AnyCharRule Default { get; } 
            = new AnyCharRule();
        
        public override bool Equals(object obj) 
            => obj is AnyCharRule;
        
        public override int GetHashCode() 
            => nameof(AnyCharRule).GetHashCode();
    }

    public class CharSetRule : Rule
    {
        public static bool[] CharsToTable(IEnumerable<char> chars)
        {
            var r = new bool[128];
            foreach (var c in chars)
            {
                if (c >= 128) throw new NotSupportedException();
                r[c] = true;
            }

            return r;
        }

        public bool[] Chars { get; }

        public CharSetRule(params char[] chars) 
            : this(CharsToTable(chars)) 
        { }

        public CharSetRule(bool[] chars) 
            => Chars = chars;

        protected override ParserState MatchImplementation(ParserState state)
            => state.AtEnd() ? null 
                : state.GetCurrent() < 128 && Chars[state.GetCurrent()] 
                    ? state.Advance() 
                    : null;

        public override bool Equals(object obj) 
            => obj is CharSetRule csr && Chars.SequenceEqual(csr.Chars);

        public override int GetHashCode() 
            => Hash(nameof(CharSetRule));

        public CharSetRule Union(CharSetRule other)
        {
            var tmp = Chars.ToArray();
            for (var i = 0; i < tmp.Length; i++)
            {
                if (other.Chars[i])
                    tmp[i] = true;
            }

            return new CharSetRule(tmp);
        }

        public CharSetRule Append(char other)
        {
            var tmp = Chars.ToArray();
            tmp[other] = true;
            return new CharSetRule(tmp);
        }

        public override string ToString()
        {
            var rangeCount = 0;
            var sb = new StringBuilder();

            for (var i = 0; i < Chars.Length; ++i)
            {
                var c = (char)i;
                if (Chars[i])
                {
                    rangeCount++;
                    if (rangeCount == 1)
                    {
                        sb.Append(c.EscapeChar());
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        var prevChar = (char)(i - 1);
                        if (rangeCount > 2)
                        {
                            sb.Append("-");
                        }

                        if (rangeCount > 1)
                        {
                            sb.Append(prevChar.EscapeChar());
                        }
                    }

                    rangeCount = 0;
                }
            }
            return sb.ToString();
        }
    }

    public class CharRule : Rule
    {
        public char Char { get; }
        
        public CharRule(char ch) 
            => Char = ch;

        protected override ParserState MatchImplementation(ParserState state)
            => state.AdvanceIf(Char);
        
        public override bool Equals(object obj) 
            => obj is CharRule csr && Char == csr.Char;

        public override int GetHashCode() 
            => Hash(Char);
    }

    public class EndOfInputRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state) 
            => state.AtEnd() ? state : null;

        public static EndOfInputRule Default 
            => new EndOfInputRule();
        
        public override bool Equals(object obj) 
            => obj is EndOfInputRule;
        
        public override int GetHashCode() 
            => nameof(EndOfInputRule).GetHashCode();
    }

    public class NodeRule : NamedRule
    {
        public NodeRule(Rule rule, string name) 
            : base(rule, name)
        { }

        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state)?.AddNode(Name, state);

        public override bool Equals(object obj) 
            => obj is NodeRule nr && Name == nr.Name && Rule.Equals(nr.Rule);

        public override int GetHashCode() 
            => Hash(Rule, Name);
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
#if DEBUG
                if (next != null)
                {
                    if (next.Position <= curr.Position)
                    {
                        throw new ParserException(curr, "Parser is no longer making progress");
                    }
                }
#endif
            }
            return curr;
        }

        public override bool Equals(object obj) => obj is ZeroOrMoreRule z && z.Rule.Equals(Rule);
        public override int GetHashCode() => Hash (Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    public class OneOrMoreRule : Rule
    {
        public Rule Rule { get; }
        public OneOrMoreRule(Rule rule) => Rule = rule;

        protected override ParserState MatchImplementation(ParserState state)
        {
            var curr = state;
            var next = Rule.Match(curr);
            if (next == null)
            {
                return null;
            }
            while (next != null)
            {
                curr = next;
                next = Rule.Match(curr);
#if DEBUG
                if (next != null)
                {
                    if (next.Position <= curr.Position)
                    {
                        throw new ParserException(curr, "Parser is no longer making progress");
                    }
                }
#endif
            }
            return curr;
        }

        public override bool Equals(object obj) => obj is OneOrMoreRule o && o.Rule.Equals(Rule);
        public override int GetHashCode() => Hash(Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    public class OptionalRule : Rule
    {
        public Rule Rule { get; }
        
        public OptionalRule(Rule rule) 
            => Rule = rule;

        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) ?? state;

        public override bool Equals(object obj) 
            => obj is OptionalRule opt && opt.Rule.Equals(Rule);
        
        public override int GetHashCode() 
            => Hash(Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    public class SequenceRule : Rule
    {
        public Rule[] Rules { get; }

        public SequenceRule(params Rule[] rules)
        {
            Rules = rules;
            if (Rules.Any(r => r == null))
                throw new Exception("One of the rules is null");
        }

        public int Count 
            => Rules.Length;
        
        public Rule this[int index] 
            => Rules[index];
        
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
                            var result = recovery.Match(prevState);
                            Debug.Assert(result == null || result.LastError != null);
                            return result;
                        }
                        return null;
                    }
                }
            }

            return newState;
        }

        public override bool Equals(object obj) => obj is SequenceRule seq && Rules.SequenceEqual(seq.Rules);
        public override int GetHashCode() => Hash(Rules);

        public override IReadOnlyList<Rule> Children => Rules;
    }

    public class ChoiceRule : Rule
    {
        public Rule[] Rules { get; }
        
        public ChoiceRule(params Rule[] rules) 
            => Rules = rules;
        
        public int Count 
            => Rules.Count();
        
        public Rule this[int index] 
            => Rules[index];

        protected override ParserState MatchImplementation(ParserState state)
        {
            foreach (var rule in Rules)
            {
                var newState = rule.Match(state);
                if (newState != null) return newState;
            }

            return null;
        }

        public override bool Equals(object obj) 
            => obj is ChoiceRule ch && Rules.SequenceEqual(ch.Rules);
        
        public override int GetHashCode() 
            => Hash(Rules);

        public override IReadOnlyList<Rule> Children => Rules;
    }

    public class AtRule : Rule
    {
        public Rule Rule { get; }
        
        public AtRule(Rule rule) 
            => Rule = rule;
        
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) != null ? state : null;

        public override bool Equals(object obj) 
            => obj is AtRule at && Rule.Equals(at.Rule);
        
        public override int GetHashCode() 
            => Hash(Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    public class NotAtRule : Rule
    {
        public Rule Rule { get; }

        public NotAtRule(Rule rule) 
            => (Rule) = (rule);
        
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) == null ? state : null;

        public override bool Equals(object obj) 
            => obj is NotAtRule notAt && Rule.Equals(notAt.Rule);
        
        public override int GetHashCode() 
            => Hash(Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    public class OnError : Rule
    {
        public Rule RecoveryRule { get; }
        
        public OnError(Rule rule) 
            => RecoveryRule = rule;
        
        protected override ParserState MatchImplementation(ParserState state) 
            => state;
        
        public override bool Equals(object obj) 
            => obj is OnError rec && RecoveryRule.Equals(rec.RecoveryRule);
        
        public override int GetHashCode() 
            => Hash(RecoveryRule);
    }

    public class BooleanRule : Rule
    {
        public bool Value {get;}

        public BooleanRule(bool b)
            => Value = b;

        protected override ParserState MatchImplementation(ParserState state)
            => Value ? state : null;

        public override bool Equals(object obj)
            => obj is BooleanRule br && br.Value == Value;

        public override int GetHashCode()
            => Hash(Value);
    }
}