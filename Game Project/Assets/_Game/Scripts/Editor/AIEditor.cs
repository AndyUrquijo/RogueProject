using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Linq;


// Used to make speed and height variables calculate each other automatically
[CustomEditor(typeof(AIController))]
public class AIControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        /* Obsolete
        AIController controller = (AIController)target;
        GUILayout.Label("Behaviour Count: " + controller.behaviours.Count);
        if (GUILayout.Button("Open stuff"))
        {
            AIEditor window = (AIEditor)EditorWindow.GetWindow(typeof(AIEditor));
            window.Initialize();
            AIEditor.graph = controller;
            controller.StartController();
        }
        */
    }
}


public class AIEditor : EditorWindow
{
    public static AIEditor editor;

    [System.NonSerialized]
    bool editorInitialized = false;

    public static AIBehaviourGraph graph = null;
    AIGraphView graphView;

    List<Type> AIBehaviourTypes;

    Event e;

    Rect control;
    float controlWidth = 100.0f; // relative position of the split

    Vector2 mousePos;
    Vector2 mousePosPrev;
    public string[] options = new string[] { "Cube", "Sphere", "Plane" };
    public int index = 0;




    GUIStyle listStyle = null;


    [MenuItem("AI Editor/Open")]
    static void Init()
    {
        AIEditor window = (AIEditor)EditorWindow.GetWindow(typeof(AIEditor));
        window.Initialize();
    }

    [MenuItem("AI Editor/Create Graph")]
    static void CreateGraph()
    {
        string path = EditorUtility.SaveFilePanel("Create AI Graph", 
            "Assets/_Game/AIControllers/", "AIGraph.asset", "asset");
        if (path == "")
            return;

        path = FileUtil.GetProjectRelativePath(path);

        graph = ScriptableObject.CreateInstance<AIBehaviourGraph>();

        AssetDatabase.CreateAsset(graph, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
    }

    [MenuItem("AI Editor/Clear current")]
    static void ClearGraphs()
    {
        if (graph != null)
                graph.Clear();
        //var assets = Selection.GetFiltered(typeof(AIBehaviourGraph), SelectionMode.Assets);
        //foreach (var asset in assets)
        //{
        //    AIBehaviourGraph graph = asset as AIBehaviourGraph;
        //    if (graph != null)
        //        graph.Clear();
        //}
    }

    [MenuItem("AI Editor/AI Serialize Test")]
    static void TestSerialize()
    {
        TestSerializer test = ScriptableObject.CreateInstance<TestSerializer>();
        test.number1 = 42;
        test.nested.number1 = 99;
        TestGeneric(test);
    }


    static void TestGeneric<T>(T test) where T :ScriptableObject
    {
        
        T clone = null;
        try
        {
            clone = ScriptableObject.Instantiate<T>(test);
            clone = ObjectCopier.Clone<T>(test);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        Debug.Log("base" + test.ToString());
        Debug.Log("clone" + clone.ToString());

        ScriptableObject.DestroyImmediate(test);
        ScriptableObject.DestroyImmediate(clone);
    }

    // Whenever a Graph asset is opened, the window initializes with it
    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        AIBehaviourGraph graphSelection = Selection.activeObject as AIBehaviourGraph;
        if (graphSelection == null)
            return false;

        AIEditor window = (AIEditor)EditorWindow.GetWindow(typeof(AIEditor));
        window.Initialize();
        AIEditor.graph = graphSelection;
        return true;
    }

    Texture2D CreateColoredTexture(Color color, int width, int height)
    {
        Texture2D texture = new Texture2D(1, 1);
        color.a = 1;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                texture.SetPixel(i, j, color);
            }
        }
        return texture;
    }
    Texture2D CreateColoredTexture(Color color)
    {
        return CreateColoredTexture(color, 1, 1);
    }

    void UpdateTypes()
    {
        if (AIBehaviourTypes == null)
            AIBehaviourTypes = new List<Type>();
        AIBehaviourTypes.Clear();
        foreach (Type type in
            Assembly.GetAssembly(typeof(AIBehaviour)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(AIBehaviour))))
            AIBehaviourTypes.Add(type);
    }

    public void Initialize()
    {
        editor = this;
        wantsMouseMove = true;
        graph = null;
        graphView = new AIGraphView();
        graphView.Initialize();



        // Create ComboBox

        GUIContent[] comboList = new GUIContent[3];
        comboList[0] = new GUIContent("Create Type A");
        comboList[1] = new GUIContent("Create Type B");
        comboList[2] = new GUIContent("Create Type C");

        listStyle = new GUIStyle();
        listStyle.normal.background = CreateColoredTexture(new Color(0.3f, 0.3f, 0.3f));
        listStyle.onNormal.background = CreateColoredTexture(new Color(0.1f, 0.1f, 0.1f));
        listStyle.onHover = listStyle.normal;
        listStyle.onHover.background = CreateColoredTexture(new Color(0, 0, 0.6f));
        listStyle.onActive.textColor = Color.red;
        listStyle.onHover.textColor = Color.blue;
        listStyle.onNormal.textColor = Color.red;
        listStyle.active.textColor = Color.green;
        listStyle.hover.background = CreateColoredTexture(new Color(0, 0, 0.6f));
        listStyle.padding = new RectOffset(4, 4, 4, 4);


        editorInitialized = true;

        UpdateTypes();
    }

    void OnGUI()
    {
        if (!editorInitialized)
            Initialize();

        e = Event.current;
        //index = EditorGUILayout.Popup(index, options);
        control = new Rect(0, 0, controlWidth, this.position.height);
        graphView.panel = new Rect(controlWidth, 0, this.position.width - controlWidth, this.position.height);

        if (e.isMouse)
        {
            mousePosPrev = mousePos;
            mousePos = Event.current.mousePosition;
            graphView.mousePos = mousePos - graphView.panel.position - graphView.canvas.position;
        }

        // --- Left Panel ---
        GUI.BeginGroup(control);
        DrawControlPane(control);
        GUI.EndGroup();


        // --- Right Panel ---
        graphView.OnGUI();
        ResizeScrollView();


        HandleMouseEvents();

        Repaint();
    }

    bool resizing = false;

    public void BeginEditorWindows()
    {
        BeginWindows();
    }

    public void EndEditorWindows()
    {
        EndWindows();
    }

    private void ResizeScrollView()
    {
        Rect splitter = new Rect(controlWidth - 2.5f, 0, 5.0f, this.position.height);
        GUI.color = Color.white;
        GUI.Box(splitter, GUIContent.none);
        EditorGUIUtility.AddCursorRect(splitter, MouseCursor.ResizeHorizontal);

        if (Event.current.type == EventType.mouseDown && splitter.Contains(Event.current.mousePosition))
            resizing = true;
        if (Event.current.type == EventType.MouseUp)
            resizing = false;

        if (resizing)
            controlWidth = Event.current.mousePosition.x;

        controlWidth = Mathf.Clamp(controlWidth, 100.0f, 400.0f);
    }

    // --------------------
    // --- Control Pane ---
    // --------------------

    void DrawControlPane(Rect panel)
    {
        GUI.color = Color.gray;
        GUI.Box(panel, GUIContent.none);

        Rect panelArea = panel;
        panelArea.xMin += 20;
        panelArea.yMin += 20;
        GUILayout.BeginArea(panelArea);

        if (!graph)
        {
            GUILayout.Label("No controller\nselected");
        }
        else
        {
            GUILayout.Label(graph.name);
            GUILayout.Label("Behaviour Count: " + graph.behaviours.Count);
        }

        GUI.color = new Color(0.7f, 0.7f, 0.7f);

        /*
        int index = comboBox.Show(controlRect);
        controlRect.y += controlRect.height + 20;

        AIBehaviour behaviour = null;
        switch (index)
        {
            case 0:
                behaviour = new PatrolBehaviour();
                break;
        }
        if (behaviour != null)
        {
            behaviour.index = graph.behaviours.Count;
            graph.behaviours.Add(behaviour);
        }
        */
        if (graph)
        {
            if (GUILayout.Button("Change"))
                graph.testString = "Change";
            GUILayout.Label(graph.testString);

            if (GUILayout.Button("Save Asset"))
                SaveAsset();
        }

        GUILayout.EndArea();

    }


    void SaveAsset()
    {
        if (!graph)
            return;
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();

    }
    void HandleMouseEvents()
    {
        if (e.type == EventType.MouseUp && e.button == 1)
        {
            UpdateTypes();
            GenericMenu contextMenu = new GenericMenu();
            foreach (Type type in AIBehaviourTypes)
            {
                contextMenu.AddItem(new GUIContent("Create Behaviour/" + type.Name), false, graphView.CreateNode, new AIGraphView.CreateArg(type, e.mousePosition));

            }
            contextMenu.ShowAsContext();
        }

        if (e.button == 2)
        {
            if (e.type == EventType.MouseDown)
                OnMiddleDown();
            else if (e.type == EventType.MouseDrag)
                OnMiddleDrag();
        }
    }

    void OnMiddleDown()
    {
        mousePosPrev = mousePos;
    }
    void OnMiddleDrag()
    {
        graphView.canvas.center += mousePos - mousePosPrev;

        mousePosPrev = mousePos;
        Event.current.Use();
    }




}
