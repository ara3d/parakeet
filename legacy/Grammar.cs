using System;
using System.Collections.Generic;
using System.IO;

namespace Parakeet
{
    public class Grammar
    {
        public static NodeRule Node(Rule rule)
        {
            return new NodeRule(rule);
        }

        public static RecursiveRule Recursive(Func<Rule> ruleGen)
        {
            return new RecursiveRule(ruleGen);
        }

        public static AtRule At(Rule rule)
        {
            return new AtRule(rule);
        }

        public static SeqRule Seq(params Rule[] rs)
        {
            return new SeqRule(rs);
        }

        public static ChoiceRule Choice(params Rule[] rs)
        {
            return new ChoiceRule(rs);
        }

        public static EndRule End = new EndRule();

        public static NotRule Not(Rule r)
        {
            return new NotRule(r);
        }

        public static ZeroOrMoreRule ZeroOrMore(Rule r)
        {
            return new ZeroOrMoreRule(r);
        }

        public static OneOrMoreRule OneOrMore(Rule r)
        {
            return new OneOrMoreRule(r);
        }

        public static OptRule Opt(Rule r)
        {
            return new OptRule(r);
        }

        public static StringRule MatchString(string s)
        {
            return new StringRule(s);
        }

        public static Rule MatchChar(char c)
        {
            return new CharRule(c);
        }

        public static Rule CharSet(string s)
        {
            return new CharRule(s);
        }

        public static Rule CharRange(char a, char b)
        {
            return new CharRule(a, b);
        }

        public static Rule AnyChar = new AnyRule();

        public static Rule AdvanceWhileNot(Rule r)
        {
            if (r == null) throw new ArgumentNullException();
            return ZeroOrMore(Seq(Not(r), AnyChar));
        }

        public static IEnumerable<Rule> GetRules(Type type)
        {
            foreach (System.Reflection.FieldInfo fi in type.GetFields())
                if (fi.FieldType.Equals(typeof(Rule)))
                    yield return (Rule)fi.GetValue(null);
        }

        /// <summary>
        /// Provides a name to all fields of type Rule.
        /// </summary>
        public static void InitGrammar(Type type)
        {
            foreach (System.Reflection.FieldInfo fi in type.GetFields())
            {
                if (fi.FieldType.Equals(typeof(Rule)))
                {
                    if (!(fi.GetValue(null) is Rule rule))
                        throw new Exception("Unexpected null rule");
                    rule.Init(fi.Name);
                    Optimizer.Optimize(rule);
                }
            }
        }

        public static void OutputGrammar(Type type, TextWriter tw)
        {
            foreach (Rule r in GetRules(type))
                tw.WriteLine("{0} <- {1}", r.Name, r.Definition);
        }

        public static void OutputGrammar(Type type)
        {
            OutputGrammar(type, Console.Out);
        }
    }
}
