namespace Ara3D.Parakeet.Grammars
{
    public class StepGrammar : BaseCommonGrammar
    {
        public static readonly StepGrammar Instance = new StepGrammar();
        public override Rule StartRule => File;
        public override Rule WS => Named((SpaceChars | CStyleBlockComment).ZeroOrMore());
        public Rule StringChar => Named(AnyChar.Except('\''));
        public Rule String => Node(SingleQuotedString(StringChar));

        public Rule DecimalPart => Named("." + Digits.Optional());
        public Rule Number => Node(Integer + DecimalPart.Optional() + ExponentPart.Optional());
        public Rule RecValue => Recursive(nameof(Value));
        public Rule Aggregate => Node(ParenthesizedList(RecValue, null, AbortOnFail));
        public Rule UnsetAttributeValue => Node('$');
        public Rule RedeclaredAttribute => Node('*');
        public Rule Symbol => Node('.' + AbortOnFail + Identifier + '.');
        public Rule InstanceName => Node('#' + AbortOnFail + Integer);
        public Rule Value => Node(
            InstanceName | 
            Symbol | 
            Aggregate | 
            String | 
            Number | 
            UnsetAttributeValue |
            RedeclaredAttribute |
            SimpleEntityData);
        public Rule EntityDataName => Node(Identifier);
        public Rule AttributeValues => Node(ParenthesizedList(RecValue, null, AbortOnFail));
        public Rule SimpleEntityData => Node(EntityDataName + AbortOnFail + AttributeValues);
        public Rule ComplexEntityData => Node(Parenthesized(SimpleEntityData.OneOrMore()));
        public Rule EntityData => Node(ComplexEntityData | SimpleEntityData);
        public Rule DataEntry => Node(InstanceName + AbortOnFail + '=' + WS + EntityData + ';');
        public Rule DataBegin => Node("DATA;");
        public Rule DataSection => Node(DataBegin + AbortOnFail + DataEntry.ZeroOrMore() + EndSection);
        public Rule Magic => Node("ISO-10303-21;");
        public Rule EndSection => Node("ENDSEC;");
        public Rule BeginHeader => Node("HEADER;");
        public Rule Header => Node(BeginHeader + AnyCharUntilPast(EndSection));
        public Rule File => Node(Magic + AbortOnFail + Header + DataSection);
    }
}