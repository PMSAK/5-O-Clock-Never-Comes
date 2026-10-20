using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Camera")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float cameraDistance = 4f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float minPitch = -25f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Camera Collision")]
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private float cameraCollisionPadding = 0.1f;
    [SerializeField] private LayerMask cameraCollisionLayers = ~0;
    [SerializeField] private float cameraReturnSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private static readonly int PlayerWalkHash =
        Animator.StringToHash("PlayerWalk");

    private static readonly int PlayerJumpHash =
        Animator.StringToHash("PlayerJump");

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float cameraYaw;
    private float cameraPitch = 10f;
    private float currentCameraDistance;

    private bool jumpRequested;
    private bool isGrounded;

    private void Awake()
    {
        InitializeComponents();
        currentCameraDistance = cameraDistance;
    }

    private void Start()
    {
        InitializeCamera();
        LockCursor();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void InitializeCamera()
    {
        if (playerCamera != null)
            cameraYaw = playerCamera.eulerAngles.y;
    }

    // INPUT SYSTEM

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpRequested = true;
    }

    // UPDATE

    private void Update()
    {
        CheckGrounded();
        UpdateCameraRotation();
        UpdateWalkingAnimation();
    }

    private void CheckGrounded()
    {
        isGrounded = groundCheck != null &&
            Physics.CheckSphere(
                groundCheck.position,
                groundCheckRadius,
                groundLayer,
                QueryTriggerInteraction.Ignore
            );
    }

    private void UpdateCameraRotation()
    {
        cameraYaw += lookInput.x * mouseSensitivity;
        cameraPitch -= lookInput.y * mouseSensitivity;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            minPitch,
            maxPitch
        );
    }

    private void UpdateWalkingAnimation()
    {
        if (animator == null)
            return;

        bool isWalking =
            isGrounded && moveInput.sqrMagnitude > 0.01f;

        animator.SetBool(PlayerWalkHash, isWalking);
    }

    // PHYSICS

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
        HandleJump();
    }

    private Vector3 GetMovementDirection()
    {
        Quaternion yawRotation =
            Quaternion.Euler(0f, cameraYaw, 0f);

        Vector3 forward = yawRotation * Vector3.forward;
        Vector3 right = yawRotation * Vector3.right;

        Vector3 direction =
            forward * moveInput.y +
            right * moveInput.x;

        return Vector3.ClampMagnitude(direction, 1f);
    }

    private void MovePlayer()
    {
        Vector3 direction = GetMovementDirection();
        Vector3 velocity = rb.linearVelocity;

        rb.linearVelocity = new Vector3(
            direction.x * moveSpeed,
            velocity.y,
            direction.z * moveSpeed
        );
    }

    private void RotatePlayer()
    {
        Vector3 direction = GetMovementDirection();

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        Quaternion smoothedRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(smoothedRotation);
    }

    private void HandleJump()
    {
        if (!jumpRequested)
            return;

        if (isGrounded)
            PerformJump();

        jumpRequested = false;
    }

    private void PerformJump()
    {
        if (animator != null)
            animator.SetTrigger(PlayerJumpHash);

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }

    // CAMERA WITH COLLISION

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        if (playerCamera == null)
            return;

        Quaternion cameraRotation = Quaternion.Euler(
            cameraPitch,
            cameraYaw,
            0f
        );

        Vector3 focusPoint =
            transform.position + Vector3.up * cameraHeight;

        Vector3 desiredOffset = cameraRotation *
            new Vector3(0f, 0f, -cameraDistance);

        Vector3 desiredPosition = focusPoint + desiredOffset;

        Vector3 direction = desiredPosition - focusPoint;
        float desiredDistance = direction.magnitude;

        if (desiredDistance > 0.001f)
            direction /= desiredDistance;

        float targetDistance = desiredDistance;

        // Pull the camera forward if a wall blocks the view.
        if (desiredDistance > 0.001f &&
            Physics.SphereCast(
                focusPoint,
                cameraCollisionRadius,
                direction,
                out RaycastHit hit,
                desiredDistance,
                cameraCollisionLayers,
                QueryTriggerInteraction.Ignore
            ))
        {
            targetDistance = Mathf.Max(
                0.05f,
                hit.distance - cameraCollisionPadding
            );
        }

        // Move in quickly when blocked, return smoothly when clear.
        if (targetDistance < currentCameraDistance)
        {
            currentCameraDistance = targetDistance;
        }
        else
        {
            currentCameraDistance = Mathf.MoveTowards(
                currentCameraDistance,
                targetDistance,
                cameraReturnSpeed * Time.deltaTime
            );
        }

        playerCamera.position =
            focusPoint + direction * currentCameraDistance;

        playerCamera.rotation = cameraRotation;
    }

    // CURSOR

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            LockCursor();
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
