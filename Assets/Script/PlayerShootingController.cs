using UnityEngine;

public class PlayerShootingController : MonoBehaviour
{
    public Weapon[] weapons; // Array of available weapons
    public Transform firePoint; // Point from which bullets are fired
    public KeyCode shootKey = KeyCode.Mouse0; // Left-click to shoot
    public KeyCode reloadKey = KeyCode.R; // Reload key

    private int currentWeaponIndex = 0;

    void Update()
    {
        // Switch weapons with number keys
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentWeaponIndex = i;
                Debug.Log("Switched to: " + weapons[currentWeaponIndex].weaponData.weaponName);
            }
        }

        // Shoot
        if (Input.GetKey(shootKey) && weapons[currentWeaponIndex].weaponData.isAutomatic ||
            Input.GetKeyDown(shootKey) && !weapons[currentWeaponIndex].weaponData.isAutomatic)
        {

            weapons[currentWeaponIndex].Shoot(firePoint);
        }

        // Reload
        if (Input.GetKeyDown(reloadKey))
        {
            weapons[currentWeaponIndex].Reload();
        }
    }
    public void updatweapon(int index)
    {
        currentWeaponIndex = index;
        Debug.Log("Switched to: " + weapons[currentWeaponIndex].weaponData.weaponName);
    }
}