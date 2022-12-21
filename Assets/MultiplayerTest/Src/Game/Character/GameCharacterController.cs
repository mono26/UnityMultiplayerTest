#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using SLGFramework;
using Unity.VisualScripting;
using UnityEngine;

namespace MultiplayerTest
{
    [RequireComponent(typeof(CharacterController))]
    public class GameCharacterController : SLGNetworkBehaviour
    {
        private const float Threshold = 0.01f;

        [Header("Player")]
        [Tooltip("Move this.speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint this.speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        private bool hasAnimator = false;

        // cinemachine
        private float cinemachineTargetYaw = 0.0f;
        private float cinemachineTargetPitch = 0.0f;

        // player
        private float speed = 0.0f;
        private float animationBlend = 0.0f;
        private float targetRotation = 0.0f;
        private float rotationVelocity = 0.0f;
        private float verticalVelocity = 0.0f;
        private float terminalVelocity = 53.0f;

        // timeout deltatime
        private float jumpTimeoutDelta = 0.0f;
        private float fallTimeoutDelta = 0.0f;

        // animation IDs
        private int animIDSpeed = 0;
        private int animIDGrounded = 0;
        private int animIDJump = 0;
        private int animIDFreeFall = 0;
        private int animIDMotionSpeed = 0;

        private bool jumped = false;
        private bool sprinted = false;
        private bool moved = false;

        private Animator animatorRef = null;

        private NetworkCharacterControllerImplementation characterController = null;

        private GameCharacterInput input = null;

#if GAME_CLIENT
        private Vector3 lastPosition = Vector3.zero;
#endif

        private bool IsCurrentDeviceMouse
        {
            get {
				return true;
            }
        }

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
            if (!this.IsInitialized || !this.IsPlaying) {
                return;
            }

            if (this.input.move != Vector2.zero) {
                this.moved = true;
            }

            if (this.input.jump == true) {
                this.jumped = true;
            }

            if (this.input.sprint == true) {
                this.sprinted = true;
            }
        }

        //private void FixedUpdate()
        //{
        //    if (!this.IsInitialized || !this.IsPlaying) {
        //        return;
        //    }

        //    this.JumpAndGravity();
        //    this.GroundedCheck();
        //    this.Move();
        //}

        private void LateUpdate()
        {
            if (!this.IsInitialized || !this.IsPlaying) {
                return;
            }

            this.RotateCameraRoot();
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) {
                Gizmos.color = transparentGreen;
            }
            else {
                Gizmos.color = transparentRed;
            }

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!this.IsInitialized || !this.IsPlaying) {
                return;
            }

            this.JumpAndGravity();
            this.GroundedCheck();
            this.Move();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.characterController = this.GetComponent<NetworkCharacterControllerImplementation>();

            this.input = this.GetComponent<GameCharacterInput>();

            this.hasAnimator = this.TryGetComponent(out this.animatorRef) || this.transform.GetChild(0).TryGetComponent(out this.animatorRef);
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            this.AssignAnimationIDs();

            // reset our timeouts on start
            this.jumpTimeoutDelta = JumpTimeout;
            this.fallTimeoutDelta = FallTimeout;

#if GAME_CLIENT
            this.lastPosition = this.transform.position;
#endif
        }

        private void AssignAnimationIDs()
        {
            this.animIDSpeed = Animator.StringToHash("Speed");
            this.animIDGrounded = Animator.StringToHash("Grounded");
            this.animIDJump = Animator.StringToHash("Jump");
            this.animIDFreeFall = Animator.StringToHash("FreeFall");
            this.animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (this.hasAnimator) {
                this.animatorRef.SetBool(this.animIDGrounded, Grounded);
            }
        }

        private void RotateCameraRoot()
        {
#if GAME_SERVER
            float deltaTime = AppWrapper.Instance.AppReference.GameServer.NetworkRunner.DeltaTime;
#elif GAME_CLIENT
            float deltaTime = AppWrapper.Instance.AppReference.GameClient.NetworkRunner.DeltaTime;
#endif

            // if there is an this.input and camera position is not fixed
            if (this.input.look.sqrMagnitude >= Threshold && !LockCameraPosition) {
                //Don't multiply mouse this.input by Time.fixedDeltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : deltaTime;

                this.cinemachineTargetYaw += this.input.look.x * deltaTimeMultiplier;
                this.cinemachineTargetPitch += this.input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            this.cinemachineTargetYaw = ClampAngle(this.cinemachineTargetYaw, float.MinValue, float.MaxValue);
            this.cinemachineTargetPitch = ClampAngle(this.cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(this.cinemachineTargetPitch + CameraAngleOverride,
                this.cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
#if GAME_SERVER
            this.UpdatePosition();
#elif GAME_CLIENT
            this.ProcessMovementUpdate();
#endif
        }

#if GAME_SERVER
        private void UpdatePosition()
        {
            float deltaTime = AppWrapper.Instance.AppReference.GameServer.NetworkRunner.DeltaTime;

            Vector3 movementDelta = new Vector3(this.input.move.x, 0, this.input.move.y);

            // set target this.speed based on move this.speed, sprint this.speed and if sprint is pressed
            float targetSpeed = this.input.sprint ? SprintSpeed : MoveSpeed;

            // note: Vector3's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no this.input, set the target this.speed to 0
            if (movementDelta == Vector3.zero) {
                targetSpeed = 0.0f;
            }

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(this.characterController.Velocity.x, 0.0f, this.characterController.Velocity.z).magnitude;

            float speedOffset = 0.1f;
            // float inputMagnitude = this.input.analogMovement ? movementDelta.magnitude : 1f;
            float inputMagnitude = this.input.analogMovement ? movementDelta.magnitude / this.input.move.magnitude : movementDelta.magnitude / 1f;

            // accelerate or decelerate to target this.speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset) {
                // creates curved result rather than a linear one giving a more organic this.speed change
                // note T in Lerp is clamped, so we don't need to clamp our this.speed
                this.speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, deltaTime * SpeedChangeRate);

                // round this.speed to 3 decimal places
                this.speed = Mathf.Round(this.speed * 1000f) / 1000f;
            }
            else {
                this.speed = targetSpeed;
            }

            this.animationBlend = Mathf.Lerp(this.animationBlend, targetSpeed, deltaTime * SpeedChangeRate);
            if (this.animationBlend < 0.01f) {
                this.animationBlend = 0f;
            }

            // normalise this.input direction
            Vector3 inputDirection = movementDelta.normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move this.input rotate player when the player is moving
            if (movementDelta.sqrMagnitude > 0.0f) {
                this.targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, this.targetRotation, ref this.rotationVelocity, RotationSmoothTime);

                // rotate to face this.input direction relative to camera position
                this.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, this.targetRotation, 0.0f) * Vector3.forward;
            // move the player
            Vector3 movementVector = targetDirection.normalized * (this.speed * deltaTime) + new Vector3(0.0f, this.verticalVelocity, 0.0f) * deltaTime;

            this.characterController.Move(movementVector);

            // update animator if using character
            if (this.hasAnimator) {
                Log.Info($"animSpeed {this.animationBlend}, motionSpeed {inputMagnitude}");

                this.animatorRef.SetFloat(this.animIDSpeed, this.animationBlend);
                this.animatorRef.SetFloat(this.animIDMotionSpeed, inputMagnitude);
            }
        }
#elif GAME_CLIENT
        private void ProcessMovementUpdate()
        {
            Vector3 movementDelta = this.transform.position - this.lastPosition;

            float deltaTime = AppWrapper.Instance.AppReference.GameClient.NetworkRunner.DeltaTime;

            if (this.moved == false && movementDelta == Vector3.zero) {
                this.animationBlend = Mathf.Lerp(this.animationBlend, 0, deltaTime * SpeedChangeRate);
                if (this.animationBlend < 0.01f) {
                    this.animationBlend = 0f;
                }

                // update animator if using character
                if (this.hasAnimator) {
                    this.animatorRef.SetFloat(this.animIDSpeed, this.animationBlend);
                    this.animatorRef.SetFloat(this.animIDMotionSpeed, 0.0f);
                }
            }
            else {
                // set target this.speed based on move this.speed, sprint this.speed and if sprint is pressed
                float targetSpeed = this.sprinted && movementDelta.sqrMagnitude >= this.MoveSpeed * deltaTime ? this.SprintSpeed : this.MoveSpeed;

                //// note: Vector3's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                //// if there is no this.input, set the target this.speed to 0
                //if (this.moved == false && movementDelta == Vector3.zero) {
                //    targetSpeed = 0.0f;
                //}

                // a reference to the players current horizontal velocity
                // float currentHorizontalSpeed = new Vector3(movementDelta.x, 0.0f, movementDelta.z).magnitude;
                float currentHorizontalSpeed = new Vector3(this.characterController.Velocity.x, 0.0f, this.characterController.Velocity.z).magnitude;

                float speedOffset = 0.1f;
                float inputMagnitude = this.moved || movementDelta.sqrMagnitude > 0.0f ? 1f : 0.0f;

                // accelerate or decelerate to target this.speed
                if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset) {
                    // creates curved result rather than a linear one giving a more organic this.speed change
                    // note T in Lerp is clamped, so we don't need to clamp our this.speed
                    this.speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, deltaTime * SpeedChangeRate);

                    // round this.speed to 3 decimal places
                    this.speed = Mathf.Round(this.speed * 1000f) / 1000f;
                }
                else {
                    this.speed = targetSpeed;
                }

                this.animationBlend = Mathf.Lerp(this.animationBlend, targetSpeed, deltaTime * SpeedChangeRate);
                if (this.animationBlend < 0.01f) {
                    this.animationBlend = 0f;
                }

                this.lastPosition = this.transform.position;

                // update animator if using character
                if (this.hasAnimator) {
                    this.animatorRef.SetFloat(this.animIDSpeed, this.animationBlend);
                    this.animatorRef.SetFloat(this.animIDMotionSpeed, inputMagnitude);
                }

                if (this.moved == true && this.input.move == Vector2.zero) {
                    this.moved = false;

                    if (this.sprinted == true && this.input.sprint == false) {
                        this.sprinted = false;
                    }
                }
            }
        }
#endif

        private void JumpAndGravity()
        {
#if GAME_SERVER
            float deltaTime = AppWrapper.Instance.AppReference.GameServer.NetworkRunner.DeltaTime;
#elif GAME_CLIENT
            float deltaTime = AppWrapper.Instance.AppReference.GameClient.NetworkRunner.DeltaTime;
#endif

            if (Grounded) {
                // reset the fall timeout timer
                this.fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (this.hasAnimator) {
                    this.animatorRef.SetBool(this.animIDJump, false);
                    this.animatorRef.SetBool(this.animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (this.verticalVelocity < 0.0f) {
                    this.verticalVelocity = -2f;
                }

                // Jump
#if GAME_SERVER
                bool jump = this.input.jump;
#elif GAME_CLIENT
                bool jump = this.jumped && this.transform.position.y != this.lastPosition.y;
#endif

                if (jump && this.jumpTimeoutDelta <= 0.0f) {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    this.verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (this.hasAnimator) {
                        this.animatorRef.SetBool(this.animIDJump, true);
                    }

                    this.jumped = false;
                }

                // jump timeout
                if (this.jumpTimeoutDelta >= 0.0f) {
                    this.jumpTimeoutDelta -= deltaTime;
                }
            }
            else {
                // reset the jump timeout timer
                this.jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (this.fallTimeoutDelta >= 0.0f) {
                    this.fallTimeoutDelta -= deltaTime;
                }
                else {
                    // update animator if using character
                    if (this.hasAnimator) {
                        this.animatorRef.SetBool(this.animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                this.input.JumpInput(false);
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly this.speed up over time)
            if (this.verticalVelocity < this.terminalVelocity) {
                this.verticalVelocity += Gravity * deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        public void OnFootstep(AnimationEvent animationEvent)
        {
#if GAME_CLIENT
            if (animationEvent.animatorClipInfo.weight > 0.5f) {
                if (FootstepAudioClips.Length > 0) {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(this.characterController.Controller.center), FootstepAudioVolume);
                }
            }
#endif
        }

        public void OnLand(AnimationEvent animationEvent)
        {
#if GAME_CLIENT
            if (animationEvent.animatorClipInfo.weight > 0.5f) {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(this.characterController.Controller.center), FootstepAudioVolume);
            }
#endif
        }
    }
}
