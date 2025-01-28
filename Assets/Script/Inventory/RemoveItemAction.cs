using UnityEngine;

[ActionCategory("Inventory")]
[System.Serializable]
public class RemoveItemAction : ActionBase
{
    [Tooltip("The name of the Item to remove from the inventory.")]
    public string itemName;

    public override void Execute(GameObject target, System.Action onComplete)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogError("RemoveItemAction: Item name is empty or null!");
            onComplete?.Invoke();
            return;
        }

        var inventory = target.GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.RemoveItem(itemName);
            Debug.Log($"RemoveItemAction: Removed {itemName} from inventory.");
        }
        else
        {
            Debug.LogError("RemoveItemAction: Target GameObject does not have an Inventory component!");
        }
        onComplete?.Invoke();
    }
}