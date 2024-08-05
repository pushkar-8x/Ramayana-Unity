using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;

namespace  RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private DialogueSO selectedDialogue = null;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        DialogueNode draggingNode =null;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;

        Vector2 dragOffset = Vector2.zero;

        [MenuItem("Window/DialogueEditor")]
        public static void ShowDialogueEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "DialogueEditor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            DialogueSO dialogue = EditorUtility.InstanceIDToObject(instanceID) as DialogueSO;
            if (dialogue != null)
            {
                ShowDialogueEditorWindow();
                Debug.Log("Opened dialogue!");
                return true;
            }
            else
            {
                return false;
            }    
            
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(10, 10, 10, 10);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            DialogueSO dialogue = Selection.activeObject as DialogueSO;
            selectedDialogue = dialogue ? dialogue : null;
            Repaint();
        }

        private void OnGUI()
        {
            if (selectedDialogue != null)
            {
                ProcessEvents();
                
                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
                {    
                    DrawConnections(dialogueNode);
                }
                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllNodes())
                {
                    DrawNodes(dialogueNode);
                }
                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Created Dialogue Node");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
            
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if(draggingNode!=null)
                dragOffset = draggingNode.rect.position - Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                draggingNode.rect.position = Event.current.mousePosition + dragOffset ;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
                
        }

        private void DrawNodes(DialogueNode dialogueNode)
        {
            GUILayout.BeginArea(dialogueNode.rect , nodeStyle);

            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextField(dialogueNode.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialogue Text");
                dialogueNode.text = newText;
            }
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Add"))
            {
                creatingNode = dialogueNode;
            }

            if (GUILayout.Button("Remove"))
            {
                deletingNode = dialogueNode;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode rootNode)
        {
            Vector2 startPosition = new Vector2(rootNode.rect.xMax , rootNode.rect.yMin + rootNode.rect.height / 2) ;
            foreach (var node in selectedDialogue.GetChildNodes(rootNode))
            {               
                Vector2 endPosition = new Vector2(node.rect.xMin, node.rect.yMin + node.rect.height/2 );
                Vector2 controlOffset = endPosition - startPosition;
                controlOffset.y = 0;
                controlOffset.x *= 0.8f; 

                Handles.DrawBezier(startPosition,endPosition,
                    startPosition+controlOffset,endPosition - controlOffset,Color.white , null , 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if(node.rect.Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}


