using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private TicketWindow _ticketWindow;

    private void Update()
    {
        _ticketWindow.InformOfTrain(GameManager.i.CurrentTrain != null);
    }
}
