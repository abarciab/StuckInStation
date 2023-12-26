using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace DialogueSystem
{
    public interface IConversationSource
    {
        public void MakeChoice(int choiceIndex);
    }


    [System.Serializable]
    public class Line
    {
        [HideInInspector] public string Name;
        public string Text;
        public string SpeakerName;
        public List<Line> NextChoices = new List<Line>();
        public int BranchID = -1;
        public int ID = -1;
        public bool LastLine;

        public Line(string text)
        {
            Text = text;
        }

        public void OnValidate()
        {
            Name = "";
            if (ID > 0) Name += ID + ": ";
            Name += Text;
            Name += " - " + SpeakerName;
        }
    }

    [System.Serializable]
    public class Conversation
    {
        [SerializeField] private List<Line> _lines = new List<Line>();
        private int _currentIndex;
        public bool ConversationEnded;

        public void OnValidate()
        {
            foreach (var l in _lines) l.OnValidate();
        }

        public Conversation(string[] lines)
        {
            foreach (var l in lines) AddLine(l);
        }

        public void AddLine(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return;
            var parts = raw.Split(":");
            if (parts.Length < 2) return;

            bool lineIsChoice = false;
            var newLine = new Line(parts[1]);

            newLine.SpeakerName = parts[0];

            if (parts.Length == 3) {
                if (parts[2].Trim() == "END") newLine.LastLine = true;
                else newLine.BranchID = int.Parse(parts[2].Trim());
            }

            var tags = parts[0].Split(")");
            if (tags.Length == 2) {
                if (tags[0].ToLower() == "c") lineIsChoice = true;
                else newLine.ID = int.Parse(tags[0].Trim());
                newLine.SpeakerName = tags[1];
            }

            if (lineIsChoice) _lines[_lines.Count - 1].NextChoices.Add(newLine);
            else _lines.Add(newLine);
        }

        public string GetCurrentSpeaker() => _lines[_currentIndex].SpeakerName;

        public string GetCurrentLine()
        {
            ConversationEnded = _lines[_currentIndex].LastLine;
            var selected = _lines[_currentIndex];
            if (!CurrentLineHasChoices()) _currentIndex += 1;
            return selected.Text;
        }

        public bool CurrentLineHasChoices()
        {
            return _lines[_currentIndex].NextChoices.Count > 0;
        }

        public List<string> GetCurrentLineChoices()
        {
            if (!CurrentLineHasChoices()) return null;

            var choices = _lines[_currentIndex].NextChoices;
            var strings = new List<string>();
            foreach (var c in choices) strings.Add(c.Text);

            return strings;
        }

        public void MakeChoice(int choiceIndex)
        {
            if (!CurrentLineHasChoices()) return;

            var chosen = _lines[_currentIndex].NextChoices[choiceIndex];
            if (chosen.LastLine) ConversationEnded = true;
            else NavigateToLineByID(chosen.BranchID);
        }

        private void NavigateToLineByID(int ID)
        {
            for (int i = 0; i < _lines.Count; i++) {
                if (_lines[i].ID == ID) _currentIndex = i; 
            }
        }
    }
}

