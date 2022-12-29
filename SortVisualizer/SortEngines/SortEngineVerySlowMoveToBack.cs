using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortVisualizer
{
    class SortEngineVerySlowMoveToBack : ISortEngine
    {
        private double[] TheArray;
        private Form1 _form;
        private Graphics g;
        private int MaxVal;
        Brush WhiteBrush = new SolidBrush(Color.White);
        Brush BlackBrush = new SolidBrush(Color.Black);
        Brush PinkBrush = new SolidBrush(Color.Pink);

        private int CurrentListPointer = 0;

        public SortEngineVerySlowMoveToBack(double[] TheArray_In, Graphics g_In, int MaxVal_In, Form1 form)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;
            _form = form;

            ReDraw();
        }

        public async Task NextStep()
        {
            await Task.Run(async () =>
            {
                if (!Constants.IsCancelling)
                {
                    if (CurrentListPointer >= TheArray.Count() - 1) CurrentListPointer = 0;
                    DrawSelectedBar(CurrentListPointer, TheArray[CurrentListPointer]);
                    Constants.Comparisons++;
                    await _form.UpdateLabel();
                    if (TheArray[CurrentListPointer] > TheArray[CurrentListPointer + 1])
                    {
                        Rotate(CurrentListPointer);
                        Constants.Swaps++;
                        await _form.UpdateLabel();
                    }
                    else
                    {
                        DrawBar(CurrentListPointer, TheArray[CurrentListPointer]);
                    }
                    CurrentListPointer++;
                }
            });
        }

        private void Rotate(int currentListPointer)
        {
            double temp = TheArray[CurrentListPointer];
            int EndPoint = TheArray.Count() - 1;
            for (int i = CurrentListPointer; i < EndPoint; i++)
            {
                TheArray[i] = TheArray[i + 1];
                DrawBar(i, TheArray[i]);
            }
            TheArray[EndPoint] = temp;
            DrawBar(EndPoint, TheArray[EndPoint]);
        }

        private void DrawBar(int position, double height)
        {
            g.FillRectangle(BlackBrush, (float)(position * ((double)Constants.MaxWidth / Constants.NumEntries)), 0, (float)(Math.Ceiling((double)Constants.MaxWidth / Constants.NumEntries) * Constants.Seperation), MaxVal);
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
