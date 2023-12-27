using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager i;

    [SerializeField] private GameObject _photoUI;

    private void Awake() { i = this; }

    private void Update()
    {
        _photoUI.SetActive(Input.GetKey(KeyCode.LeftShift));
    }
}
