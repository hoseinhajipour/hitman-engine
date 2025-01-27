using UnityEngine;

public class ProjectileBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 50f;
    public float damage = 10f;
    public float lifetime = 5f; // Time before the bullet is destroyed
    public LayerMask hitLayers; // Layers the bullet can hit

    private Vector3 direction;

    void Start()
    {
        // Destroy the bullet after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet forward
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the bullet hit an object on the specified layers
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            Debug.Log("Hit: " + other.name);

            // Apply damage to the target
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }

            // Optional: Spawn hit effect
            SpawnHitEffect(transform.position, transform.forward);

            // Destroy the bullet
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        // Example: Instantiate a particle effect or decal at the hit position
        // GameObject hitEffect = Instantiate(hitEffectPrefab, position, Quaternion.LookRotation(normal));
        // Destroy(hitEffect, 2f); // Clean up after 2 seconds
    }
}