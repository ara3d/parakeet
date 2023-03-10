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
        public Rule WhitespaceRule { get; protected set; }

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
                .Select(pi => pi.GetValue(this) as Rule);

        public static Rule Choice(IEnumerable<Rule> rules)
            => new Choice(rules.ToArray());

        public static Rule Sequence(IEnumerable<Rule> rules)
            => new Sequence(rules.ToArray());

        public static Rule Recursive(Func<Rule> f)
            => new RecursiveRule(f);

        public Rule Named(Rule r, [CallerMemberName] string name = "")
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentException("Name must not be null");
            if (Lookup.ContainsKey(name)) 
                return Lookup[name];
            r = r.Simplify();
            r = new NamedRule(r, name);
            Lookup.Add(name, r);
            return r;
        }

        public Rule Strings(params string[] values)
            => new Choice(values.OrderByDescending(v => v.Length).Select(v => (Rule)v).ToArray());

        public Rule Node(Rule r, [CallerMemberName] string name = "")
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentException("Name must not be null");
            if (Lookup.ContainsKey(name)) 
                return Lookup[name];
            r = r.Simplify();
            r = new NodeRule(r, WhitespaceRule, name);
            Lookup.Add(name, r);
            return r;
        }

        public OnError OnError(Rule r)
            => new OnError(r);       

        public Dictionary<string, Rule> Lookup = new Dictionary<string, Rule>();

        public Rule CharSet(params char[] chars)
            => new CharSetRule(chars);

        public Rule CharSet(string chars)
            => new CharSetRule(chars.ToCharArray());

        public Rule After(Rule r)
            => new LookBehind(r);

        public Rule ZeroOrMore(Rule r)
            => new ZeroOrMore(r);

        public Rule OneOrMore(Rule r) 
            => r + r.ZeroOrMore();
        
        public Rule TwoOrMore(Rule r) 
            => r + r + r.ZeroOrMore();
        
        public Rule ThreeOrMore(Rule r) 
            => r + r + r + r.ZeroOrMore();
        
        public Rule Optional(Rule r) 
            => r.Optional();
        
        public Rule Not(Rule r) 
            => r.NotAt();
    }

}