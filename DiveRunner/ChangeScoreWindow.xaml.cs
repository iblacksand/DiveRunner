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
    /// Interaction logic for ChangeScoreWindow.xaml
    /// </summary>
    public partial class ChangeScoreWindow : Window
    {
        public double[] newScores;
        public ChangeScoreWindow(string name, Dive dive, double[] scores)
        {
            InitializeComponent();
            DiverNameBlock.Text = name;
            DiveCodeBlock.Text = "Scores for : " + dive.Code + " - " + dive.Description;
            JudgeOneScoreBox.Text = "" + scores[0];
            JudgeTwoScoreBox.Text = "" + scores[1];
            JudgeThreeScoreBox.Text = "" + scores[2];
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            newScores = new double[3];
            newScores[0] = Double.Parse(JudgeOneScoreBox.Text.Trim());
            newScores[1] = Double.Parse(JudgeTwoScoreBox.Text.Trim());
            newScores[2] = Double.Parse(JudgeThreeScoreBox.Text.Trim());
        }
    }
}
