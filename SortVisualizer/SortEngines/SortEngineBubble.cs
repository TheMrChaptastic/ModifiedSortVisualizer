using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortVisualizer
{
    class SortEngineBubble : ISortEngine
    {
        private double[] TheArray;
        private Form1 _form;
        private Graphics g;
        private int MaxVal;
        Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        Brush PinkBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Pink);

        public SortEngineBubble(double[] TheArray_In, Graphics g_In, int MaxVal_In, Form1 form)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;
            _form = form;

            ReDraw();
        }

        public async Task NextStep()
        {
            await Task.Run(async() => {
                for (int i = 0; i < TheArray.Count() - Constants.NumOfRuns - 1; i++)
                {
                    if (Constants.IsCancelling)
                    {
                        break;
                    }
                    DrawSelectedBar(i, TheArray[i]);
                    DrawSelectedBar(i + 1, TheArray[i + 1]);
                    Constants.Comparisons++;
                    await _form.UpdateLabel();
                    if (TheArray[i] > TheArray[i + 1])
                    {
                        Swap(i, i + 1);
                        Constants.Swaps++;
                        await _form.UpdateLabel();
                    }
                    else
                    {
                        DrawBar(i, TheArray[i]);
                        DrawBar(i + 1, TheArray[i + 1]);
                    }
                }
            });
        }

        private void Swap(int i, int p)
        {
            double temp = TheArray[i];
            TheArray[i] = TheArray[i + 1];
            TheArray[i + 1] = temp;

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
    }
}
