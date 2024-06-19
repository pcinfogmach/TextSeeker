﻿using System;
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
        TextSeekerViewModel viewModel = new TextSeekerViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TreeNodeSerializer serializer = new TreeNodeSerializer();
            serializer.JsonFilePath = "TextSeekerTreeNodes.json";
            serializer.SaveToFile(viewModel.UnIndexedRootTreeViewNode);
            serializer.JsonFilePath = "TextSeekerIndexedTreeNodes.json";
            serializer.SaveToFile(viewModel.IndexedRootTreeViewNode);
        }

        private void AddTreeItemButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddNewNode(); 
        }

        private void DeleteTreeItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem is TreeNode treeNode)
            {
                treeNode.Parent.Children.Remove(treeNode);
                if (viewModel.IsIndexSearch == true)
                {
                    LuceneSearch luceneSearch = new LuceneSearch();
                    luceneSearch.RemoveFiles(TreeHelper.GetAllFileNodes(treeNode));
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.SearchAsync();        
        }

        private async void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { await viewModel.SearchAsync(); e.Handled = true; }
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
                string content = TextExtractor.ReadText(fileTreeNode.Path);
                WebView2Helpers.NavigateTostring(PreviewBrowser, content, SearchTextBox.Text, false);
            }
        }

        //private void IndexMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    IndexMenuList.IsDropDownOpen = true;
        //}
    }
}
