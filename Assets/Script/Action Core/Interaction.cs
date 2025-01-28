using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    private bool isNearInteractable = false; // Is the player near an interactable object?

    public ActionManager actionManager;
    public GameObject uiCanvas; // Reference to the UI Canvas
    public float fadeDistance = 5f; // Distance at which the UI starts fading
    private CanvasGroup canvasGroup;
    private GameObject player;

    void Start()
    {
        if (uiCanvas != null)
        {
            canvasGroup = uiCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // Add a CanvasGroup component if it doesn't exist
                canvasGroup = uiCanvas.AddComponent<CanvasGroup>();
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isNearInteractable)
        {
            actionManager.RunActions(player);
        }

        // Update UI opacity based on distance
        UpdateUIOpacity();
    }

    private void UpdateUIOpacity()
    {
        if (uiCanvas == null || player == null) return;

        // Calculate the distance between the player and the interactable object
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= fadeDistance)
        {
            // Set opacity to 1 when within range
            canvasGroup.alpha = 1f;
        }
        else
        {
            // Gradually fade out based on distance
            float alpha = Mathf.Clamp01(1f - ((distance - fadeDistance) / fadeDistance));
            canvasGroup.alpha = alpha;
        }
    }

    // Detect when player enters the trigger zone of an interactable object
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearInteractable = true;
            player = other.gameObject;

            // Show the UI immediately when player enters range
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }

    // Detect when player exits the trigger zone of an interactable object
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearInteractable = false;
            player = null;

            // Gradually fade out when player exits range
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }
    }
}
