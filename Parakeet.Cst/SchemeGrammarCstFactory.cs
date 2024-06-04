// DO NOT EDIT: Autogenerated file created on 2024-06-03 7:58:24 PM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Cst.SchemeGrammarNameSpace
{
    public class CstNodeFactory : INodeFactory
    {
        public static SchemeGrammar StaticGrammar = SchemeGrammar.Instance;
        public IGrammar Grammar { get; } = StaticGrammar;
        public CstNode Create(ParserTreeNode node)
        {
            switch (node.Type)
            {
                case "Abbreviation": return new CstAbbreviation(node, node.Children.Select(Create).ToArray());
                case "AbbrevPrefix": return new CstAbbrevPrefix(node, node.Contents);
                case "Alternate": return new CstAlternate(node, node.Children.Select(Create).ToArray());
                case "AndKeyword": return new CstAndKeyword(node, node.Contents);
                case "Assignment": return new CstAssignment(node, node.Children.Select(Create).ToArray());
                case "BeginKeyword": return new CstBeginKeyword(node, node.Contents);
                case "BindingSpec": return new CstBindingSpec(node, node.Children.Select(Create).ToArray());
                case "Body": return new CstBody(node, node.Children.Select(Create).ToArray());
                case "CaseClause": return new CstCaseClause(node, node.Children.Select(Create).ToArray());
                case "CaseKeyword": return new CstCaseKeyword(node, node.Contents);
                case "Command": return new CstCommand(node, node.Children.Select(Create).ToArray());
                case "CompoundDatum": return new CstCompoundDatum(node, node.Children.Select(Create).ToArray());
                case "CondClause": return new CstCondClause(node, node.Children.Select(Create).ToArray());
                case "Conditional": return new CstConditional(node, node.Children.Select(Create).ToArray());
                case "CondKeyword": return new CstCondKeyword(node, node.Contents);
                case "Consequent": return new CstConsequent(node, node.Children.Select(Create).ToArray());
                case "Datum": return new CstDatum(node, node.Children.Select(Create).ToArray());
                case "DefFormals": return new CstDefFormals(node, node.Children.Select(Create).ToArray());
                case "Definition": return new CstDefinition(node, node.Children.Select(Create).ToArray());
                case "DelayKeyword": return new CstDelayKeyword(node, node.Contents);
                case "Delimiter": return new CstDelimiter(node, node.Contents);
                case "DerivedExpression": return new CstDerivedExpression(node, node.Children.Select(Create).ToArray());
                case "DoKeyword": return new CstDoKeyword(node, node.Contents);
                case "ElseClause": return new CstElseClause(node, node.Children.Select(Create).ToArray());
                case "ElseKeyword": return new CstElseKeyword(node, node.Contents);
                case "Expression": return new CstExpression(node, node.Children.Select(Create).ToArray());
                case "Formals": return new CstFormals(node, node.Children.Select(Create).ToArray());
                case "Identifier": return new CstIdentifier(node, node.Contents);
                case "Init": return new CstInit(node, node.Children.Select(Create).ToArray());
                case "InnerDatum": return new CstInnerDatum(node, node.Children.Select(Create).ToArray());
                case "InnerDefinition": return new CstInnerDefinition(node, node.Children.Select(Create).ToArray());
                case "InnerExpression": return new CstInnerExpression(node, node.Children.Select(Create).ToArray());
                case "IterationSpec": return new CstIterationSpec(node, node.Children.Select(Create).ToArray());
                case "LambdaExpression": return new CstLambdaExpression(node, node.Children.Select(Create).ToArray());
                case "LetKeyword": return new CstLetKeyword(node, node.Contents);
                case "LetRecKeyword": return new CstLetRecKeyword(node, node.Contents);
                case "LetStarKeyword": return new CstLetStarKeyword(node, node.Contents);
                case "List": return new CstList(node, node.Children.Select(Create).ToArray());
                case "Literal": return new CstLiteral(node, node.Children.Select(Create).ToArray());
                case "Operand": return new CstOperand(node, node.Children.Select(Create).ToArray());
                case "Operator": return new CstOperator(node, node.Children.Select(Create).ToArray());
                case "OrKeyword": return new CstOrKeyword(node, node.Contents);
                case "ProcedureCall": return new CstProcedureCall(node, node.Children.Select(Create).ToArray());
                case "Program": return new CstProgram(node, node.Children.Select(Create).ToArray());
                case "Quotation": return new CstQuotation(node, node.Children.Select(Create).ToArray());
                case "Recipient": return new CstRecipient(node, node.Children.Select(Create).ToArray());
                case "SelfEvaluating": return new CstSelfEvaluating(node, node.Contents);
                case "Sequence": return new CstSequence(node, node.Children.Select(Create).ToArray());
                case "SimpleDatum": return new CstSimpleDatum(node, node.Contents);
                case "Step": return new CstStep(node, node.Children.Select(Create).ToArray());
                case "Symbol": return new CstSymbol(node, node.Children.Select(Create).ToArray());
                case "Test": return new CstTest(node, node.Children.Select(Create).ToArray());
                case "Token": return new CstToken(node, node.Contents);
                case "Variable": return new CstVariable(node, node.Children.Select(Create).ToArray());
                case "Vector": return new CstVector(node, node.Children.Select(Create).ToArray());
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
