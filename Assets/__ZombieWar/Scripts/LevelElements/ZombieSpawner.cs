using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] float spawnDistanceFromPlayer = 15;
    [SerializeField] ZombieCharacter zombiePrefab;
    [SerializeField] Vector3VariableSO playerPositionSO;
    ObjectPooling<ZombieCharacter> zombiePool;
    bool isGameOver = false;

    ObjectPooling<ZombieCharacter> ZombiePool
    {
        get
        {
            if (zombiePool == null) InitZombiePooling();
            return zombiePool;
        }
    }

    private void Start()
    {
        StartCoroutine(CR_SpawnZombieRepeatedly());
    }

    void InitZombiePooling()
    {
        if (zombiePrefab == null) return;
        zombiePool = new ObjectPooling<ZombieCharacter>(
                        instantiateMethod: InstantiateMethod,
                        destroyMethod: DestroyMethod,
                        resetMethod: ResetMethod,
                        preAddToPool: PreAddToPool,
                        preLeavePool: PreLeavePool
                    )
        {
            PregenerateOffset = 5
        };

        ZombieCharacter InstantiateMethod()
        {
            ZombieCharacter instance = Instantiate(zombiePrefab, transform);
            instance.OnDeath += () => AddToPool(instance);
            return instance;
        }
        void DestroyMethod(ZombieCharacter zombie)
        {
            Destroy(zombie.gameObject);
        }
        void ResetMethod(ZombieCharacter zombie)
        {
            zombie.ResetToOrgState();
        }
        void PreAddToPool(ZombieCharacter zombie)
        {
            zombie.gameObject.SetActive(false);
        }
        void PreLeavePool(ZombieCharacter zombie)
        {

        }
    }

    void AddToPool(ZombieCharacter zombie)
    {
        ZombiePool.Add(zombie);
    }

    IEnumerator CR_SpawnZombieRepeatedly()
    {
        float startTime = Time.time;
        while (!isGameOver)
        {
            ZombieCharacter zombie = ZombiePool.Get();
            Vector3 spawnPos = playerPositionSO.Value;
            int findTimes = 0;
            while (findTimes <= 10) //Find 10 times only
            {
                spawnPos = playerPositionSO.Value;
                var randomUnitCircle = Random.insideUnitCircle;
                spawnPos.x += randomUnitCircle.x * spawnDistanceFromPlayer;
                spawnPos.z += randomUnitCircle.y * spawnDistanceFromPlayer;
                Ray ray = new(spawnPos + Vector3.up * 2, -Vector3.up);
                if (Physics.Raycast(ray))
                {
                    break;
                }
                findTimes++;
            }
            zombie.transform.position = spawnPos;
            zombie.gameObject.SetActive(true);
            int elapsedHalfMinute = (int)(Time.time - startTime) / 30;
            yield return new WaitForSeconds(Mathf.Max(3.5f - elapsedHalfMinute * 0.5f, 0.5f));
        }
    }
}
