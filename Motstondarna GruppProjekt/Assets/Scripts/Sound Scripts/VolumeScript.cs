using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Theo

public class VolumeScript : MonoBehaviour //Kunna sänka/Höja volymen och att den sparas så att man inte måste sänka varje gång man spelar det. /Theo
{
    [SerializeField] Slider VolumeSlider;

    private void Start() //om den inte har historik på att en person har ändrat volymen tidigare så sets volymen automatiskt på 1 eller 100% /Theo
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }

        else
        {
            Load();
        }
    }
    public void ChangeVolume() //Ändrar volymen på spelet /Theo
    {
        AudioListener.volume = VolumeSlider.value;
        Save();
    }

    private void Load() //Använder float baluen som är sparad i "musicVolume" keyname och använder det som volym /Theo
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 1);
    }
    private void Save() //För att kunna spara volymen så att man inte måste sänka den varje gång man spelar spelet. /Theo
    {
        PlayerPrefs.SetFloat("musicVolume", VolumeSlider.value);
    }
}
