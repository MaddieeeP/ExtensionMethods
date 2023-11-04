using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

//Vector2
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

//Vector3
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

    public static float StandardizedDistance(this Quaternion rotation, Quaternion targetRotation) => StandardizedDistance(rotation.eulerAngles, targetRotation.eulerAngles);

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

    public static int FindClosest(this Quaternion quaternion, List<Quaternion> quaternions)
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

        return indexOfBest;
    }

//GameObject
    public static bool HasComponent<T>(this GameObject obj)
    {
        return (obj.GetComponent<T>() as UnityEngine.Component) != null;
    }

    public static void DestroyAllChildren(this GameObject obj) => obj.transform.DestroyAllChildren();

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

    public static void DestroyAllChildren(this Transform transform)
    {
        EM.DestroyAllChildren(transform);
    }

//List<Transform>
    public static List<Vector3> GetPositions(this List<Transform> list)
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (Transform transform in list) 
        { 
            positions.Add(transform.position);
        }

        return positions;
    }

    public static List<Quaternion> GetRotations(this List<Transform> list)
    {
        List<Quaternion> rotations = new List<Quaternion>();

        foreach (Transform transform in list)
        {
            rotations.Add(transform.rotation);
        }

        return rotations;
    }

//RectTransform
    public static Bounds GetBounds(this RectTransform element, Quaternion relativeRotation)
    {
        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        Vector3[] corners = new Vector3[4];

        element.GetWorldCorners(corners);

        foreach (Vector3 corner in corners)
        {
            Vector3 transformedCorner = Quaternion.Inverse(relativeRotation) * (corner - element.position) + element.position;
            min = Vector3.Min(min, transformedCorner);
            max = Vector3.Max(max, transformedCorner);
        }

        min -= element.position;
        max -= element.position;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        return bounds;
    }

    public static Bounds GetBoundsWithChildren(this RectTransform element, List<GameObject> ignoreObjects, Quaternion relativeRotation)
    {
        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        Vector3[] corners = new Vector3[4];

        if (!ignoreObjects.Contains(element.gameObject))
        {
            element.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
            {
                Vector3 transformedCorner = Quaternion.Inverse(relativeRotation) * (corner - element.position) + element.position;
                min = Vector3.Min(min, transformedCorner);
                max = Vector3.Max(max, transformedCorner);
            }
        }

        foreach (RectTransform child in element.GetComponentsInChildren<RectTransform>())
        {
            if (child.gameObject.name.StartsWith("IGNOREBOUNDS") || ignoreObjects.Contains(child.gameObject))
            {
                continue;
            }

            child.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
            {
                Vector3 transformedCorner = Quaternion.Inverse(relativeRotation) * (corner - element.position) + element.position;
                min = Vector3.Min(min, transformedCorner);
                max = Vector3.Max(max, transformedCorner);
            }
        }

        if (min == Vector3.positiveInfinity)
        {
            min = new Vector3(0f, 0f, 0f);
        }
        if (max == Vector3.negativeInfinity)
        {
            max = new Vector3(0f, 0f, 0f);
        }

        min -= element.position;
        max -= element.position;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        return bounds;
    }

//Rigidbody
    public static Vector3 CalculateForceToReachVelocity(this Rigidbody rigidbody, Vector3 targetVelocity, float deltaTime = 0.01f)
    {
        return (rigidbody.mass * targetVelocity) - (rigidbody.mass * rigidbody.velocity) / deltaTime;
    }
}
