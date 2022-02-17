using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Theo
public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip AmbientWind, Checkpoint, Click, Dash, JumpPad, K‰gglaDamage, Landa, PowerUp, Rolling, R‰nna, Skada, Laser÷gon, RocketFiende, Coins, Hoppa,GameOver,WaterSplash,LavaSplash,Spin; //Definerar audiclippen /Theo
    static AudioSource audioSrc;

    void Awake() //Parar ihop r‰tt variebel med motsvarande ljudfil
    {
        AmbientWind = Resources.Load<AudioClip>("AmbientWind");
        Checkpoint = Resources.Load<AudioClip>("CheckPoint");
        Click = Resources.Load<AudioClip>("Click");
        Dash = Resources.Load<AudioClip>("Dash2");
        JumpPad = Resources.Load<AudioClip>("JumpPad2");
        K‰gglaDamage = Resources.Load<AudioClip>("BowlingPin");
        Landa = Resources.Load<AudioClip>("Landa");
        PowerUp = Resources.Load<AudioClip>("PowerUp");
        Rolling = Resources.Load<AudioClip>("Rolling");
        R‰nna = Resources.Load<AudioClip>("R‰nna");
        Skada = Resources.Load<AudioClip>("PlayerDamage");
        Laser÷gon = Resources.Load<AudioClip>("Laser÷gon");
        RocketFiende = Resources.Load<AudioClip>("RocketFiende");
        Coins = Resources.Load<AudioClip>("Coins");
        Hoppa = Resources.Load<AudioClip>("Hoppa");
        GameOver = Resources.Load<AudioClip>("Game Over");
        WaterSplash = Resources.Load<AudioClip>("WaterSplash");
        LavaSplash = Resources.Load<AudioClip>("LavaSplash");
        Spin = Resources.Load<AudioClip>("Spin");


        audioSrc = GetComponent<AudioSource>();
    }
    public static void PlaySound(string clip) //Anv‰nder clip string value som en paramiter d‰r den parar ihop "AmbientWind" med korrekt ljudfil /Theo
    {
        switch (clip)
        {
            case "AmbientWind":
                audioSrc.PlayOneShot(AmbientWind);
                break;
            case "CheckPoint":
                audioSrc.PlayOneShot(Checkpoint);
                break;
            case "Click":
                audioSrc.PlayOneShot(Click);
                break;
            case "Dash":
                audioSrc.PlayOneShot(Dash);
                break;
            case "JumpPad":
                audioSrc.PlayOneShot(JumpPad);
                break;
            case "K‰gglaDamage":
                audioSrc.PlayOneShot(K‰gglaDamage);
                break;
            case "Landa":
                audioSrc.PlayOneShot(Landa);
                break;
            case "PowerUp":
                audioSrc.PlayOneShot(PowerUp);
                break;
            case "Rolling":
                audioSrc.PlayOneShot(Rolling);
                break;
            case "R‰nna":
                audioSrc.PlayOneShot(R‰nna);
                break;
            case "Skada":
                audioSrc.PlayOneShot(Skada);
                break;
            case "Laser÷gon":
                audioSrc.PlayOneShot(Laser÷gon);
                break;
            case "RocketFiende":
                audioSrc.PlayOneShot(RocketFiende);
                break;
            case "Coins":
                audioSrc.PlayOneShot(Coins);
                break;
            case "Hoppa":
                audioSrc.PlayOneShot(Hoppa);
                break;
            case "Game Over":
                audioSrc.PlayOneShot(GameOver);
                break;
            case "WaterSplash":
                audioSrc.PlayOneShot(WaterSplash);
                break;
            case "LavaSplash":
                audioSrc.PlayOneShot(LavaSplash);
                break;
            case "Spin":
                audioSrc.PlayOneShot(Spin);
                break;
        }
    }
}

//Fˆr att spela ljuden SoundManagerScript.PlaySound ("InsertName"); /Theo