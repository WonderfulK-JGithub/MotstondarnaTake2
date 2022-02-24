using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour//av K-J
{
    public static BossEnemy current;

    [SerializeField] Vector3 bossKnockback;
    [SerializeField] GameObject shockWavePrefab;
    [SerializeField] GameObject damageTrigger;

    [SerializeField] Vector3 leftShockWavePos; //btw det är vänster för bossens POV, inte spelarens
    [SerializeField] Vector3 rightShockWavePos;

    [SerializeField] Vector3 leftDamageTriggerPos;
    [SerializeField] Vector3 rightDamageTriggerPos;

    [SerializeField] Renderer rend;

    public float speedIncrease;

    public Animator anim;

    public Material deadMaterial;

    private void Awake()
    {
        current = this;
        anim = GetComponent<Animator>();
    }

    public void Die()//Spelar death animation och ändrar textur(material)
    {
        anim.Play("Boss_Die");
        rend.material = deadMaterial;
    }
    public void HitPlayer()//kallas av ett CollisionTriggerEvent objekt
    {
        BallHealth.current.TakeDamage(Vector3.zero, 1);
    }


    void CreateShockWave(int dir)//kallas av ett animation event i bossens attack animation
    {
        SoundManagerScript.PlaySound("Smash");

        if (dir == -1)//skapa en shockWave på vänstra eller högra sidan
        {
            Instantiate(shockWavePrefab, leftShockWavePos, Quaternion.Euler(0f, 90f, 0f));
            Destroy(Instantiate(damageTrigger, leftDamageTriggerPos, Quaternion.identity), 0.2f);
        }
        else
        {
            Instantiate(shockWavePrefab, rightShockWavePos, Quaternion.Euler(0f, 90f, 0f));
            Destroy(Instantiate(damageTrigger, rightDamageTriggerPos, Quaternion.identity),0.2f);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (BallHealth.current.aboveKillSpeed)
            {
                BallHealth.current.BossDamaged(bossKnockback);//spelaren knockas tillbaka till slottet
                BossManager.current.BossDamaged(); //bossen tar skada
            }
            else
            {
                BallHealth.current.TakeDamage(Vector3.zero, 1);
            }
        }
    }
}
