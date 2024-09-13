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
        GUIStyle playerNodeStyle;
        [NonSerialized]
        DialogueNode draggingNode =null;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode linkingParentNode = null;
        [NonSerialized]
        bool draggingCanvas;
        [NonSerialized]
        Vector2 dragOffsetCanvas;

        const float canvasSize = 5000f;
        const float backgroundSize = 50f;

        Vector2 dragOffset = Vector2.zero;
        Vector2 scrollPosition = Vector2.zero;

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


            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(10, 10, 10, 10);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            DialogueSO dialogue = Selection.activeObject as DialogueSO;
            if(dialogue != null)
            {
                selectedDialogue = dialogue;
            }
            Repaint();
        }

        private void OnGUI()
        {
            if (selectedDialogue != null)
            {
                ProcessEvents();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas =  GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);
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
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted Dialogue Node");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }

                EditorGUILayout.EndScrollView();
            }
            
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if(draggingNode!=null)
                {
                    dragOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    dragOffsetCanvas = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {                
                draggingNode.SetPosition(Event.current.mousePosition + dragOffset) ;
                GUI.changed = true;
            }

            else if(Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = dragOffsetCanvas - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }

        }

        private void DrawNodes(DialogueNode dialogueNode)
        {
            GUIStyle style = nodeStyle;
            if(dialogueNode.IsPlayerNode())
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(dialogueNode.GetRect(), style);

            dialogueNode.SetText(EditorGUILayout.TextField(dialogueNode.GetText()));

            GUILayout.BeginHorizontal();

            DrawChildConnections(dialogueNode);
            if (GUILayout.Button("Add"))
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

        private void DrawChildConnections(DialogueNode dialogueNode)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = dialogueNode;
                }
            }
            else if(linkingParentNode == dialogueNode)
            {
                if(GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }              
            }
            else if(linkingParentNode.GetChildren().Contains(dialogueNode.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveChild(dialogueNode.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(dialogueNode.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode rootNode)
        {
            Vector2 startPosition = new Vector2(rootNode.GetRect().xMax , rootNode.GetRect().yMin + rootNode.GetRect().height / 2) ;
            foreach (var node in selectedDialogue.GetChildNodes(rootNode))
            {               
                Vector2 endPosition = new Vector2(node.GetRect().xMin, node.GetRect().yMin + node.GetRect().height/2 );
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
                if(node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}


