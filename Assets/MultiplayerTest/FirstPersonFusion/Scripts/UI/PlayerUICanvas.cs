using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PlayerUICanvas : MonoBehaviour
{
    NetworkRunner networkRunner;
    Spawner spawner;
    public NetworkPlayer networkPlayer;

    void Start()
    {
        networkRunner = FindObjectOfType<NetworkRunner>();        
        spawner = FindObjectOfType<Spawner>();
    }

    public void OnClickDisconnect()
    {
        networkRunner.Despawn(networkPlayer.GetComponent<NetworkObject>());
        SceneManager.LoadScene("MainMenu");
        networkRunner.Shutdown();
    }
}
