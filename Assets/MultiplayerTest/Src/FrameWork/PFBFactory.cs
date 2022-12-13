using System;
using UnityEngine;

namespace SLGFramework
{
    public class PFBFactory<T> where T : MonoBehaviour
    {
        public T CreateInstance(Transform parent, Vector3 worldPosition, Quaternion rotation)
        {
            Type tType = typeof(T);

            T pfb = Resources.Load<T>($"PFB_{tType.Name}");
            if (pfb != null) {
                return GameObject.Instantiate(pfb, worldPosition, rotation, parent) as T;
            }

            throw new System.Exception($"{tType.Name} not found in resources folder.");
        }
    }
}