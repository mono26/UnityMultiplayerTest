using SLGFramework;
using UnityEngine.InputSystem;

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
                this.input.LookInput(data.LookDirection);
                this.input.JumpInput(data.Jump);
                this.input.SprintInput(data.Sprint);
                this.input.ActionInput(data.Action);
            }
        }

        public override void Spawned()
        {
            base.Spawned();

            if (!Object.HasInputAuthority) {
                return;
            }

            this.gameObject.AddComponent<LocalPlayerCamera>();
            this.gameObject.AddComponent<LocalPlayerInputHandler>();

            AppWrapper.Instance.AppReference.ServiceProvider.GetService<IEventManager>().Trigger(new GamePlayerSpawnedEvent(this));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.input = this.GetComponent<GameCharacterInput>();
        }
    }
}