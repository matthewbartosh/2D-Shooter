using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 25f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //rotate object on Z axis.
        transform.Rotate(new Vector3(0, 0, 1) * _rotateSpeed * Time.deltaTime);
    }

    //check for laser collision trigger
    //instantiate explosion on me (asteroid)
    //destroy explosion after 3 seconds

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(this.GetComponent<CircleCollider2D>());
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion()
    {
        var AsteroidExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
        Destroy(AsteroidExplosion, 1.4f);
        _spawnManager.StartSpawning();
        Destroy(this.gameObject);
    }
}

