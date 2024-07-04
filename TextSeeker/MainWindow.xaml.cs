using sun.swing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using TextSeeker.Helpers;
using TextSeeker.SearchModels;
using TextSeeker.TreeModels;
using TextSeeker.ViewModels;

namespace TextSeeker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string currentSearchTerm;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.SaveCurrentRootNode();
        }

        private void AddTreeItemButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddNewNode(); 
        }

        private void treeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete) { viewModel.RemoveSelectedNode(); e.Handled = true; }
        }

        private void DeleteTreeItemButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveSelectedNode();
        }

        private  void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private  void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { Search(); e.Handled = true; }
        }

        async void Search()
        {
            currentSearchTerm = SearchTextBox.Text;
            await viewModel.SearchAsync();
        }

        private async void OnceOffSearchButton_Click(object sender, RoutedEventArgs e)
        {
            currentSearchTerm = SearchTextBox.Text;
            await viewModel.OnceOffSearchAsync();
        }

        private void FileItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem listViewItem && listViewItem.Tag is FileTreeNode node)
            {
                try { System.Diagnostics.Process.Start(node.Path); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                e.Handled = true;
            }
            else if (sender is TreeViewItem item && item.Tag is FileTreeNode treeNode)
            {
                try { System.Diagnostics.Process.Start(treeNode.Path); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                e.Handled = true;
            }
            
        }

        private void FileItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is ListViewItem listViewItem && listViewItem.Tag is FileTreeNode node)
                {
                    try { System.Diagnostics.Process.Start(node.Path); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    e.Handled = true;
                }
                else if (sender is TreeViewItem item && item.Tag is FileTreeNode treeNode)
                {
                    try { System.Diagnostics.Process.Start(treeNode.Path); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    e.Handled = true;
                }
            }
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is ListViewItem listViewItem && listViewItem.DataContext is FileTreeNode fileTreeNode)
            {
                try
                {
                    string snippets = new InMemoryLuceneSearch().GetFormattedSnippets(fileTreeNode.Path, currentSearchTerm);
                    WebView2Helpers.NavigateTostring(PreviewBrowser, snippets);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SearchHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            SearchHistoryComboBox.IsDropDownOpen = true;
        }

        private void SearchHistoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchHistoryComboBox.SelectedIndex > -1 ) 
            {
                SearchTextBox.Text = (string)SearchHistoryComboBox.SelectedItem;
                SearchTextBox.Focus();
            }        
            SearchHistoryComboBox.SelectedIndex = -1;
        }

        private void SearchTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var txtControl = sender as TextBox;
            txtControl.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtControl.SelectAll();
            }));
        }

        private void FileItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem listViewItem && listViewItem.Tag is FileTreeNode node)
            {
                try { System.Diagnostics.Process.Start(node.Directory); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                e.Handled = true;
            }
            else if (sender is TreeViewItem item && item.Tag is FileTreeNode treeNode)
            {
                try { System.Diagnostics.Process.Start(treeNode.Directory); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                e.Handled = true;
            }
        }
    }
}
