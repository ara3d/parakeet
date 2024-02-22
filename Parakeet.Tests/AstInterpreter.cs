using System.Reflection;

namespace Ara3D.Parakeet.Tests
{
    public class Operators
    {
        public IValue New(IType type)
        {
            throw new NotImplementedException();
        }

        public IValue GetIndex(IValue instance, IValue index)
        {
            throw new NotImplementedException();
        }

        public IValue SetIndex(IValue instance, IValue index, IValue value)
        {
            throw new NotImplementedException();
        }

        public IValue GetField(IValue instance, string name)
        {
            throw new NotImplementedException();
        }

        public IValue SetField(IValue instance, string name, IValue value)
        {
            throw new NotImplementedException();
        }
    }

    public class Intrinsics
    {
        public bool op_And(bool x, bool y) => x && y;
        public bool op_Or(bool x, bool y) => x || y;
        public bool op_XOr(bool x, bool y) => x ^ y;
        public bool op_Not(bool x) => !x;
        public bool op_Equals(bool x, bool y) => x == y;
        public bool op_NotEquals(bool x, bool y) => x != y;

        public double op_Add(double x, double y) => x + y;
        public double op_Subtract(double x, double y) => x - y;
        public double op_Multipy(double x, double y) => x * y;
        public double op_Divide(double x, double y) => x / y;
        public double op_Modulus(double x, double y) => x % y;
        public double op_Negate(double x) => -x;
        public bool op_Equals(double x, double y) => x == y;
        public bool op_NotEquals(double x, double y) => x != y;
        public bool op_LessThan(double x, double y) => x < y;
        public bool op_GreaterThan(double x, double y) => x > y;
        public bool op_LessThanEqualTo(double x, double y) => x <= y;
        public bool op_GreaterThanEqualTo(double x, double y) => x >= y;

        public int op_Add(int x, int y) => x + y;
        public int op_Subtract(int x, int y) => x - y;
        public int op_Multipy(int x, int y) => x * y;
        public int op_Divide(int x, int y) => x / y;
        public int op_Modulus(int x, int y) => x % y;
        public int op_Negate(int x) => -x;
        public bool op_Equals(int x, int y) => x == y;
        public bool op_NotEquals(int x, int y) => x != y;
        public bool op_LessThan(int x, int y) => x < y;
        public bool op_GreaterThan(int x, int y) => x > y;
        public bool op_LessThanEqualTo(int x, int y) => x <= y;
        public bool op_GreaterThanEqualTo(int x, int y) => x >= y;
    }

    public class Value : IValue
    {
        public object Obj { get; }

        public Value(object obj)
        {
            Obj = obj;
            Type = new TypeValue(obj.GetType());
        }

        public IType Type { get; }
    }

    public class FunctionRegistry
    {
        public Dictionary<string, IFunctionValue> Functions { get; } = new();

        public IFunctionValue GetFunction(string name)
        {
            return Functions[name]; 
        }

        public void Add(MethodInfo mi)
        {
            Functions.Add(mi.Name, new MethodValue(mi));
        }

        public void AddStaticMethods(Type t)
        {
            foreach (var mi in t.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                Add(mi);
            }
        }
    }

/*
    public class FunctionValue : IFunctionValue
    {
        public IType Type { get; }
        public IType ReturnType { get; }
        public IReadOnlyList<IType> ParameterTypes { get; }
    }
*/

    public class TypeRegistry
    {
        public IType GetType(string name)
            => throw new NotImplementedException();
    }

    public interface IValue
    {
        public IType Type { get; }
    }

    public interface IFunctionValue : IValue
    {
        public IType ReturnType { get; }
        public IReadOnlyList<IType> ParameterTypes { get; }
    }

    public class MethodValue : IFunctionValue
    {
        public MethodInfo MethodInfo { get; }
    
        public MethodValue(MethodInfo mi)
        {
            MethodInfo = mi;
            ReturnType = new TypeValue(mi.ReturnType);
            ParameterTypes = mi
                .GetParameters()
                .Select(pi => new TypeValue(pi.ParameterType))
                .ToArray();
        }

        public IType Type { get; }
        public IType ReturnType { get; }
        public IReadOnlyList<IType> ParameterTypes { get; }
    }

    public interface IType
    {
        public string Name { get; }
        public IReadOnlyList<IType> TypeArguments { get; }
    }

    public class TypeValue : IType
    {   
        public string Name { get; }
        public IReadOnlyList<IType> TypeArguments { get; }

        public TypeValue(Type type)
        {
            Name = type.Name;
            TypeArguments = type
                .GenericTypeArguments
                .Select(ta => new TypeValue(ta))
                .ToArray();
        }
    }
}
    