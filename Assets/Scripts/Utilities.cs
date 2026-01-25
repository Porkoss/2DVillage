using UnityEngine;

public static class Utilities
{
    public static float Distance2D(Vector3 first, Vector3 second)
    {
        return Mathf.Sqrt(Mathf.Pow((first.x - second.x), 2) + Mathf.Pow((first.y - second.y), 2));
    }

    public static Vector3 To2DVector(Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0.543f);
    }
}
