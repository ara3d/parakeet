using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Parakeet
{
    public static class GrammarExtensions
    {
        public static Rule Body(this Rule rule)
        {
            if (rule is NamedRule tr)
                return tr.Rule.Body();
            if (rule is RecursiveRule rr)
                return rr.Rule.Body();
            return rule;
        }

        public static void OutputDefinitions(this IGrammar grammar, bool shortForm)
        {
            foreach (var r in grammar.GetRules())
            {
                var justNodes = r.Body().OnlyNodes();
                if (justNodes != null)
                {
                    Console.WriteLine($"{r.GetName()}:");
                    Console.WriteLine($"  {r.Body().ToDefinition(shortForm)}");

                    Console.WriteLine($"JUST NODES:");
                    Console.WriteLine($"  {justNodes.ToDefinition(shortForm)}");
                }
            }
        }

        public static string ToDefinition(this IEnumerable<Rule> rules, string sep, bool shortForm, string indent)
            => string.Join(sep, rules.Select(r => r.ToDefinition(shortForm, indent + "  ")));

        public static string EscapeChar(this char c) =>
            char.IsLetterOrDigit(c) || char.IsSymbol(c) ? c.ToString() : $"\\x{(int)c:X2}";

        public static string EscapeChars(this string s) =>
            s.ToCharArray().EscapeChars();

        public static string EscapeChars(this char[] cs) =>
            string.Join("", cs.Select(EscapeChar));

        public static string ToDefinition(this Rule r, bool shortForm = true, string indent = "")
        {
            var nextLine = shortForm ? "" : $"\n{indent}";
            switch (r)
            {
                case NodeRule nodeRule:
                    return nodeRule.Name;
                case NamedRule nr:
                    return shortForm
                        ? nr.Name
                        : $"({nr.Name}:{nextLine}{nr.Rule.ToDefinition(false, indent)})";
                case SequenceRule seq:
                    return $"({seq.Rules.ToDefinition(nextLine + "+", shortForm, indent)})";
                case ChoiceRule ch:
                    return $"({ch.Rules.ToDefinition(nextLine + "|", shortForm, indent)})";
                case OptionalRule opt:
                    return $"({opt.Rule.ToDefinition(shortForm, indent)})?";
                case ZeroOrMoreRule z:
                    return $"({z.Rule.ToDefinition(shortForm, indent)})*";
                case OneOrMoreRule o:
                    return $"({o.Rule.ToDefinition(shortForm, indent)})+";
                case CountedRule cr:
                    return $"({cr.Rule.ToDefinition(shortForm, indent)}){{{cr.Min},{cr.Max}}}";
                case RecursiveRule rec:
                    return rec.Rule.ToDefinition(shortForm, indent);
                case StringRule sm:
                    return $"\"{sm.Pattern.EscapeChars()}\"";
                case AnyCharRule _:
                    return $"_ANY_";
                case NotAtRule not:
                    return $"!({not.Rule.ToDefinition(shortForm, indent)})";
                case AtRule at:
                    return $"&({at.Rule.ToDefinition(shortForm, indent)})";
                case CharSetRule set:
                    return $"[{set}]";
                case CharRangeRule range:
                    return $"[{range.From}-{range.To}]";
                case OnFail set:
                    return $"_RECOVER_";
                case EndOfInputRule endOfInputRule:
                    return $"_END_";
                case CharRule ch:
                    return $"'{ch.Char}'";
                case BooleanRule br:
                    return br.Value ? "_TRUE_" : "_FALSE_";
                default:
                    return "_UNKNOWN_";
            }
        }

        public static bool HasNode(this Rule r)
        {
            switch (r)
            {
                case NodeRule nr:
                    return true;
                case SequenceRule seq:
                    return seq.Rules.Any(HasNode);
                case ChoiceRule ch:
                    return ch.Rules.Any(HasNode);
                case OptionalRule opt:
                    return opt.Rule.HasNode();
                case ZeroOrMoreRule z:
                    return z.Rule.HasNode();
                case OneOrMoreRule o:
                    return o.Rule.HasNode();
                case RecursiveRule rec:
                    return rec.Rule.HasNode();
                default:
                    return false;
            }
        }

        public static IEnumerable<Rule> ChildrenWithNodes(this Rule r, bool top = false)
        {
            switch (r)
            {
                case NodeRule nr:
                    return top ? nr.Rule.ChildrenWithNodes() : new[] { r };
                case SequenceRule seq:
                    return seq.Rules.SelectMany(r1 => r1.ChildrenWithNodes());
                case ChoiceRule ch:
                    return ch.Rules.SelectMany(r1 => r1.ChildrenWithNodes());
                case OptionalRule opt:
                    return opt.Rule.ChildrenWithNodes();
                case ZeroOrMoreRule z:
                    return z.Rule.ChildrenWithNodes();
                case OneOrMoreRule o:
                    return o.Rule.ChildrenWithNodes();
                case RecursiveRule rec:
                    return rec.Rule.ChildrenWithNodes();
                case CountedRule cr:
                    return cr.Rule.ChildrenWithNodes();
                default:
                    return Enumerable.Empty<Rule>();
            }
        }

        public static Rule OnlyNodes(this Rule r)
        {
            switch (r)
            {
                case NodeRule nr:
                    return nr;

                case SequenceRule seq:
                {
                    var tmp = seq.Rules.Select(r1 => r1.OnlyNodes()).Where(x => x != null).ToList();
                    if (tmp.Count == 1)
                        return tmp[0];
                    if (tmp.Count > 0)
                        return new SequenceRule(tmp.ToArray());
                    break;
                }
                case ChoiceRule ch:
                {
                    var tmp = ch.Rules.Select(r1 => r1.OnlyNodes()).Where(x => x != null).ToList();

                    if (tmp.Count == 1)
                        return tmp[0];
                    if (tmp.Count > 0)
                        return new ChoiceRule(tmp.ToArray());
                    break;
                }
                case OptionalRule opt:
                {
                    var tmp = opt.Rule.OnlyNodes();
                    if (tmp != null)
                        return new OptionalRule(tmp);
                    break;
                }
                case ZeroOrMoreRule z:
                {
                    var tmp = z.Rule.OnlyNodes();
                    if (tmp != null)
                        return new ZeroOrMoreRule(tmp);
                    break;
                }
                case OneOrMoreRule o:
                {
                    var tmp = o.Rule.OnlyNodes();
                    if (tmp != null)
                        return new OneOrMoreRule(tmp);
                    break;
                }
                case RecursiveRule rec:
                {
                    var tmp = rec.Rule.OnlyNodes();
                    if (tmp != null)
                        return tmp;
                    break;
                }
                case CountedRule cr:
                {
                    var tmp = cr.Rule.OnlyNodes();
                    if (tmp != null)
                        return new CountedRule(tmp, cr.Min, cr.Max);
                    break;
                }
            }

            return null;
        }

        public static ParserState Parse(this IGrammar g, string input)
            => g.StartRule.Parse(input);
    }
}
