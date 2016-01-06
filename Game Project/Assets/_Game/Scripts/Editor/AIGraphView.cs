using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;


public class AIGraphView
{
    public static AIGraphView graphView;

    public Rect panel; // Represents the rectangle in the editor window
    public Rect canvas; // Represents the whole canvas

    bool makingTransition = false; // Is true while a transition is being made
    bool transitionDrawn = false;

    public Vector2 mousePos;

    AIGraphicElement selection = null;


    public void Initialize()
    {
        graphView = this;
        canvas = new Rect(0, 0, 1000, 1000);
        makingTransition = false;
    }


    public void OnGUI()
    {
        GUI.BeginGroup(panel);

        // Clamp canvas displacement
        canvas.center = Vector2.Min(canvas.center, canvas.size / 2);
        canvas.center = Vector2.Max(canvas.center, panel.size - panel.size / 2);


        GUILayout.BeginArea(canvas);

        DrawCanvas();
        
        GUILayout.EndArea();
        
        GUI.EndGroup();
    }


    public void DrawCanvas()
    {
        Rect box = new Rect(-canvas.position, panel.size);
        //Rect viewbox = new Rect(Vector2.zero, view.size);
        GUI.color = new Color(0.2f, 0.2f, 0.2f);
        GUI.Box(box, GUIContent.none);    // Draws the background


        // Draw secondary Grid
        Handles.color = new Color(0.15f, 0.15f, 0.15f);
        for (int i = 0; i < canvas.width; i += 10)
            Handles.DrawLine(new Vector3(i, 0, 0), new Vector3(i, canvas.height, 0));
        for (int j = 0; j < canvas.height; j += 10)
            Handles.DrawLine(new Vector3(0, j, 0), new Vector3(canvas.width, j, 0));

        // Draw primmary Grid
        Handles.color = new Color(0.10f, 0.10f, 0.10f);
        for (int i = 0; i < canvas.width; i += 100)
            Handles.DrawLine(new Vector3(i, 0, 0), new Vector3(i, canvas.height, 0));
        for (int j = 0; j < canvas.height; j += 100)
            Handles.DrawLine(new Vector3(0, j, 0), new Vector3(canvas.width, j, 0));


        AIBehaviourGraph graph = AIEditor.graph;
        if (!graph)
        {
            GUI.color = new Color(0.4f, 0.4f, 0.4f, 0.7f);
            GUI.Box(box, GUIContent.none);    // Draws the background
            return;
        }

        AIGraphicNode selectedNode = selection as AIGraphicNode;
        AITransition selectedTransition = selection as AITransition;

        //Draw Behaviour Nodes
        if (selectedNode != null)
        {
            Handles.color = Color.cyan;
            Rect nodeRect = selectedNode.Area;
            Vector3[] corners = new Vector3[5];
            corners[0] = nodeRect.position;
            corners[1] = nodeRect.position + Vector2.right * nodeRect.width;
            corners[2] = nodeRect.position + nodeRect.size;
            corners[3] = nodeRect.position + Vector2.up * nodeRect.height;
            corners[4] = nodeRect.position;
            Handles.DrawPolyLine(corners);
        }


        // Draws existing Transitions and a a temporary transition whenever the user is creating one.
        GUI.color = Color.white;
        transitionDrawn = false;
        foreach (AIBehaviour behaviour in graph.behaviours)
        {
            foreach (AITransition transition in behaviour.transitions)
            {
                AIBehaviour destination = graph.behaviours[transition.endIndex];
                bool selected = DrawTransition(behaviour.graphic.Area, destination.graphic.Area, Color.white, Color.cyan);

                if (selected)
                {
                    if (transition == selection)
                        selection = null;
                    else
                        selection = transition;
                }
            }

			if( behaviour.graphic == null )
			{
				behaviour.graphic = new AIGraphicNode();
				behaviour.graphic.Area = new Rect(0,0,100,100);
			}


            AIGraphicNode graphic = behaviour.graphic;

            if (selectedNode != null && graphic.Area.Contains(mousePos)
                && makingTransition && !transitionDrawn
                && behaviour.graphic != selection)
            {
                DrawTransition(selectedNode.Area, graphic.Area, Color.white, Color.white);
                transitionDrawn = true;
            }
            Rect area = graphic.Area;
            area.center = Vector2.Max(area.min, Vector2.zero) + area.size / 2;
            area.center = Vector2.Min(area.max, canvas.size) -  area.size / 2;
            graphic.Area = area;
        }



        if (selectedNode != null && makingTransition && !transitionDrawn)
        {
            Rect selected = selectedNode.Area;
            Rect mouse = new Rect(mousePos, Vector2.zero);
            DrawTransition(selected, mouse, Color.white, Color.white);
        }

        if (selectedTransition != null)
        {
            Rect start = graph.behaviours[selectedTransition.startIndex].graphic.Area;
            Rect end = graph.behaviours[selectedTransition.endIndex].graphic.Area;
            DrawTransition(start, end, Color.cyan, Color.cyan);

        }

        // Draws Behaviour Nodes
        AIEditor.editor.BeginEditorWindows();
        foreach (AIBehaviour behaviour in graph.behaviours)
        {
            behaviour.graphic.Area = GUI.Window(behaviour.graphic.index, behaviour.graphic.Area, DrawNodeWindow, behaviour.Name);
        }
        AIEditor.editor.EndEditorWindows();


    }

    struct NodeArg
    {
        public int id;
		public NodeArg(int id)
        {
            this.id = id;
        }
    }

    //This function is called from the GUI (Right-click menu)
    void StartMakingTransition(object arg)
    {
		NodeArg nodeArg = (NodeArg)arg;
		selection = AIEditor.graph.behaviours[nodeArg.id].graphic;
        makingTransition = true;
    }


	//This function is called from the GUI (Right-click menu)
	void DeleteBehaviour(object arg)
	{
		NodeArg nodeArg = (NodeArg)arg;
		AIBehaviour behaviour = AIEditor.graph.behaviours[nodeArg.id];
		if( EditorUtility.DisplayDialog("Delete Behaviour", "Do you want to delete " + behaviour.Name, "Yes", "No") )
		{
			AIEditor.graph.behaviours.RemoveAt(nodeArg.id);
			UnityEngine.Object.DestroyImmediate(behaviour,true);
		}
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(AIEditor.graph));
		AIEditor.graph.RestoreIndices();
	}
	

    // Draws a transition line between the specified points. @returns true if the user clicked on the line
    bool DrawTransition(Rect a, Rect b, Color normalColor, Color highlightColor)
    {
        Vector2 displace = b.center - a.center;
        Vector2 endDir = new Vector2(Mathf.Sign(displace.x), Mathf.Sign(displace.y));
        Vector2 startDir = endDir;
        if (Mathf.Abs(displace.x) < Mathf.Abs(displace.y))
            startDir.x = 0;
        else
            startDir.y = 0;


        Vector2 startPos = a.center + Vector2.Scale(startDir, a.size / 2);
        Vector2 endPos = b.center - Vector2.Scale(endDir, b.size / 2);
        Vector2 startTan = startPos + Vector2.Scale(startDir, a.size * 0.3f);
        Vector2 endTan = endPos - Vector2.Scale(endDir, b.size * 0.3f);
        int division = (int)((startPos - endPos).magnitude * 0.02f);
        division = Math.Max(4, division);
        //Vector3[] bezierPoints = Handles.MakeBezierPoints(startPos, endPos, startTan, endTan, division);
        float width = 5.0f;

        bool mouseOverLine = HandleUtility.DistancePointBezier(mousePos, startPos, endPos, startTan, endTan) < width;

        Color lineColor = mouseOverLine ? highlightColor : normalColor;
        Handles.DrawBezier(startPos, endPos, startTan, endTan, lineColor, null, width);


        if (b.size != Vector2.zero)
        {
            Vector2 arrowHeight = endDir * width * 1.3f;
            Vector2 arrowWidth = Vector3.Cross(endDir, Vector3.forward) * width * 1.2f;

            Vector3[] arrowCap = new Vector3[3];
            arrowCap[0] = endPos;
            arrowCap[1] = endPos - arrowHeight + arrowWidth;
            arrowCap[2] = endPos - arrowHeight - arrowWidth;
            Handles.color = lineColor;
            Handles.DrawAAConvexPolygon(arrowCap);
        }

        return mouseOverLine && Event.current.type == EventType.mouseUp;
    }

    public void DrawNodeWindow(int id)
    {

        Event e = Event.current;
        if (e.type == EventType.MouseUp && e.button == 1)
        {
            Debug.Log("Mouse event: " + e.type.ToString());
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Make Transition"), false, StartMakingTransition, new NodeArg(id));
            contextMenu.AddItem(new GUIContent("Delete Behaviour"), false, DeleteBehaviour, new NodeArg(id));
            contextMenu.ShowAsContext();
            e.Use();
        }

        if (makingTransition)
        {

            AIGraphicNode selectedNode = selection as AIGraphicNode;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                AIEditor.graph.behaviours[selectedNode.index].AddTransition(AIEditor.graph.behaviours[id]);
                makingTransition = false;
                e.Use();
            }
        }
        AIEditor.graph.behaviours[id].DrawNodeWindow();

        GUI.DragWindow();

    }


    public struct CreateArg
    {
        public Vector2 position;
        public Type type;

        public CreateArg(Type type, Vector2 position)
        {
            this.type = type;
            this.position = position;
        }
    }


    public void CreateNode(object arg)
    {
        CreateArg createArg = (CreateArg)arg;
        AIBehaviour behaviour = (AIBehaviour)ScriptableObject.CreateInstance(createArg.type);
        AIEditor.graph.AddBehaviour(behaviour);
        Rect area = behaviour.graphic.Area;
        area.position = mousePos;
        behaviour.graphic.Area = area; 
    }


}
