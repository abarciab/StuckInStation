using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class JsonMidiConverter : MonoBehaviour
{
    [SerializeField] private Song _currentSong;

    [Space()]
    [SerializeField, ReadOnly, OverrideLabel("Missing notes")]
    private string _displayedMissingNotes;
    [SerializeField, ReadOnly]
    private List<string> _missingNotes = new List<string>();

    [ButtonMethod]
    private void Convert()
    {
        _missingNotes.Clear();
        var lines = _currentSong.JsonFile.text.Split("\n");

        var output = lines.Where(x => (x.Contains("time") || x.Contains("name")) && !(x.Contains("]") || x.Contains("["))).ToList();
        for (int i = 0; i < output.Count; i++) {
            if (output[i].Contains("time")) {
                var decodedTime = ConvertToTime(output[i]);
                output[i - 1] = decodedTime + "," + output[i - 1];
                output[i] = "";
            }
            else if (output[i].Contains("name")) output[i] = ConvertToNote(output[i]);
        }
        output = output.Where(x => !string.IsNullOrEmpty(x)).ToList();
        File.WriteAllText(AssetDatabase.GetAssetPath(_currentSong.EncodedNotesFile), string.Join("\n", output));

        _displayedMissingNotes = string.Join(", ", _missingNotes);
    }

    private string ConvertToTime(string input)
    {
        var parts = input.Split(":");
        parts[1] = parts[1].Replace(",", "");
        float num = float.Parse(parts[1].Trim());
        return num.ToString();
    }

    private string ConvertToNote(string input)
    {
        var parts = input.Split(":");
        string trimmed = parts[1].Replace("\"", "");
        trimmed = trimmed.Replace(",", "");
        trimmed = trimmed.Trim();

        float lowestNote = CheckPresence(_currentSong.LowestNotes, trimmed);
        if (lowestNote > -1) return lowestNote.ToString("0.0");
        float lowNote = CheckPresence(_currentSong.LowNotes, trimmed);
        if (lowNote > -1) return (1 + lowNote).ToString("0.0");
        float highNote = CheckPresence(_currentSong.HighNotes, trimmed);
        if (highNote > -1) return (2 + highNote).ToString("0.0");
        float highestNote = CheckPresence(_currentSong.HighestNotes, trimmed);
        if (highestNote > -1) return (3 + highestNote).ToString("0.0");

        if (!_missingNotes.Contains(trimmed) && !string.IsNullOrEmpty(trimmed)) _missingNotes.Add(trimmed);
        return trimmed;
    }

    private float CheckPresence(List<Song.NoteData> notes, string noteString) {
        for (int i = 0; i < notes.Count; i++) {
            if (string.Equals(notes[i].JsonNote.ToUpper(), noteString.ToUpper())) {
                print(noteString + " is in this list at index: " + i);
                return i / 10f;
            }
        }
        return -1;
    }

}
