using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineJump : MonoBehaviour
{
    public float jumpForce = 10f;          // 一段跳力度
    public float doubleJumpForce = 8f;     // 二段跳力度（可独立设置）
    public float jumpBufferTime = 0.2f;    // 跳跃缓冲时间
    public float coyoteTime = 0.1f;        // 土狼时间（离地后短暂可跳）

    private bool isJumpPressed = false;     // 当前帧跳跃键是否按下
    private bool wasJumpPressed = false;    // 上一帧跳跃键是否按下
    private float jumpBufferTimer = 0f;     // 跳跃缓冲计时器
    private float coyoteTimer = 0f;         // 土狼时间计时器
    private bool canDoubleJump = false;     // 是否可二段跳
    private bool isDoubleJumpUsed = false;  // 二段跳是否已使用
    private float groundedTimer = 0f;       // 落地计时器

    private Item engine;
    private GameManager gameManager; //自动寻找
    private GameObject carHeart;    //自动寻找

    public void Start()
    {
        InitializeReferences();
    }

    void Update()
    {
        // 1. 获取输入（使用GetKey代替GetKeyUp，响应更快）
        wasJumpPressed = isJumpPressed;
        isJumpPressed = UnityEngine.Input.GetKey(KeyCode.Space);
        bool jumpPressedThisFrame = isJumpPressed && !wasJumpPressed;  // 按键按下瞬间

        // 2. 更新计时器
        if (gameManager.isStart && engine.isConnected)
        {
            if (carHeart.GetComponent<CarHeart>().isGrounded)
            {
                coyoteTimer = coyoteTime;        // 落地时重置土狼时间
                groundedTimer += Time.deltaTime; // 记录落地时间

                // 落地后重置二段跳
                if (groundedTimer >= 0.1f)
                {
                    canDoubleJump = true;
                    isDoubleJumpUsed = false;
                }
            }
            else
            {
                coyoteTimer -= Time.deltaTime;    // 空中减少土狼时间
                groundedTimer = 0f;              // 重置落地计时
            }

            // 跳跃缓冲：提前按键记录
            if (jumpPressedThisFrame)
            {
                jumpBufferTimer = jumpBufferTime;
            }
            else
            {
                jumpBufferTimer -= Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (engine == null)
            print("engine is null");
        if (gameManager == null)
            print("gameManager is null");
        if (!gameManager.isStart || !engine.isConnected) return;

        var carHeartComp = carHeart.GetComponent<CarHeart>();
        var rb = carHeart.GetComponent<Rigidbody2D>();

        // 1. 一段跳：在地面或有土狼时间，且按下跳跃键
        if (jumpBufferTimer > 0 && (carHeartComp.isGrounded || coyoteTimer > 0) && !isDoubleJumpUsed)
        {
            // 重置跳跃缓冲
            jumpBufferTimer = 0f;

            // 重置垂直速度，避免下落时跳跃高度不一致
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // 应用跳跃力
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // 重置土狼时间
            coyoteTimer = 0f;

            // 允许二段跳
            canDoubleJump = true;

            Debug.Log("一段跳");
        }

        // 2. 二段跳：在空中，可二段跳，且按下跳跃键
        if (jumpBufferTimer > 0 && canDoubleJump && !carHeartComp.isGrounded && coyoteTimer <= 0)
        {
            // 重置跳跃缓冲
            jumpBufferTimer = 0f;

            // 重置垂直速度，使二段跳高度一致
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // 应用二段跳力（可设置不同力度）
            rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);

            // 标记二段跳已使用
            canDoubleJump = false;
            isDoubleJumpUsed = true;

            Debug.Log("二段跳");
        }

        // 3. 跳跃键抬起时减少上升速度（实现手感优化）
        if (!isJumpPressed && rb.velocity.y > 0)
        {
            // 短按跳得低，长按跳得高
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.8f);
        }
    }

    void InitializeReferences()
    {
        engine = GetComponent<Item>();
        gameManager = GameManager.TryGet(); // 安全调用（?.）
        carHeart = CarHeart.TryGetGameObject();

        // 检查引用是否有效，避免空引用
        if (gameManager == null) Debug.LogError("GameManager 未找到！");
        if (carHeart == null) Debug.LogError("CarHeart 未找到！");
    }
}
