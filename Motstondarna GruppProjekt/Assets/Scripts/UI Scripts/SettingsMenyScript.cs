using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Theo

public class SettingsMenyScript : MonoBehaviour
{
    private void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume", 1);
    }

    public void Settings() //Settings meny knappen
    {
        SceneManager.LoadScene("SettingsMeny");
        SceneTransition.current.EnterScene(1);
    }
}
