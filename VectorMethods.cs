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

    public static Vector3 ClampRelativeChange(this Vector3 vector, Vector3 relativeVector, float increaseMax, float decreaseMax, out Vector3 vectorPerpendicular)
    {
        if (relativeVector == Vector3.zero) //relativeVector has no meaningful direction
        {
            vectorPerpendicular = default;
            return Vector3.ClampMagnitude(vector, increaseMax);
        }

        Vector3 vectorParallel = vector.ComponentAlongAxis(relativeVector);
        vectorPerpendicular = vector - vectorParallel;

        if (vectorParallel.SignedMagnitudeInDirection(relativeVector) > 0f) //vectorParallel is in same direction as relativeVector
        {
            return Vector3.ClampMagnitude(vectorParallel, increaseMax);
        }
        
        Vector3 decreaseVector = vectorParallel.normalized * Math.Min(relativeVector.magnitude, vectorParallel.magnitude);
        Vector3 increaseVector = vectorParallel - decreaseVector; //will always have signedMagnitudeInDirection(vectorParallel) >= 0

        return Vector3.ClampMagnitude(decreaseVector, decreaseMax) + Vector3.ClampMagnitude(increaseVector, increaseMax);
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

    public static Vector3 ClampInBounds(this Vector3 vector, Vector3 a, Vector3 b)
    {
        Vector3 clampedVector = default;
        for (int i = 0; i < 3; i++)
        {
            clampedVector[i] = Math.Clamp(vector[i], Math.Min(a[i], b[i]), Math.Max(a[i], b[i]));
        }

        return clampedVector;
    }

    public static Vector3 ClampInBounds(this Vector3 vector, Bounds bounds) => vector.ClampInBounds(bounds.min, bounds.max);

    public static Vector3 ClampInDirection(this Vector3 vector, Vector3 clampVector, out Vector3 vectorPerpendicular)
    { 
        Vector3 vectorParallel = vector.ComponentAlongAxis(clampVector);
        vectorPerpendicular = vector - vectorParallel;

        if (vectorParallel.SignedMagnitudeInDirection(clampVector) > 0f)
        {
            return Vector3.ClampMagnitude(vectorParallel, clampVector.magnitude);
        }

        return vectorParallel;
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

    public static float LineOfSight(this Vector3 forward, Vector3 position, Vector3 direction, Vector3 up, List<Transform> ignoreTransforms, float maxDistance = Mathf.Infinity, float maxViewDegreesX = 90f, float maxViewDegreesY = 90f, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        if (!forward.FacingDirection(direction, up, maxViewDegreesX, maxViewDegreesY))
        {
            return -1f;
        }

        RaycastHit[] hits = Physics.RaycastAll(position, direction.normalized, maxDistance, layerMask, queryTriggerInteraction);
        hits = hits.OrderBy(hit => hit.distance).ToArray();

        foreach (RaycastHit hit in hits)
        {
            if (ignoreTransforms.Contains(hit.transform))
            {
                continue;
            }

            return hit.distance;
        }
        return maxDistance;
    }

    public static float3 ToFloat3(this Vector3 vector)
    {
        return new float3(vector.x, vector.y, vector.z);
    }
}