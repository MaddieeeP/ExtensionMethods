using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionMethods
{
    public static Quaternion DivideBy(this Quaternion quaternion, Quaternion divisor)
    {
        return quaternion * Quaternion.Inverse(divisor);
    }

    public static float StandardizedDistance(this Quaternion rotation, Quaternion targetRotation) => rotation.eulerAngles.StandardizedDistance(targetRotation.eulerAngles);

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
}
