using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PhotographyUIController : MonoBehaviour
{
    [Header("Cutout")]
    [SerializeField, ReadOnly] private Vector2 _cutoutAnchoredMax; 
    [SerializeField, ReadOnly] private Vector2 _cutoutAnchoredMin;
    [SerializeField] private RectTransform _cutout;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSmoothness = 0.1f;
    private Vector2 _moveDelta;
    [SerializeField] private Camera _photoCamera;
    [SerializeField] private Volume _photoVolume;

    [Header("controls")]
    [SerializeField] private KeyCode _zoomOut = KeyCode.Q;
    [SerializeField] private KeyCode _zoomIn = KeyCode.W;
    [SerializeField] private KeyCode _focusOut = KeyCode.Comma;
    [SerializeField] private KeyCode _focusIn = KeyCode.Period;

    [Header("Parameters")]
    [SerializeField] private Vector2 _focusLimits = new Vector2(0.8f, 5.8f);
    [SerializeField] private float _focusSpeed = 5;
    [SerializeField] private Vector2 _zoomLimits = new Vector2(-2, 1.4f);
    [SerializeField] private float _zoomSpeed = 2;
    private float _currentZoom;
    [SerializeField] private float _currentFocus;

    [Header("Picture taking")]
    [SerializeField] private GameObject _pictureFlash;

    [ButtonMethod] private void SetMax() => _cutoutAnchoredMax = _cutout.anchoredPosition;
    [ButtonMethod] private void SetMin() => _cutoutAnchoredMin = _cutout.anchoredPosition;

    private void OnEnable()
    {
        PlayerStats.i.SetModelActive(false);
        _photoVolume.gameObject.SetActive(true);
    }

    private void Update()
    {
        MoveCutout();
        AdjustFocus();
        AdjustZoom();
        if (Input.GetKeyDown(KeyCode.Space)) TakePicture();
    }

    private void TakePicture()
    {
        _pictureFlash.SetActive(true);
    }

    private void AdjustZoom()
    {
        float delta = 0;
        if (Input.GetKey(_zoomIn)) delta = 1;
        if (Input.GetKey(_zoomOut)) delta = -1;

        if (Mathf.Abs(delta) > 0.5f) {
            float z = _photoCamera.transform.localPosition.z;
            z += delta * _zoomSpeed * Time.deltaTime;
            z = Mathf.Clamp(z, _zoomLimits.x, _zoomLimits.y);
            _currentZoom = z;
        }

        var newPos = _photoCamera.transform.localPosition;
        newPos.z = _currentZoom;
        _photoCamera.transform.localPosition = newPos;
    }

    private void AdjustFocus()
    {
        float delta = 0;
        if (Input.GetKey(_focusIn)) delta = 1;
        if (Input.GetKey(_focusOut)) delta = -1;

        var profile = _photoVolume.profile;
        profile.TryGet<DepthOfField>(out var dof);

        if (Mathf.Abs(delta) > 0.5f) {
            float dist = dof.focusDistance.value;
            dist += delta * _focusSpeed * Time.deltaTime;
            dist = Mathf.Clamp(dist, _focusLimits.x, _focusLimits.y);
            _currentFocus = dist;
        }
        dof.focusDistance.Override(_currentFocus);
    }


    private void MoveCutout()
    {
        Vector2 moveDir = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) moveDir.y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) moveDir.y = -1;
        if (Input.GetKey(KeyCode.RightArrow)) moveDir.x = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) moveDir.x = -1;
        _moveDelta = Vector2.Lerp(_moveDelta, moveDir, moveSmoothness);

        var pos = _cutout.anchoredPosition;
        pos += _moveDelta * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, _cutoutAnchoredMin.x, _cutoutAnchoredMax.x);
        pos.y = Mathf.Clamp(pos.y, _cutoutAnchoredMin.y, _cutoutAnchoredMax.y);

        _cutout.anchoredPosition = pos;
    }

    private void OnDisable()
    {
        PlayerStats.i.SetModelActive(true);
        _photoVolume.gameObject.SetActive(false);
    }
}
