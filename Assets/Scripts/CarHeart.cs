using System.Collections.Generic;
using UnityEngine;

public class CarHeart : MonoBehaviour
{
    /// <summary>当前场景中的车心（每场景至多一个；销毁时自动清空）。</summary>
    public static CarHeart Instance { get; private set; }

    [Header("优先手动拖入 否则自动寻找")]
    public GameManager manager;
    [Header("车的状态")]
    public bool isBroken = false; // 车身是否损坏
    public bool isGrounded = false; // 是否在地面上
    [Header("地面层")]
    public LayerMask groundLayer; //地面层

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("CarHeart: 场景中存在多个 CarHeart，仅保留先加载的 Instance。");
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>优先使用 Instance；若尚未 Awake（脚本顺序）则回退到按标签查找。</summary>
    public static GameObject TryGetGameObject()
    {
        if (Instance != null)
            return Instance.gameObject;
        return GameObject.FindWithTag("CarHeart");
    }
    void Start()
    {
        if(manager == null)
            manager = GameManager.TryGet();
    }

    // Update is called once per frame
    void Update()
    {
        if(manager.isStart)
            GroundCheck();
    }

    //施工
    public void GroundCheck()
    {
        if (isBroken)
            return;
            
        foreach(Item wheel in Find.AllWheels)
        {
            if(!wheel.isConnected)
                continue;
            RaycastHit2D hit = Physics2D.Raycast(wheel.transform.position, Vector2.down,0.55f, groundLayer );
            //绘制射线

            Debug.DrawRay(wheel.transform.position, Vector2.down * 0.55f, hit.collider != null ? Color.green : Color.red);
            if (hit.collider == null || !hit.collider.CompareTag("Ground"))
            {
                isGrounded = false;
                return;
            }   
        }
        isGrounded = true;
    }
}
