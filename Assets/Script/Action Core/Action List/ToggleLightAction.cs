using UnityEngine;

[System.Serializable]
public class ToggleLightAction : ActionBase
{
    public GameObject target;
    public bool turnOn;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("ToggleLightAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        Light light = target.GetComponent<Light>();
        if (light == null)
        {
            Debug.LogError("ToggleLightAction: No Light component found on target!");
            onComplete?.Invoke();
            return;
        }

        light.enabled = turnOn;
        onComplete?.Invoke();
    }
}
