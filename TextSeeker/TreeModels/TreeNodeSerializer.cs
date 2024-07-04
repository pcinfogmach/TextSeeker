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

        public static TreeNode Load(string jsonText)
        {
            if (string.IsNullOrEmpty(jsonText)) { return new TreeNode(null); }
            else
            {
                TreeNode rootNode = JsonConvert.DeserializeObject<TreeNode>(jsonText, new JsonSerializerSettings
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
