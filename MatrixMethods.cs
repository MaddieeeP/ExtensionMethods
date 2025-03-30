using UnityEngine;
using Unity.Mathematics;

public static class MatrixMethods
{
    public static Vector3 SolveLinearEquations(this float3x3 equationsMatrix, Vector3 sums)
    {
        return math.mul(math.inverse(equationsMatrix), sums);
    }
}
