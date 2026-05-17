using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("优先拖动 否则自动查找")]
    public GameObject carHeart;
    public bool isFinish = false;//是否到达终点

    private static readonly float finishRadius = 2f;//检测到达终点的距离半径
    // 从GMCmd中获取已加载的Level列表（需要先调用LoadLocalConfig加载）
    private List<Level> allLevels = new List<Level>(); // 实际应从GMCmd的LoadLocalConfig获取，这里简化


    private void Awake()
    {
        // 确保 GMCmd.userData 已初始化
        GMCmd.InitUserData();

        foreach (var level in GMCmd.userData.levels)
        {
            allLevels.Add(new Level(level.levelId, level.animatorEnable)); // 深拷贝
        }

        // 加载本地配置（此时 allLevels 长度和 UserData.levels 一致）
        GMCmd.LoadLocalConfig(allLevels);
    }

    private void Start()
    {
        if (carHeart == null)
            carHeart = CarHeart.TryGetGameObject();
    }

    private void Update()
    {
        FinishCheck();
    }

    public void FinishCheck()
    {
        if (carHeart != null)
        {
            if(Vector3.Distance(carHeart.transform.position,transform.position) <= finishRadius && !isFinish)
            {
                Debug.Log("Finish!");
                GMCmd.SaveLocalConfig(allLevels);
                isFinish = true;
            }
        }
    }
}
