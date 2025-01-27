using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject inventoryUI; // Reference to the inventory UI GameObject
    public GameObject[] weaponIcons; // Array of weapon icons (UI elements)

    [Header("Input Settings")]
    public KeyCode openInventoryKey = KeyCode.Mouse1; // Right-click
    public KeyCode closeInventoryKey = KeyCode.Mouse0; // Left-click

    private int selectedWeaponIndex = 0; // Currently selected weapon index
    private bool isInventoryOpen = false;
    public TPSCameraController _TPSCameraController;

    public PlayerShootingController _PlayerShootingController;
    void Start()
    {
        // Ensure the inventory UI is hidden at the start
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }

        // Initialize weapon selection
        UpdateWeaponSelection();
    }

    void Update()
    {
        // Open inventory on right-click
        if (Input.GetKeyDown(openInventoryKey) && !isInventoryOpen)
        {
            OpenInventory();
        }

        // Close inventory on left-click
        if (Input.GetKeyDown(closeInventoryKey) && isInventoryOpen)
        {
            CloseInventory();
        }

        // Handle weapon selection when inventory is open
        if (isInventoryOpen)
        {
            HandleWeaponSelection();
        }
    }

    private void OpenInventory()
    {
        // Pause the game
        Time.timeScale = 0f;
        _TPSCameraController.active = false;
        // Show the inventory UI
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(true);
        }

        isInventoryOpen = true;

        // Reset selected weapon to the first one
        selectedWeaponIndex = 0;
        UpdateWeaponSelection();
    }

    private void CloseInventory()
    {
        // Resume the game
        Time.timeScale = 1.0f;
        _TPSCameraController.active = true;
        // Hide the inventory UI
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }

        isInventoryOpen = false;

        // Equip the selected weapon (you can add logic here to equip the weapon)
        EquipWeapon(selectedWeaponIndex);
    }

    private void HandleWeaponSelection()
    {
        // Scroll to change the selected weapon
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f) // Scroll up
        {
            selectedWeaponIndex = (selectedWeaponIndex + 1) % weaponIcons.Length;
            UpdateWeaponSelection();
        }
        else if (scrollInput < 0f) // Scroll down
        {
            selectedWeaponIndex = (selectedWeaponIndex - 1 + weaponIcons.Length) % weaponIcons.Length;
            UpdateWeaponSelection();
        }
    }

    private void UpdateWeaponSelection()
    {
        // Highlight the selected weapon icon
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (weaponIcons[i] != null)
            {
                weaponIcons[i].SetActive(i == selectedWeaponIndex);
            }
        }
    }

    private void EquipWeapon(int weaponIndex)
    {
        _PlayerShootingController.updatweapon(weaponIndex);
    }
}