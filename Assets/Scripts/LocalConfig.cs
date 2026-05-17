//用于json序列化和反序列化
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
//用于文件读写
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
//Application.persistentDataPath配置在这里

public class LocalConfig
{
    //保存用户数据文本
    public static void SaveUserData(UserData userData)
    {
        if(!File.Exists(Application.persistentDataPath + "/users"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/users");
        }
        string jsonData = JsonConvert.SerializeObject(userData);
        //将JSON字符串写入文件中(文件名为userData.name)
        File.WriteAllText(Application.persistentDataPath + string.Format("/users/{0}.json", userData.name),jsonData);
    }

    public static UserData LoadUserData(string userName)
    {
        string path = Application.persistentDataPath + string.Format("/users/{0}.json", userName);
        //检查用户配置文件是否存在
        if(File.Exists(path))
        {
            //从文本中加载JSON字符串
            string jsonData = File.ReadAllText(path);
            //将JSON字符串转换为用户内存数据
            UserData userData = JsonConvert.DeserializeObject<UserData>(jsonData);
            return userData;
        }
        else
        {
            return new UserData();
        }
    }
}

public class GMCmd
{
    public static int level = 1;
    public static UserData userData; // 静态成员，全局共享

    // 初始化方法：确保 userData 有值
    public static void InitUserData()
    {
        if (userData == null)
        {
            LoadLocalConfig(); // 初始化时加载存档
        }
    }

    public static void SaveLocalConfig(List<Level> levels)
    {
        // 【关键】去掉这里的局部变量声明，直接使用静态成员
        if (userData == null)
        {
            userData = LocalConfig.LoadUserData("ccc");
        }
        // 如果本地也没有存档，就新建一个
        if (userData == null)
        {
            userData = new UserData();
            userData.name = "ccc";
            userData.level = 1;
            userData.levels = new List<Level>();
        }

        // 更新最高关卡进度
        int currentLevelId = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        if (currentLevelId > userData.level)
        {
            Debug.Log($"过关！新进度: {currentLevelId}");
            userData.level = currentLevelId;
        }

        // 用传入的 levels 覆盖存档中的 levels
        userData.levels = new List<Level>();
        foreach (var lvl in levels)
        {
            userData.levels.Add(new Level(lvl.levelId, lvl.animatorEnable));
        }

        // 保存到本地
        LocalConfig.SaveUserData(userData);
        // 更新静态 level
        level = userData.level;
        Debug.Log($"保存成功。当前最高关卡: {level}, 关卡数: {userData.levels.Count}");
    }

    //保存数据，不保存关卡解锁
    public static void SaveLocalConfigLevels(List<Level> levels)
    {
        // 【关键】去掉这里的局部变量声明，直接使用静态成员
        if (userData == null)
        {
            userData = LocalConfig.LoadUserData("ccc");
        }
        // 如果本地也没有存档，就新建一个
        if (userData == null)
        {
            userData = new UserData();
            userData.name = "ccc";
            userData.level = 1;
            userData.levels = new List<Level>();
        }

        // 用传入的 levels 覆盖存档中的 levels
        userData.levels = new List<Level>();
        foreach (var lvl in levels)
        {
            userData.levels.Add(new Level(lvl.levelId, lvl.animatorEnable));
        }

        // 保存到本地
        LocalConfig.SaveUserData(userData);
    }

    public static void LoadLocalConfig(List<Level> levels)
    {
        // 【关键】去掉局部变量，直接操作静态成员
        userData = LocalConfig.LoadUserData("ccc");

        // 如果存档为空，初始化默认数据
        if (userData == null || userData.levels.Count <= 0)
        {
            Debug.Log("本地存档为空，创建默认存档。");
            userData = new UserData
            {
                name = "ccc",
                level = 1,
                levels = new List<Level>(),
            };
            for (int i = 0; i < 7; i++)
            {
                userData.levels.Add(new Level(i + 1, true));
            }
            LocalConfig.SaveUserData(userData);
        }

        // 更新静态 level
        level = userData.level;

        // 将存档的 levels 数据同步到传入的列表中
        levels.Clear();
        foreach (var savedLevel in userData.levels)
        {
            levels.Add(new Level(savedLevel.levelId, savedLevel.animatorEnable));
        }

        Debug.Log($"加载成功。最高解锁关卡: {level}, 同步了 {levels.Count} 个关卡的数据。");
    }

    // 重载：提供一个无参数的 LoadLocalConfig，只更新静态数据，不传入列表
    public static void LoadLocalConfig()
    {
        userData = LocalConfig.LoadUserData("ccc");
        if (userData == null || userData.levels.Count <= 0)
        {
            Debug.Log("本地存档为空，创建默认存档。");
            userData = new UserData
            {
                name = "ccc",
                level = 1,
                levels = new List<Level>(),
            };
            for (int i = 0; i < 7; i++)
            {
                userData.levels.Add(new Level(i + 1, true));
            }
            LocalConfig.SaveUserData(userData);
        }
        level = userData.level;
        Debug.Log($"加载成功（无参数）。最高解锁关卡: {level}");
    }
}
public class UserData
{
    public string name;
    public int level;
    public List<Level> levels = new List<Level>();
}

public class Level
{
    public int levelId;
    public bool animatorEnable = true;

    public Level(int id, bool animatorEnable)
    {
        this.levelId = id;
        this.animatorEnable = animatorEnable;
    }
}
