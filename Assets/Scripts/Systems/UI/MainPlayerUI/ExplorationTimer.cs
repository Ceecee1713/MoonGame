using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the exploration timer that's displayed on the main player UI 
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to a textmeshproUGUI that'll be acting as the display for timer text
/// This script is to be on the same game object that "ExplorationTimer" is on since they'll both be on the main player UI.
/// This script directly references "ExplorationTimer"
/// 
/// See <see cref="CorriosonValues"/> for how the different speed values that can be given to the corrioson zones to drop player's health
/// 
/// This script works together with scripts: "CorriosonZone" 
/// See <see cref="CorriosonZone"/> - Changing the speed of how fast corrioson zones drop player's health
/// 
/// </remarks>

public class ExplorationTimer : MonoBehaviour
{
    [Header ("Time Values")]
    public float RemainingTime; 
    public float TimerValueToChangeToRetreatGoal; 

    [SerializeField]
    private CorriosonValues corriosonValues;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private float maxDurationOfExplorationPhaseInSeconds; 

    private bool _doNotAllowTimerToCountDown = false; 

    private int _minutes = 0;
    private int _seconds = 0;

    private const int FIRST_MOON_PUZZLE_AREA_NUMBER = 1;
    private const int SECOND_MOON_PUZZLE_AREA_NUMBER = 2;
    private const int THIRD_MOON_PUZZLE_AREA_NUMBER = 3;

    void Start()
    {
        RemainingTime = maxDurationOfExplorationPhaseInSeconds;

        EventBus.Instance.Subscribe<PauseExplorationTimer>(PauseTimerCountdown);
        EventBus.Instance.Subscribe<ResetExplorationTimer>(ResetTimer);
    }

    void Update()
    {
        if(_doNotAllowTimerToCountDown == true)
            return;

        if(RemainingTime == 0)
        {
            //All publish to "CorriosonZone"
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, FIRST_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, SECOND_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, THIRD_MOON_PUZZLE_AREA_NUMBER));

            _doNotAllowTimerToCountDown = true;
            return;
        }

        if(RemainingTime > 0)
            RemainingTime -= Time.deltaTime;

        else if (RemainingTime < 0)
            RemainingTime = 0;

        _minutes = Mathf.FloorToInt(RemainingTime / 60);
        _seconds = Mathf.FloorToInt(RemainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
    }

    private void PauseTimerCountdown(PauseExplorationTimer pauseExplorationTimer) 
    {
        _doNotAllowTimerToCountDown = pauseExplorationTimer.AllowCountdown;
    }

    private void ResetTimer(ResetExplorationTimer resetExplorationTimer)
    {
        RemainingTime = maxDurationOfExplorationPhaseInSeconds;
        _doNotAllowTimerToCountDown = false;
    }
}
