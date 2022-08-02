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
    private GameObject[] _restocking;
    private bool _stopSpawning = false;
    [SerializeField]
    private float _enemySpawnTime = 1.5f;
    [SerializeField]
    private float _minPowerupSpawnTime = 8f;
    [SerializeField]
    private float _maxPowerupSpawnTime = 13f;
    [SerializeField]
    private float _minHealthAmmoSpawnTime = 4f;
    [SerializeField]
    private float _maxHealthAmmoSpawnTime = 8f;
    [SerializeField]
    private float _enemyTimeTilSpawn = 1.3f;
    [SerializeField]
    private float _powerupTimeTilSpawn = 2.9f;
    [SerializeField]
    private float _ammoTimeTilSpawn = 2.1f;
    [SerializeField]
    private float _healthSpawnChance = .25f;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(HealthAndAmmoRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_enemyTimeTilSpawn);
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
        yield return new WaitForSeconds(_powerupTimeTilSpawn);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
            int randomPowerUp = Random.Range(0, 4);
            Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));
        }
    }

    IEnumerator HealthAndAmmoRoutine()
    {
        yield return new WaitForSeconds(_ammoTimeTilSpawn);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
            float ammoOrHealth = Random.Range(0f, 1f);
            if (ammoOrHealth <= _healthSpawnChance)
            {
                Instantiate(_restocking[1], posToSpawn, Quaternion.identity);
            }
            else
            {
                Instantiate(_restocking[0], posToSpawn, Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(_minHealthAmmoSpawnTime, _maxHealthAmmoSpawnTime));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}