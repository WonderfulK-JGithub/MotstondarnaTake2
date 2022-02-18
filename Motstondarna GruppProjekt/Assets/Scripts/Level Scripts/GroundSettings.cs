using System.Collections.Generic;
using UnityEngine;

// detta skript anv�nds inte l�ngre d� vi anv�nder probuild f�r att bygga banorna, detta var aktuellt innan d� banorna var byggda av block och man inte ville rendera alla faces p� samma g�ng.

public class GroundSettings : MonoBehaviour
{
    List<Vector3> groundCoords = new List<Vector3>(); // h�mtar koordinaterna f�r varje markobjekt
    Transform[] faces; // alla sidor
    void Start()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z)); // avrundar positionen (on�dig kod f�r alla positioner �r redan avrundade) - Anton
        GroundSettings[] grounds = FindObjectsOfType<GroundSettings>(); // hittar alla markobjekt i scenen - Anton
        foreach (var ground in grounds)
        {
            groundCoords.Add(ground.transform.position); // l�gger till koordinaterna i groundCoords - Anton
        }
        faces = GetComponentsInChildren<Transform>(); // h�mtar alla sidors Transform - Anton


        if (groundCoords.Contains(transform.position + new Vector3(2, 0, 0))) { Destroy(faces[1].gameObject); } // dessa rader sk�ter bortplockning av on�diga sidor, dvs sidor som tittar in i en annan sida. - Anton
        if (groundCoords.Contains(transform.position + new Vector3(-2, 0, 0))) { Destroy(faces[2].gameObject); }
        if (groundCoords.Contains(transform.position + new Vector3(0, 0, 2))) { Destroy(faces[3].gameObject); }
        if (groundCoords.Contains(transform.position + new Vector3(0, 0, -2))) { Destroy(faces[4].gameObject); }
    }
}
