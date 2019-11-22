
using UnityEngine;

public static class Vectorf
{
    public static Vector2 Round(Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));

    public static Vector3 Round(Vector3 v) =>
        new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
}
