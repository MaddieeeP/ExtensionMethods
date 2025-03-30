using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayMethods
{
    public static T[] Fill<T>(T value, int length)
    {
        T[] newArray = new T[length];

        for (int i = 0; i < length; i++)
        {
            newArray[i] = value;
        }

        return newArray;
    }
}