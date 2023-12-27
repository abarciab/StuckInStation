using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuskingStation : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject _altCamera;
    [SerializeField] private GameObject _playerModel;
    [SerializeField] private GameObject _rhythymUI;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    [SerializeField] private List<Animator> _buttonAnimators = new List<Animator>();

    [Header("Sounds")]
    [SerializeField] private List<Sound> _sounds = new List<Sound>();
    [SerializeField] private Sound _failSound;

    [Header("Notes")]
    [SerializeField, ReadOnly] private List<Note> _notes = new List<Note>();
    [SerializeField] private float _noteSpeed = 1;
    [SerializeField] private float _floor = -2;
    [SerializeField] private float _ceiling = 310;

    [Header("Current")]
    [SerializeField] private TextAsset _currentSongNotes;
    [SerializeField] private Sound currentSong;

    [Header("Keys")]
    [SerializeField] private KeyCode _key1 = KeyCode.A;
    [SerializeField] private KeyCode _key2 = KeyCode.D;
    [SerializeField] private KeyCode _key3 = KeyCode.J;
    [SerializeField] private KeyCode _key4 = KeyCode.L;

    [Header("Misc")]
    [SerializeField] private float _extraWaitTime = 4;
    private float _waitTimeRemaing;

    [Header("CameraMovement")]
    [SerializeField] private List<Transform> _camPositions = new List<Transform>();
    [SerializeField] private float _camMoveTime = 20;
    private int currentCamPos;

    private CameraController _mainCamera;
    private float _runTime;
    private List<Transform> _spawnedNotes = new List<Transform>();

    protected override void Start()
    {
        base.Start();

        _mainCamera = FindObjectOfType<CameraController>();
        for (int i = 0; i < _sounds.Count; i++) _sounds[i] = Instantiate(_sounds[i]);
        currentSong = Instantiate(currentSong);
        _failSound = Instantiate(_failSound);

        ParseNotesFile();
    }

    [ButtonMethod]

    private void ParseNotesFile()
    {
        var lines = _currentSongNotes.text.Split("\n");
        _notes = new List<Note>();
        foreach (var line in lines) {
            var data = line.Split(",");
            float time = float.Parse(data[0]);
            for (int i = 1; i < data.Length; i++) {
                _notes.Add(new Note(time, int.Parse(data[i])));
            }
        }
        _notes = _notes.OrderBy(x => x.Time).ToList();
    }

    protected override void Update()
    {
        base.Update();
        if (!_active) return;
        if (Input.GetKeyUp(KeyCode.Escape)) Exit();

        _runTime += Time.deltaTime;
        if (_spawnedNotes.Count == 0 && _notes[_notes.Count - 1].Spawned) 
        {
            _waitTimeRemaing -= Time.deltaTime;
            if (_waitTimeRemaing <= 0) {
                Exit();
                return;
            }
        }

        SpawnNewNotes();
        MoveNotes();
        HandleInput();
    }

    private void MoveNotes()
    {
        for (int i = _spawnedNotes.Count-1; i >= 0 ; i--) {
            if (_spawnedNotes[i] != null && _spawnedNotes[i].gameObject.name.Contains("TEST")) {
                _spawnedNotes[i].localPosition += Vector3.down * _noteSpeed * Time.deltaTime;
                if (_spawnedNotes[i].localPosition.y < _floor + 11) {
                    currentSong.Play(); 
                    Destroy(_spawnedNotes[i].gameObject);
                    _spawnedNotes.RemoveAt(i);
                    print("time: " + _runTime);
                }
                continue;
            }


            if (_spawnedNotes[i] == null) _spawnedNotes.RemoveAt(i);
            //else if (_spawnedNotes[i].localPosition.y < _floor + 11) AutoPlayNote(i);
            else if (_spawnedNotes[i].localPosition.y < _floor) MissNote(i);
            else _spawnedNotes[i].localPosition += Vector3.down * _noteSpeed * Time.deltaTime;
        }
    }

    private void AutoPlayNote(int index)
    {
        int noteIndex = 0;
        var spawnedNote = _spawnedNotes[index];
        if (spawnedNote.parent == _spawnPoints[1]) noteIndex = 1;
        if (spawnedNote.parent == _spawnPoints[2]) noteIndex = 2;
        if (spawnedNote.parent == _spawnPoints[3]) noteIndex = 3;
        _sounds[noteIndex].PlayOneShot();

        Destroy(spawnedNote.gameObject);
    }

    private void MissNote(int index)
    {
        Fail();
        Destroy(_spawnedNotes[index].gameObject);
    }

    private void SpawnNewNotes()
    {
        foreach (var n in _notes) {
            if (!n.Spawned && n.Time < _runTime) SpawnNewNote(n);
            else if (n.Time > _runTime) break;
        }
    }

    private void SpawnNewNote(Note note)
    {
        var newNote = Instantiate(_notePrefab, _spawnPoints[note.StringID]);
        newNote.transform.localPosition = Vector3.zero;
        _spawnedNotes.Add(newNote.transform);
        note.Spawned = true;
    }

    private void HandleInput()
    {
        var index = -1;
        if (Input.GetKeyDown(_key1)) index = 0;
        if (Input.GetKeyDown(_key2)) index = 1;
        if (Input.GetKeyDown(_key3)) index = 2;
        if (Input.GetKeyDown(_key4)) index = 3;
        if (index == -1) return;

        _buttonAnimators[index].SetTrigger("throb");
        bool foundNote = CheckForNotesOnString(index);
        if (foundNote) _sounds[index].PlayOneShot();
        else Fail();
    }

    private void Fail()
    {
        _failSound.Play();
        FindObjectOfType<CameraShake>().ShakeFixed();
    }

    private bool CheckForNotesOnString(int index)
    {
        var parent = _spawnPoints[index];
        var foundNotes = _spawnedNotes.Where(x => x.parent == parent && x.localPosition.y < _ceiling).ToList();
        if (foundNotes.Count == 0) return false;
        foundNotes = foundNotes.OrderBy(x => x.localPosition.y).ToList();
        Destroy(foundNotes[0].gameObject);
        return true;
    }

    private void Exit()
    {
        _active = false;
        SetActiveState(false);
        ResetSong();
    }

    protected override void Activate()
    {
        base.Activate();
        _active = true;
        SetActiveState(true);
        FindObjectOfType<MusicPlayer>().FadeOutMusic(0.5f);
        ResetSong();
        StopAllCoroutines();
        StartCoroutine(CameraMove(0));
    }

    private IEnumerator CameraMove(int targetIndex)
    {
        Vector3 startPos = _altCamera.transform.position;
        float timePassed = 0;
        while (timePassed < _camMoveTime) {
            float progress = timePassed / _camMoveTime;
            _altCamera.transform.position = Vector3.Lerp(startPos, _camPositions[targetIndex].position, progress);
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(CameraMove(targetIndex + 1 < _camPositions.Count ? targetIndex + 1 : 0));
    }

    private void ResetSong()
    {
        ParseNotesFile();
        foreach (var n in _notes) n.Spawned = false;
        _runTime = 0;
        _waitTimeRemaing = _extraWaitTime;
        foreach (var s in _spawnedNotes) Destroy(s.gameObject);
        _spawnedNotes.Clear();
        SendTestNote();
    }

    private void SendTestNote()
    {
        SpawnNewNote(new Note(0, 0));
        _spawnedNotes[0].gameObject.name = "TEST";
    }

    private void SetActiveState(bool starting)
    {
        if (starting) {
            _mainCamera.SetReferenceCamera(_altCamera.transform);
        }
        else {
            _mainCamera.StartFollowingPlayer();
            FindObjectOfType<MusicPlayer>().FadeIn();
        }

        _player.gameObject.SetActive(!starting);
        _playerModel.SetActive(starting);
        _rhythymUI.SetActive(starting);
    }
}
