using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 鍦烘櫙鍐呮煡鎵句笌 Item / 杞瓙 缂撳瓨銆傛崲鍦烘櫙鎴栧姩鎬佸鍒? Item 鏃堕渶 <see cref="InvalidateItemsCache"/>銆?
/// </summary>
public static class Find
{
    static readonly List<Item> ItemCache = new List<Item>();
    static ReadOnlyCollection<Item> ItemCacheReadOnly;

    static readonly List<Item> WheelCache = new List<Item>();
    static ReadOnlyCollection<Item> WheelCacheReadOnly;

    static bool cacheDirty = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        ItemCache.Clear();
        ItemCacheReadOnly = null;
        WheelCache.Clear();
        WheelCacheReadOnly = null;
        cacheDirty = true;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cacheDirty = true;
    }

    /// <summary>杩愯鏃剁敓鎴?/閿�姣? Item 鎴栬疆瀛愬悗锛岄渶绔嬪埢涓庣紦瀛樹竴鑷存椂璋冪敤銆?</summary>
    public static void InvalidateItemsCache()
    {
        cacheDirty = true;
    }

    static void RebuildCaches()
    {
        ItemCache.Clear();
        WheelCache.Clear();

        Item[] found = Object.FindObjectsOfType<Item>(true);
        ItemCache.AddRange(found);

        foreach (Item item in found)
        {
            if (item != null && item.CompareTag("Wheel"))
                WheelCache.Add(item);
        }

        cacheDirty = false;
    }

    static void EnsureCacheFresh()
    {
        if (cacheDirty)
            RebuildCaches();
    }

    /// <summary>确保AllItems总是最新的</summary>
    public static IReadOnlyList<Item> AllItems
    {
        get
        {
            EnsureCacheFresh();
            return ItemCacheReadOnly ??= ItemCache.AsReadOnly();
        }
    }

    /// <summary>确保AllWheels总是最新的///</summary>
    public static IReadOnlyList<Item> AllWheels
    {
        get
        {
            EnsureCacheFresh();
            return WheelCacheReadOnly ??= WheelCache.AsReadOnly();
        }
    }

    public static void FindGrid(List<GameObject> presetGrid)
    {
        presetGrid.Clear();
        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
            return;

        for (int i = 0; i < grid.transform.childCount; i++)
            presetGrid.Add(grid.transform.GetChild(i).gameObject);
    }
}
