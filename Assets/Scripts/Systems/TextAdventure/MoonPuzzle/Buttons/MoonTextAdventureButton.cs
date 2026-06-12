using UnityEngine;

/// <summary>
/// Manages the moon puzzle text adventure UI's choice buttons 
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to a choice button on the moon puzzle text adventure UI.
/// The button is to act as one out of three buttons that'll be a choice to a moon puzzle's question
/// 
/// This script is to be attached to the moon puzzle text adventure UI game object as well as having "MoonPuzzleDialogue" attached to the same game object
/// to directly access public methods and variables 
/// 
/// See <see cref="MoonPuzzleDialogueText"/> - calling public methods and variables from "MoonPuzzleDialogueText"
/// and listening to "SetMoonPuzzleQuestions" event "MoonPuzzleDialogueText" publishes when moon puzzle question's dialogue has all been typed out
/// to determine new question dialogue of the current moon puzzle
/// 
/// See <see cref="MoonPuzzleDialogueData"/> for how each individual moon puzzle QUESTION branch is set up. 
/// 
/// </remarks>

public class MoonTextAdventureButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private GameObject cluebookUI;

    [SerializeField]
    private MoonPuzzleDialogueText moonPuzzleText;

    [SerializeField]
    [Range(1, 3)]
    private int buttonNumber; 

    private MoonPuzzleDialogueData _nextQuestionDialogue;
    private MoonPuzzleDialogueData _currentQuestionDialogue;

    private MoonPuzzleDialogueData _secondQuestionDialogue;
    private MoonPuzzleDialogueData _thirdQuestionDialogue;
    private MoonPuzzleDialogueData _finishedTextAdventureDialogue;

    private bool _allowInput = false; //Prevent or allow for the player to click on this button
    private bool _concludeMoonPuzzle;
    
    private int _branchIndex;

    void Awake()
    {
        EventBus.Instance.Subscribe<SetMoonPuzzleQuestions>(SetNextQuestionDialogue);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.Unsubscribe<SetMoonPuzzleQuestions>(SetNextQuestionDialogue);
    }

    //Receives a "SetMoonPuzzleQuestions" event with parameters:
    //(MoonPuzzleDialogueData) QuestionDialogue - current question dialogue for the moon puzzle text adventure
    //(int) TextBranch - current moon puzzle text branch index (first moon puzzle, second moon puzzle, third moon puzzle)
    private void SetNextQuestionDialogue(SetMoonPuzzleQuestions setMoonPuzzleQuestions) //Published by "MoonPuzzleDialogueText"
    {
        _allowInput = true;

        _branchIndex = setMoonPuzzleQuestions.TextBranchIndex;
        _currentQuestionDialogue = setMoonPuzzleQuestions.QuestionDialogue;

        _secondQuestionDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].SecondQuestionDialogue;
        _thirdQuestionDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].ThirdQuestionDialogue;
        _finishedTextAdventureDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].FinishTextAdventureDialogue;
        
        //Setting new dialogue for moon puzzle text adventure based on "_branchIndex"
        if(buttonNumber == _currentQuestionDialogue.correctButtonNumber)  
        {
            //If current question dialogue is the first question dialogue of the text branch
            if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].FirstQuestionDialogue) 
                _nextQuestionDialogue = _secondQuestionDialogue;

            //If current question dialogue is the second question dialogue of the text branch
            else if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].SecondQuestionDialogue)
                _nextQuestionDialogue = _thirdQuestionDialogue;

            //If current question dialogue is the third question dialogue of the text branch
            else if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].ThirdQuestionDialogue)
            {
                _nextQuestionDialogue = _finishedTextAdventureDialogue;
                _concludeMoonPuzzle = true;
            }
        }
    }

    public void OnDialogueButtonClick() 
    {
        if(_allowInput == false || cluebookUI.activeSelf == true || _currentQuestionDialogue == null)
            return; 

        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);

        if (buttonNumber == _currentQuestionDialogue.correctButtonNumber)
        {
            if (_nextQuestionDialogue == null)
                return;
                
            if(_concludeMoonPuzzle == true)
            {
                moonPuzzleText.FinishTextAdventure();
                _concludeMoonPuzzle = false; //Reset
            }
            
            moonPuzzleText.PromptDialogueFromButton(_nextQuestionDialogue);
        }

        else 
        {
            _allowInput = false;
            moonPuzzleText.DisableButtonOptions(); 
        }
    }
}