using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ara3D.Utils;

namespace Ara3D.Parakeet
{
    /// <summary>
    /// This class is used to generate typed classes for the concrete syntax tree,
    /// and a factory function to convert from untyped parse tree to typed CST.
    /// This is useful for complex grammars like those used in programming languages. 
    /// </summary>
    public static class CstCodeBuilder
    {
        public static string TypedNodeName(this Rule r)
        {
            if (r is NodeRule nr)
                return "Cst" + nr.Name;
            return "CstNode";
        }

        public static int ExpectedNumChildNodes(Rule r)
        {
            var body = r.Body()?.OnlyNodes();
            if (body == null)
                return 0;
            if (body is SequenceRule sequence)
                return sequence.Count;
            if (body is ChoiceRule choice)
                return choice.Count;
            return 1;
        }
        
        public static string ToNodeFieldName(this Rule r)
        {
            if (r is NodeRule nr)
                return nr.Name;
            if (r is ZeroOrMoreRule z)
                return ToNodeFieldName(z.Rule);
            if (r is OneOrMoreRule o)
                return ToNodeFieldName(o.Rule);
            if (r is SequenceRule s)
                return "Sequence";
            if (r is ChoiceRule c)
                return "Choices";
            if (r is OptionalRule opt)
                return ToNodeFieldName(opt.Rule);
            if (r is RecursiveRule rec)
                return ToNodeFieldName(rec.Rule);
            return "Node";
        }

        public static CodeBuilder OutputFields(CodeBuilder cb, HashSet<string> fields)
        {
            foreach (var f in fields)
                cb = cb.WriteLine($"public CstNodeFilter<Cst{f}> {f} => new CstNodeFilter<Cst{f}> (Children);");
            return cb;
        }

        public static HashSet<string> GatherFields(Rule r, HashSet<string> fields = null)
        {
            fields = fields ?? new HashSet<string>();

            if (r == null)
                return fields;

            if (r is NodeRule nr)
            {
                if (!fields.Contains(nr.Name))
                    fields.Add(nr.Name);
                return fields;
            }
            if (r is SequenceRule seq)
            {
                foreach (var child in seq.Rules)
                    GatherFields(child, fields);
                return fields;
            }
            if (r is ChoiceRule ch)
            {
                foreach (var child in ch.Rules)
                    GatherFields(child, fields);
                return fields;
            }
            if (r is ZeroOrMoreRule z)
                return GatherFields(z.Rule, fields);

            if (r is OneOrMoreRule o)
                return GatherFields(o.Rule, fields);

            if (r is OptionalRule opt)
                return GatherFields(opt.Rule, fields);

            if (r is RecursiveRule rec)
                return GatherFields(rec.Rule, fields);

            if (r is CountedRule cr)
                return GatherFields(cr.Rule, fields);

            throw new NotImplementedException($"Unhandled type {r}");
        }

        public static CodeBuilder OutputNodeClass(CodeBuilder cb, Grammar g, Rule r, HashSet<Rule> generated = default)
        {
            if (generated is null)
                generated = new HashSet<Rule>();
            if (generated.Contains(r))
                return cb;
            generated.Add(r);

            if (r is SequenceRule seq)
            {
                foreach (var child in seq.Rules)
                    if (child is NodeRule nr2)
                        OutputNodeClass(cb, g, nr2, generated);
            }

            if (!(r is NodeRule nr))
                return cb;

            var body = r.Body();
            body = body.Optimize();
            if (body == null)
                throw new Exception("Failed to create node");

            body = body.OnlyNodes()?.Optimize();
            var def = body?.ToDefinition()?.Replace("\n", "\\n")?.Replace("\r", "\\r");

            cb = cb.WriteLine($"/// <summary>");
            cb = cb.WriteLine($"/// Rule = {r.ToString().Replace("\n", "\\n").Replace("\r", "\\r")}");
            cb = cb.WriteLine($"/// Nodes = {def}");
            cb = cb.WriteLine($"/// </summary>");

            var fields = GatherFields(body);
            var isLeaf = IsLeaf(r);

            cb = cb.Write($"public class Cst{nr.Name}");

            var iface = GetGrammarCstInterfaceName(g);

            if (body is SequenceRule)
            {
                cb = cb.WriteLine($" : CstNodeSequence, {iface}");
            }
            else if (body is ChoiceRule)
            {
                cb = cb.WriteLine($" : CstNodeChoice, {iface}");
            }
            else if (isLeaf)
            {
                cb = cb.WriteLine($" : CstNodeLeaf, {iface}");
            }
            else
            {
                cb = cb.WriteLine($" : CstNode, {iface}");
            }

            cb = cb.WriteLine("{").Indent();

            cb = cb.WriteLine($"public static Rule Rule = CstNodeFactory.StaticGrammar.{nr.Name};");

            if (isLeaf)
            {
                cb = cb.WriteLine($"public Cst{nr.Name}(ILocation location, string text) : base(location, text) {{ }}");
            }
            else
            {
                cb = cb.WriteLine($"public Cst{nr.Name}(ILocation location, params CstNode[] children) : base(location, children) {{ }}");
            }

            OutputFields(cb, fields);

            cb = cb.Dedent().WriteLine("}");            
            return cb.WriteLine();
        }
    
        public static void OutputCstClasses(CodeBuilder cb, Grammar g)
        {
            var rules = g.GetRules();
            var generated = new HashSet<Rule>();
            foreach (var r in rules)
            {
                OutputNodeClass(cb, g, r, generated);
            }
        }

        public static bool IsLeaf(Rule r)
        {
            var body = r.Body()?.Optimize()?.OnlyNodes()?.Optimize();
            return body == null || GatherFields(body).Count == 0;
        }

        public static void OutputCstClassFactory(CodeBuilder cb, Grammar g)
        {
            cb.WriteLine("public class CstNodeFactory : INodeFactory");
            cb.WriteLine("{").Indent();
            cb.WriteLine($"public static {g.GetType().Name} StaticGrammar = {g.GetType().Name}.Instance;");
            cb.WriteLine($"public IGrammar Grammar {{ get; }} = StaticGrammar;");
            cb.WriteLine($"public CstNode Create(ParserTreeNode node)");
            cb.WriteLine("{").Indent();
            cb.WriteLine("switch (node.Type)");
            cb.WriteLine("{").Indent();

            foreach (var r in g.GetRules())
            {
                var r2 = r;
                if (r2 is SequenceRule seq)
                    r2 = seq[0];

                if (r2 is NodeRule nr)
                {
                    if (IsLeaf(nr))
                    {
                        cb.WriteLine($"case \"{nr.Name}\": return new Cst{nr.Name}(node, node.Contents);");
                    }
                    else
                    {
                        cb.WriteLine($"case \"{nr.Name}\": return new Cst{nr.Name}(node, node.Children.Select(Create).ToArray());");
                    }
                }
            }
            cb.WriteLine($"default: throw new Exception($\"Unrecognized parse node {{node.Type}}\");");
            cb.Dedent().WriteLine("}");
            cb.Dedent().WriteLine("}");
            cb.Dedent().WriteLine("}");
        }

        public static string GetGrammarCstInterfaceName(Grammar g)
        {
            var typeName = g.GetType().Name.RemoveSuffix("Grammar");
            return $"I{typeName}CstNode";
        }

        public static void OutputCstClassesFile(CodeBuilder cb, Grammar g, string namespaceName)
        {
            cb.WriteLine($"// DO NOT EDIT: Autogenerated file created on {DateTime.Now}. ");
            cb.WriteLine($"using System;"); 
            cb.WriteLine($"using System.Linq;");;
            cb.WriteLine();
            cb.WriteLine($"namespace {namespaceName}");
            cb.WriteLine("{").Indent();

            var iface = GetGrammarCstInterfaceName(g);
            cb.WriteLine("/// <summary>This interface exists to make it easy to auto-generate type switches</summary>");
            cb.WriteLine($"public interface {iface} {{ }}");
            cb.WriteLine();

            OutputCstClasses(cb, g);
            cb.Dedent().WriteLine("}");
        }

        /// <summary>
        /// Generates code for the converting from a parse tree to a CST.
        /// </summary>
        public static void OutputCstFactoryFile(CodeBuilder cb, Grammar g, string namespaceName)
        {
            cb.WriteLine($"// DO NOT EDIT: Autogenerated file created on {DateTime.Now}. ");
            cb.WriteLine($"using System;");
            cb.WriteLine($"using System.Linq;");
            cb.WriteLine($"using System.Collections.Generic;");
            cb.WriteLine($"using Ara3D.Parakeet.Grammars;");
            cb.WriteLine();
            cb.WriteLine($"namespace {namespaceName}");
            cb.WriteLine("{").Indent();
            OutputCstClassFactory(cb, g);
            cb.Dedent().WriteLine("}");
        }
    }
}
