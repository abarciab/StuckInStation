using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private TicketWindow _ticketWindow;
    [SerializeField] List<DoorController> MainDoors = new List<DoorController>();
    bool _readyToOpenDoors;

    public void OpenDoor()
    {
        if (GameManager.i.CurrentTrain) GameManager.i.CurrentTrain.OpenDoor();
        else _readyToOpenDoors = true;
    }

    public void TrainEnter(TrainController train)
    {
        foreach (var d in MainDoors) d.Open();
        GameManager.i.CurrentTrain = train;
        _ticketWindow.InformOfTrain(true);
        if (_readyToOpenDoors) train.OpenDoor();
    }

    public void TrainLeave()
    {
        foreach (var d in MainDoors) d.Close();
        GameManager.i.CurrentTrain = null;
        _ticketWindow.InformOfTrain(false);
    }
}
