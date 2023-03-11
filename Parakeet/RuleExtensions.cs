namespace Parakeet
{
    public static class RuleExtensions
    {
        public static Rule ToCharSetRule(this string s)
            => new CharSetRule(s.ToCharArray());

        public static Rule At(this Rule rule)
            => new At(rule);

        public static Rule Then(this Rule rule, Rule other)
            => new Sequence(new[] { rule, other });

        public static Rule ThenNot(this Rule rule, Rule other)
            => rule.Then(other.NotAt());

        public static Rule Optional(this Rule rule)
            => new Optional(rule);

        public static Rule Or(this Rule rule, Rule other)
            => new Choice(new[] { rule, other });

        public static Rule NotAt(this Rule rule)
            => new NotAt(rule);

        public static Rule Except(this Rule rule, Rule except)
            => (except.NotAt() + rule);

        public static Rule ZeroOrMore(this Rule rule)
            => new ZeroOrMore(rule);

        public static Rule OneOrMore(this Rule rule)
            => rule.Then(rule.ZeroOrMore());

        public static Rule To(this char c1, char c2)
            => new CharRangeRule(c1, c2);

        public static string GetName(this Rule rule)
            => rule is NamedRule nr ? nr.Name : "_unknown_";
    }
}