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
    //Triple Shot
    private bool _isTripleShotActive = false;
    [SerializeField]
    private float _tripleShotActivationTime = 5.0f;
    //Speed Boost
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private float _speedBoostSpeed = 10f;
    [SerializeField]
    private float _speedBoostActivationTime = 5.0f;
    //Shield
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisual;
    //score and UI
    [SerializeField]
    private int _score;
    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
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
         if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisual.SetActive(false);
            return;
        }

        _lives -= 1;

        _uiManager.UpdateLives(_lives);
        
        if(_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _uiManager.GameOverUI();
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

    // SpeedBoostActive handles enabling and disabling the SpeedBoost powerup
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

    // ShieldActive handles enabling and disabling the Shield powerup
    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisual.SetActive(true);
    }

    //method to add 10 to the score
    //communicate with the UI to update the score
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}   
