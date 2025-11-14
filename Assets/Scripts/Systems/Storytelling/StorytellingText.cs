using System.Collections;
using UnityEngine;
using TMPro;

public class StorytellingText : MonoBehaviour
{
    [Header ("Main Dialogues")]
    [SerializeField]
    private StorytellingDialogueData startingGameDialogue;
    [SerializeField]
    private StorytellingDialogueData finishedGameDialogue;
    [SerializeField]
    private StorytellingDialogueData prayerPhaseDialogue;

    [Header ("Prayer Dialogues for Prayer Phase")]
    [SerializeField]
    private StorytellingDialogueData badPrayerDialogue;
    [SerializeField]
    private StorytellingDialogueData goodPrayerDialogue;
    [SerializeField]
    private StorytellingDialogueData noPrayerDialogue;

    [Header ("UI Information")]
    [SerializeField]
    private GameObject storytellingUI;
    [SerializeField]
    private GameObject mainPlayerUI;
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    private StorytellingDialogueData _currentDialogue;

    private int _randomPrayerNumber = 0;
    private int _messageLength;
    private int _index = 0; //Index to go through the dialogue message array (individual messages) from "dialogueData" 
    private int _currentLineCount = 0;

    private bool _doNotRepeat = false;
    private bool _beginPrayerPhase = false;
    private bool _finishPrayerPhase = false;
    private bool _finishGame = false;
    private bool _finishedTypingMessage = false; //Prevent or allow going through individual messages when they're not fully typed out

    private const int MAX_LINES = 3; 
    private const float TYPING_SPEED = 0.015f;

    void Start()
    {
        EventBus.Instance.Subscribe<StartPrayerPhase>(StartPrayerPhaseAdventure);
        EventBus.Instance.Subscribe<StartEndGameDialogue>(StartEndGameDialogueAdventure);
        EventBus.Instance.Subscribe<AdvanceTextAdventure>(NextDialogue);

        ResetValues();
        storytellingUI.SetActive(false);
    }

    void OnEnable()
    {
        _doNotRepeat = false;
    }

    void OnDisable()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        dialogueText.text = "";

        _index = 0; 
        _currentLineCount = 0;
        _beginPrayerPhase = false;
        _finishPrayerPhase = false;

        StopAllCoroutines();
    }

    private void StartEndGameDialogueAdventure(StartEndGameDialogue startEndGameDialogue) 
    {
        _finishGame = true;
        _currentDialogue = finishedGameDialogue;
        _messageLength = _currentDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    private void StartPrayerPhaseAdventure(StartPrayerPhase startPrayerPhase) 
    {
        _beginPrayerPhase = true;
        _currentDialogue = prayerPhaseDialogue;
        _messageLength = _currentDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    private void NextDialogue(AdvanceTextAdventure advanceTextAdventure) //Called by "PlayerInputController" (keybind Enter/left mouse click)
    {
        if(_finishedTypingMessage != true || _doNotRepeat == true)
            return;

        if(_index+1 == _messageLength && _beginPrayerPhase == true)
        {
            PickPrayer();
            return;
        }

        /*
        if(_index+1 == _messageLength && _finishGame == true)
        {
           //Show end screen
        }
        */

        if(_index+1 == _messageLength && _finishPrayerPhase == true)
        {
            Debug.Log("Can't proceed further.");
            _doNotRepeat = true;
            EventBus.Instance.Publish(new FreezePlayer(false));
            EventBus.Instance.Publish(new ChangeCanvases(mainPlayerUI, false, false));
            return;
        }

        if(_index+1 == _messageLength)
            return;

        _index++;
        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    private void PickPrayer() 
    {
        _index = 0; //Reset

        _randomPrayerNumber = Random.Range(1, 3);

        if(_randomPrayerNumber == 1)
        {
            _currentDialogue = badPrayerDialogue;
            //Add effect (pass event) to make corrison accumalate faster
        }
            
        else if(_randomPrayerNumber == 2)
        {
            _currentDialogue = goodPrayerDialogue;
            //Add effect (pass event) to make corrison accumalate slower
        }
            

        else if(_randomPrayerNumber == 3)
            _currentDialogue = noPrayerDialogue;

        _beginPrayerPhase = false;
        _finishPrayerPhase = true;
        _messageLength = _currentDialogue.Messages.Length; 

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    IEnumerator TypeMessage(string message) 
    {
        _finishedTypingMessage = false;
    
        if (_currentLineCount >= MAX_LINES) //Clear text when we've reached max lines
        {
            dialogueText.text = "";
            _currentLineCount = 0;
        }
        
        if (_currentLineCount > 0) //Adding empty lines to mimick a paragraph look
        {
            dialogueText.text += "\n";
            dialogueText.text += "\n";
        }
        
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(TYPING_SPEED);
        }
        
        _currentLineCount++; 
        _finishedTypingMessage = true;
    }
}
