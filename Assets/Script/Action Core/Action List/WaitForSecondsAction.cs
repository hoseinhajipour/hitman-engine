using UnityEngine;

[System.Serializable]
public class WaitForSecondsAction : ActionBase
{
    [Tooltip("The duration to wait in seconds.")]
    public float duration = 1.0f;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        // Use the current GameObject if hit_target is null
        if (hit_target == null)
        {
            Debug.LogError("WaitForSecondsAction: hit_target is null. Please provide a valid target GameObject.");
            onComplete?.Invoke();
            return;
        }

        MonoBehaviour monoBehaviour = hit_target.GetComponent<MonoBehaviour>();
        if (monoBehaviour == null)
        {
            Debug.LogError("WaitForSecondsAction: No MonoBehaviour found on the target to start a coroutine!");
            onComplete?.Invoke();
            return;
        }

        // Start the coroutine to wait
        monoBehaviour.StartCoroutine(WaitCoroutine(onComplete));
    }

    private System.Collections.IEnumerator WaitCoroutine(System.Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke(); // Notify that the wait is complete
    }
}
