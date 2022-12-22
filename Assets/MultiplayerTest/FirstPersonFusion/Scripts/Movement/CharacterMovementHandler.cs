using UnityEngine;
using Fusion;

/// <summary>
/// This class handles the movement of the character.
/// </summary>
public class CharacterMovementHandler : NetworkBehaviour
{
    bool isRespawnedRequested = false;

    NetworkCharacterControllerPrototypeCustom characterController;
    HPHandler hpHandler;
    NetworkMecanimAnimator networkMecanimAnimator;

    public float animationSpeed = 2f;
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    public AudioClip[] FootstepAudioClips;
    public AudioClip LandingAudioClip;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    void Awake()
    {
        characterController = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        hpHandler = GetComponent<HPHandler>();
        networkMecanimAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    void Start()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (isRespawnedRequested)
            {
                Respawn();
                return;
            }

            if (hpHandler.isDead)
                return;
        }

        if (GetInput(out NetworkInputData networkInputData))
        {
            //Rotate
            transform.forward = Vector3.RotateTowards(transform.forward, networkInputData.aimForwardVector, Time.deltaTime * characterController.rotationSpeed, 0f);

            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            characterController.Move(moveDirection);
            networkMecanimAnimator.Animator.SetFloat(_animIDSpeed, moveDirection.magnitude * animationSpeed);
            networkMecanimAnimator.Animator.SetFloat(_animIDMotionSpeed, characterController.Velocity.magnitude);

            //Jump
            if (networkInputData.jumpRequest)
            {
                characterController.Jump();
                networkMecanimAnimator.Animator.SetBool(_animIDJump, networkInputData.jumpRequest);
            }

            if (characterController.IsGrounded)
            {
                networkMecanimAnimator.Animator.SetBool(_animIDGrounded, true);
                networkMecanimAnimator.Animator.SetBool(_animIDJump, false);
            }
            else
            {
                networkMecanimAnimator.Animator.SetBool(_animIDGrounded, false);
                networkMecanimAnimator.Animator.SetBool(_animIDJump, true);
            }
        }
    }

    /// <summary> Requests a respawn. </summary>
    public void RequestSpawn()
    {
        isRespawnedRequested = true;
    }

    /// <summary> Respawns the character. </summary>
    void Respawn()
    {
        characterController.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();
        isRespawnedRequested = false;
    }

    /// <summary> Turns the character controller on or off. </summary>
    /// <param name="enabled"> Bool to enable/disable the character controller. </param>
    public void SetCharacterControllerEnabled(bool enabled)
    {
        characterController.Controller.enabled = enabled;
    }

    private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(characterController.GetComponent<CharacterController>().center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(characterController.GetComponent<CharacterController>().center), FootstepAudioVolume);
            }
        }
}
