using System;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionMethods
{
    public static Quaternion DivideBy(this Quaternion quaternion, Quaternion divisor)
    {
        return quaternion * Quaternion.Inverse(divisor);
    }

    public static Quaternion Negate(this Quaternion quaternion) //Same rotation, however the axis and angle are both flipped
    {
        return new Quaternion(-quaternion.x, -quaternion.y, -quaternion.z, -quaternion.w);
    }

    public static Quaternion ShortestRotation(this Quaternion rotation, Quaternion targetRotation)
    {
        if (Quaternion.Dot(rotation, targetRotation) < 0f)
        {
            return targetRotation.DivideBy(rotation.Negate());
        }
        return targetRotation.DivideBy(rotation);
    }

    public static Quaternion RotationComponentAboutAxis(this Quaternion rotation, Vector3 direction)
    {
        return Quaternion.LookRotation((rotation * Vector3.forward).RemoveComponentAlongAxis(direction), direction);
    }

    public static Quaternion FindClosest(this Quaternion quaternion, List<Quaternion> quaternions)
    {
        float minAngle = float.MaxValue;
        Quaternion best = default;

        foreach (Quaternion quaternionOption in quaternions)
        {
            float angle = Vector3.Angle(quaternion * Vector3.forward, quaternionOption * Vector3.forward);
            if (angle < minAngle)
            {
                best = quaternionOption;
                minAngle = angle;
            }
        }

        return best;
    }

    public static List<Quaternion> GetRotationsAroundAxis(this Quaternion quaternion, Vector3 axis, int rotationCount, bool clockwise = true)
    {
        List<Quaternion> rotations = new List<Quaternion>();
        float rotationAngle = 360f / rotationCount;
        
        if (!clockwise)
        {
            rotationAngle = -rotationAngle;
        }

        for (int i = 0; i < rotationCount; i++) 
        {
            rotations.Add(quaternion * Quaternion.AngleAxis(rotationAngle * i, axis));
        }

        return rotations;
    }

    public static Quaternion ClampRotation(this Quaternion quaternion, Vector3 forward, Vector3 up, float maxXRotation, float maxYRotation) //FIX
    {
        Quaternion centralRotation = Quaternion.LookRotation(forward, up);
        Quaternion rotation = centralRotation.ShortestRotation(quaternion);
        Vector3 eulerRotation = rotation.eulerAngles;

        eulerRotation = eulerRotation.y > 180 ? new Vector3(-eulerRotation.x, eulerRotation.y - 360, eulerRotation.z) : eulerRotation;
        eulerRotation = eulerRotation.x > 180 ? new Vector3(eulerRotation.x - 360, eulerRotation.y, eulerRotation.z) : eulerRotation;

        if (Math.Abs(eulerRotation.y) > 90 && Math.Abs(eulerRotation.y) > 90)
        {
            eulerRotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        }
        else if (Math.Abs(eulerRotation.y) > 90)
        {
            eulerRotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        }
        else if (Math.Abs(eulerRotation.y) > 90)
        {
            eulerRotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        }

        Debug.Log(eulerRotation);

        return Quaternion.Euler(Math.Clamp(eulerRotation.x, -maxXRotation, maxXRotation), Math.Clamp(eulerRotation.y, -maxYRotation, maxYRotation), eulerRotation.z) * centralRotation;
    }

    public static Quaternion ClampRotation(this Quaternion quaternion, Vector3 forward, float maxRotation)
    {
        Quaternion centralRotation = Quaternion.LookRotation(forward);
        Quaternion rotation = centralRotation.ShortestRotation(quaternion);
        rotation.ToAngleAxis(out float angle, out Vector3 axis);

        return Quaternion.AngleAxis(Math.Min(0f, maxRotation - angle), axis) * quaternion;
    }
}