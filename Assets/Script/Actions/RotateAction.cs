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

    public override void Execute(GameObject targetObject)
    {
        // Use the provided targetObject if no specific target is set
        GameObject rotationTarget = target != null ? target : targetObject;

        if (rotationTarget == null)
        {
            Debug.LogError("RotateAction: Target GameObject is null!");
            return;
        }

        if (duration <= 0)
        {
            Debug.LogWarning("RotateAction: Duration must be greater than zero. Rotating instantly.");
            rotationTarget.transform.eulerAngles = targetRotation;
            return;
        }

        // Start the rotation coroutine
        MonoBehaviour behaviour = rotationTarget.GetComponent<MonoBehaviour>();
        if (behaviour != null)
        {
            behaviour.StartCoroutine(RotateCoroutine(rotationTarget));
        }
        else
        {
            Debug.LogError("RotateAction: Target GameObject does not have a MonoBehaviour component to start the coroutine.");
        }
    }

    private System.Collections.IEnumerator RotateCoroutine(GameObject target)
    {
        Quaternion startRotation = target.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // Ensure t stays between 0 and 1
            target.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the final rotation is exact
        target.transform.rotation = endRotation;
    }
}