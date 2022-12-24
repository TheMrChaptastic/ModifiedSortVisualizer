﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortVisualizer
{
    class SortEngineBruteForce : ISortEngine
    {
        private int[] TheArray;
        private Graphics g;
        private int MaxVal;
        Brush WhiteBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
        Brush BlackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        Brush PinkBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Pink);

        public SortEngineBruteForce(int[] TheArray_In, Graphics g_In, int MaxVal_In)
        {
            TheArray = TheArray_In;
            g = g_In;
            MaxVal = MaxVal_In;

            ReDraw();
        }

        public void NextStep()
        {
            for (int i = 0; i < TheArray.Count() - 1; i++)
            {
                for (int u = i + 1; u < TheArray.Count(); u++)
                {
                    if (Form1.IsCancelling)
                    {
                        break;
                    }
                    DrawSelectedBar(i, TheArray[i]);
                    DrawSelectedBar(u, TheArray[u]);
                    Task.Delay(Form1.Delay).Wait();
                    Form1.Comparisons++;
                    if (TheArray[i] > TheArray[u])
                    {
                        Swap(i, u);
                        Form1.Swaps++;
                    }
                    else
                    {
                        DrawBar(i, TheArray[i]);
                        DrawBar(u, TheArray[u]);
                    }
                }
            }
        }

        private void Swap(int i, int p)
        {
            int temp = TheArray[i];
            TheArray[i] = TheArray[p];
            TheArray[p] = temp;

            DrawBar(i, TheArray[i]);
            DrawBar(p, TheArray[p]);
        }

        private void DrawBar(int position, int height)
        {
            g.FillRectangle(BlackBrush, (float)(position * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), 0, (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries)), MaxVal);
            g.FillRectangle(WhiteBrush, (float)(position * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), MaxVal - TheArray[position], (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * Form1.Seperation), height);
        }

        private void DrawSelectedBar(int position, int height)
        {
            g.FillRectangle(PinkBrush, (float)(position * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), MaxVal - TheArray[position], (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * Form1.Seperation), height);
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
                g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), (float)(i * (Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries))), MaxVal - TheArray[i], (float)(Math.Ceiling((double)Form1.MaxWidth / Form1.NumEntries) * Form1.Seperation), MaxVal);
            }
        }
    }
}