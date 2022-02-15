using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Theo
public class Fullscreen : MonoBehaviour
{
    public void SettFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
