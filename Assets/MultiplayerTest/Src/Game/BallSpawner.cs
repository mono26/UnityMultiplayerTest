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

        private InteractableComponent interactableComponent = null;

        private void Awake()
        {
            this.OnInitialize();
        }

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.interactableComponent = this.GetComponent<InteractableComponent>();
            this.interactableComponent.OnTriggerInteraction.AddListener(this.OnInteractedWithComponent);
        }

        public void OnInteractedWithComponent(InteractableComponent component)
        {
            if (component == null || this.interactableComponent != component) {
                return;
            }

            this.RPC_SpawnPhysicsBall();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_SpawnPhysicsBall()
        {
#if GAME_SERVER
            GameServer server = AppWrapper.Instance.AppReference.GameServer;
            server.ServerSpawn(this.ballPrefab, this.transform.position + new Vector3(0, 5, 0), Quaternion.identity, null);
#endif
        }
    }
}