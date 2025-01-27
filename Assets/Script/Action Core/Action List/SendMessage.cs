using UnityEngine;

[System.Serializable]
public class SendMessage : ActionBase
{
    [Tooltip("The target GameObject to send the message to.")]
    public GameObject target;

    [Tooltip("The name of the method to invoke on the target GameObject.")]
    public string methodName;

    [Tooltip("Optional parameter to send with the message.")]
    public string parameter;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        if (target == null)
        {
            Debug.LogError("ActionSendMessage: Target GameObject is null!");
            onComplete?.Invoke();
            return;
        }

        if (string.IsNullOrEmpty(methodName))
        {
            Debug.LogError("ActionSendMessage: Method name is not provided!");
            onComplete?.Invoke();
            return;
        }

        // Send the message to the target GameObject
        try
        {
            if (!string.IsNullOrEmpty(parameter))
            {
                target.SendMessage(methodName, parameter, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                target.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SendMessage: Failed to send message to {target.name}. Error: {e.Message}");
        }

        // Mark action as complete
        onComplete?.Invoke();
    }
}
