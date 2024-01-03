using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song", menuName = "Song")]
public class Song : ScriptableObject
{
    [System.Serializable]
    public class NoteData
    {
        [HideInInspector] public string Name;
        public string JsonNote;
        public Sound PlayableNote;
    }

    [Header("Text Files")]
    public TextAsset JsonFile;
    public TextAsset EncodedNotesFile;

    [Header("Notes")]
    public List<NoteData> LowestNotes;
    public List<NoteData> LowNotes;
    public List<NoteData> HighNotes;
    public List<NoteData> HighestNotes;

    [Header("Misc")]
    public Sound SongAudio;

    public Sound GetSoundByIndex(string indexString) {
        var parts = indexString.Split(".");
        int stringIndex = int.Parse(parts[0].Trim());
        int noteIndex = int.Parse(parts[1].Trim());
        var notesList = stringIndex == 0 ? LowestNotes : stringIndex == 1 ? LowNotes : stringIndex == 2 ? HighNotes : HighestNotes;
        var note = notesList[noteIndex].PlayableNote;
        if (!note.instantialized) notesList[noteIndex].PlayableNote = Instantiate(note);
        return note;
    }

    public void EndSong() {
        foreach (var n in LowestNotes) n.PlayableNote.Kill();
        foreach (var n in LowNotes) n.PlayableNote.Kill();
        foreach (var n in HighNotes) n.PlayableNote.Kill();
        foreach (var n in HighestNotes) n.PlayableNote.Kill();
    }
}
