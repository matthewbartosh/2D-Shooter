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
    private float _waitTime = 5f;
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnRoutine()
    {
        while (_stopSpawning == false)
        {
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-9.44f, 9.46f), 7.9f, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_waitTime);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

}
