using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Public or private reference
// Data type (int, float, bool, string)
// Every variable has a name
// Optional value assigned

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;



    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
     }

    // CalculateMovement handles Player movement
    void CalculateMovement()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(HorizontalInput, VerticalInput, 0);
        
        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }
}