using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10;
    private bool _targetFound;
    private Animator _anim;
    private BoxCollider2D _bc;
    Enemy _enemyTarget;


    private void Start()
    {
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Missile animator is NULL.");
        }

        _bc = GetComponent<BoxCollider2D>();

        if (_bc == null)
        {
            Debug.LogError("Missile animator is NULL.");
        }
    }

    private void Update()
    {
        if (_enemyTarget == null)
        {
            FindEnemyTarget();
        }
        else
        {
            HomingMissileMovement();
        }

        if( _targetFound == false)
        {
            transform.position += Vector3.up * _speed * Time.deltaTime;
        }

        if (transform.position.y > 10f)
        {
            Destroy(this.gameObject);
        }
    }

    public void FindEnemyTarget()
    {
        float _closestEnemy = Mathf.Infinity;
        Enemy[] _enemyList = FindObjectsOfType<Enemy>();

        if (_enemyList != null)
        {
            _targetFound = true;
            
            foreach (Enemy enemy in _enemyList)
            {
                float _distance = (enemy.transform.position - transform.position).sqrMagnitude;

                if (_distance < _closestEnemy)
                {
                    _closestEnemy = _distance;
                    _enemyTarget = enemy;
                }
            }
        }
        else
        {
            Debug.Log("Enemy List is NULL.");
        }
        
        if (_enemyList.Length == 0)
        {
            _targetFound = false;
        }

    }

    private void HomingMissileMovement()
    {
        Vector3 direction = transform.position - _enemyTarget.transform.position;
        direction = -direction.normalized;
        transform.rotation = Quaternion.LookRotation(transform.forward, direction);
        transform.position += direction * _speed * Time.deltaTime;
    }
    public void EnemyHit()
    {
            _anim.SetTrigger("EnemyIsHit");
            _bc.enabled = false;
            Destroy(gameObject, .08f);
    }
}
