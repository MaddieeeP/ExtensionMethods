using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        if ((oldIndex == newIndex) || (0 > oldIndex) || (oldIndex >= list.Count) || (0 > newIndex) || (newIndex >= list.Count))
        {
            return;
        }

        var i = 0;
        T temp = list[oldIndex];

        if (oldIndex < newIndex)
        {
            for (i = oldIndex; i < newIndex; i++)
            {
                list[i] = list[i + 1];
            }
        }
        else
        {
            for (i = oldIndex; i > newIndex; i--)
            {
                list[i] = list[i - 1];
            }
        }

        list[newIndex] = temp;
    }

    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T temp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = temp;
    }

    public static void SetValueBelowIndex<T>(this List<T> list, T value, int index)
    {
        while (index < 0)
        {
            index += list.Count;
        }

        if (index >= list.Count)
        {
            index = list.Count - 1;
        }

        for (int i = 0; i < index; i++)
        {
            list[i] = value;
        }
    }

    public static void SetValueAtOrAboveIndex<T>(this List<T> list, T value, int index)
    {
        while (index < 0)
        {
            index += list.Count;
        }

        if (index >= list.Count)
        {
            return;
        }

        for (int i = index; i < list.Count; i++)
        {
            list[i] = value;
        }
    }
}
