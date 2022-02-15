using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Theo

public class VolumeScript : MonoBehaviour //Kunna s�nka/H�ja volymen och att den sparas s� att man inte m�ste s�nka varje g�ng man spelar det. /Theo
{
    [SerializeField] Slider VolumeSlider;

    private void Start() //om den inte har historik p� att en person har �ndrat volymen tidigare s� sets volymen automatiskt p� 1 eller 100% /Theo
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
    public void ChangeVolume() //�ndrar volymen p� spelet /Theo
    {
        AudioListener.volume = VolumeSlider.value;
        Save();
    }

    private void Load() //Anv�nder float baluen som �r sparad i "musicVolume" keyname och anv�nder det som volym /Theo
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 1);
    }
    private void Save() //F�r att kunna spara volymen s� att man inte m�ste s�nka den varje g�ng man spelar spelet. /Theo
    {
        PlayerPrefs.SetFloat("musicVolume", VolumeSlider.value);
    }
}
