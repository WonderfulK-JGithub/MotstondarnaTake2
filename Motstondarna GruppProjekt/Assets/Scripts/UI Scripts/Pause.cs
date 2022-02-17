using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject[] windows;

    public static bool gamePaused;

    public static AudioSource source;

    private void Awake()
    {
        gamePaused = false;
        source = GetComponent<AudioSource>();
    }

    public void OpenWindow(int window)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        gamePaused = true;

        source.Pause();

        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].SetActive(false);
        }
        windows[window].SetActive(true);
    }
    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        gamePaused = false;

        for (int i = 0; i < windows.Length; i++)
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
