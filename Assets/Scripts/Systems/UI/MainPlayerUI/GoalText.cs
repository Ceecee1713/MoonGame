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

    [SerializeField]
    private float timerValueToChangeToRetreatGoal; 
    
    void Start()
    {
        
    }

    void Update()
    {
        if(explorationTimer.RemainingTime < timerValueToChangeToRetreatGoal)
            goalText.text = retreatGoal;

        else
            goalText.text = generalGoal;
    }
}
