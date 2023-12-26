using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _speedSmoothness = 0.1f;
    [SerializeField] private float _angularSpeed;
    [SerializeField] private float _angularSmoothness = 0.1f;

    private Rigidbody _rb;
    private float _deltaRot;
    private float _deltaMove;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Rotate();
        Move();
    }

    private void Rotate()
    {
        float amount = 0;
        if (Input.GetKey(KeyCode.A)) amount = -1;
        if (Input.GetKey(KeyCode.D)) amount = 1;

        float smoothness = Mathf.Abs(amount) < Mathf.Abs(_deltaRot) ? 0.1f : _angularSmoothness;

        _deltaRot = Mathf.Lerp(_deltaRot, amount, smoothness);
        _rb.angularVelocity = new Vector3(0, _deltaRot * _angularSpeed, 0);
    }

    private void Move()
    {
        float amount = 0;
        if (Input.GetKey(KeyCode.W)) amount = 1;
        if (Input.GetKey(KeyCode.S)) amount = -1;
        _deltaMove = Mathf.Lerp(_deltaMove, amount, _speedSmoothness);

        var velocity = transform.forward * _deltaMove * _speed;
        velocity.y = 0;
        _rb.velocity = velocity;

    }
}
