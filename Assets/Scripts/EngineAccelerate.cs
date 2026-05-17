using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class EngineAccelerate : MonoBehaviour
{
    private GameManager gameManager; //自动寻找
    [Header("优先拖动 否则自动查找")]
    public GameObject carHeart;    //自动寻找

    [Header("加速设置")]
    public float maxSpeed = 10f;           // 车身最大速度限制
    public float accelerationForce = 1000f; // 加速力

    [Header("阻力设置")]
    public float decelerationRate = 200f;  // 减速速率（每秒减少多少速度）
    public float decelerationDuration = 1f; // 从加速到停止的总时间
    public AnimationCurve decelerationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("当前状态")]
    public float currentMotorSpeed = 0f;   // 当前轮子马达速度
    public float targetMotorSpeed = 0f;    // 目标马达速度

    private Coroutine decelerationCoroutine;
    private bool isAccelerating = false;

    private Item item;

    private bool isDecelerating = false;

    private void Start()
    {
        item = GetComponent<Item>();
        if(gameManager == null)
            gameManager = GameManager.TryGet();
        if(carHeart == null)
            carHeart = CarHeart.TryGetGameObject();
    }
    private void Update()
    {
        Accelerate();

        // 如果之前有按键按下，但现在没有任何加速键按下
        if (isAccelerating &&
            !UnityEngine.Input.GetKey(KeyCode.A) &&
            !UnityEngine.Input.GetKey(KeyCode.D))
        {
            // 如果没有按键按下，开始减速
            if (!isDecelerating)
            {
                StartDeceleration();
            }
        }
    }

    public void Accelerate()
    {
        if (gameManager.isStart && item.isConnected)
        {
            // 检测按键输入
            float inputDirection = 0f;
            inputDirection = UnityEngine.Input.GetAxisRaw("Horizontal");

            if (inputDirection != 0f)
            {
                // 计算加速速度（包含方向）
                float targetSpeed = CalculateTargetSpeed(inputDirection);

                // 应用加速
                ApplyAcceleration(targetSpeed);

                // 开始减速计时
                StartDeceleration();
            }
        }
    }

    float CalculateTargetSpeed(float direction)
    {
        // 根据方向计算目标速度
        float targetSpeed = accelerationForce * direction;

        if (carHeart != null)
        {
            Rigidbody2D rb = carHeart.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 获取当前速度的方向
                float currentSpeedMagnitude = rb.velocity.magnitude;
                Vector2 currentVelocity = rb.velocity;

                // 如果当前速度与加速方向一致
                if (Mathf.Sign(currentVelocity.x) == Mathf.Sign(direction) &&
                    Mathf.Abs(currentVelocity.x) > 0.1f)
                {
                    float currentSpeedX = Mathf.Abs(currentVelocity.x);
                    if (currentSpeedX > maxSpeed)
                    {
                        // 如果已经超速，减小加速力
                        float overSpeedRatio = currentSpeedX / maxSpeed;
                        targetSpeed *= Mathf.Clamp01(2f - overSpeedRatio);
                    }
                }
            }
        }

        return targetSpeed;
    }

    // 应用加速
    void ApplyAcceleration(float targetSpeed)
    {
        currentMotorSpeed = targetSpeed;
        targetMotorSpeed = targetSpeed;
        isAccelerating = true;

        foreach (Item wheel in Find.AllWheels)
        {
            if (wheel == null || !wheel.isConnected) continue;

            HingeJoint2D hj = wheel.gameObject.GetComponent<HingeJoint2D>();
            if (hj == null) continue;

            JointMotor2D motor = hj.motor;
            motor.motorSpeed = currentMotorSpeed;
            hj.motor = motor;
        }
    }

    // 开始减速
    void StartDeceleration()
    {
        // 停止之前的减速协程
        if (decelerationCoroutine != null)
        {
            StopCoroutine(decelerationCoroutine);
        }

        // 开始新的减速
        decelerationCoroutine = StartCoroutine(GradualDeceleration());
        isDecelerating = true;
    }

    // 修改减速协程，使其能够正确处理正负速度
    IEnumerator GradualDeceleration()
    {
        float startSpeed = currentMotorSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < decelerationDuration && Mathf.Abs(currentMotorSpeed) > 0.1f)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / decelerationDuration);

            // 使用曲线控制减速曲线
            float curveValue = decelerationCurve.Evaluate(t);

            // 计算当前速度，向0平滑减速
            currentMotorSpeed = Mathf.Lerp(startSpeed, 0f, t);

            // 应用速度
            ApplySpeedToAllWheels(currentMotorSpeed);

            yield return null;
        }

        // 确保最终速度为0
        currentMotorSpeed = 0f;
        ApplySpeedToAllWheels(0f);
        isAccelerating = false;
        isDecelerating = false;

        Debug.Log("减速完成");
        decelerationCoroutine = null;
    }

    // 紧急刹车
    public void EmergencyBrake()
    {
        if (decelerationCoroutine != null)
        {
            StopCoroutine(decelerationCoroutine);
        }

        currentMotorSpeed = 0f;
        isAccelerating = false;
        isDecelerating = false;
        ApplySpeedToAllWheels(0f);

        Debug.Log("紧急刹车");
    }

    // 方法2：每帧应用阻力效果（如果你不用协程）
    void ApplyResistance()
    {
        if (currentMotorSpeed <= 0f)
        {
            currentMotorSpeed = 0f;
            isAccelerating = false;
            return;
        }

        // 线性减速
        currentMotorSpeed -= decelerationRate * Time.deltaTime;

        // 确保不低于0
        if (currentMotorSpeed < 0f)
        {
            currentMotorSpeed = 0f;
            isAccelerating = false;
        }

        // 应用当前速度
        ApplySpeedToAllWheels(currentMotorSpeed);
    }
    // 应用速度到所有轮子
    void ApplySpeedToAllWheels(float speed)
    {
        foreach (Item wheel in Find.AllWheels)
        {
            if (wheel == null || !wheel.isConnected) continue;

            HingeJoint2D hj = wheel.gameObject.GetComponent<HingeJoint2D>();
            if (hj == null) continue;

            JointMotor2D motor = hj.motor;
            motor.motorSpeed = speed;
            hj.motor = motor;
        }
    }

    // 获取当前速度
    public float GetCurrentSpeed()
    {
        return currentMotorSpeed;
    }

    // 获取当前速度百分比
    public float GetSpeedPercentage()
    {
        return currentMotorSpeed / accelerationForce;
    }
}
