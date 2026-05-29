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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            Form2? parent = this.MdiParent as Form2;

            if (parent != null)
            {
                parent.SetStatus("작성 중");
            }
        }
    }
}