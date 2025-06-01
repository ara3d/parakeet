using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Ara3D.Utils;

#pragma warning disable CS0659 // class overrides Object.Equals(object o), but not Object.GetHashCode()

namespace Ara3D.Parakeet
{
    /// <summary>
    /// Base class of all rules. Matching is performed in a class's override
    /// of MatchImplementation. 
    /// </summary>
    public abstract class Rule
    {
        /// <summary>
        /// Attempts to match the parser starting at current state. Returns null
        /// if and only if the Rule fails. Otherwise returns a valid ParserState.
        /// The ParserState may or may not be different from the previous one.
        /// A ParserState is immutable: it never changes. 
        /// </summary>
        protected abstract ParserState MatchImplementation(ParserState state);

        protected virtual int GetHashCodeInternal() => throw new NotImplementedException();

        public Rule() => LazyHashCode = new Lazy<int>(GetHashCodeInternal);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParserState Match(ParserState state)
        {
            // Trace the parsing during debug builds.
            if (state.Input.Debugging && Debugger.IsAttached && this.HasName())
            {
                Debug.WriteLine($"Starting parse - {this.GetName()} {state}");
                var r = MatchImplementation(state);
                var result = r?.ToString() ?? "FAILED";
                Debug.WriteLine($"Finished parse - {this.GetName()} {state} - {result}");
                return r;
            }
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

        public static implicit operator Rule(int n)
            => new CharRule((char)n);

        public static implicit operator Rule(bool b)
            => new BooleanRule(b);

        public static implicit operator Rule(char[] cs) 
            => cs.Length == 1 ? (Rule)cs[0] : new CharSetRule(cs);

        public static implicit operator Rule(string[] strings)
            => strings.Length == 1 ? (Rule)strings[0] : new SequenceRule(strings.Select(s => (Rule)s).ToArray());

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<int> LazyHashCode;
        public override sealed int GetHashCode() => LazyHashCode.Value;

        public virtual IReadOnlyList<Rule> Children
            => Array.Empty<Rule>();

        public override string ToString()
            => $"{this.GetName()} ::= {this.Body().ToDefinition()}";
    }

    /// <summary>
    /// Associates a name with a rule, for the purpose of debugging and pretty-printing grammars.
    /// Succeed if and only if the child rule succeeds.
    /// Advances if and only if the child rule advances.
    /// </summary>
    public class NamedRule : Rule
    {
        public readonly Rule Rule;
        public readonly string Name;
        
        public NamedRule(Rule r, string name) 
            => (Rule, Name) = (r, name);
        
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state);
        
        public override bool Equals(object obj) 
            => obj is NamedRule other && Equals(other.GetType(), typeof(NamedRule)) && other.Rule.Equals(Rule) && Name == other.Name;
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(NamedRule), Rule, Name);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    /// <summary>
    /// Used to express recursive relationships between rules.
    /// The child rule is retrieved via a function.
    /// This is necessary to prevent infinite loops when parsing. 
    /// This rule matches iff the child rule matches.
    /// Advances iff the child rule advances. 
    /// </summary>
    public class RecursiveRule : Rule
    {
        public Rule Rule
        {
            get
            {
                if (CachedRule != null)
                    return CachedRule;
                CachedRule = RuleFunc();
                return CachedRule;
            }
        }

        private Rule CachedRule { get; set; }
        private Func<Rule> RuleFunc { get; }
        
        public RecursiveRule(Func<Rule> ruleFunc) 
            => RuleFunc = ruleFunc;
        
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state);
        
        public override bool Equals(object obj) 
        {
            if (!(obj is RecursiveRule other))
                return false;

            // compare by delegate Method and Target reference
            if (ReferenceEquals(RuleFunc.Method, other.RuleFunc.Method) 
                && ReferenceEquals(RuleFunc.Target, other.RuleFunc.Target))
                return true;

            // compare by delegate Target type  
            if (!Equals(RuleFunc.Target.GetType(), other.RuleFunc.Target.GetType()))
                return false;

            // at this point, all fast-path failed, we have to compare the
            // delegate Method's IL code and Target's content.

            // compare the delegate Method's IL code  
            var il1 = RuleFunc.Method.GetMethodBody().GetILAsByteArray();
            var il2 = other.RuleFunc.Method.GetMethodBody().GetILAsByteArray();
            if (!il1.SequenceEqual(il2))
                return false;

            // compare the delegate Target content  
            var fields = RuleFunc.Target.GetType().GetFields();
            foreach (var field in fields)
            {
                var v1 = field.GetValue(RuleFunc.Target);
                var v2 = field.GetValue(other.RuleFunc.Target);
                if (!Equals(v1, v2))
                    return false;
            }

            return true;
        }
        
        protected override int GetHashCodeInternal() 
        {
            if (RuleFunc.Target is null)
            {
                return Hash(
                    typeof(RecursiveRule),
                    Hash(RuleFunc.Method.GetMethodBody().GetILAsByteArray().Cast<object>().ToArray())
                );
            }
            else
            {
                var fields = RuleFunc.Target.GetType().GetFields();
                return Hash(
                    typeof(RecursiveRule),
                    Hash(RuleFunc.Method.GetMethodBody().GetILAsByteArray().Cast<object>().ToArray()),
                    Hash(fields.Select(f => f.GetValue(RuleFunc.Target)).ToArray())
                );
            }
        }
    }

    /// <summary>
    /// Matches a specific sequence of characters.
    /// Advances the parser when successful.
    /// </summary>
    public class StringRule : Rule
    {
        public readonly string Pattern;

        public StringRule(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("Pattern must be non-empty", nameof(s));
            Pattern = s;
        }

        protected override ParserState MatchImplementation(ParserState state)
            => state.Match(Pattern);
        
        public override bool Equals(object obj) 
            => obj is StringRule smr && smr.Pattern == Pattern;
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(StringRule), Pattern);
    }

    /// <summary>
    /// Matches a specific sequence of characters ignoring case.
    /// Advances the parser when successful.
    /// </summary>
    public class CaseInvariantStringRule : Rule
    {
        public readonly string Pattern;

        public CaseInvariantStringRule(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("Pattern must be non-empty", nameof(s));
            Pattern = s;
        }

        protected override ParserState MatchImplementation(ParserState state)
            => state.MatchInvariant(Pattern);

        public override bool Equals(object obj)
            => obj is CaseInvariantStringRule smr && smr.Pattern == Pattern;

        protected override int GetHashCodeInternal()
            => Hash(typeof(CaseInvariantStringRule), Pattern);
    }

    /// <summary>
    /// Matches any character and advances the parser one character unless at the end of the input.
    /// </summary>
    public class AnyCharRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state)
            => state.AdvanceIfNotAtEnd();
        
        public static AnyCharRule Default { get; } 
            = new AnyCharRule();
        
        public override bool Equals(object obj) 
            => obj is AnyCharRule;
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(AnyCharRule));
    }

    /// <summary>
    /// Matches a character if it is within a specific character range (inclusive).
    /// Advances the parser if successful.
    /// </summary>
    public class CharRangeRule : Rule
    {
        public readonly char From;
        public readonly char To;

        public CharRangeRule(char from, char to)
            => (From, To) = (from, to);

        protected override ParserState MatchImplementation(ParserState state)
            => state.AdvanceIfWithin(From, To);

        public override bool Equals(object obj)
            => obj is CharRangeRule crr && crr.From == From && crr.To == To;

        protected override int GetHashCodeInternal()
            => Hash(From, To);
    }

    /// <summary>
    /// Matches a character if matches one of a set of ASCII characters.
    /// Each character must be in the range of 0 to 128 (valid ASCII character set).
    /// Uses a table lookup for performance.
    /// Advances the parser if successful. 
    /// </summary>
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

        public readonly bool[] Chars;

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

        protected override int GetHashCodeInternal()
            => Hash(typeof(CharSetRule), Hash(Chars.Cast<object>().ToArray()));

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

    /// <summary>
    /// Matches a specific character.
    /// Advances the parser if successful. 
    /// </summary>
    public class CharRule : Rule
    {
        public readonly char Char;
        
        public CharRule(char ch) 
            => Char = ch;

        protected override ParserState MatchImplementation(ParserState state)
            => state.AdvanceIf(Char);
        
        public override bool Equals(object obj) 
            => obj is CharRule csr && Char == csr.Char;

        protected override int GetHashCodeInternal()
            => Hash(typeof(CharRule), Char);
    }

    /// <summary>
    /// Succeeds iff the parser is at the end of input.
    /// Does not advance the parser.
    /// </summary>
    public class EndOfInputRule : Rule
    {
        protected override ParserState MatchImplementation(ParserState state) 
            => state.AtEnd() ? state : null;

        public static EndOfInputRule Default 
            => new EndOfInputRule();
        
        public override bool Equals(object obj) 
            => obj is EndOfInputRule;
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(EndOfInputRule));
    }

    /// <summary>
    /// When the child rule matches, it creates a new parse node in the node list. 
    /// Succeeds iff the child rule is successful.
    /// Advances iff the child rules is successful.
    /// </summary>
    public class NodeRule : NamedRule
    {
        public NodeRule(Rule rule, string name)
            : base(rule, name)
        { }

        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state.AddNode(Name, null))?.AddNode(Name, state);

        public override bool Equals(object obj)
            => obj is NodeRule nr && Name == nr.Name && Rule.Equals(nr.Rule);

        protected override int GetHashCodeInternal()
            => Hash(typeof(NodeRule), Rule, Name);
    }

    /// <summary>
    /// Attempts to match a child rule as many times as possible.
    /// Advances if the child rule advances.
    /// Child rule must advance, otherwise the parser can get stuck.   
    /// </summary>
    public class ZeroOrMoreRule : Rule
    {
        public readonly Rule Rule;
        public ZeroOrMoreRule(Rule rule) => Rule = rule;

        protected override ParserState MatchImplementation(ParserState state)
        {
            var curr = state;
            var next = Rule.Match(curr);
            while (next != null)
            {
                curr = next;
                next = Rule.Match(curr);
                if (next != null && next.Position <= curr.Position)
                {
                    throw new ParserException(curr, "Parser is no longer making progress");
                }
            }
            return curr;
        }

        public override bool Equals(object obj) 
            => obj is ZeroOrMoreRule z && z.Rule.Equals(Rule);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(ZeroOrMoreRule), Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    /// <summary>
    /// Succeeds if the child rule matches at least once 
    /// </summary>
    public class OneOrMoreRule : Rule
    {
        public readonly Rule Rule;
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
                if (next != null && next.Position <= curr.Position)
                {
                    throw new ParserException(curr, "Parser is no longer making progress");
                }
            }
            return curr;
        }

        public override bool Equals(object obj) 
            => obj is OneOrMoreRule o && o.Rule.Equals(Rule);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(OneOrMoreRule), Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    /// <summary>
    /// Succeeds if the child rule matches at least "Min" times.
    /// Attempts to match the child rule up to "Max" times.
    /// Always advances (the child rule must advance). 
    /// </summary>
    public class CountedRule : Rule
    {
        public readonly int Min;
        public readonly int Max;
        public readonly Rule Rule;

        public CountedRule(Rule rule, int min, int max)
            => (Rule, Min, Max) = (rule, min, max);

        protected override ParserState MatchImplementation(ParserState state)
        {
            var curr = state;

            var i = 0;
            while (i++ < Min)
            {
                var next = Rule.Match(curr);
                if (next == null)
                    return null;
                curr = next;
            }

            while (curr != null && i++ < Max)
            {
                var next = Rule.Match(curr);
                if (next == null)
                    return curr;
                if (next.Position <= curr.Position)
                {
                    throw new ParserException(curr, "Parser is no longer making progress");
                }
                curr = next;
            }
            return curr;
        }

        public override bool Equals(object obj) 
            => obj is CountedRule o && Min == o.Min && Max == o.Max && o.Rule.Equals(Rule);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(CountedRule), Min, Max, Rule);

        public override IReadOnlyList<Rule> Children 
            => new[] { Rule };

    }

    /// <summary>
    /// Always succeeds.
    /// Advances if the child rule advances. 
    /// </summary>
    public class OptionalRule : Rule
    {
        public readonly Rule Rule;
        
        public OptionalRule(Rule rule) 
            => Rule = rule;

        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) ?? state;

        public override bool Equals(object obj) 
            => obj is OptionalRule opt && opt.Rule.Equals(Rule);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(OptionalRule), Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    /// <summary>
    /// Succeeds only if each of the sequence of rules advances.
    /// Advances if all of the rules advance. 
    /// </summary>
    public class SequenceRule : Rule
    {
        public readonly Rule[] Rules;

        public SequenceRule(params Rule[] rules)
        {
            var n = Array.IndexOf(rules, null);
            if (n != -1)
                throw new ArgumentNullException($"{nameof(rules)}[{n}]", "At least one of the rules is null");
            Rules = rules;
        }

        public int Count 
            => Rules.Length;
        
        public Rule this[int index] 
            => Rules[index];
        
        protected override ParserState MatchImplementation(ParserState state)
        {
            var newState = state;
            OnFail onFail = null;
            foreach (var rule in Rules)
            {
                if (rule is OnFail)
                {
                    onFail = (OnFail)rule;
                }
                else
                {
                    var prevState = newState;
                    newState = rule.Match(prevState);
                    if (newState == null)
                    {
                        if (onFail != null)
                        {
                            var error = new ParserError(this, state, rule, prevState, prevState.LastError);
                            prevState = prevState.WithError(error);
                            var recovery = onFail.RecoveryRule;
                            var result = recovery.Match(prevState);
                            return result;
                        }
                        return null;
                    }
                }
            }

            return newState;
        }

        public override bool Equals(object obj) 
            => obj is SequenceRule seq && Rules.SequenceEqual(seq.Rules);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(SequenceRule), Hash(Rules));

        public override IReadOnlyList<Rule> Children => Rules;
    }

    /// <summary>
    /// Succeeds if any of the child rules pass.
    /// Advances if the passing child rule advances.
    /// </summary>
    public class ChoiceRule : Rule
    {
        public readonly Rule[] Rules;
        
        public ChoiceRule(params Rule[] rules) 
        {
            var n = Array.IndexOf(rules, null);
            if (n != -1)
                throw new ArgumentNullException($"{nameof(rules)}[{n}]", "At least one of the rules is null");
            Rules = rules;
        }
        
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
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(ChoiceRule), Hash(Rules));

        public override IReadOnlyList<Rule> Children => Rules;
    }

    /// <summary>
    /// Succeed only if the rule matches.
    /// Does not advance the parser state.  
    /// </summary>
    public class AtRule : Rule
    {
        public readonly Rule Rule;

        public AtRule(Rule rule) 
            => Rule = rule;
        
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) != null ? state : null;

        public override bool Equals(object obj) 
            => obj is AtRule at && Rule.Equals(at.Rule);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(AtRule), Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    /// <summary>
    /// Succeed only if the rule does not match.
    /// Does not advance the parser state.
    /// </summary>
    public class NotAtRule : Rule
    {
        public readonly Rule Rule;

        public NotAtRule(Rule rule) 
            => (Rule) = (rule);
        
        protected override ParserState MatchImplementation(ParserState state)
            => Rule.Match(state) == null ? state : null;

        public override bool Equals(object obj) 
            => obj is NotAtRule notAt && Rule.Equals(notAt.Rule);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(NotAtRule), Rule);

        public override IReadOnlyList<Rule> Children => new[] { Rule };
    }

    /// <summary>
    /// Used in sequences, to specify what happens if one of the subsequent nodes fails.
    /// For example, in a statement you could advance to the end of statement terminator (';').
    /// Or you could use results from a structural analysis pass to jump to next code block.
    /// Does not advance parser state.
    /// </summary>
    public class OnFail : Rule
    {
        public readonly Rule RecoveryRule;
        
        public OnFail(Rule rule) 
            => RecoveryRule = rule;
        
        protected override ParserState MatchImplementation(ParserState state) 
            => state;
        
        public override bool Equals(object obj) 
            => obj is OnFail rec && RecoveryRule.Equals(rec.RecoveryRule);
        
        protected override int GetHashCodeInternal()
            => Hash(typeof(OnFail), RecoveryRule);
    }

    /// <summary>
    /// Either always succeeds or fails regardless of parser state.
    /// Does not advance parser state.
    /// </summary>
    public class BooleanRule : Rule
    {
        public static BooleanRule True { get; } = new BooleanRule(true);
        public static BooleanRule False { get; } = new BooleanRule(false);

        public readonly bool Value;

        public BooleanRule(bool b)
            => Value = b;

        protected override ParserState MatchImplementation(ParserState state)
            => Value ? state : null;

        public override bool Equals(object obj)
            => obj is BooleanRule br && br.Value == Value;

        protected override int GetHashCodeInternal()
            => Hash(typeof(BooleanRule), Value);
    }
}