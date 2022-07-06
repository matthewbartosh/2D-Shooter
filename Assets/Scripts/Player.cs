using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public or private reference
    // Data type (int, float, bool, string)
    // Every variable has a name
    // Optional value assigned
    public float speed = 3.5f;


    // Start is called before the first frame update
    void Start()
    {
        // Take the current positoion = new position (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(1, 0, 0) * 5 * Time.deltaTime);
    }
}