#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using UnityEngine;

namespace MultiplayerTest
{
    public class BallSpawner : SLGNetworkBehaviour
    {
        [SerializeField]
        private NetworkPrefabRef ballPrefab;

        private float spawnTime = 3.0f;
        private float currentTimmer = 0.0f;

        private void Start()
        {
            this.BeginPlay();
        }

#if GAME_CLIENT
        private void Update()
        {
            if (this.currentTimmer > 0.0f) {
                this.currentTimmer -= Time.fixedDeltaTime;
                return;
            }

            this.OnInteracted();

            this.currentTimmer = this.spawnTime;
        }
#endif

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.currentTimmer = this.spawnTime;
        }

        public void OnInteracted()
        {
            this.RPC_OnInteracted();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_OnInteracted()
        {
#if GAME_SERVER
            GameServer server = AppWrapper.Instance.AppReference.GameServer;
            server.ServerSpawn(this.ballPrefab, this.transform.position + new Vector3(0, 5, 0), Quaternion.identity, null);
#endif
        }
    }
}