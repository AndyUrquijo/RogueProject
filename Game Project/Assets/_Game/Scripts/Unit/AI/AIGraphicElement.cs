using UnityEngine;
using System.Collections;

[System.Serializable]
public class AIGraphicElement : ScriptableObject
{
	public void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
	}
}
