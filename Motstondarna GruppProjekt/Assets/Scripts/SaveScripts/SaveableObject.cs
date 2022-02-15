using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveableObject : MonoBehaviour//K-J
{
    [SerializeField] string id = string.Empty;//save ID (viktigt att inte l�mna den tom, H�gerklicka script ---> GenerateGuid)

    public string Id => id;

    [ContextMenu("GenerateGuid")]
    private void GenerateId() => id = Guid.NewGuid().ToString();//Guid.NewGuid() �r en funtion som genererar en random string. I praktiken �r det d� om�jligt genom detta s�tt att tv� olika sparobjekt skulle kunna ha samma id


    void Awake()
    {
        if(id == string.Empty)//varnar om man gl�mt att ge myntet en id
        {
            Debug.Log("Coin at: " + transform.position + " does not have an id!!!");
        }
    }


    public object CaptureState()
    {
        //G�r igenom alla script som har ISaveable och h�mtar spardata fr�n dem (i v�rat fall har vi bara 1)
        var state = new Dictionary<string, object>();//string �r scriptets namn, object �r datan som scriptet vill spara

        foreach (var saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.CaptureState();
        }

        return state;
    }

    public void RestoreState(object state)
    {
        //G�r igenom alla script som har ISaveable och laddar spardata fr�n dem
        var stateDictionary = (Dictionary<string, object>)state;//omvandlar state fr�n object till Dictionary

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
