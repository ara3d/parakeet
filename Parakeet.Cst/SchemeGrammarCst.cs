// DO NOT EDIT: Autogenerated file created on 2024-03-02 8:35:35 AM. 
using System;
using System.Linq;

namespace Ara3D.Parakeet.Cst.SchemeGrammarNameSpace
{
    /// <summary>
    /// Nodes = (AbbrevPrefix+Datum)
    /// </summary>
    public class CstAbbreviation : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Abbreviation;
        public CstAbbreviation(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstAbbrevPrefix> AbbrevPrefix => new CstNodeFilter<CstAbbrevPrefix> (Children);
        public CstNodeFilter<CstDatum> Datum => new CstNodeFilter<CstDatum> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstAbbrevPrefix : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.AbbrevPrefix;
        public CstAbbrevPrefix(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (Expression)?
    /// </summary>
    public class CstAlternate : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Alternate;
        public CstAlternate(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstAndKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.AndKeyword;
        public CstAndKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (Variable+Expression)
    /// </summary>
    public class CstAssignment : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Assignment;
        public CstAssignment(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstBeginKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.BeginKeyword;
        public CstBeginKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (Variable+Expression)
    /// </summary>
    public class CstBindingSpec : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.BindingSpec;
        public CstBindingSpec(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = ((Definition)*+Sequence)
    /// </summary>
    public class CstBody : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Body;
        public CstBody(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDefinition> Definition => new CstNodeFilter<CstDefinition> (Children);
        public CstNodeFilter<CstSequence> Sequence => new CstNodeFilter<CstSequence> (Children);
    }

    /// <summary>
    /// Nodes = ((Datum)*+Sequence)
    /// </summary>
    public class CstCaseClause : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.CaseClause;
        public CstCaseClause(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDatum> Datum => new CstNodeFilter<CstDatum> (Children);
        public CstNodeFilter<CstSequence> Sequence => new CstNodeFilter<CstSequence> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstCaseKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.CaseKeyword;
        public CstCaseKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstCommand : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Command;
        public CstCommand(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = (List|Vector)
    /// </summary>
    public class CstCompoundDatum : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.CompoundDatum;
        public CstCompoundDatum(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstList> List => new CstNodeFilter<CstList> (Children);
        public CstNodeFilter<CstVector> Vector => new CstNodeFilter<CstVector> (Children);
    }

    /// <summary>
    /// Nodes = (Test+(Sequence|Recipient))
    /// </summary>
    public class CstCondClause : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.CondClause;
        public CstCondClause(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstTest> Test => new CstNodeFilter<CstTest> (Children);
        public CstNodeFilter<CstSequence> Sequence => new CstNodeFilter<CstSequence> (Children);
        public CstNodeFilter<CstRecipient> Recipient => new CstNodeFilter<CstRecipient> (Children);
    }

    /// <summary>
    /// Nodes = (Test+Consequent+Alternate)
    /// </summary>
    public class CstConditional : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Conditional;
        public CstConditional(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstTest> Test => new CstNodeFilter<CstTest> (Children);
        public CstNodeFilter<CstConsequent> Consequent => new CstNodeFilter<CstConsequent> (Children);
        public CstNodeFilter<CstAlternate> Alternate => new CstNodeFilter<CstAlternate> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstCondKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.CondKeyword;
        public CstCondKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstConsequent : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Consequent;
        public CstConsequent(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = InnerDatum
    /// </summary>
    public class CstDatum : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Datum;
        public CstDatum(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerDatum> InnerDatum => new CstNodeFilter<CstInnerDatum> (Children);
    }

    /// <summary>
    /// Nodes = (Variable)*
    /// </summary>
    public class CstDefFormals : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.DefFormals;
        public CstDefFormals(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
    }

    /// <summary>
    /// Nodes = InnerDefinition
    /// </summary>
    public class CstDefinition : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Definition;
        public CstDefinition(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerDefinition> InnerDefinition => new CstNodeFilter<CstInnerDefinition> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstDelayKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.DelayKeyword;
        public CstDelayKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstDelimiter : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Delimiter;
        public CstDelimiter(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = ((CondKeyword+((CondClause)+|((CondClause)*+ElseClause)))|(CaseKeyword+((Expression+(CaseClause)+)|((CaseClause)*+ElseClause)))|(AndKeyword+(Test)*)|(OrKeyword+(Test)*)|(LetKeyword+(((BindingSpec)*+Body)|(Variable+(BindingSpec)*+Body)))|(LetStarKeyword+(BindingSpec)*+Body)|(LetRecKeyword+(BindingSpec)*+Body)|(BeginKeyword+Sequence)|(DoKeyword+(IterationSpec)*+Test+Sequence+(Command)*)|(DelayKeyword+Expression))
    /// </summary>
    public class CstDerivedExpression : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.DerivedExpression;
        public CstDerivedExpression(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCondKeyword> CondKeyword => new CstNodeFilter<CstCondKeyword> (Children);
        public CstNodeFilter<CstCondClause> CondClause => new CstNodeFilter<CstCondClause> (Children);
        public CstNodeFilter<CstElseClause> ElseClause => new CstNodeFilter<CstElseClause> (Children);
        public CstNodeFilter<CstCaseKeyword> CaseKeyword => new CstNodeFilter<CstCaseKeyword> (Children);
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
        public CstNodeFilter<CstCaseClause> CaseClause => new CstNodeFilter<CstCaseClause> (Children);
        public CstNodeFilter<CstAndKeyword> AndKeyword => new CstNodeFilter<CstAndKeyword> (Children);
        public CstNodeFilter<CstTest> Test => new CstNodeFilter<CstTest> (Children);
        public CstNodeFilter<CstOrKeyword> OrKeyword => new CstNodeFilter<CstOrKeyword> (Children);
        public CstNodeFilter<CstLetKeyword> LetKeyword => new CstNodeFilter<CstLetKeyword> (Children);
        public CstNodeFilter<CstBindingSpec> BindingSpec => new CstNodeFilter<CstBindingSpec> (Children);
        public CstNodeFilter<CstBody> Body => new CstNodeFilter<CstBody> (Children);
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
        public CstNodeFilter<CstLetStarKeyword> LetStarKeyword => new CstNodeFilter<CstLetStarKeyword> (Children);
        public CstNodeFilter<CstLetRecKeyword> LetRecKeyword => new CstNodeFilter<CstLetRecKeyword> (Children);
        public CstNodeFilter<CstBeginKeyword> BeginKeyword => new CstNodeFilter<CstBeginKeyword> (Children);
        public CstNodeFilter<CstSequence> Sequence => new CstNodeFilter<CstSequence> (Children);
        public CstNodeFilter<CstDoKeyword> DoKeyword => new CstNodeFilter<CstDoKeyword> (Children);
        public CstNodeFilter<CstIterationSpec> IterationSpec => new CstNodeFilter<CstIterationSpec> (Children);
        public CstNodeFilter<CstCommand> Command => new CstNodeFilter<CstCommand> (Children);
        public CstNodeFilter<CstDelayKeyword> DelayKeyword => new CstNodeFilter<CstDelayKeyword> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstDoKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.DoKeyword;
        public CstDoKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (ElseKeyword+Sequence)
    /// </summary>
    public class CstElseClause : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.ElseClause;
        public CstElseClause(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstElseKeyword> ElseKeyword => new CstNodeFilter<CstElseKeyword> (Children);
        public CstNodeFilter<CstSequence> Sequence => new CstNodeFilter<CstSequence> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstElseKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.ElseKeyword;
        public CstElseKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = InnerExpression
    /// </summary>
    public class CstExpression : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Expression;
        public CstExpression(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstInnerExpression> InnerExpression => new CstNodeFilter<CstInnerExpression> (Children);
    }

    /// <summary>
    /// Nodes = (Variable)*
    /// </summary>
    public class CstFormals : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Formals;
        public CstFormals(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstIdentifier : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Identifier;
        public CstIdentifier(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstInit : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Init;
        public CstInit(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = (SimpleDatum|CompoundDatum)
    /// </summary>
    public class CstInnerDatum : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.InnerDatum;
        public CstInnerDatum(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstSimpleDatum> SimpleDatum => new CstNodeFilter<CstSimpleDatum> (Children);
        public CstNodeFilter<CstCompoundDatum> CompoundDatum => new CstNodeFilter<CstCompoundDatum> (Children);
    }

    /// <summary>
    /// Nodes = ((Variable+(Expression|(DefFormals+Body)))|(Definition)*)
    /// </summary>
    public class CstInnerDefinition : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.InnerDefinition;
        public CstInnerDefinition(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
        public CstNodeFilter<CstDefFormals> DefFormals => new CstNodeFilter<CstDefFormals> (Children);
        public CstNodeFilter<CstBody> Body => new CstNodeFilter<CstBody> (Children);
        public CstNodeFilter<CstDefinition> Definition => new CstNodeFilter<CstDefinition> (Children);
    }

    /// <summary>
    /// Nodes = (Variable|Literal|ProcedureCall|LambdaExpression|Conditional|Assignment|DerivedExpression)
    /// </summary>
    public class CstInnerExpression : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.InnerExpression;
        public CstInnerExpression(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
        public CstNodeFilter<CstLiteral> Literal => new CstNodeFilter<CstLiteral> (Children);
        public CstNodeFilter<CstProcedureCall> ProcedureCall => new CstNodeFilter<CstProcedureCall> (Children);
        public CstNodeFilter<CstLambdaExpression> LambdaExpression => new CstNodeFilter<CstLambdaExpression> (Children);
        public CstNodeFilter<CstConditional> Conditional => new CstNodeFilter<CstConditional> (Children);
        public CstNodeFilter<CstAssignment> Assignment => new CstNodeFilter<CstAssignment> (Children);
        public CstNodeFilter<CstDerivedExpression> DerivedExpression => new CstNodeFilter<CstDerivedExpression> (Children);
    }

    /// <summary>
    /// Nodes = (Variable+Init+Step)
    /// </summary>
    public class CstIterationSpec : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.IterationSpec;
        public CstIterationSpec(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstVariable> Variable => new CstNodeFilter<CstVariable> (Children);
        public CstNodeFilter<CstInit> Init => new CstNodeFilter<CstInit> (Children);
        public CstNodeFilter<CstStep> Step => new CstNodeFilter<CstStep> (Children);
    }

    /// <summary>
    /// Nodes = (Formals+Body)
    /// </summary>
    public class CstLambdaExpression : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.LambdaExpression;
        public CstLambdaExpression(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstFormals> Formals => new CstNodeFilter<CstFormals> (Children);
        public CstNodeFilter<CstBody> Body => new CstNodeFilter<CstBody> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstLetKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.LetKeyword;
        public CstLetKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstLetRecKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.LetRecKeyword;
        public CstLetRecKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstLetStarKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.LetStarKeyword;
        public CstLetStarKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (Datum)*
    /// </summary>
    public class CstList : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.List;
        public CstList(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDatum> Datum => new CstNodeFilter<CstDatum> (Children);
    }

    /// <summary>
    /// Nodes = (Quotation|SelfEvaluating)
    /// </summary>
    public class CstLiteral : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.Literal;
        public CstLiteral(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstQuotation> Quotation => new CstNodeFilter<CstQuotation> (Children);
        public CstNodeFilter<CstSelfEvaluating> SelfEvaluating => new CstNodeFilter<CstSelfEvaluating> (Children);
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstOperand : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Operand;
        public CstOperand(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstOperator : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Operator;
        public CstOperator(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstOrKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.OrKeyword;
        public CstOrKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (Operator+(Operand)*)
    /// </summary>
    public class CstProcedureCall : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.ProcedureCall;
        public CstProcedureCall(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstOperator> Operator => new CstNodeFilter<CstOperator> (Children);
        public CstNodeFilter<CstOperand> Operand => new CstNodeFilter<CstOperand> (Children);
    }

    /// <summary>
    /// Nodes = ((Command|Definition))*
    /// </summary>
    public class CstProgram : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Program;
        public CstProgram(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCommand> Command => new CstNodeFilter<CstCommand> (Children);
        public CstNodeFilter<CstDefinition> Definition => new CstNodeFilter<CstDefinition> (Children);
    }

    /// <summary>
    /// Nodes = (Datum|Datum)
    /// </summary>
    public class CstQuotation : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.Quotation;
        public CstQuotation(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDatum> Datum => new CstNodeFilter<CstDatum> (Children);
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstRecipient : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Recipient;
        public CstRecipient(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstSelfEvaluating : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.SelfEvaluating;
        public CstSelfEvaluating(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = ((Command)*+Expression)
    /// </summary>
    public class CstSequence : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.Sequence;
        public CstSequence(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstCommand> Command => new CstNodeFilter<CstCommand> (Children);
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstSimpleDatum : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.SimpleDatum;
        public CstSimpleDatum(params CstNode[] children) : base(children) { }
        // No children
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstStep : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Step;
        public CstStep(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = Identifier
    /// </summary>
    public class CstSymbol : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Symbol;
        public CstSymbol(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
    }

    /// <summary>
    /// Nodes = Expression
    /// </summary>
    public class CstTest : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Test;
        public CstTest(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstExpression> Expression => new CstNodeFilter<CstExpression> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstToken : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Token;
        public CstToken(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Identifier
    /// </summary>
    public class CstVariable : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Variable;
        public CstVariable(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
    }

    /// <summary>
    /// Nodes = (Datum)*
    /// </summary>
    public class CstVector : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.Vector;
        public CstVector(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstDatum> Datum => new CstNodeFilter<CstDatum> (Children);
    }

}
