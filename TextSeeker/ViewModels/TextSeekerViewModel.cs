using DocumentFormat.OpenXml.Drawing;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TextSeeker.Models;
using TextSeeker.SearchModels;
using TextSeeker.TreeModels;

namespace TextSeeker.ViewModels
{
    public class TextSeekerViewModel : INotifyBase
    {
        #region Members
        TreeNode _rootTreeViewNode = new TreeNode(null);
        ObservableCollection<TreeNode> _searchResults = new ObservableCollection<TreeNode>();
        TreeNodeSerializer treeNodeSerializer = new TreeNodeSerializer();
        string _searchTerm;
        Visibility _searchButtonvisibilty = Visibility.Visible;
        Visibility _stopButtonvisibilty = Visibility.Collapsed;
        bool _isSearchInProgress;
        bool _searchIsEnabled = true;

        #endregion

        #region Properties
        public TreeNode RootTreeViewNode { get => _rootTreeViewNode; set { _rootTreeViewNode = value; OnPropertyChanged(nameof(RootTreeViewNode)); } }
        public ObservableCollection<TreeNode> SearchResults { get => _searchResults; set { _searchResults = value; OnPropertyChanged(nameof(SearchResults)); } }
        public string SearchTerm { get => _searchTerm; set { _searchTerm = value; OnPropertyChanged(nameof(SearchTerm)); } }
        public Visibility SearchButtonvisibilty 
        { 
            get => _searchButtonvisibilty; 
            set 
            {
                _searchButtonvisibilty = value;
                if (value == Visibility.Visible) StopButtonvisibilty = Visibility.Collapsed;
                OnPropertyChanged(nameof(SearchButtonvisibilty)); 
            } 
        }
        public Visibility StopButtonvisibilty 
        {
            get => _stopButtonvisibilty; 
            set 
            {
                _stopButtonvisibilty = value; 
                if (value == Visibility.Visible) SearchButtonvisibilty = Visibility.Collapsed; 
                OnPropertyChanged(nameof(StopButtonvisibilty)); 
            }
        }
        public bool IsSearchInProgress
        {
            get => _isSearchInProgress;
            set
            {
                if (value == true) { StopButtonvisibilty = Visibility.Visible; } else { SearchButtonvisibilty = Visibility.Visible; }
                _isSearchInProgress = value; OnPropertyChanged(nameof(IsSearchInProgress));
            }
        }

        public bool SearchIsEnabled { get => _searchIsEnabled; set { _searchIsEnabled = value; OnPropertyChanged(nameof(SearchIsEnabled)); } }


        public CancellationTokenSource searchCancellationTokenSource;
        #endregion

        #region Methods
        public TextSeekerViewModel()
        {
            RootTreeViewNode = treeNodeSerializer.LoadFromFile();
        }

        public void AddNewNode()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TreeBuilder.AddTreeNode(dialog.FileName, RootTreeViewNode); ;
            }
        }

        public List<FileTreeNode> GetCheckedFileNodes()
        {
            return TreeHelper.GetCheckedFileNodes(RootTreeViewNode);
        }

        public async Task SearchAsync()
        {
            if (IsSearchInProgress) { searchCancellationTokenSource.Cancel(); return; }
            searchCancellationTokenSource = new CancellationTokenSource();
            CancellationToken searchCancellationToken = searchCancellationTokenSource.Token;

            string searchText = SearchTerm;//added this variable in order to avoid a case where search term changes in the middle of a search  
            if (string.IsNullOrEmpty(searchText)) { MessageBox.Show("אנא הזן טקסט לחיפוש"); return; }

            SearchResults.Clear();
            IsSearchInProgress = true; //also turns on the progress bar

            var files = await Task.Run(() =>
            {
                return TreeHelper.GetCheckedFileNodes(RootTreeViewNode);
            });

            // Process the files in parallel
            await Task.Run(() =>
            {
                Parallel.ForEach(files, (file, loopState) =>
                {
                    if (searchCancellationToken.IsCancellationRequested)
                    {
                        SearchIsEnabled = false; //temporarily disable search controls till search actually stops
                        loopState.Stop();
                    }

                    if (SearchModel.Search(file, searchText, SearchModel.SearchType.ContainsSearch, searchCancellationToken))
                    {
                        Application.Current.Dispatcher.Invoke(() => {  SearchResults.Add(file);  });
                    }
                });
            }, searchCancellationToken);

            SearchIsEnabled = true;
            IsSearchInProgress = false;
            if (SearchResults.Count == 0) { MessageBox.Show("לא נמצאו תוצאות"); }
        }

        #endregion

    }
}
