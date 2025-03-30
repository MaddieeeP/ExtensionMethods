using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayMethods
{
    public static bool IntersectsPlane(this Ray ray, out Vector3 intersection, Vector3 planePoint, Vector3 normal, float maxDistance)
    {
        intersection = default;

        float directionDotProduct = Vector3.Dot(ray.direction, normal);

        //if (directionDotProduct == 0.0f && (planePoint - origin).ComponentAlongAxis(normal) == Vector3.zero) //ray lies on plane

        if (directionDotProduct >= 0.0f) //ray travels away from plane or hits back face
        {
            return false;
        }

        float length = Vector3.Dot((planePoint - ray.origin), normal) / directionDotProduct;

        if (length > maxDistance) 
        {
            return false;
        }

        intersection = ray.origin + ray.direction * length;

        return true;
    }
}
