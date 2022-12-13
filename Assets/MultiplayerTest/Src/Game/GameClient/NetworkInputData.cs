using Fusion;
using UnityEngine;

namespace MultiplayerTest
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 MoveDirection { get; set; }
        public Vector2 LookInput { get; set; }
        public bool Jump { get; set; }
        public bool Sprint { get; set; }

        public override string ToString()
        {
            return $"MoveDirection {this.MoveDirection}, LookInput {this.LookInput}, Jump {this.Jump}, Sprint {this.Sprint}";
        }
    }
}
