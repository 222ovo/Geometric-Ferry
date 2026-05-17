using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ChinWheel : MonoBehaviour
{
    [Header("车轮和车身连接的距离")]
    public float radius;

    [Header("手动拖动 否则自动查找")]
    [FormerlySerializedAs("CarHeart")]
    public GameObject carHeart;

    public GameManager gameManager;

    [Header("车轮状态")]
    public bool isGet = false;

    private void Start()
    {
        if (carHeart == null)
            carHeart = CarHeart.TryGetGameObject();
        if(gameManager == null)
            gameManager = GameManager.TryGet();
    }
    private void Update()
    {
        WheelChin();
    }

    public void WheelChin()
    {
        if(carHeart == null)
            return;
        if(!gameManager.isStart)
            return;
        if(carHeart.GetComponent<CarHeart>().isBroken)
            return;
        //遍历所有组件 如果组件是车身 并且距离小于半径 并且车轮没有和车身连接 则连接车轮和车身
        foreach (Item item in Find.AllItems)
        {
            if(!isGet)
            {
                if(Vector3.Distance(item.transform.position,transform.position) < radius && (item.CompareTag("CarHeart") || item.CompareTag("CarBody")))
                {
                    HingeJoint2D hj = this.gameObject.AddComponent<HingeJoint2D>();
                    Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
                    hj.connectedBody = rb;
                    isGet = true;
                }
            }
            else if(isGet)
            {
                if(Vector3.Distance(item.transform.position,transform.position) < radius && (item.CompareTag("CarHeart") || item.CompareTag("CarBody")))
                {
                    if(Vector3.Distance(item.transform.position,transform.position) < Vector3.Distance(transform.position,this.GetComponent<HingeJoint2D>().connectedBody.transform.position))
                    {
                        this.GetComponent<HingeJoint2D>().connectedBody = item.GetComponent<Rigidbody2D>();
                    }
                }
            }
        }
    }
}
