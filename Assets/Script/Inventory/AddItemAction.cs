using UnityEngine;

[ActionCategory("Inventory")]
[System.Serializable]
public class AddItemAction : ActionBase
{
    [Tooltip("The Item to be added to the inventory.")]
    public Item item;

    public override void Execute(GameObject target, System.Action onComplete)
    {
        if (item == null)
        {
            Debug.LogError("AddItemAction: No item assigned to add!");
            onComplete?.Invoke();
            return;
        }

        var inventory = target.GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem(item);
            Debug.Log($"AddItemAction: Added {item.itemName} to inventory.");
        }
        else
        {
            Debug.LogError("AddItemAction: Target GameObject does not have an Inventory component!");
        }
        onComplete?.Invoke();
    }
}