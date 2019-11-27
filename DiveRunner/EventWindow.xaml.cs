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
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window
    {
        public Event createdEvent;
        public List<Diver> divers;
        private string dirname;
        public EventWindow(string dirname)
        {
            this.dirname = dirname;
            divers = new List<Diver>();
            InitializeComponent();
            BoardSelection.Items.Add("1M");
            BoardSelection.Items.Add("3M");
        }

        public EventWindow(Event e)
        {
            divers = new List<Diver>(e.divers);
            InitializeComponent();
            foreach (Diver d in divers)
            {
                DiverList.Items.Add(d);
            }
            BoardSelection.Items.Add(e.Board);
            BoardSelection.SelectedIndex = 0;
            EventNameBox.Text = e.name;
            SetButton_Click(null, null);
        }


        private void NewDiverButton_Click(object sender, RoutedEventArgs e)
        {
            DiverWindow dw = new DiverWindow(createdEvent.Board);
            dw.ShowDialog();
            Diver d = dw.diver;
            DiverList.Items.Add(d);
            divers.Add(d);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            createdEvent = new Event(EventNameBox.Text, BoardSelection.Text, dirname);
            foreach (Diver d in divers)
            {
                createdEvent.AddDiver(d);
            }
            this.Close();
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            createdEvent = new Event(EventNameBox.Text, BoardSelection.SelectionBoxItem.ToString(), dirname);
            EventNameBox.IsEnabled = false;
            BoardSelection.IsEnabled = false;
            SetButton.IsEnabled = false;
            NewDiverButton.IsEnabled = true;
            EditDiverButton.IsEnabled = true;
            RemoveDiverButton.IsEnabled = true;
            DuplicateDiverButton.IsEnabled = true;
        }

        private void RemoveDiverButton_Click(object sender, RoutedEventArgs e)
        {
            int index = DiverList.SelectedIndex;
            if (index == -1) return;
            divers.RemoveAt(index);
            DiverList.Items.RemoveAt(index);
        }

        private void EditDiverButton_Click(object sender, RoutedEventArgs e)
        {
            int index = DiverList.SelectedIndex;
            if (index == -1) return;
            Diver toEdit = divers[index];
            DiverWindow dw = new DiverWindow(toEdit);
            dw.ShowDialog();
            Diver newDiver = dw.diver;
            divers[index] = newDiver;
            DiverList.Items[index] = newDiver;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            createdEvent = new Event(EventNameBox.Text, BoardSelection.Text, dirname);
            foreach (Diver d in divers)
            {
                createdEvent.AddDiver(d);
            }
        }

        private void DuplicateDiverButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < DupNumberBox.Value; i++)
            {
                Diver DiverToCopy;
                if (DiverList.SelectedIndex != -1)
                {
                    DiverToCopy = divers[DiverList.SelectedIndex];
                }
                else
                {
                    if (divers.Count == 0) return;
                    DiverToCopy = divers[0];
                }
                Diver dupDiver = new Diver(DiverToCopy.Name + " Copy " + (i+1), DiverToCopy.Board, DiverToCopy.Dives);
                divers.Add(dupDiver);
                DiverList.Items.Add(dupDiver);
            }
        }
    }
}
