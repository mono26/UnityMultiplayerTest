using Fusion;
using UnityEngine;

namespace MultiplayerTest
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 MoveDirection { get; set; }
        public bool Jump { get; set; }
        public bool Sprint { get; set; }
    }
}
