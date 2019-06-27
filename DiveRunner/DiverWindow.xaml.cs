using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for DiverWindow.xaml
    /// </summary>
    public partial class DiverWindow : Window
    {
        public Diver diver;
        private List<Dive> diveList;
        private List<Dive> dives;
        private String Board;
        private List<List<double>> scores;
        private bool hasScores;
        public DiverWindow(string Board)
        {
            dives = new List<Dive>();
            this.Board = Board;
            diveList = new List<Dive>();
            String[] divelistb = File.ReadAllText("./divelist.csv").Split('\n');
            for (int i = 1; i < divelistb.Length; i++) dives.Add(new Dive(divelistb[i]));
            InitializeComponent();
            hasScores = false;
            ChangeScoresButton.IsEnabled = false;
        }

        public DiverWindow(Diver toEdit)
        {
            dives = new List<Dive>();
            String[] divelistb = File.ReadAllText("./divelist.csv").Split('\n');
            for (int i = 1; i < divelistb.Length; i++) dives.Add(new Dive(divelistb[i]));
            InitializeComponent();
            this.diver = toEdit;
            this.Board = toEdit.Board;
            diveList = new List<Dive>(toEdit.Dives);
            foreach (Dive d in toEdit.Dives) DiveListBox.Items.Add(d);
            DiverNameBox.Text = toEdit.Name;
            if (diver.Scores.Count == 0)
            {
                ChangeScoresButton.IsEnabled = false;
            }
            else
            {
                hasScores = true;
                this.scores = toEdit.Scores;
            }
        }

        private void NewDiveButton_Click(object sender, RoutedEventArgs e)
        {
            Dive toAdd = GetDive(DiveCodeText.Text, Board);
            diveList.Add(toAdd);
            DiveListBox.Items.Add(toAdd);
        }

        /// <summary>
        /// Search for the dive that has the given dive code and boardHeight
        /// </summary>
        /// <param name="diveCode">The code of the dive. Capitalization doesn't matter.</param>
        /// <param name="boardHeight">The height of the board in meters. eg: 1m</param>
        /// <returns>A Dive object that corresponds to the query or null if not found.</returns>
        public Dive GetDive(string diveCode, string boardHeight)
        {
            Dive d = dives.FirstOrDefault(x => x.Code == diveCode.ToUpper() && x.Board == boardHeight.ToUpper());
            if (d == null) d = new Dive();
            return d;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DiveListBox.SelectedIndex == -1)
            {
                return;
            }
            Dive toRemove = (Dive) (DiveListBox.SelectedItem);
            int index = diveList.FindIndex(x => x.Code == toRemove.Code);
            diveList.RemoveAt(index);
            DiveListBox.Items.RemoveAt(index);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            diver = new Diver(DiverNameBox.Text, Board, diveList.ToArray());
            if (hasScores) diver.Scores = this.scores;
        }

        private void ChangeDiveButton_Click(object sender, RoutedEventArgs e)
        {
            int index = DiveListBox.SelectedIndex;
            InputBox ib = new InputBox("New Dive", "What is the new dive code?", diveList[index].Code);
            ib.ShowDialog();
            string newCode = ib.result;
            Dive newDive = GetDive(newCode, Board);
            diveList[index] = newDive;
            DiveListBox.Items[index] = newDive;
        }

        private void ChangeScoresButton_Click(object sender, RoutedEventArgs e)
        {
            int index = DiveListBox.SelectedIndex;
            ChangeScoreWindow csw = new ChangeScoreWindow(diver.Name, dives[index], scores[index].ToArray());
            csw.ShowDialog();
            double[] newScores = csw.newScores;
            scores[index] = new List<double>(newScores);
        }

        private void LookUpButton_Click(object sender, RoutedEventArgs e)
        {
            DiveSearch ds = new DiveSearch(dives.FindAll(o => o.Board == this.Board));
            ds.Show();
        }
    }
}
