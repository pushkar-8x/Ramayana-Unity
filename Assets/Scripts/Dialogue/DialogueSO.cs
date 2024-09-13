using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue" , fileName = "Dialogue" ,order = 0 )]
    public class DialogueSO : ScriptableObject , ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        Dictionary<string , DialogueNode> nodeLookup = new Dictionary<string , DialogueNode>();
        [SerializeField]
        Vector2 newNodeOffset = new Vector2(300, 0);

        private void Awake()
        {
            
                
        }

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetChildNodes(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                   yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return dialogueNodes;
        }

        public DialogueNode GetRootNode()
        {
            return dialogueNodes[0];
        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);

            Undo.RegisterCreatedObjectUndo(newNode, "Created object Undo");
            Undo.RecordObject(this, "Created Dialogue Node");
            AddNode(newNode);
        }

        private void AddNode(DialogueNode newNode)
        {
            dialogueNodes.Add(newNode);
            OnValidate();
        }

        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.SetText("New Node");
            newNode.name = Guid.NewGuid().ToString();

            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
                newNode.SetIsPlayerNode(!parentNode.IsPlayerNode());
                newNode.SetPosition(parentNode.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);        
            OnValidate();
            CleanChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                if (node.GetChildren().Contains(nodeToDelete.name))
                {
                    node.RemoveChild(nodeToDelete.name);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            if (dialogueNodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if(AssetDatabase.GetAssetPath(this)!= "")
            {
                foreach(DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node , this);
                    }
                }
            }


        }

        public void OnAfterDeserialize()
        {
           // throw new NotImplementedException();
        }
    }
}

