using System;
using System.Collections.ObjectModel;
using TextSeeker.TextSeekerMVVM.Helpers;
using TextSeeker.TreeModels;

namespace TextSeeker.TextSeekerMVVM.SearchModels
{
    public class ResultItem :INotifyBase
    {

        private TreeNode _treeNode;
        private string _snippet;

        public TreeNode TreeNode
        {
            get => _treeNode;
            set {   if (_treeNode != value)  {   _treeNode = value;   OnPropertyChanged(nameof(TreeNode));   }  }
        }

        public string Snippet
        {
            get => _snippet;
            set { if (_snippet != value) { _snippet = value.RemoveEmptyLines().Trim(); OnPropertyChanged(nameof(Snippet)); } }
        }
    }
}
