using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortVisualizer.Factory
{
    public static class SortEngineFactory
    {
        public static ISortEngine GetSortEngine(string name, double[] TheArray_In, Graphics g_In, int MaxVal_In)
        {
            switch(name)
            {
                case "Quick":
                    return new SortEngineQuick(TheArray_In, g_In, MaxVal_In);
                case "Bubble":
                    return new SortEngineBubble(TheArray_In, g_In, MaxVal_In);
                case "VerySlowMoveToBack":
                    return new SortEngineVerySlowMoveToBack(TheArray_In, g_In, MaxVal_In);
                case "BruteForce":
                default:
                    return new SortEngineBruteForce(TheArray_In, g_In, MaxVal_In);
            }
        }
    }
}
