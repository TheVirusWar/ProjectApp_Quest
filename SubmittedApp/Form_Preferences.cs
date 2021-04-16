using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectApp_Quest
{
    public partial class Form_Preferences : Form
    {
        public int RecentFiles { get; set; }

        public Form_Preferences()
        {
            InitializeComponent();
        }

        private void ButtonPreferencesOK_Click(object sender, EventArgs e)
        {
            if(int.TryParse(textBoxRecentNumber.Text, out int number))
            {
                RecentFiles = number;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Number of recent files must be an integer");
            }
        }
    }
}
