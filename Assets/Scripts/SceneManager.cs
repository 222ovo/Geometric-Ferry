using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public List<string> sceneName = new List<string> { "Main Menu" , "Level1", "Level2", "Level3", "Level4", "Level5", "Level6", "Level7" };

    public static void LoadNextScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            Debug.Log("已经是最后一个场景了");
        }
    }
    public static void LoadScene(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }


}
