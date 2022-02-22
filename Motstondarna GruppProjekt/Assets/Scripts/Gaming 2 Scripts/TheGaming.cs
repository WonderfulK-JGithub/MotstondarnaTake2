using UnityEngine;

public class TheGaming : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G))
        {
            Application.OpenURL("https://youtu.be/zHD85L9-s6E");
            Pause.source.mute = true;
            Invoke("gaming", 35);
        }
    }

    private void gaming()
    {
        Pause.source.mute = false;
    }
}

