using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID; // hur l�ngt man kommit i banan - Anton
    [SerializeField] ParticleSystem ps;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // n�r spelaren tr�ffar checkpointen - Anton
        {
            if (PlayerPrefs.GetInt("progress", 0) < checkpointID) // kollar om man nuddare en senare checkpoint - Anton
            {
                PlayerPrefs.SetInt("progress", checkpointID); // �ndrar ens progressv�rde - Anton
                ps.Play();
                SaveSystem.current.Save();
            }
        } // checkpoints reagerar inte om man har h�gre progress-v�rde �n checkpointID:t. Detta inneb�r att man inte kan g� tillbaka till gamla checkpoints. - Anton
    }
}
