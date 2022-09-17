using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject[] _restocking;
    private bool _stopSpawning = false;
    private bool _powerupSpawning = false;
    [SerializeField]
    private float _enemySpawnTime = 3f;
    [SerializeField]
    private float _minPowerupSpawnTime = 6.0f;
    [SerializeField]
    private float _maxPowerupSpawnTime = 10.0f;
    [SerializeField]
    private float _minHealthAmmoSpawnTime = 4.0f;
    [SerializeField]
    private float _maxHealthAmmoSpawnTime = 8.0f;
    [SerializeField]
    private float _enemyTimeTilSpawn = 1.3f;
    [SerializeField]
    private float _powerupTimeTilSpawn = 2.9f;
    [SerializeField]
    private float _ammoTimeTilSpawn = 2.1f;
    [SerializeField]
    private float _healthSpawnChance = .15f;
    [SerializeField]
    private GameObject[] _enemies;
    [SerializeField]
    private int _waveNumber = 1;
    [SerializeField]
    private float _waveDuration = 10;
    [SerializeField]
    private bool _bossActive = false;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL.");
        }
    }

    private void Update()
    {
        if (_stopSpawning == true && _enemyContainer.transform.childCount == 0)
        {
            StartSpawning();
            IncreaseWave();
        }

        if (_bossActive == true && _stopSpawning == true)
        {
            StartSpawning();
        }

        if (_bossActive == true && _enemyContainer.transform.childCount == 0 && _stopSpawning == false)
        {
            _bossActive = false;
            _stopSpawning = true;
        }

    }

    public void StartSpawning()
    {
        if (_bossActive == false)
        {
            _stopSpawning = false;
            StartCoroutine(SpawnEnemyRoutine());
            StartCoroutine(SpawnPowerupRoutine());
            StartCoroutine(WaveActive(_waveDuration));
            if (_powerupSpawning == false)
            {
                StartCoroutine(HealthAndAmmoRoutine());
            }
        }
        else if (_bossActive == true)
        {
            _stopSpawning = false;
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_enemyTimeTilSpawn);
        while (_stopSpawning == false)
        {
            if (_waveNumber <= 2)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomEnemy = Random.Range(0, 1);
                GameObject newEnemy = Instantiate(_enemies[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnTime);
            }
            else if (_waveNumber > 2 && _waveNumber <= 4)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomEnemy = Random.Range(0, 2);
                GameObject newEnemy = Instantiate(_enemies[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnTime);
            }
            else if (_waveNumber > 4 && _waveNumber <= 7)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomEnemy = Random.Range(0, 3);
                GameObject newEnemy = Instantiate(_enemies[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnTime);
            }
            else if (_waveNumber > 7 && _waveNumber <= 8)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomEnemy = Random.Range(0, 4);
                GameObject newEnemy = Instantiate(_enemies[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnTime);
            }
            else if (_waveNumber == 9)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomEnemy = Random.Range(0, 4);
                GameObject newEnemy = Instantiate(_enemies[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnTime);
            }
            else if (_waveNumber == 10)
            {
                _uiManager.ActivateBossSlider();
                StartCoroutine(BossSpawn());
                _bossActive = true;
                yield return new WaitForSeconds(120);

            }
            else if (_waveNumber > 10 && _waveNumber < 20)
            {
                _bossActive = false;
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomEnemy = Random.Range(0, _enemies.Length);
                GameObject newEnemy = Instantiate(_enemies[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnTime);
            }
            else if (_waveNumber >= 20)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                GameObject newEnemy = Instantiate(_enemies[5], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(.1f);
            }

            else
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomEnemy = Random.Range(0, _enemies.Length);
                GameObject newEnemy = Instantiate(_enemies[randomEnemy], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                yield return new WaitForSeconds(_enemySpawnTime);
            }

        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_powerupTimeTilSpawn);
        while (_stopSpawning == false)
        {
            if (_bossActive == false)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomPowerUp = Random.Range(0, _powerups.Length);
                Debug.Log("Powerup is " + randomPowerUp);
                Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(_minPowerupSpawnTime, _maxPowerupSpawnTime));
            }
            else if (_bossActive == true)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                int randomPowerUp = Random.Range(0, _powerups.Length);
                if (randomPowerUp == 0)
                {
                    float powerUpReroll = Random.Range(0f, 1f);
                    Debug.Log("Powerup: Triple Shot and reroll is: " + powerUpReroll);
                    if (powerUpReroll <= .5f)
                    {
                        randomPowerUp = 4;
                    }
                    else
                    {
                        randomPowerUp = 0;
                    }
                }
                else if (randomPowerUp == 3)
                {
                    float powerUpReroll = Random.Range(0f, 1f);
                    Debug.Log("Powerup: Missiles and reroll is: " + powerUpReroll);
                    if (powerUpReroll <= .75f)
                    {
                        randomPowerUp = 4;
                    }
                    else
                    {
                        randomPowerUp = 0;
                    }
                }
                Instantiate(_powerups[randomPowerUp], posToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(6, 12));
            }
        }
    }

    IEnumerator HealthAndAmmoRoutine()
    {
        _powerupSpawning = true;
        yield return new WaitForSeconds(_ammoTimeTilSpawn);
        while (_powerupSpawning == true)
        {
            if (_bossActive == false)
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
            else if (_bossActive == true)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
                float ammoOrHealth = Random.Range(0f, 1f);
                if (ammoOrHealth <= .25f)
                {
                    Instantiate(_restocking[1], posToSpawn, Quaternion.identity);
                }
                else
                {
                    Instantiate(_restocking[0], posToSpawn, Quaternion.identity);
                }
                yield return new WaitForSeconds(Random.Range(4, 8));
            }
        }
    }

    IEnumerator WaveActive(float WaveDuration)
    {
        yield return new WaitForSeconds(WaveDuration);
        _stopSpawning = true;

    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        _bossActive = false;
        _powerupSpawning = false;
    }



    void IncreaseWave()
    {
        _waveNumber++;
        _uiManager.UpdateWave(_waveNumber);
        if (_enemySpawnTime > .3)
        {
            _enemySpawnTime -= 0.2f;
        }
        
        if (_waveNumber == 7)
        {
            _enemySpawnTime = 1.2f;
        }
    }

    IEnumerator BossSpawn()
    {
        GameObject newEnemy = Instantiate(_bossPrefab, new Vector3(0, -17f, 10f), Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
        yield return new WaitForSeconds(120);
    }
}