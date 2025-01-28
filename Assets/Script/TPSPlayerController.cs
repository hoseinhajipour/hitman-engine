using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TPSPlayerController : MonoBehaviour
{
    public bool controlable;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public bool canJump = true;

    [Header("Crouch Settings")]
    public float crouchSpeed = 1f;
    public float crouchHeight = 1f;
    public float standHeight = 2f;

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Camera Settings")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isJumping = false;
    private bool isCrouching = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!controlable) return;

        HandleCrouch(); // Check crouch input
        HandleMovement(); // Handle movement and gravity
    }

    private void HandleMovement()
    {
        float speed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten movement vectors
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate movement direction
        Vector3 move = (forward * moveZ + right * moveX).normalized;

        // Ground check and reset vertical velocity if grounded
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f; // Keeps the player grounded
            }

            // Handle jumping
            if (canJump && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = true;
            }
        }
        else
        {
            isJumping = false;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine horizontal and vertical movement
        controller.Move((move * speed + velocity) * Time.deltaTime);

        // Rotate the player to face movement direction
        if (move.magnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Update animations
        animator.SetFloat("Speed", move.magnitude);
        animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift) && !isCrouching);
        animator.SetBool("IsCrouching", isCrouching);
    }


    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            controller.height = isCrouching ? crouchHeight : standHeight;
            controller.center = new Vector3(0, controller.height / 2, 0);

            animator.SetBool("IsCrouching", isCrouching);
        }
    }
}
