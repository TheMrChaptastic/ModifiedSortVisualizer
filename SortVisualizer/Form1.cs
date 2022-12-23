using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SortVisualizer
{
    public partial class Form1 : Form
    {
        int[] TheArray;
        Graphics g;
        BackgroundWorker bgw = null;

        int speed = 0;

        public static int Delay = 0;
        public static int Diff = 0;
        public static int MaxWidth = 0;
        public static int NumEntries = 0;

        static int _swaps = 0;
        public static int Swaps
        {
            get => _swaps;
            set => _swaps = value;
        }

        static int _comparisons = 0;
        public static int Comparisons
        {
            get => _comparisons;
            set => _comparisons = value;
        }

        private void UpdateLabel()
        {
            compLabel.Text = Comparisons.ToString();
            swapLabel.Text = Swaps.ToString();
        }

        public Form1()
        {
            InitializeComponent();
            PopulateDropdown();
        }

        private void PopulateDropdown()
        {
            List<string> ClassList = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(ISortEngine).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x.Name).ToList();
            ClassList.Sort();
            foreach (string entry in ClassList)
            {
                comboBox1.Items.Add(entry);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (TheArray == null) btnReset_Click(null, null);

            speed = speedBar.Value;
            bgw = new BackgroundWorker();
            bgw.WorkerSupportsCancellation = true;
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerAsync(argument: comboBox1.SelectedItem);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (bgw != null)
            {
                bgw.CancelAsync();
            }
            Swaps = 0;
            Comparisons = 0;
            UpdateLabel();
            g = panel1.CreateGraphics();
            MaxWidth = panel1.Width;
            NumEntries = MaxWidth;
            var isNum = int.TryParse(sizeTextBox.Text, out var num);
            if (isNum)
            {
                if (num >= 10 && num <= 1095)
                {
                    NumEntries = num;
                }
            }
            Diff = MaxWidth - NumEntries;
            int MaxVal = panel1.Height;
            TheArray = new int[NumEntries];
            g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, 0, MaxWidth, MaxVal);
            Random rand = new Random();
            for (int i = 0; i < NumEntries; i++)
            {
                TheArray[i] = rand.Next(0, MaxVal);
            }
            for (int i = 0; i < NumEntries; i++)
            {
                g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), (float)(i * (Math.Ceiling((double)MaxWidth / NumEntries))), MaxVal - TheArray[i], (float)(Math.Ceiling((double)MaxWidth / NumEntries)), MaxVal);
            }
        }

        #region BackGroundStuff

        public void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Delay = GetDelay();

            BackgroundWorker bw = sender as BackgroundWorker;

            BackgroundWorker zz = new BackgroundWorker();
            zz.DoWork += new DoWorkEventHandler(bw_DoWork);

            string SortEngineName = (string)e.Argument;
            Type type = Type.GetType("SortVisualizer." + SortEngineName);
            var ctors = type.GetConstructors();
            try
            {
                ISortEngine se = (ISortEngine)ctors[0].Invoke(new object[] { TheArray, g, panel1.Height });
                while (!se.IsSorted() && (!bgw.CancellationPending))
                {
                    se.NextStep();

                    if (!zz.IsBusy)
                    {
                        zz.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { bw_DoWork(sender, e); });
                return;
            }
            UpdateLabel();
        }

        public int GetDelay()
        {
            switch (speed)
            {
                case (0):
                    return 250;
                case (1):
                    return 50;
                case (2):
                    return 10;
                case (3):
                    return 5;
                case (4):
                default:
                    return 0;
            }
        }

        #endregion
    }
}
