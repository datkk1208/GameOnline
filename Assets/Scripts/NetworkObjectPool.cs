using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObjectPool : NetworkObjectProviderDefault
{
    private Dictionary<NetworkPrefabId, Stack<NetworkObject>> _pool = new Dictionary<NetworkPrefabId, Stack<NetworkObject>>();

    public override NetworkObjectAcquireResult AcquirePrefabInstance(NetworkRunner runner, in NetworkPrefabAcquireContext context, out NetworkObject instance)
    {
        // Thử lấy từ trong pool trước
        if (_pool.TryGetValue(context.PrefabId, out var stack) && stack.Count > 0)
        {
            instance = stack.Pop();
            instance.gameObject.SetActive(true);
            return NetworkObjectAcquireResult.Success;
        }

        // Nếu trong kho không có, tạo mới bằng logic gốc của Fusion
        return base.AcquirePrefabInstance(runner, in context, out instance);
    }

    public override void ReleaseInstance(NetworkRunner runner, in NetworkObjectReleaseContext context)
    {
        var instance = context.Object;
        if (instance != null)
        {
            // Tàng hình thay vì xóa
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(null);
            
            if (context.TypeId.IsPrefab)
            {
                var prefabId = context.TypeId.AsPrefabId;
                if (!_pool.TryGetValue(prefabId, out var stack))
                {
                    stack = new Stack<NetworkObject>();
                    _pool[prefabId] = stack;
                }
                stack.Push(instance);
            }
            else
            {
                // Nếu không phải prefab (như scene object), huỷ thật
                Destroy(instance.gameObject);
            }
        }
    }
}
