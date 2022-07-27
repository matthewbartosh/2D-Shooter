using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private float _enemySpawnTime = 3f;
    private bool _stopSpawning = false;
    [SerializeField]
    private float _minPowerupSpawnTime = 3f;
    [SerializeField]
    private float _maxPowerupSpawnTime = 7f;
    [SerializeField]
    private float _timeTilSpawn = 1.3f;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_timeTilSpawn);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnTime);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_timeTilSpawn);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
            int randomPowerUp = Random.Range(0, 4);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}