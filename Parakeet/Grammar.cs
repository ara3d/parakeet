using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ara3D.Parakeet
{
    /// <summary>
    /// A class inheriting from grammar contains a set of parsing rules. Each parse rule is defined as 
    /// property getters returning a rule created by "Token" (generating no node) or "Phrase" (generating a node).  
    /// This class will store the rules as they are created, and assign them names 
    /// so that they can have recursive relations in them, have fixed names based on the properties, 
    /// and minimizes creating superfluous objects. 
    /// </summary>
    public abstract class Grammar : IGrammar
    {
        public abstract Rule StartRule { get; }
        public virtual Rule WS => BooleanRule.True;
        public readonly Dictionary<string, Rule> Lookup = new Dictionary<string, Rule>();

        public Rule GetRuleFromName(string name)
        {
            var t = GetType();
            var pi = t.GetProperties().FirstOrDefault(p => p.Name == name);
            if (pi == null) return null;
            return pi.GetValue(this) as Rule;
        }

        public IEnumerable<Rule> GetRules()
            => GetType()
                .GetProperties()
                .OrderBy(p => p.Name)
                .Where(pi => typeof(Rule).IsAssignableFrom(pi.PropertyType))
                .Where(p => p.Name != "StartRule")
                .Select(pi => pi.GetValue(this) as Rule)
                .Where(r => r != null);

        public static Rule Choice(IEnumerable<Rule> rules)
            => new ChoiceRule(rules.ToArray());

        public static Rule Sequence(IEnumerable<Rule> rules)
            => new SequenceRule(rules.ToArray());

        public Rule Recursive(string inner)
            => new RecursiveRule(() => GetRuleFromName(inner));
        public Rule Recursive(Func<Rule> func)
            => new RecursiveRule(func);

        public Rule Named(Rule r, [CallerMemberName] string name = "")
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentException("Name must not be null");
            if (Lookup.ContainsKey(name)) 
                return Lookup[name];
            r = new NamedRule(r, name);
            Lookup.Add(name, r);
            return r;
        }

        public Rule Strings(params string[] values)
            => new ChoiceRule(values.OrderByDescending(v => v.Length).Select(v => (Rule)v).ToArray());

        public Rule Node(Rule r, [CallerMemberName] string name = "")
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentException("Name must not be null");
            if (Lookup.ContainsKey(name)) 
                return Lookup[name];
            if (WS != null)
                r = r.Then(WS);
            r = new NodeRule(r, name);
            Lookup.Add(name, r);
            return r;
        }

        public static OnFail OnFail(Rule r)
            => new OnFail(r);       

        public static Rule CharSet(params char[] chars)
            => chars.Length == 1 ? (Rule)chars[0] : new CharSetRule(chars);

        public static Rule CharSet(string chars)
            => CharSet(chars.ToCharArray());

        public static Rule ZeroOrMore(Rule r)
            => new ZeroOrMoreRule(r);

        public static Rule OneOrMore(Rule r) 
            => new OneOrMoreRule(r);
        
        public static Rule Optional(Rule r) 
            => r.Optional();
        
        public static Rule Not(Rule r) 
            => r.NotAt();

        public Rule CaseInvariant(string s)
            => new CaseInvariantStringRule(s);

        public Rule CharRange(int from, int to)
            => new CharRangeRule((char)from, (char)to);
    }

}