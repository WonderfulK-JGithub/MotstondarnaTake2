using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Max Script
public class LaserScript : MonoBehaviour //Skriptet är till för att få lasern att se lite bättre ut - Max
{
    [SerializeField] Gradient colorGradient;
    [SerializeField] float speed;

    MeshRenderer rend;

    float _gradientTime;
    float GradientTime
    {
        get { return _gradientTime; }
        set
        { 
            if(value <= 1)
            {
                _gradientTime = value;
            }
            else
            {
                //Eftersom en gradient går mellan 0 och 1 så ska den gå tillbaka till 0 när den gått över 1 för att loopa - Max
                _gradientTime = 0; 
            }
        }
    }

    private void Awake()
    {
        rend = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        //Basically sätter färgen beroende på time - Max

        GradientTime += Time.deltaTime * speed;

        rend.material.color = colorGradient.Evaluate(GradientTime);
    }
}
