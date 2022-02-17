using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID; // hur långt man kommit i banan - Anton
    [SerializeField] ParticleSystem ps;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // när spelaren träffar checkpointen - Anton
        {
            if (PlayerPrefs.GetInt("progress", 0) < checkpointID) // kollar om man nuddare en senare checkpoint - Anton
            {
                PlayerPrefs.SetInt("progress", checkpointID); // ändrar ens progressvärde - Anton
                ps.Play();
                SaveSystem.current.Save();
                SoundManagerScript.PlaySound("CheckPoint");
            }
        } // checkpoints reagerar inte om man har högre progress-värde än checkpointID:t. Detta innebär att man inte kan gå tillbaka till gamla checkpoints. - Anton
    }
}
