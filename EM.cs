using UnityEngine;

public class EM : MonoBehaviour
{
    public static void DestroyAllChildren(Transform transform)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>(true);

        for (int i = children.Length - 1; i >= 0; i--)
        {
            Destroy(children[i].gameObject);
        }
    }

    public static void DestroyImmediateAllChildren(Transform transform) //FIX - test
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>(true);

        for (int i = children.Length - 1; i >= 0; i--)
        {
            DestroyImmediate(children[i].gameObject);
        }
    }
}