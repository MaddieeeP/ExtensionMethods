using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
//double
    public static float RadiansToDegrees(this double angle)
    {
        return (float)(angle / Math.PI) * 180f;
    }

//float
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

//List<T>
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

//Vector3
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

//List<Vector3>
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

//Quaternion
    public static Quaternion DivideBy(this Quaternion quaternion, Quaternion divisor)
    {
        return quaternion * Quaternion.Inverse(divisor);
    }

    public static float StandardizedDistance(this Quaternion rotation, Quaternion targetRotation) => StandardizedDistance(rotation.eulerAngles, targetRotation.eulerAngles)

    public static Quaternion RotationComponentAboutAxis(this Quaternion rotation, Vector3 direction)
    {
        Vector3 rotationAxis = new Vector3(rotation.x, rotation.y, rotation.z);
        float dotProd = (float)Vector3.Dot(direction, rotationAxis);
        Vector3 projection = dotProd * direction;

        Quaternion twist = new Quaternion(projection.x, projection.y, projection.z, rotation.w);
        if (dotProd < 0f)
        {
            twist.x = -twist.x;
            twist.y = -twist.y;
            twist.z = -twist.z;
            twist.w = -twist.w;
        }
        return twist;
    }

    public static Vector3 FindClosest(this Quaternion quaternion, List<Quaternion> quaternions)
    {
        float minAngle = float.MaxValue;

        int indexOfBest = 0;

        for (int i = 0; i < quaternions.Count; i++)
        {
            float angle = Vector3.Angle(quaternion * Vector3.forward, quaternions[i] * Vector3.forward);
            if (angle < minAngle)
            {
                indexOfBest = i;
                minAngle = angle;
            }
        }

        return quaternions[indexOfBest];
    }

//GameObject
    public static bool HasComponent<T>(this GameObject obj)
    {
        return (obj.GetComponent<T>() as Component) != null;
    }

//Transform
    public static bool HasComponent<T>(this Transform transform) => transform.gameObject.HasComponent<T>();

    public static float ShortestDistanceToVertex(this Transform transform, Vector3 position)
    {
        float distance = float.MaxValue;

        foreach (Vector3 vertex in transform.GetComponent<MeshFilter>().mesh.vertices)
        {
            float vertexDist = Vector3.Distance(position, vertex + transform.position);
            if (vertexDist < distance)
            {
                distance = vertexDist;
            }
        }

        return distance;
    }

//Rigidbody
    public static Vector3 CalculateForceToReachVelocity(this Rigidbody rigidbody, Vector3 targetVelocity, float deltaTime = 0.01f)
    {
        return (rigidbody.mass * targetVelocity) - (rigidbody.mass * rigidbody.velocity) / deltaTime;
    }
}
