
namespace CSLox.Src.Lox
{
    internal class LoxInstance
    {
        private LoxClass klass;
        private readonly Dictionary<string, object?> fields = [];

        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        internal object? Get(Token name)
        {
            if (fields.TryGetValue(name.lexeme, out var value))
                return value;

            LoxFunction? method = klass.FindMethod(name.lexeme);
            if (method != null) return method.Bind(this);

            throw new RuntimeError(name, $"Undefined property {name.lexeme}.");
        }

        public override string ToString() => $"{klass.name} instance";

        internal void Set(Token name, object? value)
        {
            fields[name.lexeme] = value;
        }
    }
}