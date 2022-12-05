using UnityEngine;

public static class Transforms
{
    public static void DestroyChildren(this Transform t, bool destryoyImmediatly = false)
    {
        foreach (Transform child in t)
        {
            if (destryoyImmediatly)
                MonoBehaviour.DestroyImmediate(child.gameObject);
            else
                MonoBehaviour.Destroy(child.gameObject);
        }
    }
}