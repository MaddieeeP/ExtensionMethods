using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RigidbodyMethods
{
    public static Vector3 CalculateForceToReachVelocity(this Rigidbody rigidbody, Vector3 targetVelocity, float deltaTime = 0.01f)
    {
        return (rigidbody.mass * targetVelocity) - (rigidbody.mass * rigidbody.velocity) / deltaTime;
    }
}
