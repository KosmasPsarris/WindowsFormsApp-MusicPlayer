using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ThirdErgasia
{
    public partial class Form3 : Form
    {
        OpenFileDialog openFD = new OpenFileDialog
        {
            Filter = "Pictures (.jpg)|*.jpg;"
        };
        int Song;
        string Finalstring;
        Form1 f1;
        EditSong E1;
        bool BackPic = false;
        public Form3(int s, Form1 form)
        {
            InitializeComponent();
            f1 = form;
            Song = s;
        }

        private void Backb_Click(object sender, EventArgs e)
        {
            if(BackPic == true)
            {
                File.Delete("Music" + Song + ".jpg");
                File.Move("temp.jpg", "Music" + Song + ".jpg");
            }

            Close();
            f1.Show();
        }

        private void Acceptb_Click(object sender, EventArgs e)
        {
            //πρεπει να συμπληρωθουν ολα τα πεδια
            if (string.IsNullOrWhiteSpace(Title.Text))
            {
                MessageBox.Show("Please fill in the title of the song.");
            }
            else if (string.IsNullOrWhiteSpace(Artist.Text))
            {
                MessageBox.Show("Please fill in the artist of the song.");
            }
            else if (string.IsNullOrWhiteSpace(Date.Text))
            {
                MessageBox.Show("Please fill in the release date of the song.");
            }
            else if (string.IsNullOrWhiteSpace(Genre.Text))
            {
                MessageBox.Show("Please fill in the genre of the song.");
            }
            else if (string.IsNullOrWhiteSpace(Language.Text))
            {
                MessageBox.Show("Please fill in the language of the song.");
            }
            else if (pictureBox1.BackgroundImage == null)
            {
                MessageBox.Show("Please choose a picture for the song.");
            }
            else
            {
                Finalstring = Title.Text + Environment.NewLine +
                              Artist.Text + Environment.NewLine +
                              Date.Text + Environment.NewLine +
                              Genre.Text + Environment.NewLine +
                              Language.Text;

                File.WriteAllText("Music" + Song + ".txt", Finalstring);
                //η ενσωματωση των καινουργιων στοιχειων γινεται με την κλαση EditSong και την εφαμοργη της Editing
                E1 = new EditSong(Finalstring, f1);
                E1.Editing();

                if (BackPic == true)
                {
                    File.Delete("temp.jpg");
                }

                pictureBox1.BackgroundImage.Dispose();
                Close();
                f1.Show();
            }
        }

        private void chooseb_Click(object sender, EventArgs e)
        {
            if(File.Exists("Music" + Song + ".jpg"))
            {
                File.Delete("Music" + Song + ".jpg");
            }

            //Επιλεγεται μια εικονα (αλλιως βαζουμε την default εικονα) και αντιγραφεται στν φακελο Debug
            // με ονομα Music και τον αναλογο αριθμο τραγουδιου.
            if (openFD.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists("Music" + Song + ".jpg"))
                {
                    File.Delete("Music" + Song + ".jpg");
                }

                Image img;
                using (var bmpTemp = new Bitmap(openFD.FileName))
                {
                    img = new Bitmap(bmpTemp);
                }
                
                img.Save("Music" + Song + ".jpg");
                pictureBox1.BackgroundImage = img;
                

            }
            else
            {

                File.Copy("UnknownSong.jpg", "Music" + Song + ".jpg");
                pictureBox1.BackgroundImage = Image.FromFile("Music" + Song + ".jpg");
            }
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Backb.PerformClick();
        }

        private void chooseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chooseb.PerformClick();
        }

        private void acceptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Acceptb.PerformClick();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            BackPic = false; // αν υπαρχει παλια εικονα, την αντιγραφουμε στο temp για να την εχουμε σε περιπτωση
                                // που δεν θελει ο χρηστης να αλλαξει εικονα (Back)
            if (File.Exists("Music" + Song + ".jpg"))
            {
                File.Copy("Music" + Song + ".jpg","temp.jpg");
                BackPic = true;
            }
        }
    }
    class EditSong : Form1
    {
        Form1 f1;
        string RText;
        public EditSong(string richText, Form1 form)
        {
            f1 = form;
            RText = richText;
        }

        public void Editing()
        {
            //βρισκουμε το αναλογο τραγουδι (panel) και τροποποιουμε τισ πληροφοριες
            var pic = f1.Controls.Find("Checkbox" + f1.Song, true).FirstOrDefault() as CheckBox;
            var rich = f1.Controls.Find("Richtextbox" + f1.Song, true).FirstOrDefault() as RichTextBox;

            Image img;
            using (var bmpTemp = new Bitmap("Music" + f1.Song + ".jpg"))
            {
                img = new Bitmap(bmpTemp);
            }

            pic.BackgroundImage = img;
            rich.Text = RText;

            string line1 = File.ReadLines("Music" + f1.Song + ".txt").First();
            string line2 = File.ReadLines("Music" + f1.Song + ".txt").ElementAtOrDefault(1);
            f1.nameofsong.Text = line2+ " - " + line1;

        }
    }
}
