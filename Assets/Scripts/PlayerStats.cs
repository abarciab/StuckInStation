using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats i;
    public int Money;
    [SerializeField] private GameObject _modelParent;

    private void Awake()
    {
        i = this;
    }

    public void SetModelActive(bool state) => _modelParent.SetActive(state);
}
