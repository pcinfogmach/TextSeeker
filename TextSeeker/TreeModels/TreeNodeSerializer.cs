using Newtonsoft.Json;
using System;
using System.IO;

namespace TextSeeker.TreeModels
{
    internal class TreeNodeSerializer
    {
        string jsonFilePath;
        public TreeNodeSerializer()
        {
            jsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeekerTreeNodes.json");
        }

        public void SaveToFile(TreeNode rootnode)
        {
            string jsonText = JsonConvert.SerializeObject(rootnode, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(jsonFilePath, jsonText);
        }

        public TreeNode LoadFromFile()
        {
            if (!File.Exists(jsonFilePath)) { return new TreeNode(null); }
            else
            {
                string jsonText = File.ReadAllText(jsonFilePath);
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

        private void SetParentReferences(TreeNode parentNode)
        {
            foreach (var child in parentNode.Children)
            {
                child.Parent = parentNode;
                SetParentReferences(child);
            }
        }
    }
}
