using UnityEngine;

namespace SLGFramework
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Finds a Transform with the given name inside the complete child hierarchy, if any. Can return null. 
        /// </summary>
        /// <param name="transform">This transform.</param>
        /// <param name="name">Name of the child.</param>
        /// <returns></returns>
        public static Transform FindRecursive(this Transform transform, string name)
        {
            if (transform.gameObject.name.Equals(name)) {
                return transform;
            }

            Transform transformToReturn = null;
            foreach (Transform child in transform) {
                if (child.gameObject.name.Equals(name)) {
                    return child;
                }
                
                transformToReturn = child.FindRecursive(name);
                if (transformToReturn != null) {
                    break;
                }
            }

            return transformToReturn;
        }
    }
}