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
            Core core = new Core();
            Event e1 = new Event("Sample Event 1");
            e1.AddDiver(new Diver("Patrick"));
            e1.AddDiver(new Diver("Alex"));
            core.AddEvent(e1);
            Event e2 = new Event("Sample Event 2");
            e2.AddDiver(new Diver("Alexa"));
            e2.AddDiver(new Diver("Jess"));
            core.AddEvent(e2);
            string s = JsonConvert.SerializeObject(core);
            File.WriteAllText("./sample.json", s);
        }

        private void NewMeetButton_Click(object sender, RoutedEventArgs e)
        {
            TestObject j = new TestObject("hello", "world");
            string s = JsonConvert.SerializeObject(j);
            File.WriteAllText("./sample.json", s);
        }
    }
    public class TestObject
    {
        public string name;
        public string test;

        public TestObject(string name, string test)
        {
            this.name = name;
            this.test = test;
        }
    }
}

