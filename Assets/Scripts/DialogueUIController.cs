using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private TextMeshProUGUI _speakerName;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private GameObject _speakerNameParent;
    [SerializeField] private GameObject _playerNameParent;
    [SerializeField] private List<TextMeshProUGUI> _buttons = new List<TextMeshProUGUI>();

    private IConversationSource _currentSource;
    private int _currentOptionCount;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _currentOptionCount > 0) MakeSelection(0); 
        if (Input.GetKeyDown(KeyCode.Alpha2) && _currentOptionCount > 1) MakeSelection(1); 
        if (Input.GetKeyDown(KeyCode.Alpha3) && _currentOptionCount > 2) MakeSelection(2); 
    }

    public void StartConversation(IConversationSource source, string speaker, bool player)
    {
        _currentSource = source;
        SetSpeaker(speaker, player);
        _mainText.text = "";
        gameObject.SetActive(true);
        HideAllChoices();
        _speakerNameParent.SetActive(true);
    }

    private void SetSpeaker(string speaker, bool player)
    {
        _speakerNameParent.SetActive(false);
        _playerNameParent.SetActive(false);
        _speakerName.text = speaker;
        _playerName.text = speaker;
        if (player) _playerNameParent.SetActive(true);
        else _speakerNameParent.SetActive(true);
    }

    public void DisplayLine(string line, string speaker, bool player)
    {
        _mainText.text = line;
        SetSpeaker(speaker, player);
        _mainText.gameObject.SetActive(true);
    }

    public void DisplayChoices(List<string> options)
    {
        HideAllChoices();
        for (int i = 0; i < options.Count; i++) {
            _buttons[i].text = options[i];
            _buttons[i].transform.parent.gameObject.SetActive(true);
        }
        _currentOptionCount = options.Count;
        _speakerNameParent.SetActive(false);
        _mainText.gameObject.SetActive(false);
    }

    private void HideAllChoices()
    {
        foreach (var b in _buttons) b.transform.parent.gameObject.SetActive(false);
        _currentOptionCount = 0;
    }

    public void EndConversation()
    {
        _currentSource = null;
        gameObject.SetActive(false);
    }

    public void MakeSelection(int index)
    {
        _speakerNameParent.SetActive(true);
        _currentSource.MakeChoice(index);
        HideAllChoices();
    }
}
