using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverScene : MonoBehaviour
{
    public void OnClickBack()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
