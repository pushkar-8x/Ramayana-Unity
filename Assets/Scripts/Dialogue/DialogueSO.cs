using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(menuName = "Dialogue/New Dialogue" , fileName = "Dialogue" ,order = 0 )]
    public class DialogueSO : ScriptableObject
    {
        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        Dictionary<string , DialogueNode> nodeLookup = new Dictionary<string , DialogueNode>();


        private void Awake()
        {
            if (dialogueNodes.Count == 0)
            {
                DialogueNode node = new DialogueNode();
                node.text = "Title";
                node.uniqueID = Guid.NewGuid().ToString();
                dialogueNodes.Add(node);
            }
                
        }

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (DialogueNode node in dialogueNodes)
            {
                nodeLookup[node.uniqueID] = node;
            }
        }

        public IEnumerable<DialogueNode> GetChildNodes(DialogueNode parentNode)
        {
            foreach (string childID in parentNode.children)
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
            DialogueNode newNode = new DialogueNode();
            newNode.text = "New Node";
            newNode.uniqueID = Guid.NewGuid().ToString();
            dialogueNodes.Add(newNode);
            parentNode.children.Add(newNode.uniqueID);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            dialogueNodes.Remove(nodeToDelete);
            OnValidate();
            CleanChildren(nodeToDelete);
        }

        private void CleanChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                if (node.children.Contains(nodeToDelete.uniqueID))
                {
                    node.children.Remove(nodeToDelete.uniqueID);
                }
            }
        }
    }
}

