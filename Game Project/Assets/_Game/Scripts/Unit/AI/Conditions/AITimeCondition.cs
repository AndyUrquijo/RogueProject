using UnityEngine;
using System.Collections;

public class AITimeCondition : AICondition
{
	public float Duration = 2.0f;

	TimeSensor timeSensor;

	override public bool CheckCondition()
	{
		if( timeSensor == null)
			timeSensor = transition.behaviour.controller.GetComponent<TimeSensor>();

		if( timeSensor.Value > Duration )
		{
			timeSensor.Reset();
			return true;
		}
		else return false;
	}



}
