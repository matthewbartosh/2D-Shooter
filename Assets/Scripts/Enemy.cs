using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Common to all
    private Player _player;
    private Animator _anim;
    private BoxCollider2D _bc;
    [SerializeField]
    private GameObject _destroyedEnemy;
    [SerializeField]
    private GameObject _enemyLaser;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSound;    
    [SerializeField]
    private AudioClip _laserShot;
    [SerializeField] // enemy types. 0 = basic enemy, 1 = zigzag + ram, 2 = double-shot + backfiring, 3 = pro enemy (destroys pickups, avoids shots)
    private int _enemyID;
    [SerializeField]
    private float _speed = 4.0f;

    //Basic Enemy
    [SerializeField]
    private bool _basicEnemy = false;
    [SerializeField]
    private float _basicSpeed = 4.0f;
    private float _canFire;

    //Zigzag+Ram Enemy, Shield
    [SerializeField]
    private bool _zigZagEnemy = false;
    [SerializeField]
    private float _zigZagAmount = 5;
    [SerializeField]
    private bool _enemyShield = false;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private float _distance;
    [SerializeField]
    private float _rammingSpeed = 8.0f;

    //Double-shot+Backfiring Enemy ('Smart')
    [SerializeField]
    private bool _doubleShotEnemy = false;
    [SerializeField]
    private GameObject _enemyDoubleLaser;



    //Pro Enemy (Destroy Pickups, avoid fire, backfire), Shield
    private bool _proEnemy = false;

    //Boss Enemy (Only One, WIP)
    private bool _bossEnemy = false;

    //God Enemy (Everything all at once)
    private bool _godEnemy = false;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL.");
        }

        _bc = GetComponent<BoxCollider2D>();

        if (_bc == null)
        {
            Debug.LogError("Box Collider is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on enemy is NULL.");
        }
        else
        {
            _audioSource.clip = _explosionSound;
        }
        
        _canFire = Time.time + Random.Range(2f, 7f);

        switch (_enemyID)
        {
            case 0:
                _basicEnemy = true;
                break;
            case 1:
                _zigZagEnemy = true;
                _enemyShield = true;
                break;
            case 2:
                _doubleShotEnemy = true;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -14.6f, 14.6f), transform.position.y, 0);

        if (_basicEnemy == true)
        {
            transform.Translate(new Vector3(0, -1, 0) * _basicSpeed * Time.deltaTime);

            if (Time.time > _canFire)
            {
                FireLaser();
            }
        }
        else if (_zigZagEnemy == true)
        {
            if (_player != null)
            {
                _distance = Vector3.Distance(_player.transform.position, transform.position);

                if (_distance < 4 && transform.position.y > _player.transform.position.y)
                {
                    Ram();
                }
                else
                {
                    transform.Translate(new Vector3(Mathf.Cos(Time.time * _zigZagAmount) * 2, -1, 0) * _basicSpeed * Time.deltaTime);
                }
            }
            else
            {
                transform.Translate(new Vector3(Mathf.Cos(Time.time * _zigZagAmount) * 2, -1, 0) * _basicSpeed * Time.deltaTime);
            }
        }
        else if (_doubleShotEnemy == true)
        {
            transform.Translate(new Vector3(0, -1, 0) * _basicSpeed * Time.deltaTime);

            if (Time.time > _canFire)
            {
                FireDoubleLaser();
            }
        }

        if (transform.position.y < -11f)
        {
            transform.position = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
        }
    }


    // OnTriggerEnter is called when this gameobject collides with another
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }

            if (_enemyShield == true)
            {
                _enemyShield = false;
                _shieldVisual.SetActive(false);
                ShieldHit();
            }
            else if (_enemyShield == false)
            {
                Instantiate(_destroyedEnemy, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            //StartCoroutine(DeathAnim());
        }
        else if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(10);
            }
            
            if (_enemyShield == true)
            {
                _enemyShield = false;
                _shieldVisual.SetActive(false);
            }
            else if (_enemyShield == false)
            {
                Instantiate(_destroyedEnemy, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            //StartCoroutine(DeathAnim());
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
           
            if (_enemyShield == true)
            {
                _enemyShield = false;
                _shieldVisual.SetActive(false);
            }
            else if (_enemyShield == false)
            {
                Instantiate(_destroyedEnemy, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
            //StartCoroutine(DeathAnim());
        }
    }

    /*
    IEnumerator DeathAnim()
    {
        
        transform.tag = "Destroyed";
        _anim.SetTrigger("OnEnemyDeath");
        _canFire = Time.time + 10f;
        _bc.enabled = false;
        _speed = 1.5f;
        _audioSource.Play();
        if (transform.position.y >= -5.5f)
        {
            yield return new WaitForSeconds(2.35f);
            Destroy(this.gameObject);
        }
        else if (transform.position.y < -5.5f)
        {
            yield return new WaitUntil(() => transform.position.y <= -10.5f);
            Destroy(this.gameObject);
        }
      } 
    */
   
    void FireLaser()
    {
        _canFire = Time.time + Random.Range(1f, 3f);

        GameObject enemyLaser = Instantiate(_enemyLaser, transform.position + new Vector3(0, -.8f, 0), Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }

        _audioSource.PlayOneShot(_laserShot, .35f);
    }

    void FireDoubleLaser()
    {
        _canFire = Time.time + Random.Range(1f, 3f);

        GameObject enemyLaser = Instantiate(_enemyDoubleLaser, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        if (_player != null)
        {
            if (transform.position.y > _player.transform.position.y)
            {

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignEnemyLaser();
                }
            }
            else
            {
                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignEnemyLaser();
                    lasers[i].AssignBackwardsLaser();
                }
            }
        }
        else
        {
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
         
        _audioSource.PlayOneShot(_laserShot, .35f);
    }

    IEnumerator ShieldHit()
    {
        _bc.enabled = false;
        yield return new WaitForSeconds(1);
        _bc.enabled = true;
    }

    void Ram()
    {
        Vector3 direction = transform.position - _player.transform.position;
        direction = -direction.normalized;
        transform.position += direction * _rammingSpeed * Time.deltaTime;
    }
}