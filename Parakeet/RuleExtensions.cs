using System.IO;
using System.Linq;

namespace Ara3D.Parakeet
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

        public static Rule Counted(this Rule rule, int min, int max)
            => new CountedRule(rule, min, max);

        public static Rule Counted(this Rule rule, int count)
            => new CountedRule(rule, count, count);

        public static Rule CountOrMore(this Rule rule, int min)
            => new CountedRule(rule, min, int.MaxValue);

        public static Rule To(this char c1, char c2)
            => new CharSetRule(Enumerable.Range(c1, c2 - c1 + 1).Select(i => (char)i).ToArray());

        public static bool HasName(this Rule rule)
            => rule is NamedRule;

        public static string GetName(this Rule rule)
            => rule is NamedRule nr ? nr.Name : "__";

        public static Rule Optimize(this Rule rule, TextWriter logger = null)
            => new RuleOptimizer(logger).Optimize(rule);

        public static Rule RepeatUntilAt(this Rule repeat, Rule delimiter) 
            => repeat.Except(delimiter).ZeroOrMore();
        
        public static Rule RepeatUntilPast(this Rule repeat, Rule delimiter) 
            => repeat.RepeatUntilAt(delimiter).Then(delimiter);

        public static ParserState Parse(this Rule rule, string input, bool debugging = false)
            => rule.Parse(new ParserInput(input, "", debugging));
    }
}