using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{

    [Serializable]
    internal class RuntimeError : Exception
    {
        public readonly Token token;

        public RuntimeError(Token token, string message): base(message)
        {
            this.token = token;
        }
    }
}
