using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boredbone.Utility
{
    public enum ThreeState
    {
        None,
        True,
        False,
    }

    public static class ThreeStateExtensions
    {
        public static bool IsSet(this ThreeState obj)
        {
            return obj != ThreeState.None;
        }
        public static bool IsTrue(this ThreeState obj)
        {
            return obj == ThreeState.True;
        }
        public static bool IsFalse(this ThreeState obj)
        {
            return obj == ThreeState.False;
        }
        public static bool ToBool(this ThreeState obj)
        {
            return obj == ThreeState.True;
        }
    }
}
