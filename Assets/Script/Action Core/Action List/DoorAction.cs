using UnityEngine;

[System.Serializable]
public class DoorAction : ActionBase
{
    [Tooltip("The target door to rotate.")]
    public GameObject door;

    [Tooltip("The duration of the door rotation.")]
    public float duration = 1f;

    [Tooltip("The radius within which the target must remain to keep the door open.")]
    public float proximityRadius = 5f;

    [Tooltip("The target object whose proximity to the door is checked.")]
    private GameObject target;

    private bool isOpen = false;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (door == null)
        {
            Debug.LogError("DoorAction: Door GameObject is null!");
            onComplete?.Invoke();
            return;
        }
        if (target == null)
        {
            target = hit_target;
        }

        Vector3 targetRotation = CalculateTargetRotation();
        MonoBehaviour coroutineHost = target.GetComponent<MonoBehaviour>(); // Host is the ActionManager or another script

        if (coroutineHost == null)
        {
            Debug.LogError("DoorAction: No MonoBehaviour found to host the coroutine!");
            onComplete?.Invoke();
            return;
        }

        coroutineHost.StartCoroutine(RotateDoor(targetRotation, onComplete));
        isOpen = !isOpen;

        if (isOpen)
        {
            coroutineHost.StartCoroutine(CheckProximityAndCloseDoor(coroutineHost));
        }
    }

    private Vector3 CalculateTargetRotation()
    {
        // محاسبه موقعیت محلی target نسبت به درب
        Vector3 doorToTarget = door.transform.InverseTransformPoint(target.transform.position);
        Vector3 targetRotation;

        if (!isOpen)
        {
            // اگر target در سمت راست درب باشد، درب به سمت راست باز شود، در غیر این صورت به سمت چپ
            targetRotation = doorToTarget.x > 0
                ? new Vector3(0, 90, 0) // باز شدن درب به سمت راست
                : new Vector3(0, -90, 0); // باز شدن درب به سمت چپ
        }
        else
        {
            // اگر بسته می‌شود، همیشه به موقعیت اولیه بازگردد
            targetRotation = Vector3.zero;
        }

        return targetRotation;
    }


    private System.Collections.IEnumerator RotateDoor(Vector3 targetRotation, System.Action onComplete)
    {
        Quaternion startRotation = door.transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            door.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        door.transform.localRotation = endRotation;
        onComplete?.Invoke();
    }

    private System.Collections.IEnumerator CheckProximityAndCloseDoor(MonoBehaviour coroutineHost)
    {
        while (isOpen)
        {
            if (target != null && Vector3.Distance(door.transform.position, target.transform.position) > proximityRadius)
            {
                yield return new WaitForSeconds(2f);

                if (Vector3.Distance(door.transform.position, target.transform.position) > proximityRadius)
                {
                    // Close the door
                    Vector3 targetRotation = Vector3.zero;
                    yield return coroutineHost.StartCoroutine(RotateDoor(targetRotation, null));
                    target = null;
                    isOpen = false;
                    yield break;
                }
            }

            yield return null;
        }
    }

}
