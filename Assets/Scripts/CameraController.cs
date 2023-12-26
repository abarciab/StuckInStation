using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Rigidbody _player;
    [SerializeField] private float _moveSmoothness;
    [SerializeField] private float _rotSmoothness = 0.1f;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _lookOffset;

    [Header("Reference")]
    [SerializeField] private float _referenceCopySmoothness = 0.1f;

    private bool _followingPlayer = true;
    private bool _matchingReference;
    [HideInInspector] public bool IsFollowingPlayer => _followingPlayer;

    private Transform _referenceCamera;

    [ButtonMethod]
    private void SnapToPos()
    {
        transform.LookAt(_player.transform.position + _lookOffset);
        transform.position = _player.transform.TransformPoint(_offset);
    }

    private void FixedUpdate()
    {
        if (_followingPlayer) Follow();
        else if (_matchingReference) MatchReference();
    }

    private void MatchReference()
    {
        transform.position = Vector3.Lerp(transform.position, _referenceCamera.position, _referenceCopySmoothness);
        transform.rotation = Quaternion.Lerp(transform.rotation, _referenceCamera.rotation, _referenceCopySmoothness);
    }

    private void Follow()
    {
        var before = transform.rotation;
        transform.LookAt(_player.transform.position + _lookOffset);
        transform.rotation = Quaternion.Lerp(before, transform.rotation, _rotSmoothness);

        var targetPosition = _player.transform.TransformPoint(_offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, _moveSmoothness);
    }

    public void SetReferenceCamera(Transform referenceCam)
    {
        _referenceCamera = referenceCam;
        _followingPlayer = false;
        _matchingReference = true;
    }
    
    public void StartFollowingPlayer()
    {
        _followingPlayer = true;
        _matchingReference = false;
        _referenceCamera = null;
    }
}
