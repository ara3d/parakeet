// DO NOT EDIT: Autogenerated file created on 2024-01-27 2:01:53 AM. 
using System;
using System.Linq;

namespace Parakeet.Cst.CombinatorCalculusGrammarNameSpace
{
    /// <summary>
    /// Nodes = (Term+Term)
    /// </summary>
    public class CstApplication : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Application;
        public CstApplication(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstTerm> Term => new CstNodeFilter<CstTerm> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstCombinator : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Combinator;
        public CstCombinator(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstIdentifier : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Identifier;
        public CstIdentifier(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (Term|Application|Combinator)
    /// </summary>
    public class CstTerm : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.Term;
        public CstTerm(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstTerm> Term => new CstNodeFilter<CstTerm> (Children);
        public CstNodeFilter<CstApplication> Application => new CstNodeFilter<CstApplication> (Children);
        public CstNodeFilter<CstCombinator> Combinator => new CstNodeFilter<CstCombinator> (Children);
    }

}
