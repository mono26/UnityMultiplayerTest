using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InstantiatePrefabs()
    {
        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("Prefabs");

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            GameObject.Instantiate(prefab);
        }
    }
}
