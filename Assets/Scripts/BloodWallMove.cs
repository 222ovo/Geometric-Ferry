using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodWallMove : MonoBehaviour
{
    public float moveSpeed = 5f; // 血墙移动速度
    private GameManager gameManager; // 游戏管理器引用
    public Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        if(gameManager == null)
            gameManager = GameManager.TryGet();
        if(camera == null)
            camera = Camera.main.transform;
        if(gameManager.isStart)
        {
            AudioManagement.StopAudio("Start");
            AudioManagement.PlayAudio("Run");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.isStart)
        {
            if (camera.position.x - transform.position.x < 20f) // 当相机与当前对象的距离小于20时，Spike开始旋转
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
            else
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 4f);
        }
    }
}
