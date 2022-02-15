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

    public bool smash;

    bool left;
    float timer;

    private void Awake()
    {
        current = this;
    }

    void Update()
    {
        if(smash)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                left = !left;
                timer = 4f;
                CreateShockWave(left);
            }
        }
    }

    void CreateShockWave(bool _left)
    {
        if (_left) Instantiate(shockWavePrefab, leftShockWavePos,Quaternion.identity);
        else Instantiate(shockWavePrefab, rightShockWavePos, Quaternion.identity);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            BallHealth ballHD = collision.gameObject.GetComponent<BallHealth>();

            if(ballHD.aboveKillSpeed)
            {
                BallHealth.current.BossDamaged(bossKnockback);
                BossManager.current.BossDamaged();
                smash = false;
            }
        }
    }
}
