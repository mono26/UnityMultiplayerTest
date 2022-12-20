using UnityEngine;

/// <summary>
/// This class contains utility methods.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Gets a random spawn point.
    /// </summary>
    /// <returns>A random spawn point.</returns>
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-5, 5), 4, Random.Range(-5, 5));
    }
}
