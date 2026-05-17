using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraZoom2D : MonoBehaviour
{
    [Header("平滑设置")]
    public float smoothTime = 0.2f;        // 平滑时间
    public bool smoothZoom = true;         // 是否平滑过渡

    [Header("移动参数")]
    public float moveSpeed = 10f;
    public float minX = -20f;
    public float maxX = 30f;

    [Header("优先手动拖动组件 否则自动寻找所有组件")]
    public GameManager manager;
    public GameObject carHeart;

    void Start()
    { 
        if(manager == null)
            manager =  GameManager.TryGet();
        if (carHeart == null)
            carHeart = CarHeart.TryGetGameObject();
    }

    void Update()
    {
        CameraMove();
    }

    public void CameraMove()
    {
        if (!manager.isStart)
        {
            float input = 0f;
            if (Input.GetKey(KeyCode.Q))
                input = -1f;
            else if (Input.GetKey(KeyCode.E))
                input = 1f;
            else
                input = 0f;

            if (transform.position.x + input * Time.unscaledDeltaTime * moveSpeed < maxX && transform.position.x + input * Time.unscaledDeltaTime * moveSpeed > minX)
            {
                foreach (Item item in Find.AllItems)
                {
                    item.initialPosition = new Vector3(item.initialPosition.x + input * Time.unscaledDeltaTime * moveSpeed, item.initialPosition.y, item.initialPosition.z);   //物体复原位置移动
                }
                transform.Translate(input * Time.unscaledDeltaTime * moveSpeed, 0, 0);
            }
        }
        //如果游戏开始 则跟随车心移动
        if(manager.isStart)
        {
            transform.position = new Vector3(carHeart.transform.position.x+5.66f,carHeart.transform.position.y-1.6f,-10);
        }
    }
}
