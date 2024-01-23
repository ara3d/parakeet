using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parakeet
{
    public static class ParserExtensions
    {
        public static ParserState Parse(this ParserInput input, Rule r)
            => r.Match(new ParserState(input));

        public static IEnumerable<ParserRange> GetMatches(this string input, Rule rule)
            => GetMatches((ParserInput)input, rule);

        public static IEnumerable<ParserRange> GetMatches(this ParserInput input, Rule rule)
        {
            var p = new ParserState(input);
            while (!p.AtEnd())
            {
                // Don't keep nodes between matches 
                p = p.ClearNodes();

                // Test to see if we can generate a result
                var result = rule.Match(p);
                if (result != null)
                {
                    yield return p.To(result);
                }

                if (result?.Position > p.Position)
                {
                    p = result;
                }
                else
                {
                    p = p.Advance();
                }
            }
        }

        public static IEnumerable<ParserNode> ToNodes(this IEnumerable<ParserState> states)
            => states.Where(state => state.Node != null).Select(state => state.Node).SelectMany(n => n.SelfAndSiblings());

        public static IEnumerable<string> ToStrings(this IEnumerable<ParserNode> nodes)
            => nodes.Select(n => n.Contents);

        public static IEnumerable<IGrouping<string, ParserNode>> GroupNodes(this IEnumerable<ParserNode> nodes)
            => nodes.GroupBy(n => n.Name).OrderBy(g => g.Key);

        public static StringBuilder BuildXmlString(this ParserTreeNode treeNode, string indent = "", StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            if (treeNode.Children.Count == 0)
                return sb.AppendLine($"{indent}<{treeNode.Type}>{treeNode.Contents}</{treeNode.Type}>");
            sb.AppendLine($"{indent}<{treeNode.Type}>");
            foreach (var child in treeNode.Children)
            {
                BuildXmlString(child, indent + "  ", sb);
            }
            sb.AppendLine($"{indent}</{treeNode.Type}>");
            return sb;
        }

        public static string ToXml(this ParserTreeNode treeNode)
            => treeNode.BuildXmlString().ToString();

        public static IEnumerable<ParserNode> AllNodes(this ParserState state)
            => state.Node.AllNodesReversed().Reverse();

        public static IEnumerable<ParserError> AllErrorsReversed(this ParserError error)
        {
            while (error != null)
            {
                yield return error;
                error = error.Previous;
            }
        }
        
        public static IEnumerable<ParserError> AllErrors(this ParserState state)
            => state.LastError.AllErrorsReversed().Reverse();
    }
}