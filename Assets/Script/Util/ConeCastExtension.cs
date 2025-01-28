using System.Collections.Generic;
using UnityEngine;

public static class ConeCastExtension
{
    public static RaycastHit[] ConeCastAll(Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle, LayerMask obstacleLayer)
    {
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin, maxRadius, direction, maxDistance);
        List<RaycastHit> coneCastHitList = new List<RaycastHit>();

        foreach (var hit in sphereCastHits)
        {
            Vector3 closestPoint;

            // Check if the collider supports ClosestPoint
            if (hit.collider is BoxCollider || hit.collider is SphereCollider || hit.collider is CapsuleCollider || (hit.collider is MeshCollider meshCollider && meshCollider.convex))
            {
                closestPoint = hit.collider.ClosestPoint(origin);
            }
            else
            {
                closestPoint = hit.collider.bounds.center; // Fallback to the collider's bounds center
            }

            Vector3 directionToHit = closestPoint - origin;
            float angleToHit = Vector3.Angle(direction, directionToHit);

            // Check if the hit is within the cone angle
            if (angleToHit < coneAngle / 2)
            {
                // Perform a raycast to check for obstacles
                if (!Physics.Raycast(origin, directionToHit.normalized, out RaycastHit obstacleHit, directionToHit.magnitude, obstacleLayer))
                {
                    // If no obstacle is blocking the view, add it to the list
                    coneCastHitList.Add(hit);
                }
                else
                {
                  //  Debug.Log($"Obstacle blocking view: {obstacleHit.collider.name}");
                }
            }
        }

        return coneCastHitList.ToArray();
    }
}
