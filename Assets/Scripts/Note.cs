using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Note
{
    [HideInInspector] public string Name;
    public float Time;
    [HideInInspector] public bool Spawned;
    public string NoteCode;

    public Note(float time, string noteCode)
    {
        Time = time;
        Name = time + ": " + noteCode;
        NoteCode = noteCode;
    }

    public int StringID => Mathf.FloorToInt(float.Parse(NoteCode.Split(".")[0]));
}
