using Cinemachine;
using Fusion;
using SLGFramework;
using UnityEngine;

namespace MultiplayerTest
{
    public class LocalPlayerCamera : SLGBehaviour
    {
        private Transform cameraRootFollow = null;

        private GameObject cameraRef = null;

        private void Awake()
        {
            this.Initialize();
        }

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.cameraRootFollow = this.transform.FindRecursive("PlayerCameraRoot");
            this.cameraRef = this.CreateLocalPlayerCamera();
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.InitializeLocalPlayerCamera();
        }

        private GameObject CreateLocalPlayerCamera()
        {
            GameObject cameraPFB = Resources.Load<GameObject>($"PFB_{nameof(LocalPlayerCamera)}");
            if (cameraPFB != null) {
                return GameObject.Instantiate(cameraPFB);
            }

            throw new System.Exception($"{nameof(LocalPlayerCamera)} not found in resources folder.");
        }

        private void InitializeLocalPlayerCamera()
        {
            if (this.cameraRef == null) {
                return;
            }

            Transform followCameraObj = this.cameraRef.transform.FindRecursive("PlayerFollowCamera");
            if (followCameraObj == null) {
                Log.Error("No follow camera obj found.");
                return;
            }

            CinemachineVirtualCamera cinemachineVirtualCamera = followCameraObj.GetComponent<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = this.cameraRootFollow;
        }
    }
}
