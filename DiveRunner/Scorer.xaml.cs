using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DiveRunner
{
    /// <summary>
    /// Interaction logic for Scorer.xaml
    /// </summary>
    public partial class Scorer : Window
    {
        public Core c;
        public int judgeCount;
        private int timerTickCount = 0;
        private DispatcherTimer timer;
        public Scorer(Core c, int judgeCount)
        {
            this.c = c;
            this.judgeCount = judgeCount;
            InitializeComponent();
            if (judgeCount < 3)
            {
                JudgeThreeBlock.Visibility = Visibility.Hidden;
                JudgeThreeScoreBox.Visibility = Visibility.Hidden;
            }

            if (judgeCount < 2)
            {
                JudgeTwoBlock.Visibility = Visibility.Hidden;
                JudgeTwoScoreBox.Visibility = Visibility.Hidden;
            }
            c.Start();
            Update();
        }

        public void Update()
        {
            StartTimer();
            CoreState cs = c.State();
            if (cs.remainingDives == 0)
            {
                MessageBox.Show("Meet is Finished!", "Meet Finished", MessageBoxButton.OK, MessageBoxImage.Information);
                c.AutoSave();
                this.Close();
                return;
            }
            TotalDiveBlock.Text = "Total Dives: " + cs.totalDives;
            RemainingDiveBlock.Text = "Remaining Dives: " + cs.remainingDives;
            CompletedDiveBlock.Text = "Completed Dives: " + cs.completedDives;
            DiverNameBlock.Text = cs.curDiver.Name;
            EventNameBlock.Text = cs.curEvent.name + " - " + cs.curEvent.Board;
            Dive d = cs.curDiver.CurrentDive();
            DiveBlock.Text = d.Code + " | " + d.Description;
        }

        private void NextDiverButton_Click(object sender, RoutedEventArgs e)
        {
            NextDiverButton.IsEnabled = false;
            Double x;
            if (judgeCount == 1)
            {
                if (Double.TryParse(JudgeOneScoreBox.Text, out x))
                {
                    double judgeScore = Double.Parse(JudgeOneScoreBox.Text);
                    double[] scores = new[] { judgeScore, judgeScore, judgeScore };
                    c.NextScore(scores);
                    Update();
                }
            }
            else if (judgeCount == 2)
            {
                
                if (Double.TryParse(JudgeOneScoreBox.Text, out x) && Double.TryParse(JudgeTwoScoreBox.Text, out x))
                {
                    double judge1 = Double.Parse(JudgeOneScoreBox.Text);
                    double judge2 = Double.Parse(JudgeTwoScoreBox.Text);
                    double[] scores = new[] { judge1, judge2, (judge2 + judge1) / 2 };
                    c.NextScore(scores);
                    Update();
                }
            }
            else
            {
                if (Double.TryParse(JudgeOneScoreBox.Text, out x) && Double.TryParse(JudgeTwoScoreBox.Text, out x) && Double.TryParse(JudgeThreeScoreBox.Text, out x))
                {
                    double judge1 = Double.Parse(JudgeOneScoreBox.Text);
                    double judge2 = Double.Parse(JudgeTwoScoreBox.Text);
                    double judge3 = Double.Parse(JudgeThreeScoreBox.Text);
                    double[] scores = new[] { judge1, judge2, judge3 };
                    c.NextScore(scores);
                    Update();
                }
            }
        }

        public void StartTimer()
        {
            timer = new DispatcherTimer();
            timerTickCount = 0;
            timer.Interval = new TimeSpan(0, 0, 1); // will 'tick' once every second
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            if (timerTickCount == 3)
            {
                NextDiverButton.IsEnabled = true;
                timer.Stop();
            }
            timerTickCount++;
        }
    }
}
