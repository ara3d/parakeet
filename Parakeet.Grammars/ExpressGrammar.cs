namespace Ara3D.Parakeet.Grammars
{
    public class ExpressGrammar : BaseCommonGrammar
    {
        public static readonly ExpressGrammar Instance = new ExpressGrammar();

        public Rule Schema;
        public Rule Entity;
        
        public Rule SubTypes;
        public Rule SuperTypes;
        public Rule Inverse;
        public Rule Where;
        public Rule Unique;

        public Rule Field;
        public Rule FieldName;
        public Rule OptionalKeyword;
        public Rule FieldType;

        public Rule Type;
        public Rule Enumeration;
        public Rule Derivation;
        public Rule Function;
        public Rule Comment;
        public Rule String;
    }
}