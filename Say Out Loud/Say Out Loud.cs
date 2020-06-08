using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace Say_Out_Loud
{
    public partial class SayOutLoud : Form
    {
        Panel panel1 = new Panel();
        Panel panel2 = new Panel();
        public Label rtb = new Label();
        public Label score = new Label();
        public int sc = 0;
        public Button speak = new Button();
        public int[] a = new int[27];
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
        Grammar dictationGrammar = new DictationGrammar();
        Label t = new Label();
        int duration = 60, d = 10;
        string[] list = Directory.GetFiles(System.Reflection.Assembly.GetExecutingAssembly()
                   .Location + @"\..\..\Resources");
        PictureBox[] picturebox = new PictureBox[26];
        public Timer MyTimer = new Timer();
        public Timer stimer = new Timer();
        public List<string> words = new List<string>();
        public SayOutLoud()
        {
            InitializeComponent();

            panel1.Location = new Point(0, 0);
            panel1.Size = new Size(800, 500);

            Button play = new Button();
            play.Text = "PLAY";
            play.Location = new System.Drawing.Point(300, 220);
            play.Size = new System.Drawing.Size(185, 45);
            play.Click += play_click;

            Button quit = new Button();
            quit.Text = "QUIT";
            quit.Location = new System.Drawing.Point(300, 380);
            quit.Size = new System.Drawing.Size(185, 45);
            quit.Click += quit_click;

            Button help = new Button();
            help.Text = "HELP";
            help.Location = new System.Drawing.Point(300, 300);
            help.Size = new System.Drawing.Size(185, 45);
            help.Click += help_click;

            panel1.Paint += panel1_Paint;
            panel1.Controls.Add(help);
            panel1.Controls.Add(play);
            panel1.Controls.Add(quit);
            this.Controls.Add(panel1);

            panel2.Location = new Point(0, 0);
            panel2.Size = new Size(800, 500);
            panel2.BackColor = Color.AliceBlue;

            rtb.BackColor = System.Drawing.Color.White;
            rtb.Location = new Point(100, 200);
            rtb.Size = new Size(600, 170);

            speak.Size = new Size(185, 45);
            speak.Location = new Point(300, 400);
            speak.Text = "CLICK TO SPEAK";
            speak.Click += speak_click;

            t.Location = new Point(750, 25);
            t.Size = new Size(50, 50);
            MyTimer.Interval = (1000);
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            
            score.Size = new Size(50, 25);
            score.Location = new Point(0, 25);
            score.Text = sc.ToString();

            panel2.Controls.Add(score);
            panel2.Controls.Add(rtb);
            panel2.Controls.Add(speak);
            panel2.Controls.Add(t);
            this.Controls.Add(panel2);
            this.MinimumSize = new Size(800, 500);
            this.MaximumSize = new Size(800, 500);

            this.CenterToScreen();
            initialize();
        }
        void panel1_Paint(object sender, PaintEventArgs e)
        {
            Image imag = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"\bg\hhh.png");
            e.Graphics.DrawImage(imag, new Point(0, 0));
        }
        void quit_click(object sender, EventArgs e)
        {
            this.Close();
        }
        void play_click(object sender, EventArgs e)
        {
            MyTimer.Enabled = true;
            duration = 60;
            MyTimer.Start();
            panel2.BringToFront();
        }
        void help_click(object sender, EventArgs e)
        {
            string message = "Form as much words as you can, cause you only have 60 seconds․ Click the 'CLICK TO SPEAK' button and say your word out loud. If the computer does not recognize what you are saying just say 'spell it' and spell your word (here you have only 25 seconds). If you spell a wrong letter just say 'delete' and the letter will be deleted. After spelling your word click OK.";
            string title = "Instructions";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, title, buttons);
        }
        private void speak_click(object sender, EventArgs e)
        {
                speak.Text = "SPEAK NOW";
                recognizer.LoadGrammar(dictationGrammar);
                try
                {
                    recognizer.SetInputToDefaultAudioDevice();
                    RecognitionResult result = recognizer.Recognize();
                    if (result == null) return;
                    String s1;
                    s1 = result.Text;
                Debug.WriteLine(s1);
                if (s1 == "spell it" || s1 == "Spell it")
                    {
                        stimer.Start();
                        MyTimer.Stop();
                        SpellIt spellit = new SpellIt(this);
                        spellit.ShowDialog();
                    }
                    string firstWord = s1.Split(' ').First();
                    firstWord = firstWord.ToLower();
                    if (is_valid(a, firstWord) && (firstWord.Length > 1 || firstWord=="i"))
                    {
                        bool flag = true;
                        for (int l = 0; l < words.Count(); l++)
                        {
                            if (words[l] == firstWord)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            words.Add(firstWord);
                            rtb.Text += firstWord.ToLower();
                            sc += 10;
                            score.Text = sc.ToString();
                            int k = 50 - firstWord.Length;
                            if (words.Count() % 4 != 0) k -= firstWord.Length;
                            for (int i = 0; i < k; i++)
                                rtb.Text += ' ';
                        }
                    }
                    speak.Text = "CLICK TO SPEAK";
                }
                catch (InvalidOperationException exception)
                {
                    speak.Text = String.Format("Could not recognize input from default aduio device. Is a microphone or sound card available?\r\n{0} - {1}.", exception.Source, exception.Message);
                }
                finally
                {
                    recognizer.UnloadAllGrammars();
                }
                speak.Text = "CLICK TO SPEAK";
        }
        public bool is_valid(int[] a, string s)
        {
            int[] b = new int[27];

            for (int i = 0; i < 26; i++)
                b[i] = a[i];
            for (int i = 0; i < s.Length; i++)
            {
                if ((s[i] - 'a') >= 0 && (s[i] - 'a') <= 26) if (b[s[i] - 'a'] > 0) b[s[i] - 'a']--;
                    else return false;
                if (s[i] >= '0' && s[i] <= '9') return false;
            }
            return true;
        }
        private void MyTimer_Tick(object sender, EventArgs e)
            {
            if (duration == 0)
            {
                t.Text = "0";
                MyTimer.Stop();
                string message = "Do you want to play again?";
                string title = "Time is up!!!";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    duration = 60;
                    remove();
                    initialize();
                    MyTimer.Start();
                    rtb.ResetText();
                    sc = 0;
                    score.Text = sc.ToString();
                    speak.Text = "CLICK TO SPEAK";
                }
                else
                {
                    panel1.BringToFront();
                    rtb.ResetText();
                    sc = 0;
                    score.Text = sc.ToString();
                    
                }
            }
            t.Text = duration.ToString();
            duration--;
        }
        public void remove()
        {
            for (int i = 0; i < 13; i++)
            {
                panel2.Controls.Remove(picturebox[i]);
            }
        }
        public void initialize()
        {
            for (int i = 0; i <= 26; i++)
                a[i] = 0;
            Random rnd = new Random();
            int k = 0, j = 25, p = 0;
            while (k != 5)
            {
                int r = rnd.Next(1, 6);
                if (r == 1)
                {
                    if (a[0] < 2)
                    {
                        picturebox[k] = new PictureBox();
                        picturebox[k].Size = new Size(50, 50);
                        picturebox[k].Image = Image.FromFile(list[0]);
                        picturebox[k].Location = new Point(j, 50);
                        panel2.Controls.Add(picturebox[k]);
                        a[0]++;
                        k++;
                        j += 55;
                    }
                }
                else if (r == 2)
                {
                    if (a[4] < 2)
                    {
                        picturebox[k] = new PictureBox();
                        picturebox[k].Size = new Size(50, 50);
                        picturebox[k].Image = Image.FromFile(list[4]);
                        picturebox[k].Location = new Point(j, 50);
                        panel2.Controls.Add(picturebox[k]);
                        a[4]++;
                        k++;
                        j += 55;
                    }
                }
                else if (r == 3)
                {
                    if (a[8] < 2)
                    {
                        picturebox[k] = new PictureBox();
                        picturebox[k].Size = new Size(50, 50);
                        picturebox[k].Image = Image.FromFile(list[8]);
                        picturebox[k].Location = new Point(j, 50);
                        panel2.Controls.Add(picturebox[k]);
                        a[8]++;
                        k++;
                        j += 55;
                    }
                }
                else if (r == 4)
                {
                    if (a[14] < 1)
                    {
                        picturebox[k] = new PictureBox();
                        picturebox[k].Size = new Size(50, 50);
                        picturebox[k].Image = Image.FromFile(list[14]);
                        picturebox[k].Location = new Point(j, 50);
                        panel2.Controls.Add(picturebox[k]);
                        a[14]++;
                        k++;
                        j += 55;
                    }
                }
                else if (r == 5)
                {
                    if (a[20] < 1)
                    {
                        picturebox[k] = new PictureBox();
                        picturebox[k].Size = new Size(50, 50);
                        picturebox[k].Image = Image.FromFile(list[20]);
                        picturebox[k].Location = new Point(j, 50);
                        panel2.Controls.Add(picturebox[k]);
                        a[20]++;
                        k++;
                        j += 55;
                    }
                }
            }
            while (k != 13)
            {
                int r = rnd.Next(1, 27);
                if (r != 1 && r != 5 && r != 9 && r != 15 && r != 21 && a[r - 1] < 1)
                {
                    picturebox[k] = new PictureBox();
                    picturebox[k].Size = new Size(50, 50);
                    picturebox[k].Image = Image.FromFile(list[r - 1]);
                    picturebox[k].Location = new Point(j, 50);
                    panel2.Controls.Add(picturebox[k]);
                    a[r - 1]++;
                    k++;
                    j += 55;
                }
            }
        }
    }
}
