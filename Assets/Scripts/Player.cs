using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Movement and Thruster
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _thrusterSpeed = 9f;
    private float _defaultSpeed;
    [SerializeField]
    private Image _thrustFuel;
    [SerializeField]
    private float _thrustFuelBar, _maxThrustFuelBar;
    [SerializeField]
    private float _thrusterRefillTime = 3f;
    //Base Laser and Ammo
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _homingMissilePrefab;
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _defaultFireRate;
    private float _canFire = -1f;
    [SerializeField]
    private int _currentAmmo = 15;
    [SerializeField]
    private AudioClip _outOfAmmo;
    //Health and Powerups
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private GameObject _damageR;
    [SerializeField]
    private GameObject _damageL;
    [SerializeField]
    private AudioClip _damageFeedback;
    [SerializeField]
    private AudioClip _laserShot;
    [SerializeField]
    private AudioClip _powerUpPickup;
    [SerializeField]
    private int _collectionsRemaining = 3;
    //Triple Shot
    private bool _isTripleShotActive = false;
    [SerializeField]
    private float _tripleShotActivationTime = 5.0f;
    //Homing Missiles
    [SerializeField]
    private bool _isHomingMissileActive = false;
    [SerializeField]
    private float _homingMissileActivationTime = 5.0f;
    [SerializeField]
    private float _homingMissileFireRate = 1f;
    //Speed Boost
    private bool _isSpeedBoostActive = false;
    [SerializeField]
    private float _speedBoostSpeed = 14f;
    [SerializeField]
    private float _speedBoostActivationTime = 5.0f;
    //Speed Debuff
    private bool _isSpeedDebuffActive = false;
    [SerializeField]
    private float _speedDebuffMultiplier = 2f;
    [SerializeField]
    private float _speedDebuffActivationTime = 5.0f;
    //Shield
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private int _shieldStrength;
    [SerializeField]
    private AudioClip _shieldHit;
    //score and UI
    private int _score;
    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _camera;
    

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _camera = GameObject.Find("Main Camera");

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null) 
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on Player is NULL.");
        }

        if (_camera == null)
        {
            Debug.LogError("The Camera is NULL");
        }

        _damageL.SetActive(false);
        _damageR.SetActive(false);

        _defaultSpeed = _speed;
        _defaultFireRate = _fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            if (_currentAmmo > 0 || _isTripleShotActive == true || _isHomingMissileActive == true)
            {
                FireLaser();
            }
            else
            {
                _audioSource.PlayOneShot(_outOfAmmo, .35f);
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && _collectionsRemaining > 0)
        {
            CollectPowerups();
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

        if (Input.GetKey(KeyCode.LeftShift) && _thrustFuel.fillAmount > 0 && _isSpeedBoostActive == false)
        {
            Thruster();
        }
        else if (Input.GetKey(KeyCode.LeftShift) && _thrustFuel.fillAmount == 0 && _isSpeedBoostActive == false)
        {
            _thrustFuel.fillAmount = 0;
            _speed = _defaultSpeed;
        }
        else
        {
            _speed = _defaultSpeed;
            _thrustFuel.fillAmount += 1f / _thrusterRefillTime * Time.deltaTime;
        }
    }

    // FireLaser handles instantiating the laser
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            return;
        }
        
        if (_isHomingMissileActive == true)
        {
            Instantiate(_homingMissilePrefab, transform.position, Quaternion.identity);
            return;
        }

        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.005f, 0), Quaternion.identity);
        _currentAmmo--;
        _uiManager.UpdateAmmo(_currentAmmo);
        

        _audioSource.PlayOneShot(_laserShot, .45f);
    }

    // Damage handles player health
    public void Damage()
    {
        if (_isShieldActive == true)
        {
            if (_shieldStrength > 0) _shieldStrength--;

            _audioSource.PlayOneShot(_shieldHit, .85f);

            _uiManager.ShieldLife(_shieldStrength);

            if (_shieldStrength == 0)
            {
                _isShieldActive = false;
                _shieldVisual.SetActive(false);
                return;
            }
            return;
        }

        _audioSource.PlayOneShot(_damageFeedback, .85f);

        _lives -= 1;

        _camera.gameObject.SendMessage("ShakeCamera");

        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _damageL.SetActive(true);
        }
        else if (_lives == 1)
        {
            _damageR.SetActive(true);
        }
        else if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            _uiManager.GameOverUI();
            Destroy(this.gameObject);
        }
    }

    void Thruster()
    {
        _speed = _thrusterSpeed;
        _thrustFuel.fillAmount -= 1f / _thrusterRefillTime * Time.deltaTime;

    }


    //method to add 10 to the score
    //communicate with the UI to update the score
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    // TripleShotActive handles enabling and disabling the TripleShot powerup
    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        _audioSource.PlayOneShot(_powerUpPickup);
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
        _audioSource.PlayOneShot(_powerUpPickup);
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
        _audioSource.PlayOneShot(_powerUpPickup);
        _shieldVisual.SetActive(true);
        _shieldStrength = 3;
        _uiManager.ShieldLife(_shieldStrength);
    }

    // AmmoRefill handles the ammo upgrade refilling ammo count
    public void AmmoRefill()
    {
        _currentAmmo = 15;
        _uiManager.UpdateAmmo(_currentAmmo);
    }

    // HealPowerup handles healing the player when the Heal is picked up
    public void HealPowerup()
    {
        _lives++;

        if (_lives == 3)
        {
            _damageL.SetActive(false);
        }
        else if (_lives == 2)
        {
            _damageR.SetActive(false);
        }
        else if (_lives == 4)
        {
            _lives = 3;
        }

        _uiManager.UpdateLives(_lives);
    }

    public void HomingMissiles()
    {
        _isHomingMissileActive = true;
        _audioSource.PlayOneShot(_powerUpPickup);
        _fireRate = _homingMissileFireRate;
        StartCoroutine(HomingMissilePowerDown());
    }

    IEnumerator HomingMissilePowerDown()
    {
        yield return new WaitForSeconds(_homingMissileActivationTime);
        _fireRate = _defaultFireRate;
        _isHomingMissileActive = false;
    }

    public void SpeedDebuff()
    {
        _isSpeedDebuffActive = true;
        _audioSource.PlayOneShot(_powerUpPickup);
        _defaultSpeed /= _speedDebuffMultiplier;
        _thrusterSpeed /= _speedDebuffMultiplier;
        StartCoroutine(SpeedDebuffPowerDown());
    }

    IEnumerator SpeedDebuffPowerDown()
    {
        yield return new WaitForSeconds(_speedDebuffActivationTime);
        _defaultSpeed *= _speedDebuffMultiplier;
        _thrusterSpeed *= _speedDebuffMultiplier;
        _isSpeedDebuffActive = false;
    }

    void CollectPowerups()
    {
        GameObject[] _powerupList = GameObject.FindGameObjectsWithTag("Powerup");

        if (_powerupList != null)
        {
            for (int i = 0; i < _powerupList.Length; i++)
            {
                PowerUp powerup = _powerupList[i].GetComponent<PowerUp>();
                powerup.CollectButton();
            }
        }

        _collectionsRemaining -= 1;
    }
}
