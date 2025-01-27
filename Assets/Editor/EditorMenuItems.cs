using UnityEngine;
using UnityEditor;

public static class EditorMenuItems
{
    // Menu item for creating an Interaction object
    [MenuItem("GameObject/Hitman/Interaction", false, 10)]
    static void CreateInteractionObject(MenuCommand menuCommand)
    {
        // Create a new empty GameObject
        GameObject interactionObject = new GameObject("InteractionObject");

        // Add a SphereCollider component and set it as a trigger
        SphereCollider sphereCollider = interactionObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        // Add the InteractionSystem component
        interactionObject.AddComponent<InteractionSystem>();

        // Ensure the new GameObject is selected and focused in the hierarchy
        GameObjectUtility.SetParentAndAlign(interactionObject, menuCommand.context as GameObject);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(interactionObject, "Create " + interactionObject.name);

        // Select the newly created object
        Selection.activeObject = interactionObject;
    }

    // Menu item for creating a Player object from a prefab
    [MenuItem("GameObject/Hitman/Character/Player", false, 11)]
    static void CreatePlayerObject(MenuCommand menuCommand)
    {
        // Path to the Player prefab
        string prefabPath = "Assets/Prefabs/Character/Player.prefab";

        // Load the Player prefab
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (playerPrefab != null)
        {
            // Instantiate the Player prefab
            GameObject playerObject = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;

            // Ensure the new GameObject is selected and focused in the hierarchy
            GameObjectUtility.SetParentAndAlign(playerObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(playerObject, "Create " + playerObject.name);

            // Select the newly created object
            Selection.activeObject = playerObject;
        }
        else
        {
            Debug.LogError("Player prefab not found at path: " + prefabPath);
        }
    }

    // Menu item for creating an NPC object from a prefab
    [MenuItem("GameObject/Hitman/Character/NPC", false, 12)]
    static void CreateNPCObject(MenuCommand menuCommand)
    {
        // Path to the NPC prefab
        string prefabPath = "Assets/Prefabs/Character/NPC.prefab";

        // Load the NPC prefab
        GameObject npcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (npcPrefab != null)
        {
            // Instantiate the NPC prefab
            GameObject npcObject = PrefabUtility.InstantiatePrefab(npcPrefab) as GameObject;

            // Ensure the new GameObject is selected and focused in the hierarchy
            GameObjectUtility.SetParentAndAlign(npcObject, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(npcObject, "Create " + npcObject.name);

            // Select the newly created object
            Selection.activeObject = npcObject;
        }
        else
        {
            Debug.LogError("NPC prefab not found at path: " + prefabPath);
        }
    }
}