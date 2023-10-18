using System;
using System.Collections.Generic;

namespace Parakeet
{
    /// <summary>
    /// This class is used to generate typed classes for the Concrete syntax tree,
    /// and a factory function to convert from parse tree.  
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
                return z.Rule.ToNodeFieldName();
            if (r is OneOrMoreRule o)
                return o.Rule.ToNodeFieldName();
            if (r is SequenceRule s)
                return "Sequence";
            if (r is ChoiceRule c)
                return "Choices";
            if (r is OptionalRule opt)
                return opt.Rule.ToNodeFieldName();
            if (r is RecursiveRule rec)
                return rec.Rule.ToNodeFieldName();
            return "Node";
        }

        public static CodeBuilder OutputFields(CodeBuilder cb, Rule r, HashSet<string> fields)
        {
            fields = fields ?? new HashSet<string>();

            if (r is NodeRule nr)
            {
                if (fields.Contains(nr.Name))
                    return cb;
                fields.Add(nr.Name);
                return cb.WriteLine($"public CstFilter<Cst{nr.Name}> {nr.Name} => new CstFilter<Cst{nr.Name}> (Children);");
            }

            if (r is SequenceRule seq)
            {
                foreach (var child in seq.Rules)
                    cb = OutputFields(cb, child, fields);
                return cb;
            }

            if (r is ChoiceRule ch)
            {
                foreach (var child in ch.Rules)
                    cb = OutputFields(cb, child, fields);
                return cb;
            }

            if (r is ZeroOrMoreRule z)
                return OutputFields(cb, z.Rule, fields);

            if (r is OneOrMoreRule o)
                return OutputFields(cb, o.Rule, fields);

            if (r is OptionalRule opt)
                return OutputFields(cb, opt.Rule, fields);

            if (r is RecursiveRule rec)
                return OutputFields(cb, rec.Rule, fields);

            throw new NotImplementedException($"Unhandled type {r}");
        }

        public static CodeBuilder OutputNodeClass(CodeBuilder cb, Rule r)
        {
            if (r is SequenceRule seq)
            {
                foreach (var child in seq.Rules)
                    if (child is NodeRule nr2)
                        OutputNodeClass(cb, nr2);
            }

            if (!(r is NodeRule nr))
                return cb;

            var isLeaf = ExpectedNumChildNodes(nr) == 0;

            var body = r.Body();
            //cb = cb.WriteLine($"// Original Rule: {body.ToDefinition()}");

            body = body.Optimize();
            if (body == null)
                throw new Exception("Failed to create node");

            body = body.OnlyNodes();
            //cb = cb.WriteLine($"// Only Nodes: {body?.ToDefinition()}");

            body = body?.Optimize();
            //cb = cb.WriteLine($"// Optimized only nodes: {body?.ToDefinition()}");

            cb = cb.WriteLine($"/// <summary>");
            cb = cb.WriteLine($"/// Nodes = {body?.ToDefinition()}");
            cb = cb.WriteLine($"/// </summary>");

            cb = cb.Write($"public class Cst{nr.Name}");
           
            if (body is SequenceRule)
            {
                cb = cb.WriteLine($" : CstSequence");
            }
            else if (body is ChoiceRule)
            {
                cb = cb.WriteLine($" : CstChoice");
            }
            else if (isLeaf)
            {
                cb = cb.WriteLine($" : CstLeaf");
            }
            else
            {
                cb = cb.WriteLine($" : CstNode");
            }

            cb = cb.WriteLine("{").Indent();

            cb = cb.WriteLine($"public static Rule Rule = CstNodeFactory.Grammar.{nr.Name};");

            if (isLeaf)
            {
                cb = cb.WriteLine($"public Cst{nr.Name}(string text) : base(text) {{ }}");
            }
            else
            {
                cb = cb.WriteLine($"public Cst{nr.Name}(params CstNode[] children) : base(children) {{ }}");
            }

            if (body == null)
            {
                cb = cb.WriteLine("// No children");
            }
            else 
            {
                OutputFields(cb, body, null);
            }
            cb = cb.Dedent().WriteLine("}");            
            return cb.WriteLine();
        }
    
        public static void OutputCstClasses(CodeBuilder cb, IEnumerable<Rule> rules)
        {
            foreach (var r in rules)
            {
                OutputNodeClass(cb, r);
            }
        }

        public static void OutputCstClassFactory(CodeBuilder cb, string namespaceName, IEnumerable<Rule> rules)
        {
            cb.WriteLine("public class CstNodeFactory");
            cb.WriteLine("{").Indent();
            cb.WriteLine($"public static {namespaceName}Grammar Grammar = new {namespaceName}Grammar();");
            cb.WriteLine(
                "public Dictionary<CstNode, ParserTreeNode> Lookup { get;} = new Dictionary<CstNode, ParserTreeNode>();");
            
            cb.WriteLine("public CstNode Create(ParserTreeNode node)");
            cb.WriteLine("{").Indent();
            cb.WriteLine("var r = InternalCreate(node);");
            cb.WriteLine("Lookup.Add(r, node);");
            cb.WriteLine("return r;");
            cb.Dedent().WriteLine("}");

            cb.WriteLine("public CstNode InternalCreate(ParserTreeNode node)");
            cb.WriteLine("{").Indent();
            cb.WriteLine("switch (node.Type)");
            cb.WriteLine("{").Indent();
            foreach (var r in rules)
            {
                var r2 = r;
                if (r2 is SequenceRule seq)
                    r2 = seq[0];

                if (r2 is NodeRule nr)
                {
                    if (ExpectedNumChildNodes(nr) == 0)
                    {
                        cb.WriteLine($"case \"{nr.Name}\": return new Cst{nr.Name}(node.Contents);");
                    }
                    else
                    {
                        cb.WriteLine($"case \"{nr.Name}\": return new Cst{nr.Name}(node.Children.Select(Create).ToArray());");
                    }
                }
            }
            cb.WriteLine($"default: throw new Exception($\"Unrecognized parse node {{node.Type}}\");");
            cb.Dedent().WriteLine("}");
            cb.Dedent().WriteLine("}");
            cb.Dedent().WriteLine("}");
        }

        public static void OutputCstClassesFile(CodeBuilder cb, string namespaceName, IEnumerable<Rule> rules)
        {
            cb.WriteLine($"// DO NOT EDIT: Autogenerated file created on {DateTime.Now}. ");
            cb.WriteLine($"using System;"); 
            cb.WriteLine($"using System.Linq;");
            cb.WriteLine($"using Parakeet;");
            cb.WriteLine();
            cb.WriteLine($"namespace {namespaceName}");
            cb.WriteLine("{").Indent();
            OutputCstClasses(cb, rules);
            cb.Dedent().WriteLine("}");
        }

        public static void OutputCstFactoryFile(CodeBuilder cb, string namespaceName, IEnumerable<Rule> rules)
        {
            cb.WriteLine($"// DO NOT EDIT: Autogenerated file created on {DateTime.Now}. ");
            cb.WriteLine($"using System;");
            cb.WriteLine($"using System.Linq;");
            cb.WriteLine($"using Parakeet;");
            cb.WriteLine($"using System.Collections.Generic;");
            cb.WriteLine();
            cb.WriteLine($"namespace {namespaceName}");
            cb.WriteLine("{").Indent();
            OutputCstClassFactory(cb, namespaceName, rules);
            cb.Dedent().WriteLine("}");
        }
    }
}
