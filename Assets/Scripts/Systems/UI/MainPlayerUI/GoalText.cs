using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalText : MonoBehaviour
{
    [SerializeField]
    private ExplorationTimer explorationTimer;
    
    [SerializeField]
    private string generalGoal;

    [SerializeField]
    private string retreatGoal;

    [SerializeField]
    private TextMeshProUGUI goalText;
    
    void Update()
    {
        if(explorationTimer.RemainingTime < explorationTimer.TimerValueToChangeToRetreatGoal)
            goalText.text = retreatGoal;

        else
            goalText.text = generalGoal;
    }
}
