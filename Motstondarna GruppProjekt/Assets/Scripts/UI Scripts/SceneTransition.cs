using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition current;

    int sceneIndex;

    Animator anim;
    void Awake()
    {
        current = this;
        anim = GetComponent<Animator>();
    }

    public void EnterScene(int _sceneIndex)
    {
        sceneIndex = _sceneIndex;
        anim.Play("SceneTransition_Exit");
    }

    public void ReLoadScene()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        anim.Play("SceneTransition_Exit");
    }

    public int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }


    void LoadScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
