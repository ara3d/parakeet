using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Parakeet
{
    /// <summary>
    /// A class inheriting from grammar contains a set of parsing rules. Each parse rule is defined as 
    /// property getters returning a rule created by "Token" (generating no node) or "Phrase" (generating a node).  
    /// This class will store the rules as they are created, and assign them names 
    /// so that they can have recursive relations in them, have fixed names based on the properties, 
    /// and minimizes creating superfluous objects. 
    /// </summary>
    // [Mutable]
    public class Grammar
    {
        public virtual Rule WS { get; } 
        public virtual Rule Recovery { get; }

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
                .Where(pi => typeof(Rule).IsAssignableFrom(pi.PropertyType))
                .Select(pi => pi.GetValue(this) as Rule)
                .Where(r => r != null);

        public static Rule Choice(IEnumerable<Rule> rules)
            => new ChoiceRule(rules.ToArray());

        public static Rule Sequence(IEnumerable<Rule> rules)
            => new SequenceRule(rules.ToArray());

        public Rule Recursive(string inner, [CallerMemberName] string nodeName = "")
            => Node(new RecursiveRule(() => GetRuleFromName(inner)), nodeName);

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
            //r = r.Optimize();
            r = new NodeRule(r, name);
            if (WS != null)
                r = r.Then(WS);
            Lookup.Add(name, r);
            return r;
        }

        public OnError OnError(Rule r)
            => new OnError(r);       

        public Dictionary<string, Rule> Lookup = new Dictionary<string, Rule>();

        public Rule CharSet(params char[] chars)
            => chars.Length == 1 ? (Rule)chars[0] : new CharSetRule(chars);

        public Rule CharSet(string chars)
            => CharSet(chars.ToCharArray());

        public Rule ZeroOrMore(Rule r)
            => new ZeroOrMoreRule(r);

        public Rule OneOrMore(Rule r) 
            => new OneOrMoreRule(r);
        
        public Rule Optional(Rule r) 
            => r.Optional();
        
        public Rule Not(Rule r) 
            => r.NotAt();
    }

}