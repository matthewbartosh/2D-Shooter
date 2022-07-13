using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         transform.Translate(new Vector3(0, -1, 0) * _speed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            transform.position = new Vector3(Random.Range(-9.44f, 9.46f), 8f, 0);
        }    
    }

   
    // OnTriggerEnter is called when this gameobject collides with another
    private void OnTriggerEnter2D(Collider2D other)
    {
         if(other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }


        
    }

}