﻿using System;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core c;
        public MainWindow(Core c)
        {
            this.c = c;
            InitializeComponent();
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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
