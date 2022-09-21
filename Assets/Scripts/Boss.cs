using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    private Player _player;
    private BoxCollider2D _bc;
    private EdgeCollider2D _ec;
    private Rigidbody2D _rb;
    private Animator _anim;
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _wingLasers;
    [SerializeField]
    private AudioClip _laserShot;
    [SerializeField]
    private GameObject _laserBeam;
    [SerializeField]
    private GameObject _primaryLeft;
    [SerializeField]
    private GameObject _primaryRight;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private GameObject _silentExplosion;
    [SerializeField]
    private AudioClip _beamWeapon;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSound;
    [SerializeField]
    private float _speed = 4;
    private float _beamCanFire;
    private float _wingsCanFire;
    private float _primaryLeftCanFire;
    private float _primaryRightCanFire;
    [SerializeField]
    private Slider _healthBar;
    private int _movementStyle;
    [SerializeField]
    private float _currentHPBar, _maxHPBar;
    [SerializeField]
    private bool _phaseOne = false;
    [SerializeField]
    private GameObject _p1Sprite;
    [SerializeField]
    private bool _phaseTwo = false;
    [SerializeField]
    private GameObject _p2Sprite;
    [SerializeField]
    private bool _phaseThree = false;
    [SerializeField]
    private GameObject _p3Sprite;
    [SerializeField]
    private bool _phaseFinal = false;
    private bool _phaseTransition = false;
    private bool _finalDeath = false;
    [SerializeField]
    private float _phaseTransitionTime = .3f;
    [SerializeField]
    private bool _preFight = true;
    [SerializeField]
    private int _leftOrRight;
    [SerializeField]
    private bool _isFiring;

    float sliderStartTime;
    float sliderDuration = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("Boss UI manager is NULL.");
        }

        _bc = GetComponent<BoxCollider2D>();

        if (_bc == null)
        {
            Debug.LogError("Boss Box Collider is NULL.");
        }

        _ec = GetComponent<EdgeCollider2D>();

        if (_ec == null)
        {
            Debug.LogError("Boss Edge Collider is NULL.");
        }

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on Boss is NULL.");
        }
        else
        {
            _audioSource.clip = _explosionSound;
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Boss Animator is NULL.");
        }

        _healthBar = GameObject.Find("BossHP").GetComponent<Slider>();

        if (_healthBar == null)
        {
            Debug.LogError("Boss Health Bar is NULL.");
        }

        _leftOrRight = Random.Range(0, 2);
        _preFight = true;
        _beamCanFire = Time.time + 10.5f + Random.Range(.5f, 2f);
        _wingsCanFire = Time.time + 9.5f + Random.Range(1f, 3f);
        Debug.Log("Boss spawn is at " + Time.time);
        _healthBar.value = 0;
        _healthBar.enabled = true;
        sliderStartTime = Time.time;
}

    // Update is called once per frame
    void Update()
    {
        if (_preFight == true)
        {
            SpawnAnimation();
        }
        else if (_preFight == false)
        {
            Movement();
            Attacks();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }

        }
        else if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }

            TakeDamage();

        }
        else if (other.CompareTag("Missile"))
        {
            HomingMissile _homingMissile = other.GetComponent<HomingMissile>();

            if (_homingMissile != null)
            {
                _homingMissile.EnemyHit();
            }

            if (_player != null)
            {
                _player.AddScore(10);
            }

            TakeDamage();

        }
    }

    void SpawnAnimation()
    {
        _speed = 5;
        float t = (Time.time - sliderStartTime) / sliderDuration;
        _healthBar.value = Mathf.SmoothStep(0, 25, t);

        if (transform.position.z == 10 && transform.position.y < 19)
        {
            transform.Translate(new Vector2(0, 1) * _speed * Time.deltaTime);
        }
        else if (transform.position.z == 10 && transform.position.y >= 13)
        {
            transform.eulerAngles = Vector3.forward * 180;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        if (transform.position.z == 0 && transform.position.y > 6)
        {
            transform.Translate(new Vector3(0, 1, 0) * _speed * Time.deltaTime);
        }
        else if (transform.position.z == 0 && transform.position.y <= 6)
        { 
            _ec.enabled = true;
            _preFight = false;
            _phaseOne = true;
            _speed = 3;
            Debug.Log("Boss is active at " + Time.time);
        }

    }

    IEnumerator PhaseTransitionCooldown(int whichPhase)
    {
        yield return new WaitForSeconds(_phaseTransitionTime);
        _phaseTransition = false;
        switch (whichPhase)
        {
            case 0:
                _phaseTwo = true;
                _healthBar.maxValue = 15;
                _healthBar.value = 15;
                _healthBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.yellow;
                _ec.enabled = true;
                break;
            case 1:
                _phaseThree = true;
                _speed = 4;
                _healthBar.maxValue = 10;
                _healthBar.value = 10;
                _healthBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.red;
                _bc.enabled = true;
                break;
            case 2:
                _phaseFinal = true;
                _speed = 5;
                _healthBar.maxValue = 5;
                _healthBar.value = 5;
                _healthBar.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.magenta;
                _bc.enabled = true;
                break;
        }

    }

    void PhaseTwoTransition()
    {
        _ec.enabled = false;
        GameObject beamExplosion = Instantiate(_explosion, transform.position + new Vector3(0, -.55f, 0), Quaternion.identity);
        beamExplosion.transform.localScale = new Vector2(.50f, .50f);
        _p1Sprite.SetActive(false);
        _phaseOne = false;
        _phaseTransition = true;
        _primaryLeftCanFire = Time.time + _phaseTransitionTime + Random.Range(2f, 4f);
        _primaryRightCanFire = Time.time + _phaseTransitionTime + Random.Range(2f, 4f);
        StartCoroutine(PhaseTransitionCooldown(0));
    }

    void PhaseThreeTransition()
    {
        _ec.enabled = false;
        Instantiate(_silentExplosion, transform.position + new Vector3(1.7f, -.5f, 0), Quaternion.identity);
        Instantiate(_silentExplosion, transform.position + new Vector3(-1.7f, -.5f, 0), Quaternion.identity);
        _audioSource.PlayOneShot(_explosionSound, .5f);
        _p2Sprite.SetActive(false);
        _phaseTwo = false;
        _phaseTransition = true;
        StartCoroutine(PhaseTransitionCooldown(1));
    }

    void PhaseFinalTransition()
    {
        _bc.enabled = false;
        GameObject finalPhaseExplosion = Instantiate(_explosion, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        finalPhaseExplosion.transform.localScale = new Vector2(.85f, .85f);
        _p3Sprite.SetActive(false);
        _phaseThree = false;
        _phaseTransition = true;
        StartCoroutine(PhaseTransitionCooldown(2));
    }

    void Movement()
    {
        if (_phaseOne == true || _phaseTwo == true)
        {
            _movementStyle = 0;
        }
        else if (_phaseThree == true || _phaseFinal == true)
        {
            _movementStyle = 1;
        }
        else if (_phaseTransition == true || _finalDeath == true)
        {
            _movementStyle = 2;
        }

        switch (_movementStyle)
        {
            case 0:
                ZigZag();
                break;
            case 1:
                YModulatedZigZag();
                break;
            case 2:
                SimpleMovement();
                break;
        }
    }

    void SimpleMovement()
    {
        if (_leftOrRight == 0)
        {
            transform.Translate(new Vector2(-1, 0) * _speed * Time.deltaTime);
        }
        else if (_leftOrRight == 1)
        {
            transform.Translate(new Vector2(1, 0) * _speed * Time.deltaTime);
        }

        if (transform.position.x >= 13)
        {
            transform.position = new Vector2(13, transform.position.y);
            _leftOrRight = 1;
        }
        else if (transform.position.x <= -13)
        {
            transform.position = new Vector2(-13, transform.position.y);
            _leftOrRight = 0;
        }
    }

    void ZigZag()
    {
        if (_leftOrRight == 0)
        {
            transform.Translate(new Vector2(-1, 0) * _speed * Time.deltaTime);
        }
        else if (_leftOrRight == 1)
        {
            transform.Translate(new Vector2(1, 0) * _speed * Time.deltaTime);
        }


        if (transform.position.x >= 3f && transform.position.x <= 3.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x <= -3f && transform.position.x >= -3.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x >= 5f && transform.position.x <= 5.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x <= -5f && transform.position.x >= -5.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x >= 9f && transform.position.x <= 9.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x <= -9f && transform.position.x >= -9.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x >= 13)
        {
            transform.position = new Vector2(13, transform.position.y);
            _leftOrRight = 1;
        }
        else if (transform.position.x <= -13)
        {
            transform.position = new Vector2(-13, transform.position.y);
            _leftOrRight = 0;
        }
    }

    void YModulatedZigZag()
    {
        int YMod = 3;

        if (_leftOrRight == 0)
        {
            transform.Translate(new Vector2(-1, Mathf.Cos(Time.time * YMod) * 2) * _speed * Time.deltaTime);
        }
        else if (_leftOrRight == 1)
        {
            transform.Translate(new Vector2(1, Mathf.Cos(Time.time * YMod) * 2) * _speed * Time.deltaTime);
        }


        if (transform.position.x >= 3f && transform.position.x <= 3.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x <= -3f && transform.position.x >= -3.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x >= 5f && transform.position.x <= 5.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x <= -5f && transform.position.x >= -5.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x >= 9f && transform.position.x <= 9.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x <= -9f && transform.position.x >= -9.1f)
        {
            _leftOrRight = Random.Range(0, 2);
        }
        else if (transform.position.x >= 14)
        {
            transform.position = new Vector2(14, transform.position.y);
            _leftOrRight = 1;
        }
        else if (transform.position.x <= -14)
        {
            transform.position = new Vector2(-14, transform.position.y);
            _leftOrRight = 0;
        }

        if (transform.position.y >= 7.7f)
        {
            transform.position = new Vector2(transform.position.x, 7.7f);
        }
        else if (transform.position.y <= 0f)
        {
            transform.position = new Vector2(transform.position.x, 0f);
        }
    }

    //Left Main Laser instantiate -.725, -.45
    //Right Main Laser instantiate .75, -.45

    void Attacks()
    {
        if (_phaseTransition == true)
        {
            _isFiring = false;
            _anim.ResetTrigger("IsFiring");
        }
        else if (_phaseOne == true)
        {
            if (Time.time > _beamCanFire)
            {
                _beamCanFire = Time.time + Random.Range(4f, 7f);
                _isFiring = true;
                _anim.SetTrigger("IsFiring");
            }
            if (Time.time > _wingsCanFire)
            {
                _wingsCanFire = Time.time + Random.Range(0.5f, 2f);
                _audioSource.PlayOneShot(_laserShot, .25f);
                GameObject wingLasers = Instantiate(_wingLasers, transform.position + new Vector3(0, -1.20f, 0), Quaternion.identity);
                Laser[] lasers = wingLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignWingLasers();
                }
            }
        }
        else if (_phaseTwo == true)
        {
            if (Time.time > _wingsCanFire)
            {
                _wingsCanFire = Time.time + Random.Range(3f, 5f);
                _audioSource.PlayOneShot(_laserShot, .15f);
                GameObject wingLasers = Instantiate(_wingLasers, transform.position + new Vector3(0, -1.20f, 0), Quaternion.identity);
                Laser[] lasers = wingLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignWingLasers();
                }
            }
            if (Time.time > _primaryLeftCanFire)
            {
                _primaryLeftCanFire = Time.time + Random.Range(.5f, 2f);
                _audioSource.PlayOneShot(_laserShot, .25f);
                GameObject leftLasers = Instantiate(_primaryLeft, transform.position + new Vector3(-.725f, -.45f, 0), Quaternion.identity);
                Laser[] lasers = leftLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignPrimaryLasers();
                }
            }
            if (Time.time > _primaryRightCanFire)
            {
                _primaryRightCanFire = Time.time + Random.Range(.5f, 2f);
                _audioSource.PlayOneShot(_laserShot, .25f);
                GameObject rightLasers = Instantiate(_primaryRight, transform.position + new Vector3(.75f, -.45f, 0), Quaternion.identity);
                Laser[] lasers = rightLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignPrimaryLasers();
                }
            }
        }
        else if (_phaseThree == true)
        {
            if (Time.time > _primaryLeftCanFire)
            {
                _primaryLeftCanFire = Time.time + Random.Range(.3f, 2f);
                _audioSource.PlayOneShot(_laserShot, .25f);
                GameObject leftLasers = Instantiate(_primaryLeft, transform.position + new Vector3(-.725f, -.45f, 0), Quaternion.identity);
                Laser[] lasers = leftLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignPrimaryLasers();
                }
            }
            if (Time.time > _primaryRightCanFire)
            {
                _primaryRightCanFire = Time.time + Random.Range(.3f, 2f);
                _audioSource.PlayOneShot(_laserShot, .25f);
                GameObject rightLasers = Instantiate(_primaryRight, transform.position + new Vector3(.75f, -.45f, 0), Quaternion.identity);
                Laser[] lasers = rightLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignPrimaryLasers();
                }
            }
        }
        else if (_phaseFinal == true)
        {
            if (Time.time > _primaryLeftCanFire)
            {
                _primaryLeftCanFire = Time.time + Random.Range(.15f, 1f);
                _audioSource.PlayOneShot(_laserShot, .25f);
                GameObject leftLasers = Instantiate(_primaryLeft, transform.position + new Vector3(-.725f, -.45f, 0), Quaternion.identity);
                Laser[] lasers = leftLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignPrimaryLasers();
                }
            }
            if (Time.time > _primaryRightCanFire)
            {
                _primaryRightCanFire = Time.time + Random.Range(.15f, 1f);
                _audioSource.PlayOneShot(_laserShot, .25f);
                GameObject rightLasers = Instantiate(_primaryRight, transform.position + new Vector3(.75f, -.45f, 0), Quaternion.identity);
                Laser[] lasers = rightLasers.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignPrimaryLasers();
                }
            }
        }
        else if (_finalDeath == true)
        {

        }
    }

    public void TakeDamage()
    {
        _healthBar.value--;

        if (_phaseOne == true && _healthBar.value <= 0)
        {
           PhaseTwoTransition();
        }
        else if (_phaseTwo == true && _healthBar.value <= 0)
        {
            PhaseThreeTransition();
        }
        else if (_phaseThree == true && _healthBar.value <= 0)
        {
            PhaseFinalTransition();
        }
        else if (_phaseFinal == true && _healthBar.value <= 0)
        {
            StartCoroutine(BossDeath());
        }
    }

    void BeamWeapon()
    {
        _audioSource.PlayOneShot(_beamWeapon, .5f);
    }

    void DoneFiring()
    {
        _anim.ResetTrigger("IsFiring");
    }

    IEnumerator BossDeath()
    {
        _bc.enabled = false;
        _phaseFinal = false;
        _finalDeath = true;
        _speed = 0;
        GameObject deathExplosion1 = Instantiate(_explosion, transform.position + new Vector3(.31f, -.57f, 0), Quaternion.identity);
        deathExplosion1.transform.localScale = new Vector2(.25f, .25f);
        yield return new WaitForSeconds(.9f);
        GameObject deathExplosion2 = Instantiate(_explosion, transform.position + new Vector3(-.52f, -.11f, 0), Quaternion.identity);
        deathExplosion2.transform.localScale = new Vector2(.3f, .3f);
        yield return new WaitForSeconds(.7f);
        GameObject deathExplosion3 = Instantiate(_explosion, transform.position + new Vector3(.17f, .66f, 0), Quaternion.identity);
        deathExplosion3.transform.localScale = new Vector2(.20f, .20f);
        yield return new WaitForSeconds(1f);
        GameObject deathExplosionFinal = Instantiate(_explosion, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        deathExplosionFinal.transform.localScale = new Vector2(1.3f, 1.3f);
        yield return new WaitForSeconds(.5f);
        _uiManager.EndlessModeStart();

        Destroy(this.gameObject);
    }
}