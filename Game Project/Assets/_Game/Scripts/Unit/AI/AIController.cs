using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class AIController : AbstractBehaviour
{
    public List<AIBehaviour> behaviours = new List<AIBehaviour>();
    public AIBehaviourGraph graph;

    public AIBehaviour currentBehaviour;
    [HideInInspector]
    public Waypoint[] waypoints;
    public AISensor[] sensors;

    public bool started = false;
    public string current;

    public void StartController()
    {
        if(!started) 
            Start();
    }

    public void AddBehaviour(AIBehaviour behaviour)
    {
        behaviour.index = behaviours.Count;
        behaviour.SetGraphicNode(new Vector2(50, 50));
        behaviours.Add(behaviour);
         

    }

    void Start()
    {
        graph.LoadBehaviours(ref behaviours);

        foreach (AIBehaviour behaviour in behaviours)
        {
            behaviour.Link(this);
            behaviour.Initialize();
        }
        currentBehaviour = behaviours[0];

        waypoints = GetComponentsInChildren<Waypoint>();
        sensors = GetComponentsInChildren<AISensor>();
        started = true;
    }

    void Update()
    {
        int transitionIndex = currentBehaviour.CheckTransitions();
        currentBehaviour = behaviours[transitionIndex];
        currentBehaviour.Update();
        current = currentBehaviour.Name;
    }
}
