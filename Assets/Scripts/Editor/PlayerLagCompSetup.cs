using UnityEditor;
using UnityEngine;
using Fusion;

public class PlayerLagCompSetup : EditorWindow
{
    [MenuItem("Fusion/Setup Player Lag Compensation")]
    public static void SetupPlayer()
    {
        string prefabPath = "Assets/Prefabs/Player Variant.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError("Could not find Player Variant prefab at " + prefabPath);
            return;
        }

        // Mở prefab stage để sửa
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

        try
        {
            // 1. Thêm HitboxRoot vào root nếu chưa có
            HitboxRoot root = prefabInstance.GetComponent<HitboxRoot>();
            if (root == null)
            {
                root = prefabInstance.AddComponent<HitboxRoot>();
                Debug.Log("Added HitboxRoot to player prefab.");
            }

            // 2. Tìm hoặc thêm Hitbox cho phần thân
            // Giả sử ta gắn vào root luôn vì CharacterController đang ở root
            Hitbox hitbox = prefabInstance.GetComponent<Hitbox>();
            if (hitbox == null)
            {
                hitbox = prefabInstance.AddComponent<Hitbox>();
                Debug.Log("Added Hitbox to player prefab.");
            }

            // Cấu hình Hitbox (mặc định bao phủ CharacterController)
            CharacterController cc = prefabInstance.GetComponent<CharacterController>();
            if (cc != null)
            {
                // Hitbox tự động lấy thông số từ Collider nếu ta ko set thủ công, 
                // nhưng với Fusion Hitbox tốt nhất là dùng Box hoặc Capsule
                // Ở đây ta để mặc định hoặc có thể tinh chỉnh box
            }

            // Lưu lại
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            Debug.Log("Successfully setup Lag Compensation components for: " + prefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }
    }
}
