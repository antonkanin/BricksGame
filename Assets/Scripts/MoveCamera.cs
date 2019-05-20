using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Vector3 targetPosition;

    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition,
            5.0f * Time.deltaTime);
    }

    public void ResetCamera()
    {
        targetPosition = _initialPosition;
    }
}