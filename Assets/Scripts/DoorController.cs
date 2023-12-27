using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DoorState
{
    public Vector3 Pos;
    public Quaternion Rot;

    public DoorState(Vector3 pos, Quaternion rot)
    {
        Pos = pos;
        Rot = rot;
    }
}

public class DoorController : MonoBehaviour
{
    [SerializeField, ReadOnly] private DoorState _openState;
    [SerializeField, ReadOnly] private DoorState _closeState;

    [Header("Animation")]
    [SerializeField] private float _animateTime = 1.2f;
    [SerializeField] private AnimationCurve _curve;

    [ButtonMethod]
    private void SetClosed()
    {
        _closeState.Pos = transform.localPosition;
        _closeState.Rot = transform.localRotation;
    }

    [ButtonMethod]
    private void SetOpen()
    {
        _openState.Pos = transform.localPosition;
        _openState.Rot = transform.localRotation;
    }

    [ButtonMethod]
    public void Open()
    {
        if (!Application.isPlaying) SnapToState(_openState);
        else StartCoroutine(AnimateToState(_openState));
    }

    [ButtonMethod]
    public void Close()
    {
        if (!Application.isPlaying) SnapToState(_closeState);
        else StartCoroutine(AnimateToState(_closeState));
    }

    private void SnapToState(DoorState state)
    {
        transform.localPosition = state.Pos; 
        transform.localRotation = state.Rot;
    }

    private IEnumerator AnimateToState(DoorState target)
    {
        DoorState startState = new DoorState(transform.localPosition, transform.localRotation);

        float timePassed = 0;
        while (timePassed <  _animateTime) {
            float progress = timePassed / _animateTime;
            progress = _curve.Evaluate(progress);

            transform.localPosition = Vector3.Lerp(startState.Pos, target.Pos, progress);
            transform.localRotation = Quaternion.Lerp(startState.Rot, target.Rot, progress);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        SnapToState(target);
    }
}
