using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject EnemyPrefab;       
    [SerializeField] private Transform Target;            
    [SerializeField] private float SpawnDistance = 10f;  
    [SerializeField] private float MinSpawnInterval = 1f;  
    [SerializeField] private float MaxSpawnInterval = 3f;  
    [SerializeField] private int MaxEnemies = 10;    

    private int m_CurrentEnemies = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(MinSpawnInterval, MaxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (MaxEnemies > 0 && m_CurrentEnemies >= MaxEnemies)
                continue;

            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyPrefab == null || Target == null)
            return;

        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = Target.position + new Vector3(randomDir2D.x, 0f, randomDir2D.y) * SpawnDistance + new Vector3(0f, 2f, 0f);

        GameObject enemy = Instantiate(EnemyPrefab, spawnPos, Quaternion.identity);
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        enemyComponent.AttackTarget = Target.gameObject;
        m_CurrentEnemies++;

        Health enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDead.AddListener(() => m_CurrentEnemies--);
        }
        else
        {
            Destroy(enemy, 60f); // Optional: destroy after 30s
            StartCoroutine(DecreaseCountAfterDelay(60f));
        }
    }

    private IEnumerator DecreaseCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_CurrentEnemies = Mathf.Max(0, m_CurrentEnemies - 1);
    }
}
