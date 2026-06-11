using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the goal text that's displayed on the main player UI 
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to a textmeshproUGUI that'll be acting as the display for goal text
/// This script is to be on the same game object that "ExplorationTimer" is on since they'll both be on the main player UI.
/// This script directly references "ExplorationTimer"
/// 
/// This script works together with scripts: "DialogueCanvas", "MoonPuzzleDialogueText" , "StorytellingDialogueData" 
/// See <see cref="DialogueCanvas"/> - Listening to "ShowBeginnerGoal" event that "DialogueCanvas" publishes
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "NewMoonFragmentObtained" event that "MoonPuzzleDialogueText" publishes 
/// to set a new general goal and reset to general goal, and listening to "NewExplorationPhase" event that "MoonPuzzleDialogueText" publishes to reset to general goal
/// 
/// See <see cref="StorytellingDialogueData"/> - Listening to "NewExplorationPhase" event that "StorytellingDialogueData" publishes to reset to general goal
/// 
/// </remarks>

public class GoalText : MonoBehaviour
{
    [SerializeField]
    private ExplorationTimer explorationTimer;

    [SerializeField]
    private string retreatGoal;

    [Header ("UI Information")]
    [SerializeField]
    private TextMeshProUGUI goalText;
    [SerializeField]
    private GameObject goalTextObject;

    private string _generalGoal;

    private string _goal = "Collect Clues and Explore to Obtain the "; //Base goal text prefix
    private string _objectiveOne = "First Moon Fragment"; //Appended to "goal" for needing to complete the first moon puzzle
    private string _objectiveTwo = "Second Moon Fragment"; //Appended to "goal" for needing to complete the second moon puzzle
    private string _objectiveThree = "Last Moon Fragment"; //Appended to "goal" for needing to complete the third moon puzzle

    private int _numberOfAreaChanges = 0;

    private const int MAX_NUMBER_OF_AREA_CHANGES = 2; 

    void Start()
    {
        EventBus.Instance.Subscribe<ShowBeginnerGoal>(SetBeginningGoal);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(SetNewGoalText);
        EventBus.Instance.Subscribe<NewExplorationPhase>(ResetGoalText);

        _generalGoal = _goal + _objectiveOne;
        goalTextObject.SetActive(false);
    }
    
    void Update()
    {
        Mathf.Clamp(_numberOfAreaChanges, 0, MAX_NUMBER_OF_AREA_CHANGES);

        if(explorationTimer.RemainingTime < explorationTimer.TimerValueToChangeToRetreatGoal)
            goalText.text = retreatGoal;

        else
            goalText.text = _generalGoal;
    }

    //"ShowBeginnerGoal" is the name of an event. Empty event
    private void SetBeginningGoal(ShowBeginnerGoal showBeginnerGoal) //Published by "DialogueCanvas"
    {
        goalTextObject.SetActive(true);
    }

    //"NewMoonFragmentObtained" is the name of an event. Empty event
    private void SetNewGoalText(NewMoonFragmentObtained newMoonFragmentObtained) //Published by "MoonPuzzleDialogueText"
    {
        _numberOfAreaChanges++;

        if(_numberOfAreaChanges == 1)
            _generalGoal = _goal + _objectiveTwo;

        if(_numberOfAreaChanges == 2)
            _generalGoal = _goal + _objectiveThree;

        goalText.text = _generalGoal;
    }

    //"NewExplorationPhase" is the name of an event. Empty event
    private void ResetGoalText(NewExplorationPhase newExplorationPhase) //Published by "MoonPuzzleDialogueText" or "StorytellingDialogueData"
    {
        goalText.text = _generalGoal;
    }
}
