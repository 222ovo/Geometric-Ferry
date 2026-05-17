using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Spike : MonoBehaviour
{
    //自动找到所有已经连接的组件
    [SerializeField] private LayerMask componentLayers;  // 在Inspector中勾选多个层级

    [Header("优先拖动 否则自动查找")]
    public GameObject carHeart;

    private static readonly float explosionForce = 10f; // 冲击的大小
    public void Start()
    {
        if (carHeart == null)
            carHeart = CarHeart.TryGetGameObject();
    }

    public void DamageCar()
    {
        foreach (Item component in Find.AllItems)
        {
            if (component != null)
            {
                if (component.transform.parent != null)
                {
                    component.transform.parent = null; // 断开连接
                    component.transform.localScale = new Vector3(1, 1, 1); // 确保组件缩放正确
                }
                if(component.GetComponent<Rigidbody2D>() == null)
                {
                    component.AddComponent<Rigidbody2D>(); // 添加刚体组
                }
                if(component.GetComponent<HingeJoint2D>() != null)
                {
                    Destroy(component.GetComponent<HingeJoint2D>()); // 移除连接组件
                }
                Rigidbody2D rb = component.GetComponent<Rigidbody2D>(); // 添加刚体组

                Vector2 direction = (Vector2)rb.transform.position - (Vector2)carHeart.transform.position;
                direction.Normalize(); // 归一化方向

                Vector2 force = direction * explosionForce;

                rb.AddForce(force, ForceMode2D.Impulse); // 瞬间的力（爆炸感）
            }
        }

        foreach(var wheel in Find.AllWheels)
        {
            ChinWheel cW = wheel.GetComponent<ChinWheel>();
            cW.isGet = false;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & componentLayers) != 0 && carHeart != null)
        {
            if (!carHeart.GetComponent<CarHeart>().isBroken)
            {
                DamageCar();
                carHeart.GetComponent<CarHeart>().isBroken = true; // 设置车身损坏状态
            }
        }
    }
}
