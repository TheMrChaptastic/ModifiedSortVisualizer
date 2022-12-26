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
        public static int Delay = 0;
        public static int Diff = 0;
        public static int MaxWidth = 0;
        public static int NumEntries = 0;
        public static int NumOfRuns = 0;
        public static float Seperation = 0.95F;
        public static bool IsCancelling = false;

        double[] TheArray;
        int speed = 0;
        bool _isWorking = false;
        Graphics g;
        BackgroundWorker bgw = null;

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

        public Form1()
        {
            InitializeComponent();
            PopulateDropdown();
        }

        private void PopulateDropdown()
        {
            List<string> ClassList = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(ISortEngine).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x.Name.Remove(0,10)).ToList();
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
            if(IsCancelling == true || _isWorking == true) 
            { 
                MessageBox.Show("Let current worker finish before starting another.\nClick 'Reset' if trying to make changes.");
                return; 
            }
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
                IsCancelling = true;
                if (!bgw.IsBusy)
                {
                    IsCancelling = false;
                }
            }
            ClearTrackings();
            g = panel1.CreateGraphics();
            MaxWidth = panel1.Width;
            NumEntries = 10;
            var isNum = int.TryParse(sizeTextBox.Text, out var num);
            if (isNum)
            {
                if (num >= 10 && num <= 1000)
                {
                    if ((double)num / MaxWidth < 0.03)
                    {
                        Seperation = 0.95F;
                    }
                    else
                    {
                        Seperation = 1;
                    }
                    NumEntries = num;
                }
                else
                {
                    MessageBox.Show("Please use a number within the accepted range! :)\nSetting to default. 10");
                    sizeTextBox.Text = "10";
                }
            }
            Diff = MaxWidth - NumEntries;
            int MaxVal = panel1.Height;
            TheArray = new double[NumEntries];
            g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, 0, MaxWidth, MaxVal);
            for (int i = 0; i < NumEntries; i++)
            {
                TheArray[i] = ((double)MaxVal / NumEntries) + (((double)MaxVal / NumEntries) * i);
            }
            Shuffle();
            for (int i = 0; i < NumEntries; i++)
            {
                if (i == NumEntries - 1){}
                g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), (float)(i * ((double)MaxWidth / NumEntries)), (float)(MaxVal - TheArray[i]), (float)(Math.Ceiling((double)MaxWidth / NumEntries) * Seperation), MaxVal);
            }
            if (sender != null)
            {
                btnReset_Click(null, null);
            }
        }

        private void Shuffle()
        {
            Random rand = new Random();
            var temp = TheArray[TheArray.Length - 1];
            for(int i = TheArray.Length - 1; i >= 0; i--)
            {
                temp = TheArray[i];
                var place = rand.Next(0, i);
                TheArray[i] = TheArray[place];
                TheArray[place] = temp;
            }
        }

        #region BackGroundStuff

        public void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Delay = GetDelay();
            BackgroundWorker bw = sender as BackgroundWorker;
            BackgroundWorker zz = new BackgroundWorker();
            zz.DoWork += new DoWorkEventHandler(bw_DoWork);
            _isWorking = true;

            string SortEngineName = (string)e.Argument;
            if(!ConfirmLongRunTime(SortEngineName))
            { 
                return; 
            }

            Type type = Type.GetType("SortVisualizer.SortEngine" + SortEngineName);
            var ctors = type.GetConstructors();
            try
            {
                ISortEngine se = (ISortEngine)ctors[0].Invoke(new object[] { TheArray, g, panel1.Height });
                while (!se.IsSorted() && (!bgw.CancellationPending))
                {
                    se.NextStep();
                    NumOfRuns++;

                    if (!zz.IsBusy)
                    {
                        zz.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            if (IsCancelling)
            {
                ClearTrackings();
                btnReset_Click(null, null);
            }
            IsCancelling = false;
            _isWorking = false;
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

        private bool ConfirmLongRunTime(string engine)
        {
            if (engine.Contains("VerySlow") && TheArray.Length >= 30 && speed < 3)
            {
                DialogResult dialogResult = MessageBox.Show("This can take a MANY minutes to finish.\nContinue?", "Very Slow Sorting Method", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return false;
                }
            }
            if (TheArray.Length >= 100 && speed < 2)
            {
                DialogResult dialogResult = MessageBox.Show("This can take a many minutes to finish.\nContinue?", "Big Sample Size", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateLabel()
        {
            compLabel.Text = Comparisons.ToString();
            swapLabel.Text = Swaps.ToString();
        }

        private void ClearTrackings()
        {
            NumOfRuns = 0;
            Swaps = 0;
            Comparisons = 0;
            UpdateLabel();
        }

        #endregion
    }
}
