﻿using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace TextSeeker
{
    public class FolderTreeNode : TreeNode
    {
        public FolderTreeNode(string path) : base(path) { }
    }

    public class FileTreeNode : TreeNode
    {
        public FileTreeNode(string path) : base(path) { }
    }
    public class TreeNode : INotifyBase
    {
        private string _name;
        private string _path;
        private string _directory;
        private DateTime _dateLastModified;
        private TreeNode _parent;
        private ObservableCollection<TreeNode> _children = new ObservableCollection<TreeNode>();
        private bool? _isChecked = true;

        public TreeNode(string path)
        {
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

        public void AddChild(TreeNode child)
        {
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
            foreach (TreeNode child in Children)
            {
                if (value != null) { child.IsChecked = value; }
            }
        }
        void UpdateParentCheckSatus()
        {
            if (Parent != null)
            {
                bool allChecked = Parent.Children.OfType<TreeNode>().All(child => child.IsChecked == true);
                bool allUnchecked = Parent.Children.OfType<TreeNode>().All(child => child.IsChecked == false);

                if (allChecked) { Parent.IsChecked = true; }
                else if (allUnchecked) { Parent.IsChecked = false; }
                else { Parent.IsChecked = null; }
            }
        }
    }
}