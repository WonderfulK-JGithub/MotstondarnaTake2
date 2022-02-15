using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// Av Michal
public class GameOverScreen : MonoBehaviour
{
    public Text pointsText;

    private void Start()
    {
        Setup(PlayerPrefs.GetInt("score", 0));
    }
    public void Setup(int score) // Po�ng efter man har d�tt
    {
        print("setup");
        gameObject.SetActive(true);
        pointsText.text = score.ToString() + " POINTS"; // Visar "(po�ng) PO�NG"
    }
    public void RestartButton() // Buton till starta om spelet
    {
        //SceneManager.LoadScene("GameScene");
        SceneTransition.current.EnterScene(2);
    }
    public void ExitButton() // Button till g� till TitleScreen
    {
        SceneManager.LoadScene("TitleScreen");
        SceneTransition.current.EnterScene(0);
    }
}
