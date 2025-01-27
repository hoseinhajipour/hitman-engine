using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public class InteractionSystem : MonoBehaviour
{
    public GameObject interactionMenu; // Reference to the UI menu parent
    public GameObject textOptionPrefab; // Prefab for the TextMeshPro option
    public Transform optionsContainer; // Parent object for the options
    public List<string> interactionOptions = new List<string>(); // List of interaction options
    public UnityEvent[] itemActions; // Array of UnityEvents for custom actions
    public float scrollSensitivity = 1f; // Scroll sensitivity for changing items

    private List<TextMeshProUGUI> optionTexts = new List<TextMeshProUGUI>(); // List of TextMeshProUGUI components
    private int currentSelection = 0; // Currently selected item index
    private bool isMenuOpen = false; // Is the menu currently open?
    private bool isNearInteractable = false; // Is the player near an interactable object?


    void Update()
    {
        // Open menu when holding 'E' and near an interactable object
        if (Input.GetKeyDown(KeyCode.E) && isNearInteractable)
        {
            OpenMenu();
        }
        if (isNearInteractable)
        {
            // Scroll through options when menu is open
            if (isMenuOpen)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0)
                {
                    ChangeSelection(scroll);
                }
            }

            // Execute action when releasing 'E'
            if (Input.GetKeyUp(KeyCode.E) && isMenuOpen)
            {
                ExecuteAction();
                CloseMenu();
            }

            // Cancel action if 'E' is released without selecting
            if (Input.GetKeyUp(KeyCode.E) && !isMenuOpen)
            {
                CancelAction();
            }
        }
    }

    void CreateMenuOptions()
    {
        if (optionsContainer == null)
        {
            Debug.LogError("optionsContainer is null!");
            return;
        }

        if (textOptionPrefab == null)
        {
            Debug.LogError("textOptionPrefab is null!");
            return;
        }

        if (interactionOptions == null || interactionOptions.Count == 0)
        {
            Debug.LogError("interactionOptions is null or empty!");
            return;
        }
        OpenMenu();

        // Clear existing options
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
        optionTexts.Clear();

        // Create new options
        for (int i = 0; i < interactionOptions.Count; i++)
        {
            GameObject option = Instantiate(textOptionPrefab, optionsContainer);
            TextMeshProUGUI optionText = option.GetComponent<TextMeshProUGUI>();
            optionText.text = interactionOptions[i];
            optionTexts.Add(optionText);
        }
    }

    void OpenMenu()
    {
        isMenuOpen = true;
        interactionMenu.SetActive(true);
        HighlightSelection(currentSelection);
    }

    void CloseMenu()
    {
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
        isMenuOpen = false;
        interactionMenu.SetActive(false);
    }

    void ChangeSelection(float scroll)
    {
        // Update selection based on scroll direction
        currentSelection += (int)(scroll * scrollSensitivity);

        // Wrap around if out of bounds
        if (currentSelection < 0)
            currentSelection = optionTexts.Count - 1;
        else if (currentSelection >= optionTexts.Count)
            currentSelection = 0;

        HighlightSelection(currentSelection);
    }

    void HighlightSelection(int index)
    {
        // Highlight the selected option
        for (int i = 0; i < optionTexts.Count; i++)
        {
            if (i == index)
                optionTexts[i].color = Color.white; // Highlight color
            else
                optionTexts[i].color = Color.black; // Default color
        }
    }

    void ExecuteAction()
    {
        // Execute the custom action for the selected item
        if (currentSelection >= 0 && currentSelection < itemActions.Length)
        {
            itemActions[currentSelection].Invoke(); // Trigger the UnityEvent
        }
    }

    public void CancelAction()
    {
        Debug.Log("Action Cancelled");
        CloseMenu();
    }

    // Detect when player enters the trigger zone of an interactable object
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearInteractable = true;
            Debug.Log("Near Interactable Object");
            // Dynamically create TextMeshProUGUI elements for each option
            CreateMenuOptions();
        }
    }

    // Detect when player exits the trigger zone of an interactable object
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearInteractable = false;
            CloseMenu(); // Close the menu if the player moves away
            Debug.Log("Left Interactable Object");
        }
    }
}