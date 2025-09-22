using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform camera;

    [SerializeField]
    private float movementSpeed = 2.5f;
    [SerializeField]
    private float rotationSpeed = 10.0f;

    private CharacterController _characterController;

    private Vector2 _playerMovement; //Grab raw movement inputs
    
    private Vector3 _playerDirection;
    private Vector3 _movementDirection; 
    private Vector3 _targetRotationDirection;

    private float _minimumMovementDistance = 0.1f;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public void Move(Vector2 movement)
    {
        _playerMovement.x = movement.x;
        _playerMovement.y = movement.y;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        _playerDirection = new Vector3(_playerMovement.x, 0f, _playerMovement.y).normalized;

        if(_playerDirection.magnitude >= _minimumMovementDistance)
        {
            //Player Movement
            float targetAngle = Mathf.Atan2(_playerDirection.x, _playerDirection.y) * Mathf.Rad2Deg + camera.eulerAngles.y;
            _movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _characterController.Move(_movementDirection.normalized * movementSpeed * Time.deltaTime);

            //Player Rotation
            _targetRotationDirection = camera.forward * _playerDirection.z;
            _targetRotationDirection = _targetRotationDirection + camera.right * _playerDirection.x;
            _targetRotationDirection.Normalize();
            _targetRotationDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(_targetRotationDirection);
            Quaternion playerRotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            this.transform.rotation = playerRotation;
        }
    }
}
