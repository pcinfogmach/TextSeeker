using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace TextSeeker.TreeModels
{
    internal static class TreeNodeSerializer
    {
        public static string Serialize(TreeNode rootNode)
        {
            return JsonConvert.SerializeObject(rootNode, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            });
        }

        public static RootTreeNode Load(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText)) { return new RootTreeNode(null); }
            else
            {
                RootTreeNode rootNode = JsonConvert.DeserializeObject<RootTreeNode>(jsonText, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });

                SetParentReferences(rootNode);
                return rootNode;
            }
        }

        private static void SetParentReferences(TreeNode parentNode)
        {
            foreach (var child in parentNode.Children)
            {
                child.Parent = parentNode;
                SetParentReferences(child);
            }
        }
    }
}
