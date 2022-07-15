using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    [SerializeField]
    private float _tripleShotActivationTime = 5.0f;
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private float _speedBoostSpeed = 10f;
    [SerializeField]
    private float _speedBoostActivationTime = 5.0f;

    
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

     }

    // CalculateMovement handles Player movement
    void CalculateMovement()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(HorizontalInput, VerticalInput, 0);

        if (_isSpeedBoostActive == true)
        {
            transform.Translate(direction * _speedBoostSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -6.6f, 0), 0);

        if (transform.position.x > 16.15f)
        {
            transform.position = new Vector3(-16.15f, transform.position.y, 0);
        }
        else if (transform.position.x < -16.15f)
        {
            transform.position = new Vector3(16.15f, transform.position.y, 0);
        }

       
    }

    // FireLaser handles instantiating the laser
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if ( _isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.005f, 0), Quaternion.identity);
        }
    }

    // Damage handles player health
    public void Damage()
    {
        _lives -= 1;

        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }

    }

    // TripleShotActive handles enabling and disabling the TripleShot powerup
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(_tripleShotActivationTime);
        _isTripleShotActive = false;
    }

    // SpeedBoostActivee handles enabling and disabling the SpeedBoost powerup
    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(_speedBoostActivationTime);
        _isSpeedBoostActive = false;
    }    
}   
