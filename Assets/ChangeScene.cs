using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : StateMachineBehaviour
{
    public float timer = 0f; // 计时器
    private bool hasLoadedNextScene = false; // 是否已经加载下一场景
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            timer += Time.deltaTime; // 增加计时器
             if (timer >= 1.5f)
             {
                if (!hasLoadedNextScene)
                {
                    hasLoadedNextScene = true; // 设置标志，避免重复加载
                    SceneManager.LoadNextScene();
                }
             }
    }

     //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
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
