using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public static class FloatMethods
{
    public static float Squared(this float num)
    {
        return num * num;
    }

    public static float StandardizedAxisDistance(this float rotation, float targetRot)
    {
        float difference = 180f;
        difference = Mathf.Min(difference, (float)Math.Abs(targetRot - rotation));
        difference = Mathf.Min(difference, (float)Math.Abs(targetRot - (rotation - 360f)));
        difference = Mathf.Min(difference, (float)Math.Abs(targetRot - (rotation + 360f)));
        return difference;
    }
}