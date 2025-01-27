using UnityEngine;
[ActionCategory("Utility")]
[System.Serializable]
public class LogMessageAction : ActionBase
{
    [Tooltip("The message to log to the console.")]
    public string message = "Default log message";

    [Tooltip("The type of log message.")]
    public LogType logType = LogType.Log;

    public override void Execute(GameObject hit_target, System.Action onComplete)
    {
        switch (logType)
        {
            case LogType.Log:
                Debug.Log($"[LogMessageAction] {message}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"[LogMessageAction] {message}");
                break;
            case LogType.Error:
                Debug.LogError($"[LogMessageAction] {message}");
                break;
            case LogType.Assert:
                Debug.LogAssertion($"[LogMessageAction] {message}");
                break;
            case LogType.Exception:
                Debug.LogException(new System.Exception($"[LogMessageAction] {message}"));
                break;
        }

        onComplete?.Invoke(); // Notify that the action is complete
    }
}
