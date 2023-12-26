using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected Transform _player;
    [SerializeField] private bool _hasPrompt;
    [SerializeField, ConditionalField(nameof(_hasPrompt))] protected GameObject _prompt;
    [SerializeField, ConditionalField(nameof(_hasPrompt))] private float _promptRange;

    protected float _playerDist;
    protected bool _active;

    protected virtual void Start()
    {
        _player = PlayerStats.i.transform;
    }

    protected virtual void Update()
    {
        _playerDist = Vector3.Distance(transform.position, _player.position);

        if (!_hasPrompt) return;

        if (_playerDist < _promptRange && !_active) _prompt.SetActive(true);
        if (_playerDist > _promptRange && !_active) _prompt.SetActive(false);
        if (_prompt.activeInHierarchy && Input.GetKeyDown(KeyCode.E)) Activate();
    }

    protected virtual void Activate()
    {
        _prompt.SetActive(false);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (!_hasPrompt) return;
        Gizmos.DrawWireSphere(transform.position, _promptRange);
    }

    protected virtual void OnDisable()
    {
        if (_hasPrompt) _prompt.SetActive(false);
    }
}
