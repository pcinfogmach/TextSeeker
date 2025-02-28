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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextSeeker.TextSeekerMVVM
{
    /// <summary>
    /// Interaction logic for TextSeekerView.xaml
    /// </summary>
    public partial class TextSeekerView : UserControl
    {
        public TextSeekerView()
        {
            InitializeComponent();
            Loaded += (s, e) => { SearchTextBox.Focus(); };
        }

        private void SearchTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var txtControl = sender as TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }
    }
}
