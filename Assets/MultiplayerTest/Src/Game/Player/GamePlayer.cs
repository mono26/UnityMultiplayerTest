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
            if (!HasInputAuthority) {
                return;
            }

            if (this.GetInput(out NetworkInputData data)) {
                data.MoveDirection.Normalize();

                this.input.MoveInput(data.MoveDirection);
                this.input.JumpInput(data.Jump);
                this.input.SprintInput(data.Sprint);

                // this.transform.position = this.transform.position + new Vector3(data.MoveDirection.x, 0, data.MoveDirection.y);
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