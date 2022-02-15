using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public void ExitButton() // Button till gå till MainMenu
    {
        //SceneManager.LoadScene("TitleScreen");
        SceneTransition.current.EnterScene(0);
    }
}
