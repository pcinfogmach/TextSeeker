using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using TextSeeker.Helpers;
using TextSeeker.TreeModels;
using TextSeeker.ViewModels;

namespace TextSeeker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextSeekerViewModel viewModel = new TextSeekerViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TreeNodeSerializer serializer = new TreeNodeSerializer();
            serializer.SaveToFile(viewModel.RootTreeViewNode);
        }

        private void AddTreeItemButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddNewNode(); 
        }

        private void DeleteTreeItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoryTreeView.SelectedItem is TreeNode treeNode)
            {
                treeNode.Parent.Children.Remove(treeNode);
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.SearchAsync();        
        }

        private async void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { await viewModel.SearchAsync(); e.Handled = true; }
        }

        private void SearchResultsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResultsDataGrid.SelectedItem is TreeNode node)
            {
                string content = TextExtractor.ReadText(node.Path);
                WebView2Helpers.NavigateTostring(PreviewBrowser, content, viewModel.SearchTerm, false);
            }
        }

        private void FileItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridCell cell && cell.Tag is FileTreeNode node)
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
                if (sender is DataGridCell cell && cell.Tag is FileTreeNode node)
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
    }
}
