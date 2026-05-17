using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState
{
    MoveUp,
    StopUp,
    MoveDown,
    StopDown,
}
public class MovePlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager manager;
    public float timer = 0f;
    public float moveSpeed = 2f;
    public float moveTime = 2f;
    public float stopTime = 2f;
    public MoveState moveState = MoveState.MoveUp;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        manager = gameManager.GetComponent<GameManager>();
        rb = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(manager.isStart)
        {
            switch(moveState)
            {
                case MoveState.MoveUp:
                    MoveUp();
                    if(timer >= moveTime)
                    {
                        timer = 0f;
                        ChangeState();
                    }
                    break;
                case MoveState.StopUp:
                    Stop();
                    if(timer >= stopTime)
                    {
                        timer = 0f;
                        ChangeState();
                    }
                    break;
                case MoveState.MoveDown:
                    MoveDown();
                    if(timer >= moveTime)
                    {
                        timer = 0f;
                        ChangeState();
                    }
                    break;
                case MoveState.StopDown:
                    Stop();
                    if(timer >= stopTime)
                    {
                        timer = 0f;
                        ChangeState();
                    }
                    break;
            }
        }
    }

    public void ChangeState()
    {
        print("µ±Ç°×´Ì¬" + moveState);
            switch (moveState)
            {
                case MoveState.MoveUp:
                    moveState = MoveState.StopUp;
                    break;
                case MoveState.StopUp:
                    moveState = MoveState.MoveDown;
                    break;
                case MoveState.MoveDown:
                    moveState = MoveState.StopDown;
                    break;
                case MoveState.StopDown:
                    moveState = MoveState.MoveUp;
                    break;
            }
    }

    public void MoveUp()
    {
        rb.velocity = new Vector2(rb.velocity.x, moveSpeed);
        //transform.Translate(Vector2.up * Time.deltaTime * moveSpeed);
        timer += Time.deltaTime;
    }

    public void MoveDown()
    {
        rb.velocity = new Vector2(rb.velocity.x, -moveSpeed);
        timer += Time.deltaTime;
    }

    public void Stop()
    {
        timer += Time.deltaTime;
    }
}
