using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Note
{
    [HideInInspector] public string Name;
    public float Time;
    public int StringID;
    [HideInInspector] public bool Spawned;

    public Note(float time, int stringID)
    {
        Time = time;
        StringID = stringID;
        Name = time + ": " + stringID;
    }
}
