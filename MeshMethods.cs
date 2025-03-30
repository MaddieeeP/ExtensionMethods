using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class MeshMethods
{
    public static float GetForwardPointOnTriangleEdge(this Mesh mesh, int triangleIndex, Vector3 position, Vector3 forward)
    {
        int[] triangles = mesh.triangles;
        //return float with int part reflecting which edge and fraction part reflecting portion along edge
        return 0f;
    }

    public static bool TriangleContainsVertex(this Mesh mesh, int triangleIndex, int vertex)
    {
        for (int i = triangleIndex * 3; i <= triangleIndex * 3 + 2; i ++)
        {
            if (mesh.triangles[i] == vertex)
            {
                return true;
            }
        }

        return false;
    }

    public static List<int> GetConnectedTriangles(this Mesh mesh, int vertex)
    {
        List<int> connectedTriangles = new List<int>();

        for (int i = 0; i <= mesh.triangles.Length / 3; i++) 
        { 
            if (mesh.TriangleContainsVertex(i, vertex))
            {
                connectedTriangles.Add(i);
            }
        }

        return connectedTriangles;
    }

    public static List<int> GetConnectedTriangles(this Mesh mesh, int vertex1, int vertex2, int limit = 2)
    {
        List<int> connectedTriangles = new List<int>();

        for (int i = 0; i <= mesh.triangles.Length / 3; i++)
        {
            if (mesh.TriangleContainsVertex(i, vertex1) && mesh.TriangleContainsVertex(i, vertex2))
            {
                connectedTriangles.Add(i);
            }

            if (connectedTriangles.Count >= limit)
            {
                break;
            }
        }

        return connectedTriangles;
    }

    public static bool Raycast(this Mesh mesh, out int triangleIndex, out Vector3 point, out Vector3 barycentricCoordinate, Ray ray, float maxDistance = Mathf.Infinity)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        Vector3 p1;
        Vector3 p2;
        Vector3 p3;

        for (triangleIndex = 0; triangleIndex < mesh.triangles.Length / 3; triangleIndex++)
        {
            p1 = vertices[triangles[triangleIndex * 3]];

            if (ray.IntersectsPlane(out point, p1, mesh.GetFaceNormal(triangleIndex), maxDistance))
            {
                p2 = vertices[triangles[triangleIndex * 3 + 1]];
                p3 = vertices[triangles[triangleIndex * 3 + 2]];

                barycentricCoordinate = point.GetBarycentricCoordinate(p1, p2, p3);

                if (barycentricCoordinate.IsValidBarycentricCoordinate())
                {
                    return true;
                }
            }
        }

        triangleIndex = -1;
        point = default;
        barycentricCoordinate = default;

        return false;
    }

    public static Vector3 GetApproximateNormal(this Mesh mesh, int triangleIndex)
    {
        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;

        Vector3 n1 = normals[triangles[triangleIndex * 3]];
        Vector3 n2 = normals[triangles[triangleIndex * 3 + 1]];
        Vector3 n3 = normals[triangles[triangleIndex * 3 + 2]];

        return (n1 + n2 + n3) / 3;
    }

    public static Vector3 GetFaceNormal(this Mesh mesh, int triangleIndex, Vector3 approximateNormal)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        Vector3 p1 = vertices[triangles[triangleIndex * 3]];
        Vector3 p2 = vertices[triangles[triangleIndex * 3 + 1]];
        Vector3 p3 = vertices[triangles[triangleIndex * 3 + 2]];
        Vector3 normal = Vector3.Cross(p1 - p2, p2 - p3).normalized;

        return Vector3.Dot(approximateNormal, normal) < 0 ? -normal : normal;
    }

    public static Vector3 GetFaceNormal(this Mesh mesh, int triangleIndex)
    {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        Vector3 p1 = vertices[triangles[triangleIndex * 3]];
        Vector3 p2 = vertices[triangles[triangleIndex * 3 + 1]];
        Vector3 p3 = vertices[triangles[triangleIndex * 3 + 2]];

        Vector3 n1 = normals[triangles[triangleIndex * 3]];
        Vector3 n2 = normals[triangles[triangleIndex * 3 + 1]];
        Vector3 n3 = normals[triangles[triangleIndex * 3 + 2]];

        Vector3 normal = Vector3.Cross(p1 - p2, p2 - p3).normalized;

        return Vector3.Dot((n1 + n2 + n3) / 3, normal) < 0 ? -normal : normal;
    }

    public static Vector3 GetSmoothNormal(this Mesh mesh, int triangleIndex, Vector3 barycentricCoordinate)
    {
        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;

        Vector3 n1 = normals[triangles[triangleIndex * 3]];
        Vector3 n2 = normals[triangles[triangleIndex * 3 + 1]];
        Vector3 n3 = normals[triangles[triangleIndex * 3 + 2]];

        return n1 * barycentricCoordinate.x + n2 * barycentricCoordinate.y + n3 * barycentricCoordinate.z;
    }
}