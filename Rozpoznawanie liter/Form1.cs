using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Rozpoznawanie_liter
{
    public partial class Form1 : Form

    {
        private Web nw;
        private Point startP;
        private int[,] arr;

        private bool enableTraining;

        public Form1()
        {
            InitializeComponent();
            enableTraining = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Graph.ClearImage(pictureBox1);
            nw = new Web();
            string[] items = nw.GetLiteras();
            if (items.Length > 0)
            {
                comboBox1.Items.AddRange(items);
                comboBox1.SelectedIndex = 0;
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                Point endP = new Point(e.X, e.Y);
                Bitmap image = (Bitmap)pictureBox1.Image;
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.DrawLine(new Pen(Color.Black, 7), startP, endP);
                }
                pictureBox1.Image = image;
                startP = endP;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startP = new Point(e.X, e.Y);
        }
















        private void Learn()
        {
            int[,] clipArr = Graph.CutImageToArray((Bitmap)pictureBox1.Image, new Point(pictureBox1.Width, pictureBox1.Height));
            if (clipArr == null) return;
            arr = Graph.LeadArray(clipArr, new int[Web.neuronInArrayWidth, Web.neuronInArrayHeight]);
            pictureBox2.Image = Graph.GetBitmapFromArr(clipArr);
            pictureBox3.Image = Graph.GetBitmapFromArr(arr);
            string s = nw.CheckLitera(arr);
            if (s == null) s = "null";
            label6.Text = s;
            DialogResult askResult = MessageBox.Show("Wynik rozpoznawania - " + s + " ?", "", MessageBoxButtons.YesNo);
            if (askResult != DialogResult.Yes || !enableTraining || MessageBox.Show("Dodaj ten obraz do pamięci neuronu '" + s + "'", "", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            nw.SetTraining(s, arr);
            // wyraźne zdjęcia
            Graph.ClearImage(pictureBox1);
            Graph.ClearImage(pictureBox2);
            Graph.ClearImage(pictureBox3);
        }


        private void Write()
        {
            int[,] clipArr = Graph.CutImageToArray((Bitmap)pictureBox1.Image, new Point(pictureBox1.Width, pictureBox1.Height));
            if (clipArr == null) return;
            arr = Graph.LeadArray(clipArr, new int[Web.neuronInArrayWidth, Web.neuronInArrayHeight]);
            pictureBox2.Image = Graph.GetBitmapFromArr(clipArr);
            pictureBox3.Image = Graph.GetBitmapFromArr(arr);
            string s = nw.CheckLitera(arr);
            if (s == null) s = "null";
            textBox2.Text = s;
            Graph.ClearImage(pictureBox1);
            Graph.ClearImage(pictureBox2);
            Graph.ClearImage(pictureBox3);
            nw.SetTraining(s, arr);
            label6.Text = s;
        }

        // umieszczanie ciągu znaków na liście wartości
        private void AddSymbolToList(string symbol)
        {
            if (symbol == null || symbol.Length == 0)
            {
                MessageBox.Show("Wartość nie może zawierać 0 znaków..");
                return;
            }
            comboBox1.Items.Add(symbol);
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            MessageBox.Show("Teraz wartość '" + symbol + "' na liście możesz teraz nauczyć sieć rozpoznawania.");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) AddSymbolToList(((TextBox)sender).Text);
        }

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            if (radioButton1.Checked)      //trening
            {
                label3.Visible = true;
                textBox1.Visible = true;
                button1.Visible = true;
                label6.Visible = true;
                pictureBox3.Visible = true;
                comboBox1.Visible = true;
                label2.Visible = true;
                label4.Visible = true;

                if (e.Button == MouseButtons.Left) Learn();
            }
            if (radioButton2.Checked)         //rozpoznawanie
            {

                label3.Visible = false;
                textBox1.Visible = false;
                button1.Visible = false;
                comboBox1.Visible = false;
                enableTraining = true;
                label2.Visible = false;
                label4.Visible = false;
                //toMemoryToolStripMenuItem.Enabled = false;
                label6.Visible = false;
                pictureBox3.Visible = false;
                if (e.Button == MouseButtons.Left) Write();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            enableTraining = true;
            //toMemoryToolStripMenuItem.Enabled = true;
            AddSymbolToList(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            Graph.ClearImage(pictureBox1);
            Graph.ClearImage(pictureBox2);
            Graph.ClearImage(pictureBox3);
            label6.Text = "";
        }

        private void dodajWartośćDoPamięciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string litera = comboBox1.SelectedIndex >= 0 ? (string)comboBox1.Items[comboBox1.SelectedIndex] : comboBox1.Text;
            if (litera.Length == 0)
            {
                MessageBox.Show("Do zapamiętania nie wybrano żadnych znaków.");
                return;
            }
            nw.SetTraining(litera, arr);
            Graph.ClearImage(pictureBox1);
            Graph.ClearImage(pictureBox2);
            Graph.ClearImage(pictureBox3);
            MessageBox.Show("Wybrana postać '" + litera + "' pomyślnie dodane do pamięci sieciowej.");
            
        }

        private void drawFromComboBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graph.ClearImage(pictureBox1);
            Graph.ClearImage(pictureBox2);
            Graph.ClearImage(pictureBox3);
            pictureBox1.Image = Graph.DrawLitera(pictureBox1.Image, (string)comboBox1.SelectedItem);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
                nw.SaveState();
        }
    }
}
