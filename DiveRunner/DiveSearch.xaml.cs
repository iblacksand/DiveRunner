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
    /// Interaction logic for DiveSearch.xaml
    /// </summary>
    public partial class DiveSearch : Window
    {
        public List<Dive> dives;
        public DiveSearch(List<Dive> dives)
        {
            InitializeComponent();
            this.dives = dives;
            foreach (Dive dive in dives)
            {
                DiveListBox.Items.Add(dive);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Fastenshtein.Levenshtein lev = new Fastenshtein.Levenshtein(SearchTermBox.Text);
            List<Dive> sorted = dives.OrderBy(o => lev.DistanceFrom(o.DiveData)).ToList();
            dives = sorted;
            DiveListBox.Items.Clear();
            foreach (Dive dive in dives)
            {
                DiveListBox.Items.Add(dive);
            }
        }
    }
}
