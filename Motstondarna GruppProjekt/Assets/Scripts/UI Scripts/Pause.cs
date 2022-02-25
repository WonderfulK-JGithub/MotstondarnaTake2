using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject[] windows;

    public static bool gamePaused;

    public static AudioSource source;

    private void Awake()
    {
        gamePaused = false; // spelet �r inte pausat vid start - Anton
        source = GetComponent<AudioSource>();
    }

    public void OpenWindow(int window) // n�r spelet pausas �ppnas pausf�nstret - Anton
    {
        Cursor.lockState = CursorLockMode.None; // l�ser upp pekaren - Anton
        Cursor.visible = true; // visar pekaren - Anton
        Time.timeScale = 0; // pausar spelet s� inget r�r sig - Anton
        gamePaused = true;

        source.Pause();

        for (int i = 0; i < windows.Length; i++) // st�nger ner alla andra pausf�nster - Anton
        {
            windows[i].SetActive(false);
        }
        windows[window].SetActive(true); // visar pausmenyn - Anton
    }
    public void Resume() // n�r spelet �terupptas - Anton
    {
        Cursor.lockState = CursorLockMode.Locked; // l�ster pekaren i mitten igen - Anton
        Cursor.visible = false; // g�mmer pekaren - Anton
        Time.timeScale = 1; // avpausar spelet - Anton
        gamePaused = false;

        for (int i = 0; i < windows.Length; i++) // st�nger alla f�nster - Anton
        {
            windows[i].SetActive(false); // g�mmer pausf�nstret - Anton
        }

        source.Play();
    }
    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneTransition.current.ReLoadScene(); // kallar p� k-js fina scenetransition och ber den att starta om - Anton
        Time.timeScale = 1; // avpausar spelet - Anton
    }
    public void FullRestart()
    {
        PlayerPrefs.SetInt("progress", 0); // �terst�ller framsteg - Anton
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Restart(); // startar om - Anton
    }

    public void Menu()
    {
        Time.timeScale = 1; // avpausar spelet - Anton
        PlayerPrefs.SetInt("progress", 0); // �terst�ller framsteg - Anton
        SceneTransition.current.EnterScene(3); // kallar p� k-js fina scenetransition och ber den att �ppna menyscenen - Anton

        foreach (var item in FindObjectsOfType<CollectableCoin>())
        {
            item.isCollected = false;
        }

        SaveSystem.current.Save();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // n�r ESC trycks - Anton
        {
            if (windows[0].activeInHierarchy == true)
            {
                Resume(); // avpausar den om spelet �r pausat - Anton
            }
            else
            {
                OpenWindow(0); // annars pausar den - Anton
            }
        }
    }
}
