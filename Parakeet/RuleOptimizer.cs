using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Parakeet
{
    public static class RuleOptimizer
    {
        public static Rule Log(Rule r1, Rule r2, Rule result, string desc)
        {
            if (r2 == null)
            {
                Console.WriteLine($"{desc}: {r1.ToDefinition()} into {result.ToDefinition()}");
            }
            else
            {
                Console.WriteLine($"{desc}: {r1.ToDefinition()} with {r2.ToDefinition()} into {result.ToDefinition()}");
            }

            return result;
        }

        public static Rule MergeChoiceRule(Rule r1, Rule r2)
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
                if (r1 is CharSetRule csr1)
                {
                    if (r2 is CharSetRule csr2)
                    {
                        var result = new CharSetRule(csr1.Chars.Concat(csr2.Chars).ToArray());
                        return Log(r1, r2, result, "[ab]|[cd] => [abcd]");
                    }

                    if (r2 is CharRule cr)
                    {
                        var result = new CharSetRule(csr1.Chars.Append(cr.Char).ToArray());
                        return Log(r1, r2, result, "[ab]|c => [abc]");
                    }
                }
            }
            {
                if (r1 is CharRule cr)
                {
                    if (r2 is CharSetRule csr)
                    {
                        var result = new CharSetRule(csr.Chars.Append(cr.Char).ToArray());
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

        public static Rule MergeSequenceRule(Rule r1, Rule r2)
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
                        return Log(r1, r2, r1, "A*+A => A*");
                    
                    if (r2 is OptionalRule opt)
                        return Log(r1, r2, r1, "A*+A? => A");
                }
            }
            return null;
        }

        public static Rule Optimize(this Rule r)
        {
            switch (r)
            {
                case RecursiveRule rr:
                    return rr;

                case NodeRule nodeRule:
                    return nodeRule;

                case NamedRule namedRule:
                    return Log(r, null, namedRule.Rule.Optimize(), "Named rule elimination");

                case AtRule at:
                {
                    var inner = at.Rule.Optimize();

                    if (inner is AtRule)
                        return Log(r, null, inner, "&&A => &A");
                    
                    if (inner is NotAtRule notAt)
                        return Log(r, null, notAt.Rule, "&!A => !A");

                    return Log(r, null, new AtRule(inner), "");
                }

                case NotAtRule notAt:
                {
                    var inner = notAt.Rule.Optimize();
                    
                    if (inner is AtRule at)
                        return Log(r, null, new NotAtRule(at.Rule), "!&A => !A");
                    
                    if (inner is NotAtRule notAt2)
                        return Log(r, null, new AtRule(notAt2.Rule), "!!A => &A");

                    return Log(r, null, new NotAtRule(inner), "");
                }

                case SequenceRule seq:
                {
                    var list = seq.Rules.SelectMany(
                        r1 =>
                        {
                            var tmp1 = r1.Optimize();
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
                            list[i] = r3.Optimize();
                            list.RemoveAt(i+1);
                            if (i > 0) i--;
                        }
                    }

                    Debug.Assert(list.Count > 0);
                    Debug.Assert(!list.Any(t => t is SequenceRule));

                    if (list.Count == 1)
                        return Log(r, null, list[0], "(A+_) => A");

                    return Log(r, null, new SequenceRule(list.ToArray()), "");
                }

                case ChoiceRule ch:
                {
                    var list = ch.Rules.SelectMany(
                        r1 =>
                        {
                            var tmp1 = r1.Optimize();
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
                            list[i] = r3.Optimize();
                            list.RemoveAt(i + 1);
                            if (i > 0) i--;
                        }
                    }

                    Debug.Assert(list.Count > 0);
                    Debug.Assert(!list.Any(t => t is ChoiceRule));

                    if (list.Count == 1)
                        return Log(r, null, list[0], "(A+_) => A");

                    return Log(r, null, new ChoiceRule(list.ToArray()), "");
                }

                case OptionalRule opt:
                {
                    var inner = opt.Rule.Optimize();

                    if (inner is OptionalRule opt1)
                        return Log(r, null, opt1.Rule, "A?? => A?");

                    if (inner is ZeroOrMoreRule)
                        return Log(r, null, inner, "A*? => A*");

                    if (inner is SequenceRule seq)
                    {
                        if (seq.Count == 2)
                        {
                            //  (A + A*)? => A*
                            if (seq[1] is ZeroOrMoreRule z1 && seq[0] == z1.Rule)
                                return Log(r, null, z1, "(A+A*)? => A*");
                        }
                    }

                    return Log(r, null, new OptionalRule(inner), "");
                }

                case ZeroOrMoreRule z:
                {
                    var inner = z.Rule.Optimize();

                    if (inner is OptionalRule opt)
                        return Log(r, null, new ZeroOrMoreRule(opt.Rule), "A?* => A*");

                    if (inner is ZeroOrMoreRule z1)
                        return Log(r, null, z1, "A** => A*");
                        
                    return Log(r, null, new ZeroOrMoreRule(inner), "");
                }
            }
            return r;
        }
    }
}