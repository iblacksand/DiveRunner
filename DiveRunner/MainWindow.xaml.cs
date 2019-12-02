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
        private string NameOfFile;
        private List<string> listoffiles = new List<string>();
        public MainWindow(Core c, string f)
        {
            this.c = c;
            listoffiles.Add(f);
            InitializeComponent();
            JudgeCounter.Text = "1";
            foreach (Event cEvent in c.events)
            {
                EventListView.Items.Add(cEvent);
            }
            this.NameOfFile = f;
            FileList.Items.Add(f.Split('/')[f.Split('/').Length - 1]);
            FileList.Items.Add("Add new file...");
            FileList.Items.Add("Add old file...");
        }

        public MainWindow(Core c, string f, List<string> AllFiles)
        {
            this.c = c;
            InitializeComponent();
            JudgeCounter.Text = "1";
            foreach (Event cEvent in c.events)
            {
                EventListView.Items.Add(cEvent);
            }
            listoffiles = AllFiles;
            this.NameOfFile = f;
            foreach (string p in AllFiles) FileList.Items.Add(p.Split('\\')[p.Split('\\').Length - 1]);
            FileList.Items.Add("Add new file...");
            FileList.Items.Add("Add old file...");
        }

        private void NewEvent(object sender, RoutedEventArgs e)
        {
            EventWindow eventWindow = new EventWindow(c.PDFFolderName);
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
            String oc = JsonConvert.SerializeObject(c);
            string f = sfd.FileName;
            c.createdDirectory = false;
            c.FileName = f.Split('\\').Last();
            c.PDFFolderName = Environment.CurrentDirectory + "/pdfs/" + f.Split('\\').Last().Replace(".json", "") + "/";
            string s = JsonConvert.SerializeObject(c);
            File.WriteAllText(f,s);
            c = JsonConvert.DeserializeObject<Core>(oc);
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

        private void Save_Load_Button_Click(object sender, RoutedEventArgs e)
        {
            // Saving Core
            string s = JsonConvert.SerializeObject(c);
            File.WriteAllText(NameOfFile, s);
            if(FileList.SelectedIndex == listoffiles.Count)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "JSON Files | *.json";
                sfd.ShowDialog();
                string f = sfd.FileName;
                Core nc = new Core(f);
                listoffiles.Add(f);
                MainWindow nmw = new MainWindow(nc, f, listoffiles);
                this.Hide();
                nmw.ShowDialog();
                this.Close();
            }
            else if (FileList.SelectedIndex > listoffiles.Count)
            {
                OpenFileDialog sfd = new OpenFileDialog();
                sfd.Filter = "JSON Files | *.json";
                sfd.ShowDialog();
                string f = sfd.FileName;
                Core nc = JsonConvert.DeserializeObject<Core>(File.ReadAllText(f));
                listoffiles.Add(f);
                MainWindow nmw = new MainWindow(nc, f, listoffiles);
                this.Hide();
                nmw.ShowDialog();
                this.Close();
            }
            else if(FileList.SelectedIndex > -1)
            {
                string f = (string)FileList.Items[FileList.SelectedIndex];
                Core nc = JsonConvert.DeserializeObject<Core>(File.ReadAllText(f));
                MainWindow nmw = new MainWindow(nc, f, listoffiles);
                this.Hide();
                nmw.ShowDialog();
                this.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string s = JsonConvert.SerializeObject(c);
            File.WriteAllText(NameOfFile, s);
        }
    }
}
