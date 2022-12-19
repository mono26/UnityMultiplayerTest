using Fusion;
using UnityEngine;

namespace MultiplayerTest
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 MoveDirection { get; set; }
        public Vector2 LookDirection { get; set; }
        public bool Jump { get; set; }
        public bool Sprint { get; set; }
        public bool Action { get; set; }

        public override string ToString()
        {
            return $"MoveDirection {this.MoveDirection}, LookInput {this.LookDirection}, Jump {this.Jump}, Sprint {this.Sprint}, Action {this.Action}";
        }
    }
}
