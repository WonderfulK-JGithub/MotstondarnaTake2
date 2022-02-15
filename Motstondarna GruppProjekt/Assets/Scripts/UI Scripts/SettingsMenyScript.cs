using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Theo

public class SettingsMenyScript : MonoBehaviour
{
    public void Settings() //Settings meny knappen
    {
        SceneManager.LoadScene("SettingsMeny");
        SceneTransition.current.EnterScene(1);
    }
}
