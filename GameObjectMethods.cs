using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectMethods
{
    public static bool HasComponent<T>(this GameObject obj)
    {
        return (obj.GetComponent<T>() as UnityEngine.Component) != null;
    }

    public static void DestroyAllChildren(this GameObject obj) => obj.transform.DestroyAllChildren();    
}
