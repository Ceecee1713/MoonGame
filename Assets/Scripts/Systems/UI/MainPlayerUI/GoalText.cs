using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private string generalGoal;

    private string goal = "Collect Clues and Explore to Obtain the ";
    private string objectiveOne = "First Moon Fragment";
    private string objectiveTwo = "Second Moon Fragment";
    private string objectiveThree = "Last Moon Fragment";

    private int counter = 0;

    private const int MAX_NUMBER_OF_AREA_CHANGES = 2; 

    void Start()
    {
        EventBus.Instance.Subscribe<ShowBeginnerGoal>(SetBeginningGoal);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(SetNewGoalText);
        EventBus.Instance.Subscribe<NewExplorationPhase>(ResetGoalText);

        generalGoal = goal + objectiveOne;
        goalTextObject.SetActive(false);
    }
    
    void Update()
    {
        Mathf.Clamp(counter, 0, MAX_NUMBER_OF_AREA_CHANGES);

        if(explorationTimer.RemainingTime < explorationTimer.TimerValueToChangeToRetreatGoal)
            goalText.text = retreatGoal;

        else
            goalText.text = generalGoal;
    }

    private void SetBeginningGoal(ShowBeginnerGoal showBeginnerGoal)
    {
        goalTextObject.SetActive(true);
    }

    private void SetNewGoalText(NewMoonFragmentObtained newMoonFragmentObtained)
    {
        counter++;

        if(counter == 1)
            generalGoal = goal + objectiveTwo;

        if(counter == 2)
            generalGoal = goal + objectiveThree;

        goalText.text = generalGoal;
    }

    private void ResetGoalText(NewExplorationPhase newExplorationPhase)
    {
        goalText.text = generalGoal;
    }
}
