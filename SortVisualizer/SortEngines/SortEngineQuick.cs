﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortVisualizer 
{
    class SortEngineQuick : ISortEngine
    {
        private int originalLeftIndex;
        private int originalRightIndex;
        private double[] TheArray;
        private Graphics g;
        private int MaxVal;
        Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        Brush PinkBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Pink);

        public SortEngineQuick(double[] TheArray_In, Graphics g_In, int MaxVal_In)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;

            originalLeftIndex = 0;
            originalRightIndex = TheArray.Length - 1;

            ReDraw();
        }

        public void NextStep()
        {
            //Using Recursion for this so it doesnt exactly fit the setup to use NextStep well. so using if statements to check if cancelling is occuring to exit early out of recursion.
            SortArray(TheArray, originalLeftIndex, originalRightIndex); 
        }

        public void SortArray(double[] array, int leftIndex, int rightIndex)
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
                    if (Form1.IsCancelling)
                    {
                        return;
                    }
                    Form1.Comparisons++;
                    Task.Delay(Form1.Delay).Wait();
                    DrawBar(i, array[i]);
                    i++;
                    DrawSelectedBar(i, array[i]);
                }

                while (array[p] > pivot)
                {
                    if (Form1.IsCancelling)
                    {
                        return;
                    }
                    Form1.Comparisons++;
                    Task.Delay(Form1.Delay).Wait();
                    DrawBar(p, array[p]);
                    p--;
                    DrawSelectedBar(p, array[p]);
                }
                if (i <= p)
                {
                    DrawSelectedBar(i, array[i]);
                    DrawSelectedBar(p, array[p]);
                    Task.Delay(Form1.Delay).Wait();
                    Swap(i, p);
                    Form1.Swaps++;
                    i++;
                    p--;
                }
            }

            if (leftIndex < p)
            {
                DrawSelectedBar(p, array[p]);
                DrawBar(i, array[i]);
                SortArray(array, leftIndex, p);
            }
            if (i < rightIndex)
            {
                DrawSelectedBar(i, array[i]);
                DrawBar(p, array[p]);
                SortArray(array, i, rightIndex);
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
            g.FillRectangle(BlackBrush, (float)(position * ((double)Form1.MaxWidth / Form1.NumEntries)), 0, (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries)), MaxVal);
            g.FillRectangle(WhiteBrush, (float)(position * ((double)Form1.MaxWidth / Form1.NumEntries)), (float)(MaxVal - TheArray[position]), (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * Form1.Seperation), (float)height);
        }

        private void DrawSelectedBar(int position, double height)
        {
            g.FillRectangle(PinkBrush, (float)(position * ((double)Form1.MaxWidth / Form1.NumEntries)), (float)(MaxVal - TheArray[position]), (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * Form1.Seperation), (float)height);
        }

        public bool IsSorted()
        {
            ReDraw();
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
                g.FillRectangle(WhiteBrush, (float)(i * ((double)Form1.MaxWidth / Form1.NumEntries)), (float)(MaxVal - TheArray[i]), (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * Form1.Seperation), MaxVal);
            }
        }
    }
}