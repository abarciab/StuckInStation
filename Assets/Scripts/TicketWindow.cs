using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketWindow : Interactable
{
    [SerializeField] private int _ticketCost;
    private bool _ticketPurchased;

    public void InformOfTrain(bool trainState)
    {
        if (!_ticketPurchased && trainState) enabled = true;
        else if (_ticketPurchased || !trainState) enabled = false;
    }


    protected override void Activate()
    {
        base.Activate();
        if (PlayerStats.i.Money > _ticketCost) BuyTicket();
    }

    private void BuyTicket()
    {
        _ticketPurchased = true;
        PlayerStats.i.Money -= _ticketCost;
        GameManager.i.CurrentTrain.OpenDoor();
        enabled = false;
    }
}
