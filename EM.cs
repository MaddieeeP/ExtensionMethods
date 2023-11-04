using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EM : MonoBehaviour
{
    public static void DestroyAllChildren(Transform transform)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            Destroy(child.gameObject);
        }
    }
}
