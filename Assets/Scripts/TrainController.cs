using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [SerializeField] private GameObject _ramp;
    [SerializeField] private GameObject _door;

    [ButtonMethod]
    private void Arrive()
    {
        GameManager.i.CurrentStation.TrainEnter(this);
        gameObject.SetActive(true);
    }

    [ButtonMethod]
    private void Leave()
    {
        GameManager.i.CurrentStation.TrainLeave();
        gameObject.SetActive(false);
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
