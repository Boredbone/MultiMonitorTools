using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boredbone.Utility.Extensions
{
    public static class MathExtensions
    {
        /// <summary>
        /// 整数での累乗
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static double Pow(this double value, int exp)
        {
            var result = 1.0;
            if (exp > 0)
            {
                for (int i = 0; i < exp; i++)
                {
                    result *= value;
                }
                return result;
            }
            else if (exp < 0)
            {
                for (int i = 0; i < exp; i++)
                {
                    result /= value;
                }
                return result;
            }
            return 1.0;
        }
    }
}
