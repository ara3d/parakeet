// DO NOT EDIT: Autogenerated file created on 2024-03-02 8:35:35 AM. 
using System;
using System.Linq;

namespace Ara3D.Parakeet.Cst.PlatoTokenGrammarNameSpace
{
    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstBinaryLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.BinaryLiteral;
        public CstBinaryLiteral(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstBinaryOperator : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.BinaryOperator;
        public CstBinaryOperator(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstBooleanLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.BooleanLiteral;
        public CstBooleanLiteral(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstCharLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.CharLiteral;
        public CstCharLiteral(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstComment : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Comment;
        public CstComment(string text) : base(text) { }
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
    /// Nodes = Identifier
    /// </summary>
    public class CstFieldName : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.FieldName;
        public CstFieldName(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstFloatLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.FloatLiteral;
        public CstFloatLiteral(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Identifier
    /// </summary>
    public class CstFunctionName : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.FunctionName;
        public CstFunctionName(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstHexLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.HexLiteral;
        public CstHexLiteral(string text) : base(text) { }
        // No children
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
    /// Nodes = 
    /// </summary>
    public class CstIntegerLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.IntegerLiteral;
        public CstIntegerLiteral(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = (HexLiteral|BinaryLiteral|FloatLiteral|IntegerLiteral|StringLiteral|CharLiteral|BooleanLiteral|NullLiteral)
    /// </summary>
    public class CstLiteral : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.Literal;
        public CstLiteral(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstHexLiteral> HexLiteral => new CstNodeFilter<CstHexLiteral> (Children);
        public CstNodeFilter<CstBinaryLiteral> BinaryLiteral => new CstNodeFilter<CstBinaryLiteral> (Children);
        public CstNodeFilter<CstFloatLiteral> FloatLiteral => new CstNodeFilter<CstFloatLiteral> (Children);
        public CstNodeFilter<CstIntegerLiteral> IntegerLiteral => new CstNodeFilter<CstIntegerLiteral> (Children);
        public CstNodeFilter<CstStringLiteral> StringLiteral => new CstNodeFilter<CstStringLiteral> (Children);
        public CstNodeFilter<CstCharLiteral> CharLiteral => new CstNodeFilter<CstCharLiteral> (Children);
        public CstNodeFilter<CstBooleanLiteral> BooleanLiteral => new CstNodeFilter<CstBooleanLiteral> (Children);
        public CstNodeFilter<CstNullLiteral> NullLiteral => new CstNodeFilter<CstNullLiteral> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstNullLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.NullLiteral;
        public CstNullLiteral(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstOperator : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Operator;
        public CstOperator(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Identifier
    /// </summary>
    public class CstParameterName : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.ParameterName;
        public CstParameterName(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
    }

    /// <summary>
    /// Nodes = (TypeKeyword|StatementKeyword)
    /// </summary>
    public class CstSeparator : CstNodeChoice
    {
        public static Rule Rule = CstNodeFactory.Grammar.Separator;
        public CstSeparator(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstTypeKeyword> TypeKeyword => new CstNodeFilter<CstTypeKeyword> (Children);
        public CstNodeFilter<CstStatementKeyword> StatementKeyword => new CstNodeFilter<CstStatementKeyword> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstStatementKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.StatementKeyword;
        public CstStatementKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstStringLiteral : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.StringLiteral;
        public CstStringLiteral(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstTypeKeyword : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.TypeKeyword;
        public CstTypeKeyword(string text) : base(text) { }
        // No children
    }

    /// <summary>
    /// Nodes = Identifier
    /// </summary>
    public class CstTypeName : CstNode
    {
        public static Rule Rule = CstNodeFactory.Grammar.TypeName;
        public CstTypeName(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
    }

    /// <summary>
    /// Nodes = ((Comment)*+TypeParameterToken+(Comment)*+(((Comment)*+TypeParameterToken))*+(Comment)*)
    /// </summary>
    public class CstTypeParametersToken : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.TypeParametersToken;
        public CstTypeParametersToken(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstComment> Comment => new CstNodeFilter<CstComment> (Children);
        public CstNodeFilter<CstTypeParameterToken> TypeParameterToken => new CstNodeFilter<CstTypeParameterToken> (Children);
    }

    /// <summary>
    /// Nodes = (Identifier+(Comment)*)
    /// </summary>
    public class CstTypeParameterToken : CstNodeSequence
    {
        public static Rule Rule = CstNodeFactory.Grammar.TypeParameterToken;
        public CstTypeParameterToken(params CstNode[] children) : base(children) { }
        public CstNodeFilter<CstIdentifier> Identifier => new CstNodeFilter<CstIdentifier> (Children);
        public CstNodeFilter<CstComment> Comment => new CstNodeFilter<CstComment> (Children);
    }

    /// <summary>
    /// Nodes = 
    /// </summary>
    public class CstUnknown : CstNodeLeaf
    {
        public static Rule Rule = CstNodeFactory.Grammar.Unknown;
        public CstUnknown(string text) : base(text) { }
        // No children
    }

}
