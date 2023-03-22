using System;
using System.Collections.Generic;
using System.Linq;

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
                return nr.Name;
            return "CstNode";
        }

        public static CodeBuilder OutputNodeField(CodeBuilder cb, List<string> fieldNames, Rule r, int index, int child)
        {
            var fieldName = fieldNames[index];
            var cnt = 0;
            for (var i=0; i < index; ++i)
            {
                if (fieldNames[i] == fieldName)
                {
                    cnt++;
                }
            }

            // In case a field name is used multiple times. 
            if (cnt > 0)
                fieldName = $"{fieldName}_{cnt}";

            if (r is NodeRule nr)
                return cb.WriteLine($"public {nr.Name} {fieldName} => Children[{child}] as {nr.Name};");
            
            if (r is SequenceRule)
                return cb.WriteLine($"public CstSequence {fieldName} => Children[{child}] as CstSequence;");
            
            if (r is ChoiceRule)
                return cb.WriteLine($"public CstChoice {fieldName} => Children[{child}] as CstChoice;");
            
            if (r is OptionalRule opt)
                return cb.WriteLine($"public CstOptional<{opt.Rule.TypedNodeName()}> {fieldName} => Children[{child}] as CstOptional<{opt.Rule.TypedNodeName()}>;");
            
            if (r is ZeroOrMoreRule z)
                return cb.WriteLine($"public CstZeroOrMore<{z.Rule.TypedNodeName()}> {fieldName} => Children[{child}] as CstZeroOrMore<{z.Rule.TypedNodeName()}>;");

            if (r is RecursiveRule rr)
                return OutputNodeField(cb, fieldNames, rr.Rule, index, child);

            throw new NotImplementedException($"Unrecognized rule type {r}");
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
                return "ZeroOrMoreRule" + z.Rule.ToNodeFieldName();
            if (r is SequenceRule s)
                return "SequenceRule";
            if (r is ChoiceRule c)
                return "ChoiceRule";
            if (r is OptionalRule opt)
                return opt.Rule.ToNodeFieldName();
            if (r is RecursiveRule rec)
                return rec.Rule.ToNodeFieldName();
            return "Node";
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

            var body = r.Body()?.Optimize().OnlyNodes();

            cb = cb.WriteLine($"// Original Rule: {r.Body().ToDefinition()}");
            cb = cb.WriteLine($"// Only Nodes: {body?.ToDefinition()}");
            cb = cb.Write($"public class {nr.Name}");
           
            if (body is SequenceRule)
            {
                cb = cb.WriteLine($" : CstSequence");
            }
            else if (body is ChoiceRule)
            {
                cb = cb.WriteLine($" : CstChoice");
            }
            else 
            {
                cb = cb.WriteLine($" : CstNode");
            }

            cb = cb.WriteLine("{").Indent();

            cb = cb.WriteLine($"public {nr.Name}(params CstNode[] children) : base(children) {{ }}");
            cb = cb.WriteLine($"public override CstNode Transform(Func<CstNode, CstNode> f) => new {nr.Name}(Children.Select(f).ToArray());");
            var index = 0;
            if (body == null)
            {
                cb = cb.WriteLine("// No children");
            }
            else if (body is SequenceRule sequence)
            {
                var names = sequence.Rules.Select(ToNodeFieldName).ToList();
                foreach (var child in sequence.Rules)
                    cb = OutputNodeField(cb, names, child, index, index++);
            }
            else if (body is ChoiceRule choice)
            {
                var names = choice.Rules.Select(ToNodeFieldName).ToList();
                foreach (var child in choice.Rules)
                    cb = OutputNodeField(cb, names, child, index++, 0);
            }            
            else
            {
                cb = OutputNodeField(cb, new List<string>() { body.ToNodeFieldName() }, body, index, index++);
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

        public static void OutputCstClassFactory(CodeBuilder cb, IEnumerable<Rule> rules)
        {
            cb.WriteLine("public static class CstNodeFactory");
            cb.WriteLine("{").Indent(); 
            cb.WriteLine("public static CstNode Create(ParserTree node)");
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
                        cb.WriteLine($"case \"{nr.Name}\": return new {nr.Name}(node.Contents);");
                    }
                    else
                    {
                        cb.WriteLine($"case \"{nr.Name}\": return new {nr.Name}(node.Children.Select(Create).ToArray());");
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
            cb.WriteLine();
            cb.WriteLine($"namespace {namespaceName}");
            cb.WriteLine("{").Indent();
            OutputCstClassFactory(cb, rules);
            cb.Dedent().WriteLine("}");
        }
    }
}
