using UnityEngine;
using System.Collections;


[System.Serializable]
public class TestSerializer : ScriptableObject
{
    public int number1;
    int number2;

    [SerializeField]
    private int number3;

    public NestedTestSerializer nested = new DerivedNestedTestSerializer();

    public override string ToString()
    {
        return "(" + number1 + "," + number2 + "," + number3 + "," + nested.ToString() + ")";
    }
}

public class NestedTestSerializer
{
    public int number1 = 3;

}

public class DerivedNestedTestSerializer : NestedTestSerializer
{
    public int number2 = 5;

    public override string ToString()
    {
        return "(" + number1 + "," + number2 + ")";
    }
}