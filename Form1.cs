using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ThirdErgasia
{
    public partial class Form1 : Form // MusicPlayer
    {
        List<int> RandomNumbers = new List<int>();
        List<int> DeletedNumbers = new List<int>();
        public List<string> ExistingSongsAdd = new List<string>();
        List<string> ExistingSongsDelete = new List<string>();
        public List<int> FavoredSongs = new List<int>();
        private WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
        static string DebugPath = AppDomain.CurrentDomain.BaseDirectory; // φακελος Debug που εχει τα απαραιτητα αρχεια
        DirectoryInfo dir = new DirectoryInfo(DebugPath + "\\Delete");
        Random random = new Random();
        int rand;
        double pausedmoment;
        public int Song = 0;
        int NumberofSongs = 0;
        bool notStartTwice = false;
        bool StopPauseSynergy = false;
        bool Favor = false;
        string line1, line2;
        string ExistingSongs = "";
        string ExistngFavors = "";
        public string ExistString;
        OpenFileDialog openFD = new OpenFileDialog
        {
            Filter = "Music (.mp3)|*.mp3"
        };
        NewSong N1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Size = new Size(915, 600);
            StartPosition = FormStartPosition.CenterScreen;

            //αν εχουν αποθηκευτει τραγουδια στην εφαρμογη
            if (File.Exists("NumberSongs.txt") && !(new FileInfo("NumberSongs.txt").Length == 0))
            {
                //τοτε με την βοθεια πινακων 
                ExistingSongs = File.ReadAllText("NumberSongs.txt");
                string[] tempo1 = ExistingSongs.Split(' ');

                ExistngFavors = File.ReadAllText("FavorPoints.txt");
                string[] tempo2 = ExistngFavors.Split(' ');

                foreach (string item in tempo1)
                {
                    //δημιουργουμαι panel για καθενα απο αυτα κρατοντας τις απαραιτητες πληροφοριες
                    if(!string.IsNullOrWhiteSpace(item))
                    {
                        ExistingSongsAdd.Add(item);//αριθμοι τραγουδιων που υπαρχουν
                        ExistString = File.ReadAllText("Music" + item + ".txt");//πληροφοριες
                        N1 = new NewSong("Music" + item + ".mp3", this, true);
                        N1.Paneling();
                    }
                }
                foreach (string item in tempo2)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        FavoredSongs.Add(Int32.Parse(item)); // επαναφερουμε τους ποντους προτιμησης στο 
                    }                                        // αναλογο τραγουδι
                }
                NumberofSongs = ExistingSongsAdd.Count;
            }
            ExistingSongs = "";
            ExistngFavors = "";
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (openFD.ShowDialog() == DialogResult.OK)
            {
                //καινουργια επιλογη για αγαπημενο τραγουδι
                FavoredSongs.Add(0);
                // Αν ο χρηστης πατησει Ανοιγμα / Open, το ονομα του τραγουδιου αποθηκευεται
                // και δημιουργειται καινουργιο panel , picturebox , richtextbox με την κλαση NewSong
                N1 = new NewSong(openFD.FileName,this,false);
                N1.Paneling();
                NumberofSongs++;
            }
        }

        //notStartTwice : δεν θελουμε να επιτρεπεται το "πατημα" του κουμπιου Play πανω απο μια φορα
        // καθως ξανα αναπαραγει το τραουδι απο την αρχη σε καποιες περιπτωσεις.
        //StopPauseSynergy : Θελουμε να μπορει ο χρηστης να πατησει stop στο τραγουδι οσο ειναι σε κατασταση pause

        private void Playb_Click(object sender, EventArgs e)
        {
            if(!(Song == 0)&&(notStartTwice == false))
            {
                //αναλογα με τον αριθμο τραγουδιου παιζει το καταλληλο τραγουδι
                wplayer.URL = "Music" + Song + ".mp3";
                wplayer.controls.currentPosition = pausedmoment;
                wplayer.controls.play();
                notStartTwice = true;
                StopPauseSynergy = false;
            }
        }

        private void Stopb_Click(object sender, EventArgs e)
        {
            if(notStartTwice == true || StopPauseSynergy == true) // απο pause θελουμε να πηγαινουμε σε stop
            {
                pausedmoment = 0;
                wplayer.controls.stop();
                notStartTwice = false;
                StopPauseSynergy = true;
            }
        }

        private void Pauseb_Click(object sender, EventArgs e)
        {
            if (notStartTwice == true || StopPauseSynergy == true)
            {
                //οταν γινει pause του τραγουδιου, κραταμε το μερος στο οποιο σταματησε, για να 
                // συνεχισει απο αυτο το σημειο η αναπαραγωγη πατωντας το κομπι Play
                pausedmoment = wplayer.controls.currentPosition;
                wplayer.controls.pause();
                notStartTwice = false;
                StopPauseSynergy = true;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (!(Song == 0))
            {
                wplayer.controls.stop();
                notStartTwice = false;
                StopPauseSynergy = true;

                //προσθετουμε στη λιστα με τα διαγραμμενα τραγουδια το τραγουδι προσ διαγραφη
                ExistingSongsDelete.Add(Song.ToString());

                //διαγραφω τις πληροφοριες του τραγουδιου και το τραγουδι
                //(στην ουσια τα μεταφερω σε εναν φακελο που τα υπαρχοντα του διαγραφονται οταν κλεισει
                // η εφαρμογη)

                if(File.Exists("Music" + Song + ".jpg"))
                {
                    File.Move("Music" + Song + ".jpg", DebugPath + "\\Delete\\" + "Music" + Song + ".jpg");
                }
                File.Move("Music" + Song + ".txt", DebugPath + "\\Delete\\" + "Music" + Song + ".txt");
                File.Move("Music" + Song + ".mp3", DebugPath + "\\Delete\\" + "Music" + Song + ".mp3");


                //βρισκω το τραγουδι (panel) που θελω να διαγραψω
                //var : ο compiler καθοριζει τον καταλληλο τυπο για την μεταβλητη pan
                //με την μεθοδο Find βρισκει και βαζει σε πινακα ολα τα controls που τα ονοματα τους ταιριαζουν
                // με το "Panel"+Song.
                //true : για να συμπεριλαβει και τα υπο controls του control που θελουμε
                //να βρουμε π.χ. "Checkbox"+Song.
                //FirstOrDefault() : επιστρεφει το πρωτο matching control και τα υπο controls του ή 
                // null για types οπως string και 0 για types οπως int...
                // γνωριζουμε οτι το control που θελουμε ειναι Panel αρα μπορουμε να το κανουμε 
                // cast σαν Panel χωρις errors. Ειναι απαραιτητο διαφορετικα θα υπαρξει σφαλμα
                //καθιστοντας αδυνατη την αφαιρεση του τραγουδιου απο το flowpanellayout

                var pan = this.Controls.Find("Panel"+Song, true).FirstOrDefault() as Panel;
                flow1.Controls.Remove(pan);
                NumberofSongs--;

                // δεν μπορουμε να βαλουμε null οποτε βαζουμε 0, δεν προκειτε το διαγραμμενο τραγουδι να παρει 
                // ξανα ποντους
                int[] temp = FavoredSongs.ToArray();
                temp[Song-1] = 0;
                FavoredSongs = temp.ToList();

                //βαζουμε στη λιστα με τους αριθμους των διαγραμμενων τραγουδιων, το καινουργιο διαφραμμενο
                //τραγουδι
                DeletedNumbers.Add(Song);

                Song = 0;
                nameofsong.Text = " ";

            }
            else
            {
                MessageBox.Show("Select a song you wish to delete.");
            }
        }


        private void Edit_Click(object sender, EventArgs e)
        {
            if (!(Song == 0))
            {
                Form3 f3 = new Form3(Song,this);
                f3.Show();
                Hide();
            }
            else
            {
                MessageBox.Show("Select a song you wish to edit.");
            }
        }

        private void Favored_Click(object sender, EventArgs e)
        {
            int[] temp = FavoredSongs.ToArray();
            // κοιταω αν υπαρχει εστω και ενας ποντος σε ενα τραουδι
            foreach(int item in temp)
            {
                if(item != 0)
                {
                    Favor = true;
                    break;
                }
            }
           // ControlNumber = temp.ToList();
            if (NumberofSongs == 0)
            {
                MessageBox.Show("There are no songs yet.");
            }
            else if (Favor == false)
            {
                MessageBox.Show("There are no favored songs yet.");
            }
            else
            {
                //στο temp2 θα βαλουμε το καθε αριθμο καθε τραγουδιου για την παρακατω ταξινομηση
                int[] temp2 = new int[temp.Length];
                //οσα τραγουδια εχουν 0 Favor ειτε ειναι διαγραμμενα ειτε δεν προτειμουνται απο τον χρηστη
                for (int i = 0; i < temp.Length; i++)
                {
                    temp2[i] = i + 1;
                }
                //ταξινομηση αυξουσα με βαση τις τιμες προτιμησης τραγουδιου
                Array.Sort(temp, temp2); // temp = βαθμοι     temp2 = τραγουδια
                //φθινουσα ταξινομηση
                Array.Reverse(temp2);
                Array.Reverse(temp);
                listBox1.BeginUpdate();
                //εμφανιζει αγαπημενων τραγουδιων
                for (int j = 0; j < temp2.Length; j++)
                {
                    //δεν θελουμε να εμφανιζει τα διαγραμμενα ή αυτα που δεν του αρεσουν
                    if(temp[j] != 0)
                    {
                        //check αν εχει μονο τιλτο ή οχι και φτιαχνουμε αναλογα το τελικο αποτελεσμα
                        line1 = File.ReadLines("Music" + temp2[j] + ".txt").First();
                        line2 = File.ReadLines("Music" + temp2[j]+ ".txt").ElementAtOrDefault(1);
                        if (line2 == null)
                        {
                            listBox1.Items.Add((j + 1) + ") " + line1);
                        }
                        else
                        {
                            listBox1.Items.Add((j + 1) + ") " + line2 + " - " + line1);
                        }
                    }
                }
                //μεσω ενος κρυφου listbox γινεται τροποποιηση του αποτελεσματος και εμφανιζεται
                string s = "";
                foreach (var item in listBox1.Items)
                {
                    s += item.ToString() + Environment.NewLine;
                }
                MessageBox.Show(s);
                listBox1.Items.Clear();
                listBox1.EndUpdate();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //οταν κλεινει η εφαρμογη διαγραφονται τα αρχεια που μεταφερθηκαν στον φακελο Delete
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            int[] Favtemp = FavoredSongs.ToArray();

            for (int i = 1; i < ExistingSongsAdd.Count + 1; i++)// απο 1 εως μεγιστο αριθμο σε τιτλο τραγουδιων
            {
                //αν υπαρχει ακομα το τραγουδι και δεν ειναι διαγραμμενο
                if (ExistingSongsAdd.Contains(i.ToString()) && !(ExistingSongsDelete.Contains(i.ToString())))
                {
                    ExistingSongs += i.ToString() + " ";//σημειωνεται για να προστεθει σε αρχειο
                    ExistngFavors += Favtemp[i-1] + " ";//σημειωνεται ο αντιστοιχος βαθμος προτιμησης σε αλλο αρχειο
                }
            }
            File.WriteAllText("NumberSongs.txt", ExistingSongs);
            File.WriteAllText("FavorPoints.txt", ExistngFavors);
        }

        private void pauseStripMenuItem2_Click(object sender, EventArgs e)
        {
            Pauseb.PerformClick();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Playb.PerformClick();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopb.PerformClick();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add.PerformClick();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Edit.PerformClick();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete.PerformClick();
        }

        private void favoredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Favored.PerformClick();
        }

        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomsong.PerformClick();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Randomsong_Click(object sender, EventArgs e)
        {
            if (!(NumberofSongs == 0))
            {
                wplayer.controls.stop();
                notStartTwice = false;
                StopPauseSynergy = true;

                //βαζουμε σε μια μεταβλητη ολα τα τραγουδια
                var songs = Directory.GetFiles(DebugPath, "*.mp3");

                // βαζουμε στη λιστα RandomNumber τυχαιους αριθμους απο το 1 εως το μεγιστο μηκος του πινακα 
                // songs +  1(διοτι π.χ random.Next(1,3) = 1 εως 2) + τον αριθμο των διαγραμμενων τραγουδιων
                // καθως συνολικα φτανει τον μεγιστο αριθμο των υπαρχοντων τραγουδιων,
                //ελεγχοντας καθε φορα να μην μπει καποιος αριθμος που εχει ηδη μπει, ή διαγραφει.
                // αυτο γινεται για να φτιαξουμε μια τυχαια σειρα αναπαραγωγης τραγουδιων
                // αναλογα τον αριθμο. 
                do
                {
                    rand = random.Next(1, songs.Length + DeletedNumbers.Count + 1);
                    if ((!RandomNumbers.Contains(rand)) && (!DeletedNumbers.Contains(rand)))
                    {
                        RandomNumbers.Add(rand);
                    }
                } while (RandomNumbers.Count < songs.Length);

                Form2 f2 = new Form2(RandomNumbers,this);
                f2.Show();
                Hide();
                RandomNumbers.Clear();

                
            }
        }

    }
    class NewSong : Form1
    {
        static List<string> ControlNumber = new List<string>(); // για να μην γινεται reset της λιστας
        int ControlIndex;
        private string SongName;
        Form1 f1;
        string line1, line2;
        bool Move;
        public NewSong(string name,Form1 form,bool booly)
        {
            SongName = name;
            f1 = form;
            Move = booly;
        }

        public void Paneling()
        {

            Panel panel = new Panel();

            RichTextBox richy = new RichTextBox();

            CheckBox song = new CheckBox();

            //η λιστα ControlNumber κραταει ποσα τραγουδια υπαρχουν ηδη , και αναλογα με τον αριθμο
            // ονομαζει τα καινουργια τραγουδια (ειναι ControlIndex +1 διοτι αρχιζει απο 0 η λιστα ενω 
            // τα controls απο 1)
            ControlIndex = 0;

            // "ετοιμαζουμε" το επομενο διαθεσιμο αριθμο για τα controls βαζοντας στο κελι του αριθμου
            // null
            ControlNumber.Add(null);

            string[] temp = ControlNumber.ToArray();
            while (true)
            {
                if (temp[ControlIndex] == null)
                {
                    panel.Name = "Panel" + (ControlIndex + 1);
                    song.Name = "Checkbox" + (ControlIndex + 1);
                    richy.Name = "Richtextbox" + (ControlIndex + 1);
                    temp[ControlIndex] = "not null";
                    ControlNumber = temp.ToList();

                    if(Move == false)
                    {
                        File.Copy(SongName, "Music" + (ControlIndex + 1) + ".mp3");
                        // φτιαχνς φακελο για το καθε τραγουδι με αρχικη πληροφορια τον τιλτο
                        File.WriteAllText("Music" + (ControlIndex + 1) + ".txt", Path.GetFileNameWithoutExtension(SongName));
                    }
                    else
                    {
                        //γινεται οταν καλειται απο το form1 load
                        //γινεται ανανεωση των αριθμων των τραγουδιων και των αντιστοιχων απαραιτητων αρχειων
                        File.Move(SongName, "Music" + (ControlIndex + 1) + ".mp3");

                        if(File.Exists(Path.GetFileNameWithoutExtension(SongName) + ".txt"))
                        {
                            // π.χ Path.GetFileNameWithoutExtension(SongName) + ".txt" = Music3 + .txt
                            File.Move(Path.GetFileNameWithoutExtension(SongName) + ".txt", "Music" + (ControlIndex + 1) + ".txt");
                            File.WriteAllText("Music" + (ControlIndex + 1) + ".txt", f1.ExistString);
                        }
                        if (File.Exists(Path.GetFileNameWithoutExtension(SongName) + ".jpg"))
                        {
                            File.Move(Path.GetFileNameWithoutExtension(SongName) + ".jpg", "Music" + (ControlIndex + 1) + ".jpg");
                        }
                    }
                    break;
                }
                ControlIndex++;
            }
            // προσθετουμε στη λιστα το τραγουδι που υπαρχει ηδη
            f1.ExistingSongsAdd.Add((ControlIndex+1).ToString());

            //τα τροποποιω εδω διοτι χρειαζεται το αρχειο για το event Click
            panel.Location = new Point(3, 120);
            panel.Size = new Size(284, 384);

            if(Move == false)
            {
                song.BackgroundImage = Image.FromFile("UnknownSong.jpg");
            }
            else
            {
                song.BackgroundImage = Image.FromFile("Music" + (ControlIndex + 1) + ".jpg");
            }
            song.BackgroundImageLayout = ImageLayout.Stretch;
            song.Appearance = Appearance.Button;
            song.FlatStyle = FlatStyle.Popup;
            song.AutoSize = false;
            song.Location = new Point(3, 120);
            song.Size = new Size(185, 170);
            //βαζω event για καθε τραγουδι
            song.Click += (sender, e) =>
            {
                //  ο τιτλος του πατημενου τραγουδιου θα εμφανιστει στο label nameofsong
                if (song.Checked == true)
                {
                    //παιρνει την πρωτη γραμμη του αρχειου (καθε γραμμη = κελι ενος πινακα)
                    line1 = File.ReadLines("Music" + (ControlIndex + 1) + ".txt").First();
                    // παιρνη την δευτερη γραμμη του αρχειου
                    line2 = File.ReadLines("Music" + (ControlIndex + 1) + ".txt").ElementAtOrDefault(1);
                    //αν υπαρχει μονο ο τιτλος του τραγουδιου θα εμφανιστει μονος του
                    //διαφορετικα θα εμφανιστει το ονομα του καλλιτεχνη - τιλτος 
                    if(line2 == null)
                    {
                        f1.nameofsong.Text = line1;
                    }
                    else
                    {
                        f1.nameofsong.Text = line2 + " - " + line1;
                    }
                    // μεταβλητη για να ξερουμε για ποιο τραγουδι προκειτε για αλλες λειτουργιες
                    f1.Song = ControlIndex + 1;

                    //καθε φορα που επιλεγεται το τραγουδι θα παιρνει 1 ποντο αγαπημενου τραγουδιου
                    int[] temp2 = f1.FavoredSongs.ToArray();
                    temp2[ControlIndex]++;
                    f1.FavoredSongs = temp2.ToList();
                }
            };
            //Βαζουμε την εικονα (checkbox) του τραγουδιου
            panel.Controls.Add(song);

            //Βαζουμε το τραγουδι (panel) στην λιστα
            f1.flow1.Controls.Add(panel);

            richy.Location = new Point(3, 291);
            richy.Size = new Size(185, 80);
            richy.ReadOnly = true;
            richy.Font = new Font("Calibri", 10, FontStyle.Bold);

            if(Move == false)
            {
                //αν ειναι καινουργιο τραγουδι απλως σημειωνεται ο τιτλος
                richy.Text = Path.GetFileNameWithoutExtension(SongName);
            }
            else
            {
                //αλλιως σημειωνεται ο τιτρλος και/ή ο καλλιτεχνης συμφωνα με το αρχειο που σωθηκε
                line1 = File.ReadLines("Music" + (ControlIndex + 1) + ".txt").First();
                line2 = File.ReadLines("Music" + (ControlIndex + 1) + ".txt").ElementAtOrDefault(1);
                richy.Text = f1.ExistString;
            }
            richy.BackColor = Color.Thistle;
            richy.BorderStyle = BorderStyle.None;
            //Βαζουμε τις πληροφοριες (richtextbox) του τραγουδιου
            panel.Controls.Add(richy);

        }
    }
}
