using System.Collections;
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

    public static Quaternion OldRotationComponentAboutAxis(this Quaternion rotation, Vector3 direction)
    {
        direction = direction.normalized;
        Vector3 rotationAxis = new Vector3(rotation.x, rotation.y, rotation.z).normalized;
        float dotProd = (float)Vector3.Dot(direction, rotationAxis);
        Vector3 projection = dotProd * direction;

        Quaternion twist = new Quaternion(projection.x, projection.y, projection.z, rotation.w);
        if (dotProd < 0f)
        {
            return twist.Negate();
        }
        return twist;
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
}
