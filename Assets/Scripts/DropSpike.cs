using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpike : MonoBehaviour
{
     [Header("优先拖动 否则自动查找")]
    public GameObject carHeart;    //自动寻找
    public GameManager gameManager;

    void Start()
    {
        if(carHeart == null)
            carHeart = CarHeart.TryGetGameObject();
        if(gameManager == null)
            gameManager = GameManager.TryGet();
    }

    // Update is called once per frame
    void Update()
    {
        if(carHeart.transform.position.x >= this.transform.position.x && gameManager.GetComponent<GameManager>().isStart)
            this.GetComponent<Rigidbody2D>().simulated = true;
    }
}
