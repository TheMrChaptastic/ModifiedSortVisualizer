﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortVisualizer
{
    class SortEngineVerySlowMoveToBack : ISortEngine
    {
        private double[] TheArray;
        private Graphics g;
        private int MaxVal;
        Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        Brush PinkBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Pink);

        private int CurrentListPointer = 0;

        public SortEngineVerySlowMoveToBack(double[] TheArray_In, Graphics g_In, int MaxVal_In)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;

            ReDraw();
        }

        public async Task NextStep()
        {
            if (!Constants.IsCancelling)
            {
                if (CurrentListPointer >= TheArray.Count() - 1) CurrentListPointer = 0;
                DrawSelectedBar(CurrentListPointer, TheArray[CurrentListPointer]);
                await Task.Delay(Constants.Delay);
                Constants.Comparisons++;
                if (TheArray[CurrentListPointer] > TheArray[CurrentListPointer + 1])
                {
                    Rotate(CurrentListPointer);
                    Constants.Swaps++;
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
                g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), (float)(i * ((double)Constants.MaxWidth / Constants.NumEntries)), (float)(MaxVal - TheArray[i]), (float)(Math.Ceiling((double)Constants.MaxWidth / Constants.NumEntries) * Constants.Seperation), MaxVal);
            }
        }
    }
}
