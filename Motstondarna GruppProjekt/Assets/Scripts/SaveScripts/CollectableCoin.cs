using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCoin : MonoBehaviour, ISaveable//K-J
{
    [SerializeField] Color normalColor;
    [SerializeField] Color alreadyCollectedColor;

    public bool isCollected;//om man har tagit myntet
    public bool isStored;//om man tidigare har tagit myntet och laddar om banan

    MeshRenderer rend;
    MeshCollider col;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        col = GetComponent<MeshCollider>();
    }

    //spara data
    public object CaptureState()
    {
        return new SaveData
        {
            isCollected = isCollected,
            isStored = isStored,
        };
        
    }
    //ladda data
    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        isCollected = saveData.isCollected;
        isStored = saveData.isStored;

        //ger myntet en färg baserat på om den redan har tagits eller inte;
        if(isCollected)
        {
            rend.enabled = false;
            col.enabled = false;
        }
        else if(isStored)
        {
            rend.material.color = alreadyCollectedColor;
        }
        else
        {
            rend.material.color = normalColor;
        }
    }

    [System.Serializable] struct SaveData //Spardata
    {
        public bool isCollected;
        public bool isStored;
    }


    public void CollectCoin()//när man nuddar myntet
    {
        rend.enabled = false;
        col.enabled = false;

        isCollected = true;
    }
}
