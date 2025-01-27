using UnityEngine;

public class TPSCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform playerBody;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    public float orbitDistance = 3f;
    public Vector3 cameraOffset = new Vector3(0, 1.5f, 0);
    public LayerMask collisionMask; // Layer mask برای تشخیص برخورد با اشیاء

    private float verticalLookRotation = 0f;
    private float horizontalRotation = 0f;
    public bool active = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (active)
        {
            HandleCameraRotation();
        }
    }

    private void HandleCameraRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update rotation angles
        horizontalRotation += mouseX;
        verticalLookRotation -= mouseY;

        // Clamp vertical rotation
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -maxLookAngle, maxLookAngle);

        // Calculate camera position and rotation
        Quaternion rotation = Quaternion.Euler(verticalLookRotation, horizontalRotation, 0);
        Vector3 targetPosition = playerBody.position + cameraOffset - (rotation * Vector3.forward * orbitDistance);

        // Check for collisions
        Vector3 direction = targetPosition - (playerBody.position + cameraOffset);
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(playerBody.position + cameraOffset, direction.normalized, out hit, distance, collisionMask))
        {
            // اگر برخورد وجود داشت، موقعیت دوربین را به نقطه برخورد منتقل کنید
            targetPosition = hit.point;
        }

        // Set camera position and rotation
        transform.position = targetPosition;
        transform.LookAt(playerBody.position + cameraOffset);
    }
}