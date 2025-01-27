using UnityEngine;

public class ActionTrigger : MonoBehaviour
{
    public ActionManager actionManager; // مدیر اکشن‌ها
    public GameObject targetObject;     // شیء هدف
    public TriggerType triggerType = TriggerType.OnTriggerEnter; // نوع تریگر
    public KeyCode triggerKey = KeyCode.Space; // کلید کیبورد برای تریگر
    public string triggerTag = "Player"; // تگ برای تریگر

    private void Update()
    {
        // بررسی تریگر بر اساس نوع
        switch (triggerType)
        {
            case TriggerType.OnKeyPress:
                if (Input.GetKeyDown(triggerKey))
                {
                    ExecuteActions();
                }
                break;

            case TriggerType.OnMouseClick:
                if (Input.GetMouseButtonDown(0)) // کلیک چپ موس
                {
                    CheckMouseClick();
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.OnTriggerEnter)
        {
            if (actionManager != null && other.gameObject == targetObject)
            {
                ExecuteActions();
            }
        }
        else if (triggerType == TriggerType.OnTag)
        {
            if (actionManager != null && other.CompareTag(triggerTag))
            {
                ExecuteActions();
            }
        }
    }

    private void CheckMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == targetObject)
            {
                ExecuteActions();
            }
        }
    }

    private void ExecuteActions()
    {
        if (actionManager != null && targetObject != null)
        {
            actionManager.RunActions(targetObject);
        }
    }
}