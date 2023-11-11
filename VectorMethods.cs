using System;
using System.Collections;
using System.Collections.Generic;
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

    public static Vector3 StandardizeRotation(this Vector3 rotation)
    {
        while (rotation.x <= -180f)
        {
            rotation += new Vector3(360f, 0f, 0f);
        }
        while (rotation.x > 180f)
        {
            rotation -= new Vector3(360f, 0f, 0f);
        }
        while (rotation.y <= -180f)
        {
            rotation += new Vector3(0f, 360f, 0f);
        }
        while (rotation.y > 180f)
        {
            rotation -= new Vector3(0f, 360f, 0f);
        }
        while (rotation.z <= -180f)
        {
            rotation += new Vector3(0f, 0f, 360f);
        }
        while (rotation.z > 180f)
        {
            rotation -= new Vector3(0f, 0f, 360f);
        }

        return rotation;
    }

    public static float StandardizedDistance(this Vector3 rotation, Vector3 targetRotation)
    {
        Vector3 difference = default(Vector3);
        difference.x = rotation.x.StandardizedAxisDistance(targetRotation.x);
        difference.y = rotation.y.StandardizedAxisDistance(targetRotation.y);
        difference.z = rotation.z.StandardizedAxisDistance(targetRotation.z);
        return difference.magnitude;
    }

    public static Vector3 ComponentInDirection(this Vector3 vector, Vector3 direction)
    {
        float angle = (float)Math.Acos(Vector3.Dot(vector, direction) / vector.magnitude / direction.magnitude);
        Vector3 component = direction.normalized * vector.magnitude * (float)Math.Cos(angle);
        if (Double.IsNaN((double)component.magnitude))
        {
            return new Vector3(0f, 0f, 0f);
        }
        return component;
    }

    public static Vector3 RemoveComponentInDirection(this Vector3 vector, Vector3 direction)
    {
        return vector - vector.ComponentInDirection(direction);
    }

    public static Vector3 ToTransDirection(this Vector3 vector, Transform transform)
    {
        Vector3 vectorInDirection = transform.right * vector.x + transform.up * vector.y + transform.forward * vector.z;
        return vectorInDirection;
    }

    public static float AngleFrom(this Vector3 vector, Vector3 direction, bool absolute = false)
    {
        float angle = (Math.Acos(Vector3.Dot(vector, direction) / vector.magnitude / direction.magnitude)).RadiansToDegrees();
        Vector3 perpendicular = vector.RemoveComponentInDirection(direction).normalized;
        if (!absolute && !perpendicular.IsComponentInDirectionPositive(new Vector3(1f, 1f, 1f)))
        {
            return 360f - angle;
        }

        return angle;
    }

    public static Vector3 FlattenAgainstDirection(this Vector3 vector, Vector3 direction)
    {
        float vectorMagnitude = vector.magnitude;
        vector = vector.normalized;
        vector -= vector.ComponentInDirection(direction);
        return (vector.normalized * vectorMagnitude).StandardizeRotation();
    }

    public static bool IsComponentInDirectionPositive(this Vector3 vector, Vector3 direction)
    {
        if (vector.ComponentInDirection(direction).normalized == direction.normalized)
        {
            return true;
        }
        return false;
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
