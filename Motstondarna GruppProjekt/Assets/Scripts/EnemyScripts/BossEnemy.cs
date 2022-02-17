using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public static BossEnemy current;


    [SerializeField] Vector3 bossKnockback;
    [SerializeField] GameObject shockWavePrefab;

    [SerializeField] Vector3 leftShockWavePos; //btw det är vänster för bossens POV, inte spelarens
    [SerializeField] Vector3 rightShockWavePos;

    public Animator anim;

    private void Awake()
    {
        current = this;
        anim = GetComponent<Animator>();
    }

    

    void CreateShockWave(int dir)
    {
        bool _left = dir == -1;
        if (_left) Instantiate(shockWavePrefab, leftShockWavePos,Quaternion.Euler(0f,90f,0f));
        else Instantiate(shockWavePrefab, rightShockWavePos, Quaternion.Euler(0f, 90f, 0f));

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            BallHealth ballHD = collision.gameObject.GetComponent<BallHealth>();
            if (ballHD.aboveKillSpeed)
            {
                BallHealth.current.BossDamaged(bossKnockback);
                BossManager.current.BossDamaged();
            }
        }
    }
}
