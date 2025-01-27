using UnityEngine;

[System.Serializable]
public class RotateAction : ActionBase
{
    [Tooltip("The target rotation in degrees.")]
    public Vector3 targetRotation = Vector3.zero;

    [Tooltip("The GameObject to rotate. If null, the target passed to Execute will be used.")]
    public GameObject target;

    [Tooltip("The duration of the rotation in seconds.")]
    public float duration = 1f;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("RotateAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        if (duration <= 0)
        {
            Debug.LogWarning("RotateAction: Duration must be greater than zero. Rotating instantly.");
            target.transform.eulerAngles = targetRotation;
            onComplete?.Invoke();
            return;
        }

        target.GetComponent<MonoBehaviour>()?.StartCoroutine(RotateCoroutine(target, onComplete));
    }

    private System.Collections.IEnumerator RotateCoroutine(GameObject target, System.Action onComplete)
    {
        Quaternion startRotation = target.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            target.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        target.transform.rotation = endRotation;
        onComplete?.Invoke();
    }

}