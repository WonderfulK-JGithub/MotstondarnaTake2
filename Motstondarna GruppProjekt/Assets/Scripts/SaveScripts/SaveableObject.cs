using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveableObject : MonoBehaviour//K-J
{
    [SerializeField] string id = string.Empty;//save ID (viktigt att inte lämna den tom, Högerklicka script ---> GenerateGuid)

    public string Id => id;

    [ContextMenu("GenerateGuid")]
    private void GenerateId() => id = Guid.NewGuid().ToString();//Guid.NewGuid() är en funtion som genererar en random string. I praktiken är det då omöjligt genom detta sätt att två olika sparobjekt skulle kunna ha samma id


    void Awake()
    {
        if(id == string.Empty)//varnar om man glömt att ge myntet en id
        {
            Debug.Log("Coin at: " + transform.position + " does not have an id!!!");
        }
    }


    public object CaptureState()
    {
        //Går igenom alla script som har ISaveable och hämtar spardata från dem (i vårat fall har vi bara 1)
        var state = new Dictionary<string, object>();//string är scriptets namn, object är datan som scriptet vill spara

        foreach (var saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.CaptureState();
        }

        return state;
    }

    public void RestoreState(object state)
    {
        //Går igenom alla script som har ISaveable och laddar spardata från dem
        var stateDictionary = (Dictionary<string, object>)state;//omvandlar state från object till Dictionary

        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();
            if (stateDictionary.TryGetValue(typeName, out object value))//kollar om scriptet som kan spara data har sparat data
            {
                saveable.RestoreState(value);
            }
        }
    }

}
