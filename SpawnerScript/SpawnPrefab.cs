using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnPrefab : MonoBehaviour
{
    #region Variable Field

    // Amount spawning
    public int spawnAmount;
    private int prefabSpawned = 0;
    private int point = 0;
    public bool bSetAmount = false;
    public float timeBetween;
    public float wait = 2f;

    // Timer Spawning
    public int spawnPointsSize;

    // Timer
    public bool bTimer = false;
    public float spawnTimer;
    public float spawnCooldown;
    public float initialSpawnTime;

    // Preload prefab
    public bool preload = false;
    public int quantity;

    // Position & Prefab
    public GameObject prefabToSpawn;
    public List<Transform> spawnPos;

    // What are you doing?
    private const bool no = false;

    #endregion Variable Field

    #region Unity Functions

    private void Start()
    {
        if (preload)
        {
            // Preload zero enemies?
            if (quantity < 1)
            {
                Assert.IsFalse(quantity < 1, "？");
            }
            else
            {
                ObjectPool.Preload(prefabToSpawn, quantity);
            }
        }
    }

    private void Update()
    {
        if (bSetAmount)
        {
            AmountSpawn();
        }

        if (bTimer)
        {
            TimerSpawn();
        }

        if (bTimer && bSetAmount)
        {
            Assert.IsTrue(no, "Why do you have both spawn 'Timer & Set', on at the same time? This is not intended to work like this.");
        }
    }

    #endregion Unity Functions

    #region Functions

    private void Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        ObjectPool.Spawn(prefab, pos, rot);
    }

    private void TimerSpawn()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= initialSpawnTime)
        {
            // We have waited initialSpawnTime, set it to zero.
            initialSpawnTime = 0;

            if (spawnTimer >= spawnCooldown)
            {
                for (int i = 0; i < spawnPos.Count; i++)
                {
                    Spawn(prefabToSpawn, spawnPos[i].position, Quaternion.identity);
                }
                spawnTimer = 0;
            }
        }
    }

    private void AmountSpawn()
    {
        timeBetween += Time.deltaTime;

        if (prefabSpawned < spawnAmount)
        {
            if (timeBetween >= wait)
            {
                if (point >= spawnPos.Count)
                {
                    // If we have less spawn points than enemies to spawn
                    // loop back to first spawn point.
                    point = 0;
                }

                Spawn(prefabToSpawn, spawnPos[point].position, Quaternion.identity);
                timeBetween = 0; prefabSpawned++; point++;
            }
        }
    }

    public void DespawnGameObject(GameObject gameObject)
    {
        ObjectPool.Despawn(gameObject);
    }

    #endregion Functions
}
