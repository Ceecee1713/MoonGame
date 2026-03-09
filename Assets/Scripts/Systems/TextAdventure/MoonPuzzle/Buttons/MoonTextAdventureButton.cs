using UnityEngine;

public class MoonTextAdventureButton : MonoBehaviour
{
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

    private bool _allowPlayerToInteract = false;
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

    private void SetNextQuestionDialogue(SetMoonPuzzleQuestions setMoonPuzzleQuestions) 
    {
        _allowPlayerToInteract = true;

        _branchIndex = setMoonPuzzleQuestions.TextBranchIndex;
        _currentQuestionDialogue = setMoonPuzzleQuestions.QuestionDialogue;

        _secondQuestionDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].SecondQuestionDialogue;
        _thirdQuestionDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].ThirdQuestionDialogue;
        _finishedTextAdventureDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].FinishTextAdventureDialogue;
        
        //Setting new dialogue for moon puzzle text adventure based on "_branchIndex"
        if(buttonNumber == _currentQuestionDialogue.correctButtonNumber)  
        {
            if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].FirstQuestionDialogue) 
                _nextQuestionDialogue = _secondQuestionDialogue;

            else if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].SecondQuestionDialogue)
                _nextQuestionDialogue = _thirdQuestionDialogue;

            else if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].ThirdQuestionDialogue)
            {
                _nextQuestionDialogue = _finishedTextAdventureDialogue;
                _concludeMoonPuzzle = true;
            }
        }
    }

    public void OnDialogueButtonClick()
    {
        if(_allowPlayerToInteract == false || cluebookUI.activeSelf == true || _currentQuestionDialogue == null)
            return; 

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
            _allowPlayerToInteract = false;
            moonPuzzleText.DisableButtonOptions();
        }
    }
}