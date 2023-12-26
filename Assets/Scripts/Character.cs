using DialogueSystem;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Interactable, IConversationSource
{
    [SerializeField] private TextAsset _conversation;
    [SerializeField, ReadOnly] private Conversation parsedConversation;
    DialogueUIController _dialogueController;
    private bool _shouldDisplayChoices;

    protected override void Start()
    {
        base.Start();
        ParseConversation();
        _dialogueController = FindObjectOfType<DialogueUIController>(true);
    }

    protected override void Update()
    {
        base.Update();
        if (!_active) return;
        if (Input.GetKeyDown(KeyCode.E)) DisplayNext();
    }

    private void DisplayNext()
    {
        if (parsedConversation.ConversationEnded) EndConversation();
        else if (_shouldDisplayChoices) DisplayChoices();
        else DisplayNextLine();
    }

    private void EndConversation()
    {
        _dialogueController.EndConversation();
        enabled = false;
    }

    private void DisplayChoices()
    {
        _dialogueController.DisplayChoices(parsedConversation.GetCurrentLineChoices());
    }

    private void DisplayNextLine()
    {
        if (parsedConversation.CurrentLineHasChoices()) _shouldDisplayChoices = true;
        string currentSpeaker = parsedConversation.GetCurrentSpeaker();
        _dialogueController.DisplayLine(parsedConversation.GetCurrentLine(), currentSpeaker, currentSpeaker == "PLAYER");
    }

    public void MakeChoice(int choice)
    {
        parsedConversation.MakeChoice(choice);
        _shouldDisplayChoices = false;
        DisplayNext();
    }

    [ButtonMethod]
    private void ParseConversation()
    {
        var lines = _conversation.text.Split("\n");
        parsedConversation = new Conversation(lines);
        parsedConversation.OnValidate();
    }

    protected override void Activate()
    {
        base.Activate();
        _active = true;
        string currentSpeaker = parsedConversation.GetCurrentSpeaker();

        _dialogueController.StartConversation(this, currentSpeaker, currentSpeaker == "PLAYER");
    }
}
