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
        public DiverWindow(string Board)
        {
            dives = new List<Dive>();
            this.Board = Board;
            diveList = new List<Dive>();
            String[] divelistb = File.ReadAllText("./divelist.csv").Split('\n');
            for (int i = 1; i < divelistb.Length; i++) dives.Add(new Dive(divelistb[i]));
            InitializeComponent();
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
            Dive toRemove = (Dive) (DiveListBox.SelectedItem);
            int index = diveList.FindIndex(x => x.Code == toRemove.Code);
            diveList.RemoveAt(index);
            DiveListBox.Items.RemoveAt(index);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            diver = new Diver(DiverNameBox.Text, Board, diveList.ToArray());
            this.Close();
        }
    }
}
