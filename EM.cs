using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EM : MonoBehaviour
{
    public static void DestroyAllChildren(Transform transform)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>(true);

        for (int i = 1; i < children.Length; i++)
        {
            Destroy(children[i].gameObject);
        }
    }

    public static void DestroyImmediateAllChildren(Transform transform) //FIX
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
