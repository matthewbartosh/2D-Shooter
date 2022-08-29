using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10;
    private bool _isEnemyLaser = false;
    private bool _isBackwardsEnemy = false;
    private bool _isProEnemy = false;

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser == false || _isBackwardsEnemy == true)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    private void MoveUp()
    {
        transform.Translate(new Vector3(0, 1, 0) * _speed * Time.deltaTime);

        if (transform.position.y > 10f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    void MoveDown()
    {
        transform.Translate(new Vector3(0, -1, 0) * _speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void AssignBackwardsLaser()
    {
        _isBackwardsEnemy = true;
    }

    public void AssignProLaser()
    {
        _isProEnemy = true;
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();

                Destroy(this.gameObject);
            }
        } 
        else if (other.CompareTag("Powerup") && _isProEnemy == true)
        {
            PowerUp powerup = other.GetComponent<PowerUp>();

            if (powerup != null)
            {
                powerup.ProShot();

                Destroy(this.gameObject);
            }
        }
    }
}
