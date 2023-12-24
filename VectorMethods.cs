using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public static class VectorMethods
{
    public static Vector2 ChangeValue(this Vector2 vector, int index, float value)
    {
        if (index == 0)
        {
            vector = new Vector2(value, vector.y);
        }
        else
        {
            vector = new Vector2(vector.x, value);
        }

        return vector;
    }

    public static Vector3 ChangeValue(this Vector3 vector, int index, float value)
    {
        if (index == 0)
        {
            vector = new Vector3(value, vector.y, vector.z);
        }
        else if (index == 1)
        {
            vector = new Vector3(vector.x, value, vector.z);
        }
        else
        {
            vector = new Vector3(vector.x, vector.y, value);
        }

        return vector;
    }

    public static Vector3 ComponentInDirection(this Vector3 vector, Vector3 direction)
    {
        float angle = (float)Math.Acos(Vector3.Dot(vector, direction) / vector.magnitude / direction.magnitude);
        Vector3 component = direction.normalized * vector.magnitude * (float)Math.Cos(angle);
        if (Double.IsNaN((double)component.magnitude))
        {
            return default;
        }
        return component;
    }

    public static Vector3 RemoveComponentInDirection(this Vector3 vector, Vector3 direction)
    {
        return vector - vector.ComponentInDirection(direction);
    }

    public static Vector3 ToTransDirection(this Vector3 vector, Transform transform)
    {
        return transform.right * vector.x + transform.up * vector.y + transform.forward * vector.z;
    }

    public static Vector3 ScaleBy(this Vector3 vector, Vector3 scale)
    {
        return new Vector3(vector.x * scale.x, vector.y * scale.y, vector.z * scale.z);
    }

    public static Vector3 Reciprocal(this Vector3 vector)
    {
        return new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
    }

    public static float SignedAngle(this Vector3 normal, Vector3 a, Vector3 b)
    {
        return Vector3.Angle(a, b) * Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a, b)));
    }

    public static Vector3 FlattenAgainstDirection(this Vector3 vector, Vector3 direction)
    {
        return vector.magnitude * vector.RemoveComponentInDirection(direction).normalized;
    }

    public static bool IsComponentInDirectionPositive(this Vector3 vector, Vector3 direction)
    {
        if (vector.ComponentInDirection(direction).normalized == direction.normalized)
        {
            return true;
        }
        return false;
    }

    public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
    {
        Vector3 clampedVector = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            clampedVector[i] = Math.Clamp(vector[i], min[i], max[i]);
        }
        return clampedVector;
    }

    public static Vector3 SetMagnitude(this Vector3 vector, float magnitude)
    {
        return vector.normalized * magnitude;
    }

    public static Vector3 Average(this List<Vector3> list)
    {
        Vector3 average = new Vector3(0f, 0f, 0f);

        foreach (Vector3 vector in list)
        {
            average += vector;
        }
        return average / list.Count;
    }

    public static float3 ToFloat3(this Vector3 vector)
    {
        return new float3(vector.x, vector.y, vector.z);
    }

    public static List<Vector3> OrderByXYArgument(this List<Vector3> list)
    {
        List<float> arguments = new List<float>();
        List<Vector3> sortedList = new List<Vector3>();

        foreach (Vector3 vector in list)
        {
            float argument = (float)Math.Abs(Math.Atan(vector.y / vector.x));
            if (vector.y < 0f)
            {
                argument = 2f * (float)Math.PI - argument;
            }

            bool sorted = false;
            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i] > argument)
                {
                    arguments.Insert(i, argument);
                    sortedList.Insert(i, vector);
                    sorted = true;
                    break;
                }
            }

            if (!sorted)
            {
                arguments.Add(argument);
                sortedList.Add(vector);
            }
        }
        return sortedList;
    }
}
