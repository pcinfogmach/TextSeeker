using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TextSeeker.Models;
using TextSeeker.SearchModels;
using TextSeeker.TreeModels;

namespace TextSeeker.Old
{
    internal class OldViewModel : INotifyBase
    {
        //#region Members

        //RootTreeNode _currentRootTreeViewNode;
        //RootTreeNode _unIndexedRootTreeViewNode;
        //RootTreeNode _indexedRootTreeViewNode;
        //ObservableCollection<TreeNode> _searchResults = new ObservableCollection<TreeNode>();
        //string _searchTerm = Properties.Settings.Default.MostRecentSearchTerm;
        //ObservableCollection<string> _recentSearches;
        //Visibility _searchButtonvisibilty = Visibility.Visible;
        //Visibility _stopButtonvisibilty = Visibility.Collapsed;
        //Visibility _searchInstructionsVisibilty = Visibility.Collapsed;
        //bool _showSearchInstructions;
        //bool _isSearchInProgress;
        //bool _searchIsEnabled = true;
        //bool _isIndexSearch = Properties.Settings.Default.IsIndexedSearch;
        //IndexedLuceneSearch luceneSearch = new IndexedLuceneSearch();
        //public CancellationTokenSource searchCancellationTokenSource = new CancellationTokenSource();

        //#endregion

        //#region Properties
        //public RootTreeNode RootTreeViewNode
        //{
        //    get
        //    {
        //        if (_currentRootTreeViewNode != null) { SaveCurrentRootNode(); }

        //        if (IsIndexSearch)
        //        {
        //            if (_indexedRootTreeViewNode == null) { _indexedRootTreeViewNode = TreeNodeSerializer.Load(Properties.Settings.Default.IndexedTreeView) ?? new RootTreeNode(null); }
        //            _currentRootTreeViewNode = _indexedRootTreeViewNode;
        //        }
        //        else
        //        {
        //            if (_unIndexedRootTreeViewNode == null) { _unIndexedRootTreeViewNode = TreeNodeSerializer.Load(Properties.Settings.Default.UnIndexedTreeView) ?? new RootTreeNode(null); }
        //            _currentRootTreeViewNode = _unIndexedRootTreeViewNode;
        //        }
        //        return _currentRootTreeViewNode;
        //    }
        //}

        //public ObservableCollection<TreeNode> SearchResults
        //{
        //    get => _searchResults;
        //    set
        //    {
        //        if (_searchResults != value)
        //        {
        //            _searchResults = value; OnPropertyChanged(nameof(SearchResults));
        //        }
        //    }
        //}

        //public string SearchTerm
        //{
        //    get => _searchTerm;
        //    set
        //    {
        //        if (_searchTerm != value)
        //        {
        //            Properties.Settings.Default.MostRecentSearchTerm = value;
        //            Properties.Settings.Default.Save();
        //            _searchTerm = value;
        //            OnPropertyChanged(nameof(SearchTerm));
        //        }
        //    }
        //}

        //public ObservableCollection<string> RecentSearches
        //{
        //    get => _recentSearches;
        //    set
        //    {
        //        if (_recentSearches != value)
        //        {
        //            _recentSearches = value;
        //            OnPropertyChanged(nameof(RecentSearches));
        //        }
        //    }
        //}

        //public Visibility SearchButtonvisibilty
        //{
        //    get => _searchButtonvisibilty;
        //    set
        //    {
        //        if (_searchButtonvisibilty != value)
        //        {
        //            _searchButtonvisibilty = value;
        //            if (value == Visibility.Visible) StopButtonvisibilty = Visibility.Collapsed;
        //            OnPropertyChanged(nameof(SearchButtonvisibilty));
        //        }
        //    }
        //}
        //public Visibility StopButtonvisibilty
        //{
        //    get => _stopButtonvisibilty;
        //    set
        //    {
        //        if (_stopButtonvisibilty != value)
        //        {
        //            _stopButtonvisibilty = value;
        //            if (value == Visibility.Visible) SearchButtonvisibilty = Visibility.Collapsed;
        //            OnPropertyChanged(nameof(StopButtonvisibilty));
        //        }
        //    }
        //}

        //public Visibility SearchInstructionsVisibilty
        //{
        //    get => _searchInstructionsVisibilty;
        //    set
        //    {
        //        if (_searchInstructionsVisibilty != value)
        //        {
        //            _searchInstructionsVisibilty = value;
        //            OnPropertyChanged(nameof(SearchInstructionsVisibilty));
        //        }
        //    }
        //}

        //public bool ShowSearchInstructions
        //{
        //    get => _showSearchInstructions;
        //    set
        //    {
        //        if (_showSearchInstructions != value)
        //        {
        //            if (value == true) { SearchInstructionsVisibilty = Visibility.Visible; } else { SearchInstructionsVisibilty = Visibility.Collapsed; }
        //            _showSearchInstructions = value;
        //            OnPropertyChanged(nameof(ShowSearchInstructions));
        //        }
        //    }
        //}

        //public bool IsSearchInProgress
        //{
        //    get => _isSearchInProgress;
        //    set
        //    {
        //        if (_isSearchInProgress != value)
        //        {
        //            if (value == true) { StopButtonvisibilty = Visibility.Visible; } else { SearchButtonvisibilty = Visibility.Visible; }
        //            _isSearchInProgress = value;
        //            OnPropertyChanged(nameof(IsSearchInProgress));
        //        }
        //    }
        //}

        //public bool SearchIsEnabled { get => _searchIsEnabled; set { if (_searchIsEnabled != value) { _searchIsEnabled = value; OnPropertyChanged(nameof(SearchIsEnabled)); } } }

        //public bool IsIndexSearch
        //{
        //    get => _isIndexSearch;
        //    set
        //    {
        //        if (_isIndexSearch != value)
        //        {
        //            Properties.Settings.Default.IsIndexedSearch = value;
        //            Properties.Settings.Default.Save();
        //            _isIndexSearch = value;

        //            OnPropertyChanged(nameof(IsIndexSearch));
        //            OnPropertyChanged(nameof(RootTreeViewNode));
        //        }
        //    }
        //}

        //#endregion

        //#region Methods
        //public OldViewModel()
        //{
        //    OnPropertyChanged(nameof(IsIndexSearch));
        //    RecentSearches = new ObservableCollection<string>(Properties.Settings.Default.RecentSearches.Cast<string>().ToList());
        //}

        //public void SaveCurrentRootNode()
        //{
        //    if (_currentRootTreeViewNode != null)
        //    {
        //        if (_currentRootTreeViewNode == _unIndexedRootTreeViewNode)
        //        {
        //            Properties.Settings.Default.UnIndexedTreeView = TreeNodeSerializer.Serialize(_currentRootTreeViewNode);
        //        }
        //        else if (_currentRootTreeViewNode == _indexedRootTreeViewNode)
        //        {
        //            Properties.Settings.Default.IndexedTreeView = TreeNodeSerializer.Serialize(_currentRootTreeViewNode);
        //        }
        //        Properties.Settings.Default.Save();
        //    }
        //}

        //public async void AddNewNode()
        //{
        //    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        //    dialog.IsFolderPicker = true;

        //    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        //    {
        //        if (IsIndexSearch)
        //        {
        //            IsSearchInProgress = true;
        //            SearchIsEnabled = false;
        //            List<string> files = System.IO.Directory.GetFiles(dialog.FileName, "*.*", System.IO.SearchOption.AllDirectories).ToList();
        //            await Task.Run(() =>
        //            {
        //                luceneSearch.IndexFiles(files);
        //            });
        //            IsSearchInProgress = false;
        //            SearchIsEnabled = true;
        //        }
        //        TreeBuilder.AddTreeNode(dialog.FileName, RootTreeViewNode);
        //    }
        //}

        //public void RemoveSelectedNode()
        //{
        //    var selectedNode = RootTreeViewNode.AllTreeNodes.FirstOrDefault(node => node.IsSelected);
        //    if (selectedNode != null)
        //    {
        //        if (IsIndexSearch)
        //        {
        //            IsSearchInProgress = true;
        //            luceneSearch.RemoveFiles(TreeHelper.GetAllFileNodes(selectedNode));
        //        }
        //        selectedNode.Parent.RemoveChild(selectedNode);
        //    }
        //}

        //public List<TreeNode> GetCheckedFileNodes()
        //{
        //    return RootTreeViewNode.AllTreeNodes.Where(node => node.IsChecked == true).ToList();
        //}

        //public async Task SearchAsync()
        //{
        //    if (IsSearchInProgress) { searchCancellationTokenSource.Cancel(); return; }
        //    if (string.IsNullOrEmpty(SearchTerm)) { MessageBox.Show("אנא הזן טקסט לחיפוש"); return; }

        //    IsSearchInProgress = true; //also turns on the progress bar

        //    List<TreeNode> files = await Task.Run(() =>
        //    {
        //        return TreeHelper.GetCheckedFileNodes(RootTreeViewNode);
        //    });

        //    SearchResults.Clear();
        //    UpdateRecentSearchCollection(SearchTerm);

        //    if (IsIndexSearch)
        //    {
        //        SearchResults = await Task.Run(() =>
        //        {
        //            return new ObservableCollection<TreeNode>(luceneSearch.Search(SearchTerm, files));
        //        });
        //    }
        //    else
        //    {
        //        await Task.Run(() => { FullTextSearch(files); });
        //    }

        //    IsSearchInProgress = false;
        //    if (SearchResults.Count == 0) { MessageBox.Show("לא נמצאו תוצאות"); }
        //}

        //public async Task OnceOffSearchAsync()
        //{
        //    if (IsSearchInProgress) { searchCancellationTokenSource.Cancel(); return; }
        //    if (string.IsNullOrEmpty(SearchTerm)) { MessageBox.Show("אנא הזן טקסט לחיפוש"); return; }

        //    CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        //    dialog.IsFolderPicker = true;
        //    if (dialog.ShowDialog() != CommonFileDialogResult.Ok) { return; }

        //    SearchResults.Clear();
        //    UpdateRecentSearchCollection(SearchTerm);
        //    IsSearchInProgress = true; //also turns on the progress bar

        //    List<TreeNode> files = await Task.Run(() =>
        //    {
        //        List<TreeNode> tempNodes = new List<TreeNode>();
        //        List<string> filesList = System.IO.Directory.GetFiles(dialog.FileName, "*.*", System.IO.SearchOption.AllDirectories).ToList();
        //        foreach (var file in filesList) { tempNodes.Add(new FileTreeNode(file)); }
        //        return tempNodes;
        //    });

        //    await Task.Run(() => { FullTextSearch(files); });

        //    IsSearchInProgress = false;
        //    if (SearchResults.Count == 0) { MessageBox.Show("לא נמצאו תוצאות"); }
        //}

        //void FullTextSearch(List<TreeNode> files)
        //{
        //    searchCancellationTokenSource = new CancellationTokenSource();
        //    CancellationToken searchCancellationToken = searchCancellationTokenSource.Token;

        //    string searchText = SearchTerm;//added this variable in order to avoid a case where search term changes in the middle of a search            

        //    Parallel.ForEach(files, (file, loopState) =>
        //    {
        //        if (searchCancellationToken.IsCancellationRequested)
        //        {
        //            SearchIsEnabled = false; //temporarily disable search controls till search actually stops
        //            loopState.Stop();
        //        }

        //        if (new InMemoryLuceneSearch().Search(file.Path, searchText) == true)
        //        {
        //            Application.Current.Dispatcher.Invoke(() => { SearchResults.Add(file); });
        //        }
        //    });

        //    //SearchResults.OrderByDescending(f => f.searchScore);
        //    SearchIsEnabled = true;
        //}

        //public void UpdateRecentSearchCollection(string input)
        //{
        //    if (RecentSearches.Contains(input)) { RecentSearches.Remove(input); }
        //    if (RecentSearches.Count > 10) { RecentSearches.RemoveAt(RecentSearches.Count - 1); }
        //    RecentSearches.Insert(0, input);

        //    Properties.Settings.Default.RecentSearches.Clear();
        //    Properties.Settings.Default.RecentSearches.AddRange(RecentSearches.ToArray());
        //    Properties.Settings.Default.Save();
        //}

        //public void SortSearchResults(bool ascending = true)
        //{
        //    if (ascending)
        //    {
        //        SearchResults = new ObservableCollection<TreeNode>(SearchResults.OrderBy(node => node.Name));
        //    }
        //    else
        //    {
        //        SearchResults = new ObservableCollection<TreeNode>(SearchResults.OrderByDescending(node => node.Name));
        //    }
        //}


        //#endregion

    }
}
