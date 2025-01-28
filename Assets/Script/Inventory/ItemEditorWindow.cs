using UnityEditor;
using UnityEngine;

public class ItemEditorWindow : EditorWindow
{
    private string itemName;
    private Sprite itemIcon;
    private float itemPrice;

    [MenuItem("Tools/Item Editor")]
    public static void ShowWindow()
    {
        GetWindow<ItemEditorWindow>("Item Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Item", EditorStyles.boldLabel);

        itemName = EditorGUILayout.TextField("Item Name", itemName);
        itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", itemIcon, typeof(Sprite), false);
        itemPrice = EditorGUILayout.FloatField("Item Price", itemPrice);

        if (GUILayout.Button("Create Item"))
        {
            CreateItem();
        }
    }

    private void CreateItem()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("Item name cannot be empty!");
            return;
        }

        // Create the Item ScriptableObject
        Item newItem = CreateInstance<Item>();
        newItem.itemName = itemName;
        newItem.icon = itemIcon;
        newItem.price = itemPrice;

        // Save the ScriptableObject as an asset
        string path = $"Assets/Items/{itemName}.asset";
        AssetDatabase.CreateAsset(newItem, path);
        AssetDatabase.SaveAssets();

        Debug.Log($"Item '{itemName}' created at {path}");
        itemName = "";
        itemIcon = null;
        itemPrice = 0f;
    }
}
