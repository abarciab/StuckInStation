using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats i;
    public int Money;

    private void Awake()
    {
        i = this;
    }
}
