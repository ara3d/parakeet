namespace Ara3D.Parakeet.Grammars
{
    // https://en.wikipedia.org/wiki/SKI_combinator_calculus
    public class CombinatorCalculusGrammar : BaseCommonGrammar
    {
        public static readonly CombinatorCalculusGrammar Instance 
            = new CombinatorCalculusGrammar();
        public override Rule StartRule
            => Term;
        public Rule Combinator 
            => Node(Letter);
        public Rule Application 
            => Node(Recursive(nameof(Term)) + Recursive(nameof(Term)));
        public Rule Term 
            => Node(Parenthesized(Recursive(nameof(Term)) | Application | Combinator));
    }
}