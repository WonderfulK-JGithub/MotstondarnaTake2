using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour//K-J
{
    public static SaveSystem current;

    string dataPath = null;

    private void Awake()
    {
        dataPath = Application.persistentDataPath + "/bowlingSave.txt";
        current = this;
    }

    private void Start()
    {
        Load();//laddar myntens data vid start av banan
    }

    [ContextMenu("Save")]
    public void Save()
    {
        var state = LoadFile();
        CaptureState(state);
        SaveFile(state);
    }

    [ContextMenu("Load")]
    void Load()
    {
        var state = LoadFile();
        RestoreState(state);
    }

    [ContextMenu("Delete")]
    void Delete()
    {
        File.Delete(dataPath);
        print("File deleted");
    }//tar bort sparfilen

    //sparar och laddar binärfil på samma gammla sätt som vanligt
    void SaveFile(object state)
    {
        FileStream stream = new FileStream(dataPath, FileMode.Create);


        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, state);

        stream.Close();

    }
    Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(dataPath))
        {
            print("no file");
            return new Dictionary<string, object>();
            
        }
        else
        {
            FileStream stream = new FileStream(dataPath, FileMode.Open);

            var formatter = new BinaryFormatter();


            Dictionary<string, object> data = formatter.Deserialize(stream) as Dictionary<string, object>;

            stream.Close();

            return data;
        }
    }

    //datan tas hand om med hjälp av en Dictionary som består av string och object
    //stringen fungerar som ett id
    //object har själva spardatan. Eftersom det är variabeln object kan man göra olika classes/structs men ändå kunna spara allt på samma plats
    void CaptureState(Dictionary<string, object> state)
    {
        //fångar alla objekt i scenen med scriptet SaveableObject på sig och ber dem skicka in spardata
        foreach (var saveable in FindObjectsOfType<SaveableObject>())
        {
            state[saveable.Id] = saveable.CaptureState();
        }
    }
    void RestoreState(Dictionary<string, object> state)
    {
        //går igenom alla objekt i scenen med scriptet SaveableObject på sig, kollar om de har spardata och om de har de så laddas den datan
        foreach (var saveable in FindObjectsOfType<SaveableObject>())
        {
            if (state.TryGetValue(saveable.Id, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }

    //Om spelaren bestämmer sig för att alt f4 ska myntent sparas som !isCollected
    private void OnApplicationQuit()
    {
        foreach (var item in FindObjectsOfType<CollectableCoin>())
        {
            item.isCollected = false;
        }

        Save();

        PlayerPrefs.SetInt("progress", 0);
    }
}


public interface ISaveable //alla objekt som ska spara behöver kunna ta emot och skicka in spardata/"state"
{
    object CaptureState();
    void RestoreState(object state);
}

