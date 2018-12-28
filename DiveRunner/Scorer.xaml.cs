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

namespace DiveRunner
{
    /// <summary>
    /// Interaction logic for Scorer.xaml
    /// </summary>
    public partial class Scorer : Window
    {
        public Core c;
        public int judgeCount;
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
            CoreState cs = c.State();
            if (cs.remainingDives == 0)
            {
                MessageBox.Show("Meet is Finished!", "Meet Finished", MessageBoxButton.OK);
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
            if (judgeCount == 1)
            {
                double judgeScore = Double.Parse(JudgeOneScoreBox.Text);
                double[] scores = new[] {judgeScore, judgeScore, judgeScore};
                c.NextScore(scores);
            }
            else if (judgeCount == 2)
            {
                double judge1 = Double.Parse(JudgeOneScoreBox.Text);
                double judge2 = Double.Parse(JudgeTwoScoreBox.Text);
                double[] scores = new[] { judge1, judge2, (judge2 + judge1)/2};
                c.NextScore(scores);
            }
            else
            {
                double judge1 = Double.Parse(JudgeOneScoreBox.Text);
                double judge2 = Double.Parse(JudgeTwoScoreBox.Text);
                double judge3 = Double.Parse(JudgeThreeScoreBox.Text);
                double[] scores = new[] { judge1, judge2, judge3 };
                c.NextScore(scores);
            }
            Update();
        }
    }
}
