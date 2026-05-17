using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RestCamera : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Animator>().IsDestroyed())
        {
            mainCamera.orthographicSize = 5f;
            transform.position = new Vector3(0, 0, -10); // 重置为默认位置
            Destroy(this);
        }
    }
}
