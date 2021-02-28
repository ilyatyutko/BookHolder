using System;
using System.IO;
using System.Windows.Forms;

namespace BOOKS
{
    public partial class добавление_книги : Form
    {
        public добавление_книги()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = "";
            string file = "";
            string cost = "";
            string date = "";
            string[] author = new string[1];


            if (textBox1.Text == "")
            { MessageBox.Show("Uncorrect Name"); return; }
            else name = textBox1.Text;
            if (textBox2.Text == "")
            { MessageBox.Show("Uncorrect File destination"); return; }
            else
            {
                if (File.Exists(textBox2.Text))
                    file = textBox2.Text;
                else
                { MessageBox.Show("Uncorrect File destination"); return; }
            }
            if (textBox3.Text != "")
            {
                if (int.TryParse(textBox3.Text, out int tmp))
                { cost = textBox3.Text; }
                else
                { MessageBox.Show("Uncorrect Cost"); return; }
            }
            if (textBox4.Text != "")
            {
                char[] tmp2 = textBox4.Text.Replace(",", ".").Replace(" ", ".").ToCharArray();
                char[] findChars2 = Array.FindAll(tmp2, Predicat);
                if (findChars2.Length < 1)
                    textBox4.Text = "01." + textBox4.Text.Replace(",", ".").Replace(" ", ".");
                if (findChars2.Length < 2)
                    textBox4.Text = "01." + textBox4.Text.Replace(",", ".").Replace(" ", ".");
                if (DateTime.TryParse(textBox4.Text, out DateTime tmpr))
                { textBox4.Text = tmpr.ToString().Substring(0, 10); }
                else
                { MessageBox.Show("UncorrectDate"); return; }
                if (DateTime.TryParse(textBox4.Text, out DateTime tmp))
                { date = textBox4.Text; }
                else
                { MessageBox.Show("UncorrectDate"); return; }
            }
            if (textBox5.Text == "")
            {
                MessageBox.Show("Must be Author");
                return;
            }
            else
            { author[0] = textBox5.Text; }

            Book newOne = new Book(
                author, name, cost, file, date);
            Data.bookList.Add(newOne);
            this.Close();
        }

        private static bool Predicat(char chr)
        {
            if (chr == '.') return true;
            else return false;
        }
    }
}
