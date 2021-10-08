using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace ThirdErgasia
{
    public partial class Form2 : Form // PlayList
    {
        List<int> PlayList = new List<int>();
        IWMPPlaylist playlist;
        private WindowsMediaPlayer wplayer = new WindowsMediaPlayer();
        string PlaylistName = "PlaylistRandom";
        string line1, line2;
        double pausedmoment;
        bool notStartTwice = true;
        bool StopPauseSynergy = false;
        Form1 f1;
        int MaxIndex,MinIndex,CurrentIndex,NumSongs;
        int FindIndex = 0;
        
        public Form2(List<int> Lista,Form1 form)
        {
            InitializeComponent();
            PlayList = Lista;
            f1 = form;
        }

        private void Nextb_Click(object sender, EventArgs e)
        {
            if (NumSongs == 1)
            {
                wplayer.controls.stop();
                wplayer.controls.play();
            }
            else
            {
                //σταματαει το τραγουδι που παιζει και γινεται επιλογη του επομενου στην λιστα
                wplayer.controls.stop();
                wplayer.controls.next();

                int[] temp = PlayList.ToArray();

                listBox1.BeginUpdate();
                listBox1.SelectedItem = Color.Turquoise; // το παλιο τραγουδι δεν ειναι πια highlighted
                                                         //αν ειμαστε στο τελος του playlist παμε στην αρχη
                if (CurrentIndex == MaxIndex)
                {
                    CurrentIndex = MinIndex;
                    FindIndex = 0;
                }
                else
                {
                    //αλλιως παμε στο επομενο κομματι
                    CurrentIndex++;
                    FindIndex++;
                }
                listBox1.SetSelected(CurrentIndex, true);
                listBox1.SelectedItem = Color.CornflowerBlue;
                listBox1.EndUpdate();

                //ανανεωση στοιχειων προς προβολη
                line1 = File.ReadLines("Music" + temp[FindIndex] + ".txt").First();
                line2 = File.ReadLines("Music" + temp[FindIndex] + ".txt").ElementAtOrDefault(1);
                if (line2 == null)
                {
                    label1.Text = line1;
                }
                else
                {
                    label1.Text = line2 + " - " + line1;
                }

                richTextBox1.Text = File.ReadAllText("Music" + temp[FindIndex] + ".txt");
                if (File.Exists("Music" + temp[FindIndex] + ".jpg"))
                {
                    pictureBox1.BackgroundImage = Image.FromFile("Music" + temp[FindIndex] + ".jpg");
                }
                else
                {
                    pictureBox1.BackgroundImage = Image.FromFile("UnknownSong.jpg");
                }

                //αναπαραγωγη επομενου τραγουδιου
                notStartTwice = true;
                wplayer.controls.play();
            }
        }

        private void Previousb_Click(object sender, EventArgs e)
        {
            foreach (int item in PlayList)
            {
                MessageBox.Show(item.ToString());
            }
            MessageBox.Show(PlayList.Count.ToString());
            if (NumSongs == 1)
            {
                wplayer.controls.stop();
                wplayer.controls.play();
            }
            else
            {
                wplayer.controls.stop();
                wplayer.controls.previous();

                int[] temp = PlayList.ToArray(); // για να βρουμε ποιο τραγουδι παίζει και τις πληροφορίες του

                listBox1.BeginUpdate();
                listBox1.SelectedItem = Color.Turquoise;
                //αν ειμαστε στην αρχη του playlist παμε στο τελος
                if (CurrentIndex == MinIndex)
                {
                    CurrentIndex = MaxIndex;
                    FindIndex = temp.Length - 1;
                }
                else
                {
                    //αλλιως παμε στο προηγουμενο κομματι
                    CurrentIndex--;
                    FindIndex--;
                }
                listBox1.SetSelected(CurrentIndex, true);
                listBox1.SelectedItem = Color.CornflowerBlue;
                listBox1.EndUpdate();

                line1 = File.ReadLines("Music"+ temp[FindIndex] + ".txt").First();
                line2 = File.ReadLines("Music" + temp[FindIndex] + ".txt").ElementAtOrDefault(1);
                if (line2 == null)
                {
                    label1.Text = line1;
                }
                else
                {
                    label1.Text = line2 + " - " + line1;
                }

                richTextBox1.Text = File.ReadAllText("Music" + temp[FindIndex] + ".txt");
                if (File.Exists("Music" + temp[FindIndex] + ".jpg"))
                {
                    pictureBox1.BackgroundImage = Image.FromFile("Music" + temp[FindIndex] + ".jpg");
                }
                else
                {
                    pictureBox1.BackgroundImage = Image.FromFile("UnknownSong.jpg");
                }

                notStartTwice = true;
                wplayer.controls.play();
            }
        }

        private void Stopb_Click(object sender, EventArgs e)
        {
            if (notStartTwice == true || StopPauseSynergy == true)
            {
                pausedmoment = 0;
                wplayer.controls.stop();
                notStartTwice = false;
                StopPauseSynergy = true;
            }
        }

        private void Playb_Click(object sender, EventArgs e)
        {
            if (notStartTwice == false)
            {
                wplayer.controls.currentPosition = pausedmoment;
                wplayer.controls.play();
                notStartTwice = true;
                StopPauseSynergy = false;
            }
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Backb.PerformClick();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Playb.PerformClick();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pauseb.PerformClick();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopb.PerformClick();
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Nextb.PerformClick();
        }

        private void previousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Previousb.PerformClick();
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

        private void Backb_Click(object sender, EventArgs e)
        {
            wplayer.controls.stop();
            Close();
            f1.Show();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            int[] temp = PlayList.ToArray();

            NumSongs = temp.Length;
            listBox1.BeginUpdate(); //προετοιμασια listbox για επεξεργασια
            listBox1.Items.Clear(); // καθαρισμα στοιχειων για να μπει καινουργια λιστα τραγουδιων

            playlist = wplayer.newPlaylist(PlaylistName, ""); // δημιουργω playlist χωρις metafile

            for (int k = 0; k < temp.Length; k++)
            {
                //αν υπαρχουν τα απαραιτητα αρχεια συνεχιζει αλλιως ελεγχουμε το επομενο τραγουδι
                if(File.Exists("Music" + temp[k] + ".txt") && File.Exists("Music" + temp[k] + ".mp3"))
                {
                    //βαζουμε στο playlist το τραγουδι
                    playlist.appendItem(wplayer.newMedia("Music" + temp[k] + ".mp3"));

                    //check αν εχει μονο τιτλο ή οχι 
                    line1 = File.ReadLines("Music" + temp[k] + ".txt").First();
                    line2 = File.ReadLines("Music" + temp[k] + ".txt").ElementAtOrDefault(1);
                    if (line2 == null)
                    {
                        listBox1.Items.Add((k + 1) + ") " + line1); 
                    }
                    else
                    {
                        listBox1.Items.Add((k + 1) + ") " + line2 + " - " + line1);
                    }
                    //αν ειναι το πρωτο τραγουδι, τοτε γινονται διαφορες αρχικοποιησεις
                    if (k == 0)
                    {
                        richTextBox1.Text = File.ReadAllText("Music" + temp[k] + ".txt");
                        //αν εχει δικια του εικονα, την βαζουμε, αλλιως την default
                        if (File.Exists("Music" + temp[k] + ".jpg"))
                        {
                            pictureBox1.BackgroundImage = Image.FromFile("Music" + temp[k] + ".jpg");
                        }
                        else
                        {
                            pictureBox1.BackgroundImage = Image.FromFile("UnknownSong.jpg");
                        }

                        if (line2 == null)
                        {
                            label1.Text = line1;
                        }
                        else
                        {
                            label1.Text = line2 + " - " + line1;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            //μεταβλητες για να βρουμε ποιο τραγουδι αναπαραγεται για να αλλαξουμε το χρωμα του στο listbox1
            //για highlight
            MinIndex = 0;
            CurrentIndex = 0;
            MaxIndex = temp.Length - 1;
            listBox1.SetSelected(0, true); // true : επιλογη συγκεκριμενου item (0)
            listBox1.SelectedItem = Color.CornflowerBlue;
            listBox1.EndUpdate();

            PlayList = temp.ToList(); // γιατι θελουμε να χρησιμοποιησουμε την PlayList ξανα πιο μετα

            wplayer.currentPlaylist = playlist;//καθορισμος playlist
            //εγγραφη του αντικειμενου wplayer στο παρακατω event
            wplayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
            wplayer.controls.play();
        }

        private void wplayer_PlayStateChange(int NewState)
        {
            //οταν τελειωσει να παιζει ενα τραγουδι , η κατασταση του θα γινει 8 (WMPPlayState.wmppsMediaEnded)
            // ειναι object αρα χρειαζεται cast για να το συγκρινουμε με το NewState
            if (NewState == (int)WMPPlayState.wmppsMediaEnded)
            {
                // πραγματοποιειται το event Click του κουμπιου Nextb
                Nextb.PerformClick();
                // για να συνεχισει στο επομενο τραγουδι του playlist
                wplayer.controls.next();
                wplayer.controls.play();
            }
        }
    }
}
