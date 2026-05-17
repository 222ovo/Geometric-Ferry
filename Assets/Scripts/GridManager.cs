using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private List<GameObject> grids = new List<GameObject>(); // 初始化列表
    public LayerMask targetLayer;

    public void Start()
    {
        Find.FindGrid(grids);
    }
}
