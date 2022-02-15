using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DeleteTheFile : MonoBehaviour
{
    private void Awake()
    {
        File.Delete(Application.persistentDataPath + "/bowlingSave.txt");
        print("da file is gone");
    }
}
