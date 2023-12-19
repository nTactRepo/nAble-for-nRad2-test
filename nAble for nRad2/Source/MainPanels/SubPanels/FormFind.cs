using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormFind : Form
    {
        public string SearchString;
        public FormFind()
        {
            InitializeComponent();
        }

        private void textBoxSearchString_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SearchString = textBoxSearchString.Text;
        }
    }
}
