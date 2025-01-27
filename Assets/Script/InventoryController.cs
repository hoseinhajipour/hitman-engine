using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject inventoryUI; // Reference to the inventory UI GameObject
    public GameObject[] weaponIcons; // Array of weapon icons (UI elements)
    public Text weaponStatsText; // UI Text for displaying weapon stats

    [Header("Input Settings")]
    public KeyCode openInventoryKey = KeyCode.Mouse1; // Right-click
    public KeyCode closeInventoryKey = KeyCode.Mouse0; // Left-click

    private int selectedWeaponIndex = 0; // Currently selected weapon index
    private bool isInventoryOpen = false;
    public TPSCameraController _TPSCameraController;
    public PlayerShootingController _PlayerShootingController;

    [Header("Pickup Settings")]
    public Transform weaponHolder; // Parent object for picked-up weapons
    public float pickupRange = 2f; // Range to detect weapons for pickup
    public LayerMask pickupLayer; // Layer mask for weapon pickups

    void Start()
    {
        // Ensure the inventory UI is hidden at the start
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }

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

        // Weapon pickup
        if (Input.GetKeyDown(KeyCode.E)) // Press 'E' to pick up weapons
        {
            TryPickupWeapon();
        }
    }

    private void OpenInventory()
    {
        Time.timeScale = 0f;
        _TPSCameraController.active = false;
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(true);
        }

        isInventoryOpen = true;
        selectedWeaponIndex = 0;
        UpdateWeaponSelection();
    }

    private void CloseInventory()
    {
        Time.timeScale = 1.0f;
        _TPSCameraController.active = true;
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }

        isInventoryOpen = false;
        EquipWeapon(selectedWeaponIndex);
    }

    private void HandleWeaponSelection()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput > 0f)
        {
            selectedWeaponIndex = (selectedWeaponIndex + 1) % weaponIcons.Length;
            UpdateWeaponSelection();
        }
        else if (scrollInput < 0f)
        {
            selectedWeaponIndex = (selectedWeaponIndex - 1 + weaponIcons.Length) % weaponIcons.Length;
            UpdateWeaponSelection();
        }
    }

    private void UpdateWeaponSelection()
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (weaponIcons[i] != null)
            {
                weaponIcons[i].SetActive(i == selectedWeaponIndex);
            }
        }

        UpdateWeaponStats();
    }

    private void UpdateWeaponStats()
    {
        Weapon currentWeapon = _PlayerShootingController.weapons[selectedWeaponIndex];
        if (currentWeapon != null && weaponStatsText != null)
        {
            WeaponData data = currentWeapon.weaponData;
            weaponStatsText.text = $"Name: {data.weaponName}\nDamage: {data.damage}\nAmmo: {currentWeapon.GetCurrentAmmo()}/{data.maxAmmo}";
        }
    }

    private void EquipWeapon(int weaponIndex)
    {
        _PlayerShootingController.updatweapon(weaponIndex);
    }

    private void TryPickupWeapon()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        foreach (Collider obj in nearbyObjects)
        {
            Weapon weapon = obj.GetComponent<Weapon>();
            if (weapon != null)
            {
                AddWeaponToInventory(weapon);
                Destroy(obj.gameObject); // Remove the weapon from the scene after pickup
                break;
            }
        }
    }

    private void AddWeaponToInventory(Weapon weapon)
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (weaponIcons[i] == null)
            {
                weaponIcons[i] = Instantiate(weapon.gameObject, weaponHolder);
                _PlayerShootingController.weapons[i] = weapon;
                UpdateWeaponSelection();
                return;
            }
        }

        Debug.Log("Inventory full! Cannot pick up weapon.");
    }
/*
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
    */
}
