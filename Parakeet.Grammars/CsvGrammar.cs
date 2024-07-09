namespace Ara3D.Parakeet.Grammars
{
    // https://github.com/antlr/grammars-v4/blob/master/csv/CSV.g4
    public class CsvGrammar : BaseCommonGrammar
    {
        public static readonly CsvGrammar Instance = new CsvGrammar();
        public override Rule StartRule => File;

        public Rule StringChar => Named(AnyChar.Except('"') | "\"\"");
        public Rule String => Node(DoubleQuotedString(StringChar));
        public Rule Text => Node(AnyChar.Except(",\n\r\"").OneOrMore());
        public Rule Field => Node(Text | String);
        public Rule Row => Node(Field.ZeroOrMore() + Optional('\r') + '\n');
        public Rule File => Node(Row.ZeroOrMore());
    }

    // https://github.com/antlr/grammars-v4/blob/master/csv/CSV.g4
}