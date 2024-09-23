using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Vector2 _framingOffset;

    [SerializeField] private float _distance =5;
    [SerializeField] private float _rotationSpeed = 2;
    [SerializeField] private float _minVerticalAngle = -45f;
    [SerializeField] private float _maxVerticalAngle = 45f;

    private float _rotationX;
    private float _rotationY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        _rotationY += Input.GetAxis("Mouse X") * _rotationSpeed;
        _rotationX += Input.GetAxis("Mouse Y") * _rotationSpeed;
        _rotationX = Mathf.Clamp(_rotationX, _minVerticalAngle, _maxVerticalAngle);

        Quaternion targetRotation = Quaternion.Euler(_rotationX, _rotationY, 0);
        var focusPosition = _followTarget.position + new Vector3(_framingOffset.x, _framingOffset.y);
        transform.position = focusPosition - targetRotation * new Vector3(0, 0, _distance);
        transform.rotation = targetRotation;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, _rotationY, 0);
}
