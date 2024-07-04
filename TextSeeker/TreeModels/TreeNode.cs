using com.sun.org.apache.bcel.@internal.generic;
using Newtonsoft.Json;
using org.bouncycastle.mail.smime.examples;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace TextSeeker
{
    public class FolderTreeNode : TreeNode  {    public FolderTreeNode(string path) : base(path) { } }
    public class FileTreeNode : TreeNode {  public FileTreeNode(string path) : base(path) { } }
    public class TreeNode : INotifyBase
    {
        private string _name;
        private string _path;
        private string _directory;
        private DateTime _dateLastModified;
        private TreeNode _parent;
        private ObservableCollection<TreeNode> _children = new ObservableCollection<TreeNode>();
        public List<TreeNode> AllTreeNodes = new List<TreeNode>();
        private bool _isSelected;
        private bool? _isChecked = true;
        public float searchScore;

        public TreeNode(string path)
        {   
            if (string.IsNullOrWhiteSpace(path)) { return; }
            _path = path;
            _directory = System.IO.Path.GetDirectoryName(path);
            _name = System.IO.Path.GetFileName(path);
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        public string Directory
        {
            get => _directory;
            set
            {
                if (_directory != value)
                {
                    _directory = value;
                    OnPropertyChanged(nameof(Directory));
                }
            }
        }

        public DateTime DateLastModified
        {
            get => _dateLastModified;
            set
            {
                if (_dateLastModified != value)
                {
                    _dateLastModified = value;
                    OnPropertyChanged(nameof(DateLastModified));
                }
            }
        }
        public TreeNode Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged(nameof(Parent));
                }
            }
        }

        public ObservableCollection<TreeNode> Children
        {
            get => _children;
            set
            {
                if (_children != value)
                {
                    _children = value;
                    OnPropertyChanged(nameof(Children));
                }
            }
        }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                    UpdateChildCheckSatus(value);
                    UpdateParentCheckSatus();
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public void AddChild(TreeNode child)
        {
            child.AllTreeNodes = this.AllTreeNodes;
            AllTreeNodes.Add(child);
            child.Parent = this;
            _children.Add(child);
            OnPropertyChanged(nameof(Children));
        }

        public void RemoveChild(TreeNode child)
        {
            if (_children.Remove(child))
            {
                child.Parent = null;
                OnPropertyChanged(nameof(Children));
            }
        }

        void UpdateChildCheckSatus(bool? value)
        {
            if (value != null && Children.Any(child => child.IsChecked != value)) 
            {
                foreach (TreeNode child in Children) { child.IsChecked = value; }
            }
        }

        void UpdateParentCheckSatus()
        {
            if (Parent != null)
            {
                bool allChecked = Children.All(child => child.IsChecked == true);
                bool allUnchecked = Children.All(child => child.IsChecked == false);

                if (allChecked) { Parent.IsChecked = true; }
                else if (allUnchecked) { Parent.IsChecked = false; }
                else { Parent.IsChecked = null; }
            }
        }

        public TreeNode HardCopy()
        {
            if (this is FileTreeNode) 
            {
                FileTreeNode treeNode = new FileTreeNode(Path);
                foreach (var node in Children)
                {
                    treeNode.AddChild(node.HardCopy());
                }
                return treeNode;
            }
            else if (this is FolderTreeNode) 
            {
                FolderTreeNode treeNode = new FolderTreeNode(Path);
                foreach (var node in Children)
                {
                    treeNode.AddChild(node.HardCopy());
                }
                return treeNode;
            }
            else
            {
                TreeNode treeNode = new TreeNode(Path);
                foreach (var node in Children)
                {
                    treeNode.AddChild(node.HardCopy());
                }
                return treeNode;
            }
        }
    }
}