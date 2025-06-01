using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Ara3D.Parakeet
{
    public class RuleOptimizer
    {
        public Dictionary<Rule, Rule> OptimizedRules = new Dictionary<Rule, Rule>();

        private readonly TextWriter Logger;

        public RuleOptimizer(Grammar g, TextWriter logger = null) :
            this(logger)
        {
            foreach (var r in g.GetRules())
            {
                OptimizedRules[r] = Optimize(r);
            }
        }

        public RuleOptimizer(TextWriter logger = null)
        {
            Logger = logger ?? TextWriter.Null;
        }

        public Rule Log(Rule r1, Rule r2, Rule result, string desc)
        {
            if (r2 == null)
            {
                Logger.WriteLine($"{desc}: {r1.ToDefinition()} into {result.ToDefinition()}");
            }
            else
            {
                Logger.WriteLine($"{desc}: {r1.ToDefinition()} with {r2.ToDefinition()} into {result.ToDefinition()}");
            }

            return result;
        }

        public static Rule First(SequenceRule seq)
        {
            return seq[0];
        }

        public static Rule Tail(SequenceRule seq)
        {
            if (seq.Count < 2) return null;
            if (seq.Count == 2) return seq[1];
            return new SequenceRule(seq.Rules.Skip(1).ToArray());
        }

        public Rule MergeChoiceRule(Rule r1, Rule r2)
        {
            {
                if (r1 is OptionalRule opt)
                    return Log(r1, r2, opt, "A?|x => A?");
            }
            {
                if (r1 is ZeroOrMoreRule z)
                    return Log(r1, r2, z, "A*|x => A*");
            }
            {
                if (r2 is OptionalRule opt && opt.Rule == r1)
                    return Log(r1, r2, opt, "A|A? => A?");
            }
            {
                if (r1 is SequenceRule seq && seq.Count == 2 && seq[0] == r2)
                    return Log(r1, r2, seq[0].Then(r2.Optional()), "A+B|A => A+B?");
            }
            {
                if (r2 is SequenceRule seq && seq.Rules.Length > 1 && seq[0] == r1)
                    return Log(r1, r2, r1, "A|A+B => A");
            }
            {
                if (r1 is SequenceRule seq1 && r2 is SequenceRule seq2)
                {
                    if (seq1[0] == seq2[0] && seq1.Count > 1 && seq2.Count > 1)
                    {
                        return Log(r1, r2, seq1[0].Then(Tail(seq1) | Tail(seq2)), "A+B|A+C => A+(B|C)");
                    }
                }
            }
            {
                if (r1 is CharSetRule csr1)
                {
                    if (r2 is CharSetRule csr2)
                    {
                        var result = csr1.Union(csr2);
                        return Log(r1, r2, result, "[ab]|[cd] => [abcd]");
                    }

                    if (r2 is CharRule cr && cr.Char < 128)
                    {
                        var result = csr1.Append(cr.Char);
                        return Log(r1, r2, result, "[ab]|c => [abc]");
                    }
                }
            }
            {
                if (r1 is CharRule cr)
                {
                    if (r2 is CharSetRule csr)
                    {
                        var result = csr.Append(cr.Char);
                        return Log(r1, r2, result, "a|[bc] => [abc]");
                    }
                    if (r2 is CharRule cr1)
                    {
                        var result = new CharSetRule(cr.Char, cr1.Char);
                        return Log(r1, r2, result, "a|b => [ab]");
                    }
                }

            }
            return null;
        }

        public Rule MergeSequenceRule(Rule r1, Rule r2)
        {
            {
                if (r1 is StringRule sr1)
                {
                    if (r2 is StringRule sr2)
                        return Log(r1, r2, sr1.Pattern + sr2.Pattern, "ab+cd => abcd");
                    
                    if (r2 is CharRule cr)
                        return Log(r1, r2, sr1.Pattern + cr.Char, "ab+c => abc");
                }
            }

            {
                if (r1 is CharRule cr)
                {
                    if (r2 is StringRule sr)
                        return Log(r1, r2, cr.Char + sr.Pattern, "a+bc => abc");
                }
            }

            {
                if (r1 is ZeroOrMoreRule z)
                {
                    if (r1 == r2)
                        return Log(r1, r2, r1, "A*+A* => A*");

                    if (z.Rule == r2)
                        return Log(r1, r2, r2.OneOrMore(), "A*+A => A+");
                    
                    if (r2 is OptionalRule opt && z.Rule == opt.Rule)
                        return Log(r1, r2, r1, "A*+A? => A*");
                }
            }

            {
                if (r2 is ZeroOrMoreRule z)
                {
                    if (r1 == z.Rule)
                        return Log(r1, r2, r1.OneOrMore(), "A+A* => A+");
                }
            }
            return null;
        }

        public Rule Optimize(Rule r)
        {
            if (OptimizedRules.ContainsKey(r))
                return OptimizedRules[r];
            var opt = OptimizeImplementation(r);
            return OptimizedRules[r] = opt;
        }

        private Rule OptimizeImplementation(Rule r)
        {
            switch (r)
            {
                case RecursiveRule rr:
                    return rr;

                case NodeRule nodeRule:
                    return nodeRule;

                case NamedRule namedRule:
                    return Optimize(namedRule.Rule);

                case StringRule stringRule:
                    if (stringRule.Pattern.Length == 1)
                        return new CharRule(stringRule.Pattern[0]);
                    break;

                case AtRule at:
                {
                    var inner = Optimize(at.Rule);

                    if (inner is AtRule)
                        return Log(r, null, inner, "&&A => &A");
                    
                    if (inner is NotAtRule notAt)
                        return Log(r, null, notAt.Rule, "&!A => !A");

                    return new AtRule(inner);
                }

                case NotAtRule notAt:
                {
                    var inner = Optimize(notAt.Rule);
                    
                    if (inner is AtRule at)
                        return Log(r, null, new NotAtRule(at.Rule), "!&A => !A");
                    
                    if (inner is NotAtRule notAt2)
                        return Log(r, null, new AtRule(notAt2.Rule), "!!A => &A");

                    return new NotAtRule(inner);
                }

                case SequenceRule seq:
                {
                    var list = seq.Rules.SelectMany(
                        r1 =>
                        {
                            var tmp1 = Optimize(r1);
                            if (tmp1 is SequenceRule seq1)
                                return seq1.Rules;
                            return new[] { tmp1 };
                        }).ToList();
                    
                    Debug.Assert(list.Count > 0);
                    Debug.Assert(!list.Any(t => t is SequenceRule));

                    for (var i = 0; i < list.Count - 1; )
                    {
                        var r1 = list[i];
                        var r2 = list[i + 1];
                        var r3 = MergeSequenceRule(r1, r2);
                        if (r3 == null)
                        {
                            i++;
                        }
                        else
                        {
                            list[i] = Optimize(r3);
                            list.RemoveAt(i+1);
                            if (i > 0) i--;
                        }
                    }

                    Debug.Assert(list.Count > 0);
                    Debug.Assert(!list.Any(t => t is SequenceRule));

                    if (list.Count == 1)
                        return Log(r, null, list[0], "(A+_) => A");

                    return new SequenceRule(list.ToArray());
                }

                case ChoiceRule ch:
                {
                    var list = ch.Rules.SelectMany(
                        r1 =>
                        {
                            var tmp1 = Optimize(r1);
                            if (tmp1 is ChoiceRule seq1)
                                return seq1.Rules;
                            return new[] { tmp1 };
                        }).ToList();

                    Debug.Assert(list.Count > 0);
                    Debug.Assert(!list.Any(t => t is ChoiceRule));

                    for (var i = 0; i < list.Count - 1;)
                    {
                        var r1 = list[i];
                        var r2 = list[i + 1];
                        var r3 = MergeChoiceRule(r1, r2);
                        if (r3 == null)
                        {
                            i++;
                        }
                        else
                        {
                            list[i] = Optimize(r3);
                            list.RemoveAt(i + 1);
                            if (i > 0) i--;
                        }
                    }

                    Debug.Assert(list.Count > 0);
                    Debug.Assert(!list.Any(t => t is ChoiceRule));

                    if (list.Count == 1)
                        return Log(r, null, list[0], "(A|_) => A");

                    return new ChoiceRule(list.ToArray());
                }

                case OptionalRule opt:
                {
                    var inner = Optimize(opt.Rule);

                    if (inner is OptionalRule opt1)
                        return Log(r, null, opt1.Rule, "A?? => A?");

                    if (inner is ZeroOrMoreRule)
                        return Log(r, null, inner, "A*? => A*");

                    if (inner is OneOrMoreRule o)
                        return Log(r, null, o.Rule.ZeroOrMore(), "A+? => A*");

                    if (inner is SequenceRule seq)
                    {
                        if (seq.Count == 2)
                        {
                            //  (A + A*)? => A*
                            if (seq[1] is ZeroOrMoreRule z1 && seq[0] == z1.Rule)
                                return Log(r, null, z1, "(A+A*)? => A*");
                        }
                    }

                    return new OptionalRule(inner);
                }

                case ZeroOrMoreRule z:
                {
                    var inner = Optimize(z.Rule);

                    if (inner is OptionalRule opt)
                        return Log(r, null, new ZeroOrMoreRule(opt.Rule), "A?* => A*");

                    if (inner is ZeroOrMoreRule z1)
                        return Log(r, null, z1, "A** => A*");
                        
                    return new ZeroOrMoreRule(inner);
                }
            }

            return r;
        }
    }
}