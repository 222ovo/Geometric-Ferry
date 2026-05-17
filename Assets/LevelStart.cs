using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelStart : StateMachineBehaviour
{
    [SerializeField]protected GameObject mainCamera;
    [SerializeField]protected GameObject grid;
    private float timer = 0f; // 计时器
    public float spawnTime = 0f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var item in Find.AllItems)
        {
            item.gameObject.SetActive(false); // 禁用车心组件
        }
        grid = GameObject.Find("Grid");
        grid.SetActive(false); // 禁用网格
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime; // 增加计时器
        if (timer >= spawnTime) // 如果计时器达到3秒
        {
            foreach (var item in Find.AllItems)
            {
                item.gameObject.SetActive(true);
            }
            grid.SetActive(true);
            if(mainCamera == null)
                mainCamera = Camera.main.gameObject;
            Destroy(mainCamera.GetComponent<Animator>()); // 销毁LevelStart组件，防止重复执行
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
