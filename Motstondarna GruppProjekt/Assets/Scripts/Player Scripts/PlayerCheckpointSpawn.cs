using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCheckpointSpawn : MonoBehaviour
{
    //public float lowestLevel = -4; // hur lågt ner spelaren kan vara innan spelet startar om - Anton
    // Start is called before the first frame update
    void Start() // kollar vid start av spelet - Anton
    {
        if (PlayerPrefs.GetInt("progress", 0) != 0) // kollar om inte progress == 0 - Anton
        {
            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>(); // hittar alla checkpoints - Anton
            foreach (var checkpoint in checkpoints) // kollar vilken checkpoint som har samma id som sin progress - Anton
            {
                if (checkpoint.checkpointID == PlayerPrefs.GetInt("progress"))
                {
                    transform.position = checkpoint.transform.position; // flyttar bollen till checkpointens position - Anton
                }
            }
        }
        
    }
}
