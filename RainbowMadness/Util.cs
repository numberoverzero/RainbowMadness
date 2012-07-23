using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness
{
    public static class Util
    {
        public static int WrappedIndex(int size, int index)
        {
            return index % size;
        }
    }
}
