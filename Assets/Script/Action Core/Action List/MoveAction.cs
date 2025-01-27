using UnityEngine;

[ActionCategory("GameObject")]
[System.Serializable]
public class MoveAction : ActionBase
{
    public GameObject target;
    public Vector3 targetPosition; // Target position to move to
    public float duration;         // Duration of the movement in seconds

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("MoveAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        if (duration <= 0)
        {
            Debug.LogWarning("MoveAction: Duration must be greater than zero. Moving instantly.");
            target.transform.position = targetPosition;
            onComplete?.Invoke();
            return;
        }

        target.GetComponent<MonoBehaviour>()?.StartCoroutine(MoveCoroutine(target, onComplete));
    }


    private System.Collections.IEnumerator MoveCoroutine(GameObject target, System.Action onComplete)
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
        onComplete?.Invoke(); // Notify that the action is complete
    }
}