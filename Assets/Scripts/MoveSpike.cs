using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpike : MonoBehaviour
{
    public GameObject spike; // 预制体
    public float moveSpeed = 2f;
    private Transform camera;

    private void Start()
    {
        camera = Camera.main.transform;
    }

    void Update()
    {
        if (camera.position.x - transform.position.x < 20f) // 当相机与当前对象的距离小于20时，Spike开始旋转
        {
            RollingSpike();
        }
        if(camera.position.x - transform.position.x < 35f)
            AudioManagement.PlayAudio("SpikeRoll");
        else
            AudioManagement.StopAudio("SpikeRoll");
    }

    public void RollingSpike()
    {
        // 从后往前遍历，避免删除后索引错乱
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            child.localPosition += Vector3.down * moveSpeed * Time.deltaTime;

            if (child.localPosition.y < -0.725f)
            {
                Destroy(child.gameObject);
                GameObject newSpike = Instantiate(spike); // 设置子对象
                newSpike.transform.SetParent(transform);
                newSpike.transform.localScale = new Vector3(0.5326052f, 0.04079258f, 0.56984f);
                newSpike.transform.localPosition = new Vector3(0.4989545f, 0.746f, 0);
            }
        }
    }
}
