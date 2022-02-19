using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager current;

    [SerializeField] EnemyWave[] enemyWaves;
    [SerializeField] GameObject poofPrefab;


    [SerializeField] GameObject laserEnemyPrefab;
    [SerializeField] Vector3[] laserEnemyPositions;

    List<BaseEnemy> currentEnemies = new List<BaseEnemy>();

    [SerializeField] Animator frontDoor;
    [SerializeField] Animator leftDoor;
    [SerializeField] Animator rightDoor;

    [SerializeField] GameObject theEnd;

    BossState state;

    int currentWave;

    List<GameObject> laserEnemiesOnPillar = new List<GameObject>();

    [SerializeField] AudioClip clip;

    private void Awake()
    {
        current = this;
    }

    void Update()
    {
        switch(state)
        {
            case BossState.EnemyWave:

                for (int i = 0; i < currentEnemies.Count; i++)
                {
                    if(currentEnemies[i].hasDied)
                    {
                        currentEnemies.RemoveAt(i);
                        i--;
                    }
                }

                if(currentEnemies.Count == 0)
                {
                    state = BossState.BigGuy;

                    print("Enemies Cleared");

                    BossEnemy.current.anim.SetTrigger("StartAttack");

                    leftDoor.Play("Door_Open");
                    rightDoor.Play("Door_Open");

                    foreach (var item in laserEnemiesOnPillar)
                    {
                        if (item != null) Destroy(item);
                    }
                    laserEnemiesOnPillar.Clear();

                    if(currentWave > 0)
                    {
                        laserEnemiesOnPillar.Add(Instantiate(laserEnemyPrefab, laserEnemyPositions[0], Quaternion.Euler(0f,180f,0f)));
                    }
                    if(currentWave > 1)
                    {
                        laserEnemiesOnPillar.Add(Instantiate(laserEnemyPrefab, laserEnemyPositions[1], Quaternion.Euler(0f, 0f, 0f)));
                        laserEnemiesOnPillar.Add(Instantiate(laserEnemyPrefab, laserEnemyPositions[2], Quaternion.Euler(0f, 0f, 0f)));
                    }
                }
                break;
        }
    }

    public void StartBattle()
    {
        frontDoor.Play("Door_Close");
        SpawnWave();

        Pause.source.Stop();
        Pause.source.clip = clip;
        Pause.source.Play();
    }


    [ContextMenu("SpawnEnemy")]
    void SpawnWave()
    {
        state = BossState.EnemyWave;

        for (int i = 0; i < enemyWaves[currentWave].enemiesToSpawn.Length; i++)
        {
            currentEnemies.Add(Instantiate(enemyWaves[currentWave].enemiesToSpawn[i], enemyWaves[currentWave].enemyPositions[i],Quaternion.identity).GetComponent<BaseEnemy>());
            Destroy(Instantiate(poofPrefab, enemyWaves[currentWave].enemyPositions[i], Quaternion.identity), 3f);
        }
    }

    public void BossDamaged()
    {
        

        currentWave++;

        if(currentWave == 3)
        {
            SoundManagerScript.PlaySound("BossDeath");
            state = BossState.End;
            BossEnemy.current.Die();
            theEnd.SetActive(true);
            Pause.source.Stop();
        }
        else
        {
            SoundManagerScript.PlaySound("BossDamaged");
            SpawnWave();
            BossEnemy.current.anim.SetTrigger("GetHurt");
            BossEnemy.current.anim.speed *= BossEnemy.current.speedIncrease;
        }

        

        leftDoor.Play("Door_Close");
        rightDoor.Play("Door_Close");
    }

    

    enum BossState
    {
        Off,
        EnemyWave,
        BigGuy,
        End,
    }
}

[System.Serializable]
public class EnemyWave
{
    public GameObject[] enemiesToSpawn;
    public Vector3[] enemyPositions;
}