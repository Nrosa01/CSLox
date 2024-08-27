using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src
{
    static public class StringExtensions
    {
        /// <summary>
        /// Sweep over text
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> WordList(this string Text)
        {
            int cIndex = 0;
            int nIndex;
            while ((nIndex = Text.IndexOf(' ', cIndex + 1)) != -1)
            {
                int sIndex = (cIndex == 0 ? 0 : cIndex + 1);
                yield return Text.Substring(sIndex, nIndex - sIndex);
                cIndex = nIndex;
            }
            yield return Text.Substring(cIndex + 1);
        }
    }
}
