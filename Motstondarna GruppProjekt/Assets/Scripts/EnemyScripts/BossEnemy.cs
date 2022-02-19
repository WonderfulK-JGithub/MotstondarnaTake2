using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
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

    public void Die()
    {
        anim.Play("Boss_Die");
        rend.material = deadMaterial;
    }
    public void HitPlayer()
    {
        BallHealth.current.TakeDamage(Vector3.zero, 1);
    }


    void CreateShockWave(int dir)
    {
        SoundManagerScript.PlaySound("Smash");

        if (dir == -1)
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
            

            BallHealth ballHD = other.gameObject.GetComponent<BallHealth>();
            if (ballHD.aboveKillSpeed)
            {
                BallHealth.current.BossDamaged(bossKnockback);
                BossManager.current.BossDamaged();
            }
            else
            {
                BallHealth.current.TakeDamage(Vector3.zero, 1);
            }
        }
    }
}
