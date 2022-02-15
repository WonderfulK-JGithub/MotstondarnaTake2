using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager current;

    [SerializeField] EnemyWave[] enemyWaves;

    List<BaseEnemy> currentEnemies = new List<BaseEnemy>();

    BossState state;

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

                    BossEnemy.current.smash = true;
                }
                break;
        }
    }


    [ContextMenu("SpawnEnemy")]
    void SpawnWave()
    {
        int currentWave = 0;

        for (int i = 0; i < enemyWaves[currentWave].enemiesToSpawn.Length; i++)
        {
            currentEnemies.Add(Instantiate(enemyWaves[currentWave].enemiesToSpawn[i], enemyWaves[currentWave].enemyPositions[i],Quaternion.identity).GetComponent<BaseEnemy>());
        }
    }

    public void BossDamaged()
    {
        state = BossState.EnemyWave;
        SpawnWave();
    }

    enum BossState
    {
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