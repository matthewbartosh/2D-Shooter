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
    [SerializeField]
    private bool _proEnemy = false;
    [SerializeField]
    private float _proSpeed = 3.0f;
    [SerializeField]
    private GameObject _proLaser;
    private float _proCanFire;
    [SerializeField]
    private Vector2 _rayBoxSize = new Vector2(2.5f, 5.0f);
    [SerializeField]
    private float _proSight = 3.5f;
    RaycastHit _hit;

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

        _canFire = Time.time + Random.Range(5f, 9f);

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
            case 3:
                _proEnemy = true;
                _enemyShield = true;
                _proCanFire = Time.time + Random.Range(2f, 5f);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -14.6f, 14.6f), transform.position.y, 0);

        if (_basicEnemy == true || _doubleShotEnemy == true)
        {
            transform.Translate(_basicSpeed * Time.deltaTime * new Vector3(0, -1, 0));

            if (Time.time > _canFire && _basicEnemy == true)
            {
                FireLaser();
            }
            else if (Time.time > _canFire && _doubleShotEnemy == true)
            {
                FireDoubleLaser();
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
                    transform.Translate(_basicSpeed * Time.deltaTime * new Vector3(Mathf.Cos(Time.time * _zigZagAmount) * 2, -1, 0));
                }
            }
            else
            {
                transform.Translate(_basicSpeed * Time.deltaTime * new Vector3(Mathf.Cos(Time.time * _zigZagAmount) * 2, -1, 0));
            }
        }
        else if (_proEnemy == true)
        {
            transform.Translate(_proSpeed * Time.deltaTime * new Vector3(0, -1, 0));

            if (Time.time > _proCanFire)
            {
                FireLaser();
            }

            RaycastHit2D hit = Physics2D.BoxCast(transform.position, _rayBoxSize, 0, Vector2.down, _proSight, LayerMask.GetMask("PlayerLaser"));

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Laser"))
                {
                    Transform laser = hit.collider.GetComponent<Transform>();
                    if( laser != null)
                    {
                        if (laser.position.x > transform.position.x)
                        {
                            transform.Translate(_proSpeed * Time.deltaTime * new Vector3(-1, -1, 0));
                        }
                        else if (laser.position.x < transform.position.x)
                        {
                            transform.Translate(_proSpeed * Time.deltaTime * new Vector3(1, -1, 0));
                        }
                    }
                }
            }
        }

        if (transform.position.y < -11f && _proEnemy == true)
        {
            transform.position = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
            _enemyShield = true;
            _shieldVisual.SetActive(true);
        }
        else if (transform.position.y < -11f)
        {
            transform.position = new Vector3(Random.Range(-14.55f, 14.55f), 10.8f, 0);
        }
    }

    private void OnDrawGizmos()
    {
            Gizmos.DrawWireCube(new Vector2 (transform.position.x, transform.position.y - _proSight), _rayBoxSize);
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
                StartCoroutine(ShieldHit());
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
        GameObject enemyLaser = Instantiate(_enemyLaser, transform.position + new Vector3(0, -.8f, 0), Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        if (_player != null && _proEnemy == true)
        {
            _proCanFire = Time.time + Random.Range(.5f, 2f);

            if (transform.position.y > _player.transform.position.y)
            {
                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignProLaser();
                }
            }
            else
            {
                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignProLaser();
                    lasers[i].AssignBackwardsLaser();
                }
            }
        }
        else
        {
            _canFire = Time.time + Random.Range(1f, 3f);

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
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
        transform.position += _rammingSpeed * Time.deltaTime * direction;
    }
}