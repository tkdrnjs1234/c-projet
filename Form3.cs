using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MDD_Text
{
    public partial class Form3 : Form
    {
        public string FilePath = "";

        public Form3()
        {
            InitializeComponent();
        }

        public RichTextBox Editor
        {
            get { return richTextBox1; }
        }
    }
}