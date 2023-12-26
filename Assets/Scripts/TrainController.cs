using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [SerializeField] private GameObject _ramp;
    [SerializeField] private GameObject _door;
    [SerializeField] private bool _inStation;

    private void Update()
    {
        if (_inStation) GameManager.i.CurrentTrain = this;
        else if (GameManager.i.CurrentTrain == this) GameManager.i.CurrentTrain = null;
    }

    public void OpenDoor()
    {
        SetDoorState(false);
    }

    public void CloseDoor()
    {
        SetDoorState(true);
    }

    private void SetDoorState(bool state)
    {
        _door.SetActive(state);
        _ramp.SetActive(!state);
    }
}
