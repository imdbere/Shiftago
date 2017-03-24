using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shiftago
{
    public partial class StartScreen : Form
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            labelSL.Text = trackBar3.Value.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            labelSZ.Text = trackBar1.Value.ToString();
            trackBarBot.Maximum = trackBar1.Value;
            trackBarBot_Scroll(sender, e);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            labelFG.Text = trackBar2.Value.ToString();
            CheckBotPossible();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            GameScreen MainForm = new GameScreen(trackBar2.Value, trackBar1.Value, trackBar3.Value, trackBarBot.Value);
            this.Hide();
            MainForm.ShowDialog();
            Close();
        }

        void CheckBotPossible()
        {
            if (trackBarBot.Value > 0 && trackBar2.Value != 7)
                buttonStart.Enabled = false;
            else
                buttonStart.Enabled = true;
        }
        private void trackBarBot_Scroll(object sender, EventArgs e)
        {
            labelBot.Text = trackBarBot.Value.ToString();
            CheckBotPossible();
        }
    }
}
