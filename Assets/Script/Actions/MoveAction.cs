using UnityEngine;

[System.Serializable]
public class MoveAction : ActionBase
{
    public GameObject target;
    public Vector3 targetPosition; // Target position to move to
    public float duration;         // Duration of the movement in seconds

    public override void Execute(GameObject collider)
    {
        if (target == null)
        {
            Debug.LogError("MoveAction: Target GameObject is null!");
            return;
        }

        if (duration <= 0)
        {
            Debug.LogWarning("MoveAction: Duration must be greater than zero. Moving instantly.");
            target.transform.position = targetPosition;
            return;
        }

        // Start a coroutine for smooth movement
        target.GetComponent<MonoBehaviour>()?.StartCoroutine(MoveCoroutine(target));
    }

    private System.Collections.IEnumerator MoveCoroutine(GameObject target)
    {
        Vector3 startPosition = target.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            target.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        target.transform.position = targetPosition;
    }
}