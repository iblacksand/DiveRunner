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
using Microsoft.Win32;
using Newtonsoft.Json;

namespace DiveRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core c;
        private int swapIndex;
        public MainWindow(Core c)
        {
            this.c = c;
            InitializeComponent();
            JudgeCounter.Text = "1";
            foreach (Event cEvent in c.events)
            {
                EventListView.Items.Add(cEvent);
            }
        }

        private void NewEvent(object sender, RoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow();
            eventWindow.ShowDialog();
            Event ev = eventWindow.createdEvent;
            c.AddEvent(ev);
            EventListView.Items.Add(ev);
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            int index = EventListView.SelectedIndex;
            if (index == -1) return;
            Event toEdit = c.events[index];
            EventWindow ew = new EventWindow(toEdit);
            ew.ShowDialog();
            Event newEvent = ew.createdEvent;
            c.events[index] = newEvent;
            EventListView.Items[index] = newEvent;
        }

        private void RemoveEventButton_Click(object sender, RoutedEventArgs e)
        {
            int index = EventListView.SelectedIndex;
            if (index == -1) return;
            c.events.RemoveAt(index);
            EventListView.Items.RemoveAt(index);
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON Files | *.json";
            sfd.ShowDialog();
            string f = sfd.FileName;
            string s = JsonConvert.SerializeObject(c);
            File.WriteAllText(f,s);
            //            c.AutoSave();
            //            c.GenerateDiveList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            c.AutoSave();
            System.Windows.Application.Current.Shutdown();
        }

        private void AnnouncersListButton_Click(object sender, RoutedEventArgs e)
        {
            c.GenerateDiveList();
        }

        private void ScorerButton_Click(object sender, RoutedEventArgs e)
        {
            int judges = Int32.Parse(JudgeCounter.Text);
            Scorer s = new Scorer(c, judges);
            s.ShowDialog();
            this.c = s.c;
        }

        private void DiveSheetButton_Click(object sender, RoutedEventArgs e)
        {
            c.GenerateReports();
        }

        private void EndSwapButton_Click(object sender, RoutedEventArgs e)
        {
            int secondSwap = EventListView.SelectedIndex;
            if (secondSwap == -1) return;
            Event ev = c.events[swapIndex];
            c.events[swapIndex] = c.events[secondSwap];
            c.events[secondSwap] = ev;
            StartSwapButton.Visibility = Visibility.Visible;
            EndSwapButton.Visibility = Visibility.Hidden;
            EventListView.Items.Clear();
            foreach (Event cEvent in c.events)
            {
                EventListView.Items.Add(cEvent);
            }
        }

        private void StartSwapButton_Click(object sender, RoutedEventArgs e)
        {
            swapIndex = EventListView.SelectedIndex;
            if (swapIndex == -1) return;
            StartSwapButton.Visibility = Visibility.Hidden;
            EndSwapButton.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            c.AutoSave();
        }
    }
}
