using UnityEngine;
using System.Collections;
using UnityEditor;

class MakeEmptyParent
{
    [MenuItem("GameObject/Make empty Parent")]
    static void Make()
    {
        
        Transform grandparent = Selection.transforms[0].parent;
        GameObject parentObj = new GameObject();
        Transform parent = parentObj.transform;
        parent.name = Selection.activeTransform.name;
        if (grandparent)
            parent.parent = grandparent;

        foreach (Transform transform in Selection.transforms)
        {
            transform.parent = parent;
        }

        Selection.activeTransform = parent;
    }

    //verifies that the selection is a non-zero array of siblings
    [MenuItem("GameObject/Make empty Parent", true)]
    static bool Check()
    {
        if (Selection.transforms.Length == 0)
            return false;

        Transform parent = Selection.transforms[0].parent;

        foreach (Transform transform in Selection.transforms)
        {
            if (!Transform.ReferenceEquals(transform.parent, parent))
                return false;
        }
        return true;
    }
}
