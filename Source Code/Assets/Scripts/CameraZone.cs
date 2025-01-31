using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraZone : MonoBehaviour
{
    #region Fields
    
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    public CinemachineVirtualCamera _currentVCamera;
    
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        _virtualCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _currentVCamera.enabled = true;
            _virtualCamera.enabled = false;
            Debug.Log("2");
        }

        Debug.Log("1");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _currentVCamera.enabled = true;
            _virtualCamera.enabled = false;
        }
    }
}
