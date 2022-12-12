#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

#if GAME_CLIENT
using UnityEngine;

namespace MultiplayerTest
{
    public class GameClientFactory
    {
        public GameClient CreateInstance()
        {
            GameClient clientPFB = Resources.Load<GameClient>($"PFB_{nameof(GameClient)}");
            if (clientPFB != null) {
                return GameObject.Instantiate(clientPFB);
            }

            throw new System.Exception($"{nameof(GameClient)} not found in resources folder.");
        }
    }
}
#endif