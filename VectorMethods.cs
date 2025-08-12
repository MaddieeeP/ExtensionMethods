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
            return new Vector2(value, vector.y);
        }

        return new Vector2(vector.x, value);
    }

    public static Vector3 ChangeValue(this Vector3 vector, int index, float value)
    {
        if (index == 0)
        {
            return new Vector3(value, vector.y, vector.z);
        }
        if (index == 1)
        {
            return new Vector3(vector.x, value, vector.z);
        }

        return new Vector3(vector.x, vector.y, value);
    }

    public static Vector3 ComponentAlongAxis(this Vector3 vector, Vector3 axis)
    {
        return axis.normalized * Vector3.Dot(vector, axis.normalized);
    }

    public static Vector3 RemoveComponentAlongAxis(this Vector3 vector, Vector3 axis)
    {
        return vector - vector.ComponentAlongAxis(axis);
    }

    public static Vector3 FlattenAgainstAxis(this Vector3 vector, Vector3 axis)
    {
        return vector.magnitude * vector.RemoveComponentAlongAxis(axis).normalized;
    }

    public static float SignedMagnitudeInDirection(this Vector3 vector, Vector3 direction)
    {
        return Vector3.Dot(vector, direction.normalized);
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

    public static bool IsComponentInDirectionPositive(this Vector3 vector, Vector3 direction)
    {
        return vector.SignedMagnitudeInDirection(direction) > 0f;
    }

    public static Vector3 ClampInBounds(this Vector3 vector, Vector3 min, Vector3 max)
    {
        Vector3 clampedVector = default;
        for (int i = 0; i < 3; i++)
        {
            clampedVector[i] = Math.Clamp(vector[i], min[i], max[i]);
        }

        return clampedVector;
    }

    public static Vector3 ClampInBounds(this Vector3 vector, Bounds bounds) => vector.ClampInBounds(bounds.min, bounds.max);

    public static Vector3 GetClosestPointOnLine(this Vector3 point, Vector3 direction, Vector3 linePoint)
    {
        direction.Normalize();
        var transformedPoint = point - linePoint;
        var t = Vector3.Dot(transformedPoint, direction);
        return linePoint + direction * t;
    }

    public static Vector3 GetClosestPointOnLineSegment(this Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        Vector3 direction = (linePoint2 - linePoint1).normalized;
        var transformedPoint = point - linePoint1;
        var t = Mathf.Clamp(Vector3.Dot(transformedPoint, direction), 0f, Vector3.Distance(linePoint1, linePoint2));
        return linePoint1 + direction * t;
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

    public static bool FacingDirection(this Vector3 forward, Vector3 direction, Vector3 up, float maxViewDegreesX = 90f, float maxViewDegreesY = 90f)
    {
        forward = forward.normalized;
        direction = direction.normalized;
        up = up.normalized;

        float xDegrees = Vector3.Angle(direction.RemoveComponentAlongAxis(up), forward);
        float yDegrees = Vector3.Angle(direction.RemoveComponentAlongAxis(up), direction);

        if (xDegrees > maxViewDegreesX || yDegrees > maxViewDegreesY)
        {
            xDegrees = 180f - xDegrees;
            yDegrees = 180f - yDegrees;
        }

        if (xDegrees <= maxViewDegreesX && yDegrees <= maxViewDegreesY)
        {
            return true;
        }

        return false;
    }

    public static bool CanBeSeen(this Vector3 target, Vector3 position, Vector3 forward, Vector3 up, List<Transform> ignoreTransforms, float maxViewDegreesX = 90f, float maxViewDegreesY = 90f, float maxDistance = float.PositiveInfinity, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        Vector3 positionToTarget = target - position;
        Vector3 direction = positionToTarget.normalized;
        float distance = positionToTarget.magnitude;

        if (distance > maxDistance || !forward.FacingDirection(direction, up, maxViewDegreesX, maxViewDegreesY))
        {
            return false;
        }

        RaycastHit[] hits = Physics.RaycastAll(position, direction, distance, layerMask, queryTriggerInteraction);
        hits = hits.OrderBy(hit => hit.distance).ToArray();

        foreach (RaycastHit hit in hits)
        {
            if (ignoreTransforms.Contains(hit.transform))
            {
                continue;
            }

            return false;
        }
        return true;
    }

    public static float3 ToFloat3(this Vector3 vector)
    {
        return new float3(vector.x, vector.y, vector.z);
    }

    public static Vector3 GetBarycentricCoordinate(this Vector3 point, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        float3x3 equationMatrix = new float3x3(vertex1.x, vertex1.y, vertex1.z, vertex2.x, vertex2.y, vertex2.z, vertex3.x, vertex3.y, vertex3.z);

        return equationMatrix.SolveLinearEquations(point);
    }

    public static bool IsValidBarycentricCoordinate(this Vector3 barycentricCoordinate)
    {
        return barycentricCoordinate.x >= 0f && barycentricCoordinate.y >= 0f && barycentricCoordinate.z >= 0f && barycentricCoordinate.x + barycentricCoordinate.y + barycentricCoordinate.z == 1f;
    }

    public static Vector3 PointOnTriangle(this Vector3 barycentricCoordinate, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        return vertex1 * barycentricCoordinate.x + vertex2 * barycentricCoordinate.y + vertex3 * barycentricCoordinate.z;
    }
}