using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

public class MenuCarAnimation : MonoBehaviour
{
    [Header("随机生成预制体障碍物")]
    public List<GameObject> Obstacles = new List<GameObject>(); //预制体列表
    public float spawnTime = 3f;    //生成间隔
    public float timer = 0; //计时器

    public Transform carHeart;
    public Transform camera;

    [Header("关卡面板")]
    public GameObject levelList;
    public List<GameObject> levels = new List<GameObject>();

    private GameObject instance;
    private int currentNum = 0;

    public Dictionary<string, ObjectPool<GameObject>> pools = new Dictionary<string, ObjectPool<GameObject>>();
    private Dictionary<string, GameObject> activeInstances = new Dictionary<string, GameObject>();

    // 从GMCmd中获取已加载的Level列表（需要先调用LoadLocalConfig加载）
    private List<Level> allLevels = new List<Level>(); // 实际应从GMCmd的LoadLocalConfig获取，这里简化
    private void Awake()
    {
        // 初始化 GMCmd.userData
        GMCmd.InitUserData(); // 这会调用 LoadLocalConfig() 加载存档

        // 注意：这里不再需要 GMCmd.LoadLocalConfig(allLevels);
        // 因为 InitUserData() 已经加载了存档，并更新了 GMCmd.level

        // 如果你需要 allLevels 数据用于其他逻辑，可以这样同步：
        allLevels.Clear();
        foreach (var level in GMCmd.userData.levels)
        {
            allLevels.Add(new Level(level.levelId, level.animatorEnable));
        }

        
    }

    void Start()
    {
        foreach (var Obstacle in Obstacles)
        {
            var key = Obstacle.name;
            var pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(Obstacle),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 50);

            pools[key] = pool;
            // 初始化 activeInstances 字典
            activeInstances[key] = null;
        }

        currentNum = Random.Range(0, Obstacles.Count);
        GMCmd.SaveLocalConfig(allLevels);
        print(allLevels.Count);
        print(GMCmd.level);
        for (int i = 0; i < levels.Count; i++)
        {
            // 关卡号 = i + 1
            bool isUnlocked = (i + 1) <= GMCmd.level;


            // 可选：设置不同颜色
            levels[i].GetComponent<Image>().color = isUnlocked ? Color.yellow : Color.gray;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CarHeartFllowCamera();
        RandomObstacleEmerge();
    }

    public void CarHeartFllowCamera()
    {
        if(carHeart.transform.position.x >= -6f)
        {
            camera.position = new Vector3(carHeart.transform.position.x + 6, camera.position.y, camera.position.z);
        }
    }

    //生成随机障碍物
    public void RandomObstacleEmerge()
    {
        timer += Time.deltaTime;
        if (timer < spawnTime) return;

        // 在生成下一个之前，先释放/归还当前显示的实例（如果有）
        var prevIndex = currentNum;
        var prevKey = Obstacles[prevIndex].name;
        if (activeInstances.TryGetValue(prevKey, out var prevInst) && prevInst != null)
        {
            // 归还到对应的对象池
            if (pools.ContainsKey(prevKey))
            {
                pools[prevKey].Release(prevInst);
            }
            else
            {
                Destroy(prevInst);
            }
            activeInstances[prevKey] = null;
        }

        // 重置计时并切换到下一个障碍物索引
        timer = 0;
        currentNum = (currentNum + 1) % Obstacles.Count;

        var prefab = Obstacles[currentNum];
        var key = prefab.name;

        // 从池中获取实例（第一次会通过 createFunc Instantiate）
        if (pools.ContainsKey(key))
        {
            instance = pools[key].Get();
        }
        else
        {
            instance = Instantiate(prefab);
        }

        // 记录并摆放
        if (instance != null)
        {
            activeInstances[key] = instance;
            instance.transform.position = new Vector3(camera.transform.position.x + 15, -0.92f, 0);
        }
    }

    public void OnClickGameStart()
    {
        levelList.SetActive(true);
    }

    public void OnClickGameExit()
    {
        // 在编辑器模式下，停止播放
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        // 在独立构建的游戏（PC、Mac、Linux等）中，退出应用程序
#else
            Application.Quit();
#endif
    }

    public void OnClickLevel1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        GetComponent<Animator>().SetTrigger("Start");
    }

    public void OnClickLevel2()
    {
        if (GMCmd.level > 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level2");
            GetComponent<Animator>().SetTrigger("Start");
        }
     }

    public void OnClickLevel3()
    {
        if (GMCmd.level > 2)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level3");
            GetComponent<Animator>().SetTrigger("Start");
        }
    }

    public void OnClickLevel4()
    {
        if (GMCmd.level > 3)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level4");
            GetComponent<Animator>().SetTrigger("Start");
        }
    }

    public void OnClickLevel5()
    {
        if (GMCmd.level > 4)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level5");
            GetComponent<Animator>().SetTrigger("Start");
        }
    }

    public void OnClickLevel6()
    {
        if (GMCmd.level > 5)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level6");
            GetComponent<Animator>().SetTrigger("Start");
        }
    }

    public void OnClickLevel7()
    {
        if (GMCmd.level > 6)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level7");
            GetComponent<Animator>().SetTrigger("Start");
        }
    }

    public void OnClickBack()
    {
        levelList.SetActive(false);
    }
}
