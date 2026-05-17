using UnityEngine;

/// <summary>
/// 用 InstanceID 查找 GameObject；不依赖 FindObjectFromInstanceID（部分 Unity 变体下该 API 不可用）。
/// </summary>
public static class InstanceIdResolver
{
    public static GameObject TryGetGameObject(int instanceId)
    {
        if (instanceId == 0)
            return null;

        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go != null && go.GetInstanceID() == instanceId)
                return go;
        }

        return null;
    }
}
