using Fusion;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTest
{
    public class GamePlayer : SLGNetworkBehaviour
    {
        private StarterAssetsInputs input = null;

        private void Awake()
        {
            this.input = this.GetComponent<StarterAssetsInputs>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data)) {
                data.MoveDirection.Normalize();

                this.input.MoveInput(data.MoveDirection);
                this.input.JumpInput(data.Jump);
                this.input.SprintInput(data.Sprint);
            }
        }
    }
}