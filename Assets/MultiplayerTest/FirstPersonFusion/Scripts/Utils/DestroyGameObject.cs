using System.Collections;
using UnityEngine;

/// <summary>
/// This class is responsible for destroying the game object after a certain time.
/// </summary>
public class DestroyGameObject : MonoBehaviour
{
    public float lifeTime = 2f;
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
