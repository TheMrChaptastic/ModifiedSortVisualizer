using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortVisualizer 
{
    class SortEngineQuick : ISortEngine
    {
        private int originalLeftIndex;
        private int originalRightIndex;
        private double[] TheArray;
        private Form1 _form;
        private Graphics g;
        private int MaxVal;
        Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        Brush PinkBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Pink);

        public SortEngineQuick(double[] TheArray_In, Graphics g_In, int MaxVal_In, Form1 form)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;
            _form = form;

            originalLeftIndex = 0;
            originalRightIndex = TheArray.Length - 1;

            ReDraw();
        }

        public async Task NextStep()
        {
            //Using Recursion for this so it doesnt exactly fit the setup to use NextStep well. so using if statements to check if cancelling is occuring to exit early out of recursion.
            await SortArray(TheArray, originalLeftIndex, originalRightIndex); 
        }

        public async Task SortArray(double[] array, int leftIndex, int rightIndex)
        {
            var i = leftIndex;
            var p = rightIndex;
            var pivot = array[leftIndex];
            while (i <= p)
            {
                DrawSelectedBar(i, array[i]);
                DrawSelectedBar(p, array[p]);
                while (array[i] < pivot)
                {
                    if (Constants.IsCancelling)
                    {
                        return;
                    }
                    Constants.Comparisons++;
                    await _form.UpdateLabel();
                    DrawBar(i, array[i]);
                    i++;
                    DrawSelectedBar(i, array[i]);
                }

                while (array[p] > pivot)
                {
                    if (Constants.IsCancelling)
                    {
                        return;
                    }
                    Constants.Comparisons++;
                    await _form.UpdateLabel();
                    DrawBar(p, array[p]);
                    p--;
                    DrawSelectedBar(p, array[p]);
                }
                if (i <= p)
                {
                    DrawSelectedBar(i, array[i]);
                    DrawSelectedBar(p, array[p]);
                    await Task.Delay(Constants.Delay);
                    Swap(i, p);
                    Constants.Swaps++;
                    await _form.UpdateLabel();
                    i++;
                    p--;
                }
            }

            if (leftIndex < p)
            {
                DrawSelectedBar(p, array[p]);
                if (i < TheArray.Length)
                {
                    DrawBar(i, array[i]);
                }
                await SortArray(array, leftIndex, p);
            }
            if (i < rightIndex)
            {
                DrawSelectedBar(i, array[i]);
                if (p >= 0)
                {
                    DrawBar(p, array[p]);
                }
                await SortArray(array, i, rightIndex);
            }
        }

        private void Swap(int i, int p)
        {
            double temp = TheArray[i];
            TheArray[i] = TheArray[p];
            TheArray[p] = temp;

            DrawBar(i, TheArray[i]);
            DrawBar(p, TheArray[p]);
        }

        private void DrawBar(int position, double height)
        {
            g.FillRectangle(BlackBrush, (float)(position * ((double)Constants.MaxWidth / Constants.NumEntries)), 0, (float)(Math.Ceiling((double)Constants.MaxWidth / Constants.NumEntries)), MaxVal);
            g.FillRectangle(WhiteBrush, (float)(position * ((double)Constants.MaxWidth / Constants.NumEntries)), (float)(MaxVal - TheArray[position]), (float)(Math.Ceiling((double)Constants.MaxWidth / Constants.NumEntries) * Constants.Seperation), (float)height);
        }

        private void DrawSelectedBar(int position, double height)
        {
            g.FillRectangle(PinkBrush, (float)(position * ((double)Constants.MaxWidth / Constants.NumEntries)), (float)(MaxVal - TheArray[position]), (float)(Math.Ceiling((double)Constants.MaxWidth / Constants.NumEntries) * Constants.Seperation), (float)height);
        }

        public async Task<bool> IsSorted()
        {
            ReDraw();
            return await Task.Run(() => {
                for (int i = 0; i < TheArray.Count() - 1; i++)
                {
                    if (TheArray[i] > TheArray[i + 1]) return false;
                }
                return true;
            });
            
        }

        public void ReDraw()
        {
            for (int i = 0; i < (TheArray.Count() - 1); i++)
            {
                g.FillRectangle(WhiteBrush, (float)(i * ((double)Constants.MaxWidth / Constants.NumEntries)), (float)(MaxVal - TheArray[i]), (float)(Math.Ceiling((double)Constants.MaxWidth / Constants.NumEntries) * Constants.Seperation), MaxVal);
            }
        }

        public delegate void LabelUpdateCallBackPointer();
    }
}
