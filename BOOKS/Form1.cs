using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace BOOKS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (!File.Exists("Meta.txt"))
                File.Create("Meta.txt");

            StreamReader metaInput = new StreamReader("Meta.txt");
            while (!metaInput.EndOfStream)
            {
                string[] tmp = metaInput.ReadLine().Split(' ');
                Data.bookList.Add(new Book(new string[] { tmp[0] }, tmp[1], tmp[2], tmp[3], tmp[4]));
            }
            metaInput.Close();
            LoadToListbox();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button5_Click(sender, e);
        }
        private void LoadToListbox(object comparer = null)
        {
            if (comparer == null)
                comparer = new Record.SortByYear();
            if (comparer is IComparer<Book>)
            {
                Data.bookList.Sort((IComparer<Book>)comparer);
                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                foreach (var book in Data.bookList)
                    listBox1.Items.Add(book.ConvertToTable());
                listBox1.EndUpdate();
            }
            else
                throw new Exception("Wrong Comparer");
        }
        private void LoadToListbox(List<Book> LoadList, object comparer = null)
        {
            if (comparer == null)
                comparer = new Record.SortByYear();
            if (comparer is IComparer<Book>)
            {
                Data.bookList.Sort((IComparer<Book>)comparer);
                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                foreach (var book in LoadList)
                {
                    listBox1.Items.Add(book.ConvertToTable());
                }
                listBox1.EndUpdate();
            }
            else
                throw new Exception("Wrong Comparer");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LoadToListbox(new Book.SortByAutor(Data.sortingInversionAutor));
            Data.sortingInversionAutor ^= true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            LoadToListbox(new Book.SortByName(Data.sortingInversionName));
            Data.sortingInversionName ^= true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            LoadToListbox(new Book.SortByCost(Data.sortingInversionCost));
            Data.sortingInversionCost ^= true;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            LoadToListbox(new Book.SortByYear(Data.sortingInversionYear));
            Data.sortingInversionYear ^= true;
        }
        private long FindID(string autor, string name, string year = "")
        {
            foreach (var book in Data.bookList)
            {
                if (book.autors[0] == autor)
                    if (book.name == name)
                        if ((year == "" || book.cost.Value == int.Parse(year)))
                            return book.ID;
            }
            return -1;
        }
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            добавление_книги newForm = new добавление_книги();
            newForm.Show();
        }
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                string tmp = ((string)listBox1.SelectedItem).Replace("\0", "").Replace("  ", " ");
                tmp = tmp.Trim();
                while (tmp.Contains("  "))
                    tmp = tmp.Replace("  ", " ");

                string[] tmpArray = tmp.Split(' ');
                long ID = FindID(tmpArray[0], tmpArray[1]);
                if (ID < 0) MessageBox.Show("No such book!?!?!?");
                else foreach (var book in Data.bookList)
                        if (book.ID == ID)
                        {
                            Process.Start(Environment.CurrentDirectory + '\\' + book.file);
                            break;
                        }
            }
        }
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                string tmp = ((string)listBox1.SelectedItem).Replace("\0", "").Replace("  ", " ");
                tmp = tmp.Trim();
                while (tmp.Contains("  "))
                    tmp = tmp.Replace("  ", " ");

                string[] tmpArray = tmp.Split(' ');
                long ID = FindID(tmpArray[0], tmpArray[1]);

                Book tamptamp = null;
                if (ID < 0) MessageBox.Show("No such book!?!?!?");
                else foreach (var book in Data.bookList)
                        if (book.ID == ID)
                        {
                            tamptamp = book;
                            break;
                        }
                Data.bookList.Remove(tamptamp);
                Data.deleteList.Add(tamptamp.file);
                LoadToListbox();
            }
        }
        private void сохранитьИзмененияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter metaWriter = new StreamWriter("Meta.txt");
            foreach (var item in Data.bookList)
            {
                metaWriter.WriteLine(item.ToString());
            }
            metaWriter.Close();
            foreach (var file in Data.deleteList)
                if (File.Exists(Environment.CurrentDirectory + '\\' + file))
                    File.Delete(Environment.CurrentDirectory + '\\' + file);
            Data.deleteList.Clear();

        }
        private void запросToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableLayoutPanel3.RowStyles[0].Height = 120;
        }
        public enum ChooseMethod
        {
            OnlyFrom,
            OnlyTo,
            FromTo,
            Any
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChooseMethod Autor;
            ChooseMethod Name;
            ChooseMethod Cost;
            ChooseMethod Date;

            if (textBox1.Text != "")
                Autor = ChooseMethod.FromTo;
            else
                Autor = ChooseMethod.Any;

            if (textBox2.Text != "")
                Name = ChooseMethod.FromTo;
            else
                Name = ChooseMethod.Any;

            if (textBox3.Text != "")
            {
                if (textBox5.Text != "")
                    Cost = ChooseMethod.FromTo;
                else
                    Cost = ChooseMethod.OnlyFrom;
            }
            else
            {
                if (textBox5.Text != "")
                    Cost = ChooseMethod.OnlyTo;
                else
                    Cost = ChooseMethod.Any;
            }

            if (textBox4.Text != "")
            {
                textBox4.Text = DatePreparing(textBox4.Text);
                DateTime tmpr = new DateTime();
                if (DateTime.TryParse(textBox4.Text, out tmpr))
                { textBox4.Text = tmpr.ToString().Substring(0, 10); }
                else
                { MessageBox.Show("UncorrectDate"); return; }
                if (textBox6.Text != "")
                {
                    Date = ChooseMethod.FromTo;
                    textBox6.Text = DatePreparing(textBox6.Text);
                    if (DateTime.TryParse(textBox6.Text, out DateTime tmpw))
                        textBox6.Text = tmpw.ToString().Substring(0, 10);
                    else
                    {
                        MessageBox.Show("UncorrectDate"); 
                        return; 
                    }
                }
                else
                    Date = ChooseMethod.OnlyFrom;
            }
            else
            {
                if (textBox6.Text != "")
                {
                    Date = ChooseMethod.OnlyTo;
                    textBox6.Text = DatePreparing(textBox6.Text);
                    if (DateTime.TryParse(textBox6.Text, out DateTime tmp))
                        textBox6.Text = tmp.ToString().Substring(0, 10); 
                    else
                    { 
                        MessageBox.Show("UncorrectDate");
                        return;
                    }
                }
                else
                    Date = ChooseMethod.Any;
            }

            listBox1.Items.Clear();
            List<Book> LoadList = new List<Book>();

            foreach (var book in Data.bookList)
            {
                if ((Autor == ChooseMethod.OnlyTo || Autor == ChooseMethod.FromTo)
                    && (!book.autors[0].Contains(textBox1.Text)))
                    continue;

                if ((Name == ChooseMethod.OnlyTo || Name == ChooseMethod.FromTo)
                    && (!book.name.Contains(textBox2.Text)))
                    continue;

                if ((Cost == ChooseMethod.OnlyTo || Cost == ChooseMethod.FromTo)
                    && (book.cost > int.Parse(textBox5.Text)))
                    continue;

                if ((Cost == ChooseMethod.OnlyFrom || Cost == ChooseMethod.FromTo)
                    && (book.cost < int.Parse(textBox3.Text)))
                    continue;

                if ((Date == ChooseMethod.OnlyTo || Cost == ChooseMethod.FromTo)
                    && (!book.date.HasValue
                    || book.date > Convert.ToDateTime(textBox6.Text)))
                    continue;
                if ((Date == ChooseMethod.OnlyFrom || Cost == ChooseMethod.FromTo)
                    && (!book.date.HasValue
                    || book.date < Convert.ToDateTime(textBox4.Text)))
                    continue;
                LoadList.Add(book);
            }
            LoadToListbox(LoadList);
        }
        private void ClearFields()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";

        }

        private string DatePreparing(string str)
        {
            char[] tmp = str.Replace(",", ".").Replace(" ", ".").ToCharArray();
            char[] findChars2 = Array.FindAll(tmp, Predicat);
            if (findChars2.Length < 1)
                str = "01." + str.Replace(",", ".").Replace(" ", ".");
            if (findChars2.Length < 2)
                str = "01." + str.Replace(",", ".").Replace(" ", ".");
            return str;
        }

        private static bool Predicat(char chr)
        {
            if (chr == '.') return true;
            else return false;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            ClearFields();
            button4_Click(sender, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tableLayoutPanel3.RowStyles[0].Height = 1;
            button6_Click(sender, e);
        }
    }
}
