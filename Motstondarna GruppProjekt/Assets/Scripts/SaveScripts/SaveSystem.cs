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

    //sparar och laddar bin�rfil p� samma gammla s�tt som vanligt
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

    //datan tas hand om med hj�lp av en Dictionary som best�r av string och object
    //stringen fungerar som ett id
    //object har sj�lva spardatan. Eftersom det �r variabeln object kan man g�ra olika classes/structs men �nd� kunna spara allt p� samma plats
    void CaptureState(Dictionary<string, object> state)
    {
        //f�ngar alla objekt i scenen med scriptet SaveableObject p� sig och ber dem skicka in spardata
        foreach (var saveable in FindObjectsOfType<SaveableObject>())
        {
            state[saveable.Id] = saveable.CaptureState();
        }
    }
    void RestoreState(Dictionary<string, object> state)
    {
        //g�r igenom alla objekt i scenen med scriptet SaveableObject p� sig, kollar om de har spardata och om de har de s� laddas den datan
        foreach (var saveable in FindObjectsOfType<SaveableObject>())
        {
            if (state.TryGetValue(saveable.Id, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }

    //Om spelaren best�mmer sig f�r att alt f4 ska myntent sparas som !isCollected
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


public interface ISaveable //alla objekt som ska spara beh�ver kunna ta emot och skicka in spardata/"state"
{
    object CaptureState();
    void RestoreState(object state);
}

