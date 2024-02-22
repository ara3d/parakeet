namespace Ara3D.Parakeet.Grammars
{
    // https://en.wikipedia.org/wiki/Lambda_calculus
    public class SimpleLambdaCalculusGrammar : BaseCommonGrammar
    {
        public static readonly SimpleLambdaCalculusGrammar Instance 
            = new SimpleLambdaCalculusGrammar();
        public override Rule StartRule
            => Term;
        public Rule Variable 
            => Node(Identifier);
        public Rule Parameter 
            => Node("\\" + Variable);
        public Rule Abstraction 
            => Node("(" + Parameter + "." + Term + ")");
        public Rule InnerTerm 
            => Node(Variable | Abstraction | Application);
        public Rule Term
            => Node(Recursive(nameof(InnerTerm)));
        public Rule Application 
            => Node("(" + Term + WS + Term + ")");
    }
}