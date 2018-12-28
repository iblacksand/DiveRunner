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
        public EventWindow()
        {
            InitializeComponent();
            BoardSelection.Items.Add("1M");
            BoardSelection.Items.Add("3M");
        }

        private void NewDiverButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            createdEvent = new Event(EventNameBox.Text, BoardSelection.Text);
        }
    }
}
