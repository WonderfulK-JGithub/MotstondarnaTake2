using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Theo
public class PlayButton : MonoBehaviour
{
    public void Play() // Button till starta spelet
    {
        //SceneManager.LoadScene("GameScene");
        SceneTransition.current.EnterScene(3);
    }
}
