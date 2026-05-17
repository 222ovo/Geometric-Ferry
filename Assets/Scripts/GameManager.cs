using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum E_State
{
    BeforeStart,
    Start,
    Stop
}
public class GameManager : MonoBehaviour
{
    /// <summary>当前场景中的 GameManager（每场景至多一个；销毁时自动清空）。</summary>
    public static GameManager Instance { get; private set; }

    [Header("当前游戏状态")]
    public E_State currentState = E_State.BeforeStart;
    public bool isBeforeStart => currentState == E_State.BeforeStart;
    public bool isStart => currentState == E_State.Start;
    public bool isStop => currentState == E_State.Stop;

    [Header("自动查找组件")]
    private List<GameObject> Wheels = new List<GameObject>();
    private List<GameObject> CarBodys = new List<GameObject>();
    private GameObject Engine;
    private GameObject CarHeart;
    private Item carHeart;
    private FinishLine finishLine;
    private GameObject grid;

    [Header("手动放置按钮")]
    public GameObject startButton;
    public GameObject stopButton;
    public GameObject reStartButton;
    public GameObject reSetButton;

    [Header("控制胜利后跳转下一关的时间")]
    public float timer = 0f;
    public float spawnTime = 2f;

    [Header("手动设置关卡名列表")]
    public List<string> levelNames = new List<string> { "Level1", "Level2", "Level3","Level4","Level5","Level6","Level7" };
    private GameObject itemBox;

    public LayerMask groundLayer;
    public bool isRestore = false;

    /// <summary>OnClikStart 结束时拍摄，供 Replay 恢复（不整关重载）。</summary>
    private GameplaySnapshot _gameplaySnapshotAfterStart;

    public void OnClikStart()
    {
        if (!SetRight())
            return;
        _gameplaySnapshotAfterStart = GameplaySnapshot.Capture(this);
        CancelParent();
        ChinCarBody(CarHeart);

        foreach(Item item in Find.AllItems)
        {
            if (!item.isConnected)
            {
                item.gameObject.SetActive(false);
            }
            if(item.gameObject.CompareTag("Engine") || item.gameObject.CompareTag("CarBody"))
            {
                if(item.GetComponent<HingeJoint2D>() == null)
                    item.gameObject.AddComponent<HingeJoint2D>();
                HingeJoint2D hj = item.GetComponent<HingeJoint2D>();
                hj.connectedBody = CarHeart.GetComponent<Rigidbody2D>();
                JointAngleLimits2D limits = hj.limits;
                hj.useLimits = true;
                limits.max = 0;
                hj.limits = limits;
            }
        }
 
        ActiveRigidbody2D();
        InactiveGrid();
        ChangeStateTo(E_State.Start);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("GameManager: 场景中存在多个 GameManager，仅保留先加载的 Instance。");
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>优先使用 <see cref="Instance"/>；若尚未 Awake 则回退查找（脚本顺序不确定时）。</summary>
    public static GameManager TryGet()
    {
        if (Instance != null)
            return Instance;
        return FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //得到车心
        CarHeart = GameObject.FindWithTag("CarHeart");

        //得到车心的Item
        carHeart = CarHeart.GetComponent<Item>();

        //得到终点线
        finishLine = GameObject.Find("FinishLine").GetComponent<FinishLine>();

        grid = GameObject.Find("Grid");

    }

    // Update is called once per frame
    void Update()
    {
        GameWinning();

        BackMenu();
    }

    //是否连接完成

    public bool SetRight()
    {
        if (!carHeart.isConnected)
        {
            return false;
        }
        foreach (var item in Find.AllItems)
        {
            if (item.CompareTag("CarHeart") && item.isConnected)
            {
                CarHeart = item.gameObject;
            }
            else if (item.CompareTag("Engine") && item.isConnected)
            {
                Engine = item.gameObject;
            }
            else if (item.CompareTag("CarBody") && item.isConnected)
            {
                CarBodys.Add(item.gameObject);
            }
            else if (item.CompareTag("Wheel") && item.isConnected)
            {
                Wheels.Add(item.gameObject);
            }
        }
        return true;
    }

    //连接引擎
    //连接车体
    public void ChinCarBody(GameObject CarHeart)
    {
        foreach(GameObject component in CarBodys)
        {
            Item carBody = component.GetComponent<Item>();
            if (carBody.isConnected)
            {
                component.transform.SetParent(CarHeart.transform);
            }
        }
        if (Engine != null)
        {
            Item engine = Engine.GetComponent<Item>();
            if (engine.isConnected)
            {
                engine.transform.SetParent(CarHeart.transform);
            }
        }
        Rigidbody2D rb = CarHeart.GetComponent<Rigidbody2D>();
        rb.simulated = true;

        foreach(GameObject compnent in Wheels)
        {
            Item wheel = compnent.GetComponent<Item>();
            if(wheel.isConnected)
            {
                Rigidbody2D rbw = wheel.GetComponent<Rigidbody2D>();
                rbw.simulated = true;
                wheel.transform.SetParent(CarHeart.transform);
            }
        }
    }

    public void OnClickStop()
    {
        if (!isStop)
        {
            InactiveRigidbody2D();
            ChangeStateTo(E_State.Stop);
        }
    }

    public void OnClickReStart()
    {
        if (isStop)
        {
            ActiveRigidbody2D();
            ChangeStateTo(E_State.Start);
        }
    }

    public void GameWinning()
    {
        if(finishLine.isFinish)
        {
            AudioManagement.PlayAudio("Win");
            timer += Time.deltaTime;
            if (timer >= spawnTime)
            {
               transform.GetChild(0).gameObject.SetActive(true);
            }
            GameObject gameObject = GameObject.Find("Main Camera");
            gameObject.GetComponent<CameraZoom2D>().enabled = false;
        }
    }

    public void InactiveGrid()
    {
        grid.SetActive(false);
    }

    public void InactiveRigidbody2D()
    {
        foreach (Item item in Find.AllItems)
        {
            if (item.isConnected)
            {
                if(item.GetComponent<Rigidbody2D>() != null)
                {
                    Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
                    rb.simulated = false;
                }
            }
        }
    }

    public void ActiveRigidbody2D()
    {
        foreach (Item item in Find.AllItems)
        {
            if (item.isConnected)
            {
                print(item.name + "已连接");
                if (item.GetComponent<Rigidbody2D>() == null)
                {
                    print(item.name + "添加了刚体组件");
                    item.gameObject.AddComponent<Rigidbody2D>();
                }
                Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
                rb.simulated = true;
                print(item.name + "的刚体已激活");
            }
        }
    }

    public void Replay()
    {
        if (_gameplaySnapshotAfterStart != null)
        {
            grid.SetActive(true);
            _gameplaySnapshotAfterStart.Apply(this);
            return;
        }
        // SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BackMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Main Menu");
    }

    public void CancelParent()
    {
        foreach (Item item in Find.AllItems)
        {
            if (item.isConnected)
            {
                item.transform.SetParent(null);
            }
        }
    }

    public void ResetCar()
    {
        if (isStart && !isRestore)
        {
            RaycastHit2D hit = Physics2D.Raycast(CarHeart.transform.position, Vector2.down, 2.5f, groundLayer);
            if (hit)
            {
                CarHeart.transform.position = new Vector3(CarHeart.transform.position.x, CarHeart.transform.position.y + 2.5f, CarHeart.transform.position.z);
                CarHeart.transform.localEulerAngles = new Vector3(0, 0, -23);
                isRestore = true;
                reSetButton.SetActive(false);
            }
        }
    }

    public void ChangeStateTo(E_State nextState)
    {
        currentState = nextState;
        if(isBeforeStart)
        {
            AudioManagement.StopAllAudio();
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 7)
                AudioManagement.PlayAudio("Prepare");
            startButton.SetActive(true);
            reStartButton.SetActive(false);
            stopButton.SetActive(false);
            if(!isRestore)
                reSetButton.SetActive(false);
        }
        if(isStart)
        {
            startButton.SetActive(false);
            reStartButton.SetActive(false);
            stopButton.SetActive(true);
            AudioManagement.StopAudio("Prepare");
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 7)
            {
                AudioManagement.PlayAudio("Run");
            }
            else
                AudioManagement.PlayAudio("Start");
            if(!isRestore)
                reSetButton.SetActive(true);
        }
        if(isStop)
        {
            startButton.SetActive(false);
            reStartButton.SetActive(true);
            stopButton.SetActive(false);
            if(!isRestore)
                reSetButton.SetActive(false);
        }
    }
}
