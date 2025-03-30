using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColliderMethods
{
    public static void UpdateMesh(this MeshCollider meshCollider, SkinnedMeshRenderer skinnedMeshRenderer)
    {
        Mesh mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
    }
}