using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIRS.Shared.Extensions
{
    public static class NumericExtensions
    {
        public static double MegabytesToBytes(this long byteCount)
        {
            double byteReturn = 0;
            if (byteCount == 0)
            {
                byteReturn = 0;
            }
            else
            {
                byteReturn = byteCount * 1048576;
            }
            return byteReturn;
        }
    }
}