using UnityEngine;

public class TextAdventureButton : MonoBehaviour
{
    [SerializeField]
    private DialogueText moonPuzzleText;

    [SerializeField]
    private int buttonNumber; 

    private DialogueData _nextQuestionDialogue;
    private DialogueData _currentQuestionDialogue;

    private bool _allowPlayerToInteract = false;
    private bool _concludeMoonPuzzle;
    private int _branchIndex;

    void Awake()
    {
        EventBus.Instance.Subscribe<SetTextAdventureQuestion>(SetNextQuestionDialogue);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.Unsubscribe<SetTextAdventureQuestion>(SetNextQuestionDialogue);
    }

    private void SetNextQuestionDialogue(SetTextAdventureQuestion setTextAdventureQuestion) 
    {
        _allowPlayerToInteract = true;
        _branchIndex = setTextAdventureQuestion.TextBranchIndex;
        _currentQuestionDialogue = setTextAdventureQuestion.QuestionDialogue;

        if(buttonNumber == _currentQuestionDialogue.correctButtonNumber) 
        {
            if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].FirstQuestionDialogue) 
                _nextQuestionDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].SecondQuestionDialogue;

            else if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].SecondQuestionDialogue)
                _nextQuestionDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].ThirdQuestionDialogue;

            else if(_currentQuestionDialogue == moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].ThirdQuestionDialogue)
            {
                _nextQuestionDialogue = moonPuzzleText.TextAdventureDialogue.TextBranches[_branchIndex].FinishTextAdventureDialogue;
                _concludeMoonPuzzle = true;
            }
                
        }
    }

    public void OnDialogueButtonClick()
    {
        if(_allowPlayerToInteract == false)
            return; 

        if (_currentQuestionDialogue == null)
        {
            Debug.LogError($"Button {buttonNumber}: No current dialogue set!");
            return;
        }

        if (buttonNumber == _currentQuestionDialogue.correctButtonNumber)
        {
            if (_nextQuestionDialogue == null)
            {
                Debug.LogError($"Button {buttonNumber}: _nextQuestionDialogue is null!");
                return;
            }

            if(_concludeMoonPuzzle == true)
            {
                moonPuzzleText.FinishTextAdventure();
                _concludeMoonPuzzle = false; //Reset
            }
            
            moonPuzzleText.PromptDialogueFromButton(_nextQuestionDialogue);
        }

        else //Wrong button chosen, pass event to dialogueText to show return button
        {
            Debug.Log("Wrong button");
            _allowPlayerToInteract = false;
            moonPuzzleText.DisableButtonOptions();
        }
    }
}