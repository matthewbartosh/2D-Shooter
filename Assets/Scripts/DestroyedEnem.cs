using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedEnem : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.5f;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _explosionSound;
    private Animator _anim;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on enemy is NULL.");
        }
        else
        {
            _audioSource.clip = _explosionSound;
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL.");
        }

        _speed = 1.5f;
        _audioSource.Play();
        Destroy(this.gameObject, 2.35f);
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, -1, 0) * _speed * Time.deltaTime);
    }
}