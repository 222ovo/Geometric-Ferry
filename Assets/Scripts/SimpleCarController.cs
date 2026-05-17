using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class SimpleCarController : MonoBehaviour
{
    [Header("控制参数")]
    [Tooltip("车轮的扭矩，控制加速能力")]
    public float wheelTorque = 1000f;  // 扭矩，不是速度
    [Tooltip("最大前进速度")]
    public float maxSpeed = 10f;

    [Header("调试信息")]
    [Tooltip("启用调试信息输出")]
    public bool showDebug = true;

    [Header("手动指定轮子")]
    public HingeJoint2D[] WheelJoint;  // 轮子连接点

    private Rigidbody2D carRigidbody;

    void Start()
    {
        // 获取车身刚体
        carRigidbody = GetComponent<Rigidbody2D>();
        if (carRigidbody == null)
        {
            Debug.LogError("CarController: 找不到Rigidbody2D组件！");
            return;
        }

        SetupWheelJoints();

    }

    void SetupWheelJoints()
    {
        // 配置左轮
        for (int i = 0; i < WheelJoint.Length; i++)
        {
            if (WheelJoint[i] != null)
            {
                WheelJoint[i].useMotor = true;  // 启用马达

                // 创建并配置马达
                JointMotor2D motor = WheelJoint[i].motor;
                motor.maxMotorTorque = wheelTorque;  // 设置最大扭矩
                WheelJoint[i].motor = motor;

                // 如果Connected Body未设置，自动连接
                if (WheelJoint[i].connectedBody == null)
                {
                    WheelJoint[i].connectedBody = carRigidbody;
                }
            }
        }
    }

    void Update()
    {

        // 获取输入
        float moveInput = UnityEngine.Input.GetAxis("Horizontal");

        // 控制马达
        ControlMotor(moveInput);

    }

    void ControlMotor(float input)
    {
        // 计算目标速度（单位：度/秒）
        // 这里用input来控制方向和速度大小
        float targetMotorSpeed = input * 500f;  // 基础速度，可调整

        // 速度限制
        float currentSpeed = carRigidbody.velocity.magnitude;
        if (currentSpeed > maxSpeed && Mathf.Abs(input) > 0.1f)
        {
            // 如果速度过快，减小马达速度
            targetMotorSpeed *= 0.5f;
        }

        // 应用马达设置轮子
        for (int i = 0; i < WheelJoint.Length; i++)
        {
            if (WheelJoint[i] != null)
            {
                JointMotor2D motor = WheelJoint[i].motor;
                motor.motorSpeed = targetMotorSpeed;  // 设置马达速度
                WheelJoint[i].motor = motor;
            }
        }

    }

}
