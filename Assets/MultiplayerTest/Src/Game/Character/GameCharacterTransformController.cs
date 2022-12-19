#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using SLGFramework;
using UnityEngine;

namespace MultiplayerTest
{
    public class GameCharacterTransformController : SLGBehaviour
    {
        [Header("Player")][Tooltip("Move this.speed of the character in m/s")]
        [SerializeField]
        private float MoveSpeed = 2.0f;

        private GameCharacterInput input = null;

        private void Awake()
        {
            this.Initialize();
        }

#if GAME_SERVER
        private void FixedUpdate()
        {
            Vector3 currentPosition = this.transform.position;
            currentPosition += new Vector3(this.input.move.x, 0, this.input.move.y) * this.MoveSpeed * AppWrapper.Instance.AppReference.GameServer.NetworkRunner.DeltaTime;
            this.transform.position = currentPosition;
        }
#endif

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.input = this.GetComponent<GameCharacterInput>();
        }
    }
}