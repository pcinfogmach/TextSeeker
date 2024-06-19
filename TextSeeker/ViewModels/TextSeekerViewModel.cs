using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2013.Word;
using iText.Kernel.Colors;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        TreeNode _currentRootTreeViewNode = new TreeNode(null);
        TreeNode _unIndexedRootTreeViewNode = new TreeNode(null);
        TreeNode _indexedRootTreeViewNode = new TreeNode(null);
        ObservableCollection<TreeNode> _searchResults = new ObservableCollection<TreeNode>();
        TreeNodeSerializer treeNodeSerializer = new TreeNodeSerializer();
        string _searchTerm;
        Visibility _searchButtonvisibilty = Visibility.Visible;
        Visibility _stopButtonvisibilty = Visibility.Collapsed;
        bool _isSearchInProgress;
        bool _searchIsEnabled = true;
        bool _isIndexSearch;
        Visibility _indexOptionsVisiblity = Visibility.Collapsed;
        LuceneSearch luceneSearch = new LuceneSearch();
        public CancellationTokenSource searchCancellationTokenSource;

        #endregion

        #region Properties
        public TreeNode CurrentRootTreeViewNode { get => _currentRootTreeViewNode; set { _currentRootTreeViewNode = value; OnPropertyChanged(nameof(CurrentRootTreeViewNode)); } }
        public TreeNode UnIndexedRootTreeViewNode { get => _unIndexedRootTreeViewNode; set { _unIndexedRootTreeViewNode = value; OnPropertyChanged(nameof(UnIndexedRootTreeViewNode)); } }
        public TreeNode IndexedRootTreeViewNode { get => _indexedRootTreeViewNode; set { _indexedRootTreeViewNode = value; OnPropertyChanged(nameof(IndexedRootTreeViewNode)); } }
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
        public bool IsIndexSearch 
        {
            get => _isIndexSearch; 
            set 
            {
                if (value == true) 
                {
                    PopulateIndex(); 
                    IndexOptionsVisiblity = Visibility.Visible;
                    CurrentRootTreeViewNode = IndexedRootTreeViewNode;
                }
                else 
                { 
                    IndexOptionsVisiblity = Visibility.Collapsed;
                    CurrentRootTreeViewNode = UnIndexedRootTreeViewNode;
                }
                _isIndexSearch = value; 
                OnPropertyChanged(nameof(IsIndexSearch)); 
            } 
        }

        public Visibility IndexOptionsVisiblity
        {
            get => _indexOptionsVisiblity;
            set
            {
                _indexOptionsVisiblity = value;
                OnPropertyChanged(nameof(IndexOptionsVisiblity));
            }
        }


       
        #endregion

        #region Methods
        public TextSeekerViewModel()
        {
            treeNodeSerializer.JsonFilePath = "TextSeekerTreeNodes.json";
            UnIndexedRootTreeViewNode = treeNodeSerializer.LoadFromFile();
            CurrentRootTreeViewNode = UnIndexedRootTreeViewNode;
            treeNodeSerializer.JsonFilePath = "TextSeekerIndexedTreeNodes.json";
            IndexedRootTreeViewNode = treeNodeSerializer.LoadFromFile();
        }

        public void AddNewNode()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TreeBuilder.AddTreeNode(dialog.FileName, CurrentRootTreeViewNode);
                if (CurrentRootTreeViewNode == IndexedRootTreeViewNode)
                {
                    List<string> files = System.IO.Directory.GetFiles(dialog.FileName, "*.*", System.IO.SearchOption.AllDirectories).ToList();
                    luceneSearch.IndexFiles(files);
                }
            }        
        }

        public List<TreeNode> GetCheckedFileNodes()
        {
            return TreeHelper.GetCheckedFileNodes(CurrentRootTreeViewNode);
        }

        void PopulateIndex()
        {
            if (IndexedRootTreeViewNode.Children.Count <= 0 && CurrentRootTreeViewNode.Children.Count > 0)
            {
                var result = MessageBox.Show("האם ברצונך ליצור אינדקס כעת?", "חיפוש אינדקס",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes,
                    options: MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                if (result == MessageBoxResult.Yes)
                {
                    IsSearchInProgress = true;
                    IndexedRootTreeViewNode = CurrentRootTreeViewNode.HardCopy();
                    List<string> files = TreeHelper.GetAllFileNodes(IndexedRootTreeViewNode);
                    luceneSearch.IndexFiles(files);
                    IsSearchInProgress = false;
                }
            }
        }

        public async Task SearchAsync()
        {
            if (IsSearchInProgress) { searchCancellationTokenSource.Cancel(); return; }
            if (string.IsNullOrEmpty(SearchTerm)) { MessageBox.Show("אנא הזן טקסט לחיפוש"); return; }

            searchCancellationTokenSource = new CancellationTokenSource();
            CancellationToken searchCancellationToken = searchCancellationTokenSource.Token;
            IsSearchInProgress = true; //also turns on the progress bar

            List<TreeNode> files = await Task.Run(() =>
            {
                return TreeHelper.GetCheckedFileNodes(CurrentRootTreeViewNode);
            });
            
            if (IsIndexSearch) 
            {
                SearchResults = await Task.Run(() =>
                {
                    return new ObservableCollection<TreeNode>(luceneSearch.Search(SearchTerm, files));
                });
            } 
            else
            {
                string searchText = SearchTerm;//added this variable in order to avoid a case where search term changes in the middle of a search  

                SearchResults.Clear();

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
                            Application.Current.Dispatcher.Invoke(() => { SearchResults.Add(file); });
                        }
                    });
                }, searchCancellationToken);
            }

            SearchIsEnabled = true;
            IsSearchInProgress = false;
            if (SearchResults.Count == 0) { MessageBox.Show("לא נמצאו תוצאות"); }
        }

        public void SortSearchResults(bool ascending = true)
        {
            if (ascending)
            {
                SearchResults = new ObservableCollection<TreeNode>(SearchResults.OrderBy(node => node.Name));
            }
            else
            {
                SearchResults = new ObservableCollection<TreeNode>(SearchResults.OrderByDescending(node => node.Name));
            }
        }


        #endregion

    }
}
