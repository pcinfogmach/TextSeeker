using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSeeker.TreeModels
{
    internal class TreeHelper
    {
        public static List<TreeNode> GetCheckedFileNodes(TreeNode rootNode)
        {
            List<TreeNode> checkedFileNodes = new List<TreeNode>();
            foreach (var treeNode in rootNode.Children)
            {
                TraverseCheckedTreeNodes(treeNode, ref checkedFileNodes);
            }
            return checkedFileNodes;
        }

        private static void TraverseCheckedTreeNodes(TreeNode node, ref List<TreeNode> checkedFileNodes)
        {
            if (node.IsChecked == true)
            {
                if (node is FileTreeNode fileNode)
                {
                    checkedFileNodes.Add(fileNode);
                }
                else
                {
                    foreach (var childNode in node.Children)
                    {
                        TraverseCheckedTreeNodes(childNode, ref checkedFileNodes);
                    }
                }
               
            }          
        }

        public static List<string> GetAllFileNodes(TreeNode rootNode)
        {
            List<string> files = new List<string>();
            foreach (var treeNode in rootNode.Children)
            {
                if (treeNode is FolderTreeNode folderTreeNode)
                {
                    files.AddRange(GetAllFileNodes(folderTreeNode));
                }
                else if (treeNode is FileTreeNode fileTreeNode)
                {
                    files.Add(fileTreeNode.Path);
                }
            }
            return files;
        }
    }
}