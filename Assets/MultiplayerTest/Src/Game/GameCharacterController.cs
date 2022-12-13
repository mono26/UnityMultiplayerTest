#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using SLGFramework;
using UnityEngine;

namespace MultiplayerTest
{
    [RequireComponent(typeof(CharacterController))]
    public class GameCharacterController : SLGBehaviour
    {
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

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

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

        // cinemachine
        private float cinemachineTargetYaw = 0.0f;
        private float cinemachineTargetPitch = 0.0f;

        // player
        private float speed = 0.0f;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator animatorRef = null;

        private NetworkCharacterControllerImplementation characterController = null;

        private GameCharacterInput input = null;

        private const float _threshold = 0.01f;

        private bool hasAnimator = false;

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

            this.hasAnimator = TryGetComponent(out this.animatorRef) || this.transform.GetChild(0).TryGetComponent(out this.animatorRef);
        }

        private void FixedUpdate()
        {
            if (!this.IsInitialized || !this.IsPlaying) {
                return;
            }

            this.JumpAndGravity();
            this.GroundedCheck();
            this.Move();
        }

        private void LateUpdate()
        {
            if (!this.IsInitialized || !this.IsPlaying) {
                return;
            }

            this.RotateCameraRoot();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.characterController = this.GetComponent<NetworkCharacterControllerImplementation>();

            this.input = this.GetComponent<GameCharacterInput>();
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            this.hasAnimator = this.TryGetComponent(out this.animatorRef) || this.transform.GetChild(0).TryGetComponent(out this.animatorRef);

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
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
                this.animatorRef.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void RotateCameraRoot()
        {
            // if there is an this.input and camera position is not fixed
            if (this.input.look.sqrMagnitude >= _threshold && !LockCameraPosition) {
                //Don't multiply mouse this.input by Time.fixedDeltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

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
            // set target this.speed based on move this.speed, sprint this.speed and if sprint is pressed
            float targetSpeed = this.input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no this.input, set the target this.speed to 0
            if (this.input.move == Vector2.zero) {
                targetSpeed = 0.0f;
            }

            // a reference to the players current horizontal velocity
            // float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float currentHorizontalSpeed = new Vector3(this.characterController.Velocity.x, 0.0f, this.characterController.Velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = this.input.analogMovement ? this.input.move.magnitude : 1f;

            // accelerate or decelerate to target this.speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset) {
                // creates curved result rather than a linear one giving a more organic this.speed change
                // note T in Lerp is clamped, so we don't need to clamp our this.speed
                this.speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.fixedDeltaTime * SpeedChangeRate);

                // round this.speed to 3 decimal places
                this.speed = Mathf.Round(this.speed * 1000f) / 1000f;
            }
            else {
                this.speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.fixedDeltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise this.input direction
            Vector3 inputDirection = new Vector3(this.input.move.x, 0.0f, this.input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move this.input rotate player when the player is moving
            if (this.input.move != Vector2.zero) {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

#if GAME_SERVER
                // rotate to face this.input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
#endif
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            // move the player
            Vector3 movementVector = targetDirection.normalized * (this.speed * Time.fixedDeltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.fixedDeltaTime;

#if GAME_SERVER
            this.characterController.Move(movementVector);
#endif

            // update animator if using character
            if (this.hasAnimator) {
                this.animatorRef.SetFloat(_animIDSpeed, _animationBlend);
                this.animatorRef.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded) {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (this.hasAnimator) {
                    this.animatorRef.SetBool(_animIDJump, false);
                    this.animatorRef.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f) {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (this.input.jump && _jumpTimeoutDelta <= 0.0f) {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (this.hasAnimator) {
                        this.animatorRef.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f) {
                    _jumpTimeoutDelta -= Time.fixedDeltaTime;
                }
            }
            else {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f) {
                    _fallTimeoutDelta -= Time.fixedDeltaTime;
                }
                else {
                    // update animator if using character
                    if (this.hasAnimator) {
                        this.animatorRef.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                this.input.JumpInput(false);
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly this.speed up over time)
            if (_verticalVelocity < _terminalVelocity) {
                _verticalVelocity += Gravity * Time.fixedDeltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
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
            Gizmos.DrawSphere( new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
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
