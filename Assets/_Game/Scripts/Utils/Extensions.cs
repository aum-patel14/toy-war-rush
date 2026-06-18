// TOY WAR RUSH - Extensions.cs
// Common utility extensions.

using UnityEngine;

public static class Extensions
{
    public static void SetLayerRecursively(this GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            child.gameObject.SetLayerRecursively(layer);
    }

    public static Vector3 WithX(this Vector3 v, float x) => new(x, v.y, v.z);
    public static Vector3 WithZ(this Vector3 v, float z) => new(v.x, v.y, z);

    public static int UpgradeCost(int baseCost, int level) =>
        Mathf.RoundToInt(baseCost * Mathf.Pow(1.5f, level));
}
