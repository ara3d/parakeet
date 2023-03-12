using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Parakeet
{
    public static class GrammarExtensions
    {
        public static Rule Body(this Rule rule)
        {
            if (rule is NamedRule tr)
                return tr.Rule.Body();
            if (rule is RecursiveRule rr)
                return rr.Rule.Body();
            return rule;
        }

        public static void OutputDefinitions(this Grammar grammar, bool shortForm)
        {
            foreach (var r in grammar.GetRules())
            {
                var justNodes = r.Body().OnlyNodes();
                var simplified = r.Body().Optimize();
                if (justNodes != null)
                {
                    Console.WriteLine($"{r.GetName()}:");
                    Console.WriteLine($"  {r.Body().ToDefinition(shortForm)}");

                    Console.WriteLine($"OPTIMIZED:");
                    Console.WriteLine($"  {simplified.ToDefinition(shortForm)}");

                    Console.WriteLine($"JUST NODES:");
                    Console.WriteLine($"  {justNodes.ToDefinition(shortForm)}");

                    var justNodesSimplified = justNodes.Optimize();
                    Console.WriteLine($"JUST NODES OPTIMIZED:");
                    Console.WriteLine($"  {justNodesSimplified.ToDefinition(shortForm)}");
                }
            }
        }

        public static string ToDefinition(this IEnumerable<Rule> rules, string sep, bool shortForm, string indent)
            => string.Join(sep, rules.Select(r => r.ToDefinition(shortForm, indent + "  ")));

        public static string EscapeChar(this char c) =>
            char.IsLetterOrDigit(c) || char.IsSymbol(c) ? c.ToString() : $"\\x{(int)c:X2}";

        public static string EscapeChars(this string s) =>
            s.ToCharArray().EscapeChars();

        public static string EscapeChars(this char[] cs) =>
            string.Join("", cs.Select(EscapeChar));

        public static string ToDefinition(this Rule r, bool shortForm = true, string indent = "")
        {
            var nextLine = shortForm ? "" : $"\n{indent}";
            switch (r)
            {
                case NodeRule nodeRule:
                    return nodeRule.Name;
                case NamedRule nr:
                    return shortForm 
                        ? nr.Name 
                        : $"({nr.Name}:{nextLine}{nr.Rule.ToDefinition(false, indent)})";
                case Sequence seq:
                    return $"({seq.Rules.ToDefinition(nextLine + "+", shortForm, indent)})";
                case Choice ch:
                    return $"({ch.Rules.ToDefinition(nextLine + "|", shortForm, indent)})";
                case Optional opt:
                    return $"({opt.Rule.ToDefinition(shortForm, indent)})?";
                case ZeroOrMore z:
                    return $"({z.Rule.ToDefinition(shortForm, indent)})*";
                case RecursiveRule rec:
                    return rec.Rule.ToDefinition(shortForm, indent);
                case StringMatchRule sm:
                    return $"\"{sm.Pattern.EscapeChars()}\"";
                case AnyCharRule _:
                    return $"_ANY_";
                case NotAt not:
                    return $"!({not.Rule.ToDefinition(shortForm, indent)})";
                case At at:
                    return $"&({at.Rule.ToDefinition(shortForm, indent)})";
                case CharRangeRule range:
                    return $"[{range.Low.EscapeChar()}..{range.High.EscapeChar()}]";
                case CharSetRule set:
                    return $"[{set.Chars.EscapeChars()}]";
                case CharMatchRule charMatch:
                    return $"[{charMatch.Ch.EscapeChar()}]";
                case OnError set:
                    return $"_RECOVER_";
                case EndOfInputRule endOfInputRule:
                    return $"_END_";
                case LookBehind lb:
                    return $"~({lb.Rule.ToDefinition(shortForm, indent)})";
                default:
                    return "_UNKNOWN_";
            }
        }

        public static bool HasNode(this Rule r)
        {
            switch (r)
            {
                case NodeRule nr:
                    return true;
                case Sequence seq:
                    return seq.Rules.Any(HasNode);
                case Choice ch:
                    return ch.Rules.Any(HasNode);
                case Optional opt:
                    return opt.Rule.HasNode();
                case ZeroOrMore z:
                    return z.Rule.HasNode();
                case RecursiveRule rec:
                    return rec.Rule.HasNode();
                default:
                    return false;
            }
        }

        public static IEnumerable<Rule> ChildrenWithNodes(this Rule r, bool top = false)
        {
            switch (r)
            {
                case NodeRule nr:
                    return top ? nr.Rule.ChildrenWithNodes() : new[] { r };
                case Sequence seq:
                    return seq.Rules.SelectMany(r1 => r1.ChildrenWithNodes());
                case Choice ch:
                    return ch.Rules.SelectMany(r1 => r1.ChildrenWithNodes());
                case Optional opt:
                    return opt.Rule.ChildrenWithNodes();
                case ZeroOrMore z:
                    return z.Rule.ChildrenWithNodes();
                case RecursiveRule rec:
                    return rec.Rule.ChildrenWithNodes();
                default:
                    return Enumerable.Empty<Rule>();
            }
        }

        public static Rule MergeChoiceRule(Rule r1, Rule r2)
        {
            // A|A* => A*
            {
                if (r2 is ZeroOrMore z && z.Rule.Equals(r1))
                    return z;
            }
            // A*|A => A*
            {
                if (r1 is ZeroOrMore z && z.Rule.Equals(r2))
                    return z;
            }
            return null;
        }

        public static Rule OnlyNodes(this Rule r)
        {
            switch (r)
            {
                case NodeRule nr:
                    return nr;

                case Sequence seq:
                    {
                        var tmp = seq.Rules.Select(r1 => r1.OnlyNodes()).Where(x => x != null).ToList();
                        if (tmp.Count > 0)
                            return new Sequence(tmp.ToArray());
                        break;
                    }

                case Choice ch:
                    {
                        var tmp = ch.Rules.Select(r1 => r1.OnlyNodes()).Where(x => x != null).ToList();

                        if (tmp.Count > 0)
                            return new Choice(tmp.ToArray());
                        break;
                    }
                case Optional opt:
                    {
                        var tmp = opt.Rule.OnlyNodes();
                        if (tmp != null)
                            return new Optional(tmp);
                        break;
                    }
                case ZeroOrMore z:
                    {
                        var tmp = z.Rule.OnlyNodes();
                        if (tmp != null)
                            return new ZeroOrMore(tmp);
                        break;
                    }
                case RecursiveRule rec:
                    {
                        var tmp = rec.Rule.OnlyNodes();
                        if (tmp != null)
                            return tmp;
                        break;
                    }
            }
            return null;
        }

        public static Rule Optimize (this Rule r)
        {
            switch (r)
            {
                case RecursiveRule rr:
                    return rr;

                case NodeRule nodeRule:
                    return nodeRule;

                case NamedRule namedRule:
                    return namedRule.Rule.Optimize();

                case At at:
                    {
                        var inner = at.Rule.Optimize();
                        if (inner is NamedRule nr)
                            return nr.Rule;
                        if (inner is At at2)
                            return at.Rule;
                        if (inner is At notAt)
                            return notAt.Rule;
                        return new At(inner);
                    }

                case NotAt notAt:
                    {
                        var inner = notAt.Rule.Optimize();
                        if (inner is NamedRule nr)
                            return nr.Rule;
                        if (inner is At at)
                            inner = at.Rule;
                        if (inner is At notAt2)
                            return new At(notAt2.Rule);
                        return new NotAt(inner);
                    }

                case Sequence seq:
                    {
                        // (A + B) + C => A + B + C
                        // TODO: A* + A* => A*                         
                        var tmp = seq.Rules.SelectMany(
                            r1 =>
                            {
                                var tmp1 = r1.Optimize();
                                if (tmp1 is Sequence seq1)
                                    return seq1.Rules;
                                return new[] { tmp1 };
                            }).ToArray();
                        Debug.Assert(tmp.Length > 0);
                        Debug.Assert(!tmp.Any(t => t is Sequence));
                        if (tmp.Length == 1)
                            return tmp[0];
                        return new Sequence(tmp);
                    }

            case Choice ch:
                {
                    // (A | B) | C => A | B | C
                    var tmp = ch.Rules.SelectMany(
                        r1 =>
                        {
                            var tmp1 = r1.Optimize();
                            if (tmp1 is Choice ch1)
                                return ch1.Rules;
                            return new[] { tmp1 };
                        }).ToArray();

                    Debug.Assert(tmp.Length > 0);
                    Debug.Assert(!tmp.Any(t => t is Choice));
                    if (tmp.Length == 1)
                        return tmp[0];
                   
                    return new Choice(tmp);
                    /*
                    var list = new List<Rule>();
                    for (var i=0; i < tmp.Length - 1; ++i)
                    {
                        var r1 = tmp[i];
                        var r2 = tmp[i+1];
                        var r3 = MergeChoiceRule(r1, r2);
                        if (r3 == null)
                        {
                            list.Add(r1);
                        }
                        else
                        {
                            list.Add(r3);
                            i++;
                        }
                    }

                    return new Choice(list.ToArray());
                   */
                }                    

                case Optional opt:
                    {
                        var inner = opt.Rule.Optimize();

                        // A?? => A?
                        if (inner is Optional opt1)
                            return opt1.Rule;

                        // A*? => A*
                        if (inner is ZeroOrMore)
                            return inner;

                        if (inner is Sequence seq)
                        {
                            if (seq.Count == 2)
                            {
                                //  (A + A*)? => A*
                                if (seq[1] is ZeroOrMore z1)
                                    if (seq[0].Equals(z1.Rule))
                                        return z1;

                                //  (A* + A)? => A*
                                if (seq[0] is ZeroOrMore z2)
                                    if (seq[1].Equals(z2.Rule))
                                        return z2;
                            }
                        }

                        return new Optional(inner);
                    }

                case ZeroOrMore z:
                    {
                        var inner = z.Rule.Optimize();

                        // A?* => A*
                        if (inner is Optional opt)
                            return new ZeroOrMore(opt.Rule);

                        // A** => A*
                        if (inner is ZeroOrMore z1)
                            return z1;
                        
                        return new ZeroOrMore(inner);
                    }
            }
            return r;
        }
    }
}
