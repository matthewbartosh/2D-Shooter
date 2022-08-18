using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] //0 = Triple Shot, 1 = Speed, 2 = Shields, 3 = Ammo, 4 = heal
    private int _powerupID;
    private Player _player;
    private bool _collecting = false;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_collecting == false)
        {
            transform.Translate(new Vector3(0, -1, 0) * _speed * Time.deltaTime);
        }
        else if (_collecting == true)
        {
            Vector3 direction = transform.position - _player.transform.position;
            direction = -direction.normalized;
            transform.position += direction * _speed * Time.deltaTime;
        }

        if(transform.position.y < -9.0f)
        {
            Destroy(this.gameObject);
        }
    }

    // OnTriggerEnter is called when this gameobject collides with another
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AmmoRefill();
                        break;
                    case 4:
                        player.HealPowerup();
                        break;
                    case 5:
                        player.HomingMissiles();
                        break;
                    case 6:
                        player.SpeedDebuff();
                        break;
                    default:
                        Debug.Log("Powerup Error");
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }

    public void CollectButton()
    {
        _collecting = true;
        _speed *= 2;
    }
}