using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections;

using Cinemachine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class FPController : MonoBehaviour
{
    public bool IsActive
    {
        get => isActive;
        private set
        {
            isActive = value;
            if (value)
            {
                move?.Enable();

                if (look != null)
                {
                    look.Enable();
                    look.performed += Look;
                }

                if (jump != null)
                {
                    jump.Enable();
                    jump.performed += Jump;
                }

                if (runToggle != null)
                {
                    runToggle.Enable();
                    runToggle.performed += ToggleRun;
                }

                if (crouchToggle != null)
                {
                    crouchToggle.Enable();
                    crouchToggle.performed += HandleCrouch;
                }

                if (interact != null)
                {
                    interact.Enable();
                    interact.performed += HandleInteract;
                }
            }
            else
            {
                move?.Disable();

                if (look != null)
                {
                    look.Disable();
                    look.performed -= Look;
                }

                if (jump != null)
                {
                    jump.Disable();
                    jump.performed -= Jump;
                }

                if (runToggle != null)
                {
                    runToggle.Disable();
                    runToggle.performed -= ToggleRun;
                }

                if (crouchToggle != null)
                {
                    crouchToggle.Disable();
                    crouchToggle.performed -= HandleCrouch;
                }

                if (interact != null)
                {
                    interact.Disable();
                    interact.performed -= HandleInteract;
                }
            }
        }
    }

    public PlayerBaseState CurrentState { get; set; }
    public PlayerStateFactory PlayerStateFactory { get; private set; }

    public bool IsGrounded { get; set; }
    public bool IsMoving { get; private set; }
    public bool IsRunning { get; set; }

    public bool JumpStarted { get; set; }
    public bool IsJumping => PlayerRigidbody.velocity.y > 0.1f;

    public float TargetFOV => IsRunning && IsMoving ? PlayerSettings.RunningFOV : PlayerSettings.WalkingFOV;

    public float RemainingCoyoteTime { get; set; }

    public float CurrentHeadBobProgress { get; private set; }

    public float LastAirTime { get; set; }

    public bool CrouchStarted { get; private set; }
    public bool IsCrouching { get; private set; }

    public bool CanMove { get; private set; }
    public Vector2 MoveInput { get; private set; }

    public float CurrentMaxSpeed
    {
        get
        {
            float speed = IsRunning ? PlayerSettings.RunSpeed : PlayerSettings.WalkSpeed;
            if (!IsGrounded)
            {
                speed *= PlayerSettings.AirSpeedFactor;
            }
            else if (IsCrouching)
            {
                speed = PlayerSettings.WalkSpeed * PlayerSettings.CrouchSpeedFactor;
            }

            return speed;
        }
    }

    public float CurrentHeight
    {
        get
        {
            float height = PlayerSettings.PlayerHeight;
            if (IsCrouching)
            {
                height *= PlayerSettings.CrouchHeightFactor;
            }

            return height;
        }
    }

    public Interactable CurrentInteractable
    {
        get => currentInteractable;
        set
        {
            currentInteractable = value;
            if (value != null)
            {
                UIManager.Instance?.SetCenterSpriteToInteract();
            }
            else
            {
                UIManager.Instance?.SetCenterSpriteToDefault();
            }
        }
    }

    public CinemachineVirtualCamera LinkedCamera => linkedCamera;
    public PlayerSettings PlayerSettings => playerSettings;
    public RaycastHit GroundHit => groundHit;

    public Rigidbody PlayerRigidbody
    {
        get
        {
            if (playerRigidbody == null)
            {
                playerRigidbody = GetComponent<Rigidbody>();
            }

            return playerRigidbody;
        }
    }
    public CapsuleCollider PlayerCollider
    {
        get
        {
            if (playerCollider == null)
            {
                playerCollider = GetComponent<CapsuleCollider>();
            }

            return playerCollider;
        }
    }

    [SerializeField] private CinemachineVirtualCamera linkedCamera;
    [SerializeField] private PlayerSettings playerSettings;

    private InputAction move, look, jump, runToggle, crouchToggle, interact;

    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCollider;

    private float currentCameraPitch;

    private RaycastHit groundHit;

    [SerializeField] private Interactable currentInteractable;

    private Coroutine crouchCoroutine;
    private bool isActive;
    
    private void Start()
    {
        InputManager inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogWarning("InputManager instance not found ...");
        }
        else
        {
            move = inputManager.GameInputActions.InGame.Move;
            look = inputManager.GameInputActions.InGame.Look;
            jump = inputManager.GameInputActions.InGame.Jump;
            runToggle = inputManager.GameInputActions.InGame.ToggleRun;
            crouchToggle = inputManager.GameInputActions.InGame.ToggleCrouch;
            interact = inputManager.GameInputActions.InGame.Interact;

            IsActive = true;
        }

        PlayerStateFactory = new PlayerStateFactory(this);
        ChangeState(PlayerStateFactory.GetGroundedState());

        CanMove = true;
        UIManager.Instance?.SetCursorLockState(CursorLockMode.Locked);
    }

    private void Update()
    {
        RemainingCoyoteTime -= Time.deltaTime;

        bool wasGrounded = IsGrounded;
        IsGrounded = CheckGrounded(out groundHit);
        if (IsGrounded)
        {
            CanMove = true;

            // Auto Parenting
            if (PlayerSettings.AutoParentMode != PlayerSettings.EAutoParentMode.NONE)
            {
                if (PlayerSettings.AutoParentMode == PlayerSettings.EAutoParentMode.ANY && groundHit.transform != transform.parent)
                {
                    transform.SetParent(groundHit.transform, true);
                }
                else if (PlayerSettings.AutoParentMode == PlayerSettings.EAutoParentMode.TARGET_PARENT) // "else if" is only for code clarity
                {
                    ParentTarget parentTarget = groundHit.transform.GetComponentInParent<ParentTarget>();
                    if (parentTarget != null)
                    {
                        if (parentTarget.transform != transform.parent)
                        {
                            transform.SetParent(parentTarget.transform, true);
                        }
                    }
                    else // Clear auto parenting for general ground without parent target
                    {
                        transform.SetParent(null, true);
                    }
                }
            }
        }
        else
        {
            transform.SetParent(null, true);
        }

        if (!IsGrounded && wasGrounded && !IsJumping) // Maybe refactor this into a PlayerCoyoteState?
        {
            RemainingCoyoteTime = PlayerSettings.CoyoteTime;
        }

        // Handle FOV
        if (LinkedCamera.m_Lens.FieldOfView != TargetFOV)
        {
            LinkedCamera.m_Lens.FieldOfView = Mathf.MoveTowards(LinkedCamera.m_Lens.FieldOfView, TargetFOV, PlayerSettings.FOVSlewRate * Time.deltaTime);
        }

        HandleMovement();
        CurrentState.UpdateChildStates();
    }

    private void FixedUpdate()
    {
        CurrentState.FixedUpdateChildStates();
    }

    private void LateUpdate()
    {
        HeadBob();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        CurrentState = newState;
        CurrentState.EnterState();
    }

    private void HandleMovement()
    {
        MoveInput = move.ReadValue<Vector2>();
        if (MoveInput.sqrMagnitude == 0f)
        {
            PlayerCollider.sharedMaterial = PlayerSettings.PlayerSlopeMaterial;
            IsMoving = false;
        }
        else
        {
            UIFaderManager.Instance?.FadeOutSequence(0);

            PlayerCollider.sharedMaterial = PlayerSettings.PlayerDefaultMaterial;
            IsMoving = true;
        }
    }

    private void Look(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        if (lookInput.sqrMagnitude != 0f)
        {
            float invertFactor = PlayerSettings.InvertedControls ? 1f : -1f;
            float yawDelta = -lookInput.x * PlayerSettings.Sensitivity.x * invertFactor * GameManager.Instance.SensitivityFactor;
            float pitchDelta = lookInput.y * PlayerSettings.Sensitivity.y * invertFactor * GameManager.Instance.SensitivityFactor;

            // Player yaw rotation
            transform.localRotation = transform.localRotation * Quaternion.Euler(0f, yawDelta, 0f);

            // Camera pitch rotation
            currentCameraPitch = Mathf.Clamp(currentCameraPitch + pitchDelta, PlayerSettings.PitchRange.Min, PlayerSettings.PitchRange.Max);
            LinkedCamera.transform.localRotation = Quaternion.Euler(currentCameraPitch, 0f, 0f);
        }
    }

    private void HeadBob()
    {
        if (PlayerSettings.HeadBobEnabled && IsGrounded)
        {
            Vector3 currentCameraPosition = LinkedCamera.transform.localPosition;
            currentCameraPosition.y = PlayerCollider.height * 0.8f;

            float currentVelocity = PlayerRigidbody.velocity.magnitude;
            if (currentVelocity >= PlayerSettings.HeadBobMinVelocity)
            {
                float speedFactor = currentVelocity / CurrentMaxSpeed;
                CurrentHeadBobProgress += Time.deltaTime / PlayerSettings.HeadBobPeriodCurve.Evaluate(speedFactor);
                CurrentHeadBobProgress %= 1f;

                float maxVerticalTranslation = PlayerSettings.HeadBobVerticalCurve.Evaluate(speedFactor);
                float maxHorizontalTranslation = PlayerSettings.HeadBobHorizontalCurve.Evaluate(speedFactor);

                Vector3 cameraOffset = Mathf.Cos(2f * Mathf.PI * CurrentHeadBobProgress) * new Vector3(maxHorizontalTranslation, maxVerticalTranslation, 0f);
                LinkedCamera.transform.localPosition = currentCameraPosition + cameraOffset;
            }
            else
            {
                CurrentHeadBobProgress = 0f;
                LinkedCamera.transform.localPosition = currentCameraPosition;
            }
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && (IsGrounded || RemainingCoyoteTime > 0f))
        {
            CanMove = PlayerSettings.AllowAirMovement;
            JumpStarted = true;
        }
    }

    private void ToggleRun(InputAction.CallbackContext context)
    {
        if (context.performed && !IsCrouching)
        {
            IsRunning = !IsRunning;

            if (IsRunning)
            {
                UIFaderManager.Instance?.FadeOutSequence(4);
            }
        }
    }

    private void HandleCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleCrouch();
        }
    }

    public void ToggleCrouch()
    {
        if (IsCrouching)
        {
            Vector3 lookUpStart = transform.position;
            lookUpStart.y += PlayerSettings.PlayerHeight * PlayerSettings.CrouchHeightFactor;

            Vector3 lookUpDirection = Vector3.up;
            float lookUpDistance = PlayerSettings.PlayerHeight * (1f - playerSettings.CrouchHeightFactor);

            Debug.DrawRay(lookUpStart, lookUpDirection * lookUpDistance, Color.red, 0.1f);

            if (Physics.Raycast(lookUpStart, lookUpDirection, lookUpDistance))
            {
                return;
            }
        }

        IsCrouching = !IsCrouching;
        crouchCoroutine = StartCoroutine(ToggleCrouchCoroutine(PlayerSettings.CrouchTime));

        if (IsCrouching)
        {
            UIFaderManager.Instance?.FadeOutSequence(5);
        }
        else
        {
            UIFaderManager.Instance?.FadeOutSequence(6);
        }
    }

    private IEnumerator ToggleCrouchCoroutine(float toggleTime)
    {
        if (crouchCoroutine != null)
        {
            StopCoroutine(crouchCoroutine);
            crouchCoroutine = null;
        }

        float minHeight = PlayerSettings.PlayerHeight * PlayerSettings.CrouchHeightFactor;
        float maxHeight = PlayerSettings.PlayerHeight;

        float startHeight = PlayerCollider.height;
        float targetHeight = CurrentHeight;

        float distanceRatio = Mathf.Abs((targetHeight - startHeight) / (maxHeight - minHeight));
        float progress = 1f - distanceRatio;
        while (progress < 1f)
        {
            progress = Mathf.Min(1f, progress + Time.deltaTime / toggleTime);

            // Update collider height
            float nextHeight = Mathf.Lerp(startHeight, targetHeight, progress);
            PlayerCollider.height = nextHeight;

            // Update collider center
            Vector3 newCenter = PlayerCollider.center;
            newCenter.y = PlayerCollider.height * 0.5f;
            PlayerCollider.center = newCenter;

            // Update FP camera position
            if (LinkedCamera.transform != null)
            {
                Vector3 newCameraPosition = LinkedCamera.transform.localPosition;
                newCameraPosition.y = PlayerCollider.height * 0.8f;
                LinkedCamera.transform.localPosition = newCameraPosition;
            }

            yield return null;
        }

        crouchCoroutine = null;
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UIFaderManager.Instance?.FadeOutSequence(3);

            if (CurrentInteractable != null)
            {
                CurrentInteractable.Interact();
            }
        }
    }

    private bool CheckGrounded(out RaycastHit hit)
    {
        Vector3 start = PlayerRigidbody.position + Vector3.up * CurrentHeight * 0.5f + transform.forward * PlayerSettings.PlayerRadius;
        float checkDistance = CurrentHeight * 0.5f + PlayerSettings.GroundOffset;

        return Physics.Raycast(start, Vector3.down, out hit, checkDistance, PlayerSettings.GroundMask, QueryTriggerInteraction.Ignore);
    }

    private void OnEnable()
    {
        IsActive = true;
        UIManager.Instance?.SetCenterSpriteToDefault();
    }

    private void OnDisable()
    {
        IsActive = false;
        UIManager.Instance?.SetCenterSpriteToNone();
    }

    private void OnValidate()
    {
        if (PlayerSettings != null)
        {
            PlayerCollider.radius = PlayerSettings.PlayerRadius;
            PlayerCollider.height = CurrentHeight;

            Vector3 newCenter = PlayerCollider.center;
            newCenter.y = CurrentHeight * 0.5f;
            PlayerCollider.center = newCenter;

            if (LinkedCamera != null)
            {
                Vector3 newCameraPosition = LinkedCamera.transform.localPosition;
                newCameraPosition.y = CurrentHeight * 0.8f;
                LinkedCamera.transform.localPosition = newCameraPosition;
            }
        }
    }
}
