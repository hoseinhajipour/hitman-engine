using UnityEngine;

public class ActionTrigger : MonoBehaviour
{
    public ActionManager actionManager; // مدیر اکشن‌ها

    public TriggerType triggerType = TriggerType.OnStart; // نوع تریگر
    public KeyCode triggerKey = KeyCode.Space; // کلید کیبورد برای تریگر
    public string triggerTag = "Player"; // تگ برای تریگر
    public GameObject targetObject;     // شیء هدف


    void Start()
    {
        if (triggerType == TriggerType.OnStart)
        {
            ExecuteActions();
        }
    }
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

        if (triggerType == TriggerType.onGameObject)
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
        if (actionManager != null)
        {
            if(targetObject==null){
               targetObject=this.gameObject; 
            }
            actionManager.RunActions(targetObject);
        }
    }
}