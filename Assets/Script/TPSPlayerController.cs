using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TPSPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public bool canJump = true;

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Camera Settings")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isJumping = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Get camera forward and right directions
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten the directions to avoid tilting
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate movement direction relative to the camera
        Vector3 move = (forward * moveZ + right * moveX).normalized;

        controller.Move(move * speed * Time.deltaTime);

        // Ground check and gravity
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f; // Small constant to keep grounded

                if (isJumping)
                {
                    isJumping = false;
                    animator.SetBool("IsJumping", isJumping);
                    animator.SetTrigger("JumpLand"); // Trigger the landing animation
                }
            }

            // Jumping logic
            if (canJump && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isJumping = true;
                animator.SetTrigger("JumpStart"); // Trigger the jump start animation
            }
        }
        else if (isJumping)
        {
            animator.SetBool("IsJumping", true); // Set the loop animation while in the air
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Rotate player to face movement direction
        if (move.magnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Update animator
        animator.SetFloat("Speed", move.magnitude);
        animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));
    }
}
