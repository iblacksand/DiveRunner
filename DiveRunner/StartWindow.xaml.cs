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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DiveRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileD = new OpenFileDialog();
            fileD.Filter = "JSON Files | *.json";
            fileD.ShowDialog();
            string f = fileD.FileName;
            Core c = JsonConvert.DeserializeObject<Core>(File.ReadAllText(f));
            ShowMainWindow(c,f);
        }

        private void NewMeetButton_Click(object sender, RoutedEventArgs e)
        {
            Core c = new Core();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON Files | *.json";
            sfd.ShowDialog();
            string f = sfd.FileName;
            ShowMainWindow(c, f);
        }

        private void ShowMainWindow(Core c, string f)
        {
            MainWindow mw = new MainWindow(c , f);
            this.Hide();
            mw.ShowDialog();
            this.Close();
        }

        private void Testbutton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}

