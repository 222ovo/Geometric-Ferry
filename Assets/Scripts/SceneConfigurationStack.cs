using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用栈保存场景配置，用于回滚场景配置回复当前场景中与拼装相关的配置（Transform 层级与局部坐标、Item、Grid）
/// 通过InstanceID查找游戏对象，并恢复其配置 仅同一场景、同一运行会话内有效；对象被销毁后无法恢复该选项
/// </summary>
public static class SceneConfigurationStack
{
    private static readonly Stack<SceneConfigSnapshot> Stack = new Stack<SceneConfigSnapshot>();

    public static int Depth => Stack.Count;

    public static void PushCurrentConfiguration()
    {
        Stack.Push(SceneConfigSnapshot.Capture());
    }

    public static bool TryPopAndRestore()
    {
        if (Stack.Count == 0)
            return false;
        Stack.Pop().Apply();
        return true;
    }

    public static void Clear() => Stack.Clear();
}

[Serializable]
public class TransformSnapshot
{
    public int instanceId;
    public int parentInstanceId;
    public int hierarchyDepth;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;
    public bool activeSelf;
}

[Serializable]
public class ItemStateSnapshot
{
    public int instanceId;
    public bool isConnected;
    public int currentGridInstanceId;
}

[Serializable]
public class GridStateSnapshot
{
    public int instanceId;
    public bool isFilled;
    public int filledByInstanceId;
}

[Serializable]
public class SceneConfigSnapshot
{
    public List<TransformSnapshot> Transforms = new List<TransformSnapshot>();
    public List<ItemStateSnapshot> Items = new List<ItemStateSnapshot>();
    public List<GridStateSnapshot> Grids = new List<GridStateSnapshot>();

    public static SceneConfigSnapshot Capture()
    {
        var snap = new SceneConfigSnapshot();

        var items = UnityEngine.Object.FindObjectsOfType<Item>();
        foreach (var item in items)
        {
            if (item == null)
                continue;

            var tr = item.transform;
            snap.Transforms.Add(new TransformSnapshot
            {
                instanceId = item.gameObject.GetInstanceID(),
                parentInstanceId = tr.parent != null ? tr.parent.gameObject.GetInstanceID() : 0,
                hierarchyDepth = GetHierarchyDepth(tr),
                localPosition = tr.localPosition,
                localEulerAngles = tr.localEulerAngles,
                localScale = tr.localScale,
                activeSelf = item.gameObject.activeSelf
            });

            snap.Items.Add(new ItemStateSnapshot
            {
                instanceId = item.gameObject.GetInstanceID(),
                isConnected = item.isConnected,
                currentGridInstanceId = item.currentPresetGrid != null ? item.currentPresetGrid.GetInstanceID() : 0
            });
        }

        var grids = UnityEngine.Object.FindObjectsOfType<Grid>();
        foreach (var grid in grids)
        {
            if (grid == null)
                continue;
            snap.Grids.Add(new GridStateSnapshot
            {
                instanceId = grid.gameObject.GetInstanceID(),
                isFilled = grid.isFilled,
                filledByInstanceId = grid.filledBy != null ? grid.filledBy.GetInstanceID() : 0
            });
        }

        return snap;
    }

    public void Apply()
    {
        Transforms.Sort((a, b) => a.hierarchyDepth.CompareTo(b.hierarchyDepth));
        foreach (var t in Transforms)
        {
            var go = ResolveGameObject(t.instanceId);
            if (go == null)
                continue;

            Transform tr = go.transform;
            Transform parent = null;
            if (t.parentInstanceId != 0)
            {
                var p = ResolveGameObject(t.parentInstanceId);
                parent = p != null ? p.transform : null;
            }

            tr.SetParent(parent, false);
            tr.localPosition = t.localPosition;
            tr.localEulerAngles = t.localEulerAngles;
            tr.localScale = t.localScale;
            go.SetActive(t.activeSelf);
        }

        foreach (var i in Items)
        {
            var go = ResolveGameObject(i.instanceId);
            if (go == null)
                continue;
            var item = go.GetComponent<Item>();
            if (item == null)
                continue;
            item.isConnected = i.isConnected;
            item.currentPresetGrid = i.currentGridInstanceId != 0
                ? ResolveGameObject(i.currentGridInstanceId)
                : null;
        }

        foreach (var g in Grids)
        {
            var go = ResolveGameObject(g.instanceId);
            if (go == null)
                continue;
            var grid = go.GetComponent<Grid>();
            if (grid == null)
                continue;
            grid.isFilled = g.isFilled;
            grid.filledBy = g.filledByInstanceId != 0 ? ResolveGameObject(g.filledByInstanceId) : null;
        }
    }

    private static int GetHierarchyDepth(Transform tr)
    {
        int d = 0;
        while (tr.parent != null)
        {
            d++;
            tr = tr.parent;
        }
        return d;
    }

    private static GameObject ResolveGameObject(int instanceId) => InstanceIdResolver.TryGetGameObject(instanceId);
}
