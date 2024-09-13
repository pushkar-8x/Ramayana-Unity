using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        string text;
        [SerializeField]
        bool isPlayerNode;
        [SerializeField]
        List<string> children = new List<string>();
        [SerializeField]
        Rect rect = new Rect(0, 0, 200, 100);
        

        public Rect GetRect()
        {
            return rect;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public bool IsPlayerNode()
        {
            return isPlayerNode;
        }


#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Update Dialogue Text");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }
        public void SetIsPlayerNode(bool v)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerNode = v;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }


        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Added Child !");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Removed Child !");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

     
    }
#endif
}

