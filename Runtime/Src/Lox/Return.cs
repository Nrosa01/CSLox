using System.Runtime.Serialization;

namespace CSLox.Src.Lox
{
    [Serializable]
    internal class Return(object value) : Exception(null, null)
    {
        public object Value { get; } = value;
    }
}