using UnityEngine;

[System.Serializable]
public class DestroyAction : ActionBase
{
    [Tooltip("The GameObject to destroy. If null, the target passed to Execute will be destroyed.")]
    public GameObject target;

    [Tooltip("Delay before destroying the GameObject, in seconds.")]
    public float delay = 0f;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        GameObject objectToDestroy = target != null ? target : hit_target;

        if (objectToDestroy == null)
        {
            Debug.LogError("DestroyAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        if (delay > 0f)
        {
            MonoBehaviour mb = objectToDestroy.GetComponent<MonoBehaviour>();
            if (mb != null)
            {
                mb.StartCoroutine(DestroyAfterDelay(objectToDestroy, delay, onComplete));
            }
            else
            {
                Debug.LogError("DestroyAction: Target GameObject does not have a MonoBehaviour component!");
                onComplete?.Invoke();
            }
        }
        else
        {
            Object.Destroy(objectToDestroy);
            onComplete?.Invoke();
        }
    }

    private System.Collections.IEnumerator DestroyAfterDelay(GameObject obj, float delay, System.Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        Object.Destroy(obj);
        onComplete?.Invoke();
    }
}
