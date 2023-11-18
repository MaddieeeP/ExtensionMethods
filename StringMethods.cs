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
}
