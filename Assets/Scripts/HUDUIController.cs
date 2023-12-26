using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moneyDisplay;
    [SerializeField] private string _moneyTemplateString = "$MONEY";
    private PlayerStats _stats;

    private void Start()
    {
        _stats = PlayerStats.i;
    }

    private void Update()
    {
        _moneyDisplay.text = _moneyTemplateString.Replace("MONEY", _stats.Money.ToString());
    }
}
