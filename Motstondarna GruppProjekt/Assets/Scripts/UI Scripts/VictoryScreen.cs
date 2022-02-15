using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    public static VictoryScreen current;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        current = this;
    }
    public void BackToTitle()
    {
        PlayerPrefs.SetInt("progress", 0);
        Cursor.lockState = CursorLockMode.None;
        SceneTransition.current.EnterScene(0);
    }

    
    public void RePlay()
    {
        PlayerPrefs.SetInt("progress", 0);
        Cursor.lockState = CursorLockMode.None;
        SceneTransition.current.ReLoadScene();
    }

    public void Show()
    {
        anim.Play("VictoryScreen_Enter");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Pause.gamePaused = true;
        FindObjectOfType<Pause>().enabled = false;
    }
}
