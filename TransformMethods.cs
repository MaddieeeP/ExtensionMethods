using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformMethods
{
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

    public static void SetScale(this Transform transform, Vector3 scale)
    {
        transform.localScale = Vector3.one; //reset local scale so that lossyScale represents the parent's scale (or default scale if parent is null)
        transform.localScale = new Vector3(scale.x / transform.lossyScale.x, scale.y / transform.lossyScale.y, scale.z / transform.lossyScale.z);
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        EM.DestroyAllChildren(transform);
    }

    public static void DestroyImmediateAllChildren(this Transform transform)
    {
        EM.DestroyImmediateAllChildren(transform);
    }

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

    public static Bounds GetBoundsWithChildren(this RectTransform element, List<GameObject> ignoreObjects, Quaternion relativeRotation, string ignoreString = "IGNOREBOUNDS")
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
            if (child.gameObject.name.Contains(ignoreString) || ignoreObjects.Contains(child.gameObject))
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

    public static float LineOfSight(this Transform transform, Vector3 direction, List<Transform> ignoreTransforms, float maxDistance = Mathf.Infinity, float maxViewDegreesX = 90f, float maxViewDegreesY = 90f, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore) => transform.forward.LineOfSight(transform.position, direction, transform.up, ignoreTransforms, maxDistance, maxViewDegreesX, maxViewDegreesY, layerMask, queryTriggerInteraction);
}