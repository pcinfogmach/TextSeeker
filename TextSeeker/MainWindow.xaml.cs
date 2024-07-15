using sun.swing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using TextSeeker.Helpers;
using TextSeeker.SearchModels;

namespace TextSeeker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void ClearSettings() 
        {
            Properties.Settings.Default.CheckedFileNodes.Clear();
            Properties.Settings.Default.RootFolderNodes.Clear();
            Properties.Settings.Default.Save();
        }

    }
}
