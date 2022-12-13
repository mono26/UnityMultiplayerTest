using Fusion;
using UnityEngine;

namespace MultiplayerTest
{
    public class GamePlayer : SLGNetworkBehaviour
    {
        private GameCharacterInput input = null;

        private void Awake()
        {
            this.Initialize();
        }

        public override void FixedUpdateNetwork()
        {
            if (this.GetInput(out NetworkInputData data)) {
                data.MoveDirection.Normalize();

                this.input.MoveInput(data.MoveDirection);
                this.input.LookInput(data.LookInput);
                this.input.JumpInput(data.Jump);
                this.input.SprintInput(data.Sprint);
            }
        }

        public override void Spawned()
        {
            base.Spawned();

            if (!Object.HasInputAuthority) {
                return;
            }

            this.gameObject.AddComponent<LocalPlayerCamera>();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.input = this.GetComponent<GameCharacterInput>();
        }
    }
}