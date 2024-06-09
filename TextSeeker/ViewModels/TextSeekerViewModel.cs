using DocumentFormat.OpenXml.Drawing;
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
        string _searchButtonText = "🔍";
        bool _isSearchInProgress;
        bool _isEnabled = true;
        #endregion

        #region Properties
        public TreeNode RootTreeViewNode { get => _rootTreeViewNode; set { _rootTreeViewNode = value; OnPropertyChanged(nameof(RootTreeViewNode)); } }
        public ObservableCollection<TreeNode> SearchResults { get => _searchResults; set { _searchResults = value; OnPropertyChanged(nameof(SearchResults)); } }
        public string SearchTerm { get => _searchTerm; set { _searchTerm = value; OnPropertyChanged(nameof(SearchTerm)); } }
        public string SearchButtonText { get => _searchButtonText; set { _searchButtonText = value; OnPropertyChanged(nameof(SearchButtonText)); } }
        public bool IsSearchInProgress
        {
            get => _isSearchInProgress;
            set
            {
                if (value == true) { SearchButtonText = "■"; } else { SearchButtonText = "🔍"; }
                _isSearchInProgress = value; OnPropertyChanged(nameof(IsSearchInProgress));
            }
        }

        public bool IsEnabled { get => _isEnabled; set { _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); } }


        public CancellationTokenSource searchCancellationTokenSource;
        #endregion

        #region Methods
        public TextSeekerViewModel()
        {
            RootTreeViewNode = treeNodeSerializer.LoadFromFile();
        }

        public void AddNewNode()
        {
            string folderPath = FolderPickerLauncher.SelectedFolder("בחר תיקייה");
            if (!string.IsNullOrEmpty(folderPath))
            {
                TreeBuilder.AddTreeNode(folderPath, RootTreeViewNode);
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
                        IsEnabled = false; //temporarily disable search controls till search actually stops
                        loopState.Stop();
                    }

                    if (SearchModel.Search(file, searchText, SearchModel.SearchType.ContainsSearch, searchCancellationToken))
                    {
                        Application.Current.Dispatcher.Invoke(() => {  SearchResults.Add(file);  });
                    }
                });
            }, searchCancellationToken);

            IsEnabled = true;
            IsSearchInProgress = false;
        }

        #endregion

    }
}
