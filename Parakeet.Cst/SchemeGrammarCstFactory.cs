// DO NOT EDIT: Autogenerated file created on 2024-03-02 9:43:37 PM. 
using System;
using System.Linq;
using System.Collections.Generic;
using Ara3D.Parakeet.Grammars;

namespace Ara3D.Parakeet.Cst.SchemeGrammarNameSpace
{
    public class CstNodeFactory
    {
        public static readonly SchemeGrammar Grammar = new SchemeGrammar();
        public Dictionary<CstNode, ParserTreeNode> Lookup { get;} = new Dictionary<CstNode, ParserTreeNode>();
        public CstNode Create(ParserTreeNode node)
        {
            var r = InternalCreate(node);
            Lookup.Add(r, node);
            return r;
        }
        public CstNode InternalCreate(ParserTreeNode node)
        {
            switch (node.Type)
            {
                case "Abbreviation": return new CstAbbreviation(node.Children.Select(Create).ToArray());
                case "AbbrevPrefix": return new CstAbbrevPrefix(node.Contents);
                case "Alternate": return new CstAlternate(node.Children.Select(Create).ToArray());
                case "AndKeyword": return new CstAndKeyword(node.Contents);
                case "Assignment": return new CstAssignment(node.Children.Select(Create).ToArray());
                case "BeginKeyword": return new CstBeginKeyword(node.Contents);
                case "BindingSpec": return new CstBindingSpec(node.Children.Select(Create).ToArray());
                case "Body": return new CstBody(node.Children.Select(Create).ToArray());
                case "CaseClause": return new CstCaseClause(node.Children.Select(Create).ToArray());
                case "CaseKeyword": return new CstCaseKeyword(node.Contents);
                case "Command": return new CstCommand(node.Children.Select(Create).ToArray());
                case "CompoundDatum": return new CstCompoundDatum(node.Children.Select(Create).ToArray());
                case "CondClause": return new CstCondClause(node.Children.Select(Create).ToArray());
                case "Conditional": return new CstConditional(node.Children.Select(Create).ToArray());
                case "CondKeyword": return new CstCondKeyword(node.Contents);
                case "Consequent": return new CstConsequent(node.Children.Select(Create).ToArray());
                case "Datum": return new CstDatum(node.Children.Select(Create).ToArray());
                case "DefFormals": return new CstDefFormals(node.Children.Select(Create).ToArray());
                case "Definition": return new CstDefinition(node.Children.Select(Create).ToArray());
                case "DelayKeyword": return new CstDelayKeyword(node.Contents);
                case "Delimiter": return new CstDelimiter(node.Contents);
                case "DerivedExpression": return new CstDerivedExpression(node.Children.Select(Create).ToArray());
                case "DoKeyword": return new CstDoKeyword(node.Contents);
                case "ElseClause": return new CstElseClause(node.Children.Select(Create).ToArray());
                case "ElseKeyword": return new CstElseKeyword(node.Contents);
                case "Expression": return new CstExpression(node.Children.Select(Create).ToArray());
                case "Formals": return new CstFormals(node.Children.Select(Create).ToArray());
                case "Identifier": return new CstIdentifier(node.Contents);
                case "Init": return new CstInit(node.Children.Select(Create).ToArray());
                case "InnerDatum": return new CstInnerDatum(node.Children.Select(Create).ToArray());
                case "InnerDefinition": return new CstInnerDefinition(node.Children.Select(Create).ToArray());
                case "InnerExpression": return new CstInnerExpression(node.Children.Select(Create).ToArray());
                case "IterationSpec": return new CstIterationSpec(node.Children.Select(Create).ToArray());
                case "LambdaExpression": return new CstLambdaExpression(node.Children.Select(Create).ToArray());
                case "LetKeyword": return new CstLetKeyword(node.Contents);
                case "LetRecKeyword": return new CstLetRecKeyword(node.Contents);
                case "LetStarKeyword": return new CstLetStarKeyword(node.Contents);
                case "List": return new CstList(node.Children.Select(Create).ToArray());
                case "Literal": return new CstLiteral(node.Children.Select(Create).ToArray());
                case "Operand": return new CstOperand(node.Children.Select(Create).ToArray());
                case "Operator": return new CstOperator(node.Children.Select(Create).ToArray());
                case "OrKeyword": return new CstOrKeyword(node.Contents);
                case "ProcedureCall": return new CstProcedureCall(node.Children.Select(Create).ToArray());
                case "Program": return new CstProgram(node.Children.Select(Create).ToArray());
                case "Quotation": return new CstQuotation(node.Children.Select(Create).ToArray());
                case "Recipient": return new CstRecipient(node.Children.Select(Create).ToArray());
                case "SelfEvaluating": return new CstSelfEvaluating(node.Contents);
                case "Sequence": return new CstSequence(node.Children.Select(Create).ToArray());
                case "SimpleDatum": return new CstSimpleDatum(node.Children.Select(Create).ToArray());
                case "Step": return new CstStep(node.Children.Select(Create).ToArray());
                case "Symbol": return new CstSymbol(node.Children.Select(Create).ToArray());
                case "Test": return new CstTest(node.Children.Select(Create).ToArray());
                case "Token": return new CstToken(node.Contents);
                case "Variable": return new CstVariable(node.Children.Select(Create).ToArray());
                case "Vector": return new CstVector(node.Children.Select(Create).ToArray());
                default: throw new Exception($"Unrecognized parse node {node.Type}");
            }
        }
    }
}
