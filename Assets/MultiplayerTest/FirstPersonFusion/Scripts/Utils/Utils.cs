using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomSpawnePoint()
    {
        return new Vector3(Random.Range(-5, 5), 4, Random.Range(-5, 5));
    }
}
