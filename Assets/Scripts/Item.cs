using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("自动寻找格子列表")]
    public List<GameObject> presetGrid = new List<GameObject>();  
    [Header("优先手动添加 否则自动查找")]
    public GameManager manager;

    [Header("物体初始位置")]
    public Vector3 initialPosition;

    [Header("物体状态")]
    public bool isDragging = false;
    public bool isConnected = false; // 是否已连接到小车

    [Header("物体当前所属的格子")]
    public GameObject currentPresetGrid;

    public void RegisterGrid(GameObject presetGrid)
    {
        presetGrid.GetComponent<Grid>().isFilled = true; // 标记格子已被占用
        presetGrid.GetComponent<Grid>().filledBy = this.gameObject; // 记录占用格子的物体
        currentPresetGrid = presetGrid;
    }

    public void CancelRegisterGrid()
    {
        for (int i = 0; i < presetGrid.Count; i++)
        {
            if (presetGrid[i].GetComponent<Grid>().filledBy != null)
            {
                if (presetGrid[i].GetComponent<Grid>().filledBy == this.gameObject)
                {
                    presetGrid[i].GetComponent<Grid>().isFilled = false; // 释放格子占用状态
                    presetGrid[i].GetComponent<Grid>().filledBy = null; // 清除占用格子的记录
                    currentPresetGrid = null;
                    break;
                }
            }
        }
    }

    public bool CheckGridFilled(int i)
    {
        if (presetGrid[i].GetComponent<Grid>().isFilled)
        {
            return true;//格子被占用 
        }
        return false;
    }

    /// <summary>
    /// 在 maxDistance 内查找离 worldPos 最近的、可吸附的格子下标（跳过 i>=1 且已被占用的格子）。
    /// 若没有候选则返回 -1。仍需遍历列表一次，与原先复杂度相同，但选的是「最近」而不是列表顺序第一个。
    /// </summary>
    protected int FindNearestPlaceableGridIndex(Vector3 worldPos, float maxDistance)
    {
        float maxSq = maxDistance * maxDistance;
        int bestIndex = -1;
        float bestSq = float.MaxValue;

        for (int i = 0; i < presetGrid.Count; i++)
        {
            GameObject cell = presetGrid[i];
            if (cell == null)
                continue;

            float sq = (worldPos - cell.transform.position).sqrMagnitude;
            if (sq > maxSq)
                continue;
            if (i >= 1 && CheckGridFilled(i))
                continue;

            if (sq < bestSq)
            {
                bestSq = sq;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
}

