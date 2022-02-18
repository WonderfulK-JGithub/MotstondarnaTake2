using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehavior : MonoBehaviour
{
    [SerializeField] GameObject waterSplashPS;
    [SerializeField] float mogusDrip = 69f;

    private void OnTriggerEnter(Collider other) //K-Js kod
    {
        if (other.gameObject.tag == "Coin" || other.gameObject.layer == 6) return; //Gjorde så att coin inte splashar - Max

        if(mogusDrip == 69f)SoundManagerScript.PlaySound("WaterSplash");
        else SoundManagerScript.PlaySound("LavaSplash");
        Destroy(Instantiate(waterSplashPS, other.transform.position, Quaternion.Euler(-90f,0f,0f)), 5f);

    }
}
