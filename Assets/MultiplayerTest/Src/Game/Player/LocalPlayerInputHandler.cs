using SLGFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTest
{
    public class LocalPlayerInputHandler : SLGBehaviour
    {
        private NetworkInputData inputData = new NetworkInputData();

        private PFBFactory<LocalPlayerInputListener> inputListenerFactory = null;
        private LocalPlayerInputListener inputListener = null;

        public NetworkInputData InputData => this.inputData;

        private void Awake()
        {
            this.Initialize();
        }

        private void Start()
        {
            this.BeginPlay();
        }

        private void Update()
        {
            if (this.inputListener == null) {
                return;
            }

            this.inputData.MoveDirection = this.inputListener.MoveInputValue;
            this.inputData.LookDirection = this.inputListener.LookInputValue;
            this.inputData.Jump = this.inputListener.JumpInputValue;
            this.inputData.Sprint = this.inputListener.SprintInputValue;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.inputListenerFactory = new PFBFactory<LocalPlayerInputListener>();
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.inputListener = this.inputListenerFactory.CreateInstance(this.transform, Vector3.zero, Quaternion.identity);
        }
    }
}