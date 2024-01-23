namespace Parakeet.Grammars.WIP
{
    public class CsvGrammar : CommonGrammar
    {
        public Rule StringChar => AnyChar.Except('"') | "\"\"";
        public Rule String => Node('"' + StringChar.ZeroOrMore() + '"');
        public Rule Text => AnyChar.Except(",\n\r\"").OneOrMore();
        public Rule Field => Text | String;
        public Rule Row => Node(Field.ZeroOrMore() + Optional('\r') + '\n');
        public Rule File => Row.ZeroOrMore();
    }
}