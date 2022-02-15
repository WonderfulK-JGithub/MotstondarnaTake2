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
    public void Setup(int score) // Poäng efter man har dött
    {
        print("setup");
        gameObject.SetActive(true);
        pointsText.text = score.ToString() + " POINTS"; // Visar "(poäng) POÄNG"
    }
    public void RestartButton() // Buton till starta om spelet
    {
        //SceneManager.LoadScene("GameScene");
        SceneTransition.current.EnterScene(2);
    }
    public void ExitButton() // Button till gå till TitleScreen
    {
        SceneManager.LoadScene("TitleScreen");
        SceneTransition.current.EnterScene(0);
    }
}
