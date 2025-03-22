using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMenuKey : MonoBehaviour
{
    public TransitionSettings transition;
    public float startDelay;

    public DemoLoadScene loadScene;

    public void LoadScene(string _sceneName)
    {
        TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {

            loadScene.LoadScene("IsMenuScene");

        }
    }
}
