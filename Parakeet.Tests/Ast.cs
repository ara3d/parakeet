    namespace Parakeet.Tests
{
    public class AstNode
    {
    }

    public class AstLambda : AstNode
    {
        public IReadOnlyList<AstVar> Parameters { get; }
        public AstNode Body { get; }

        public AstLambda(AstNode body, params AstVar[] parameters)
            => (Parameters, Body) = (parameters, body);
    }

    public class AstBlock : AstNode
    {
        public IReadOnlyList<AstNode> Statements { get; }

        public AstBlock(params AstNode[] statements)
            => Statements = statements;
    }

    public class AstLoop : AstNode
    {
        public AstNode Condition { get; }
        public AstNode Body { get; }

        public AstLoop(AstNode condition, AstNode body)
            => (Condition, Body) = (condition, body);
    }

    public class AstConditional : AstNode
    {
        public AstNode Condition { get; }
        public AstNode IfTrue { get; }
        public AstNode IfFalse { get; }

        public AstConditional(AstNode condition, AstNode ifTrue, AstNode ifFalse)
            => (Condition, IfTrue, IfFalse) = (condition, ifTrue, ifFalse);
    }

    public class AstVar : AstNode
    {
        public string Name { get; }
        public AstNode Value { get; }

        public AstVar(string name, AstNode value)
            => (Name, Value) = (name, value);
    }

    public class AstAssign : AstNode
    {
        public AstVar Var { get; }
        public AstNode Value { get; }

        public AstAssign(AstVar var, AstNode value)
            => (Var, Value) = (var, value);
    }

    public class AstInvoke : AstNode
    {
        public AstNode Function { get; }
        public IReadOnlyList<AstNode> AstArguments { get; }

        public AstInvoke(AstNode function, params AstNode[] arguments)
            => (Function, AstArguments) = (function, arguments);
    }
}
