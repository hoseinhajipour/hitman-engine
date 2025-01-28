using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<Item> items = new List<Item>();

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log($"Added: {item.itemName}, Price: {item.price}");
    }

    public void RemoveItem(string itemName)
    {
        var item = items.Find(i => i.itemName == itemName);
        if (item != null)
        {
            items.Remove(item);
            Debug.Log($"Removed: {item.itemName}");
        }
        else
        {
            Debug.LogWarning($"Item '{itemName}' not found.");
        }
    }

    public void PrintInventory()
    {
        Debug.Log("Inventory:");
        foreach (var item in items)
        {
            Debug.Log($"- {item.itemName} ({item.price})");
        }
    }
}
