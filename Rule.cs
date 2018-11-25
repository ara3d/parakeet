using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parakeet
{
    public abstract class Rule
    {
        protected Rule(IEnumerable<Rule> rules)
        {
            if (rules.Any(r => r == null)) throw new Exception("No child rule can be null");
            Children = new List<Rule>(rules);
        }

        protected Rule(params Rule[] rules)
            : this((IEnumerable<Rule>)rules)
        {
        }

        public List<Rule> Children = new List<Rule>();

        public string Name { get; private set; }
        public int Index { get; private set; }

        public Rule Child => Children[0];

        protected abstract bool InternalMatch(ParserState state);

        public bool Match(ParserState state)
        {
            // HINT: This is a good place to set a conditional break-point when debugging.
            // Using the Name = X as a condition.
            /*
            // This slows things down, but it is a useful debugging tool.
            state.Rules.Push(this);
            try
            {
                return InternalMatch(state);
            }
            finally
            {
                state.Rules.Pop();
            }*/

            return InternalMatch(state);
        }

        public static Rule operator +(Rule r1, Rule r2)
        {
            return Grammar.Seq(r1, r2);
        }

        public static Rule operator |(Rule r1, Rule r2)
        {
            return Grammar.Choice(r1, r2);
        }

        public static implicit operator Rule(string s)
        {
            if (s.Length == 1)
                return new CharRule(s[0]);
            else 
                return new StringRule(s);
        }

        public override string ToString()
        {
            return Name ?? Definition;
        }

        public ParserState Parse(string input)
        {
            var state = new ParserState { Input = input, Pos = 0 };
            if (!Match(state))
                throw new Exception(string.Format("Rule {0} failed to match", Name));
            return state;
        }

        public bool Match(string input)
        {
            var state = new ParserState { Input = input, Pos = 0 };
            return Match(state);
        }

        public virtual Rule Init(string name)
        {
            Name = name;

            // TODO: this can prevent the same API from being used in two places.
            // What i need to do is put it on the Grammar 

            // For now if two rules have the same name, I just don't give it an index
            if (RuleIds.ContainsKey(Name))
                return this; // We don't 

            Index = RuleIds.Count;
            RuleIds.Add(Name, Index);
            RuleLookup.Add(Index, this);

            return this;
        }

        public static Dictionary<string, int> RuleIds = new Dictionary<string, int>();
        public static Dictionary<int, Rule> RuleLookup = new Dictionary<int, Rule>();

        public abstract string Definition { get; }

        public Rule Then(Rule r) => new SeqRule(this, r);
        public Rule At(Rule r) => new SeqRule(this, new AtRule(r));
        public Rule Not(Rule r) => new SeqRule(this, new NotRule(r));
        public Rule Until(Rule r) => Not(r).Then(this).ZeroOrMore;
        public Rule ZeroOrMore => new ZeroOrMoreRule(this);
        public Rule OneOrMore => new OneOrMoreRule(this);

        // TODO: decide a better way to implement assertions
        // Setting assertions_off speeds things up 
#if !ASSERTIONS_OFF
        public Rule Assert => new ActionRule(this, (result, state) => {
            if (!result) throw new Exception($"Assertion failed parsing {this} while at {state}");
        });
#else
        public Rule Assert => this;
#endif

        public Rule Log => new ActionRule(this, (result, state) => {
            Console.WriteLine($"Parsing of {this} returned {result} while at {state}");
        });

        public Rule Opt => new OptRule(this);
    }

    public class ActionRule : Rule
    {
        public readonly Action<bool, ParserState> Action;

        public ActionRule(Rule r, Action<bool, ParserState> action)
            : base(r)
        {
            Action = action;
        }

        protected override bool InternalMatch(ParserState state)
        {
            var result = Child.Match(state);
            Action(result, state);
            return result;
        }

        public override string Definition => $"Action({Child})";
    }

    public class AtRule : Rule
    {
        public AtRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.GetState();
            var result = Child.Match(state);
            state.RestoreState(old);
            return result;
        }

        public override string Definition => $"At({Child})";
    }

    public class NotRule : Rule
    {
        public NotRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.GetState();
            if (Child.Match(state))
            {
                state.RestoreState(old);
                return false;
            }
            return true;
        }

        public override string Definition => $"Not({Child})";
    }

    public class AnyRule : Rule
    {
        protected override bool InternalMatch(ParserState state)
        {
            if (state.Pos >= state.Input.Length) return false;
            state.Pos++;
            return true;
        }

        public override string Definition => ".";
    }

    public class NodeRule : Rule
    {
        public NodeRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            Debug.Assert(!string.IsNullOrEmpty(Name), "Node rules must have a name");
            var tmp = state.AddNode(this, true);
            if (Child.Match(state))
            {
                state.AddNode(this, false);
                return true;
            }
            // A NodeRule is usually used in another context that will restore state. 
            // Restoring state 
            state.RestoreState(tmp);
            return false;
        }

        public override string Definition => Child.Definition;
    }

    public class RecursiveRule : Rule
    {
        readonly Func<Rule> ruleGen;

        public RecursiveRule(Func<Rule> ruleGen)
        {
            this.ruleGen = ruleGen;
        }

        public override Rule Init(string name)
        {
            if (Children.Count == 0)
                Children.Add(ruleGen());
            return base.Init(name);
        }

        protected override bool InternalMatch(ParserState state)
        {
            return Child.Match(state);
        }

        public override string ToString()
        {
            return Name ?? (Children.Count > 0 ? Children[0].ToString() : "recursive");
        }

        public override string Definition => ruleGen().Definition;
    };

    public class SeqRule : Rule
    {
        public SeqRule(params Rule[] rs)
            : base(rs)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.GetState();
            for (var i=0; i < Children.Count; ++i)
            {
                var r = Children[i];
                if (!r.Match(state))
                {
                    state.RestoreState(old);
                    return false;
                }
            }
            return true;
        }

        public override string Definition 
        {
            get 
            {
                var sb = new StringBuilder();               
                sb.Append(Children[0].ToString());
                if (Children.Count == 2 && Children[1] is SeqRule)
                {
                    sb.Append(" + ");
                    sb.Append(Children[1].Definition);
                }
                else
                {
                    for (int i=1; i < Children.Count; ++i) 
                        sb.Append(" + ").Append(Children[i].ToString());
                }
                return sb.ToString();
            }
        }

        public override string ToString() => $"({base.ToString()})";
    }

    public class ChoiceRule : Rule
    {
        public ChoiceRule(params Rule[] rs)
            : base(rs)
        {
        }

        protected override bool InternalMatch(ParserState state)
        {
            var old = state.GetState();
            for (var i = 0; i < Children.Count; ++i)
            {
                var r = Children[i];
                if (r.Match(state)) return true;
                state.RestoreState(old);
            }
            return false;
        }

        public override string Definition 
        {
            get 
            {
                var sb = new StringBuilder();               
                sb.Append(Children[0].ToString());
                if (Children.Count == 2 && Children[1] is ChoiceRule)
                {
                    sb.Append(" | ");
                    sb.Append(Children[1].Definition);
                }
                else
                {
                    for (int i=1; i < Children.Count; ++i) 
                        sb.Append(" | ").Append(Children[i].ToString());
                }
                return sb.ToString();
            }
        }

        public override string ToString() => $"({base.ToString()})";
    }

    public class EndRule : Rule
    {
        protected override bool InternalMatch(ParserState state)
        {
            return state.Pos == state.Input.Length;
        }

        public override string Definition => "_EOF_";
    }

    public class ZeroOrMoreRule : Rule
    {
        public ZeroOrMoreRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            while (Child.Match(state)) { }
            return true;
        }

        public override string Definition => string.Format("{0}*", Child);
    }

    public class OneOrMoreRule : Rule
    {
        public OneOrMoreRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            if (!Child.Match(state)) return false;
            while (Child.Match(state)) { }
            return true;
        }

        public override string Definition => $"{Child}+";
    }

    public class OptRule : Rule
    {
        public OptRule(Rule r)
            : base(r)
        { }

        protected override bool InternalMatch(ParserState state)
        {
            Child.Match(state);
            return true;
        }

        public override string Definition => $"{Child}?";
    }

    public class StringRule : Rule
    {
        readonly string s;

        public StringRule(string s)
        {
            this.s = s;
        }

        protected override bool InternalMatch(ParserState state)
        {
            if (state.Pos + s.Length > state.Input.Length)
                return false;
            for (var i = 0; i < s.Length; ++i)
            {
                var c = state.Input[state.Pos + i];
                if (c != s[i])
                    return false;
            }
            state.Pos += s.Length;
            return true;
        }

        public override string Definition => $"\"{s}\"";
    }

    public class CharRule : Rule
    {
        public char C;

        public CharRule(char c)
        {
            C = c;
        }

        protected override bool InternalMatch(ParserState state)
        {
            if (state.Pos >= state.Input.Length)
                return false;
            if (state.Input[state.Pos] != C)
                return false;
            state.Pos++;
            return true;
        }

        public override string Definition => $"[{C}]";
    }

    public class CharSetRule : Rule
    {
        public readonly string Set;

        public CharSetRule(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentException();
            Set = s;
        }

        protected override bool InternalMatch(ParserState state)
        {
            if (state.Pos >= state.Input.Length)
                return false;
            var c = state.Input[state.Pos++];
            for (var i = 0; i < Set.Length; ++i)
                if (Set[i] == c)
                    return true;
            state.Pos--;
            return false;
        }

        public override string Definition => $"[{Set}]";
    }

    public class CharRangeRule : Rule
    {
        public readonly char Lo;
        public readonly char Hi;

        public CharRangeRule(char lo, char hi)
        {
            if (lo > hi) throw new ArgumentException();
            this.Lo = lo;
            this.Hi = hi;
        }

        protected override bool InternalMatch(ParserState state)
        {
            if (state.Pos >= state.Input.Length)
                return false;
            var c = state.Input[state.Pos];
            if (c < Lo || c > Hi) return false;
            state.Pos++;
            return true;
        }

        public override string Definition => $"[{Lo}-{Hi}]";
    }

    public class RegexRule : Rule
    {
        readonly Regex re;

        public RegexRule(Regex re)
        {
            this.re = re;
        }

        protected override bool InternalMatch(ParserState state)
        {
            var m = re.Match(state.Input, state.Pos);
            if (m == null || m.Index != state.Pos) return false;
            state.Pos += m.Length;
            return true;
        }

        public override string Definition => $"regex({re})";
    }

    public class CharTableRule : Rule
    {
        public readonly bool[] Table = new bool[255];

        public CharTableRule(bool[] table)
        {
            Table = table;
        }

        public CharTableRule(IEnumerable<CharTableRule> xs) 
            : this(xs.ToArray())
        {  }

        public CharTableRule(params CharTableRule[] xs)
        {
            foreach (var x in xs)
            {
                for (var i = 0; i < x.Table.Length; ++i)
                    Table[i] |= x.Table[i];
            }
        }

        public CharTableRule(string s)
        {
            foreach (var c in s)
                Table[c] = true;
        }

        public CharTableRule(char c)
        {
            Table[c] = true;
        }

        public CharTableRule(char lo, char hi)
        {
            while (lo <= hi)
            {
                Table[lo++] = true;
            }
        }

        public CharTableRule Invert
        {
            get { return new CharTableRule(Table.Select(x => !x).ToArray()); }
        }

        public override string Definition => "_TODO_";

        protected override bool InternalMatch(ParserState state)
        {
            if (state.Pos >= state.Input.Length) return false;
            var c = state.Input[state.Pos];
            if (!Table[c]) return false;
            state.Pos++;
            return true;
        }
    }
}
