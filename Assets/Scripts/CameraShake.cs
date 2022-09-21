using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform _cameraTransform;
    [SerializeField]
    private float _shakeDuration = 0.45f;
    private float _originalShakeDuration;
    [SerializeField]
    private float _shakeAmount = 0.25f;
    private float _decreaseFactor = 1f;
    private bool _shakeTrue = false;
    Vector3 _originalPos;

    // Start is called before the first frame update
    void Start()
    {
        if (_cameraTransform == null)
        {
            _cameraTransform = GetComponent<Transform>();
        }

        _originalPos = _cameraTransform.position;
        _originalShakeDuration = _shakeDuration;
    }


    // Update is called once per frame
    void Update()
    {
        if (_shakeTrue == true)
        {
            if (_shakeDuration > 0)
            {
                _cameraTransform.position = _originalPos + Random.insideUnitSphere * _shakeAmount;
                _shakeDuration -= Time.deltaTime * _decreaseFactor;
            }
            else
            {
                _shakeTrue = false;
                _shakeDuration = _originalShakeDuration;
                _cameraTransform.position = _originalPos;
            }            
        }
    }

    public void ShakeCamera()
    {
        _shakeTrue = true;
    }
}
