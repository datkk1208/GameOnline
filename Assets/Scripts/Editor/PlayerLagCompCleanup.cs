using UnityEditor;
using UnityEngine;
using Fusion;

public class PlayerLagCompCleanup : EditorWindow
{
    [MenuItem("Fusion/Cleanup Player Lag Compensation")]
    public static void CleanupPlayer()
    {
        string prefabPath = "Assets/Prefabs/Player Variant.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError("Could not find Player Variant prefab at " + prefabPath);
            return;
        }

        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

        try
        {
            bool changed = false;

            // Xoá HitboxRoot
            HitboxRoot root = prefabInstance.GetComponent<HitboxRoot>();
            if (root != null)
            {
                DestroyImmediate(root, true);
                changed = true;
                Debug.Log("Removed HitboxRoot from player prefab.");
            }

            // Xoá Hitbox
            Hitbox hitbox = prefabInstance.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                DestroyImmediate(hitbox, true);
                changed = true;
                Debug.Log("Removed Hitbox from player prefab.");
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                Debug.Log("Successfully cleaned up Lag Compensation components for: " + prefabPath);
            }
            else
            {
                Debug.Log("No Lag Compensation components found to cleanup.");
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }
    }
}
