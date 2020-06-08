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
    public partial class SpellIt : Form
    {
        int k = 0;
        Label[] letters = new Label[15];
        SayOutLoud sol;
        SpeechSynthesizer speechsynth = new SpeechSynthesizer();
        public SpeechRecognitionEngine receng = new SpeechRecognitionEngine();
        Choices choice = new Choices();
        Timer stimer = new Timer();
        int d = 25;
        public SpellIt(SayOutLoud f)
        {
            InitializeComponent();
            this.sol = f;
            for (int i = 0; i < 13; i++)
            {
                letters[i] = new Label();
                letters[i].Location = new System.Drawing.Point(i * 25 + 65, 100);
                letters[i].Size = new Size(25, 25);
                letters[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                letters[i].BackColor = Color.LightSkyBlue;
                Controls.Add(letters[i]);
            }
            choice.Add(new string[] { "a", "b", "c", "d", "e", "f", "g", "h", " i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "delete" });
            Grammar gr = new Grammar(new GrammarBuilder(choice));
            try
            {
                receng.RequestRecognizerUpdate();
                receng.LoadGrammar(gr);
                receng.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(receng_SpeechRecognized);
                receng.SetInputToDefaultAudioDevice();
                receng.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            this.CenterToScreen();
            stimer.Interval = (1000);
            stimer.Tick += new EventHandler(stimer_Tick);
            stimer.Start();
        }
        private void stimer_Tick(object sender, EventArgs e)
        {
            if (d == 0)
            {
                stimer.Stop();
                sol.MyTimer.Start();
                this.Close();
            }
            d--;
        }
        public void OK_Click(object sender, EventArgs e)
        {
            sol.MyTimer.Start();
            receng.RecognizeAsyncStop();
            char[] l = new char[k];
            for (int k2 = 0; k2 < k; k2++)
            {
                l[k2] = letters[k2].Text[0];
            }
            string s2 = new string(l);
            Debug.WriteLine(s2.Length);
            var DICTIONARY_DIR = AppDomain.CurrentDomain.BaseDirectory + @"\.nuget\packages\netspell\2.1.7\dic";
            Debug.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            var usDictionary = new NetSpell.SpellChecker.Dictionary.WordDictionary()
            {
                DictionaryFolder = DICTIONARY_DIR,
                DictionaryFile = "en-US.dic"
            };
            usDictionary.Initialize();
            var usSpellChecker = new NetSpell.SpellChecker.Spelling
            {
                Dictionary = usDictionary
            };
            var correct = usSpellChecker.TestWord(s2);
            Debug.Write(correct);
            if (correct && sol.is_valid(sol.a, s2))
            {
                bool flag = true;
                for (int l1 = 0; l1 < sol.words.Count(); l1++)
                {
                    if (sol.words[l1] == s2)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    sol.words.Add(s2);
                    sol.rtb.Text += s2;
                    sol.sc += 10;
                    sol.score.Text = sol.sc.ToString();
                    int k = 50 - s2.Length;
                    for (int i = 0; i < k; i++)
                        sol.rtb.Text += ' ';
                }
            }

            for (int k1 = 0; k1 < 13; k1++)
                letters[k1].Text = " ";
            k = 0;
            this.Close();
        }
        public void receng_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if(e.Result.Text.ToString()=="delete" && k!=0)
            {
                letters[k - 1].Text = " ";
                k--;
            }
            if (e.Result.Text.ToString() != "delete") letters[k++].Text = e.Result.Text.ToString();
        }
    }
}
