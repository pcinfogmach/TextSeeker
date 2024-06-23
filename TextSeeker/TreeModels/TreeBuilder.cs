using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TextSeeker.Models
{
    public static class TreeBuilder
    {
        public static FolderTreeNode AddTreeNode(string rootFolderPath, TreeNode parentNode)
        {
            var existingNode = parentNode.Children.FirstOrDefault(node => node.Path == rootFolderPath);
            if (existingNode != null)
            {
                existingNode.Children.Clear();
                PopulateChildren(rootFolderPath, existingNode);
                return existingNode as FolderTreeNode;
            }
            else
            {
                FolderTreeNode rootNode = new FolderTreeNode(rootFolderPath);
                parentNode.AddChild(rootNode);
                PopulateChildren(rootFolderPath, rootNode);
                return rootNode;
            }
        }


        private static void PopulateChildren(string folderPath, TreeNode parentNode)
        {
            // Get all directories and files in the current folder
            string[] directories = Directory.GetDirectories(folderPath);
            string[] files = Directory.GetFiles(folderPath);

            // Add files as child nodes to the parent node
            foreach (string file in files)
            {
                FileTreeNode fileNode = new FileTreeNode(file);
                fileNode.Parent = parentNode;
                parentNode.AddChild(fileNode);
            }

            // Recursively populate tree for each subdirectory
            foreach (string directory in directories)
            {
                FolderTreeNode directoryNode = new FolderTreeNode(directory);
                parentNode.AddChild(directoryNode);
                PopulateChildren(directory, directoryNode);
            }
        }
    }
}
