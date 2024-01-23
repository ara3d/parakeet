using System.Linq;

namespace Parakeet
{
    public static class RuleExtensions
    {
        public static Rule ToCharSetRule(this string s)
            => new CharSetRule(s.ToCharArray());

        public static Rule At(this Rule rule)
            => new AtRule(rule);

        public static Rule Then(this Rule rule, Rule other)
            => new SequenceRule(new[] { rule, other });

        public static Rule ThenNot(this Rule rule, Rule other)
            => rule.Then(other.NotAt());

        public static Rule Optional(this Rule rule)
            => new OptionalRule(rule);

        public static Rule Or(this Rule rule, Rule other)
            => new ChoiceRule(new[] { rule, other });

        public static Rule NotAt(this Rule rule)
            => new NotAtRule(rule);

        public static Rule Except(this Rule rule, Rule except)
            => (except.NotAt() + rule);

        public static Rule ZeroOrMore(this Rule rule)
            => new ZeroOrMoreRule(rule);

        public static Rule OneOrMore(this Rule rule)
            => new OneOrMoreRule(rule);

        public static Rule To(this char c1, char c2)
            => new CharSetRule(Enumerable.Range(c1, c2 - c1 + 1).Select(i => (char)i).ToArray());

        public static string GetName(this Rule rule)
            => rule is NamedRule nr ? nr.Name : "__";

        public static Rule Optimize(this Rule rule)
            => new RuleOptimizer().Optimize(rule);

        public static ParserState Parse(this Rule rule, string input)
            => new ParserInput(input).Parse(rule);
    }
}