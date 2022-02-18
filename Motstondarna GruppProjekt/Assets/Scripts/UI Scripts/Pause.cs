using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject[] windows;

    public static bool gamePaused;

    public static AudioSource source;

    private void Awake()
    {
        gamePaused = false; // spelet är inte pausat vid start - Anton
        source = GetComponent<AudioSource>();
    }

    public void OpenWindow(int window) // när spelet pausas öppnas pausfönstret - Anton
    {
        Cursor.lockState = CursorLockMode.None; // låser upp pekaren - Anton
        Cursor.visible = true; // visar pekaren - Anton
        Time.timeScale = 0; // pausar spelet så inget rör sig - Anton
        gamePaused = true;

        source.Pause();

        for (int i = 0; i < windows.Length; i++) // stänger ner alla andra pausfönster - Anton
        {
            windows[i].SetActive(false);
        }
        windows[window].SetActive(true); // visar pausmenyn - Anton
    }
    public void Resume() // när spelet återupptas - Anton
    {
        Cursor.lockState = CursorLockMode.Locked; // låster pekaren i mitten igen - Anton
        Cursor.visible = false; // gömmer pekaren - Anton
        Time.timeScale = 1; // avpausar spelet - Anton
        gamePaused = false;

        for (int i = 0; i < windows.Length; i++) // stänger alla fönster - Anton
        {
            windows[i].SetActive(false);
        }

        source.Play();
    }
    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneTransition.current.ReLoadScene();
        Time.timeScale = 1;
    }
    public void FullRestart()
    {
        PlayerPrefs.SetInt("progress", 0);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneTransition.current.ReLoadScene();
        Time.timeScale = 1;
    }

    public void Menu()
    {
        Time.timeScale = 1;
        SceneTransition.current.EnterScene(0);

        foreach (var item in FindObjectsOfType<CollectableCoin>())
        {
            item.isCollected = false;
        }

        SaveSystem.current.Save();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (windows[0].activeInHierarchy == true)
            {
                Resume();
            }
            else
            {
                OpenWindow(0);
            }
        }
    }
}
