

namespace CSLox.Src.Lox
{
    internal class LoxClass(string lexeme, LoxClass? superclass, Dictionary<string, LoxFunction> methods) : ILoxCallable
    {
        internal readonly string name = lexeme;
        internal readonly LoxClass? superclass = superclass;
        internal readonly Dictionary<string, LoxFunction> methods = methods;

        public int Arity
        {
            get
            {
                LoxFunction? initializer = FindMethod("init");
                return initializer?.Arity ?? 0;
            }
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            LoxInstance instance = new(this);
            LoxFunction? initializer = FindMethod("init");
            initializer?.Bind(instance)?.Call(interpreter, arguments);

            return instance;
        }

        public override string ToString() => name;

        internal LoxFunction? FindMethod(string name) => superclass?.FindMethod(name) ?? methods.GetValueOrDefault(name);
    }
}