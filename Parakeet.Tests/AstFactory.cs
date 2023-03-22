using Parakeet.Demos.Json;
using Array = Parakeet.Demos.Json.Array;
using Object = Parakeet.Demos.Json.Object;
using String = Parakeet.Demos.Json.String;

namespace Parakeet.Tests;

using Parakeet.Demos.CSharp;

public class AstFactory
{
    public static AstNode Node(CstNode node)
    {
        switch (node)
        {
            case AccessSpecifier accessSpecifier:
                break;
            case ArrayInitializationValue arrayInitializationValue:
                break;
            case ArrayRankSpecifier arrayRankSpecifier:
                break;
            case ArrayRankSpecifiers arrayRankSpecifiers:
                break;
            case ArraySizeSpecifier arraySizeSpecifier:
                break;
            case AsOperation asOperation:
                break;
            case Attribute attribute:
                break;
            case AttributeList attributeList:
                break;
            case BaseCall baseCall:
                break;
            case BaseClassList baseClassList:
                break;
            case BaseOrThisCall baseOrThisCall:
                break;
            case BinaryLiteral binaryLiteral:
                break;
            case BinaryOperation binaryOperation:
                break;
            case BinaryOperator binaryOperator:
                break;
            case BooleanLiteral booleanLiteral:
                break;
            case BracedStructure bracedStructure:
                break;
            case BracketedStructure bracketedStructure:
                break;
            case BreakStatement breakStatement:
                break;
            case CaseClause caseClause:
                break;
            case CastExpression castExpression:
                break;
            case CatchClause catchClause:
                break;
            case CharLiteral charLiteral:
                break;
            case CompoundStatement compoundStatement:
                break;
            case CompoundTypeExpr compoundTypeExpr:
                break;
            case ConditionalMemberAccess conditionalMemberAccess:
                break;
            case Constraint constraint:
                break;
            case ConstraintClause constraintClause:
                break;
            case ConstraintList constraintList:
                break;
            case ConstructorDeclaration constructorDeclaration:
                break;
            case ContinueStatement continueStatement:
                break;
            case ConverterDeclaration converterDeclaration:
                break;
            case DeclarationPreamble declarationPreamble:
                break;
            case Default @default:
                break;
            case DefaultValue defaultValue:
                break;
            case DoWhileStatement doWhileStatement:
                break;
            case Element element:
                break;
            case ElseClause elseClause:
                break;
            case Expression expression:
                break;
            case ExpressionBody expressionBody:
                break;
            case ExpressionStatement expressionStatement:
                break;
            case FieldDeclaration fieldDeclaration:
                break;
            case File file:
                break;
            case FileStructure fileStructure:
                break;
            case FinallyClause finallyClause:
                break;
            case FloatLiteral floatLiteral:
                break;
            case ForEachStatement forEachStatement:
                break;
            case ForStatement forStatement:
                break;
            case FunctionArg functionArg:
                break;
            case FunctionArgKeyword functionArgKeyword:
                break;
            case FunctionArgs functionArgs:
                break;
            case FunctionBody functionBody:
                break;
            case FunctionParameter functionParameter:
                break;
            case FunctionParameterKeywords functionParameterKeywords:
                break;
            case FunctionParameterList functionParameterList:
                break;
            case Getter getter:
                break;
            case HexLiteral hexLiteral:
                break;
            case Identifier identifier:
                break;
            case IfStatement ifStatement:
                break;
            case ImplicitOrExplicit implicitOrExplicit:
                break;
            case Indexer indexer:
                break;
            case IndexerDeclaration indexerDeclaration:
                break;
            case Initialization initialization:
                break;
            case InitializationClause initializationClause:
                break;
            case InitializationValue initializationValue:
                break;
            case Initializer initializer:
                break;
            case InitializerClause initializerClause:
                break;
            case Initter initter:
                break;
            case InnerTypeExpr innerTypeExpr:
                break;
            case IntegerLiteral integerLiteral:
                break;
            case InvariantClause invariantClause:
                break;
            case IsOperation isOperation:
                break;
            case Kind kind:
                break;
            case LambdaBody lambdaBody:
                break;
            case LambdaExpr lambdaExpr:
                break;
            case LambdaParameter lambdaParameter:
                break;
            case LambdaParameters lambdaParameters:
                break;
            case LeafExpression leafExpression:
                break;
            case Literal literal:
                break;
            case MemberAccess memberAccess:
                break;
            case MemberDeclaration memberDeclaration:
                break;
            case MethodDeclaration methodDeclaration:
                break;
            case Modifier modifier:
                break;
            case NameOf nameOf:
                break;
            case NamespaceDeclaration namespaceDeclaration:
                break;
            case NewOperation newOperation:
                break;
            case Nullable nullable:
                break;
            case NullLiteral nullLiteral:
                break;
            case OperatorDeclaration operatorDeclaration:
                break;
            case OverloadableOperator overloadableOperator:
                break;
            case ParenthesizedExpression parenthesizedExpression:
                break;
            case ParenthesizedStructure parenthesizedStructure:
                break;
            case PostfixOperator postfixOperator:
                break;
            case PropertyBody propertyBody:
                break;
            case PropertyClauses propertyClauses:
                break;
            case PropertyDeclaration propertyDeclaration:
                break;
            case QualifiedIdentifier qualifiedIdentifier:
                break;
            case ReturnStatement returnStatement:
                break;
            case Separator separator:
                break;
            case Setter setter:
                break;
            case Statement statement:
                break;
            case StatementKeyword statementKeyword:
                break;
            case StatementStructure statementStructure:
                break;
            case Static @static:
                break;
            case StringInterpolation stringInterpolation:
                break;
            case StringInterpolationContent stringInterpolationContent:
                break;
            case StringLiteral stringLiteral:
                break;
            case Structure structure:
                break;
            case SwitchStatement switchStatement:
                break;
            case TernaryOperation ternaryOperation:
                break;
            case ThisCall thisCall:
                break;
            case ThrowExpression throwExpression:
                break;
            case Token token:
                break;
            case TokenGroup tokenGroup:
                break;
            case TryStatement tryStatement:
                break;
            case TypeArgList typeArgList:
                break;
            case TypeDeclaration typeDeclaration:
                break;
            case TypeDeclarationWithPreamble typeDeclarationWithPreamble:
                break;
            case TypeExpr typeExpr:
                break;
            case TypeKeyword typeKeyword:
                break;
            case TypeOf typeOf:
                break;
            case TypeParameter typeParameter:
                break;
            case TypeParameterList typeParameterList:
                break;
            case TypeStructure typeStructure:
                break;
            case TypeVariance typeVariance:
                break;
            case UnaryOperator unaryOperator:
                break;
            case UsingDirective usingDirective:
                break;
            case ValueLiteral valueLiteral:
                break;
            case VarDecl varDecl:
                break;
            case VarDeclStatement varDeclStatement:
                break;
            case VariantClause variantClause:
                break;
            case WhileStatement whileStatement:
                break;
            case YieldBreak yieldBreak:
                break;
            case YieldReturn yieldReturn:
                break;
            case YieldStatement yieldStatement:
                break;
            case Array array:
                break;
            case Constant constant:
                break;
            case Demos.Json.Element element1:
                break;
            case Json json:
                break;
            case Member member:
                break;
            case Number number:
                break;
            case Object o:
                break;
            case String s:
                break;
            case CstChoice cstChoice:
                break;
            case CstSequence cstSequence:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(node));
        }

        throw new NotImplementedException();
    }
}