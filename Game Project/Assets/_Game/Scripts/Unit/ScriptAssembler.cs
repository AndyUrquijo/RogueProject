using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptAssembler : MonoBehaviour
{

    public List<GameObject> prefabs;

    class TypeA
    {
        public int a = 1;

    };

    class TypeB : TypeA
    {
        public int b = 2;
    }

    void Awake()
    {
        foreach (GameObject prefab in prefabs)
        {
            MonoBehaviour[] originals = prefab.GetComponents<MonoBehaviour>();
            List<MonoBehaviour> copies = new List<MonoBehaviour>();

            foreach (MonoBehaviour original in originals)
            {
                System.Type type = original.GetType();
                MonoBehaviour copy = (MonoBehaviour)gameObject.AddComponent(type);
                copies.Add(copy);

                //Debug.Log("Type: " + type.Name + "-----------");


               // Debug.Log("Fields:");
                System.Reflection.BindingFlags fieldFlags =
                    System.Reflection.BindingFlags.CreateInstance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.SetField |
                    System.Reflection.BindingFlags.GetField |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance;
                System.Reflection.FieldInfo[] fields = type.GetFields(fieldFlags);
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    //Debug.Log(field.Name);
                    field.SetValue(copy, field.GetValue(original));
                }

                /*
                Debug.Log("Properties:");
                System.Reflection.BindingFlags propFlags =
                    System.Reflection.BindingFlags.CreateInstance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.SetProperty |
                    System.Reflection.BindingFlags.GetProperty |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance;

                System.Reflection.PropertyInfo[] properties = type.GetProperties(propFlags);
                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    Debug.Log(property.Name);
                    if(property.CanWrite && property.CanRead)
                        property.SetValue(copy, property.GetValue(original, null), null);
                }
                */
                copy.enabled = original.enabled;
            }

        }

        AbstractBehaviour[] absBehaviours = GetComponents<AbstractBehaviour>();
        foreach (AbstractBehaviour absBehaviour in absBehaviours)
            absBehaviour.Initialize();
    }

}
