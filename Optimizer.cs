using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parakeet
{
    public static class Optimizer
    {
        /// <summary>
        /// Todo: make this actually create new rules rather than just modify existing ones.
        /// </summary>
        public static Rule Optimize(Rule r)
        {
            if (r is RecursiveRule)
                return r;
            r.Children = r.Children.Select(Optimize).ToList();

            if (r is SeqRule)
            {
                var children = new List<Rule>();
                foreach (var x in r.Children)
                {
                    if (x is SeqRule)
                    {
                        children.AddRange(x.Children);
                    }
                    else
                    {
                        children.Add(x);
                    }
                }
                r.Children = children;
                return r;
            }
            if (r is ChoiceRule)
            {
                var children = new List<Rule>();
                foreach (var x in r.Children)
                {
                    if (x is ChoiceRule)
                    {
                        children.AddRange(x.Children);
                    }
                    else
                    {
                        children.Add(x);
                    }
                }

                // Merge all of the char table rules 
                var charTableRules = children.Where(x => x is CharTableRule).Select(x => x as CharTableRule);
                var newList = children.Where(x => !(x is CharTableRule)).ToList();
                newList.Add(new CharTableRule(charTableRules));
                r.Children = newList;
                return r;
            }
            if (r is CharSetRule cs)
            {
                return new CharTableRule(cs.Set);
            }
            if (r is CharRangeRule cr)
            {
                return new CharTableRule(cr.Lo, cr.Hi);
            }
            if (r is CharRule c)
            {
                return new CharTableRule(c.C);
            }
            return r;
        }
    }
}
