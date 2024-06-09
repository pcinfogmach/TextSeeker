using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSeeker.TreeModels
{
    internal class TreeHelper
    {
        public static List<FileTreeNode> GetCheckedFileNodes(TreeNode rootNode)
        {
            List<FileTreeNode> checkedFileNodes = new List<FileTreeNode>();
            foreach (var treeNode in rootNode.Children)
            {
                TraverseTree(treeNode, ref checkedFileNodes);
            }
            return checkedFileNodes;
        }

        private static void TraverseTree(TreeNode node, ref List<FileTreeNode> checkedFileNodes)
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
                        TraverseTree(childNode, ref checkedFileNodes);
                    }
                }
               
            }          
        }
    }
}