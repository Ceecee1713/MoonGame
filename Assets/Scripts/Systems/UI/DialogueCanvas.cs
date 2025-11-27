using System;
using System.Collections;
using UnityEngine;
using TMPro;

//This is for the dialogue canvas for the NPCs to use 

public class DialogueCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    private bool _finishedTypingMessage = false;
    private bool _newExplorationPhase = false;

    private string _dialogue;

    private const float TYPING_SPEED = 0.01f;
    private const float DELAY = 1.5f;

    void Awake()
    {
        EventBus.Instance.Subscribe<TypeOutSingleDialogue>(DisplayMessage);
        EventBus.Instance.Subscribe<AdvanceSingleMessage>(FinishMessage);
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        dialogueText.text = "";
        _finishedTypingMessage = false;
        _newExplorationPhase = false;
    }

    private void FinishMessage(AdvanceSingleMessage advanceSingleMessage) //Called by "PlayerInputController" (keybind Enter/left mouse click)
    {
        if(_finishedTypingMessage != true)
            return;

        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new MaintainPlayerHealth(false));

        if(_newExplorationPhase == true)
            EventBus.Instance.Publish(new ResetExplorationTimer());

        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

    private void DisplayMessage(TypeOutSingleDialogue typeOutSingleDialogue)
    {
        _newExplorationPhase = typeOutSingleDialogue.NewExplorationPhase;
        _dialogue = typeOutSingleDialogue.Message;
        StopAllCoroutines();
        StartCoroutine(TypeMessage(_dialogue));
    }

    IEnumerator TypeMessage(string message) 
    {
        _finishedTypingMessage = false;
        dialogueText.text = ""; //Clearing the "dialogueText".text for the new dialouge to be said
        
        foreach (char letter in message.ToCharArray()) //Conversion of string to a char array to mimick a "typing" effect of dialouge
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(TYPING_SPEED); //Time in between of each character being typed out
        } 

        _finishedTypingMessage = true;
    }
}
