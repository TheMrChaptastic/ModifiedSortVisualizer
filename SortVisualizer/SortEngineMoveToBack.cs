using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortVisualizer
{
    class SortEngineMoveToBack : ISortEngine
    {
        private int[] TheArray;
        private Graphics g;
        private int MaxVal;
        Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        Brush PinkBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Pink);

        private int CurrentListPointer = 0;

        public SortEngineMoveToBack(int[] TheArray_In, Graphics g_In, int MaxVal_In)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;

            ReDraw();
        }

        public void NextStep()
        {
            if (!Form1.IsCancelling)
            {
                if (CurrentListPointer >= TheArray.Count() - 1) CurrentListPointer = 0;
                DrawSelectedBar(CurrentListPointer, TheArray[CurrentListPointer]);
                Task.Delay(Form1.Delay).Wait();
                Form1.Comparisons++;
                if (TheArray[CurrentListPointer] > TheArray[CurrentListPointer + 1])
                {
                    Rotate(CurrentListPointer);
                    Form1.Swaps++;
                }
                else
                {
                    DrawBar(CurrentListPointer, TheArray[CurrentListPointer]);
                }
                CurrentListPointer++;
            }
        }

        private void Rotate(int currentListPointer)
        {
            int temp = TheArray[CurrentListPointer];
            int EndPoint = TheArray.Count() - 1;
            for (int i = CurrentListPointer; i < EndPoint; i++)
            {
                TheArray[i] = TheArray[i + 1];
                DrawBar(i, TheArray[i]);
            }
            TheArray[EndPoint] = temp;
            DrawBar(EndPoint, TheArray[EndPoint]);
        }

        private void DrawBar(int position, int height)
        {
            g.FillRectangle(BlackBrush, (float)(position * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), 0, (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * .95), MaxVal);
            g.FillRectangle(WhiteBrush, (float)(position * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), MaxVal - TheArray[position], (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * .95), height);
        }

        private void DrawSelectedBar(int position, int height)
        {
            g.FillRectangle(PinkBrush, (float)(position * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), MaxVal - TheArray[position], (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * .95), height);
        }

        public bool IsSorted()
        {
            for (int i = 0; i < TheArray.Count() - 1; i++)
            {
                if (TheArray[i] > TheArray[i + 1]) return false;
            }
            return true;
        }

        public void ReDraw()
        {
            for (int i = 0; i < (TheArray.Count() - 1); i++)
            {
                g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), (float)(i * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), MaxVal - TheArray[i], (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * .95), MaxVal);
            }
        }
    }
}
