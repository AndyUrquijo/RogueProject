using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class SensorCondition : AICondition {

    public enum Condition
    {
        SENSE_ANY,
    };
    //Condition condition;

    public override bool CheckCondition()
    {
        AIController controller = transition.behaviour.controller;

        //if( condition == Condition.SENSE_ANY)
        {
            foreach (AISensor sensor in controller.sensors)
            {
                GameObject[] sensed = sensor.Detect();
                if (sensed.Length > 0)
                    return true;
            }
        }

        return false;
    }
}
