using SortVisualizer.Factory;
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
        double[] TheArray;
        int speed = 0;
        bool _isWorking = false;
        Graphics g;


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

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if(Constants.IsCancelling == true || _isWorking == true) 
            { 
                MessageBox.Show("Let current worker finish before starting another.\nClick 'Reset' if trying to make changes.");
                return; 
            }
            if (TheArray == null) btnReset_Click(null, null);

            speed = speedBar.Value;
            await DoWork(comboBox1.SelectedItem);
        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            if (_isWorking)
            {
                Constants.IsCancelling = true;
            }
            g = panel1.CreateGraphics();
            Constants.MaxWidth = panel1.Width;
            Constants.NumEntries = 10;
            var isNum = int.TryParse(sizeTextBox.Text, out var num);
            if (isNum)
            {
                if (num >= 10 && num <= 1000)
                {
                    if ((double)num / Constants.MaxWidth < 0.03)
                    {
                        Constants.Seperation = 0.95F;
                    }
                    else
                    {
                        Constants.Seperation = 1;
                    }
                    Constants.NumEntries = num;
                }
                else
                {
                    MessageBox.Show("Please use a number within the accepted range! :)\nSetting to default. 10");
                    sizeTextBox.Text = "10";
                }
            }
            Constants.Diff = Constants.MaxWidth - Constants.NumEntries;
            int MaxVal = panel1.Height;
            TheArray = new double[Constants.NumEntries];
            g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, 0, Constants.MaxWidth, MaxVal);
            for (int i = 0; i < Constants.NumEntries; i++)
            {
                TheArray[i] = ((double)MaxVal / Constants.NumEntries) + (((double)MaxVal / Constants.NumEntries) * i);
            }
            Shuffle();
            for (int i = 0; i < Constants.NumEntries; i++)
            {
                if (i == Constants.NumEntries - 1){}
                g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.White), (float)(i * ((double)Constants.MaxWidth / Constants.NumEntries)), (float)(MaxVal - TheArray[i]), (float)(Math.Ceiling((double)Constants.MaxWidth / Constants.NumEntries) * Constants.Seperation), MaxVal);
            }

            await ClearTrackings();
        }

        #region BackGroundStuff

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

        public async Task DoWork(object engineName)
        {
            Constants.Delay = GetDelay();
            _isWorking = true;

            var Engine = engineName.ToString();
            var confirmContinue = await ConfirmLongRunTimeAsync(Engine);
            if (!confirmContinue)
            { 
                return; 
            }

            try
            {
                var se = SortEngineFactory.GetSortEngine(Engine, TheArray, g, panel1.Height);
                while (!await se.IsSorted() && !Constants.IsCancelling)
                {
                    await se.NextStep();
                    Constants.NumOfRuns++;

                    await UpdateLabel();
                }
            }
            catch (Exception e)
            { }

            if (Constants.IsCancelling)
            {
                await ClearTrackings();
                btnReset_Click(null, null);
            }
            Constants.IsCancelling = false;
            _isWorking = false;
        }



        public int GetDelay()
        {
            switch (speed)
            {
                case (0):
                    return 250;
                case (1):
                    return 100;
                case (2):
                    return 20;
                case (3):
                    return 5;
                case (4):
                    return 1;
                case (5):
                default:
                    return 0;
            }
        }

        private async Task<bool> ConfirmLongRunTimeAsync(string engine)
        {
            return await Task.Run(() =>
            {
                if (engine.Contains("VerySlow") && TheArray.Length >= 30 && speed < 3)
                {
                    DialogResult dialogResult = MessageBox.Show("This can take a MANY minutes to finish.\nContinue?", "Very Slow Sorting Method", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        return false;
                    }
                }
                if (TheArray.Length >= 100 && speed < 4)
                {
                    if (!engine.Contains("Quick") && speed < 2)
                    {
                        DialogResult dialogResult = MessageBox.Show("This can take a many minutes to finish.\nContinue?", "Big Sample Size", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.No)
                        {
                            return false;
                        }
                    }
                }
                else if (TheArray.Length >= 200 && speed < 5 && !engine.Contains("Quick"))
                {
                    if (!engine.Contains("Quick") && speed < 3)
                    {
                        DialogResult dialogResult = MessageBox.Show("This can take a many minutes to finish.\nContinue?", "Big Sample Size", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.No)
                        {
                            return false;
                        }
                    }
                }
                return true;
            });
        }

        private async Task UpdateLabel()
        {
            compLabel.Text = Constants.Comparisons.ToString();
            swapLabel.Text = Constants.Swaps.ToString();
            await Task.Delay(10);
        }

        private async Task ClearTrackings()
        {
            await Task.Run(() =>
            {
                Constants.NumOfRuns = 0;
                Constants.Swaps = 0;
                Constants.Comparisons = 0;
            });
            await UpdateLabel();
        }

        #endregion
    }
}
