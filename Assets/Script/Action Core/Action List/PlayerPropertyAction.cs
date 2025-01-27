using UnityEngine;

[ActionCategory("Player")]
[System.Serializable]
public class PlayerPropertyAction : ActionBase
{
    [Tooltip("Enable or disable player control.")]
    public bool controlable;

    [Tooltip("Allow or disallow jumping.")]
    public bool canJump = true;

    [Tooltip("Walking speed of the player.")]
    public float walkSpeed = 2f;

    [Tooltip("Running speed of the player.")]
    public float runSpeed = 5f;

    [Tooltip("Gravity applied to the player.")]
    public float gravity = -9.81f;

    [Tooltip("Jump height of the player.")]
    public float jumpHeight = 2f;

    public override void Execute(GameObject target, System.Action onComplete)
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player tag not found in the scene.");
            onComplete?.Invoke();
            return;
        }

        // Get the TPSPlayerController component
        TPSPlayerController controller = player.GetComponent<TPSPlayerController>();
        if (controller == null)
        {
            Debug.LogError("TPSPlayerController component not found on the Player.");
            onComplete?.Invoke();
            return;
        }

        // Set properties
        controller.controlable = this.controlable;
        controller.canJump = this.canJump;
        controller.walkSpeed = this.walkSpeed;
        controller.runSpeed = this.runSpeed;
        controller.gravity = this.gravity;
        controller.jumpHeight = this.jumpHeight;

        if (this.controlable == false)
        {
            controller.animator.SetFloat("Speed", 0);
        }

        Debug.Log("Player properties updated successfully.");
        onComplete?.Invoke();
    }
}