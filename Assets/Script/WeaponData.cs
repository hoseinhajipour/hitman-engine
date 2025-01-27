using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ShootingSystem/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float fireRate; // Shots per second
    public float range;
    public int maxAmmo;
    public float reloadTime;
    public float spreadAngle; // Bullet spread in degrees
    public bool isAutomatic; // Full-auto or semi-auto
    public GameObject bulletPrefab; // Optional: For projectile-based weapons
    public AudioClip shootSound;
    public AudioClip reloadSound;
}