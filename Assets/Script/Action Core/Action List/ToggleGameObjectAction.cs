using UnityEngine;

[System.Serializable]
public class ToggleGameObjectAction : ActionBase
{
    public GameObject target;
    public bool activate;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("ToggleGameObjectAction: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        target.SetActive(activate);
        onComplete?.Invoke();
    }
}
