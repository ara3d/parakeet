namespace Ara3D.Parakeet.Tests
{
    internal class TestGrammar : Grammar
    {
        public static TestGrammar Instance { get; } = new TestGrammar();
        public override Rule StartRule => null;

        public Rule ReferenceSelfByName => Node(Recursive(nameof(ReferenceSelfByName)));
        public Rule ReferenceSelfByProperty => Node(Recursive(() => ReferenceSelfByProperty));
    }

    public static class RuleTests
    {
        private static Rule[] TestRules = new[] { new StringRule("test1"), new StringRule("test2"), };
        private static Rule TestRuleProp => new StringRule(nameof(TestRuleProp));
        private static Rule TestRuleFunc() => new StringRule(nameof(TestRuleFunc));
        private static RecursiveRule GetRecursiveRuleLambda() => new RecursiveRule(() => TestRuleProp);
        private static RecursiveRule GetRecursiveRuleMethod() => new RecursiveRule(() => TestRuleFunc());
        private static RecursiveRule GetRecursiveRuleLambda(int n) => new RecursiveRule(() => TestRules[n]);

        public static IEnumerable<(Rule, Rule, string)> RecursiveRuleEquals
        {
            get
            {
                var g1 = new TestGrammar();
                var g2 = new TestGrammar();
                var n = new NamedRule("test", "test");

                yield return (new RecursiveRule(() => TestRuleProp), new RecursiveRule(() => TestRuleProp), "Lambda returning same rule");
                yield return (new RecursiveRule(TestRuleFunc), new RecursiveRule(TestRuleFunc), "Referencing same method");
                yield return (new RecursiveRule(() => n), new RecursiveRule(() => n), "Reference same lambda");
                yield return (GetRecursiveRuleLambda(), GetRecursiveRuleLambda(), "Same method returns (Lambda)");
                yield return (GetRecursiveRuleMethod(), GetRecursiveRuleMethod(), "Same method returns (Method)");
                yield return (GetRecursiveRuleLambda(0), GetRecursiveRuleLambda(0), "Same method returns (Lambda with same parameters)");
                yield return (GetRecursiveRuleLambda(), new RecursiveRule(() => TestRuleProp), "Different method but same lambda");
                yield return (g1.ReferenceSelfByName, g1.ReferenceSelfByName, "Recursive rule referencing self (by name)");
                yield return (g2.ReferenceSelfByProperty, g2.ReferenceSelfByProperty, "Recursive rule referencing self (by property)");
            }
        }

        public static IEnumerable<(Rule, Rule, string)> RecursiveRuleNotEquals
        {
            get
            {
                var g1 = new TestGrammar();
                var g2 = new TestGrammar();
                var n1 = new NamedRule("test1", "test1");
                var n2 = new NamedRule("test2", "test2");
                yield return (new RecursiveRule(() => n1), new RecursiveRule(() => n2), "Different inner rules");
                yield return (GetRecursiveRuleLambda(0), GetRecursiveRuleLambda(1), "Same method with different lambda parameters");
                yield return (g1.ReferenceSelfByName, g2.ReferenceSelfByName, "Same rule from different grammar instance");
            }
        }

        [Test]
        [TestCaseSource(nameof(RecursiveRuleEquals))]
        public static void RecursiveRuleTest_Equals((Rule rr1, Rule rr2, string desc) input)
        {
            Console.WriteLine($"{input.desc}:");

            var hash1 = input.rr1.GetHashCode();
            var hash2 = input.rr2.GetHashCode();

            Console.WriteLine($"  rr1 => 0x{hash1:X8}");
            Console.WriteLine($"  rr2 => 0x{hash2:X8}");

            Assert.AreEqual(hash1, hash2);
            Assert.AreEqual(input.rr1, input.rr2);
        }

        [Test]
        [TestCaseSource(nameof(RecursiveRuleNotEquals))]
        public static void RecursiveRuleTest_NotEquals((Rule rr1, Rule rr2, string desc) input)
        {
            Console.WriteLine($"{input.desc}:");

            var hash1 = input.rr1.GetHashCode();
            var hash2 = input.rr2.GetHashCode();

            Console.WriteLine($"  rr1 => 0x{hash1:X8}");
            Console.WriteLine($"  rr2 => 0x{hash2:X8}");

            Assert.AreNotEqual(hash1, hash2);
            Assert.AreNotEqual(input.rr1, input.rr2);
        }
    }
}