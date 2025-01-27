using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    private int currentAmmo;
    private float lastShotTime;
    private bool isReloading;

    void Start()
    {
        currentAmmo = weaponData.maxAmmo;
    }

    public void Shoot(Transform firePoint)
    {


        if (isReloading || Time.time < lastShotTime + 1f / weaponData.fireRate)
            return;

        Debug.Log("Shoot");
        Debug.Log("lastShotTime : "+lastShotTime);
        Debug.Log("fireRate :"+ (lastShotTime + 1f / weaponData.fireRate));
        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }


        // Play shoot sound
        if (weaponData.shootSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponData.shootSound, firePoint.position);
        }
        Debug.Log("shoot");
        // Handle bullet spread
        Vector3 shootDirection = firePoint.forward;
        if (weaponData.spreadAngle > 0)
        {
            shootDirection = GetSpreadDirection(firePoint);
        }

        // Raycast to detect hits
        if (Physics.Raycast(firePoint.position, shootDirection, out RaycastHit hit, weaponData.range))
        {
            Debug.Log("Hit: " + hit.collider.name);
            // Apply damage to the target
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(weaponData.damage);
            }
        }

        // Reduce ammo
        currentAmmo--;
        lastShotTime = Time.time;

        // Optional: Instantiate bullet prefab for visual effect
        if (weaponData.bulletPrefab != null)
        {
            Instantiate(weaponData.bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
        }
    }

    private Vector3 GetSpreadDirection(Transform firePoint)
    {
        float spread = Random.Range(-weaponData.spreadAngle, weaponData.spreadAngle);
        return Quaternion.Euler(spread, spread, 0) * firePoint.forward;
    }

    public void Reload()
    {
        isReloading = true;
        if (isReloading || currentAmmo == weaponData.maxAmmo)
            return;


        Invoke(nameof(FinishReload), weaponData.reloadTime);

        // Play reload sound
        if (weaponData.reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponData.reloadSound, transform.position);
        }
    }

    private void FinishReload()
    {
        currentAmmo = weaponData.maxAmmo;
        isReloading = false;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}