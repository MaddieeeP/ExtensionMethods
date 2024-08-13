using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringMethods
{
    public static string Truncate(this string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str)) return str;

        return str.Substring(0, Math.Min(str.Length, maxLength));
    }

    public static string RemoveFirstOccurence(this string str, string substring)
    {
        int index = str.IndexOf(substring);
        return (index < 0) ? str : str.Remove(index, substring.Length);
    }
}
