using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAnimationController : MonoBehaviour
{
    private Level currentLevel; // 当前关卡的Level数据
    private Animator levelAnimator; // 关卡的动画组件

    // 从GMCmd中获取已加载的Level列表（需要先调用LoadLocalConfig加载）
    private List<Level> allLevels = new List<Level>(); // 实际应从GMCmd的LoadLocalConfig获取，这里简化

    void Awake()
    {
        levelAnimator = GetComponent<Animator>();
        GMCmd.InitUserData();

        // 关键：复制数据到 allLevels
        allLevels.Clear();
        if (GMCmd.userData?.levels != null)
        {
            foreach (var level in GMCmd.userData.levels)
            {
                allLevels.Add(new Level(level.levelId, level.animatorEnable));
            }
        }
        else
        {
            Debug.LogError("GMCmd.userData.levels 为空！");
        }

        // 现在 allLevels 应该有数据了
        Debug.Log($"allLevels 数量: {allLevels.Count}");

        int currentSceneBuildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int levelId = currentSceneBuildIndex; // 或 +1，取决于你的设计

        currentLevel = allLevels.Find(level => level.levelId == levelId);
        print(currentLevel.animatorEnable);
    }

    void Start()
    {
        // 检查是否需要播放动画
        if (currentLevel != null && currentLevel.animatorEnable)
        { 
            // 标记为“已播放过”，下次不再播放
            currentLevel.animatorEnable = false;

            print(currentLevel.animatorEnable + "ss");
            //// 保存配置（更新animatorEnable状态）
            GMCmd.SaveLocalConfigLevels(allLevels); // 把修改后的allLevels传进去保存
        }
        else
            GameObject.Destroy(levelAnimator);
    }
}
