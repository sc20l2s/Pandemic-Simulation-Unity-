using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private static int targetFrames = 100;//since our movement is based on fps
    //set a "limit" to at least get a bit of consistency
    
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrames;
    }


    public void RestartSim()//put in camera so that we do not have to attach entire level to botton
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
